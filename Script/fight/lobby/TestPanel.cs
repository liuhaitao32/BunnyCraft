using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;
using System.IO;


public class TestPanel : FightViewBase {

    public Button load_server_btn;

    public L_Input input;

    public L_Button select_btn;

    public Button copy_btn;

    public Button read_btn;

    public Button close_btn;

    public Text path1;
    public Text path2;

    public Text log;


    private string _id = "";

    private List<L_Button> selects = new List<L_Button>();


    void Start() {
        this.path1.text = getPath() + "\\fight.txt";
        this.path2.text = getPath() + "\\fightRead.txt";

        this.close_btn.onClick.AddListener(() => {
            this.hide();
        });

        this.copy_btn.onClick.AddListener(() => {
            if(FightMain.instance.selection != null) {
                try {
                    NetAdapter.steps["server"] = NetWorkServer.steps;
                    this.writeFile(JsonMapper.ToJson(NetAdapter.steps));
                    
                    this.logDebug("写入成功");
                } catch(Exception e) {
                    this.logDebug("写入失败");
                }
            }
        });

        this.read_btn.onClick.AddListener(this.updateBtn);

        this.load_server_btn.onClick.AddListener(this.loadServer);
    }

    private void loadServer() {
        //NetAdapter.sendReplay(int.Parse(input.text));
        NetAdapter.sendReplay(29343, 999999);
    }

    public void logDebug(string message) {
        this.log.text += message + "\n";
    }

    public void writeFile(string str) {
        StreamWriter file;
        FileInfo f = new FileInfo(this.path1.text);
        if(f.Exists) {
            file = f.CreateText();
        } else {
            file = f.CreateText();
        }
        file.Write(str);
        file.Close();
        file.Dispose();
    }

    public string readFile() {
        StreamReader file = null;
        FileInfo f = new FileInfo(this.path2.text);
        if(!f.Exists) {
            this.logDebug("没有文件");
            return "";
        } else {
            file = f.OpenText();
        }
        //file.Close();
        //file.Dispose();
        
        return file.ReadToEnd();
    }

    private string getPath() {
        string path = Application.persistentDataPath;
        int index = path.IndexOf("Android");
        if(index != -1) {
            path = path.Remove(index);
        }
#if UNITY_EDITOR
        path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#endif
        return path;
    }


    public void show() {
        this.gameObject.SetActive(true);
        this.log.text = "打印框:\n";

        this.updateBtn();

    }

    private void updateBtn() {
        while(this.selects.Count != 0) { Destroy(this.selects[0].gameObject); this.selects.RemoveAt(0); };
        try {
            string str = this.readFile();
            if(str == "") return;

            FightMain.ccccc = "";

            Dictionary<string, object> dic = JsonMapper.ToObject<Dictionary<string, object>>(str);

            foreach(string key in dic.Keys) {
                L_Button btn = ResFactory.createObject<L_Button>(this.select_btn);
                ( (RectTransform)btn.transform ).sizeDelta = new Vector2(80, 50);
                btn.transform.SetParent(this.transform);
                btn.transform.localPosition = new Vector3(-52 + ( this.selects.Count % 2 ) * 100, 69 + (int)( this.selects.Count / 2 ) * 100, 0);
                btn.SetText(key);
                this.selects.Add(btn);

                new TestSelecteItem(key, btn).addListener("change", (e) => {
                    string uid = e.data.ToString().Split('_')[0];
                    int teamIndex = Convert.ToInt32(e.data.ToString().Split('_')[1]);
                    FightMain.instance.autoRunClient.startFight((object[])dic[uid], uid, teamIndex, ConfigConstant.MODE_TEST_WATCH);
                });
            }


        } catch(Exception e) {
            this.logDebug("解析错误");
        }

    }
    

    public void runServer2(object[] keyFrame, object[] mapData, int start, int end) {
        ServerRunTime server = new ServerRunTime();
        server.init();
        string clientId = "";
        int teamIndex = 0;
        List<List<Dictionary<string, object>>> steps = new List<List<Dictionary<string, object>>>();
        int mapDataIndex = 0;
        for(int i = 0, len = keyFrame.Length + 1; i < len; i++) {
            if(keyFrame.Length > i) {
                object[] keyDatas = (object[])keyFrame[i];
                List<Dictionary<string, object>> keyFrameList = new List<Dictionary<string, object>>();
                steps.Add(keyFrameList);
                for(int j = 0, len2 = keyDatas.Length; j < len2; j++) {
                    List<object> list = new List<object>((object[])keyDatas[j]);
                    Dictionary<string, object> data = (Dictionary<string, object>)list[1];
                    string uid = null == list[2] ? "" : list[2].ToString();
                    string key = list[0].ToString();
                    server.netWork.receiveClient(key, uid, data);
                    keyFrameList.Add(new Dictionary<string, object> {
                        {"type", key },
                        {"data", data },
                        {"uid", uid }
                    });
                    if(key == ConfigConstant.NET_C_CREATE_USER && clientId == "") {
                        clientId = uid;
                        teamIndex = (int)data["teamIndex"];
                    }
                    
                }
            }
            
            //server.nextFrame();
            //server.getData();
            if(server.stepIndex >= start && server.stepIndex <= end && false) {
                Dictionary<string, object> frameData = mapData[mapDataIndex] as Dictionary<string, object>;
                try {
                    Dictionary<string, object> equalInfo = ViewUtils.equal(server.getData(), frameData, "");
                    if(equalInfo.Count > 0) {
                        this.logDebug(ViewUtils.toString(equalInfo));
                        break;

                    }
                } catch(Exception e) {
                    Debug.Log("sdfsdfsfd");
                }
                
                mapDataIndex++;
            }
        }

        this.logDebug("比较完毕");
        FightMain.instance.autoRunClient.startFight(keyFrame, clientId, teamIndex, ConfigConstant.MODE_WATCH);
    }
    
    public void hide() {
        this.gameObject.SetActive(false);
    }
    
}

class TestSelecteItem :EventClass {


    public TestSelecteItem(string uid, L_Button btn) {
        btn.onClick.AddListener(() => {
            dispatchEventWith("change", uid);
        });

    }
}
