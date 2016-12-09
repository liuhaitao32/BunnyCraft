using System;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightMain : FightViewBase {

    public LobbyMain lobby;

    public TestPanel testPanel;

    public EqualPanel equalPanel;


    public static FightMain instance;

	public ServerRunTime server;

	private ClientRunTime _selection;
    /// 这个是采用网络还是本地直接初始化。
	public static bool isLocal = false;		//提交用 false 测试用 true
    
    /// 这个是直接运行战斗，显示大厅界面
	public static bool fightTest = false;	//提交用 false 测试用 true
	///是否固定点出生 Biggo添加
	public static bool isFixedBirth = false;
	///战斗结束等待时间秒数 Biggo添加
	public static float fightEndWaitTime = 3;//看菜单用 3 仅测战斗用 0

	public static bool isTest = false;

    public bool isPlay = false;

    public string roomId = "3";

    public static bool equal = false;
    
    //下面两个测试用的。
    public int mode = ConfigConstant.MODE_RADISH;
    public string mapId = "map02";
    //public int mode = ConfigConstant.MODE_WATCH;

    public string id = "-1";

    public string roomKey;
    public static string ccccc = "";

    public Canvas canvas;

    public Scene scene;

    public AutoRunClient autoRunClient = new AutoRunClient();


    public static bool checkError = false;

    public FightMain () {

    }

    protected override void preInit() {
        base.preInit();
        instance = this;
        Collision.init();
        this.canvas.enabled = false;
        if(null == Main.inst) {
            FightMain.fightTest = true;
            this.gameObject.AddComponent<Main>();
            Application.targetFrameRate = 30;
            Debug.Log("单独战斗版本");
        } else {
            Debug.Log("正式上线版本");
            //这个是正式版本！
            FightMain.fightTest = false;
            if(ModelManager.inst.fightModel.fightType == ModelFight.FIGHT_MATCHGUIDE) {
                FightMain.isLocal = true;
                PlayerData.instance.data["mode"] = ConfigConstant.MODE_GUIDE;
                Debug.Log("新手引导");
            } else {
                FightMain.isLocal = false;
            }
            PlayerData.instance.uid = ModelManager.inst.userModel.uid;
            //TODO:临时这么写一下。
            PlayerData.instance.cardGroup = ModelManager.inst.userModel.GetCurrCardGroup();
        }

        this.gameObject.AddComponent<FPS>();
        Dictionary<string, object> data = PlayerData.instance.data;
        FightMain.instance.mode = (int)data["mode"];
        this.roomId = data["room_id"].ToString();
        this.roomKey = data["room_key"].ToString();
        //自己初始化好了，就显示战斗了哦！
        this.addListener(EventConstant.PLAYER_IN, (e) => {
            this.canvas.enabled = true;
            DispatchManager.inst.Dispatch(new MainEvent(MainEvent.FIGHT_INIT_COMPLETE));
            this.scene.gameObject.SetActive(true);
            if(!FightMain.fightTest) {
                GameObject.Find("Main Camera").GetComponent<Camera>().enabled = false;
                GameObject.Find("Stage Camera").GetComponent<Camera>().enabled = false;
            }
        });
    }

    public void init() {
        ConfigConstant.init();
        ViewConstant.init();
		//Biggo添加
		//if (FightMain.isLocal) {
			AIConstant.init ();
		//}

		if(this.isShowLobby) {
			this.lobby.init();
            this.gotoLobby();
        } else {
            this.startFight();

        }
    }

    public bool isTestMode { get { return this.mode == ConfigConstant.MODE_TEST_CHAOS || this.mode == ConfigConstant.MODE_TEST_RADISH || true; } }
    public bool isShowLobby { get { return FightMain.fightTest || this.isTestMode; } }
    public bool isWatch { get { return this.mode == ConfigConstant.MODE_WATCH || this.mode == ConfigConstant.MODE_TEST_WATCH; } }
    public bool isInitMap { get { return (FightMain.instance.isWatch || FightMain.instance.isTestMode) && !isGuide; } }
    public bool isGuide { get { return FightMain.instance.mode == ConfigConstant.MODE_GUIDE; } }

    public void gotoLobby() {        
        this.gotoLobbyNow();
        //EasyTouch.instance.enableRemote = true;
    }

    public int cc;

    private void gotoLobbyNow() {
        this.scene.gameObject.SetActive(false);
//		Stage.inst.gameObject.SetActive (true);
        this.testPanel.hide();
        this.clearFight();
        this.autoRunClient.endFight();
        this.isPlay = false;
        this.cc = 1;
        ETCJoystick.instance.activated = false;

        if(this.isShowLobby) {
            this.lobby.show();
			if(fightEndWaitTime == 0)
				this.startFight ();
        } else {
            this.clear();
        }
    }

    public void clearFight() {
        ResFactory.clear();
        Utils.clearObject(this.server);
        Utils.clearObject(this._selection);
        ResFactory.enable = true;
		this._selection = null;
        this.server = null;
    }



    public void startFight() {
        this.lobby.hide();
        this.cc = 2;
        this.isPlay = true;
//		Stage.inst.gameObject.SetActive (false);
        if(!this.isWatch) {
            if(FightMain.isLocal) {
                this.server = new ServerRunTime();
                this.server.init();
                if(FightMain.fightTest) {
                    this.addUser("0", 0, 0);
                    //if(FightMain.instance.mode == ConfigConstant.MODE_CHAOS) {
                    //    for(int i = 1, len = 11; i < len; i++) {
                    //        this.addUser("-" + i, i, i);
                    //    }
                    //}
                    
                } else {
                    this.addUser(PlayerData.instance.uid, 0, 0);
                }
            } else {
                if(this.isTestMode) {
                    NetAdapter.sendStartMatch2();
                } else {
                    NetAdapter.sendStartMatch();
                }
            }
        }
        
    }

    void Start() {
        if(!FightMain.fightTest) {
            this.onStart();
        }
    }


    public void onStart() {
        if(FightMain.fightTest) PlayerData.instance.uid = null;

        if(!FightMain.fightTest) {
            if(null != GameObject.Find("Stage").GetComponent<AudioSource>()) GameObject.Find("Stage").GetComponent<AudioSource>().enabled = false;
            GameObject.Find("Stage").transform.Find("Sound").GetComponent<AudioSource>().enabled = false;
            SoundManager.inst.SetMusic2(false, false);
        }

        

        if(FightMain.isLocal) {
            this.init();
        } else {
            NetAdapter.init(() => {
                this.init();
            });
        }
    }

    public void changeTest() {
        //FightMain.isTest = !FightMain.isTest;
        //NetSocket.test = true;
        //NetAdapter.delayCount = Convert.ToInt32(TestValue.value) / ConfigConstant.MAP_ACT_TIME_S;
        //NetAdapter.clear();
        //this.selection.regainMap();
        //Dictionary<string, object> data = this.selection.getData();


        this._selection.findEntity.createGrid2();
        //Log.debug(data);
    }
    

    public void addUser(string uid, int teamIndex, int index = 0) {

        if(null == this._selection) {
            ClientRunTime client = new ClientRunTime();
            client.teamIndex = teamIndex;
            client.scene = this.scene;
            client.uid = uid;
            client.init();

            client.index = index;
            this._selection = client;
        }
            

        if(FightMain.isLocal) {
            this.server.netWork.addUser(uid, new Dictionary<string, object> { {"cardGroup", PlayerData.instance.cardGroup.ToArray() }, { "teamIndex", teamIndex} });
        }
	}

	public ClientRunTime selection{
		get{ return this._selection;}
	}


	void Update() {
        if(!this.isPlay) return;
        if(Input.GetKeyUp(KeyCode.Escape)) {
            this.isPlay = false;
            this.cc = 4;
        } else if(Input.GetKeyUp(KeyCode.F1)) {
            this.isPlay = true;
            this.cc = 5;
        } else if(Input.GetKeyUp(KeyCode.A)) {
            isTest = !isTest;
        } else if(Input.GetKeyUp(KeyCode.S)) {
            //NetSocket.test = true;
            NetAdapter.delayCount = Convert.ToInt32(TestValue.value) / ConfigConstant.MAP_ACT_TIME_S;
        } else if(Input.GetKeyUp(KeyCode.W)) {
            FightMain.instance.selection.localPlayer.changeScore(-100000);
        } else if(Input.GetKeyUp(KeyCode.D)) {
            FightMain.instance.selection.localPlayer.changeScore(100000);
        } else if(Input.GetKeyUp(KeyCode.C)) {
            this.testUseCard();
        } else if(Input.GetKeyUp(KeyCode.Z)) {
            this._selection.refereeController.radishController.addTeamPoint(0, 1);
        } else if(Input.GetKeyUp(KeyCode.X)) {
            this._selection.refereeController.radishController.addTeamPoint(1, 1);
        } else if(Input.GetKeyUp(KeyCode.B)) {
            Dictionary<string, object> bulletData = new Dictionary<string, object> {
                        {"resId", "bulletNull"},
                        {"speed", 1},
                        {"lifeTime", 30},
                        {"atk", 100000},
                        {"range", new object[] { ConfigConstant.SHIP_RADIUS } },
                        {"posTarget", 1},
                        {"bangRes", "hitCollide"},
                    };
            BulletEntity bullet = this._selection.createFightEntity(ConfigConstant.ENTITY_BULLET) as BulletEntity;
            bullet.lockTarget = this._selection.localPlayer;
            bullet.owner = this._selection.localPlayer;
            bullet.initConfig(bulletData);
        } else if(Input.GetKeyUp(KeyCode.P)) {
            this._selection.startTime += 33f / 1000;
        }


        MediatorSystem.timeStart ("FightMain");
        //try {
            if(FightMain.isLocal && null != this.server) this.server.update();

            if(null != this._selection) this._selection.update();
        //} catch(Exception e) {
        //    LogMessage.instance.text.text += e.Message + "\n" + e.StackTrace;            
        //    NetAdapter.sendQuitFight();
        //}


        //if(Input.GetKeyUp(KeyCode.Keypad1)) {
        //    this.selection = this.clients[0];
        //}else if(Input.GetKeyUp(KeyCode.Keypad2)) {
        //    this.selection = this.clients[1];
        //}else if(Input.GetKeyUp(KeyCode.Keypad3)) {
        //    this.selection = this.clients[2];
        //}
		MediatorSystem.getRunTime ("FightMain");
    }

    internal void testUseCard() {

        MediatorSystem.timeStart("useCard");
        for(int i = 0, len = FightMain.instance.selection.players.Count; i < len; i++) {
            PlayerEntity player = FightMain.instance.selection.players[i];
            player.cardGroup.expendCard(1);
            //NetAdapter.sendUseCard(player.uid, 1);
        }
        MediatorSystem.getRunTime("useCard");
    }

    public override void OnDestroy(){        
        //base.OnDestroy();
        //Utils.clearObject(this);
	}
    

    public override void clear() {
        //try {
            if(null != this._selection) PlayerData.instance.rank = this._selection.getSortPlayer(1).IndexOf(this._selection.localPlayer) + 1;
            NetAdapter.clear();
            base.clear();
            this.clearFight();
            if(!FightMain.fightTest) {
                SoundManager.inst.SetMusic2(ModelManager.inst.userModel.isBGM, true);
                GameObject.Find("Main Camera").GetComponent<Camera>().enabled = true;
                GameObject.Find("Stage Camera").GetComponent<Camera>().enabled = true;
                if(null != GameObject.Find("Stage").GetComponent<AudioSource>()) GameObject.Find("Stage").GetComponent<AudioSource>().enabled = true;
                GameObject.Find("Stage").transform.Find("Sound").GetComponent<AudioSource>().enabled = true;
            }
        //} catch(Exception e) {
        //    Debug.Log(e);
        //}
        //if(this.isTestMode) {
        //    fightModel.fightType = "match_guide";
        //}
        DispatchManager.inst.Dispatch(new MainEvent(MainEvent.FIGHT_QUIT));
    }

}
