using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using FairyGUI;

public class MediatorGetCardBox : BaseMediator {//购买卡片小界面

	private ModelUser userModel;

//	private GTextField l_title;//标题
	private GTextField l_info;//介绍
	private GTextField l_info1;//介绍1
	private GTextField l_coin;
	private GTextField l_goldnum;

	private GButton btn_buy;//购买按钮

	private GGraph img_box;//箱子图片

	private GComponent g_bd0;
	private GComponent g_bd1;
	private GComponent g_bd2;

	private Dictionary<string,object> cfg = DataManager.inst.random_award;
	private Dictionary<string,object> boxCfg;
	public override void Init ()
	{
        userModel = ModelManager.inst.userModel;
		boxCfg = (Dictionary<string,object>)(((Dictionary<string,object>)cfg ["box"])["Lv" + MediatorItemCardGet._index]);
		base.Init ();
		this.Create (Config.VIEW_GETCARDBOX, false, Tools.GetMessageById (boxCfg ["name"].ToString ()));

		Dictionary<string,object> award_cfg = (Dictionary<string,object>)DataManager.inst.award [boxCfg ["award_id"].ToString ()];
//		l_title = view.GetChild ("n1").asTextField;
		l_info = this.GetChild ("n10").asTextField;
		l_info1 = this.GetChild ("n11").asTextField;
		l_coin = this.GetChild ("n14").asTextField;
		l_goldnum = this.GetChild ("n9").asTextField;
		img_box = this.GetChild ("n4").asGraph;

		g_bd0 = this.GetChild ("n19").asCom;
		g_bd1 = this.GetChild ("n18").asCom;
		g_bd2 = this.GetChild ("n17").asCom;

		btn_buy = this.GetChild ("n12").asButton;
		btn_buy.onClick.Add (OnBuyHandler);

//		view.GetChild ("n13").asTextField.text = Tools.GetMessageById ("17016");
		this.GetChild("n2").asTextField.text = Tools.GetMessageById("19016");
		l_info1.text = Tools.GetMessageById("19017");
		EffectManager.inst.AddPrefab (Tools.GetEggName (boxCfg ["picture"].ToString ()), img_box);
//		l_title.text = Tools.GetMessageById("17004",new string[]{boxCfg ["effort_lv"].ToString()});

		float[] card_num = Tools.NumSection ((object[])(((Dictionary<string,object>)award_cfg ["card"]) ["num"]),userModel.effort_lv);
		l_info.text = Tools.GetMessageById ("17001", new string[]{ Math.Floor (card_num [0]).ToString () });

		float[] gold_num = Tools.NumSection((object[])award_cfg ["gold"],userModel.effort_lv);
		l_goldnum.text = Math.Floor(gold_num[0]) + "-" + Math.Floor(gold_num[1]);
		l_coin.text = ((int)boxCfg ["price"]).ToString();

//		view.GetChild ("n21").asButton.onClick.Add (() => {
//			ViewManager.inst.CloseView(this);
//		});
		showCardBDNum ();
    }
    private void OnBuyHandler()
	{
		if (ModelUser.GetCanBuy (Config.ASSET_COIN,((int)boxCfg ["price"]),"17025")) {
			NetHttp.inst.Send (NetBase.HTTP_GET_RANDOM_BOX, "box_id=Lv" + MediatorItemCardGet._index, OnGetCard);
		}
	}
	private void OnGetCard(VoHttp vo)
	{
        int gold = ModelManager.inst.userModel.gold;
        ModelManager.inst.userModel.UpdateData (vo.data);
        Tools.FullCard(( (Dictionary<string, object>)vo.data ), gold);
        ViewManager.inst.ShowGift ((Dictionary<string,object>)(((Dictionary<string,object>)vo.data) ["gifts_dict"]), Tools.GetEggName (boxCfg ["picture"].ToString ()));
        
        

        //		MediatorGiftShow.all_Data = (Dictionary<string,object>)(((Dictionary<string,object>)vo.data) ["gifts_dict"]);
        //		ViewManager.inst.ShowView (new MediatorGiftShow ());
    }


	private string getInfoLevel(object[] arr)
	{
		string str = "";
		for (int i = 0; i < arr.GetLength (0); i++) {
			if (i == (arr.GetLength (0) - 1)) {
				str += Tools.GetMessageById("17002",new string[]{arr[i].ToString()});
			} else {
				str += Tools.GetMessageById("17003",new string[]{arr[i].ToString()});
			}
		}
		return str;
	}
	private void showCardBDNum()
	{
		g_bd0.visible = false;
		g_bd1.visible = false;
		g_bd2.visible = false;
		Dictionary<string,object> award_cfg = (Dictionary<string,object>)DataManager.inst.award [boxCfg ["award_id"].ToString ()];
		int cardindex = 0;
		string nums = "";
		int ra = 0;
		object[] obj = (object[])(((Dictionary<string,object>)award_cfg ["card"])["steps"]);
		for (int i = 0; i < obj.GetLength (0); i++) {
			Dictionary<string,object> dic = obj [i] as Dictionary<string,object>;
			if ((int)dic ["group"] == 0)
				break;
			if (Tools.NumSectionOne ((object[])dic ["open"], ModelManager.inst.userModel.effort_lv) < 1f)
				continue;
			nums = Math.Floor (Tools.NumSection ((object[])dic ["num"], ModelManager.inst.userModel.effort_lv) [0]) + "";
			ra = (int)dic ["group"];
			showCard (cardindex,ra,nums);
			cardindex++;
		}
		if (cardindex == 1) {
			g_bd0.y += 40;
		}
	}
	private void showCard(int index,int ra,string num)
	{
		GComponent goss = null;//new GComponent();
		switch (index) {
		case 0:
			goss = g_bd0;
			g_bd0.visible = true;
			break;
		case 1:
			goss = g_bd1;
			g_bd1.visible = true;
			break;
		case 2:
			goss = g_bd2;
			g_bd2.visible = true;
			break;
		}
		goss.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image:icon_kp" + (ra + 1));
		GTextField l = goss.GetChild ("n2").asTextField;
		l.text = "x"+num;
//		switch (ra) {
//		case 1:
//			l.color = Color.green;
//			break;
//		case 2:
//			l.color = new Color(170f/255f,0,1,1);
//			break;
//		case 3:
//			l.color = new Color(1,140f/255f,0,1);
//			break;
//		}
	}
	public override void Clear ()
	{
		base.Clear ();
		if (img_box != null) {
			img_box.Dispose ();
		}
		img_box = null;
	}
}
