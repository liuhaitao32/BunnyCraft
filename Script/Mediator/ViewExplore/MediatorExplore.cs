using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using FairyGUI;
using DG.Tweening;

public class MediatorExplore : BaseMediator
{
	public delegate void OnCall ();

	public OnCall call;

	private GButton close;
	private GGraph move;

	private GButton RanderBox0;
	private GButton RanderBox1;
	private GButton RanderBox2;

	private GComponent RanderBox0x;
	private GComponent RanderBox1x;
	private GComponent RanderBox2x;

	private ModelUser userModelr;
	private Dictionary<string,object> userExplore;
	private Dictionary<string,object> ExpCfg;

	private int type = 0;
	//0未解锁1倒计时2领奖
	private ComProgressBar pro_pro;
	private GLoader fill;

	public override void Init ()
	{		
		base.Init ();
		this.Create (Config.VIEW_EXPLORE);

		this.x = ModelManager.inst.gameModel.width;
		this.y = 0;

		ExpCfg = DataManager.inst.explore;
		userModelr = ModelManager.inst.userModel;
		userExplore = (Dictionary<string,object>)(userModelr.records ["explore"]);

		close = this.GetChild ("n7").asButton;
		close.onClick.Add (OnOverFunction);
		RanderBox0x = this.GetChild ("n2").asCom;
		RanderBox0 = RanderBox0x.GetChild ("n0").asButton;
		RanderBox0.onClick.Add (OnClickRanderBox1);
		RanderBox1x = this.GetChild ("n3").asCom;
		RanderBox1 = RanderBox1x.GetChild ("n0").asButton;
		RanderBox1.onClick.Add (OnClickRanderBox2);
		RanderBox2x = this.GetChild ("n4").asCom;
		RanderBox2 = RanderBox2x.GetChild ("n0").asButton;
		RanderBox2.onClick.Add (OnClickRanderBox3);

		move = this.GetChild ("n8").asGraph;

		pro_pro = this.GetChild ("n5") as ComProgressBar;
		ViewManager.inst.AddTouchTip (Config.TOUCH_PROGRESSBAR, pro_pro, Config.TOUCH_COMELSCORE);
		pro_pro.skin = ComProgressBar.BAR9;
		pro_pro.offsetY = -2;
		fill = pro_pro.GetBar ();
		ColorFilter gggg = new ColorFilter();
		fill.filter = gggg;
		gggg.Reset ();
		pro_pro.value = userModelr.el_score;
		pro_pro.max = (int)(((object[])DataManager.inst.systemSimple ["el_score"]) [1]);
		if (userModelr.el_score == (int)(((object[])DataManager.inst.systemSimple ["el_score"]) [1]))
		{
			gggg.AdjustBrightness (0.1f);
			gggg.AdjustContrast (0.5f);
			gggg.AdjustSaturation (0.5f);
			gggg.AdjustHue (-0.2f);
		}

		this.AddGlobalListener (MainEvent.EXPLORE_UNLOCK, OnUnlockHandler);
		this.AddGlobalListener (MainEvent.EXPLORE_GIFT, OnGiftHandler);
		this.AddGlobalListener (MainEvent.MAIN_EXPLORE, OnMainExploreFush);
		this.AddGlobalListener (MainEvent.JUMP_ELSCORE, Onfunction);
		if (userExplore ["index"] != null)
		{
			DateTime overTime = (DateTime)userExplore ["time"];
			if (Tools.GetSystemTicks () > overTime.Ticks)
			{
				type = 2;
			}
			else
			{
				type = 1;
				TimerManager.inst.Add (1f, 0, Time_Tick);
				Time_Tick (0f);
			}
		}
		else
		{
			type = 0;
		}
		setBoxData (RanderBox0x, (object[])(((object[])userExplore ["el_ids"]) [0]), 0);
		setBoxData (RanderBox1x, (object[])(((object[])userExplore ["el_ids"]) [1]), 1);
		setBoxData (RanderBox2x, (object[])(((object[])userExplore ["el_ids"]) [2]), 2);

		this.visible = false;
	}
	private Tweener proTween;
	private void Onfunction(MainEvent e)
	{
		string[] str = e.data as string[];
		pro_pro.value += Convert.ToInt32 (str [1]);
		if (pro_pro.value > pro_pro.max)
			pro_pro.value = pro_pro.max;

//		DOTween.Kill (pro_pro, true);
		if (proTween != null) {
			proTween.Kill ();
		}
		proTween = pro_pro.TweenMoveY (18, 0.05f).OnComplete (() =>
		{
			pro_pro.y = 29;
		});
		ComGoldCoinExp.Elcount--;
		if (ComGoldCoinExp.Elcount <= 0)
		{
			ComGoldCoinExp.Elcount = 0;
			this.DispatchGlobalEvent (new MainEvent (MainEvent.JUMPEL_OVER));
		}
	}
	public void UpElScore()
	{
		pro_pro.value = userModelr.el_score;
	}
	private void Time_Tick (float time)
	{
		int oldtype = type;

		DateTime overTime = (DateTime)userExplore ["time"];
		if (Tools.GetSystemTicks () > overTime.Ticks)
		{
			TimerManager.inst.Remove (Time_Tick);
			type = 2;
		}
		else
		{
			type = 1;
		}
		if (type != oldtype) {
			if (userExplore ["index"] != null) {
				switch ((int)userExplore ["index"]) {
				case 0:
					setBoxData (RanderBox0x, (object[])(((object[])userExplore ["el_ids"]) [0]), 0);
					break;
				case 1:
					setBoxData (RanderBox1x, (object[])(((object[])userExplore ["el_ids"]) [1]), 1);
					break;
				case 2:
					setBoxData (RanderBox2x, (object[])(((object[])userExplore ["el_ids"]) [2]), 2);
					break;
				}
			}
		} else {
			if (userExplore ["index"] != null) {
				switch ((int)userExplore ["index"]) {
				case 0:
					setBoxText (RanderBox0x, (object[])(((object[])userExplore ["el_ids"]) [0]), 0);
					break;
				case 1:
					setBoxText (RanderBox1x, (object[])(((object[])userExplore ["el_ids"]) [1]), 1);
					break;
				case 2:
					setBoxText (RanderBox2x, (object[])(((object[])userExplore ["el_ids"]) [2]), 2);
					break;
				}
			}
		}

	}

	private void OnGiftHandler (MainEvent e)
	{
		userExplore = (Dictionary<string,object>)(userModelr.records ["explore"]);
		if (userExplore ["index"] == null)
		{
			type = 0;
			TimerManager.inst.Remove (Time_Tick);
		}
		setBoxData (RanderBox0x, (object[])(((object[])userExplore ["el_ids"]) [0]), 0);
		setBoxData (RanderBox1x, (object[])(((object[])userExplore ["el_ids"]) [1]), 1);
		setBoxData (RanderBox2x, (object[])(((object[])userExplore ["el_ids"]) [2]), 2);
		OnMainExploreFush ();
	}

	private void OnUnlockHandler (MainEvent e)
	{//动画效果
		userExplore = (Dictionary<string,object>)(userModelr.records ["explore"]);
		TimerManager.inst.Remove (Time_Tick);
		TimerManager.inst.Add (1f, 0, Time_Tick);
		Time_Tick (0f);
		type = 1;
	}
	private void setBoxText(GComponent go, object[] boxID, int index)
	{
		GTextField l_label = go.GetChild ("n7").asTextField;
		DateTime overTime = (DateTime)userExplore ["time"];
		long timetext = (overTime.Ticks - Tools.GetSystemTicks ());
		l_label.text = Tools.TimeFormat2 (timetext);
	}
	private void setBoxData (GComponent go, object[] boxID, int index,bool boxMove = false)
	{
		Dictionary<string,object> cc = DataManager.inst.systemSimple;
		GButton btn = go.GetChild ("n0").asButton;
		Controller c1 = go.GetController("c1");
		GGraph icon = go.GetChild ("n4").asGraph;
		GGroup n5 = go.GetChild ("n5").asGroup;
		GGroup n6 = go.GetChild ("n6").asGroup;

		GTextField l_label = go.GetChild ("n7").asTextField;
		GTextField l_coin = go.GetChild ("n9").asTextField;
		GImage img_coin = go.GetChild ("n10").asImage;
		GTextField l_ll = go.GetChild ("n8").asTextField;
		GTextField l_price = go.GetChild ("n12").asTextField;
		go.GetChild ("n11").text = Tools.GetMessageById ("19015");

		GTextField dianjikaiqi = go.GetChild ("n17").asTextField;
//		ColorFilter gggg = btn_img.filter as ColorFilter;
//		gggg.Reset ();
//		btn_img.url = Tools.GetResourceUrl ("Image:bg_zhuyeyouce1");
		c1.selectedIndex = 0;
		l_price.text = ((int)((object[])cc ["el_score"]) [2]).ToString ();
//		n5.visible = false;
//		n6.visible = false;
		if (boxID == null)
		{
			n5.visible = false;
			n6.visible = true;
		}
		else
		{
			Dictionary<string,object> cfg = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [boxID [0].ToString ()]);
			n5.visible = true;
			n6.visible = false;

			l_label.text = Tools.GetMessageById ("19012");
			dianjikaiqi.text = "";
			l_ll.text = "";
			img_coin.visible = false;

			long timetext = (long)(Convert.ToSingle ((int)cfg ["time"]) * 10000000);
			l_coin.text = Tools.TimeFormat2 (timetext, 1);
			l_coin.x = 25f;
			Tools.RemoveGraphObject (go.GetChild ("n16").asGraph);
			Tools.RemoveGraphObject (icon);
			GameObject dh = EffectManager.inst.AddEffect (Tools.GetExploreBoxID (cfg ["icon"].ToString ()), "stand", icon);
			dh.transform.localScale *= 0.6f;
//			GameObjectScaler.Scale (dh, 0.6f);
			if (boxMove) {
				icon.TweenScale (new Vector2 (0.5f, 0.5f), 0.2f).OnComplete(()=>{
					icon.TweenScale(new Vector2(1.2f,1.2f),0.2f).OnComplete(()=>{
						icon.scale = new Vector2(1,1);
					});
				});
			}
			if (userExplore ["index"] != null)
			{
				if (index == (int)userExplore ["index"] || ((int)boxID [1]) == 1) {
					l_ll.text = Tools.GetMessageById ("19013");
					long time = (long)(Convert.ToSingle ((int)cfg ["time"]) * 10000000);
					if (((int)boxID [1]) == 1 || (type == 2 && index == (int)userExplore ["index"])) {
//						btn_img.url = Tools.GetResourceUrl ("Image:bg_zhuyeyouce2");
						c1.selectedIndex = 1;
//						EffectManager.inst.PlayEffect (dh, "ready");
						Tools.RemoveGraphObject (icon);
						dh = EffectManager.inst.AddEffect (Tools.GetExploreBoxID (cfg ["icon"].ToString ()), "ready", icon);
//						GameObjectScaler.Scale (dh, 0.6f);
						dh.transform.localScale *= 0.6f;
						l_ll.text = "";
						l_label.text = "";
						dianjikaiqi.text = Tools.GetMessageById ("19006");
						l_coin.text = "";
					} else if (type == 1) {
						EffectManager.inst.AddEffect (Config.EFFECT_COUNTDOWN, "countdown", go.GetChild ("n16").asGraph);
						DateTime overTime = (DateTime)userExplore ["time"];
						timetext = (overTime.Ticks - Tools.GetSystemTicks ());
						l_label.text = Tools.TimeFormat2 (timetext);
						if (Tools.GetSystemTicks () > overTime.Ticks) {
							TimerManager.inst.Remove (Time_Tick);
//							EffectManager.inst.PlayEffect (dh, "ready");
							Tools.RemoveGraphObject (icon);
							dh = EffectManager.inst.AddEffect (Tools.GetExploreBoxID (cfg ["icon"].ToString ()), "ready", icon);
//							GameObjectScaler.Scale (dh, 0.6f);
							dh.transform.localScale *= 0.6f;
							dianjikaiqi.text = Tools.GetMessageById ("19006");
							l_label.text = "";
							type = 2;
							l_ll.text = "";
							l_coin.text = "";
							img_coin.visible = false;
						} else {
							timetext = timetext / 10000 / 1000;
							l_coin.text = Tools.GetCoinByTime (timetext).ToString ();
							l_coin.x = 14f;
							img_coin.visible = true;

						}
					}
				}
			} else {
				if (((int)boxID [1]) == 1) {
//					btn_img.url = Tools.GetResourceUrl ("Image:bg_zhuyeyouce2");
					c1.selectedIndex = 1;
//					gggg.AdjustBrightness (0.1f);
//					gggg.AdjustContrast (0.5f);
//					gggg.AdjustSaturation (0.5f);
//					gggg.AdjustHue (-0.2f);
					l_ll.text = "";//Tools.GetMessageById ("19013");
					l_label.text = Tools.GetMessageById ("19006");
					l_coin.text = "";
//					EffectManager.inst.PlayEffect (dh, "ready");
					Tools.RemoveGraphObject (icon);
					dh = EffectManager.inst.AddEffect (Tools.GetExploreBoxID (cfg ["icon"].ToString ()), "ready", icon);
//					GameObjectScaler.Scale (dh, 0.6f);
					dh.transform.localScale *= 0.6f;
				}
			}
		}
	}

	private GameObject boxBoom;

	private void Refresh_Explore (VoHttp vo)
	{
		ModelManager.inst.userModel.UpdateData (vo.data);
		this.DispatchGlobalEvent (new MainEvent (MainEvent.RED_UPDATE));
//		NetHttp.inst.Send (NetBase.HTTP_GETEFFORT, "", (VoHttp _vo) =>
//			{
//				ModelManager.inst.userModel.UpdateData (_vo.data);
//				this.DispatchGlobalEvent (new MainEvent (MainEvent.RED_UPDATE));
//			});
		
		ExpCfg = DataManager.inst.explore;
		userModelr = ModelManager.inst.userModel;
		userExplore = (Dictionary<string,object>)(userModelr.records ["explore"]);
		pro_pro.value = userModelr.el_score;
		bool b0 = false;
		bool b1 = false;
		bool b2 = false;
		switch (fefresh)
		{
		case 0:
			move.y = RanderBox0x.y + 90;
			b0 = true;
			break;
		case 1:
			move.y = RanderBox1x.y + 90;
			b1 = true;
			break;
		case 2:
			move.y = RanderBox2x.y + 90;
			b2 = true;
			break;
		}
		boxBoom = EffectManager.inst.AddEffect (Config.EFFECT_GETBOX, "", this.GetChild ("n8").asGraph);
//		GameObjectScaler.Scale (boxBoom, 0.8f);
		boxBoom.transform.localScale *= 0.8f;

		fefresh = -1;
		setBoxData (RanderBox0x, (object[])(((object[])userExplore ["el_ids"]) [0]), 0, b0);
		setBoxData (RanderBox1x, (object[])(((object[])userExplore ["el_ids"]) [1]), 1, b1);
		setBoxData (RanderBox2x, (object[])(((object[])userExplore ["el_ids"]) [2]), 2, b2);
		fill.filter = null;
//		ColorFilter gggg = fill.filter as ColorFilter;
//		gggg.Reset ();
	}

	private int fefresh = -1;

	private void OnClickRanderBox1 ()
	{
		TimerManager.inst.Remove (onTimersFun);
		if (((object[])userExplore ["el_ids"]) [0] == null)
		{
			Dictionary<string,object> cc = DataManager.inst.systemSimple;
			if ((int)((object[])cc ["el_score"]) [2] > userModelr.el_score) {
				ViewManager.inst.ShowText (Tools.GetMessageById("10030"));
				return;
			}
			fefresh = 0;
			NetHttp.inst.Send (NetBase.HTTP_REFRESH_EXPLORE, "index=0", Refresh_Explore);
			frushiTime ();
			return;
		}
		string id = ((object[])(((object[])userExplore ["el_ids"]) [0])) [0].ToString ();
		click_id = id;
		if ((type == 2 && (int)userExplore ["index"] == 0) || ((object[])(((object[])userExplore ["el_ids"]) [0])) [1].ToString () == "1")
		{
			Dictionary<string,object> cfg = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [id]);
			clickBox = Tools.GetExploreBoxID (cfg ["icon"].ToString ());
			NetHttp.inst.Send (NetBase.HTTP_OPEN_EXPLORE, "index=0", OpenBox);
			return;
		}
		MediatorBoxItem.ID = id;
		MediatorBoxItem.index = 0;
		MediatorBoxItem.data = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [id]);
		ViewManager.inst.ShowView<MediatorBoxItem> ();
	}

	private void OnClickRanderBox2 ()
	{
		TimerManager.inst.Remove (onTimersFun);
		if (((object[])userExplore ["el_ids"]) [1] == null)
		{
			Dictionary<string,object> cc = DataManager.inst.systemSimple;
			if ((int)((object[])cc ["el_score"]) [2] > userModelr.el_score) {
				ViewManager.inst.ShowText (Tools.GetMessageById("10030"));
				return;
			}
			fefresh = 1;
			NetHttp.inst.Send (NetBase.HTTP_REFRESH_EXPLORE, "index=1", Refresh_Explore);
			frushiTime ();
			return;
		}
		string id = ((object[])(((object[])userExplore ["el_ids"]) [1])) [0].ToString ();
		click_id = id;
		if ((type == 2 && (int)userExplore ["index"] == 1) || ((object[])(((object[])userExplore ["el_ids"]) [1])) [1].ToString () == "1")
		{
			Dictionary<string,object> cfg = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [id]);
			clickBox = Tools.GetExploreBoxID (cfg ["icon"].ToString ());
			NetHttp.inst.Send (NetBase.HTTP_OPEN_EXPLORE, "index=1", OpenBox);
			return;
		}
		MediatorBoxItem.ID = id;
		MediatorBoxItem.index = 1;
		MediatorBoxItem.data = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [id]);
		ViewManager.inst.ShowView<MediatorBoxItem> ();
	}

	private void OnClickRanderBox3 ()
	{
		TimerManager.inst.Remove (onTimersFun);
		if (((object[])userExplore ["el_ids"]) [2] == null)
		{
			Dictionary<string,object> cc = DataManager.inst.systemSimple;
			if ((int)((object[])cc ["el_score"]) [2] > userModelr.el_score) {
				ViewManager.inst.ShowText (Tools.GetMessageById("10030"));
				return;
			}
			fefresh = 2;
			NetHttp.inst.Send (NetBase.HTTP_REFRESH_EXPLORE, "index=2", Refresh_Explore);
			frushiTime ();
			return;
		}
		string id = ((object[])(((object[])userExplore ["el_ids"]) [2])) [0].ToString ();
		click_id = id;
		if ((type == 2 && (int)userExplore ["index"] == 2) || ((object[])(((object[])userExplore ["el_ids"]) [2])) [1].ToString () == "1")
		{
			Dictionary<string,object> cfg = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [id]);
			clickBox = Tools.GetExploreBoxID (cfg ["icon"].ToString ());
			NetHttp.inst.Send (NetBase.HTTP_OPEN_EXPLORE, "index=2", OpenBox);
			return;
		}
		MediatorBoxItem.ID = id;
		MediatorBoxItem.index = 2;
		MediatorBoxItem.data = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [id]);
		ViewManager.inst.ShowView<MediatorBoxItem> ();
	}
	private string click_id = "";
	private string clickBox;

	private void OpenBox (VoHttp vo)
	{
		ModelManager.inst.userModel.UpdateData (vo.data);
        ViewManager.inst.CloseView (this);
		ViewManager.inst.ShowGift ((Dictionary<string,object>)(((Dictionary<string,object>)vo.data) ["gifts_dict"]), clickBox);
		MediatorGiftShow.isExplore = true;
		if (click_id.IndexOf ("S") != -1) {
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_UPDATE));
		}
        
    }

	private void onCloseHandler ()
	{
		ViewManager.inst.ShowScene <MediatorMain>();
	}
	private string BiDaXiao(string id1,string id2)
	{
		string idwin = "";
		if (id1.IndexOf ("S") != -1) {
			if (id2.IndexOf ("S") != -1) {
				if (Convert.ToInt32 (id1.Split ('S') [1]) >= Convert.ToInt32 (id2.Split ('S') [1])) {
					idwin = id1;
				} else {
					idwin = id2;
				}
			} else {
				idwin = id1;
			}
		} else {
			if (id2.IndexOf ("S") != -1) {
				idwin = id2;
			} else {
//				Log.debug(Convert.ToInt32 (id1.Split ('R') [1]),Convert.ToInt32 (id1.Split ('R') [1]))
				if (Convert.ToInt32 (id1.Split ('R') [1]) >= Convert.ToInt32 (id2.Split ('R') [1])) {
					idwin = id1;
				} else {
					idwin = id2;
				}
			}
		}
		return idwin;
	}
	public Dictionary<string,object> MainShowBtnType ()
	{
		Dictionary<string,object> cfg;
		//0没箱子&&没分（纯宝箱）1有分||有箱子没开（箱子+问号）2正在倒计时（箱子+倒计时）3 已开要领奖（箱子+叹号）
		Dictionary<string,object> returnData = new Dictionary<string, object> ();
//		bool haveBox = false;
		string boxid = null;
		int _i = -1;

		if (userExplore ["index"] != null)
		{
			DateTime overTime = (DateTime)userExplore ["time"];
			if (Tools.GetSystemTicks () > overTime.Ticks)
			{
				for (int i = 0; i < ((object[])userExplore ["el_ids"]).Length; i++)
				{
					if (((object[])userExplore ["el_ids"]) [i] != null)
					{
						if (((object[])(((object[])userExplore ["el_ids"]) [i])) [1].ToString () == "1"||(int)userExplore ["index"] == i)
						{
							if (boxid != null) {
								if (BiDaXiao (((object[])(((object[])userExplore ["el_ids"]) [i])) [0].ToString (), boxid) != boxid) {
									_i = i;
								}
								boxid = BiDaXiao (((object[])(((object[])userExplore ["el_ids"]) [i])) [0].ToString (), boxid);
							} else {
								_i = i;
								boxid = ((object[])(((object[])userExplore ["el_ids"]) [i])) [0].ToString ();
							}
						}
					}
				}
				if (_i != -1) {
					returnData.Add ("type", 3);
					cfg = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [((object[])(((object[])userExplore ["el_ids"]) [_i])) [0].ToString ()]);
					returnData.Add ("icon", cfg ["icon"].ToString ());
					returnData.Add ("endTime", -1);
					return returnData;
				}
			}
			else
			{
				returnData.Add ("type", 2);
				cfg = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [((object[])(((object[])userExplore ["el_ids"]) [(int)userExplore ["index"]])) [0].ToString ()]);
				returnData.Add ("icon", cfg ["icon"].ToString ());
				returnData.Add ("endTime", (overTime.Ticks - Tools.GetSystemTicks ()));
				return returnData;
			}
		}
		_i = -1;
		for (int i = 0; i < ((object[])userExplore ["el_ids"]).Length; i++)
		{
			if (((object[])userExplore ["el_ids"]) [i] != null)
			{
				if (((object[])(((object[])userExplore ["el_ids"]) [i])) [1].ToString () == "1")
				{
					if (boxid != null) {
						if (BiDaXiao (((object[])(((object[])userExplore ["el_ids"]) [i])) [0].ToString (), boxid) != boxid) {
							_i = i;
						}
						boxid = BiDaXiao (((object[])(((object[])userExplore ["el_ids"]) [i])) [0].ToString (), boxid);
					} else {
						_i = i;
						boxid = ((object[])(((object[])userExplore ["el_ids"]) [i])) [0].ToString ();
					}
				}
			}
		}
		if (_i != -1) {
			returnData.Add ("type", 3);
			cfg = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [((object[])(((object[])userExplore ["el_ids"]) [_i])) [0].ToString ()]);
			returnData.Add ("icon", cfg ["icon"].ToString ());
			returnData.Add ("endTime", -1);
			return returnData;
		}
		_i = -1;
		for (int i = 0; i < ((object[])userExplore ["el_ids"]).Length; i++)
		{
			if (((object[])userExplore ["el_ids"]) [i] != null)
			{
				if (boxid != null) {
					if (BiDaXiao (((object[])(((object[])userExplore ["el_ids"]) [i])) [0].ToString (), boxid) != boxid) {
						_i = i;
					}
					boxid = BiDaXiao (((object[])(((object[])userExplore ["el_ids"]) [i])) [0].ToString (), boxid);
				} else {
					_i = i;
					boxid = ((object[])(((object[])userExplore ["el_ids"]) [i])) [0].ToString ();
				}	
			}
		}
		if (_i != -1) {
			returnData.Add ("type", 1);
			cfg = (Dictionary<string,object>)(((Dictionary<string,object>)ExpCfg ["box"]) [((object[])(((object[])userExplore ["el_ids"]) [_i])) [0].ToString ()]);
			returnData.Add ("icon", cfg ["icon"].ToString ());
			returnData.Add ("endTime", -1);
			return returnData;
		}

		if (CanFushBox ())
		{
			returnData.Add ("type", -1);
			returnData.Add ("icon", "box001");
			returnData.Add ("endTime", -1);
			return returnData;
		}
		else
		{
			returnData.Add ("type", 0);
			returnData.Add ("endTime", -1);
			return returnData;
		}
		return returnData;
	}

	private bool CanFushBox ()
	{
		int cfg = (int)(((object[])DataManager.inst.systemSimple ["el_score"]) [2]);
		if (userModelr.el_score >= cfg)
		{
			return true;
		}
		return false;
	}

	private void frushiTime ()
	{
		TimerManager.inst.Add (60, 1, onTimersFun);
	}

	public void BtnExplore_Click (EventContext ev, OnCall call)
	{
		DOTween.Kill (this);
		this.call = call;
		this.touchable = false;
		this.visible = true;
		this.TweenMoveX (ModelManager.inst.gameModel.width - 209, 0.5f).OnComplete (() =>
		{
			TimerManager.inst.Add (60, 1, onTimersFun);
			this.touchable = true;
		});
	}

	private void OnOverFunction ()
	{
		DOTween.Kill (this);
		this.TweenMoveX (ModelManager.inst.gameModel.width, 0.5f).OnComplete (() =>
		{
			if (this.call != null)
			{
				call ();
			}
			this.call = null;
			this.visible = false;
			Tools.Clear (boxBoom);
		});
	}

	private void onTimersFun (float time)
	{
		if (ModelManager.inst.alertModel.isTip&&ModelManager.inst.alertModel.type == Config.TOUCH_PROGRESSBAR) {
			OnMainExploreFush ();
			Log.debug ("aaa");
			return;
		}
		DOTween.Kill (this);
		this.touchable = false;
		this.TweenMoveX (ModelManager.inst.gameModel.width, 1f).OnComplete (() =>
		{
			call ();
			this.call = null;
			this.visible = false;
			Tools.Clear (boxBoom);
		});
	}

	private void OnMainExploreFush (MainEvent e = null)
	{
		TimerManager.inst.Remove (onTimersFun);
		frushiTime ();
	}

	public override void Clear ()
	{
		TimerManager.inst.Remove (Time_Tick);
		ComGoldCoinExp.Elcount = 0;
		base.Clear ();
	}
}
