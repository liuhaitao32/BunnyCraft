using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///战斗裁判管理器
public class RefereeController : NetObjectBase {

    public const int TIME_TYPE_NORMAL = 0;
    public const int TIME_TYPE_FINAL = 1;
    public const int TIME_TYPE_OVER = 2;
    public const int TIME_TYPE_END = -2;

    public bool isFinal { get { return this.timeType >= RefereeController.TIME_TYPE_FINAL; } }
    public bool isOver { get { return this.timeType == RefereeController.TIME_TYPE_OVER; } }
    public bool isEnd { get { return this.timeType == RefereeController.TIME_TYPE_END; } }

    ///加时赛类别（达成不同条件决定胜负）
    public int overTimeType;

    public RadishController teamController;

    public RadishController radishController { get { return (RadishController)teamController; } }

    ///战斗阶段 0正常时间 1能量时间 2加时时间 -2战斗逻辑已结束
    public int timeType = RefereeController.TIME_TYPE_NORMAL;

    public TimeAction totalTime;

    public RefereeController(Map map, int netId = -1):base(map, netId) {

    }

    public void enterFinal() {
        this.timeType = RefereeController.TIME_TYPE_FINAL;
        if(null != this.teamController) this.teamController.enterFinal();
    }

    public void init () {
        if(this._map.mapData.isRadish) {
            this.teamController = new RadishController(this._map);
            this.teamController.init(this);
        }
        
        this.totalTime = this._map.addDelayCall(this._map.mapData.fightTime * 1000, this.checkNextTime, ConfigConstant.MAP_CALL_BACK_TOTAL_TIME);
    }
		
	//战斗模式时间相关运算，返回值是战斗是否完结
	public void update () {
        if(this.totalTime.isFinish || ( null != this.teamController && this.teamController.checkEnd() )) {
            this.timeType = RefereeController.TIME_TYPE_END;
            this._map.finish();
            return;
        }
        if(null != this.teamController) this.teamController.update();
    }

    public void resetCountDown(int time) {
        this.totalTime.totalTime = time;
        this.totalTime.reset();
    }


	///判定战斗是否进入下一阶段  ,返回值是战斗是否完结
	private void checkNextTime () {
        if(this.timeType == RefereeController.TIME_TYPE_FINAL) {
            if(this._map.mapData.overTime > 0) {
                if(null != this.teamController) {
                    //需要加时
                    if(!this.teamController.checkOverTime()) {
                        this.timeType = RefereeController.TIME_TYPE_OVER;
                    }
                }
            }
        } else if(this.timeType == RefereeController.TIME_TYPE_OVER) {
            if(null != this.teamController) this.teamController.checkOverEnd();
        }
	}
    
    
	///得到持有萝卜的队伍
	public int getRadishTeam () {
        return ( (RadishController)this.teamController ).radish.teamIndex;
	}
	///得到萝卜
	public RadishEntity getRadish () {
        return this.radishController.radish;
	}
	///玩家得到萝卜
	public void gainRadish (PlayerEntity player) {
        this.radishController.gainRadish(player);
	}


	///判定玩家是否抛弃萝卜旗
	public bool checkDropRadish (PlayerEntity player) {
        return this.radishController.checkDropRadish (player);
	}

	///指定队伍获得加点
	public void addTeamPoint (int team,int value = 1) {
        this.teamController.addTeamPoint(team, value);
	}

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["totalTime"] = this.totalTime.netId;
        data["overTimeType"] = this.overTimeType;
        if(null != this.teamController) {
            data["teamController"] = this.teamController.netId;
        }
        
        data["timeType"] = this.timeType;
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        this.totalTime = (TimeAction)this._map.getNetObject((int)( data["totalTime"] ));
        this.totalTime.callBack = this.checkNextTime;
        this.overTimeType = (int)data["overTimeType"];
        if(data.ContainsKey("teamController")) {
            this.teamController = (RadishController)this._map.getNetObject((int)( data["teamController"] ));
        }
        
        this.timeType = (int)data["timeType"];
        base.setData(data);
    }

    public override int type { get { return ConfigConstant.REFEREE_CONTROLLER; } }
}
