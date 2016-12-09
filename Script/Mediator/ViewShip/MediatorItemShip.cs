using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using FairyGUI;
using DG.Tweening;


public class MediatorItemShip : BaseMediator
{
	private Dictionary<string,object> userCard;
	private ModelUser userModel;
	private Dictionary<string,object> cfg;
	private GameObject thiss;

	private GList list;
	private GList list1;
	private GList list2;
	private ComCard c_card;
	private GTextField L_Label;
	private GTextField l_mycardNum;
	private GButton btn_clear;

//	private GImage gui1;
	//	private GImage gui0;
	private GGroup g_have;

	private bool _isChange = false;
	private ModelCard cardModel;
	private Controller c1;
	private bool isMove = false;
	private int oldred;

	private GGroup cardinfo;
	private GGroup cardGroup;
	private string msg = "";
	private bool canChangeGroup = false;
	private bool cardMove = false;

    private Vector2 v2;
    public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_ITEMSHIP);
		cardModel = ModelManager.inst.cardModel;
		userModel = ModelManager.inst.userModel;
//		this.x = cardModel.v2.x;
		this.y = cardModel.v2.y;
//		Log.debug ("xxx-" + this.x + " yyy-" + this.y);
//		return;
//		view.x = 103;
//		view.y = 40;
//		view.width = ModelManager.inst.gameModel.width;
//		isFirst = true;
		cardModel.SetData ();
		cfg = DataManager.inst.systemSimple;
		userCard = ModelManager.inst.userModel.card;
		oldred = (int)userModel.records [Config.ASSET_REDBAGCOIN] < 0 ? 0 : (int)userModel.records [Config.ASSET_REDBAGCOIN];
		list = this.GetChild ("n28").asList;//Tools.FindChild<L_List> (this.thiss, "L_List");
		list1 = this.GetChild ("n23").asList;//Tools.FindChild<L_List> (this.thiss, "L_ListZu");
		list2 = this.GetChild ("n31").asList;//Tools.FindChild<L_List> (this.thiss, "L_List2");
		L_Label = this.GetChild ("n25").asTextField;
		l_mycardNum = this.GetChild ("n16").asTextField;
		c_card = this.GetChild ("n24") as ComCard;
		btn_clear = this.GetChild ("n26").asButton;
		btn_clear.text = Tools.GetMessageById ("24126");
		btn_clear.onClick.Add (OnClearChangeHandler);
//		gui1 = this.GetChild ("n30").asImage;
//		gui0 = view.GetChild ("n29").asImage;
		this.GetChild ("n25").asTextField.text = Tools.GetMessageById ("24124");
		g_have = this.GetChild ("n27").asGroup;
		c1 = this.GetController ("c1");
		cardinfo = this.GetChild ("n40").asGroup;
		cardGroup = this.GetChild("n47").asGroup;

		c1.onChanged.Add (Tab_Change);
		c1.onChangeTip.Add (Tab_ChangeTip);
		canChangeGroup = userModel.GetUnlcok (Config.UNLOCK_CARDGROUP, this.GetChild("n3").asButton);
		userModel.GetUnlcok (Config.UNLOCK_CARDGROUP, this.GetChild("n4").asButton);
		userModel.GetUnlcok (Config.UNLOCK_CARDGROUP, this.GetChild("n5").asButton);

		Dictionary<string, object> cfg1 = (Dictionary<string, object>)DataManager.inst.systemSimple ["unlock"];
		object[] i = (object[])cfg1 [Config.UNLOCK_CARDGROUP];
		msg = Tools.GetMessageById ("14028", new string[] { i [0].ToString () });

		c1.changeObj.Add (this.GetChild("n3").asButton);
		c1.changeObj.Add (this.GetChild("n4").asButton);
		c1.changeObj.Add (this.GetChild("n5").asButton);

		list1.itemRenderer = List_Render1;
		list.itemRenderer = List_Render;
		list2.itemRenderer = List_Render2;

		l_mycardNum.text = Tools.GetMessageColor (Tools.GetMessageById ("17010", new string[] {
			userCard.Keys.Count + "",
			DataManager.inst.GetCardNumByLv (ModelManager.inst.userModel.effort_lv) + ""
		}), new string[]{ "F8DD2CFF" });
		this.GetChild ("n9").asTextField.text = Tools.GetMessageById("24127");
		this.GetChild ("n14").asTextField.text = Tools.GetMessageById("24128");

		IsChangeType (false);

		this.AddGlobalListener (MainEvent.CARD_CHANGE, OnChangeCallBack);
		this.AddGlobalListener (MainEvent.CARD_LEVELUP, OnlevelUp);
		this.AddGlobalListener (MainEvent.LEVEL_UP_USER, OnUserLevelup);
//		this.AddGlobalListener (MainEvent.JUMP_COINGOLDEXPGET, OnJumpFunction);

		if ((int)userModel.records ["card_group_index"] == 0)
		{
			Tab_Change ();
		}
		else
		{
			c1.selectedIndex = (int)userModel.records ["card_group_index"];
		}
		cardModel.teamIndex = c1.selectedIndex;
		cardModel.UpdataMyCardData ();

		if (GuideManager.inst.Check ("1:0"))//引导开始 升级卡牌
		{
			List_Render1 (0, this.GetChild ("n46"));
			GuideManager.inst.Next ();
			GuideManager.inst.Show (this);
		}
//		else if(GuideManager.inst.Check ("2:0")){
//			GuideManager.inst.Clear ();
//			GuideManager.inst.Show (this);
//		}
		else
		{
			this.RemoveChild (this.GetChild ("n46"));
		}
	}
	//	private void OnJumpFunction(MainEvent e)
	//	{
	//		ComGoldCoinExp.count--;
	//	}
	private void OnClearChangeHandler ()
	{
		MediatorItemShipInfo.changeCID = "";
		MediatorItemShipInfo.CID = "";
		cX = -1;
		cY = -1;
		IsChangeType (false);
		doudoduodu = false;
		list1.numItems = cardModel.getDataMyTeamData (c1.selectedIndex).Count;
		list1.foldInvisibleItems = true;
		list1.ResizeToFit (cardModel.getDataMyTeamData (c1.selectedIndex).Count);
	}
	private void Tab_ChangeTip(EventContext data)
	{
		ViewManager.inst.ShowText (msg);
	}
	private void Tab_Change ()
	{
		int card_group_index = (int)userModel.records ["card_group_index"];
		if (card_group_index != c1.selectedIndex) {
			Debug.Log ("card_group_index " + card_group_index);
			NetHttp.inst.Send (NetBase.HTTP_ALTER_CARD_GROUP_INDEX, "index=" + c1.selectedIndex, OnChangeCardCurrGroup);
		}
		long tim = DateTime.Now.Ticks;

		cardModel.teamIndex = c1.selectedIndex;
		cardModel.UpdataMyCardData ();

		cardMove = true;
		OnClearChangeHandler ();
		cardMove = false;
		this.GetChild ("n3").asButton.GetChild ("title").asTextField.strokeColor = c1.selectedIndex == 0?Tools.GetColor("6aa965"):Tools.GetColor("1a91e5");
		this.GetChild ("n4").asButton.GetChild ("title").asTextField.strokeColor = c1.selectedIndex == 1?Tools.GetColor("6aa965"):Tools.GetColor("1a91e5");
		this.GetChild ("n5").asButton.GetChild ("title").asTextField.strokeColor = c1.selectedIndex == 2?Tools.GetColor("6aa965"):Tools.GetColor("1a91e5");

		Dictionary<string,object> big = (Dictionary<string,object>)cfg ["show_attr"];
		this.GetChild ("n35").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shuxing1");
		this.GetChild ("n36").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shuxing2");
		this.GetChild ("n37").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shuxing3");
		this.GetChild ("n38").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shuxing4");
		this.GetChild ("n39").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shuxing5");

		this.GetChild ("n35").asCom.GetChild ("n1").asCom.data = this.GetChild ("n35").asCom.GetChild ("n1").asCom.width;
		this.GetChild ("n36").asCom.GetChild ("n1").asCom.data = this.GetChild ("n36").asCom.GetChild ("n1").asCom.width;
		this.GetChild ("n37").asCom.GetChild ("n1").asCom.data = this.GetChild ("n37").asCom.GetChild ("n1").asCom.width;
		this.GetChild ("n38").asCom.GetChild ("n1").asCom.data = this.GetChild ("n38").asCom.GetChild ("n1").asCom.width;
		this.GetChild ("n39").asCom.GetChild ("n1").asCom.data = this.GetChild ("n39").asCom.GetChild ("n1").asCom.width;

		this.GetChild ("n35").asCom.GetChild ("n1").asCom.width = (cardModel.GetHPTeam (c1.selectedIndex) / (int)big ["hp_score_max"]) * 94;
		this.GetChild ("n36").asCom.GetChild ("n1").asCom.width = (cardModel.GetAtkTeam (c1.selectedIndex) / (int)big ["atk_score_max"]) * 94;
		this.GetChild ("n37").asCom.GetChild ("n1").asCom.width = (cardModel.GetMoveTeam (c1.selectedIndex) / (int)big ["move_score_max"]) * 94;
		this.GetChild ("n38").asCom.GetChild ("n1").asCom.width = (cardModel.GetSpecTeam (c1.selectedIndex) / (int)big ["special_score_max"]) * 94;
		this.GetChild ("n39").asCom.GetChild ("n1").asCom.width = (cardModel.GetCostTeam (c1.selectedIndex) / (int)big ["cost_score_max"]) * 94;
		for (int i = 35; i < 40; i++)
		{
			GComponent img = this.GetChild ("n" + i).asCom.GetChild ("n1").asCom;
			float wid = img.width;
			if (wid > 94)
				wid = 94;
			float c = (float)img.data;
			img.width = c;
			DOTween.To (() => c, x => c = x, wid, 0.5f).OnUpdate (() =>
			{
				img.width = c;
			});
		}


		list.numItems = cardModel.myCard.Count;
		list.foldInvisibleItems = true;
		list.ResizeToFit (cardModel.myCard.Count);

		if (cardModel.myCard.Count == 0)
			this.GetChild ("n34").visible = false;
		else
			this.GetChild ("n34").visible = true;

		list2.y = list.y + list.height + 30;
//		gui1.y = list2.y - 25;

		list2.numItems = cardModel.noMyCard.Count;
		list2.foldInvisibleItems = true;
		list2.ResizeToFit (cardModel.noMyCard.Count);

		this.GetChild ("n34").height = list2.y - this.GetChild ("n34").y - 15;
		this.height = 640;
//		this.scrollPane.viewHeight -= 30;
		this.height -= 138;
//		this.height += (ModelManager.inst.gameModel.height - 640) / 2;
		this.GetChild ("n45").y = this.GetChild ("n31").y + this.GetChild ("n31").height;
//		this.y = cardModel.v2.y;
//		Log.debug (view.viewHeight + "|" + view.height+"|"+ModelManager.inst.gameModel.height);
	}

	private void OnChangeCardCurrGroup (VoHttp vo)
	{
		userModel.records ["card_group_index"] = c1.selectedIndex;
	}

	private void List_Render2 (int index, GObject ss)
	{
		GComponent go = ss.asCom;
		Dictionary<string,object> _da = (Dictionary<string,object>)(cardModel.noMyCard [index]);
		string id = _da ["id"].ToString ();
		setItemData1 (go, id, _da);
	}
    
	private void List_Render1 (int index, GObject ss)
	{
		ComCard go = ss.asCom.GetChild ("n0") as ComCard;
		go.ClearTime ();
		ss.pivot = new Vector2 (0.5f, 0.5f);
        if(cardMove)
		{
			ss.scale = new Vector2 (1.25f, 1.25f);
			ss.alpha = 0;
			ss.TweenScale (new Vector2 (1f, 1f), 0.1f).SetDelay (index * 0.05f).OnStart(()=>{
				ss.alpha = 1;
			});
            		
		}
		go.onClick.Clear ();
		go.onClick.Add (() =>
		{
			this.onListItemClick1 (index);
            //point = ss.LocalToGlobal(new Vector2(ss.x, ss.y));
            //Debug.LogError(point);
            v2 = Stage.inst.touchPosition;

		});
		string id = (cardModel.getDataMyTeamData (c1.selectedIndex) [(index)]).ToString ();
		go.SetData (id);
		if (doudoduodu)
		{
			isMove = true;
			LeenTwennOne (go);
		}
	}

	private void List_Render (int index, GObject ss)
	{
		ComCard go = ss.asCom.GetChild ("n0") as ComCard;
		go.ClearTime ();
		go.onClick.Clear ();
        ss.pivot = new Vector2(0.5f, 0.5f);
        Dictionary<string,object> card = cardModel.myCard [index] as Dictionary<string,object>;
		if ((int)card ["new"] == 1)
		{
			userModel.Add_Notice (go.asCom, new Vector2 (125, 0));
		}
		else
		{
			userModel.Remove_Notice (go.asCom);
		}
		string id = card ["id"].ToString ();
		go.SetData (id);
		go.onClick.Add (() =>
		{
			this.onListItemClick (index, go, (int)card ["new"]);
            v2 = Stage.inst.touchPosition;
        });
	}

	private void onListItemClick1 (int index)
	{
		if (_isChange == true)
		{
			NetHttp.inst.Send (NetBase.HTTP_ALTER_CARD_GROUP, "cg_x=" + c1.selectedIndex + "|cg_y=" + (index) + "|card_id=" + MediatorItemShipInfo.changeCID, ChangCardHandler);
			cX = c1.selectedIndex;
			cY = (index);
			return;
		}
		string id = (cardModel.getDataMyTeamData (c1.selectedIndex) [(index)]).ToString ();
		MediatorItemShipInfo.CID = id;
		MediatorItemShipInfo.isKu = 1;
		ViewManager.inst.ShowView <MediatorItemShipInfo> ();
	}

	private int cX = -1;
	private int cY = -1;

	private void ChangCardHandler (VoHttp vo)
	{
		doudoduodu = false;
		if ((bool)(vo.data) && cX != -1 && cY != -1)
		{
			((userModel.card_group [cX] as object[]) [cY]) = MediatorItemShipInfo.changeCID;
			cardModel.SetData ();
			c1.selectedIndex = cX;
			Tab_Change ();
		}
		isMove = false;
		MediatorItemShipInfo.changeCID = "";
		MediatorItemShipInfo.CID = "";
		cX = -1;
		cY = -1;
		IsChangeType (false);
	}

	private void onListItemClick (int index, GComponent go, int read)
	{
		
		Dictionary<string,object> card = cardModel.myCard [index] as Dictionary<string,object>;
		string id = card ["id"].ToString ();
		MediatorItemShipInfo.CID = id;
		MediatorItemShipInfo.isKu = 0;
		if (read == 1)
		{
			NetHttp.inst.Send (NetBase.HTTP_READ_CARD, "cid=" + id, (VoHttp vo) =>
			{
				if (vo.data is bool)
				{
					if ((bool)vo.data == true)
					{
						((Dictionary<string,object>)userCard [id]) ["new"] = 0;
						userModel.Remove_Notice (go.asCom);
					}
				}
			});
		}
		ViewManager.inst.ShowView <MediatorItemShipInfo> ();
	}

	private bool doudoduodu = false;

	private void OnChangeCallBack (MainEvent e)
	{
		this.scrollPane.SetPosY (0, false);
		IsChangeType (true);
		c_card.SetData (MediatorItemShipInfo.changeCID);
		doudoduodu = true;
		cardMove = false;
		list1.numItems = cardModel.getDataMyTeamData (c1.selectedIndex).Count;
		list1.ResizeToFit (cardModel.getDataMyTeamData (c1.selectedIndex).Count);
	}
	private void OnUserLevelup(MainEvent e)
	{
		oldred = (int)ModelManager.inst.userModel.records [Config.ASSET_REDBAGCOIN] < 0 ? 0 : (int)ModelManager.inst.userModel.records [Config.ASSET_REDBAGCOIN];
		canChangeGroup = userModel.GetUnlcok (Config.UNLOCK_CARDGROUP, this.GetChild("n3").asButton);
		userModel.GetUnlcok (Config.UNLOCK_CARDGROUP, this.GetChild("n4").asButton);
		userModel.GetUnlcok (Config.UNLOCK_CARDGROUP, this.GetChild("n5").asButton);
//		userModel.GetUnlcok (Config.UNLOCK_CARDGROUP, this.GetChild("n6").asButton);
//		userModel.GetUnlcok (Config.UNLOCK_CARDGROUP, this.GetChild("n7").asButton);
	}
	private void OnlevelUp (MainEvent e)
	{
//		if (GuideManager.inst.Check ("1:3"))
//		{
//			GuideManager.inst.Next ();
//			GuideManager.inst.Show (this);
//		}
		if (this.GetChild ("n46") != null)
		{
			this.RemoveChild (this.GetChild ("n46"));
		}
		if (oldred != (int)userModel.records [Config.ASSET_REDBAGCOIN])
		{ 
            ViewManager.inst.ShowGetRedBagCoin((int)userModel.records[Config.ASSET_REDBAGCOIN]-oldred,v2);
        }
		oldred = (int)userModel.records [Config.ASSET_REDBAGCOIN];
		if (((int)DataManager.inst.redbag ["exp"] * (int)userModel.records [Config.ASSET_REDBAGCOIN] + userModel.exp) >= userModel.GetExpMax (userModel.lv))
		{
            if(userModel.lv < 15) {//目前满级15  有可能会变
                ViewManager.inst.ShowAlert(Tools.GetMessageById("13110"), (int bo) => {
                    if(bo == 1)
                        ViewManager.inst.ShowView<MediatorRedPackage>();
                }, true);
            }
		}
		Tab_Change ();
	}

	public void setItemData (GComponent go, string id, bool vhave = true, bool vlevel = true)
	{
		CardVo cards = DataManager.inst.GetCardVo (id);
		GLoader bg = go.GetChild ("n6").asLoader;
		bg.url = Tools.GetResourceUrl ("Image:bg_kapai" + cards.rarity);

		GLoader icon = go.GetChild ("n7").asLoader;
		icon.url = Tools.GetResourceUrl ("Icon:" + id);

		GTextField l_have = go.GetChild ("n2").asTextField;
		GImage img_have = go.GetChild ("n1").asImage;
		if (!vhave)
		{
			l_have.visible = false;
			img_have.visible = false;
		}

		l_have.text = Convert.ToString (cards.cost / 1000);

		GTextField l_lv = go.GetChild ("n10").asTextField;
		GImage img_lv = go.GetChild ("n9").asImage;
		if (!vlevel)
		{
			l_lv.visible = false;
			img_lv.visible = false;
		}
		GProgressBar pro = go.GetChild ("n8").asProgress;
		GTextField proText = pro.GetChild ("title").asTextField;

		l_lv.text = cards.lv + "";

		Dictionary<string,object> exp_cfg = (Dictionary<string,object>)(cfg ["card_lv_exp"]);
		exp_cfg = (Dictionary<string,object>)exp_cfg [cards.rarity + ""];

		GImage img_up = go.GetChild ("n12").asImage;
		if (cards.lv >= cards.maxLv)
		{
			proText.text = cards.exp + "/" + ((object[])exp_cfg ["lv_max"]) [0].ToString ();
			proText.color = new Color (1, 1, 0);
			img_up.visible = false;
		}
		else
		{
			pro.max = cards.maxExp;
			pro.value = cards.exp > cards.maxExp ? cards.maxExp : cards.exp;
			proText.text = cards.exp + "/" + cards.maxExp;

			if (pro.value < pro.max)
			{
				img_up.visible = false;
			}
			else
			{
				img_up.visible = true;
//				EffectManager.inst.TweenJump (img_up.gameObject, 1f);
			}
		}
	}

	private void setItemData1 (GComponent go, string id, Dictionary<string,object> datad)
	{
//		GImage img_no = go.GetChild ("n15").asImage;
		GTextField l_chengjiu = go.GetChild ("n13").asTextField;

		CardVo cards = DataManager.inst.GetCardVo (id);
		Dictionary<string,object> card_exp = (Dictionary<string,object>)cfg ["card_lv_exp"];
		card_exp = (Dictionary<string,object>)card_exp [cards.rarity + ""];
		Dictionary<string,object> card_pool = ((Dictionary<string,object>)cfg ["award_card_pool"]);
		int my_effort_lv = userModel.effort_lv;
		int card_effort_lv = cards.effort_lv;

//		img_no.visible = false;
		go.GetController ("c1").selectedIndex = 0;
		GButton btn = go.GetChild ("n0").asButton;
		if (card_effort_lv <= my_effort_lv)
		{//满足
			if ((int)(((object[])card_exp ["poke_show"]) [1]) == 1)
			{
				l_chengjiu.text = Tools.GetMessageById ("13053");

				btn.RemoveEventListeners ();
				btn.onClick.Add (() =>
				{
					MediatorItemShipInfo2.CID = id;
//					MediatorItemShipInfo.isKu = 4;
					ViewManager.inst.ShowView<MediatorItemShipInfo2> ();
				});
			}
			else
			{
				go.GetController ("c1").selectedIndex = 1;
//				img_no.visible = true;
			}
		}
		else
		{
			btn.RemoveEventListeners ();
			btn.onClick.Add (() =>
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("24119", new string[] {
					Tools.GetMessageById (cards.name),
					Tools.GetEffortName (cards.effort_lv)
				}));
			});
			if ((int)(((object[])card_exp ["poke_show"]) [0]) == 1)
			{
				l_chengjiu.text = Tools.GetMessageById ("13054", new string[]{ Tools.GetEffortName (card_effort_lv) });
			}
			else
			{
//				img_no.visible = true;
				go.GetController ("c1").selectedIndex = 1;
			}
		}
		GImage img_have = go.GetChild ("n1").asImage;

		GTextField l_have = go.GetChild ("n2").asTextField;
		l_have.text = Convert.ToString (cards.cost / 1000);
		if (cards.cost < 0)
		{
			l_have.text = "?";
		}

//		if (img_no.visible)
//		{
//			img_have.visible = false;
//			l_have.visible = false;
//		}
//		else
//		{
//			img_have.visible = true;
//			l_have.visible = true;
//		}

		GLoader bg = go.GetChild ("n6").asLoader;
		bg.url = Tools.GetResourceUrl ("Icon:bg_kapai" + cards.rarity);

		GLoader icon = go.GetChild ("n7").asLoader;
		icon.url = Tools.GetResourceUrl ("Icon:" + id);
	}

	private void IsChangeType (bool change)
	{
		if (change)
		{
			list.visible = false;
			list1.x = 25;
			c_card.visible = true;
			L_Label.visible = true;
			btn_clear.visible = true;
			list2.visible = false;
//			view.GetChild ("n33").asImage.visible = false;
			this.GetChild ("n34").asImage.visible = false;
//			view.GetChild("n36").asImage.visible = false;
			g_have.visible = false;
			this.GetChild ("n44").asImage.visible = true;
			this.GetChild ("n47").asGroup.visible = false;
//			gui0.visible = false;
//			gui1.visible = false;
			this.scrollPane.touchEffect = false;
			this._isChange = true;
//			cardinfo.visible = false;
		}
		else
		{
			list.visible = true;
			list1.TweenMoveX (123f, 0.3f).SetEase (Ease.OutQuad);
			c_card.visible = false;
			L_Label.visible = false;
			btn_clear.visible = false;
			list2.visible = true;
//			view.GetChild ("n33").asImage.visible = true;
			this.GetChild ("n34").asImage.visible = true;
//			view.GetChild("n36").asImage.visible = true;
			g_have.visible = true;
			this.GetChild ("n44").asImage.visible = false;
			this.GetChild ("n47").asGroup.visible = true;
//			gui0.visible = true;
//			gui1.visible = true;
			this.scrollPane.touchEffect = true;
			this._isChange = false;
			isMove = false;
//			cardinfo.visible = true;
		}
	}

	private void LeenTwennOne (GComponent go)
	{
		if (isMove == false)
		{
			DOTween.Kill (go);
			go.x = 0;
			go.y = 0;
			go.rotation = 0;
			return;
		}
		go.TweenMove (new Vector2 (UnityEngine.Random.value * -3, 0), 0.05f);
		go.TweenRotate (UnityEngine.Random.value * 2, 0.1f).OnComplete (() =>
		{
			LeenTweenTwe (go);
		});			
	}

	private void LeenTweenTwe (GComponent go)
	{				
		go.TweenMove (new Vector2 (UnityEngine.Random.value * 3, 0), 0.05f);
		go.TweenRotate (UnityEngine.Random.value * -2, 0.1f).OnComplete (() =>
		{
			LeenTwennOne (go);
		});
	}

	public override void Clear ()
	{
//		DispatchManager.inst.Unregister (MainEvent.CARD_CHANGE, OnChangeCallBack);
		base.Clear ();
		ComGoldCoinExp.Redcount = 0;
	}
}
