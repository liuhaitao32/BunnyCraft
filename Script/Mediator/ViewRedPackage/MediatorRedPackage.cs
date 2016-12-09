using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorRedPackage : BaseMediator {

	public long todayUpTime = 0;
	//上一个红包领取时间  秒
	public long currTime = 20000;
	//当前系统时间   秒

	private GTextField l_TimeInfo;
	private GButton btn_Rob;
	private GButton btn_Give;
//	private GTextField l_getRedNum;
	private GTextField l_redcoin;
	private GTextField n20;
    private GTextField n26;
	private Dictionary<string,object> redbagCfg;

	private ModelUser userModel;
	public MediatorRedPackage ()
	{
	}
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_REDPACKAGE,false);
		todayUpTime = Convert.ToInt64 (Tools.GetSystemSecond ()) + (0);
		userModel = ModelManager.inst.userModel;
		redbagCfg = DataManager.inst.redbag;

		n20 = this.GetChild ("n20").asTextField;
		btn_Rob = this.GetChild ("n13").asButton;
		btn_Give = this.GetChild ("n12").asButton;
		l_TimeInfo = this.GetChild ("n8").asTextField;
		l_redcoin = this.GetChild ("n10").asTextField;
        n26 = this.GetChild("n26").asTextField;
        n26.text = Tools.GetMessageById("13155");
//		GameObject asd = EffectManager.inst.AddEffect (Config.EFFECT_MAILBOX,"stand", this.GetChild ("n1").asGraph);
//		GameObjectScaler.ScaleParticles (asd, 0);
//		EffectManager.inst.StopAnimation (asd);
//		TimerManager.inst.Add (0.01f, 1, (float obj) => {
//			EffectManager.inst.StopAnimation (asd);
//		});
//		view.GetChild ("n10").asTextField.text = Tools.GetMessageById ("12014");
		ViewManager.inst.AddTouchTip ("GButton", this.GetChild ("n11").asButton, new Dictionary<string,object>{{Config.ASSET_REDBAGCOIN,1}});
//		view.GetChild ("n15").asTextField.text = Tools.GetMessageById ("12022", new object[]{ redbagCfg ["exp"] });
		btn_Give.text = Tools.GetMessageById("12024");
		btn_Rob.text = Tools.GetMessageById ("12025");
        n26.visible = false;
		this.GetChild ("n24").visible = false;
		btn_Rob.onClick.Add (OnRobRedViewHandler);
		btn_Give.onClick.Add (OnGiveRedViewHandler);
		n20.text = "";
		l_TimeInfo.text = "";
		OnUpdataHandler ();
		if ((int)(ModelManager.inst.userModel.records ["redbag_coin"]) < 1) {
			btn_Give.text = Tools.GetMessageById ("12018");
            n26.visible=true;
			userModel.Remove_Notice (btn_Give);
		} else {
			this.GetChild ("n24").visible = true;
			//btn_Give.GetChild ("title").y -= 15;
			this.GetChild ("n22").asTextField.text = Tools.GetMessageById ("12022", new object[]{ (int)redbagCfg ["exp"] * (int)userModel.records ["redbag_coin"] + "" });
			if ((int)userModel.records ["redbag_coin"] > (int)DataManager.inst.redbag ["redpoint"]) {
				userModel.Add_Notice (btn_Give, new Vector2 (130, -10));
			} else {
				userModel.Remove_Notice (btn_Give);
			}
		}
		TimerManager.inst.Add (1f,0,Time_Tick);
		Time_Tick (0f);

//		view.GetChild ("n18").asButton.onClick.Add (() => {
//			ViewManager.inst.CloseView(this);
//		});
		this.AddGlobalListener(MainEvent.RED_GIFT, OnUpdataHandler);
	}
	private void Time_Tick (float time)
	{
		currTime = ModelManager.inst.userModel.ReturnTickTime ();
		DateTime date = (DateTime)ModelManager.inst.userModel.records ["redbag_time"];
		DateTime nowDate = ModelManager.inst.gameModel.time;
		bool isToday = true;
		l_TimeInfo.text = "asd";
		n20.text = "veveveve";
		if (date.Year != nowDate.Year || date.Month != nowDate.Month || date.Day != nowDate.Day)
			isToday = false;

		if ((int)(ModelManager.inst.userModel.records ["redbag_times"]) >= ((object[])redbagCfg["redbag_pool"]).GetLength (0)&&isToday) {
			btn_Rob.grayed = true;
			l_TimeInfo.text = "";
			n20.text = Tools.GetMessageById ("10015");
		} else {
			if (currTime > 0)
			{
				btn_Rob.grayed = true;
				string s = Tools.TimeFormat (currTime * 10000, 0);
				n20.text = "";
				l_TimeInfo.text = s;
			}
			else
			{
				btn_Rob.grayed = false;
				n20.text = Tools.GetMessageById ("12003");

				l_TimeInfo.text = "";
			}
		}
	}
	private void OnUpdataHandler(MainEvent e = null)
	{
		DateTime date = (DateTime)ModelManager.inst.userModel.records ["redbag_time"];
		DateTime nowDate = ModelManager.inst.gameModel.time;
		bool isToday = true;
		if (date.Year != nowDate.Year || date.Month != nowDate.Month || date.Day != nowDate.Day)
			isToday = false;
		TimerManager.inst.Add (1f,0,Time_Tick);
		Time_Tick (0f);
		if(this.text != null)
		{
			this.text = Tools.GetMessageById ("12011",new string[]{((object[])redbagCfg["redbag_pool"]).GetLength (0)+""});
			if (isToday) {
				this.text = Tools.GetMessageById ("12011", new string[]{ (((object[])redbagCfg["redbag_pool"]).GetLength (0) - (int)(ModelManager.inst.userModel.records ["redbag_times"])) + "" });
			}
		}
		if(l_redcoin != null)
		{
			l_redcoin.text = "X " + ((int)(ModelManager.inst.userModel.records ["redbag_coin"]) < 0 ? 0 : (int)(ModelManager.inst.userModel.records ["redbag_coin"])) + "";
//			l_redcoin.text = ((int)(ModelManager.inst.userModel.records ["redbag_coin"]) < 0 ? 0 : (int)(ModelManager.inst.userModel.records ["redbag_coin"])) + "";
		}
	}
	private void OnEventHandler ()
	{
		todayUpTime = Convert.ToInt64 (Tools.GetSystemSecond ()) + (1 * 1000 * 10);
	}
	private void OnRobRedViewHandler ()
	{
		currTime = ModelManager.inst.userModel.ReturnTickTime ();
		DateTime date = (DateTime)ModelManager.inst.userModel.records ["redbag_time"];
		DateTime nowDate = ModelManager.inst.gameModel.time;
		bool isToday = true;
		if (date.Year != nowDate.Year || date.Month != nowDate.Month || date.Day != nowDate.Day)
			isToday = false;

		if ((int)(ModelManager.inst.userModel.records ["redbag_times"]) >= ((object[])redbagCfg["redbag_pool"]).GetLength (0)&&isToday) {
			ViewManager.inst.ShowText (Tools.GetMessageById ("12006"));
		} else {
			if (currTime > 0)
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("12004"));
			}
			else
			{
				NetHttp.inst.Send (NetBase.HTTP_GET_REDBAG, "", Get_Red);
			}
		}


	}
	private void Get_Red(VoHttp vo)
	{
		MediatorRobRed._coin = ModelManager.inst.userModel.coin;
		MediatorRobRed._gold = ModelManager.inst.userModel.gold;
		MediatorRobRed.redVo = vo;
		ModelManager.inst.userModel.UpdateData (vo.data);
		ViewManager.inst.ShowView<MediatorRobRed> (false);
		OnEventHandler ();
	}
	private void OnGiveRedViewHandler ()
	{
		if((int)ModelManager.inst.userModel.records ["redbag_coin"] > 0)
		{
			btn_Give.touchable = false;
			btn_Rob.touchable = false;
			string prom = "rnum=";
			prom += ModelManager.inst.userModel.records ["redbag_coin"].ToString();
			NetHttp.inst.Send (NetBase.HTTP_SEND_REDBAG, prom, OnSendHandler);
			return;
		}
		ViewManager.inst.ShowView<MediatorGiveRed> ();
//		ViewManager.inst.CloseView (this);
	}
	private VoHttp redvo;
	private void OnSendHandler (VoHttp vo)
	{
		int addExp = (int)(((Dictionary<string,object>)vo.data) ["add_exp"]);
		ViewManager.inst.ShowText (Tools.GetMessageById ("12008", new string[] { addExp.ToString () }));
		Dictionary<string,object> d = new Dictionary<string, object> ();
		l_redcoin.text = "X " + "0";
		d.Add (Config.ASSET_EXP, addExp);
		btn_Give.text = Tools.GetMessageById ("12018");
        n26.visible = true;
		userModel.Remove_Notice (btn_Give);
		this.GetChild ("n24").visible = false;
		//btn_Give.GetChild ("title").y = 0;
		this.DispatchGlobalEvent (new MainEvent (MainEvent.RED_UPDATE));
		redvo = vo;
		ViewManager.inst.ShowIcon (d,()=>{
			ModelManager.inst.userModel.UpdateData (vo.data);
			redvo = null;
			btn_Give.touchable = true;
			btn_Rob.touchable = true;
			this.DispatchGlobalEvent (new MainEvent (MainEvent.RED_UPDATE));
		});
	}
	public override void Clear ()
	{
		if (redvo != null) {
			ViewManager.inst.ShowIconOver ();
			if (redvo!=null && redvo.data != null) {
				ModelManager.inst.userModel.UpdateData (redvo.data);
			}
		}
		redvo = null;
		TimerManager.inst.Remove (Time_Tick);
		base.Clear ();
	}
}
