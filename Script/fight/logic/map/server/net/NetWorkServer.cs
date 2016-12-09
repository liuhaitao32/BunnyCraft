using System;
using System.Collections.Generic;
using UnityEngine;

public class NetWorkServer {


	private delegate void Handler(string uid, Dictionary<string, object> dic);

	private Dictionary<string, Handler> _commands = new Dictionary<string, Handler>();


	public ServerRunTime server;
    /// <summary>
    /// 这个我为了方便写成了static。 但是这个是后端保留的整个数据。要在结束的时候保存到本地的。
    /// </summary>
    public static Dictionary<int, List<Dictionary<string, object>>> steps = new Dictionary<int, List<Dictionary<string, object>>>();

	public NetWorkServer() {
		this.registerCommand ();
	}


	public void sendClient(string uid, List<Dictionary<string, object>> actions){
        if(!this.server.mapData.isWatch) NetAdapter.receive (uid, actions);
	}

	public void receiveClient(string command, string uid, Dictionary<string, object> data){
        //Log.debug("服务端接收到信息" + command);
        if(this._commands.ContainsKey(command)) {
            //if(this.server.mapData.isWatch) {
            //    int stepIndex = this.server.stepIndex;
            //    if(!steps.ContainsKey(stepIndex)) steps[stepIndex] = new List<Dictionary<string, object>>();
            //    steps[stepIndex].Add(new Dictionary<string, object> { { "command", command }, { "uid", uid }, { "data", data } });
            //}
            this._commands[command].Invoke(uid, data);
        }
		
	}

	public void registerCommand() {
        this._commands [ConfigConstant.NET_C_CREATE_USER] = this.createUser;//这个是为了跑本地测试环境用的。
        this._commands [ConfigConstant.NET_S_ADD_USER] = this.addUser;
		this._commands [ConfigConstant.NET_S_CHANGE_MOVE] = this.changeMove;
        this._commands [ConfigConstant.NET_S_USE_CARD] = this.useCard;
    }

    private void createUser(string uid, Dictionary<string, object> dic) {
        if(null != this.server.getPlayer(uid)) return;
        PlayerEntity player = this.server.createFightEntity(ConfigConstant.ENTITY_PLAYER) as PlayerEntity;
        player.initConfig(dic);
    }

    private void useCard(string uid, Dictionary<string, object> dic) {
        PlayerEntity player = this.server.getPlayer(uid);
        if(null != player) {
            int index = (int)( dic["index"] );
            player.cardGroup.expendCard(index);
        }
        this.server.addNetAction(ConfigConstant.NET_C_USE_CARD, dic);
    }

    private void changeMove(string uid, Dictionary<string, object> dic) {
        PlayerEntity player = this.server.getPlayer (uid);
        if(null != player) {
            if(player.hasBuff(ConfigConstant.BUFF_STUN)) return;
            Vector2D joystick = Vector2D.createVector(Utils.toFloat((int)dic["axisX"]), Utils.toFloat((int)dic["axisY"]));
            player.setJoystick(joystick);
            joystick.clear();
            dic["netId"] = player.netId;
        }        
		this.server.addNetAction (ConfigConstant.NET_C_CHANGE_MOVE, dic);
	}


	public void addUser(string uid, Dictionary<string, object> dic) {		
        //地图所有的生成信息。
        this.server.addNetAction(ConfigConstant.NET_C_INIT_MAP, this.server.getData(), uid);
        PlayerEntity player = this.server.getPlayer(uid);

        object[] cardGroup = (object[])dic["cardGroup"];//{id:C001, level:1}
        Dictionary<string, object> head = FightMain.fightTest ? null : ModelManager.inst.userModel.head;
        //TODO:这里是后端准备的人物数据
        Dictionary<string, object> playerData = new Dictionary<string, object> {
            { "name", FightMain.fightTest && uid == ModelManager.inst.userModel.uid ? ModelManager.inst.userModel.GetUName() : uid},
            { "uid", uid},
            { "cardGroup", cardGroup},
            { "shipId", this.server.mapData.isTeam ? "team1" : "ship800"},
            { "level", 1},
            { "teamIndex", dic["teamIndex"]},
            { "head", head}
        };

        this.server.createUser(playerData);
        //生成一个人。 但是要通知所有的人更新。 但是这个可能会造成 一帧有两个人 又加到createEntity里 又假如addUser里. 这边我客户端过滤吧.
        this.server.addNetAction(ConfigConstant.NET_C_CREATE_USER, playerData);
    }


}

