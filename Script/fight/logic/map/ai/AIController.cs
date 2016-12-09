using System;
using System.Collections.Generic;

/// AI控制器
public class AIController:BaseAI {
	private static string AI_UID = "ai_";

	private int _aiNum;
	private List<AIPlayer> _aiPlayers;

	public AIController(Map map):base(map) {
		init ();
	}

	public void init() {
		this._aiNum = 0;
		this._aiPlayers = new List<AIPlayer>();
	}

	public void update() {
		//测试临时补满AI
		if(_aiNum < AIConstant.AI_PLAYER_NUM)
			addUser ();
		for(int i = 0,len = this._aiPlayers.Count; i < len; i++) {
			//执行AI，传递出摇杆和用牌信息
			AIPlayer aiPlayer = this._aiPlayers [i];
			aiPlayer.update ();
		}
//		MediatorSystem.timeStart("【ai】find");
//		for(int i = 0,len = this._aiPlayers.Count; i < len; i++) {
//			AIPlayer aiPlayer = this._aiPlayers [i];
//			aiPlayer.findController.update();
//		}
//		MediatorSystem.getRunTime("【ai】find");
//
//		MediatorSystem.timeStart("【ai】assess");
//		for(int i = 0,len = this._aiPlayers.Count; i < len; i++) {
//			AIPlayer aiPlayer = this._aiPlayers [i];
//			aiPlayer.assessController.update ();
//		}
//		MediatorSystem.getRunTime("【ai】assess");
//
//		MediatorSystem.timeStart("【ai】tactics");
//		for(int i = 0,len = this._aiPlayers.Count; i < len; i++) {
//			AIPlayer aiPlayer = this._aiPlayers [i];
//			aiPlayer.tacticsController.update();
//		}
//		MediatorSystem.getRunTime("【ai】tactics");
//
//		MediatorSystem.timeStart("【ai】mood");
//		for(int i = 0,len = this._aiPlayers.Count; i < len; i++) {
//			AIPlayer aiPlayer = this._aiPlayers [i];
//			aiPlayer.moodController.update();
//		}
//		MediatorSystem.getRunTime("【ai】mood");
	}
	///生成新玩家
	public void addUser() {		
		string uid = "-" + (this._aiNum + 1);
        Dictionary<string, object> dic = (Dictionary<string, object>)( (object[])ConfigConstant.combat["testAI"] )[this._aiNum];

        this._aiNum++;

        
        //TODO:这里是后端准备的人物数据
        Dictionary<string, object> playerData = new Dictionary<string, object> {
            { "name", dic["name"].ToString()},
            { "uid", uid},
            { "cardGroup", (object[])dic["cardGroup"]},
            { "shipId", "ship800"},
            { "level", 1},
            { "teamIndex", this._aiNum},
            { "head", dic["head"].ToString()}
        };
        double IQ = Convert.ToDouble(dic["ai"]);
        //FightMain.instance.server.netWork.addUser(uid, );
        PlayerEntity player = (PlayerEntity)this._map.createFightEntity(ConfigConstant.ENTITY_PLAYER);
        player.initConfig(playerData);
//		PlayerEntity player = FightMain.instance.selection.
		this.openAI (player, IQ);
	}
	///托管自己
	public void switchSelfAI() {		
		string uid = FightMain.instance.selection.uid;
		PlayerEntity player = this._map.getPlayer(uid);
//		PlayerEntity player = FightMain.instance.server.getPlayer(uid);
		if (this.isAI (player))
			this.closeAI (player);
		else
			this.openAI (player, 0.5);
	}
	///某个玩家开启托管
	public void openAI(PlayerEntity player, double IQ) {
		AIPlayer aiPlayer;
		for(int i = this._aiPlayers.Count - 1; i >= 0; i--) {
			aiPlayer = this._aiPlayers[i];
			if(aiPlayer.player == player) return;
		}
		aiPlayer = new AIPlayer (this._map,player, IQ);
		this._aiPlayers.Add(aiPlayer);
	}
	///某个玩家关闭托管
	public void closeAI(PlayerEntity player) {
		AIPlayer aiPlayer;
		for(int i = this._aiPlayers.Count - 1; i >= 0; i--) {
			aiPlayer = this._aiPlayers[i];
			if (aiPlayer.player == player) {
				aiPlayer.clear ();
				this._aiPlayers.Remove (aiPlayer);
				return;
			}
		}
	}
	///判断某玩家是否开启托管
	public bool isAI(PlayerEntity player) {
		AIPlayer aiPlayer;
		for(int i = this._aiPlayers.Count - 1; i >= 0; i--) {
			aiPlayer = this._aiPlayers[i];
			if(aiPlayer.player == player) return true;
		}
		return false;
	}

	///重新随机场上所有AI的IQ
	public void resetIQ(double value) {		
		AIPlayer aiPlayer;
		for(int i = this._aiPlayers.Count - 1; i >= 0; i--) {
			aiPlayer = this._aiPlayers[i];
			aiPlayer.resetIQ (value);
		}
	}

	///清除所有AI相关player
	public void clearAll() {	
		while(this._aiPlayers.Count >0)
		{
			AIPlayer aiPlayer = this._aiPlayers [0];
//			if (aiPlayer.player.uid != FightMain.instance.selection.uid) {
//				aiPlayer.player.clear ();
//			}
			aiPlayer.clear ();
			this._aiPlayers.Remove (aiPlayer);
		}	
	}
}
