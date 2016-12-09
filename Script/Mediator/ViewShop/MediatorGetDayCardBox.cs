using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using FairyGUI;

public class MediatorGetDayCardBox : BaseMediator {//每日抽将小界面
	private Dictionary<string,object> userdata = ModelManager.inst.userModel.records;
	private Dictionary<string,object> cfg = DataManager.inst.random_award;

	private GTextField l_info;
	private GTextField l_coin;
	private ComCard card1;
	private ComCard card2;
	private ComCard card3;
    public int gold1;
    public int gold2;
	private GButton btn_buy;
	public int pri;
	public override void Init ()
	{
		base.Init ();
		cfg = (Dictionary<string,object>)cfg ["daily_box"];
		this.Create (Config.VIEW_GETDAYCARDBOX, false, Tools.GetMessageById (cfg["name"].ToString()));

		userdata = (Dictionary<string,object>)userdata ["daily_box"];
        gold1 = ModelManager.inst.userModel.gold;
        card1 = this.GetChild ("n27") as ComCard;
		card2 = this.GetChild ("n28") as ComCard;
		card3 = this.GetChild ("n29") as ComCard;
		l_info = this.GetChild ("n17").asTextField;
		l_coin = this.GetChild ("n20").asTextField;

		btn_buy = this.GetChild ("n18").asButton;
		btn_buy.onClick.Add (OnBuyHandler);

		this.GetChild ("n12").asTextField.text = Tools.GetMessageById ("17017");
		this.GetChild ("n30").asTextField.text = Tools.GetMessageById ("17019");
//		GameObjectScaler.Scale(EffectManager.inst.AddPrefab (Tools.GetExploreBoxID (cfg ["picture"].ToString ()), this.GetChild ("n13").asGraph),0.7f);
		EffectManager.inst.AddPrefab (Tools.GetExploreBoxID (cfg ["picture"].ToString ()), this.GetChild ("n13").asGraph).transform.localScale *= 0.65f;
		Dictionary<string,object> cardcfg = DataManager.inst.card;
		object[] arr = userdata ["card_class"] as object[];
		for (int i = 0; i < arr.GetLength (0); i++) {
			object[] array = (object[])arr [i];
			CardVo ca = DataManager.inst.GetCardVo ((string)array [0]);
			switch (i) {
			case 0:
				card1.SetData ((string)array [0], -1, 4);
				card1.SetText (Tools.GetMessageById (((Dictionary<string,object>)cardcfg [(string)array [0]]) ["name"].ToString ()));
				card1.name = (string)array [0];
				break;
			case 1:
				card2.SetData ((string)array [0], -1, 4);
				card2.SetText (Tools.GetMessageById (((Dictionary<string,object>)cardcfg [(string)array [0]]) ["name"].ToString ()));
				card2.name = (string)array [0];
				break;
			case 2:
				card3.SetData ((string)array [0], -1, 4);
				card3.SetText (Tools.GetMessageById (((Dictionary<string,object>)cardcfg [(string)array [0]]) ["name"].ToString ()));
				card3.name = (string)array [0];
				break;
			}
		}
		card1.onClick.Add((EventContext ev)=>{
			MediatorItemShipInfo2.CID = ((ComCard)ev.sender).name;
//			MediatorItemShipInfo2.isKu = 2;
			ViewManager.inst.ShowView <MediatorItemShipInfo2>();
		});
		card2.onClick.Add((EventContext ev)=>{
			MediatorItemShipInfo2.CID = ((ComCard)ev.sender).name;
//			MediatorItemShipInfo2.lv = 10;
//			MediatorItemShipInfo.isKu = 2;
			ViewManager.inst.ShowView <MediatorItemShipInfo2>();
		});
		card3.onClick.Add((EventContext ev)=>{
			MediatorItemShipInfo2.CID = ((ComCard)ev.sender).name;
//			MediatorItemShipInfo.isKu = 2;
			ViewManager.inst.ShowView<MediatorItemShipInfo2> ();
		});


		l_info.text = Tools.GetMessageById("17009",new string[]{((object[])cfg ["num"])[0].ToString()});
//		view.GetChild ("n33").asButton.onClick.Add (() => {
//			ViewManager.inst.CloseView(this);
//		});
		coinUpdata ();
		this.AddGlobalListener (MainEvent.DAY_BOX_FRUSH, OnBoxFrushFunction);
	}
	private void OnBoxFrushFunction(MainEvent e)
	{
		ViewManager.inst.CloseView (this);
	}
	private void coinUpdata()
	{
		object[] price = (object[])cfg ["price"];
		int len = price.GetLength (0) - 3;
		int axb = ((int)price [len + 1]) * (int)userdata ["num"] + ((int)price [len + 2]);
		pri = (int)userdata["num"] > len?axb:(int)price [(int)userdata["num"]];
		l_coin.text = pri + "";
	}
	private void OnBuyHandler()
	{
		if (ModelUser.GetCanBuy (Config.ASSET_GOLD,pri,"17024")) {
			NetHttp.inst.Send (NetBase.HTTP_GET_DAILY_BOX, "", OnGetCard);
		}
	}
	private void OnGetCard(VoHttp vo)
	{
        gold1 = ModelManager.inst.userModel.gold;
		ModelManager.inst.userModel.UpdateData (vo.data);
        gold2 = ModelManager.inst.userModel.gold;
        if(gold1 - pri != gold2)
            ModelManager.inst.userModel.isShowText = true;
        userdata = ModelManager.inst.userModel.records;
		userdata = (Dictionary<string,object>)userdata ["daily_box"];
		coinUpdata ();
		Dictionary<string,object> ddddd = new Dictionary<string, object> ();
		ddddd.Add (Config.ASSET_CARD, (Dictionary<string,object>)(((Dictionary<string,object>)vo.data) ["daily_box_card"]));
		ViewManager.inst.ShowGift (ddddd, Tools.GetExploreBoxID (cfg ["picture"].ToString ()));
//		MediatorGiftShow.data = ddddd;
//		ViewManager.inst.ShowView (new MediatorGiftShow ());
		this.DispatchGlobalEvent(new MainEvent (MainEvent.DAY_BOX_BUY));
	}
	public override void Clear ()
	{
		this.RemoveGlobalListener (MainEvent.DAY_BOX_FRUSH, OnBoxFrushFunction);
		base.Clear ();          
	}
}
