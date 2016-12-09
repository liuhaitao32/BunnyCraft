using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager:MonoBehaviour,IClientView {
    

    public Text time;

    private Card[] _initCards = new Card[ConfigConstant.CARD_HAND_MAX];

    public Rank rank = new Rank();
    
    public GameObject testBtn;

    public Text stepText;

    public InputField stepInputText;

    public Button testBtnChange;

    public Button voiceBtn;

    public GameObject testCanvas;

    public ScoreView scoreView;

    public Text playerReliveText;

    private PlayerEntity player {
        get { return FightMain.instance.selection.localPlayer; }
    }

    public UIManager() {

    }

    void Awake() {
        GameObject go = ResFactory.loadPrefab("card");
        for(int i = 0, len = this._initCards.Length; i < len; i++) {
            Card card = ResFactory.createObject<GameObject>(go).GetComponent<Card>();
            card.index = i;
            this._initCards[i] = card;

            card.transform.SetParent(Tools.FindChild2("bottomLayer", this.gameObject).transform, false);
            card.transform.localPosition = new Vector3(230 + i * 130, 75, 0);
        }
        this.testBtnChange.onClick.AddListener(() => {
            this.testBtn.SetActive(!this.testBtn.activeSelf);
        });

        this.voiceBtn.onClick.AddListener(() => {
            this.testCanvas.SetActive(!this.testCanvas.activeSelf);
        });

    }
    

    public void init() {
        ///这个要替换其他的btn。
        bool visible = Utils.equal(DataManager.inst.systemSimple, "testBtn", 1);
        this.voiceBtn.gameObject.SetActive(visible);

        ClientRunTime clientRunTime = FightMain.instance.selection;
        clientRunTime.registerClientView(this);
        this.rank.init(Tools.FindChild2("topRightLayer/scoreRankLayer", this.gameObject));
        //TODO:断网重连 这个不知道会不会有问题。 可能有坑。 先留着。
        this.initCards();

        GameObject scoreGame = Tools.FindChild2("topRightLayer/teamBallsLayer", this.gameObject);
        if(clientRunTime.mapData.isTeam) {
            this.scoreView = scoreGame.GetComponent<RadishScoreView>();
            if(null == this.scoreView) this.scoreView = scoreGame.AddComponent<RadishScoreView>();

            scoreView.init(clientRunTime);
            scoreGame.SetActive(true);
        } else {
            scoreGame.SetActive(false);
        }
        

        clientRunTime.addListener(EventConstant.END, (e) => {
            GameObject finish = ResFactory.createObject<GameObject>(ResFactory.getOther("finish"));
            finish.transform.SetParent(this.transform, false);
            TimerManager.inst.Add(2, 1, (t) => {
                Destroy(finish);
            });
        });

        this.videoChange();

        clientRunTime.addListener(EventConstant.LOGIC_COMPLETE, (e) => {
            if(null != clientRunTime.localPlayer.reviveAction) {
                this.playerReliveText.enabled = true;
                //TODO:改成语言配置
                string str = string.Format("复活中...{0:F1}", clientRunTime.localPlayer.reviveAction.time / 1000f);
                this.playerReliveText.text = str;
            } else {
                this.playerReliveText.enabled = false;
            }
        });
    }
    

    private void initCards(MainEvent e = null) {
        for(int i = 0, len = this.player.cardGroup.handCards.Count; i < len; i++) {
            this._initCards[i].cardData = this.player.cardGroup.handCards[i];
        }
        this.player.cardGroup.addListener(EventConstant.CHANGE, initCards);
    }

    void Start() {
        //TODO:写成这样是因为 这个类的生存周期就是战斗本身。
        FightMain.instance.addListener(EventConstant.PLAYER_IN, (MainEvent e) => {
            init();
        });
        this.setTest();
        //重新设置下卡牌的样子。
        FightMain.instance.addListener(EventConstant.CLIENT_CHANGE, (MainEvent e) => {
            Dictionary<string, object> dic = (Dictionary<string, object>)e.data;
            ClientRunTime oldClient = (ClientRunTime)dic["old"];
            ClientRunTime newClient = (ClientRunTime)dic["new"];
            if(null != oldClient && null != oldClient.localPlayer) oldClient.localPlayer.cardGroup.removeListener(EventConstant.CHANGE, initCards);
            if(null != newClient && null != newClient.localPlayer) this.initCards();

        });
    }

    public void videoChange() {
        bool b = this.player.map.mapData.isWatch;
        //this.getTestComponent<Button>("pause").gameObject.SetActive(b);
        this.getTestComponent<Button>("goFrame").gameObject.SetActive(b);
        this.getTestComponent<Button>("addPlayer").gameObject.SetActive(!b);
        
        //this.stepInputText.text = FightMain.instance.testPanel.totalIndex.ToString();
        //this.stepInputText.gameObject.SetActive(b);

        //Tools.FindChild2("Joystick", this.gameObject).SetActive(!b);
    }

    private void setTest() {
        this.getTestComponent<Button>("breakNet").onClick.AddListener(NetAdapter.sendQuitFight);
        this.getTestComponent<Button>("test").onClick.AddListener(FightMain.instance.changeTest);
        


        this.getTestComponent<Button>("goFrame").onClick.AddListener(() => {
            int totalIndex = int.Parse(this.stepInputText.text);
            FightMain.instance.autoRunClient.totalIndex = totalIndex;
            this.stepInputText.text = FightMain.instance.autoRunClient.totalIndex.ToString();
            FightMain.instance.selection.pause = false;
        });
        this.getTestComponent<Button>("addPlayer").onClick.AddListener(() => {
            FightMain.instance.addUser(UnityEngine.Random.value.ToString(), FightMain.instance.selection.players.Count);
            //TimerManager.inst.flag = !TimerManager.inst.flag;

        });

        this.getTestComponent<Button>("pause").onClick.AddListener(() => {
            FightMain.instance.isPlay = !FightMain.instance.isPlay;
            FightMain.instance.cc = 6;
        });

        this.getTestComponent<Button>("test1").onClick.AddListener(() => {
			//biggo修改
			FightMain.instance.selection.aiController.addUser();
        });
        this.getTestComponent<Button>("test2").onClick.AddListener(() => {
			//biggo修改
			FightMain.instance.selection.aiController.switchSelfAI();
        });
        this.getTestComponent<Button>("test3").onClick.AddListener(() => {
			//biggo修改
			string uid = FightMain.instance.selection.uid;
			PlayerEntity cPlayer = FightMain.instance.selection.getPlayer(uid);
			PlayerEntity sPlayer = FightMain.instance.server.getPlayer(uid);
			Debug.Log("C:"+cPlayer.position + "  \nS:"+sPlayer.position);
        });
        this.getTestComponent<Button>("test4").onClick.AddListener(() => {
            //biggo修改
            //AIPlayer.SHOW_PATH = !AIPlayer.SHOW_PATH;
            TestValue.test4 = !TestValue.test4;

            for(int i = 0, len = FightMain.instance.selection.players.Count; i < len; i++) {
                FightMain.instance.selection.players[i].setJoystick(new Vector2D());
            }
        });

        List<int> cc = new List<int>();
        for(int i = 0, len = 100001; i < len; i++) {
            cc.Add(i);
        }
        this.getTestComponent<Button>("test5").onClick.AddListener(() => {
            //if(null != FightMain.instance.selection) FightMain.instance.selection.scene.beanLayer.gameObject.SetActive(!FightMain.instance.selection.scene.beanLayer.gameObject.activeSelf);
            MediatorSystem.log("testzzz", FightMain.instance.cc);
        });
        this.getTestComponent<Button>("test6").onClick.AddListener(() => {
            //if(null != FightMain.instance.selection) FightMain.instance.selection.scene.bulletLayer.gameObject.SetActive(!FightMain.instance.selection.scene.bulletLayer.gameObject.activeSelf);
            TestValue.test6 = !TestValue.test6;
            
            FightMain.instance.testUseCard();

        });
        this.getTestComponent<Button>("test7").onClick.AddListener(() => {
            //if(null != FightMain.instance.selection) FightMain.instance.selection.scene.bulletLayer.gameObject.SetActive(!FightMain.instance.selection.scene.bulletLayer.gameObject.activeSelf);
            TestValue.test7 = !TestValue.test7;
        });
        this.getTestComponent<Button>("test8").onClick.AddListener(() => {
            //if(null != FightMain.instance.selection) FightMain.instance.selection.scene.bulletLayer.gameObject.SetActive(!FightMain.instance.selection.scene.bulletLayer.gameObject.activeSelf);
            TestValue.test8 = !TestValue.test8;
        });
			
		this.getTestComponent<Slider> ("aiSlider").onValueChanged.AddListener((float value) => {
			//biggo添加
			AIConstant.IQ_DEFAULT = value;
			FightMain.instance.server.AI_IQ = value;
			FightMain.instance.server.aiController.resetIQ(value);
		});
    }



    private T getTestComponent<T>(string name) {
        return (T)Tools.FindChild2(name, this.testBtn).GetComponent<T>();
    }

    public void onUpdate(float rate) {


        string t = Tools.TimeFormat(Convert.ToInt64(this.player.map.refereeController.totalTime.time) * 10000);
        this.time.text = "" == t ? "0:00" : t.Substring(4);

        if(this.player.map.mapData.isWatch) {
            this.stepText.text = FightMain.instance.selection.stepIndex + "/" + FightMain.instance.autoRunClient.steps.Count;
        } else {
            this.stepText.text = FightMain.instance.selection.stepIndex.ToString();
        }

        for(int i = 0, len = this._initCards.Length; i < len; i++) {
            this._initCards[i].onUpdate(rate);
        }
    }

}
