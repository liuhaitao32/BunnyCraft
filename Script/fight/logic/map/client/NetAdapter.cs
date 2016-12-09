using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetAdapter {

    public delegate void invokeHandler();

    private static NetSocket socket;

    public static void connect(object[] data) {

    }


    public static ServerRunTime server { get { return FightMain.instance.server; } }



    public static void sendPackage(string command, Dictionary<string, object> dic) {
        if(FightMain.instance.isWatch) return;
        if(FightMain.isLocal) {
            if(null != server && dic["uid"] != null) server.netWork.receiveClient(command, dic["uid"].ToString(), dic);
        } else {
            socket.Send(command, dic);
        }
        //        

    }

    public static void sendQuitFight() {
        if(FightMain.isLocal) {
            if(null != FightMain.instance.selection) FightMain.instance.selection.endFight();
        } else {
            NetAdapter.sendPackage("stop_match", new Dictionary<string, object> { { "uid", PlayerData.instance.uid } });
        }

    }

    public static void sendStartMatch() {
        steps.Clear();
        sendPackage(ConfigConstant.NET_C_START_MATCH, new Dictionary<string, object> { { "room_id", FightMain.instance.roomId }, { "room_key", FightMain.instance.roomKey }, { "uid", PlayerData.instance.uid }, { "equal", FightMain.equal } });
    }

    public static void sendRoomInfo() {
        sendPackage(ConfigConstant.NET_C_GET_ROOM_INFO, new Dictionary<string, object> { { "room_id", FightMain.instance.roomId }, { "uid", PlayerData.instance.uid } });
    }


    public static void sendStartMatch2() {
        steps.Clear();
        sendPackage(ConfigConstant.NET_C_START_MATCH2, new Dictionary<string, object> { { "room_id", FightMain.instance.roomId } });
        Debug.Log("sendStartMatch2");
    }

    public static void sendReady() {
        steps.Clear();
        int teamIndex = FightMain.instance.mode == ConfigConstant.MODE_CHAOS ? Mathf.CeilToInt(UnityEngine.Random.value * 10000000) : 0;
        string uid = FightMain.instance.mode == ConfigConstant.MODE_CHAOS ? PlayerData.instance.uid : PlayerData.instance.uid;
        List <object> group = PlayerData.instance.cardGroup.ConvertAll<object>((e) => { return new object[] { e["id"], e["level"] }; });
        sendPackage(ConfigConstant.NET_C_USER_JOIN, new Dictionary<string, object> { { "uid", uid }, { "room_id", FightMain.instance.roomId }, { "equal", FightMain.equal }, { "cards", group.ToArray() }, { "teamIndex", teamIndex }, { "lv", 1 } });
        Debug.Log("sendReady");
    }

    //public static void sendReady2() {
    //    sendPackage("sssss", new Dictionary<string, object> { { "uid", PlayerData.instance.uid }, { "room_id", FightMain.instance.roomId }, { "equal", FightMain.equal }, { "cards", PlayerData.instance.cardGroup.ToArray() }, { "teamIndex", Mathf.CeilToInt(UnityEngine.Random.value * 10000000) }, { "lv", 1 } });
    //    Debug.Log("sendReady");
    //}

    public static void sendReplay(int id, int start = 0, int end = 9999) {
        sendPackage("match_rePlay", new Dictionary<string, object> { { "match_log_id", id }, { "start", start }, { "end", end } });
    }

    public static void sendChangeMove(string uid, double x, double y, int netId) {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["axisX"] = Utils.toInt(x);
        dic["axisY"] = Utils.toInt(y);
        dic["uid"] = uid;
        dic["netId"] = netId;
        //server.netWork.receiveClient(ConfigConstant.NET_S_CHANGE_MOVE, uid, dic);
        sendPackage(ConfigConstant.NET_S_CHANGE_MOVE, dic);

        //Log.debug ("move" + x + "         " + y);
    }


    public static void sendUseCard(string uid, int index) {

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["index"] = index;
        dic["uid"] = FightMain.instance.selection.uid;
        //		server.netWork.receiveClient(ConfigConstant.NET_S_CHANGE_MOVE, uid, dic);
        sendPackage(ConfigConstant.NET_S_USE_CARD, dic);

        //Log.debug ("move" + x + "         " + y);
    }

    ///AI使用卡牌，Biggo添加
    public static void sendUseCardAI(string uid, int index) {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["index"] = index;
        dic["uid"] = uid;
        sendPackage(ConfigConstant.NET_S_USE_CARD, dic);
    }

    public static int delayCount = 1;

    public static List<List<Dictionary<string, object>>> delayPackage = new List<List<Dictionary<string, object>>>();

    public static void receive(string uid, List<Dictionary<string, object>> actions) {
        if(FightMain.instance.selection.uid != uid) {
            return;
        }
        invoke(() => {
            delayPackage.Add(actions);
            //getClient(uid).receive(actions);
            if(delayPackage.Count >= delayCount) {
                for(int i = 0, len = delayPackage.Count; i < len; i++) {
                    FightMain.instance.selection.receive(delayPackage[i]);
                }
                delayPackage.Clear();
                delayCount = 1;
            }
        });
    }


    private static void stopMatch(VoSocket vo) {

        FightMain.instance.equalPanel.fightId.text = FightMain.instance.id = ( (Dictionary<string, object>)vo.data )["id"].ToString();
        Debug.Log("stopMatch");
        FightMain.instance.onStart();
    }

    public static void init(Action complete) {
        Dictionary<string, object> data = PlayerData.instance.data;
        clear();
        socket = new NetSocket();
        socket.AddListener("init_user", initUser);
        socket.AddListener("sync", sync);
        //socket.AddListener("get_room_info", getRoomInfo);
        socket.AddListener("stop_match", stopMatch);
        socket.AddListener("match_rePlay", matchRePlay);
        socket.AddListener("not_room", onNoteRoom);
        object[] server = data["match_server"] as object[];
        socket.Start(1, server[0].ToString(), (int)( server[1] ), 1000);
        socket.onConnect = () => {
            socket.onConnect = null;
            TimerManager.inst.Add(0, 1, (e) => {
                Debug.Log("网络连接成功");
                complete();
            });
        };


    }

    private static void onNoteRoom(VoSocket vo) {
        Debug.Log("onNoteRoom");
        if(FightMain.fightTest) {
            FightMain.instance.onStart();
        } else {
            FightMain.instance.clear();
        }

    }

    private static void matchRePlay(VoSocket vo) {
        Debug.Log("matchRePlay");
        Dictionary<string, object> data = vo.data as Dictionary<string, object>;
        FightMain.instance.testPanel.runServer2((object[])data["frame_list"], (object[])data["map_data_list"], (int)data["start"], (int)data["end"]);
    }

    public static void clear() {
        mapData.Clear();
        if(null != socket) {
            socket.RemoveListeners();
            socket.Close();
            socket = null;
        }

    }

    private static void getRoomInfo(VoSocket vo) {
        //FightMain.instance.lobby.updateRoomInfo((Dictionary<string, object>)vo.data);
    }

    private static long t = 0;

    private static double t2 = 0;

    public static List<object> mapData = new List<object>();

    public static int voId = -1;

    private static void sync(VoSocket vo) {
        //Log.debug("收到服务器的间隔时间：" + ( System.DateTime.Now.Ticks - t ));

        t = System.DateTime.Now.Ticks;
        invoke(() => {
            Dictionary<string, object> map = vo.data as Dictionary<string, object>;
            object[] objs = map["ops"] as object[];
            double offset = Math.Abs(t2 - (double)map["time"]);
            //			if(offset > 101 || offset < 99){
            //				MediatorSystem.log("error", offset);
            //			}
            //			MediatorSystem.log("data", offset);

            t2 = (double)map["time"];
            if(map.ContainsKey("map_data")) {
                mapData.Add(map["map_data"]);
            }
            if(-1 != voId && voId + 1 != (int)map["sync_num"]) {
                throw new Exception("voId对不上了！ 请检查！");
            }
            voId = (int)map["sync_num"];
            syncOne(objs);
            //	List<object> list = new List<object> ((object[])data ["sync"]);

        });
    }

    public static void sendFightResult(bool v) {
        sendPackage(ConfigConstant.NET_C_FIGHT_EQUAL, new Dictionary<string, object> { { "equal", v } });
    }

    private static int total = 0;
    private static int curr = 0;

    public static Dictionary<string, object> steps = new Dictionary<string, object>();


    public static void syncOne(object[] objs) {
        //if(!FightMain.instance.isWatch) {
        //string uid = FightMain.instance.selection.uid + "_" + FightMain.instance.selection.teamIndex;
        //if(!steps.ContainsKey(uid)) steps[uid] = new List<object>();
        //( (List<object>)steps[uid] ).Add(objs);
        //}
        total++;
        if(objs.Length >= 1) {
            List<Dictionary<string, object>> dic = new List<Dictionary<string, object>>();

            for(int i = 0, len = objs.Length; i < len; i++) {
                List<object> list = new List<object>((object[])objs[i]);
                Dictionary<string, object> data = (Dictionary<string, object>)list[1];
                string uid = null == list[2] ? "" : list[2].ToString();
                string key = list[0].ToString();
                dic.Add(new Dictionary<string, object> {
                    {"type", key },
                    {"data", data },
                    {"uid", uid }
                });
            }
            receive(FightMain.instance.selection.uid, dic);
            curr++;
        } else {
            receive(FightMain.instance.selection.uid, null);
        }
    }


    private static void initUser(VoSocket vo) {
        voId = -1;
        FightMain.instance.lobby.hide();
        FightMain.instance.cc = 3;
        FightMain.instance.isPlay = true;
        Debug.Log("recevie:initUser");
        Dictionary<string, object> data = (Dictionary<string, object>)vo.data;
        string uid = data["uid"].ToString();
        int teamIndex = (int)data["teamIndex"];
        PlayerData.instance.uid = uid;
        FightMain.instance.addUser(uid, teamIndex);
        mapData.Clear();
    }


    public static void invoke(invokeHandler invoke, double time = 0.1f) {
        //RenderManager.Instance ().AddTimeOver ((double x)=>{invoke.Invoke();}, time);
        invoke();
    }

}

