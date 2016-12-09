using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CardData:EventClass {

    public int cost = 1000;


    public int index = -1;	//手牌位置

    public PlayerEntity player;
    
	private Dictionary<string, object> _data;

    public int level = 0;

    public string id;

    public int power = 0;

    public CardData(Dictionary<string, object> data, PlayerEntity player, int index){
        this.player = player;
		this.id = data["id"].ToString();
        this.level = (int)(data["level"]);
        this.index = index;
        //this.id = "C501";
        //this.level = 2;

        //if(!ConfigConstant.combat.ContainsKey(this.id)) this.id = "C610";

        this._data = (Dictionary<string, object>)DataManager.inst.card[this.id];

        if(this._data.ContainsKey("up") && false) {
            this._data = (Dictionary<string, object>)Utils.clone(this._data);
            Dictionary<string, object> up = (Dictionary<string, object>)this._data["up"];
            foreach(string key in up.Keys) {
                Utils.setDictionay(this._data, key, (Dictionary<string, object>)up[key], this.level);
            }
        }
        this.cost = (int)( this._data["cd"] );
    }

    public void changePower(int value) {
        //后端只要抄写this.power += value; 这句话就好。
        bool flag = power < cost;
        this.power += value;
        if(flag != power < cost) this.dispatchEventWith(EventConstant.CHANGE);
    }

    
    public void resetPower(float rate = 1) {
        this.power = (int)( this.cost * rate );
    }

    public bool canUse {
        get {
            return index >= 0 && this.power >= cost && this.player.cardGroup.useCardCD <= 0 && !this.player.hasBuff(ConfigConstant.BUFF_STUN);
        }
    }


    //生效效果
    public void cast() {
        //		this.player.addAction (new SkillAction("C603");
        Dictionary<string, object> data = this._data;
        if(data.ContainsKey("part")) {
			player.partGroup.addPart((Dictionary<string, object>) data["part"]);
        }

        if(data.ContainsKey("magic")) {
            MagicSkillAction action = (MagicSkillAction)new MagicSkillAction(this.player.map).init((Dictionary<string, object>)data["magic"], this.player);
            action.cardId = this.id;
            this.player.addAction(action);
        }
    }

    public override void clear() {
        base.clear();
        this.player = null;
        this._data = null;
    }
}

