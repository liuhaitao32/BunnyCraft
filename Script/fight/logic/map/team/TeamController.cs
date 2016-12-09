using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// 组队战模式控制器
public class TeamController:NetObjectBase {

	public static int TEAM_COUNT = 2;
	///当前双方得分
	public List<int> teamPointArray;
	///双方玩家的快捷列表
	public List<List<PlayerEntity>> playerListArray;

	protected RefereeController _refereeController;

	///胜出的team
	public int winnerTeam = -1;

    public TeamController(Map map, int netId = -1):base(map, netId) {

    }
    
	public virtual void init (RefereeController refereeController) {
		this._refereeController = refereeController;
        this.teamPointArray = new List<int>();
		this.playerListArray = new List<List<PlayerEntity>>();
		for (int i = 0; i < TEAM_COUNT; i++) {
			this.playerListArray.Add(new List<PlayerEntity> ());
            this.teamPointArray.Add(0);
		}
        
        for(int i = 0, len = this._map.players.Count; i < len; i++) {
            PlayerEntity player = this._map.players[i];
            this.playerListArray[player.teamIndex].Add(player);
        }
    }

    

	///队伍胜利
	public void teamWin (int team) {
		this.winnerTeam = team;
		//如果分数相平，胜利方+1分
		if (this.teamPointArray [this.winnerTeam] == this.teamPointArray [1 - this.winnerTeam]) {
			this.addTeamPoint (this.winnerTeam);
		}
	}
	///指定队伍获得加点
	public void addTeamPoint (int team,int value = 1) {
		this.teamPointArray[team] += value;
        this.dispatchEventWith(EventConstant.ADD_POINT, team);
    }

	///不考虑时间，判定战斗是否达成立即结束条件   ,返回值是战斗是否完结
	public virtual bool checkEnd () {
		return winnerTeam >=0;
	}
	///不考虑时间，判定现状是否需要继续加时赛  ,返回值是战斗是否完结
	public virtual bool checkOverTime () {
		return false;
	}
	///不考虑时间，判定加时赛是否满足结束条件   ,返回值是战斗是否完结
	public virtual bool checkOverEnd () {
		return false;
	}


    public virtual void enterFinal() {

    }



    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["teamPointArray"] = this.teamPointArray.ConvertAll<object>((e)=> { return e; }).ToArray();
        data["playerListArray"] = this.playerListArray.ConvertAll<object>((List<PlayerEntity> arr) => { return arr.ConvertAll<object>((PlayerEntity p) => { return p.netId; }).ToArray(); }).ToArray();
        data["refereeController"] = this._refereeController.netId;
        data["winnerTeam"] = this.winnerTeam;
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        this.teamPointArray = new List<object>((object[])data["teamPointArray"]).ConvertAll<int>((object o)=> { return (int)o; });
        this.playerListArray = new List<object>((object[])data["playerListArray"]).ConvertAll<List<PlayerEntity>>((object o)=> {
            return new List<object>((object[])o).ConvertAll<PlayerEntity>((object id)=> {
                return (PlayerEntity)this._map.getNetObject((int)id);
            });
        });
        this._refereeController = (RefereeController)this._map.getNetObject((int)( data["refereeController"] ));
        this.winnerTeam = (int)data["winnerTeam"];
        base.setData(data);
    }
}
