using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardGroupAction:ActionBase {
    public List<Dictionary<string, object>> ids;

	//手牌
	public List<CardData> handCards;
	//下一张牌
	public List<CardData> nextCards;
	//待抽牌
	public List<CardData> waitCards;
	//死牌
	public List<CardData> deadCards;
	//使用牌的CD
	public int useCardCD;
	//补手牌CD
	public int fillHandCardCD;
	//补NEXT牌CD
	public int fillNextCardCD;

    public PlayerEntity player;
    

    public CardGroupAction(Map map, int netId = -1):base(map, netId) {

    }

    //初始化
    public void init (PlayerEntity player) {
        this.player = player;
        
		useCardCD = 0;
		fillHandCardCD = 0;
		fillNextCardCD = 0;
		resetCards ();
    }
    
    public void changePower(int value) {
        for(int i = 0, len = ConfigConstant.CARD_HAND_MAX; i < len; i++) {
            CardData card = this.handCards[i];
            if(null != card) card.changePower(value);
        }
    }

    override public void update(){
        this.changePower(this.player.map.powerGain);
        if(useCardCD>0)
			useCardCD -= ConfigConstant.MAP_ACT_TIME_S;
		if (fillHandCardCD > 0) {
			fillHandCardCD -= ConfigConstant.MAP_ACT_TIME_S;
			if (fillHandCardCD <= 0) {
				fillHandCards (false);
			}
		}
		if(fillNextCardCD>0)
		{
			fillNextCardCD -= ConfigConstant.MAP_ACT_TIME_S;
			if (fillNextCardCD <= 0) {
				if (fillNextCard ()) {
					fillHandCardCD = ConfigConstant.CARD_FILL_HAND_CD;
					//					fillHandCards (false);
				}
			}
		}
	}

	//初始牌
	public void resetCards(){
//				int cardsNum = 0;
		handCards = new List<CardData>();
		nextCards = new List<CardData>();
		waitCards = new List<CardData>();
		deadCards = new List<CardData>();
		

		

		for (int i = 0; i < ids.Count; i++) {
			CardData card = new CardData(ids [i], this.player, -1);
			deadCards.Add (card);
		}
		//乱序洗待抽牌
		washWaitCards();
		//补上下一张
		fillNextCard ();
		//抽取所有手牌
		fillHandCards (true);
	}




	//把死牌放到待抽牌，乱序洗待抽牌
	public void washWaitCards(){
		while (deadCards.Count > 0) {
			waitCards.Add (deadCards [0]);
			deadCards.RemoveAt (0);
		}
		randomList (waitCards);
	}
	//打乱List
	void randomList(List<CardData> list)  
	{   
		CardData card;
		for(int i = list.Count-1; i>0; i--)  
		{  
			int index = (int)this.player.map.getRandomRange(0,i+1);  
			if (index == i)
				continue;
			card=list[i];  
			list[i] = list[index];  
			list[index] = card;
		}  
	} 


	//填补next
	public bool fillNextCard(){
		if(nextCards.Count<1 && waitCards.Count>0)
		{
			if (nextCards.Count <=1) {
				nextCards.Add (waitCards [0]);
			} else if (handCards [0] == null) {
				nextCards [0] = waitCards [0];
			}
			waitCards.RemoveAt (0);
			fillNextCardCD = ConfigConstant.CARD_FILL_NEXT_CD;

			//如果wait数量不足，开始从dead洗牌
			if (waitCards.Count <= ConfigConstant.CARD_WAIT_MIN) {
				washWaitCards ();
			}
			showCard (-1);
			return true;
		}
		return false;
	}
	//摸牌（如果没有next，则先填补next；如果有，直接从next取牌）
	public bool fillHandCards(bool immediateFill){
		bool b = false;
		for(int i=0;i<ConfigConstant.CARD_HAND_MAX;i++)
		{
			if (fillHandCard (i, immediateFill)) {
				b = true;
				if (!immediateFill) {
					fillNextCardCD = ConfigConstant.CARD_FILL_NEXT_CD;
					break;
				}
			}
		}
		return b;
	}

	//摸牌
	public bool fillHandCard(int index,bool immediateFill){
		CardData card;
		if (nextCards.Count > 0) {
			card = nextCards [0];
			if (null != card) {
				if (handCards.Count <= index) {
					handCards.Add (card);
				} else if (handCards [index] == null) {
					handCards [index] = card;
				} else {
					return false;
				}	
				nextCards.RemoveAt (0);
				if (immediateFill) {
                    card.resetPower(ConfigConstant.HAND_CARD_RESET_CD_PER[index]);
                    fillNextCard ();
				} else {
                    card.resetPower(0);
					fillHandCardCD = ConfigConstant.CARD_FILL_HAND_CD;
				}
				showCard (index);
				return true;
			}
		}
		return false;
	}

	//使用牌
	public bool expendCard(int index) {
        if(handCards.Count > index) {
            CardData card = handCards[index];
			if (null != card && card.canUse) {
				useCardCD = ConfigConstant.CARD_USE_CD;
				fillHandCardCD = ConfigConstant.CARD_FILL_HAND_CD;

				deadCards.Add (card);
				card.index = -1;
				handCards [index] = null;
				card.cast ();
				return true;
			}
        }
		//当前不能使用 通知视图进行恢复。
		this.dispatchEventWith(EventConstant.CHANGE, index);

		return false;
	}

	//显示牌
	//public void showCards(){
	//	for (int i = -1; i < handCards.Count; i++) {
	//		showCard (i);
	//	}
	//}
	//显示牌
	public void showCard(int index){
		CardData card;
        card = index < 0 ? nextCards[0] : handCards[index];
        card.index = index;
		this.dispatchEventWith(EventConstant.CHANGE, index);
	}

    private object[] getCardsData(List<CardData> cards) {
        return cards.ConvertAll<object>((e) => {
            return null == e ? null : new Dictionary<string, object> { { "id", e.id }, { "level", e.level }, { "index", e.index }, {"power", e.power } };
        }).ToArray();
    }

    private List<CardData> setCardsData(object[] cardDatas) {
        return new List<object>(cardDatas).ConvertAll<CardData>((object data)=> {
            if(null == data) return null;
            Dictionary<string, object> dic = (Dictionary<string, object>)data;
            CardData card = new CardData(dic, this.player, (int)( dic["index"] ));
            card.power = (int)dic["power"];
            return card;
        });
    }

    override public void setData(Dictionary<string, object> data) {
        base.setData(data);
        this.ids = new List<object>((object[])data["ids"]).ConvertAll<Dictionary<string, object>>((e)=> { return (Dictionary<string, object>)e; });
        this.player = (PlayerEntity)this._map.getNetObject((int)(data["player"]));
        this.handCards = this.setCardsData((object[])data["handCards"]);
        this.nextCards = this.setCardsData((object[])data["nextCards"]);
        this.waitCards = this.setCardsData((object[])data["waitCards"]);
        this.deadCards = this.setCardsData((object[])data["deadCards"]);

        this.useCardCD = (int)(data["useCardCD"]);
        this.fillHandCardCD = (int)(data["fillHandCardCD"]);
        this.fillNextCardCD = (int)(data["fillNextCardCD"]);
    }

    override public Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["ids"] = Utils.changeTo3(this.ids);
        data["handCards"] = this.getCardsData(this.handCards);
        data["nextCards"] = this.getCardsData(this.nextCards);
        data["waitCards"] = this.getCardsData(this.waitCards);
        data["deadCards"] = this.getCardsData(this.deadCards);
        data["useCardCD"] = this.useCardCD;
        data["fillHandCardCD"] = this.fillHandCardCD;
        data["fillNextCardCD"] = this.fillNextCardCD;
        data["player"] = this.player.netId;
        return data;
    }

    public override void clear() {
        Utils.clearList(this.handCards);
        Utils.clearList(this.nextCards);
        Utils.clearList(this.waitCards);
        Utils.clearList(this.deadCards);
        base.clear();
    }

    public override int type { get { return ConfigConstant.CARD_GROUP_ACTION; } }
}
