using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;
using DG.Tweening;

public class MediatorMain :BaseMediator
{
	private ModelUser userModel;
	private ModelRole roleModel;
	private ModelChat chatModel;
	private ModelFight fightModel;
	private ModelGuide guideModel;
	
	private GButton btn_Match;
	private GButton btn_Free;
	private GButton btn_Team;
	private GButton btn_Cancel;
	private GTextField name;
	private GTextField lv;
	private GButton btn_Red;
	private GButton btn_Menu1;
	private GButton btn_Menu2;
	private GButton btn_Menu3;
	private GButton btn_Menu4;
	private GButton head;
	//private GButton btn_Mail;
	private GButton btn_Menu5;
	//private GTextField l_RedTime;
	private GButton btn_Effort;
	private GButton btn_Code;
	private GButton btn_Explore;
	private MediatorExplore explore;
//	private ComProgressBar bar;
	private GProgressBar bar;
	private GTextField barTxt;
	private GameObject countdown;
	private Controller c1;
	private GTextField txt1;
	private GTextField txt2;
	private GLoader rankImg;
	private GTextField rankScore;
	private GComponent gold;
	private GComponent coin;
	private GGroup redGroup;
	private GGroup teamGroup;
	private GameObject addd;
	private GGraph mask;
    private GButton btn_Ctr;
    private GButton btn_test;
	private GGroup userInfo;
	private GGraph gift_box;
	private bool guide_isOpen;
    //	private int oldType = -12;
    public override void Init ()
	{
		base.Init ();
		this.Create (Config.SCENE_MAIN, true);
        Tools.setRankData();
		//SoundManager.inst.PlayMusic (Config.SOUND_MAIN);
		userModel = ModelManager.inst.userModel;
		roleModel = ModelManager.inst.roleModel;
		chatModel = ModelManager.inst.chatModel;
		fightModel = ModelManager.inst.fightModel;
		guideModel = ModelManager.inst.guideModel;
        shareModel=ModelManager.inst.shareModel;
		if (userModel.isBGM) {
			SoundManager.inst.PlayMusic (Config.SOUND_MAIN);
		}
		guide_isOpen = ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_GUIDE);
        //addrDiction = (Dictionary<string, object>)DataManager.inst.systemSimple["area_config"];
        //ViewManager.inst.ShowView<MediatorCustom>();//客服
        //ViewManager.inst.ShowView<MediatorNotice>();//通知
        btn_Ctr = this.GetChild("n108").asButton;
        notice = this.GetChild ("n96").asButton;
		notice.text = Tools.GetMessageById ("14061");
		notice.onClick.Add (() => {
            ViewManager.inst.ShowView<MediatorNotice> ();//公告
            userModel.Remove_Notice(notice);
        });
	    mail = this.GetChild ("n97").asButton;
		mail.text = Tools.GetMessageById ("14059");
		mail.onClick.Add (() => {
			ViewManager.inst.ShowView<MediatorGift> ();//邮件
		});
	    cust = this.GetChild ("n98").asButton;
		cust.text = Tools.GetMessageById ("21008");
		cust.onClick.Add (() => {
			ViewManager.inst.ShowView<MediatorCustom> ();//客服
		});
		//GButton red = this.GetChild ("n115").asButton;//红包
		//red.text = Tools.GetMessageById ("12026");
//		red.onClick.Add (() => {
//			BtnRed_Click();
//		});
		string imgUrl = "Flash:Mc_RedbagCoin";
        //GMovieClip g = Tools.GetComponent (imgUrl).asMovieClip;
        //g.scale = new Vector2 (1.1f, 1.1f);
        //red.AddChild (g);
        //g.visible = false;
        //g.name = "mc";
        //g.x = 19;
        //g.y = 18;

        //		Debug.Log ("--send server log---");
		btn_Match = this.GetChild ("n4").asButton;
        btn_Match.y = GRoot.inst.height * (float)0.45;
        //		btn_Match.GetChild ("n1").asGraph.shape.DrawRect//.displayObject.gameObject.GetComponent<MeshRenderer> ().material.color = Color.Lerp (Color.blue, Color.green, 0.2f);
        //		new Color(255-2*i, i, 2*i);
        btn_Free = this.GetChild ("n5").asButton;
		btn_Team = this.GetChild ("n64").asButton;
		btn_Cancel = this.GetChild ("n71").asButton;
		name = this.GetChild ("n1").asTextField;
//		lv = view.GetChild ("n2").asTextField;
		btn_Red = this.GetChild ("n115").asButton;
        btn_Red.text = Tools.GetMessageById("12026");
        btn_Menu1 = this.GetChild ("n6").asButton;
		btn_Menu2 = this.GetChild ("n7").asButton;
		btn_Menu3 = this.GetChild ("n8").asButton;
		btn_Menu4 = this.GetChild ("n9").asButton;
		head = this.GetChild ("n25").asButton;
        GButton clickBG = this.GetChild("n122").asButton;
//		btn_Mail = this.GetChild ("n10").asButton;
		btn_Menu5 = this.GetChild ("n11").asButton;
		//	l_RedTime = this.GetChild ("n47").asTextField;
		btn_Effort = this.GetChild ("n44").asButton;
		btn_Explore = this.GetChild ("n50").asButton;
		btn_Explore.visible = false;
		bar = this.GetChild ("n54").asProgress;// as ComProgressBar;
		barTxt = this.GetChild ("n54").asCom.GetChild ("title").asTextField;
		//	bar.GetChild ("bar").asCom.GetChild("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shengji2");
		btn_Code = this.GetChild ("n49").asButton;//奖励
		c1 = this.GetController ("c1");
		txt1 = this.GetChild ("n72").asTextField;
		txt2 = this.GetChild ("n73").asTextField;
		rankImg = this.GetChild ("n64").asCom.GetChild ("n5").asLoader;
		rankImg.url = userModel.GetRankImg (userModel.rank_score);
//		this.GetChild ("n79").visible = false;
		rankScore = this.GetChild ("n79").asCom.GetChild ("n2").asTextField;
		redGroup = this.GetChild ("n77").asGroup;
//		teamGroup = this.GetChild ("n78").asGroup;
		coin = this.GetChild ("n56").asCom;
		coin.visible = false;
		gold = this.GetChild ("n55").asCom;
		gold.visible = false;
		mask = this.GetChild ("n84").asGraph;

		//
		ViewManager.SetWidthHeight (mask);
		mask.visible = false;
		//
		userInfo = this.GetChild("n114").asGroup;
		gift_box =this.GetChild ("n124").asGraph;
		//
		userInfo.visible = false;
		btn_Ctr.visible = false;
		btn_Red.visible = false;
		gift_box.visible = false;
		//
        if(userModel.isShowText) {//满卡之后 多余的卡牌数转化成金币提示消息
            ViewManager.inst.ShowText(Tools.GetMessageById("33212"));
            userModel.isShowText = false;
        }

		//   head.GetChild("n0").asButton.GetChild("n2").asImage.visible = false;//隐藏头像白圈
		this.GetChild("n85").visible = false;
		this.GetChild("n86").visible = false;
//		this.GetChild ("n85").asButton.onClick.Add (() => {			
//			userModel.SetGuide (1, (VoHttp vo) => {
//				Dictionary<string,object> temp = new Dictionary<string, object> ();
//				temp ["data"] = vo.data;
//				temp ["type"] = ModelFight.FIGHT_MATCHGUIDE;
//				this.DispatchGlobalEventWith (MainEvent.FIGHT_RESULT, temp);
//			});
//		});
//		this.GetChild ("n86").asButton.onClick.Add (() => {
//			userModel.SetGuide (4, (VoHttp vo) => {
//				Dictionary<string,object> temp = new Dictionary<string, object> ();
//				temp ["data"] = vo.data;
//				temp ["type"] = ModelFight.FIGHT_MATCHGUIDE;
//				this.DispatchGlobalEventWith (MainEvent.FIGHT_RESULT, temp);
//			});
//		});

		ViewManager.SetWidthHeight (this.GetChild ("n0"));

		//==============================================================unlock
		userModel.GetUnlcok (Config.UNLOCK_MATCH, btn_Match);
		userModel.GetUnlcok (Config.UNLOCK_TEAMMATCH, btn_Team);//抢萝卜
		userModel.GetUnlcok (Config.UNLOCK_FREEMATCH1, btn_Free);//自定义

//		userModel.GetUnlcok (Config.UNLOCK_ASSET, coin);
//		userModel.GetUnlcok (Config.UNLOCK_ASSET, gold);
		userModel.GetUnlcok (Config.UNLOCK_CARD, btn_Menu1);
		userModel.GetUnlcok (Config.UNLOCK_FRIEND, btn_Menu2);
		userModel.GetUnlcok (Config.UNLOCK_GUILD, btn_Menu4);
		userModel.GetUnlcok (Config.UNLOCK_SHOP, btn_Menu3);
		userModel.GetUnlcok (Config.UNLOCK_CODE, btn_Code);
		userModel.GetUnlcok (Config.UNLOCK_MAIL, mail);
		userModel.GetUnlcok (Config.UNLOCK_RANK, btn_Menu5);
		userModel.GetUnlcok (Config.UNLOCK_EXPLORE, btn_Explore);
		userModel.GetUnlcok (Config.UNLOCK_EFFORT, btn_Effort);
		userModel.GetUnlcok (Config.UNLOCK_REDBAG, redGroup);
		userModel.GetUnlcok (Config.UNLOCK_HEAD, head);
        //List<GComponent> gs = new List<GComponent> ();
        //int[] pos = new int[3]{ 13, 88, 163 };
        //if (btn_Code.visible)
        //	gs.Add (btn_Code);
        //if (btn_Mail.visible)
        //	gs.Add (btn_Mail);
        //if (btn_Friend.visible)
        //	gs.Add (btn_Friend);
        //for (int i = 0; i < gs.Count; i++)
        //	gs [i].y = pos [i];
        //==============================================================

        GGroup gg = this.GetChild ("n87").asGroup;
		if (!btn_Team.visible) {//不显示抢萝卜按钮时，快速比赛按钮变高。
			btn_Match.y -= 80;
			gg.y -= 80;
		}
		btn_Match.onClick.Add (BtnMatch_Click);
		btn_Free.onClick.Add (BtnFree_Click);
		btn_Team.onClick.Add (BtnTeam_Click);
		btn_Cancel.onClick.Add (BtnCancel_Click);
		btn_Red.onClick.Add (BtnRed_Click);
//		btn_Mail.onClick.Add (BtnMail_Click);
		btn_Menu5.onClick.Add (BtnRank_Click);
        head.onClick.Add (BtnHead_Click);
        clickBG.onClick.Add(BtnHead_Click);
		btn_Menu1.onClick.Add (BtnCard_Click);
		btn_Menu2.onClick.Add (BtnSicial_Click);
		btn_Menu3.onClick.Add (BtnShop_Click);
		btn_Menu4.onClick.Add (BtnGuild_Click);
		btn_Effort.onClick.Add (BtnIcon_Click);
		btn_Explore.onClick.Add (BtnExplore_Click);
        btn_Ctr.onClick.Add(()=> { BtnCtrClick(); });
//		btn_Code.visible = userModel.IsSharedWeiXin ();
		btn_Code.onClick.Add (BtnCode_Click);
		//
		btn_Menu1.text = Tools.GetMessageById ("14030");
		btn_Menu2.text = Tools.GetMessageById ("14031");
		btn_Menu3.text = Tools.GetMessageById ("14033");
		btn_Menu4.text = Tools.GetMessageById ("14032");
//		rankImg.url = userModel.GetRankImg (userModel.rank_score);
		rankScore.text = userModel.rank_score.ToString ();
		btn_Match.text = Tools.GetMessageById ("14017");
		btn_Free.text = Tools.GetMessageById ("14023");
		btn_Team.text = Tools.GetMessageById ("14024");
		btn_Cancel.text = Tools.GetMessageById ("14025");
		btn_Menu5.text = Tools.GetMessageById ("14058");
		btn_Code.text = Tools.GetMessageById ("14060");
        //		bar.skin = ComProgressBar.BAR11;

        bar.value = userModel.exp;
		bar.max = userModel.GetExpMax (userModel.lv);
		if (userModel.IsMaxLv()) {
			bar.GetChild ("bar").asCom.GetChild ("n0").visible = false;//asGraph.color = FairyGUI.Utils.ToolSet.ConvertFromHtmlColor("#FDFB8C"); //#FDFB8C
			bar.GetChild("title").asTextField.color = FairyGUI.Utils.ToolSet.ConvertFromHtmlColor("#848484");
		}
		name.text = userModel.uname;
		ViewManager.inst.AddTouchTip (Config.TOUCH_PROGRESSBAR, bar, Config.TOUCH_COMEXP);//经验条浮窗
		ViewManager.inst.AddTouchTip (Config.TOUCH_RANKSCORE, this.GetChild ("n79").asCom, Config.ASSET_RANKSCORE);
		//addd = EffectManager.inst.AddEffect (Config.EFFECT_HONGBAO, "new", this.GetChild ("n124").asGraph,null,false,2000,"gifticon/");
              
        //		GameObjectScaler.Scale (addd, 0.45f);
        //		addd.transform.localScale *= 0.45f;
        if (userModel.GetRedTime () != -2) {
            //	EffectManager.inst.StopAnimation (addd);
            EffectManager.inst.AddEffect(Config.EFFECT_HONGBAO, "stand", this.GetChild("n124").asGraph, null, false, 620);
			//g.visible = false;
			//red.GetChild ("n6").asImage.visible = true;
		} else {
            //	EffectManager.inst.PlayAnimation (addd);
            EffectManager.inst.AddEffect(Config.EFFECT_HONGBAO, "new", this.GetChild("n124").asGraph, null, false, 620, "gifticon/");
           // g.visible = true;
			//red.GetChild ("n6").asImage.visible = false;
		}
		head.GetChild ("n2").text = userModel.lv.ToString ();
		this.GetChild ("n118").asTextField.text = Tools.GetMessageById ("23013");
		Tools.SetLoaderButtonUrl (head.GetChild ("n0").asButton, ModelUser.GetHeadUrl (userModel.head ["use"].ToString ()));
//		Tools.SetLoaderButtonUrl (head.GetChild ("n0").asButton, ModelUser.GetHeadUrl ("http://q.qlogo.cn/qqapp/101347709/367F8C9525DC8690EC9DF1578F6714A1/40"));
//		Dictionary<string, object> data = new Dictionary<string, object>();
//		data["head_key"] = "http://q.qlogo.cn/qqapp/101347709/367F8C9525DC8690EC9DF1578F6714A1/40";
//		NetHttp.inst.Send (NetBase.HTTP_CHOOSE_HEAD, data, (VoHttp vo) => {
//		});

		string str = Tools.GetEffortBuildID ("0" + userModel.effort_lv);
//		btn_Effort.GetChild ("n2").asGraph.alpha = 0;
		GameObject go = EffectManager.inst.AddEffect (str, str, btn_Effort.GetChild ("n2").asGraph, null, false, 40, null, true);
//        Debug.Log(go.transform.localScale);
        //==============================================================红包
        //		DOTween.Kill (btn_Red);
//        if (userModel.GetRedTime () > 0) {
//			TimerManager.inst.Add (1f, 0, Time_Tick);
//			Time_Tick (0f);

//		}
		//==============================================================

		//==============================================================探险
		explore = new MediatorExplore ();
		this.AddChild (explore.group);
		Dictionary<string,object> exDa = explore.MainShowBtnType ();
		if (exDa.ContainsKey ("type")) {
			if ((int)exDa ["type"] == 2) {
				TimerManager.inst.Add (1f, 0, Time_Explore);
			}
		}
		Time_Explore (0f);
		//==============================================================

		AddGlobalListener (MainEvent.WEIXIN_UPDATE, ViewUpdate);
		AddGlobalListener (MainEvent.GUIDE_UPDATE_OVER, onGUIDE_UPDATE_OVER);
		AddGlobalListener (MainEvent.USER_UPDATE, USER_UPDATE);
		AddGlobalListener (MainEvent.USER_UPDATE_UI, USER_UPDATE_UI);
		AddGlobalListener (MainEvent.RED_GIFT, OnUpdateRedHandler);
		AddGlobalListener (MainEvent.JUMPEL_OVER, OnJumpElOver);
		//AddGlobalListener(MainEvent.FIGHT_DATA_END, OnfightDataEnd);
		AddGlobalListener (MainEvent.GUIDE_UPDATE_OK,CheckGuide);
		AddGlobalListener (MainEvent.RED_UPDATE, RED_UPDATE);
		AddGlobalListener (MainEvent.RED_CHATUPDATE, RED_CHATUPDATE);
		AddGlobalListener (MainEvent.EXPLORE_UNLOCK, OnUnlockHandler);
		//
        RED_CHATUPDATE (null);
		OnfightDataEnd (null);
//		EffectManager.inst.AddEffect (Config.EFFECT_BG1, "obj01", this.GetChild ("n67").asGraph, null, false, 35);
//		EffectManager.inst.AddEffect (Config.EFFECT_BG2, "obj02", this.GetChild ("n68").asGraph, null, false, 45);
//		EffectManager.inst.AddEffect (Config.EFFECT_BG3, "obj03", this.GetChild ("n69").asGraph, null, false, 48);
//		EffectManager.inst.AddEffect (Config.EFFECT_BG4, "obj04", this.GetChild ("n70").asGraph, null, false, 80);//EFFECT_BIGEARTH
//		GameObject earth = EffectManager.inst.AddEffect (Config.EFFECT_BIGEARTH, null, this.GetChild ("n70").asGraph, null, false, 70);//EFFECT_BIGEARTH
//		earth.transform.position = new Vector3 (-180, -750, 2000);

		NetSocket.inst.AddListener (NetBase.SOCKET_MATCHJOIN, (VoSocket vo) => {	
			mask.visible = true;
			fightModel.fightType = ModelFight.FIGHT_MATCH;
			TimerManager.inst.Remove (Match_Tick);
			if (vo.data is IDictionary) {
				Dictionary<string,object> da = (Dictionary<string,object>)vo.data;
				fightModel.roomId = da ["room_id"].ToString ();
				fightModel.groupId = da ["group_id"].ToString ();
				fightModel.team2 = (object[])da ["team"];

//				ViewManager.inst.ShowScene (new MediatorFightWorld ());
//				Debug.LogError("this is true fight SOCKET_MATCHJOIN");
				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.START_FIGHT, da));
			} else {				
				fightModel.matchTime = 0;
				fightModel.preTime = Convert.ToInt32 (vo.data);
				c1.selectedIndex = 1;
				txt1.text = Tools.GetMessageById ("14026", new string[]{ fightModel.preTime.ToString () });
				txt2.text = Tools.GetMessageById ("14027", new string[]{ fightModel.matchTime.ToString () });
				TimerManager.inst.Add (1f, 0, Match_Tick);
//				ViewManager.inst.ShowText (Tools.GetMessageById ("25013"));
			}
		});
		#if UNITY_IOS && !UNITY_EDITOR_OSX
		PlatForm.inst.GetSdk().Call(Ex_Local.CALL_RE_TOUCH3D, touch3d_back);

		if(!PlatForm.inst.touch3dGot){
			PlatForm.inst.touch3dGot = true;
			PlatForm.inst.GetSdk().Call(Ex_Local.CALL_RE_TOUCH3D_GET, touch3d_back);
		}
		#endif

        if(userModel.isSettle) {
            ViewManager.inst.ShowAlert(Tools.GetMessageById("25066"), (int index) => {
                if(index == 1)
                    base.Close();
            });
            userModel.isSettle = false;
        }
		//第一次判断 用户名称 是否为  null
		if (userModel.unameTrue == null) {
//			ViewManager.inst.ShowView<MediatorChangeNameGuide> ();
//			Debug.LogError (userModel.unameTrue == null);
		} else {
			Clip_guide_over ();
		}

		roleModel.tab_CurSelect1 = 0;
		roleModel.tab_CurSelect2 = 0;
		roleModel.tab_CurSelect3 = 0;
		roleModel.tab_Select1 = -1;
		roleModel.tab_Select2 = -1;
		roleModel.tab_Select3 = -1;
		//
		//
		this.GetController("c1").onChanged.Add(()=>{
//			Debug.LogError("c1 change");
			if(this.GetController("c1").selectedIndex == 0){
				OnUpdateRedHandler();
			}
		});
		OnUpdateRedHandler();
		//
		CheckGuide ();
    }

	#if UNITY_IOS
	private void touch3d_back(object type){
		string t3dType = (string)type;
		if(t3dType != 	null && t3dType!="" && t3dType.Length>0){
			BtnMail_Click ();
		};
		PlatForm.inst.GetSdk ().CallReturn (Ex_Local.CALL_RE_TOUCH3D_SET);
	}
	#endif

	/**
	 * 赛季结束 判断
	 */
	private void SeasonOver(){
		NetHttp.inst.Send (NetBase.HTTP_GET_SEASON_DATA, "", (VoHttp vo) => {
			//			Debug.Log(vo.data);
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			if (data.Count <= 0)
				return;
			//			Debug.Log(data["-1"]);
			int max = 0;
			int curr = 0;
			if (data.ContainsKey ("-1")) {
				if (data ["-1"] is bool) {

					//					Debug.Log(data["-1"]);
					if ((bool)data ["-1"]) {
						data.Remove ("-1");
						//
						foreach (string i in data.Keys) {

							curr = Convert.ToInt32 (i);
							if (curr > max) {
								max = curr;
							}
						}
						object[] s = (object[])data [max.ToString ()];
						MediatorFightSeason.data = s;
						ViewManager.inst.ShowView<MediatorFightSeason> ();
					}
				}
			}
		});
	}
	private void CheckGuide (MainEvent e = null)
	{	
//		bool c020 = userModel.Guide_card_lv ();
//		bool c603 = userModel.Guide_card_had ("C603");
		if(e!=null){
			if (userModel.unameTrue == null) {
				GuideManager.inst.guide11 = 3;
				GuideManager.inst.guide12 = 0;
				ViewManager.inst.ShowView<MediatorChangeNameGuide> ();
			} else {
				Clip_guide_over ();
			}
			return;
		}
		if (GuideManager.inst.Check ("1:0")) {
			GuideManager.inst.Show (this);
		} else if (GuideManager.inst.Check ("2:0")) {
			GuideManager.inst.Next ();
			GuideManager.inst.Show (this);
		} else if (GuideManager.inst.Check ("3:0")) {
//			if (userModel.unameTrue == null) {
			ViewManager.inst.ShowView<MediatorChangeNameGuide> ();
//			} else {
//				Clip_guide_over ();
//			}
		}
		else if(GuideManager.inst.Check ("4:0") || !guide_isOpen){
			if (userModel.unameTrue == null) {
				ViewManager.inst.ShowView<MediatorChangeNameGuide> ();
			} else {
				//只有 全部过完新手引导的 用户 才能开始其他功能
				NetHttp.inst.Send (NetBase.HTTP_GETEFFORT, "", (VoHttp vo) => {
					userModel.UpdateData (vo.data);
					//			CheckGuide ();
					RED_UPDATE (null);
					CheckGuide_EXIST();

				});
				Clip_guide_over ();
			}
		}
	}
	private void CheckGuide_EXIST(){
		if (userModel.effort_lv<=1 && ModelManager.inst.guideModel.CheckEffort () == 2) {//GuideManager.inst.Check ("100:0") && 
			if (GuideManager.inst.Check ("100:0")) {
				GuideManager.inst.Show (this);
			}
		}
		SeasonOver();
	}
//	public static bool Clip_guide_over_tag = false;
	private void onGUIDE_UPDATE_OVER(MainEvent e){
		DataManager.Clip_guide_over_tag = true;
		Clip_guide_over ();
		CheckGuide ();
	}
	private void Clip_guide_over(){
		userInfo.visible = true;
		btn_Ctr.visible = true;
		btn_Red.visible = true;
		gift_box.visible = true;
		if (!DataManager.Clip_guide_over_tag) {
			return;
		}
		DataManager.Clip_guide_over_tag = false;
		//
		float ax = userInfo.x;
		userInfo.x = -userInfo.width;
		userInfo.TweenMoveX (ax, 0.5f);
		//
		float bx = btn_Ctr.x;
		btn_Ctr.x = GRoot.inst.width + 50;
		btn_Ctr.TweenMoveX (bx, 0.5f).OnComplete(()=>{
			OnUpdateRedHandler();
		});
	}
	private void OnUnlockHandler (MainEvent e)
	{//动画效果
//		if (userModel.GetRedTime () > 0)
//		{
			TimerManager.inst.Remove (Time_Tick);
			TimerManager.inst.Add (1f, 0, Time_Tick);
			Time_Tick (0f);
//		}
	}

	private void OnJumpElOver (MainEvent e)
	{
		userModel.UpdateData (fightModel.fightDatatTemp);
		explore.UpElScore ();
		Dictionary<string, object> data = (Dictionary<string, object>)fightModel.fightDatatTemp ["gifts"];
		int pr_el_score = Convert.ToInt32 (fightModel.el_score);
		int el_score = (int)data ["el_score"];
		int rankScoreEnd = (int)((object[])DataManager.inst.systemSimple ["el_score"]) [1];
		if (pr_el_score + el_score > rankScoreEnd)
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("19966"));
		}
		fightModel.el_score = null;
		fightModel.fightDatatTemp = null;

	}

	private void OnfightDataEnd (MainEvent e)
	{
		if (fightModel.fightData != null && fightModel.el_score != null)
		{
			if (fightModel.fightData ["statementType"].Equals (ModelFight.FIGHT_MATCHTEAM) || fightModel.fightData ["statementType"].Equals (ModelFight.FIGHT_MATCH))
			{
				Dictionary<string, object> data = (Dictionary<string, object>)fightModel.fightData ["gifts"];
				int pr_el_score = Convert.ToInt32 (fightModel.el_score);
                if (data.ContainsKey("el_score"))
                {
                    int el_score = (int)data["el_score"];
                    int rankScoreEnd = (int)((object[])DataManager.inst.systemSimple["el_score"])[1];
                    if (pr_el_score >= rankScoreEnd)
                    {
                        userModel.UpdateData(fightModel.fightData);
                        fightModel.fightData = null;
                        fightModel.el_score = null;
                        ViewManager.inst.ShowText(Tools.GetMessageById("19966"));
                    }
                    else
                    {
                        btn_Explore.onClick.Call();
                        Vector2 v = new Vector2();
                        switch (fightModel.fightData["statementType"].ToString())
                        {
                            case ModelFight.FIGHT_MATCHTEAM:
                                v = new Vector2(btn_Team.x + 72, btn_Team.y);
                                break;
                            case ModelFight.FIGHT_MATCH:
                                v = new Vector2(btn_Match.x + 124, btn_Match.y);
                                break;
                        }
                        if (pr_el_score + el_score > rankScoreEnd)
                        {
                            ViewManager.inst.ShowGetElScore(rankScoreEnd - pr_el_score, v);

                        }
                        else
                        {
                            ViewManager.inst.ShowGetElScore(el_score, v);
                        }
                        fightModel.fightDatatTemp = (Dictionary<string, object>)Tools.Clone(fightModel.fightData);
                        fightModel.fightData = null;
                    }
                }
                else
                {
                    fightModel.fightData = null;
                    fightModel.el_score = null;
                }
				
			}
		}
	}

	private void RED_UPDATE (MainEvent e)
	{
		if(this.view == null || (this.view!=null && this.view.parent==null)){
			return;
		}
		///////////////////////////////////////////////////////商城
		ModelCard cardModel = ModelManager.inst.cardModel;
		if (cardModel.CanFrush () || LocalStore.GetLocal (LocalStore.LOCAL_SHOPRED) == "1") 
		{
			LocalStore.SetLocal (LocalStore.LOCAL_SHOPRED, "1");
			userModel.Add_Notice (btn_Menu3, new Vector2 (68, -3));
		}
		else
			userModel.Remove_Notice (btn_Menu3);
		///////////////////////////////////////////////////////卡牌
		int num = cardModel.GetNewCardNum ();
		int num2 = cardModel.GetLevelUpCardNum ();
		int num3 = cardModel.GetShipNum ();
		if (num != 0)
		{
			userModel.Add_Notice (btn_Menu1, new Vector2 (68, -3), num);
		}
		else if (num3 != 0)
		{
			userModel.Add_Notice (btn_Menu1, new Vector2 (68, -3), num3);
		}
		else if (num2 != 0)
		{
			userModel.Add_Notice (btn_Menu1, new Vector2 (68, -3), num2, true);
		}
		else
		{
			userModel.Remove_Notice (btn_Menu1);
		}
		///////////////////////////////////////////////////////红包
		if ((int)userModel.records ["redbag_coin"] > (int)DataManager.inst.redbag ["redpoint"])
		{
			userModel.Add_Notice (btn_Red, new Vector2 (45, 0),0,false,false,true);
		}
		else
		{
			userModel.Remove_Notice (btn_Red);
		}
		///////////////////////////////////////////////////////成就

        //新消息 n82
		if (userModel.GetOverEffortNum () != 0 && !DataManager.inst.EffortIsMax(userModel.effort_lv))
		{
			this.GetChild ("n82").asGroup.visible = true;
			this.GetChild ("n82").y = 193;
			EffectManager.inst.ReTweenJump (this.GetChild ("n82"));
			EffectManager.inst.EffortMainFuHaoJump (this.GetChild ("n82"));
		}
		else
		{
			this.GetChild ("n82").asGroup.visible = false;
			EffectManager.inst.ReTweenJump (this.GetChild ("n82"));
		}

        //奖励
        if (shareModel.isShareRed())
        {
            userModel.Add_Notice(btn_Code, new Vector2(40, 0),0,false,false,true);
        }
        else
        {
            userModel.Remove_Notice(btn_Code);
        }


        //客服
        int kefu = userModel.Get_NoticeState(ModelUser.RED_BUGMSG);
        if (kefu > 0)
        {
            userModel.Add_Notice(cust, new UnityEngine.Vector2(40, 0),0,false,false,true);
        }
        else
        {
            userModel.Remove_Notice(cust);

        }

        //邮件
        int count = userModel.Get_NoticeState(ModelUser.RED_GIFTMSG);
        if (count > 0)
        {
            userModel.Add_Notice(mail, new UnityEngine.Vector2(40, 0), count, false,false,true);
        }
        else
        {
            userModel.Remove_Notice(mail);
        }
        //公告
        if (ModelManager.inst.mailModel.Get_RedMail())
        {
            userModel.Add_Notice(notice, new UnityEngine.Vector2(40, 0), count, false,false,true);
        }
        else
        {
            userModel.Remove_Notice(notice);
        }
        //单向关注
        int follow_count = userModel.Get_NoticeState (ModelUser.RED_FOLLOW);
		int fans_count = userModel.Get_NoticeState (ModelUser.RED_FANS);
		if (fans_count > 0 || follow_count > 0)
		{
			userModel.Add_Notice (btn_Menu2, new UnityEngine.Vector2 (68, -3), fans_count + follow_count, false);
		}
		else
		{
			userModel.Remove_Notice (btn_Menu2);
		}
        //右上角开关
        if((kefu + count > 0 || ModelManager.inst.mailModel.Get_RedMail())|| shareModel.isShareRed()) {
            userModel.Add_Notice(btn_Ctr, new UnityEngine.Vector2(42, 0), 0, false, false, true);
        }else {
            userModel.Remove_Notice(btn_Ctr);
        }
        Time_Explore (1f);
	}
    
    private void BtnCtrClick(MainEvent e=null) {
        if(btn_Ctr.selected) {
            if(userModel.Get_NoticeState(ModelUser.RED_GIFTMSG)>0|| ModelManager.inst.mailModel.Get_RedMail()|| userModel.Get_NoticeState(ModelUser.RED_BUGMSG)>0||shareModel.isShareRed()) {
                ModelManager.inst.userModel.Add_Notice(btn_Ctr, new UnityEngine.Vector2(42, 0), 0, false, false, true);
            }
        } else {
            ModelManager.inst.userModel.Remove_Notice(btn_Ctr);
        }
    }

	private void RED_CHATUPDATE (MainEvent e)
	{
		if (this.view != null && this.view.parent != null) {
			///////////////////////////////////////////////////////公会
			int count = chatModel.GetChatRedCount ();
			if (count > 0)
				userModel.Add_Notice (btn_Menu4, new Vector2 (68, -3), count);
			else {
				userModel.Remove_Notice (btn_Menu4);
			}
		}
	}

	private void OnUpdateRedHandler (MainEvent e = null)
	{
		OnUnlockHandler(null);
	}

	private void USER_UPDATE_UI(MainEvent e){
		bar.TweenMoveX (bar.x, 0.5f).OnComplete (()=>{
			bar.InvalidateBatchingState(true);
		});
	}
	private void USER_UPDATE (MainEvent e)
	{		
		bar.value = userModel.exp;
		bar.max = userModel.GetExpMax (userModel.lv);
//		bar.text;
		barTxt.text = userModel.exp + "/" + userModel.GetExpMax (userModel.lv);

//		lv.text = Tools.GetMessageById ("14015", new string[]{ userModel.lv + "" });
		name.text = userModel.uname;
		head.GetChild ("n2").text = userModel.lv.ToString ();
		Tools.SetLoaderButtonUrl (head.GetChild ("n0").asButton, ModelUser.GetHeadUrl (userModel.head ["use"].ToString ()));
//		rankImg.url = userModel.GetRankImg (userModel.rank_score);
		rankScore.text = userModel.rank_score.ToString ();
        userModel.GetUnlcok(Config.UNLOCK_TEAMMATCH, btn_Team);//抢萝卜
        userModel.GetUnlcok(Config.UNLOCK_FREEMATCH1, btn_Free);//自定义
		if (btn_Team.visible) {
			btn_Match.y = GRoot.inst.height * (float)0.45;
		}
//		this.group.InvalidateBatchingState (true);
    }

	private void ViewUpdate (MainEvent e)
	{
		Dictionary<string,object> dd = (Dictionary<string,object>)e.data;
		if (dd ["tag"].Equals ("weixin"))
		{
			btn_Code.visible = false;
		}
	}

    //	public static string testTexture;

    private void BtnRank_Click ()
	{
        bool isOk = userModel.GetUnlcok(Config.UNLOCK_RANK, null, true);
        if (!isOk)
            return;
        roleModel.isSave = false;
        bool[] isSettle = Tools.RankTimer(0);
        if (isSettle.Length > 0)
        {
            if ((bool)isSettle[0])
            {
                ViewManager.inst.ShowAlert(Tools.GetMessageById("25066"), (int index) =>
                {
                    if (index == 1)
                        base.Close();
                });
            }
            else
            {
                ViewManager.inst.ShowScene<MediatorRankRoot>();

            }
        }

        //ViewManager.inst.ShowView<MediatorAd> ();
        //ViewManager.inst.ShowView<MediatorFightBoxStatement>();

    }

    private void BtnCode_Click ()
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_SHARD, null, true);
		if (!isOk)
			return;
		ViewManager.inst.ShowScene<MediatorShare> ();
		//		if(testTexture=="" || testTexture==null)return;
		//		GLoader gl =  this.view.GetChild ("n85").asLoader;
		//		Texture2D pic = new Texture2D(640, 960);
		//		byte[] data = System.Convert.FromBase64String(testTexture);
		//		pic.LoadImage(data);
		//		gl.texture = new NTexture (pic);
	}

	private void Time_Explore (float time)
	{
		Dictionary<string,object> exDa = explore.MainShowBtnType ();
		DOTween.Kill (btn_Explore.GetChild ("n7"));
		btn_Explore.GetChild ("n7").y = 26;
		if (exDa.ContainsKey ("type"))
		{
			if ((int)exDa ["type"] != 2)
			{
				TimerManager.inst.Remove (Time_Explore);
				if (countdown != null)
				{
					Tools.Clear (countdown);
				}
			}
			switch ((int)exDa ["type"])
			{
			case -1:
//				if (oldType != -1)
//				{
//					oldType = -1;
//				GameObjectScaler.Scale (EffectManager.inst.AddEffect (Tools.GetExploreBoxID ((string)exDa ["icon"]), "stand", btn_Explore.GetChild ("n6").asGraph), 0.5f);
				EffectManager.inst.AddEffect (Tools.GetExploreBoxID ((string)exDa ["icon"]), "stand", btn_Explore.GetChild ("n6").asGraph).transform.localScale *= 0.5f;
//				}
				btn_Explore.GetChild ("n8").asTextField.text = "";
				btn_Explore.GetChild ("n7").asLoader.url = Tools.GetResourceUrl ("Image:icon_sha");
				EffectManager.inst.EffortMainFuHaoJump (btn_Explore.GetChild ("n7"));
				break;
			case 0:
//				if (oldType != 0)
//				{
//					oldType = 0;
//				GameObjectScaler.Scale (EffectManager.inst.AddEffect (Config.EFFECT_BOX001, "stand", btn_Explore.GetChild ("n6").asGraph), 0.5f);
				EffectManager.inst.AddEffect (Config.EFFECT_BOX001, "stand", btn_Explore.GetChild ("n6").asGraph).transform.localScale *= 0.5f;
//				}
				btn_Explore.GetChild ("n8").asTextField.text = "";
				btn_Explore.GetChild ("n7").asLoader.url = "";
				break;
			case 1:
//				if (oldType != 1)
//				{
//					oldType = 1;
//				GameObjectScaler.Scale (EffectManager.inst.AddEffect (Tools.GetExploreBoxID ((string)exDa ["icon"]), "stand", btn_Explore.GetChild ("n6").asGraph), 0.5f);
				EffectManager.inst.AddEffect (Tools.GetExploreBoxID ((string)exDa ["icon"]), "stand", btn_Explore.GetChild ("n6").asGraph).transform.localScale *= 0.5f;
//				}
				btn_Explore.GetChild ("n8").asTextField.text = "";
				btn_Explore.GetChild ("n7").asLoader.url = Tools.GetResourceUrl ("Image:icon_sha");
				EffectManager.inst.EffortMainFuHaoJump (btn_Explore.GetChild ("n7"));
				break;
			case 2:
//				if (oldType != 2)
//				{
//					oldType = 2;
//				GameObjectScaler.Scale (EffectManager.inst.AddPrefab (Tools.GetExploreBoxID (exDa ["icon"].ToString ()), btn_Explore.GetChild ("n6").asGraph), 0.5f);
				EffectManager.inst.AddPrefab (Tools.GetExploreBoxID (exDa ["icon"].ToString ()), btn_Explore.GetChild ("n6").asGraph).transform.localScale *= 0.5f;
				countdown = EffectManager.inst.AddEffect (Config.EFFECT_COUNTDOWN, "countdown", btn_Explore.GetChild ("n10").asGraph);
//				}
				btn_Explore.GetChild ("n8").asTextField.text = Tools.TimeFormat2 (Convert.ToInt64 (exDa ["endTime"]));
				btn_Explore.GetChild ("n7").asLoader.url = "";//Tools.GetResourceUrl ("Image:icon_shalou");

				break;
			case 3:
//				if (oldType != 3)
//				{
//					oldType = 3;
//				GameObjectScaler.Scale (EffectManager.inst.AddEffect (Tools.GetExploreBoxID (exDa ["icon"].ToString ()), "ready", btn_Explore.GetChild ("n6").asGraph), 0.5f);
				EffectManager.inst.AddEffect (Tools.GetExploreBoxID (exDa ["icon"].ToString ()), "ready", btn_Explore.GetChild ("n6").asGraph).transform.localScale *= 0.5f;
//				}
				btn_Explore.GetChild ("n8").asTextField.text = "";
				btn_Explore.GetChild ("n7").asLoader.url = Tools.GetResourceUrl ("Image:icon_sha2");
				EffectManager.inst.EffortMainFuHaoJump (btn_Explore.GetChild ("n7"));
				break;
			}
		}
	}

	private bool dengyu = false;
    private object ww;
    private int country_tag;
    //private Dictionary<string, object> addrDiction;
    private GButton notice;
    private GButton mail;
    private GButton cust;
    private ModelShare shareModel;

    private void Time_Tick (float time = 0)
	{
//		Debug.LogError ("Time_Tick");
		GButton tex = this.GetChild ("n115").asButton;
		long intt = userModel.GetRedTime ();
		if (intt < 0)
		{
//			btn_Red.y = 147;
			tex.text = "";
			TimerManager.inst.Remove (Time_Tick);
			if (intt != -2 && dengyu != false)
			{
//				addd = EffectManager.inst.AddEffect (Config.EFFECT_MAILBOX, "stand", btn_Red.GetChild ("n2").asGraph);
//				addd.transform.localScale *= 0.45f;
//				EffectManager.inst.StopAnimation (addd);
				dengyu = false;

				//tex.GetChild("mc").visible = false;
				//tex.GetChild ("n6").asImage.visible = true;
			}
			else if (dengyu != true)
			{
//				EffectManager.inst.PlayAnimation (addd);
				dengyu = true;
			}
			if (intt == -2) {
				tex.text = Tools.GetMessageById ("12026");
                //tex.GetChild("mc").visible = true;
                // tex.GetChild ("n6").asImage.visible = false;
                EffectManager.inst.AddEffect(Config.EFFECT_HONGBAO, "new", this.GetChild("n124").asGraph, null, false, 620, "gifticon/");
               
			}
			return;
		}
//		DOTween.Kill (btn_Red);
		tex.text = Tools.TimeFormat (userModel.GetRedTime () * 10000, 0);
		if (tex.text == "00:00:00")
		{
			tex.text = "";
		}
	}
   
    public override void Clear ()
	{
		base.Clear ();
		//fightModel.fightData = null;
		//fightModel.el_score = null;
		if (fightModel.fightDatatTemp != null)
		{
			userModel.UpdateData (fightModel.fightDatatTemp);
			fightModel.el_score = null;
			fightModel.fightDatatTemp = null;
		}
		ComGoldCoinExp.Redcount = 0;

		TimerManager.inst.Remove (Match_Tick);
		TimerManager.inst.Remove (Time_Tick);
		TimerManager.inst.Remove (Time_Explore);
        
		RemoveGlobalListener (MainEvent.USER_UPDATE, USER_UPDATE);
		RemoveGlobalListener (MainEvent.USER_UPDATE_UI, USER_UPDATE_UI);
		RemoveGlobalListener (MainEvent.WEIXIN_UPDATE, ViewUpdate);
		RemoveGlobalListener (MainEvent.RED_GIFT, OnUpdateRedHandler);
		RemoveGlobalListener (MainEvent.RED_UPDATE, RED_UPDATE);
		RemoveGlobalListener (MainEvent.RED_CHATUPDATE, RED_CHATUPDATE);
		RemoveGlobalListener (MainEvent.GUIDE_UPDATE_OK,CheckGuide);
		RemoveGlobalListener (MainEvent.GUIDE_UPDATE_OVER, onGUIDE_UPDATE_OVER);
		RemoveGlobalListener (MainEvent.EXPLORE_UNLOCK, OnUnlockHandler);

		NetSocket.inst.RemoveListener (NetBase.SOCKET_MATCHJOIN);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_NEWTEAM);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_KILLTEAM);
       
	}

    //快速
    private void BtnMatch_Click() {
        //		Debug.LogError (ModelManager.inst.gameModel.time.ToLongTimeString());
        //		return;
        //		btn_Match.enabled = false;
        //		TimerManager.inst.Add (3f, 1, (float t) =>
        //		{
        //			btn_Match.enabled = true;
        //		});
        //		return;
        //		EffectManager.inst.SetFilterAdjustBrightness (this.group, 2f);
        //		return;
        //		ViewManager.inst.ShowAlert ("xxxxx");
        //		return;

        //dic[""]
        //Dictionary<string, object> da = new Dictionary<string, object>();
        //fightModel.roomId = "test";
        //				ViewManager.inst.ShowScene (new MediatorFightWorld ());
//		NetSocket.inst.Close();
//		return;
		if (GuideManager.inst.Check ("2:1"))
		{
			GuideManager.inst.Clear ();
		}
		if (GuideManager.inst.Check ("4:0") || !guide_isOpen) {//正式 快速游戏
			bool isOk = userModel.GetUnlcok (Config.UNLOCK_MATCH, null, true, true);
			if (!isOk)
				return;		
			fightModel.isPreTimeRequest = false;
			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["open_room"] = 0;
//			Debug.LogError ("is true fight");
			NetSocket.inst.Send (NetBase.SOCKET_MATCHJOIN, data);
			return;
		}

		ModelManager.inst.fightModel.fightType = ModelFight.FIGHT_MATCHGUIDE;//新手引导战斗//临时 测试演示战斗
        DispatchManager.inst.Dispatch(new MainEvent(MainEvent.START_FIGHT, PlayerData.instance.data));
        return;
	}

	private void Match_Tick (float t)
	{
		fightModel.matchTime += 1;
		if (fightModel.matchTime > fightModel.preTime)
		{
			if (!fightModel.isPreTimeRequest)
			{
				fightModel.isPreTimeRequest = true;
				Dictionary<string,object> data = new Dictionary<string, object> ();
				data ["open_room"] = 1;
				NetSocket.inst.Send (NetBase.SOCKET_MATCHJOIN, data);
			}
		}
		txt2.text = Tools.GetMessageById ("14027", new string[]{ fightModel.matchTime.ToString () });
	}

	//竞赛
	private void BtnFree_Click ()
	{
//		Dictionary<string,object> dic = new Dictionary<string, object> ();
//		dic.Add ("gold", 100);
//		dic.Add ("coin", 100);
//		dic.Add ("exp", 200);
//		ViewManager.inst.ShowIcon (dic);
//		ViewManager.inst.ShowView<MediatorChangeNameGuide>();
//		ViewManager.inst.ShowGetRedBagCoin(100);
//		btn_Explore.onClick.Call ();
//		GameObjectScaler.Scale(EffectManager.inst.AddEffect("Bag103/bag103","open",this.GetChild("n70").asGraph),0.5f);
//		ViewManager.inst.ShowView<MediatorUserLevelUp> (false);
//		NetSocket.inst.ReConnect(0);
//		return;
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_FREEMATCH1, null, true, true);
		if (!isOk)
			return;
		ViewManager.inst.CloseInviteAlertAll ();
		ViewManager.inst.ShowView<MediatorFreeMatch> ();
	}

    
	//抢萝卜
	private void BtnTeam_Click ()
	{
//		ViewManager.inst.ShowWait ();
//		return;
        //		ViewManager.inst.ShowText (Tools.GetMessageById ("25015"));
//		Debug.LogError(NetSocket.inst.IsConnected() + "");
//        		return;
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_TEAMMATCH, null, true, true);
		if (!isOk)
			return;
		ViewManager.inst.CloseInviteAlertAll ();
        
        bool[] isClose = Tools.RankTimer(0);
        if(isClose.Length > 0) {
            if((bool)isClose[0]) {
                //jiesuan
                ViewManager.inst.ShowAlert(Tools.GetMessageById("25066"), (int index) => {
                    if(index == 1)
                        base.Close();
                    });
                }else if((bool)isClose[1]){
                //baohu
                ViewManager.inst.ShowAlert(Tools.GetMessageById("25065"), (int index) => {
                    if(index == 1)
                        base.Close();
                    });
            } else {
				//socket 侦听 房间建立
				NetSocket.inst.AddListener (NetBase.SOCKET_NEWTEAM, onSOCKET_NEWTEAM);
				NetSocket.inst.Send (NetBase.SOCKET_NEWTEAM, null);
            }
        } 
	}
	public void onSOCKET_NEWTEAM(VoSocket vo){
//		Log.debug ("创建队伍 - " + vo.data.ToString ());
		NetSocket.inst.RemoveListener (NetBase.SOCKET_NEWTEAM);
		//1//2
		System.Type type = vo.data.GetType ();
		if (typeof(int) == type || typeof(string) == type) {
			int id = (int)vo.data;
			if (id == 1) {
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("25066"));
			} else {
				if (id == 2) {
					DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_ING));
				};
				NetSocket.inst.AddListener (NetBase.SOCKET_KILLTEAM, onSOCKET_KILLTEAM);
//				NetSocket.inst.AddListener (NetBase.SOCKET_QUITTEAM, onSOCKET_QUITTEAM);
				NetSocket.inst.Send (NetBase.SOCKET_KILLTEAM, null);
			}
		} else if (vo.data is Boolean) {
//					bool isOk = Convert.ToBoolean (vo.data);
					//MainEvent evt = new MainEvent(MainEvent.FIGHT_ING);
					//evt.data = "clear";
					//DispatchManager.inst.Dispatch(evt);
					//vo.data = false;

		} else {
			fightModel.CreatTeam (vo.data);
			ViewManager.inst.ShowView<MediatorTeamMatch> ();
		}
	}
	public void onSOCKET_KILLTEAM(VoSocket vo){
		NetSocket.inst.RemoveListener (NetBase.SOCKET_KILLTEAM);
		fightModel.Clear ();
		NetSocket.inst.Send (NetBase.SOCKET_NEWTEAM, null);
	}
	public void onSOCKET_QUITTEAM(VoSocket vo){
		NetSocket.inst.RemoveListener (NetBase.SOCKET_QUITTEAM);
	}

	private void BtnCancel_Click ()
	{
		NetSocket.inst.AddListener (NetBase.SOCKET_MATCHQUIT, (VoSocket vo) =>
		{			
			mask.visible = false;
			NetSocket.inst.RemoveListener (NetBase.SOCKET_MATCHQUIT);
			c1.selectedIndex = 0;
			TimerManager.inst.Remove (Match_Tick);
//			ViewManager.inst.ShowText (Tools.GetMessageById ("25015"));
		});
		NetSocket.inst.Send (NetBase.SOCKET_MATCHQUIT, null);
	}

	private void BtnRed_Click ()
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_REDBAG, null, true);
		if (!isOk)
			return;		
		if (userModel.GetRedTime () == -2)
		{
			NetHttp.inst.Send (NetBase.HTTP_GET_REDBAG, "", Get_Red);
		}
		else
		{
			ViewManager.inst.ShowView<MediatorRedPackage> ();
//			ViewManager.inst.ShowView<MediatorGiveRed> ();
		}
	}

	private void Get_Red (VoHttp vo)
	{
		MediatorRobRed._coin = ModelManager.inst.userModel.coin;
		MediatorRobRed._gold = ModelManager.inst.userModel.gold;
		MediatorRobRed.redVo = vo;
		ModelManager.inst.userModel.UpdateData (vo.data);

		ViewManager.inst.ShowView<MediatorRobRed> (false);
		GButton tex = this.GetChild ("n115").asButton;
		TimerManager.inst.Add (0.01f, 1, (float obj) =>
		{			
			if (userModel.GetRedTime () != -2)
			{
                //				addd = EffectManager.inst.AddEffect (Config.EFFECT_MAILBOX, "stand", btn_Red.GetChild ("n2").asGraph);
                //				GameObjectScaler.Scale (addd, 0.45f);
                //				addd.transform.localScale *= 0.45f;
                //				EffectManager.inst.StopAnimation (addd);
                
                EffectManager.inst.AddEffect(Config.EFFECT_HONGBAO, "stand", this.GetChild("n124").asGraph, null, false, 620);
               // tex.GetChild("mc").visible = false;
				//tex.GetChild ("n6").asImage.visible = true;
				dengyu = true;
				if(userModel.GetRedTime() == -1)
				{
					tex.text = Tools.GetMessageById ("12026");
				}
			}
			else
			{
                //				EffectManager.inst.PlayAnimation (addd);
                EffectManager.inst.AddEffect(Config.EFFECT_HONGBAO, "new", this.GetChild("n124").asGraph, null, false, 620, "gifticon/");
               // tex.GetChild("mc").visible = true;
				//tex.GetChild ("n6").asImage.visible = false;
                
			}
		});
	}

	private void BtnMail_Click ()
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_MAIL, null, true);
		if (!isOk)
			return;
		ViewManager.inst.ShowView<MediatorMail> ();
	}

	private void BtnHead_Click ()
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_HEAD, null, true);
		if (!isOk)
			return;
		roleModel.otherInfo = null;
		this.DispatchGlobalEvent (new MainEvent (MainEvent.SHOW_USER, new object[] {
			null,
			userModel.uid,
			-1,
			-1,
			-1
		}));
		//ViewManager.inst.ShowScene<MediatorFightBoxStatement>();
	}


	private void BtnCard_Click ()
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_CARD, null, true);
		if (!isOk)
			return;
		ViewManager.inst.ShowScene<MediatorShip> ();
	}

	private void BtnShop_Click ()
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_SHOP, null, true);
		if (!isOk)
			return;
		ViewManager.inst.ShowScene<MediatorShop> ();
	}

	private void BtnSicial_Click ()
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_FRIEND, null, true);
		if (!isOk)
			return;
		roleModel.isSave = false;
		ViewManager.inst.ShowScene<MediatorFriendRoot> ();
	}

	private void BtnGuild_Click ()
	{		
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_GUILD, null, true);
		if (!isOk)
			return;
		//请求被通过
		userModel.Del_Notice (ModelUser.RED_GUILDJOIN);
		userModel.Del_Notice (ModelUser.RED_GUILDEXIT);
		NetHttp.inst.Send (NetBase.HTTP_GUILD_INDEX, "", (VoHttp vo) =>
		{
			ModelManager.inst.guildModel.viewData = vo;
			ViewManager.inst.ShowScene<MediatorGuildMain> ();
		});
	}

	private void BtnExplore_Click (EventContext ev)
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_EXPLORE, null, true);
		if (!isOk)
			return;
		explore.BtnExplore_Click (ev, OnExploreCall);
//		btn_Explore.visible = false;
	}

	private void OnExploreCall ()
	{
//		btn_Explore.visible = true;
		Time_Explore (0f);
	}

	private void BtnIcon_Click ()
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_EFFORT, null, true);
		if (!isOk)
			return;
		GuideManager.inst.Clear ();
		ViewManager.inst.ShowScene <MediatorEffort> ();
	}

}