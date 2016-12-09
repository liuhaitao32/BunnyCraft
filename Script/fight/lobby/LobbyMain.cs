using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LobbyMain : FightViewBase {

    public Button change_mode_btn;

    public Button enter_btn;

    public Button ready_btn;

    public Button add_user_btn;
    public Text add_user_text;

    public Text fight_id;

    public Text count_down;

    public GameObject cards;

    public L_Input roomId;

    public GameObject fightUI;

    public List<string> allCards = new List<string>();

    public List<CardIcon> selectCards = new List<CardIcon>();


    public static LobbyMain instance;

    public bool isOpen = false;


    private int _totalTime = 0;

    protected override void preInit() {
        instance = this;
        this.enter_btn.onClick.AddListener(this.onEnterHandler);
        this.ready_btn.onClick.AddListener(this.onReadyHandler);
        this.roomId.onValueChanged.AddListener((e) => {
            FightMain.instance.roomId = this.roomId.text.ToString();
            LocalStore.SetLocal("fight_room_id", FightMain.instance.roomId);
        });

        this.add_user_btn.onClick.AddListener(() => {
            int a = int.Parse(this.add_user_text.text.Replace("加人", ""));
            this.add_user_text.text = "加人" + (a + 1);
            NetSocket socket = new NetSocket();
            socket.Start(1, PlatForm.inst.SERVER4, 7777, 1000);
            socket.onConnect = () => {
                socket.onConnect = null;
                TimerManager.inst.Add(0, 1, (e) => {
                    socket.Send(ConfigConstant.NET_C_USER_JOIN, new Dictionary<string, object> { { "room_id", FightMain.instance.roomId }, { "equal", FightMain.equal }, { "cards", PlayerData.instance.cardGroup.ToArray() }, { "teamIndex", Mathf.CeilToInt(UnityEngine.Random.value * 10000000) }, { "lv", 1 } });
                    socket.Close();
                });
            };
        });

        Text text = Tools.FindChild2("Text", this.change_mode_btn.gameObject).GetComponent<Text>();
        this.change_mode_btn.onClick.AddListener(() => {
            if(text.text == "乱斗") {
                text.text = "组队";
                FightMain.instance.mode = ConfigConstant.MODE_RADISH;
            } else {
                text.text = "乱斗";
                FightMain.instance.mode = ConfigConstant.MODE_CHAOS;
            }
        });

    }

    void Start() {
        string rid = LocalStore.GetLocal("fight_room_id");
        if("" == rid) {
            rid = FightMain.instance.roomId;
        }
        if(FightMain.fightTest) {
            this.roomId.text = FightMain.instance.roomId = rid;
        }
        

    }

    //private void updateTime(float time) {
    //    this._totalTime = (int)Math.Max(0, this._totalTime - time * 1000);
    //    this.updateView();
    //}

    public void init() {
        //if(FightMain.isLocal) return;
        //foreach(string key in ConfigConstant.combat.Keys) {
        //    if(key.IndexOf('C') == 0) {
        //        this.allCards.Add(key);
        //    }
        //}
        this.allCards = new List<object>((object[])DataManager.inst.combat["testCard"]).ConvertAll<string>((e)=> { return e.ToString(); });
		//Biggo临时测试用
		//this.allCards = new List<string>(){"C501", "C507", "C501", "C507", "C501", "C507", "C501", "C507" };
        this.selectCards.Clear();
        for(int i = 0, len = 8; i < len; i++) {
            GameObject go = Tools.FindChild2("card_" + i, this.cards);
            if(this.allCards.Count != 0) {
                this.selectCards.Add(new CardIcon(go, this.allCards[0]));
                this.allCards.RemoveAt(0);
            } else {
                go.SetActive(false);
            }
        }
        this.setCardsData();
    }

    public void setCardsData() {
        if(FightMain.fightTest) {
            PlayerData.instance.cardGroup = this.selectCards.ConvertAll<Dictionary<string, object>>((e) => {
                return new Dictionary<string, object> { { "id", e.id }, { "level", 1 } };
            });
        }        
    }

    private void onReadyHandler() {
        //this.ready_btn.enabled = false;
        this.ready_btn.gameObject.SetActive(false);
        this.enter_btn.gameObject.SetActive(true);
        NetAdapter.sendReady();
    }

    public void show() {
        if(this.isOpen) {
            print("已经显示出来了。");
        }
        this.add_user_text.text = "加人0";
        bool b = FightMain.instance.isTestMode;

        this.ready_btn.gameObject.SetActive(b);
        this.add_user_btn.gameObject.SetActive(b);
        this.add_user_text.gameObject.SetActive(b);

        this.enter_btn.gameObject.SetActive(!b);
        this.change_mode_btn.gameObject.SetActive(b);

        this.fightUI.SetActive(false);
        this.gameObject.SetActive(true);
        this.fight_id.text = "战斗id：" + FightMain.instance.id;




        //跳过大厅直接进游戏。
        this.onReadyHandler();
        //NetAdapter.sendRoomInfo();
    }

    public void hide() {
        this.isOpen = false;
        this.fightUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void onEnterHandler() {
        //SceneManager.LoadScene("fight", LoadSceneMode.Additive);
        //onReadyHandler();
        //临时凑的数据。
        if(FightMain.fightTest) {
            if(FightMain.instance.mode == ConfigConstant.MODE_RADISH) {
                object[] users = new object[8];
                PlayerData.instance.data["users"] = users;


                for(int i = 0, len = users.Length; i < len; i++) {
                    //[users]:uid lv effor_lv name head["use"] cards[id, lv];
                    users[i] = new object[] {
                                            (i), 1, 1, "name" + (i), null, PlayerData.instance.cardGroup.ConvertAll<object>((e)=> {return new object[] { e["id"], e["level"]}; }).ToArray(), "ship800", 50
                                            };
                }

            } else {
                PlayerData.instance.data.Remove("users");
            }
        }
        
        FightMain.instance.startFight();

    }
  

    public override void clear() {
        base.clear();
    }
}
