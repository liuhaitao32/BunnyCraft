using UnityEngine;
using FairyGUI;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

public class Main : MonoBehaviour
{
	public static Main inst;
	public static Load loadView;
	private ModelGame gameModel;
	private ModelUser userModel;
	private ModelRole roleModel;
	private ModelChat chatModel;
	private ModelGuild guildModel;
	private ModelFight fightModel;
	private ModelShare shareModel;

	private float time = 0;
	private FunQueue initQueue = new FunQueue ();
	private bool isFightEnd = false;

	private ComLog log;

	/// <summary>
	/// // ios gamecenter 
	/// </summary>
	#if UNITY_IOS
	string socialUID;
	string socialUNAME;
	bool socialFirstGot;
	bool socialUIDchanged;
	#endif
	//
	public Main ()
	{
	}

	void Awake ()
	{
//		PlayerPrefs.DeleteAll ();
		inst = this;
	}
	void Start ()
	{
		#if UNITY_IOS
		GameCenterManager.GetInstance ().Start ();
		#endif
//		LocalStore.Clear ();
//		Debug.Log(Config.OffLineTxt ["cn_14056"]);
//		return;

		//
		ModelManager.inst.Register ();
		gameModel = ModelManager.inst.gameModel;
		this.InitConfigXml ();
		//非战斗直接进游戏
		if (!FightMain.fightTest)
		{
			this.initQueue.Init (new List<Action> {
				this.InitConfig,
				this.InitRigester,
				this.InitOther,
				this.InitData,
				this.InitLogin
			});
		}
		else
		{
			//直接进入战斗
			this.initQueue.Init (new List<Action> {
				//this.initConfig,
				//this.InitRigester,
				//this.initModel,
				//this.initOther,
				() =>
				{
					ModelManager.inst.Register ();
					this.initQueue.Next ();
				},
				this.InitData,
				FightMain.instance.onStart
			});
		}        
	}

	private void InitConfig ()
	{
		UIConfig.defaultFont = "default";
		UIConfig.defaultComboBoxVisibleItemCount = 5;
		UIConfig.loaderErrorSign = "ui://go13tig0sdw37t";//"ui://e44hzvgsj4v12c";//ui://e44hzvgsj4v12c
		UIConfig.allowSoftnessOnTopOrLeftSide = true;

		UIObjectFactory.SetLoaderExtension (typeof(BaseLoader));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComNumeric"), typeof(ComNumeric));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComGold"), typeof(ComGold));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComCoin"), typeof(ComCoin));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComIcon"), typeof(ComIcon));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComCard"), typeof(ComCard));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComProgress"), typeof(ComProgress));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComBigIcon"), typeof(ComBigIcon));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ProgressBar"), typeof(ComProgressBar));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComExp"), typeof(ComExp));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComTabEffect"), typeof(ComTabEffect));
		UIObjectFactory.SetPackageItemExtension (UIPackage.GetItemURL ("Base", "ComRankScore"), typeof(ComRankScore));
		//配置
		PlatForm.inst.CheckPlatForm ();
		PlatForm.inst.CheckLanguage ();
		PlatForm.inst.CheckNetwork ();
		PlatForm.inst.GetSdk ().Init ("");
		this.initQueue.Next ();
	}

	private void InitConfigXml ()
	{
		#if UNITY_STANDALONE_WIN
		string path = Environment.GetFolderPath (Environment.SpecialFolder.DesktopDirectory) + "\\config.json";			
		string cfg = Tools.GetConfigFile (path);
		string[] cfgs = cfg.Split (new string[]{ "\r\n" }, StringSplitOptions.None);
		string[] tem;
		bool temb = false;
		for (int i = 0; i < cfgs.Length; i++)
		{
			if (cfgs [i] == "")
				continue;
			tem = cfgs [i].Split ('-');
			temb = tem [1] == "0" ? false : true;
			if (tem [0] == "fight_test")
			{
				//gameModel.fightTest = temb;
				//FightMain.fightTest = gameModel.fightTest;
			}
			else if (tem [0] == "fight_local")
				FightMain.isLocal = gameModel.fightLocal = temb;
			else if (tem [0] == "remote")
			{				
				gameModel.remote = tem [1];
				RemoteLog.Instance.Start (gameModel.remote, 2010);
			}
			else if (tem [0] == "login_double")
			{
				gameModel.loginDoubel = temb;				
			}
		}
		#endif
	}

	private void InitRigester ()
	{		
		SoundManager.inst.Register (this.gameObject);
		MicroManager.inst.Register (this.gameObject);
		DispatchManager.inst.Register (MainEvent.RESTART_GAME, RESTART_GAME);
		DispatchManager.inst.Register (MainEvent.RELOGIN_GAME, RELOGIN_GAME);
		DispatchManager.inst.Register (MainEvent.SHOW_USER, SHOW_USER);
		DispatchManager.inst.Register (MainEvent.SOCKET_ERROR, SOCKET_ERROR);
		DispatchManager.inst.Register (MainEvent.FIGHT_RESULT, FIGHT_RESULT);
		DispatchManager.inst.Register (MainEvent.START_FIGHT, START_FIGHT);
		DispatchManager.inst.Register (MainEvent.CLOSE_FIGHT, CLOSE_FIGHT);
		DispatchManager.inst.Register (MainEvent.FIGHT_INIT_COMPLETE, FIGHT_INIT_COMPLETE);
		DispatchManager.inst.Register (MainEvent.FIGHT_QUIT, FIGHT_QUIT);
		DispatchManager.inst.Register (MainEvent.FIGHT_ING, FIGHT_ING);
		DispatchManager.inst.Register (MainEvent.SHOW_FIGHTDETAIL, SHOW_FIGHTDETAIL);
		DispatchManager.inst.Register (MainEvent.SOCKET_CLOSE, SOCKET_CLOSE);

		userModel = ModelManager.inst.userModel;
		roleModel = ModelManager.inst.roleModel;
		chatModel = ModelManager.inst.chatModel;
		guildModel = ModelManager.inst.guildModel;
		fightModel = ModelManager.inst.fightModel;
		shareModel = ModelManager.inst.shareModel;

		gameModel.width = GRoot.inst.width;
		gameModel.height = GRoot.inst.height;
		this.initQueue.Next ();
	}

	private void InitOther ()
	{
		//GameObject.Find ("Stage").AddComponent<FPS> ();
		this.initQueue.Next ();
	}

	private void InitData ()
	{
		NetHttp.inst.Send (NetBase.HTTP_CONFIG, "lan=" + PlatForm.inst.language, (vo) => {
			DataManager.inst.ConfigWasGet = true;
			DataManager.inst.Decode (vo.data);
			string newVersion = PlatForm.inst.App_Version;
			if (DataManager.inst.systemSimple != null && DataManager.inst.systemSimple.ContainsKey ("app_version")) {
				newVersion = (string)DataManager.inst.systemSimple ["app_version"];
			}
			if (PlatForm.inst.App_Version == newVersion) {
				this.initQueue.Next ();
			} else {
				Check_update_down ();
			}
		},'|',null,60);
	}
	private void Check_update_down(){
		ViewManager.inst.ShowAlert(Tools.GetMessageById((string)DataManager.inst.systemSimple["app_update_txt"]),(int v)=>{//app_update_txt
			Application.OpenURL (PlatForm.inst.GetPF_update_link(DataManager.inst.systemSimple["app_update_down_link"]));
//			Application.OpenURL ("meng52://ios.app");
			Check_update_down();
		});
	}

	private void SOCKET_ERROR (MainEvent e)
	{
		//			ViewManager.inst.ShowAlert (Tools.GetMessageById ("14004"), ReCall, false);
	}

	//	private void ReCall (int index)
	//	{
	//		Main.inst.Socket_Close ();
	//		Main.inst.Socket_Start ();
	//	}

	private void RESTART_GAME (MainEvent e)
	{
		NetSocket.inst.RemoveListenersMethod ();

		ViewManager.inst.Clear ();
		ViewManager.inst.ShowScene <ComLan> (true);
		TimerManager.inst.Add (3f, 1, (float time) =>
		{
			this.Clear ();
			this.Init ();
		});
	}

	private void RELOGIN_GAME (MainEvent e)
	{
		NetSocket.inst.RemoveListenersMethod ();

		ViewManager.inst.Clear ();
		ViewManager.inst.ShowScene <ComLan> (true);
		TimerManager.inst.Add (3f, 1, (float time) =>
		{
			this.initQueue.Init (new List<Action> {
				this.InitData,
				this.InitLogin
			});
		});
	}

	private void SOCKET_CLOSE (MainEvent e)
	{
//		RELOGIN_GAME (null);
		NetSocket.inst.Close ();
	}

	//重新登录 清理数据
	public void Clear ()
	{
		ModelManager.inst.Clear ();
	}

	private void SHOW_USER (MainEvent e)
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_HEAD, null, true);
		if (!isOk)
			return;
		object[] obs = (object[])e.data;
		Dictionary<string, object> dd = new Dictionary<string, object> ();
		dd ["fuid"] = obs [1].ToString ();
		NetHttp.inst.Send (NetBase.HTTP_FUSERGET, dd, (VoHttp vo) =>
		{
			if (vo.data != null)
			{
				roleModel.SetTabSelect (obs);
				roleModel.otherInfo = (Dictionary<string, object>)vo.data;
				ViewManager.inst.ShowScene <MediatorRoleRoot> ();
			}
		});
	}

	private void SHOW_FIGHTDETAIL (MainEvent e)
	{
		Dictionary<string, object> data = (Dictionary<string, object>)e.data;
		object[] data1 = (object[])data ["fight_data"];
		Dictionary<string, object> value1 = new Dictionary<string, object> ();
		//value1["uid"] = data["uid"];
		value1 ["log_id"] = data1 [1];
		NetHttp.inst.Send (NetBase.HTTP_GET_FIGHT_DATA, value1, (VoHttp v) =>
		{
			fightModel.isOpenFromRecord = false;
			fightModel.myUid = data ["uid"].ToString ();
			List<object> list = ((object[])v.data).ToList ();
			if (Tools.IsNullEmpty (data ["uname"]))
			{
				list.Add (data ["uid"]);
			}
			else
			{
				list.Add (data ["uname"]);
			}

			fightModel.fightDataDetails = list.ToArray ();
			switch ((int)data1 [0])
			{
			case 1:
				ViewManager.inst.ShowView<MediatorFightDataShowMatch> ();
				break;
			case 2:
				ViewManager.inst.ShowView<MediatorFightDataShowRank> ();
				break;
			}
		});
	}

	private void FIGHT_RESULT (MainEvent e)
	{
		//reward
		fightModel.fightResult = null;

		Dictionary<string, object> data = (Dictionary<string, object>)e.data;
		Dictionary<string, object> data1 = (Dictionary<string, object>)data ["data"];
		data1 ["statementType"] = data ["type"].ToString ();
		fightModel.fightData = data1;
		ViewManager.inst.ShowScene<MediatorFightBoxStatement> (true);
	}

	private void START_FIGHT (MainEvent e)
	{
		ViewManager.inst.CloseInviteAlertAll ();
		PlayerData.instance.data = (Dictionary<string,object>)e.data;
		ViewManager.inst.ShowScene <MediatorLoadFight> (true);
	}

	private void CLOSE_FIGHT (MainEvent e)
	{		
	}

	private void FIGHT_INIT_COMPLETE (MainEvent e)
	{
//		TimerManager.inst.Add (1f, 1, (float obj) =>
//		{
		ViewManager.inst.RemoveEventTouch ();
		GRoot.inst.visible = false;
		this.Clear ();
//		});
	}

	private void FIGHT_QUIT (MainEvent e)
	{
		GRoot.inst.visible = true;
		ViewManager.RemoveScene ("snake");
		ViewManager.inst.AddEventTouch ();
		this.isFightEnd = true;
		this.Init ();
	}

	private void FIGHT_ING (MainEvent e)
	{
//		GRoot.inst.visible = true;
		ModelFight fightModel = ModelManager.inst.fightModel;
//		fightModel.isLeader = true;
		ViewManager.inst.ShowText("战斗进行中。。。！中途退出过战斗");
		if (e.data != null && e.data is string)
		{
//			ViewManager.inst.ShowAlert ("战斗进行中。。继续战斗 ", (int v) =>
//			{
//				ViewManager.inst.CloseView ();
//			}, false, false);
		}
		else
		{
//			ViewManager.inst.ShowAlert ("战斗进行中。。继续战斗 ", (int v) =>
//			{
//				ViewManager.inst.ShowScene <MediatorFightWorld> (true);
//			});
		}
	}

	void OnDestroy ()
	{
		if (gameModel != null && gameModel.loginDoubel)
		{
			string idn = LocalStore.GetLocal (LocalStore.LOCAL_IDNUM);
			LocalStore.SetLocal (LocalStore.LOCAL_IDNUM, idn == "0" ? "1" : "0");
		}
		NetSocket.inst.Close ();
		DispatchManager.inst.Unregister (MainEvent.RESTART_GAME, RESTART_GAME);
		DispatchManager.inst.Unregister (MainEvent.RELOGIN_GAME, RELOGIN_GAME);
		DispatchManager.inst.Unregister (MainEvent.SHOW_USER, SHOW_USER);
		DispatchManager.inst.Unregister (MainEvent.SOCKET_ERROR, SOCKET_ERROR);
		DispatchManager.inst.Unregister (MainEvent.FIGHT_RESULT, FIGHT_RESULT);
		DispatchManager.inst.Unregister (MainEvent.START_FIGHT, START_FIGHT);
		DispatchManager.inst.Unregister (MainEvent.CLOSE_FIGHT, CLOSE_FIGHT);
		DispatchManager.inst.Unregister (MainEvent.FIGHT_INIT_COMPLETE, FIGHT_INIT_COMPLETE);
		DispatchManager.inst.Unregister (MainEvent.FIGHT_QUIT, FIGHT_QUIT);
		DispatchManager.inst.Unregister (MainEvent.FIGHT_ING, FIGHT_ING);
		DispatchManager.inst.Unregister (MainEvent.SOCKET_CLOSE, SOCKET_CLOSE);
	}

	/// <summary>
	/// // ios gamecenter 
	/// </summary>
	#if UNITY_IOS
	void ChangeUserInfo(){
		if (GameCenterManager.gameCenterIsSuccess) {
			socialUIDchanged = false;
			socialUID = Social.localUser.id;
			socialUNAME = Social.localUser.userName;
		}
	}
	void CheckChange(){
		if (GameCenterManager.gameCenterIsSuccess) {
			if (socialUID != Social.localUser.id && !socialUIDchanged) {//如果 gameCenterUID 改变了
				socialUIDchanged = true;
				Debug.Log (socialUNAME + "|" + socialUID + " : －－账号切换了－－ : " + Social.localUser.userName + "|" + Social.localUser.id);
				RELOGIN_GAME (null);
				//重新给当前 gameCenter 数据赋值
				ChangeUserInfo ();
			}
		}
	}
	#endif

	void Update ()
	{
		// ios gamecenter 
		#if UNITY_IOS
		if (Social.localUser.authenticated) {
			if (!socialFirstGot && GameCenterManager.gameCenterIsSuccess) {//第一次打开游戏，记录第一个 gameCenterUID
				socialUID = Social.localUser.id;
				socialUNAME = Social.localUser.userName;
				socialUIDchanged = false;
				socialFirstGot = true;//d
			} else {
				CheckChange ();
			}

		} else {
			if (socialFirstGot) {
				CheckChange ();
			}
		}
		#endif
//		if (gameModel != null && gameModel.time.Ticks != 0)
//		{
//			gameModel.time = gameModel.time.AddTicks ((long)(Time.deltaTime * 1000 * 10000));
//		}
		if (time <= 0)
		{
			if (Input.GetKey (KeyCode.F1))
			{
				LocalStore.Clear ();
//				LocalStore.DelLocal(LocalStore.LOCAL_TYPE);
//				LocalStore.DelLocal(LocalStore.LOCAL_UID);
//				LocalStore.DelLocal(LocalStore.LOCAL_PWD);
				RELOGIN_GAME (null);
				roleModel.uids.Clear ();
				time = 1;
			}
			else if (Input.GetKey (KeyCode.F4))
			{
				PlatForm.inst.HTTP = "http://192.168.1.118:8899/gateway/";
				RELOGIN_GAME (null);
				roleModel.uids.Clear ();
				time = 1;
			}
			else if (Input.GetKey (KeyCode.F5))
			{
				PlatForm.inst.HTTP = "http://192.168.1.119:8899/gateway/";
				RELOGIN_GAME (null);
				roleModel.uids.Clear ();
				time = 1;
			}
			else if (Input.GetKey (KeyCode.F2))
			{						
				PhoneManager.inst.ScreenImage ("screen_" + Tools.GetDateTimeString ());						
				time = 1;
			}
			else if (Input.GetKey (KeyCode.F3))
			{			
				this.StartCoroutine (ScreenImage ());
				time = 1;
			}
			else if (Input.GetKey (KeyCode.Home))
			{
				if (log == null)
				{
					log = new ComLog ();	
					GRoot.inst.AddChild (log.group);
				}
				else
				{
					GRoot.inst.RemoveChild (log.group);
					log = null;
				}
				time = 1;
			}
		}
		time -= Time.deltaTime;
        //		Log.debug ("time - " + time);
        MediatorSystem.timeStart("timerManager");
		TimerManager.inst.Update ();
        MediatorSystem.getRunTime("timerManager");
    }

	private IEnumerator<object> ScreenImage ()
	{
		yield return new WaitForEndOfFrame ();
		float sw = GRoot.inst.width * GRoot.contentScaleFactor;
		float sh = GRoot.inst.height * GRoot.contentScaleFactor;
		PhoneManager.inst.CaptureScreen ("screen_" + Tools.GetDateTimeString (), new Rect (0, 0, sw, sh), new Vector2 (500, 300));
		this.StopCoroutine (ScreenImage ());
	}

	//程序退出
	void OnApplicationQuit ()
	{
		//		Log.debug ("Game Exit");
	}

	void OnApplicationFocus ()
	{
		//		Log.debug ("Game Focus");
	}

	void OnApplicationPause ()
	{
		//		Log.debug ("Game Pause");
	}


	private void InitLogin ()
	{
//		NetHttp.inst.sendHttp_wx ("https://graph.qq.com/user/get_user_info?access_token=" + "F9DB32C8E692C5F636A2A9AE4CD964B0" + "&oauth_consumer_key=meng52&openid=" + "367F8C9525DC8690EC9DF1578F6714A1");
//		return;

//		LocalStore.SetLocal (LocalStore.LOCAL_UID, "2469");
//		LocalStore.SetLocal (LocalStore.LOCAL_PWD, "ee840253e1e7a9d2f7d6f1176640b4da");
//		return;

		string type = LocalStore.GetLocal (LocalStore.LOCAL_TYPE);

		string uid = "";
//		if (type == Ex_Local.LOGIN_TYPE_UNAME)
//		{
//			uid = LocalStore.GetLocal (LocalStore.LOCAL_UNAME);
//		}
//		 if (type == Ex_Local.LOGIN_TYPE_TEL)
//		{
			uid = LocalStore.GetLocal (LocalStore.LOCAL_UID);
//		}
//		else if (type == Ex_Local.LOGIN_TYPE_UID)
//		{
//			uid = LocalStore.GetLocal (LocalStore.LOCAL_UID);
//		}
		string pwd = LocalStore.GetLocal (LocalStore.LOCAL_PWD);
		//
		userModel.type_login = type;
		userModel.uid = uid;
		userModel.pwd = pwd;
		//
		if (PlatForm.inst.pf != PlatForm.EX_LOCAL)
		{
			PlatForm.inst.GetSdk ().InitPF (type);//平台初始化全部
		}
		//
//		if (type == Ex_Local.LOGIN_TYPE_QQ || type == Ex_Local.LOGIN_TYPE_WEIXIN)
//		{
//			//
//			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_LOGIN_PF, Pf_login);
//			PlatForm.inst.GetSdk ().Login (type);
//		}
//		else
//		{
			InitLoginToServer (new string[]{ type, uid, pwd });
//		}

	}

	private void Pf_login (object data)
	{
		InitLoginToServer (new string[]{ Ex_Local.LOGIN_TYPE_UID, userModel.uid, userModel.pwd });//test
	}

	private void InitLoginToServer (string[] data)
	{
		userModel.Login (data [0], data [1], data [2], () =>
		{
			this.Clear ();
			this.Init ();
		});
	}

	private void Init ()
	{
		if (loadView)
		{
			loadView.Clear ();
			loadView = null;
		}
		gameModel.isLogin = true;
		if (isFightEnd)
		{
			isFightEnd = false;
            //主界面
            //			ViewManager.inst.ShowScene (new MediatorFightWorld ());
            if(fightModel.fightType == ModelFight.FIGHT_MATCHTEAM) {

            } else if(fightModel.fightType == ModelFight.FIGHT_MATCH) {
//				Debug.LogError (fightModel.fightType);
                //				NetSocket.inst.AddListener (NetBase.SOCKET_GETRESUILTPUSH, (VoSocket vo) =>
                //				{
                //					NetSocket.inst.RemoveListener (NetBase.SOCKET_GETRESUILTPUSH);
                //
                //					Dictionary<string,object> data = new Dictionary<string, object> ();
                //					data ["data"] = vo.data;
                //					data ["type"] = fightModel.fightType;
                //					DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_RESULT, data));
                //				});
                //				NetSocket.inst.Send (NetBase.SOCKET_GETRESUILT, null);
//				if (fightModel.fightResult != null) {
//					DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_RESULT, fightModel.fightResult));
//				}
			} else if(fightModel.fightType == ModelFight.FIGHT_MATCHGUIDE) {
//				Debug.Log ("Main match_guide init");
				Debug.Log ("HTTP_DOGUIDE + test");
				if (GuideManager.inst.Check ("0:0")) {
					userModel.SetGuide (1, Guide_fight);
				} else if (GuideManager.inst.Check ("2:1")) {
					userModel.SetGuide (3, Guide_fight);
				} else if (GuideManager.inst.Check ("4:0")) {
					//临时 测试 单机 接口

					NetHttp.inst.Send (NetBase.HTTP_DOGUIDE + "_test", new Dictionary<string, object> (), Guide_fight);
				} else {
					NetHttp.inst.Send (NetBase.HTTP_DOGUIDE + "_test", new Dictionary<string, object> (), Guide_fight);
				}
            } else {
                NetSocket.inst.AddListener(NetBase.SOCKET_GETRESULTFREEMATCHPUSH, (VoSocket vo) => {
                    NetSocket.inst.RemoveListener(NetBase.SOCKET_GETRESULTFREEMATCHPUSH);

                    Dictionary<string, object> data = new Dictionary<string, object>();
                    data["data"] = vo.data;
                    data["type"] = fightModel.fightType;
                    DispatchManager.inst.Dispatch(new MainEvent(MainEvent.FIGHT_RESULT, data));
                });
                NetSocket.inst.Send(NetBase.SOCKET_GETRESULTFREEMATCH, null);
            }
		}
		else
		{
			if (GuideManager.inst.Check ("0:0")) {
				//			GuideManager.inst.Next ();
				//			GuideManager.inst.Show (this);
//				BtnMatch_Click ();//第一场战斗
				ModelManager.inst.fightModel.fightType = ModelFight.FIGHT_MATCHGUIDE;//新手引导战斗//临时 测试演示战斗
				DispatchManager.inst.Dispatch(new MainEvent(MainEvent.START_FIGHT, PlayerData.instance.data));
				return;
			} else {
				//主界面
				ViewManager.inst.ShowScene<MediatorMain> ();
				//离线卡牌
				if (userModel.records ["guild_support_logs"] != null) {
					ViewManager.inst.ShowView <MediatorLoginCard> ();
				}
			}
		}

//		Socket_Start ();
	}
	private void Guide_fight(VoHttp e){
		Dictionary<string, object> dic = new Dictionary<string, object>();
		dic["type"] = ModelFight.FIGHT_MATCHGUIDE;
		Dictionary<string, object> data = (Dictionary<string, object>)e.data;
		dic["data"] = data;
		data ["rank"] = PlayerData.instance.rank;
		//
		DispatchManager.inst.Dispatch(new MainEvent(MainEvent.FIGHT_RESULT, dic));
	}
	public void Socket_Start ()
	{
		if (NetSocket.inst.IsConnected ())
		{
			this.Socket_Close ();
			return;
		}
		NetSocket.inst.onConnect = () =>
		{
			NetSocket.inst.onConnect = null;
			Socket_Login ();
//			Log.debug ("Socket Connect");
		};

		NetSocket.inst.Start (0, PlatForm.inst.SERVER4, PlatForm.inst.PORT);		
//		NetSocket.inst.Start (0, "dtczk.meng52.net", 7777,true);
	}

	private void Socket_Login ()
	{
		Dictionary<string, object> data = new Dictionary<string, object> ();
		data ["uid"] = userModel.uid;
		data ["sessionid"] = userModel.sessionid;
		//		Log.debug ("uid" + userModel.uid + " - sessionid" + userModel.sessionid);
		NetSocket.inst.AddListener (NetBase.SOCKET_LOGIN, userModel.Socket_loginBack);//(VoSocket vo) =>
//		{
				
//			if (vo.data is Boolean)
//			{
////					Debug.LogError("SOCKET_LOGIN false");
//				NetSocket.inst.socketLogin = false;
//			}
//			else
//			{
////					Debug.LogError("SOCKET_LOGIN true");
////				Log.debug ("Socket Login - " + vo.data.ToString ());
//				NetSocket.inst.socketLogin = true;//Convert.ToBoolean (vo.data);
//
//				Dictionary<string,object> re = (Dictionary<string,object>)vo.data;
//				bool isBattle = Convert.ToBoolean (re ["in_battle"]);
//				if (isBattle)
//				{
//					int typeId = (int)re ["type"];
//					ModelFight fightModel = ModelManager.inst.fightModel;
//					fightModel.isLeader = true;
//					if (typeId == 1)
//					{
//						fightModel.fightType = ModelFight.FIGHT_MATCH;
//					}
//					else if (typeId == 2)
//					{
//						fightModel.fightType = ModelFight.FIGHT_MATCHTEAM;
//					}
//					else if (typeId == 3)
//					{
//						fightModel.fightType = ModelFight.FIGHT_FREEMATCH1;
//					}
////					DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_ING));
//				}
//				else
//				{
//					
//				}
//			}
//			Socket_Listen ();
//		}
//		);
		NetSocket.inst.Send (NetBase.SOCKET_LOGIN, data);
	}

	public void Socket_Close ()
	{
		NetSocket.inst.RemoveListeners ();
		NetSocket.inst.Close ();
	}

	public void Socket_Check ()
	{
	}
	public void Socket_Listen ()
	{
		NetSocket.inst.AddListener (NetBase.SOCKET_CHAT, (VoSocket vo) =>
		{			
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			data ["ctype"] = "chat";
			data ["effect"] = true;
			chatModel.all.Add (data);
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_SEND, vo.data));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_APPLEJOINGUILD, (VoSocket vo) =>
		{
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			data ["ctype"] = "guild";
			data ["effect"] = true;
			chatModel.all.Add (data);
			chatModel.Add_RedCount ();
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_APPLEJOINGUILD, vo.data));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_GUILDJOIN, (VoSocket vo) =>
		{						
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			Dictionary<string,object> user1 = (Dictionary<string,object>)data ["user1"];
			Dictionary<string,object> user2 = (Dictionary<string,object>)data ["user2"];
			string uid = user2 ["uid"].ToString ();
			int index = chatModel.RemoveChat ("guild", uid);
			data ["ctype"] = "guild_join";
			data ["effect"] = true;
			chatModel.all.Add (data);

			if (index != -1)
				chatModel.Remove_RedCount ();
			else
				chatModel.Add_RedCount ();
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_GUILDJOIN, vo.data));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_GUILDMODIFY, (VoSocket vo) =>
		{			
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			Dictionary<string,object> user1 = (Dictionary<string,object>)data ["user1"];
			Dictionary<string,object> user2 = (Dictionary<string,object>)data ["user2"];
			if (Convert.ToInt32 (data ["num"]) == -1)//被T
			{				
				chatModel.Clear ();
				if (userModel.uid == user2 ["uid"].ToString ())
				{					
					DispatchManager.inst.Dispatch (new MainEvent (MainEvent.GUILD_ESC, 10));
				}
				else if (guildModel.my_guild_member != null)
				{
					guildModel.my_guild_member.Remove (user2 ["uid"].ToString ());
				}
			}
			else
			{
				Dictionary<string,object> ttt;
				if (guildModel.my_guild_member != null && guildModel.my_guild_member.ContainsKey (user2 ["uid"].ToString ()))
				{
					ttt = (Dictionary<string,object>)guildModel.my_guild_member [user2 ["uid"].ToString ()];
					ttt ["gradation"] = Convert.ToInt32 (data ["num"]);

					if (userModel.uid == user2 ["uid"].ToString ())
					{
						guildModel.my_guild_job = Convert.ToInt32 (data ["num"]);
					}
					if (Convert.ToInt32 (data ["num"]) == 0)
					{
						ttt = (Dictionary<string,object>)guildModel.my_guild_member [user1 ["uid"].ToString ()];
						ttt ["gradation"] = 1;
						if (userModel.uid == user1 ["uid"].ToString ())
						{
							guildModel.my_guild_job = 1;
						}
					}
				}
			}
			data ["ctype"] = "guild_modify";
			data ["effect"] = true;
			chatModel.all.Add (data);
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_GUILDMODIFY, vo.data));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_GUILDEXIT, (VoSocket vo) =>
		{
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			Dictionary<string,object> user1 = (Dictionary<string,object>)data ["user"];
			if (userModel.uid == user1 ["uid"].ToString ())
			{
				chatModel.Clear ();
			}
			else
			{
				data ["ctype"] = "guild_modify";
				data ["effect"] = true;
				chatModel.all.Add (data);
				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_GUILDMODIFY, vo.data));
			}
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_SENDGUILDREDBAG, (VoSocket vo) =>
		{						
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			data ["ctype"] = "send_guild_redbag";
			data ["effect"] = true;
			chatModel.all.Add (data);
			chatModel.Add_RedCount ();
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_SENDGUILDREDBAG, vo.data));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_REQUIREGUILDSUPPORT, (VoSocket vo) =>
		{			
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			data ["ctype"] = "require_guild_support";
			data ["effect"] = true;
			chatModel.all.Add (data);
			if (data ["uid"].ToString () != userModel.uid)
				chatModel.Add_RedCount ();
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_REQUIREGUILDSUPPORT, vo.data));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_SENDGUILDSUPPORT, (VoSocket vo) =>
		{									
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			Dictionary<string,object> da = (Dictionary<string,object>)data ["require_user"];
			string uid = da ["uid"].ToString ();
			string cardId = "";
			string name = ModelUser.GetUname (data ["uid"], data ["name"]);
			int count = -1;
			if (userModel.uid == uid)
			{								
				foreach (string n in da.Keys)
				{
					if (n == Config.ASSET_GOLD)
					{
						userModel.gold = Convert.ToInt32 (da [n]);
					}
					else if (n == Config.ASSET_CARD)
					{
						Dictionary<string,object> temp = (Dictionary<string,object>)da [n];
						foreach (string id in temp.Keys)
						{
							count = userModel.GetCardMulCard (userModel.card [id], temp [id]);
							userModel.card [id] = temp [id];
							cardId = id;
						}
					}
				}
				if (count != -1)
				{
                    string str = "";
                    str = count == 0 ? " " : "X"+count.ToString();
                    string msg = Tools.GetMessageById ("14035", new string[]{ name, CardVo.GetCardName (cardId), str });
					msg = Tools.GetMessageColor (msg, new string[]{ "dc8100", "2699ea" });
                    if(count == 0) ViewManager.inst.ShowText(Tools.GetMessageById("33212"));
					ViewManager.inst.ShowMessage (msg);
                    
				}
			}
			chatModel.UpdateGuildSupport (data);//List<int> index = 
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_SENDGUILDSUPPORT));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_GUILDFIGHTSHARE, (VoSocket vo) =>
		{
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			data ["ctype"] = "guild_fight_share";
			data ["effect"] = true;
			chatModel.all.Add (data);
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_FIGHTSHARE, vo.data));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_NOTICE, (VoSocket vo) =>
		{
			Dictionary<string,object> d = (Dictionary<string,object>)vo.data;
			foreach (string n in d.Keys)
			{
				userModel.notice [n] = d [n];
			}
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_UPDATE, vo.data));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_GETINVITE, (VoSocket vo) =>
		{
			if (!userModel.IsInvite ())
				return;
//			bool isOk = userModel.GetUnlcok (Config.UNLOCK_TEAMMATCH);
//			if (!isOk)
//				return;
//			isOk = userModel.GetUnlcok (Config.UNLOCK_FREEMATCH1);
//			if (!isOk)
//				return;
//			isOk = userModel.GetUnlcok (Config.UNLOCK_FREEMATCH2);
//			if (!isOk)
//				return;
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			data ["type"] = 0;
			ViewManager.inst.ShowInviteAlert (data);
//			string name = ModelUser.GetUname (data ["uid"], data ["name"]);
//			string teamId = data ["team_id"].ToString ();
//			ViewManager.inst.ShowAlert (Tools.GetMessageById ("25006", new String[]{ name }), (int index) =>
//			{
//				if (index == 1)
//				{
//					Dictionary<string,object> da = new Dictionary<string, object> ();
//					da ["team_id"] = teamId;
//					NetSocket.inst.Send (NetBase.SOCKET_ACCEPTINVITE, da);
//				}
//			}, true);
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_ACCEPTINVITE, (VoSocket vo) =>
		{
			if (vo.data is Boolean && !Convert.ToBoolean (vo.data))
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("25007"));
				return;
			}
			fightModel.SetTeam (vo.data);
			fightModel.isRequest = false;
			ViewManager.inst.ShowScene <MediatorMain> ();
			ViewManager.inst.ShowView <MediatorTeamMatch> ();
			ViewManager.inst.CloseInviteAlertAll ();
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_FREEGETINVITE, (VoSocket vo) =>
		{			
			if (!userModel.IsInvite ())
				return;
			bool isOk = userModel.GetUnlcok (Config.UNLOCK_TEAMMATCH);
			if (!isOk)
				return;
			isOk = userModel.GetUnlcok (Config.UNLOCK_FREEMATCH1);
			if (!isOk)
				return;
			isOk = userModel.GetUnlcok (Config.UNLOCK_FREEMATCH2);
			if (!isOk)
				return;
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			data ["type"] = 1;
			ViewManager.inst.ShowInviteAlert (data);
//			string name = ModelUser.GetUname (data ["uid"], data ["name"]);
//			string roomId = data ["room_id"].ToString ();
//			ViewManager.inst.ShowAlert (Tools.GetMessageById ("25040", new String[]{ name }), (int index) =>
//			{
//				if (index == 1)
//				{
//					Dictionary<string,object> da = new Dictionary<string, object> ();
//					da ["room_id"] = roomId;
//					NetSocket.inst.Send (NetBase.SOCKET_FREEACCEPTINVITE, da);
//				}
//			}, true);
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_FREEACCEPTINVITE, (VoSocket vo) =>
		{
			if (vo.data is Boolean && !Convert.ToBoolean (vo.data))
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("25007"));
				return;
			}
			fightModel.SetFreeTeam (vo.data);
			fightModel.isRequest = false;
			ViewManager.inst.ShowScene<MediatorMain> ();
			ViewManager.inst.ShowView<MediatorFreeMatch> ();
			ViewManager.inst.CloseInviteAlertAll ();
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_SHOWMSG, (VoSocket vo) =>
		{
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			string id = data ["id"].ToString ();
			Dictionary<string,object> reward = userModel.GetReward (data ["data"]);
			userModel.UpdateData (data ["data"]);
			string msg = "";
			if (id == "share")
			{
				msg = Tools.GetMessageById ("14034", new string[]{ reward ["coin"].ToString () });
				msg = Tools.GetMessageColor (msg, new string[]{ "dc8100", "2699ea" });
				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.EVENTSHARE, "3"));
				ViewManager.inst.ShowMessage (msg);
			}
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_GRABGUILDREDBAG, (VoSocket vo) =>
		{
			userModel.records ["guild_redbag_log"] = vo.data;
//			Dictionary<string,object> da = (Dictionary<string,object>)vo.data;
//			Log.debug ("remain_num - " + da ["remain_num"].ToString ());
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_ISSENDREDBAG));
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_GETRESUILTPUSH, (VoSocket vo) =>
		{
			if (fightModel.fightResult != null)
				return;
			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["data"] = vo.data;
			data ["type"] = fightModel.fightType;
			fightModel.fightResult = data;
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_RESULT, data));
		});
		NetSocket.inst.AddListener (NetBase.SOCKET_GETRESUILTTEAMPUSH, (VoSocket vo) =>
		{
			if (fightModel.fightResult != null)
				return;
			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["data"] = vo.data;
			data ["type"] = fightModel.fightType;
			fightModel.fightResult = data;
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_RESULT, data));
		});
	}
}