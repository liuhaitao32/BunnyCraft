using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using FairyGUI;

public class MediatorBoxItem : BaseMediator {
	private ModelUser userModelr;
	private Dictionary<string,object> userExplore;
	private Dictionary<string,object> cfg;

//	private GTextField l_name;
	private GTextField l_coinNum;
	private GTextField l_cardNum;
	private GTextField l_time;
	private GImage img_btnIcon;
	private GGraph img_box;
	private GLoader img_coin;

	private GButton btn_sure;
	private GTextField L_buttontext;
	private GTextField l_jiesuo;

	public static Dictionary<string,object> data;
	public static string ID;
	public static int index;

	private GComponent g_bd0;
	private GComponent g_bd1;
	private GComponent g_bd2;

	private int _type;
	private bool eventFun = true;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_BOXITEM,false,Tools.GetMessageById((data["name"]).ToString()));

		cfg = DataManager.inst.systemSimple;
		userModelr = ModelManager.inst.userModel;
		userExplore = (Dictionary<string,object>)(userModelr.records ["explore"]);
//		l_name = view.GetChild ("n1").asTextField;
		l_coinNum = this.GetChild ("n8").asTextField;
		l_cardNum = this.GetChild ("n9").asTextField;
		l_time = this.GetChild ("n16").asTextField;
		img_btnIcon = this.GetChild ("n20").asImage;
		img_box = this.GetChild ("n4").asGraph;
		btn_sure = this.GetChild ("n17").asButton;
		img_coin = this.GetChild ("n7").asLoader;
		L_buttontext = this.GetChild ("n18").asTextField;
		l_jiesuo = this.GetChild ("n19").asTextField;
		g_bd0 = this.GetChild ("n15").asCom;
		g_bd1 = this.GetChild ("n14").asCom;
		g_bd2 = this.GetChild ("n13").asCom;

		this.GetChild ("n5").asTextField.text = Tools.GetMessageById ("19016");
		this.GetChild ("n10").asTextField.text = Tools.GetMessageById ("19017");
		L_buttontext.text = Tools.GetMessageById ("19018");
		GameObject asd = EffectManager.inst.AddPrefab (Tools.GetExploreBoxID(data ["icon"].ToString ()), img_box);
//		GameObjectScaler.Scale (asd, 0.8f);
		asd.transform.localScale *= 0.8f;
//		l_name.text = l_name.text = Tools.GetMessageById((data["name"]).ToString());
		btn_sure.text = "";


		if (ID.IndexOf ('S') != -1) {
			float[] card_num;
			if (((Dictionary<string,object>)data ["reward"]).ContainsKey ("gold")) {
				card_num = Tools.NumSection ((object[])(((Dictionary<string,object>)data ["reward"]) ["gold"]), userModelr.effort_lv);
				l_coinNum.text = Tools.GetMessageById("19002",new string[]{Math.Floor(card_num[0])+"",Math.Floor(card_num[1])+""});
				img_coin.url = Tools.GetResourceUrl ("Image:icon_xm");
			} else if (((Dictionary<string,object>)data ["reward"]).ContainsKey ("coin")) {
//				card_num = Tools.NumSection ((object[])(((Dictionary<string,object>)data ["reward"]) ["coin"]), userModelr.effort_lv);
//				l_coinNum.text = Tools.GetMessageById("19002",new string[]{Math.Floor(card_num[0])+"",Math.Floor(card_num[1])+""});
//				img_coin.url = Tools.GetResourceUrl ("Image:icon_xm");
				l_coinNum.text = "";
				img_coin.visible = false;
			} else {
				l_coinNum.text = "";
				img_coin.visible = false;
			}
			string id = "";
			foreach (string i in ((Dictionary<string,object>)data ["reward"]).Keys) {
				if (i != Config.ASSET_COIN) {
					id = ((Dictionary<string,object>)data ["reward"]) [i] as string;
				}
			}
			l_cardNum.text = Tools.GetMessageById((data["info"]).ToString(),new string[]{id});
		} else if (ID.IndexOf ('R') != -1) {
			Dictionary<string,object> award_cfg = (Dictionary<string,object>)DataManager.inst.award [data ["reward"].ToString ()];
			float[] card_num;
			if (award_cfg.ContainsKey ("gold")) {
				card_num = Tools.NumSection (((object[])award_cfg["gold"]), userModelr.effort_lv);
				l_coinNum.text = Tools.GetMessageById ("19011", new string[]{ Math.Floor (card_num [0]) + "", Math.Floor (card_num [1]) + "" });
				img_coin.url = Tools.GetResourceUrl ("Image:icon_xm");
			} else if (award_cfg.ContainsKey ("coin")) {
//				card_num = Tools.NumSection (((object[])award_cfg["coin"]), userModelr.effort_lv);
//				l_coinNum.text = Tools.GetMessageById ("19002", new string[]{ Math.Floor (card_num [0]) + "", Math.Floor (card_num [1]) + "" });
//				img_coin.url = Tools.GetResourceUrl ("Image:icon_xm");
				l_coinNum.text = "";
				img_coin.visible = false;
			} else {
				l_coinNum.text = "";
				img_coin.visible = false;
			}


			card_num = Tools.NumSection((object[])(((Dictionary<string,object>)award_cfg ["card"]) ["num"]),userModelr.effort_lv);
			l_cardNum.text = Tools.GetMessageById((data["info"]).ToString(),new string[]{Math.Floor(card_num[0])+""});
		}

		if (ID.IndexOf ('S') != -1) {
//			view.height = 400;
			this.GetController ("c1").selectedIndex = 1;
			l_cardNum.text = Tools.GetMessageById ("19194", new string[]{ "1" });
			this.GetChild ("n6").asLoader.url = ""+ Tools.GetResourceUrl ("Image:icon_xiaofeiji");
			this.height = 354;
//			group.y += (466 - 354) / 2;
		} else {
			showCardBDNum ();
		}
		if (img_coin.visible) {
			this.GetChild ("n6").asLoader.y = 127;
			l_cardNum.y = 136;
		} else {
			this.GetChild ("n6").asLoader.y = 97;
			l_cardNum.y = 106;
		}
//		view.GetChild ("n24").asButton.onClick.Add (() => {
//			ViewManager.inst.CloseView(this);
//		});

		long time = (long)(Convert.ToSingle ((int)data ["time"])*10000000);
		l_time.text = Tools.GetMessageById ("19003", new string[]{ Tools.TimeFormat2(time,1) });

		if (userExplore ["index"] != null) {
			DateTime overTime = (DateTime)userExplore ["time"];
			if (index == (int)userExplore ["index"]) {
				if (Tools.GetSystemTicks () > overTime.Ticks) {
					setType (2);
				} else {
					setType (1);
				}
			} else {
				if (Tools.GetSystemTicks () > overTime.Ticks) {
					setType (0);
				} else {
					setType (3);
				}

			}
		} else {
			setType (0);
		}
		btn_sure.onClick.Add (OnSureClickHandler);
	}
	//0未解锁，1正在解锁，2解锁完成
	private void setType(int type)
	{
		_type = type;
		switch(type)
		{
		case 0:
			btn_sure.text = Tools.GetMessageById ("19007");
			img_btnIcon.visible = false;
			L_buttontext.visible = false;
			l_jiesuo.text = "";
			break;
		case 1:
			TimerManager.inst.Add (1f, 0, Time_Tick);
			l_jiesuo.text = "";
			Time_Tick (1);
			L_buttontext.visible = true;
			break;
		case 2:
			btn_sure.text = Tools.GetMessageById ("19008");
			img_btnIcon.visible = false;
			l_time.text = "";
			L_buttontext.visible = false;
			l_jiesuo.text = "";
			break;
		case 3:
			long timetext = (int)data ["time"];
			l_time.text = Tools.GetMessageById ("19014");
			float time_coin = (int)cfg ["time_coin"];
			btn_sure.text = "";
			L_buttontext.visible = true;
			l_jiesuo.text = Tools.GetMessageById ("19019", new string[]{ Tools.GetCoinByTime (timetext).ToString () });
			break;
		}
	}
	private void Time_Tick (float time)
	{
		DateTime overTime = (DateTime)userExplore ["time"];
		if (Tools.GetSystemTicks () > overTime.Ticks) {
			TimerManager.inst.Remove (Time_Tick);
			setType (2);
		} else {
			long timetext = (overTime.Ticks - Tools.GetSystemTicks ());
			l_time.text = Tools.GetMessageById ("19004", new string[]{ Tools.TimeFormat(timetext,1)});
			timetext = timetext / 10000 / 1000;
			l_jiesuo.text = Tools.GetMessageById ("19005", new string[]{ Tools.GetCoinByTime (timetext).ToString () });
		}
	}
	private void OnSureClickHandler()
	{
		
		switch(_type)
		{
		case 0:
			NetHttp.inst.Send (NetBase.HTTP_UNLOCK_EXPLORE, "index="+index, UnlockBox);
			break;
		case 1:
			DateTime overTime = (DateTime)userExplore ["time"];
			long timetext = (overTime.Ticks - Tools.GetSystemTicks ());
			timetext = timetext / 10000 / 1000;
			if(ModelUser.GetCanBuy (Config.ASSET_COIN, Tools.GetCoinByTime (timetext)))
			{
				NetHttp.inst.Send (NetBase.HTTP_OPEN_EXPLORE, "index="+index, OpenBox);
			}	
			break;
		case 2:
			NetHttp.inst.Send (NetBase.HTTP_OPEN_EXPLORE, "index="+index, OpenBox);
			break;
		case 3:
			timetext = (int)data ["time"];
			if(ModelUser.GetCanBuy (Config.ASSET_COIN, Tools.GetCoinByTime (timetext)))
			{
				NetHttp.inst.Send (NetBase.HTTP_OPEN_EXPLORE, "index="+index, OpenBox);
			}
			break;
		}

	}
	private void OpenBox(VoHttp vo)
	{
        int gold = userModelr.gold;
		eventFun = false;
		ModelManager.inst.userModel.UpdateData (vo.data);
        Tools.FullCard(( (Dictionary<string, object>)vo.data ), gold);
        if (ID.IndexOf ("S") != -1) {
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_UPDATE));
		}
		ViewManager.inst.CloseView (this);
		ViewManager.inst.ShowGift ((Dictionary<string,object>)(((Dictionary<string,object>)vo.data) ["gifts_dict"]), Tools.GetExploreBoxID (data ["icon"].ToString ()));
//		MediatorGiftShow.all_Data = (Dictionary<string,object>)(((Dictionary<string,object>)vo.data) ["gifts_dict"]);
		MediatorGiftShow.isExplore = true;
        
    }
	private void showCardBDNum()
	{
		g_bd0.visible = false;
		g_bd1.visible = false;
		g_bd2.visible = false;
		Dictionary<string,object> award_cfg = (Dictionary<string,object>)DataManager.inst.award [data ["reward"].ToString ()];
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
		if (cardindex == 0) {
//			view.height = 400;
			this.GetController ("c1").selectedIndex = 1;
			this.height = 354;
//			group.y += (466 - 354) / 2;
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
		switch (ra) {
		case 1:
			l.color = Color.green;
			break;
		case 2:
			l.color = new Color(170f/255f,0,1,1);
			break;
		case 3:
			l.color = new Color(1,140f/255f,0,1);
			break;
		}
	}
	private void UnlockBox(VoHttp vo)
	{
		ModelManager.inst.userModel.UpdateData (vo.data);
		ViewManager.inst.CloseView (this);
		this.DispatchGlobalEvent(new MainEvent (MainEvent.EXPLORE_UNLOCK));
	}
	public override void Clear ()
	{
		if (eventFun) {
			this.DispatchGlobalEvent (new MainEvent (MainEvent.MAIN_EXPLORE));
		}
		TimerManager.inst.Remove (Time_Tick);
		base.Clear ();
	}
}
