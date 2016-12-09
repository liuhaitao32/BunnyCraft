using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// 抢萝卜模式控制器
public class RadishController:TeamController {


	///加时赛胜出规则 先抢到萝卜，或超时后距离萝卜较近队伍（全部阵亡则阵亡地最近者队伍获胜）
	public const int OVER_TIME_GET_RADISH = 100;
	///加时赛胜出规则 击落萝卜，或保持萝卜直到分数反超获胜
	public const int OVER_TIME_DROP_RADISH = 101;

    
	///萝卜奖杯
	public RadishEntity radish;

    protected IntervalAction radishCD;

    public RadishController(Map map, int netId = -1):base(map, netId) {

    }
    

	public override void init (RefereeController refereeController)  {
		base.init (refereeController);
        this.radish = (RadishEntity)this._map.createFightEntity(ConfigConstant.ENTITY_RADISH);
        this.radish.initConfig((Dictionary<string, object>)ConfigConstant.combat["radish"]);
        this.radishCD = new IntervalAction(this._map).init(ConfigConstant.RADISH_POINT_CD, int.MaxValue, this.addPoint);
        this.radishCD.cd = this.radishCD.interval;
	}

    public void update() {
        if(this.radish.teamIndex != -1) {
            this.radishCD.update();
        }
    }

    
	///不考虑时间，判定战斗是否达成立即结束条件   ,返回值是战斗是否完结
	public override bool checkEnd () {
		if(base.checkEnd())
			return true;
		if (-1 != this.radish.teamIndex && this.teamPointArray [this.radish.teamIndex] >= ConfigConstant.RADISH_POINT_MAX) {
			this.teamWin(this.radish.teamIndex);
            this.dispatchEventWith(EventConstant.NOTIFY, 8);
            return true;
		}
		return false;
	}
	///不考虑时间，判定现状是否需要继续加时赛  ,返回值是战斗是否完结
	public override bool checkOverTime () {
		if (this.teamPointArray [0] == this.teamPointArray [1]) {
			//此时双方同分
			if (-1 != this.radish.teamIndex) {
				this.teamWin (this.radish.teamIndex);
                this.dispatchEventWith(EventConstant.NOTIFY, 0);
				return true;
			} else {
				//先抢到萝卜，或超时后距离萝卜较近队伍（全部阵亡则阵亡地最近者队伍获胜）
				this._refereeController.overTimeType = OVER_TIME_GET_RADISH;
				this._refereeController.resetCountDown(this._map.mapData.overTime * 1000);
                this.dispatchEventWith(EventConstant.NOTIFY, 1);
            }
		} else {
			int higherPointTeam = this.teamPointArray [0] > this.teamPointArray [1] ? 0 : 1;
			int lowerPointTeam = 1 - higherPointTeam;
            //持有萝卜的一方 分数大
			if (this.radish.teamIndex == -1 || this.radish.teamIndex == higherPointTeam) {
				this.teamWin (higherPointTeam);
                this.dispatchEventWith(EventConstant.NOTIFY, 2);
                return true;
			} else {
				//击落萝卜，或保持萝卜直到分数反超获胜
				this._refereeController.overTimeType = OVER_TIME_DROP_RADISH;
                this._refereeController.resetCountDown(this.radishCD.cd + ( this.teamPointArray[higherPointTeam] - this.teamPointArray[lowerPointTeam] ) * ConfigConstant.RADISH_FINAL_POINT_CD);
                this.dispatchEventWith(EventConstant.NOTIFY, 3);
            }
		}
		return false;
	}
	///不考虑时间，判定加时赛超时后最终的胜负   ,返回值是战斗是否完结
	public override bool checkOverEnd () {
		switch (this._refereeController.overTimeType) {
		    case RadishController.OVER_TIME_GET_RADISH://先抢到萝卜的获胜
                PlayerEntity player = this._map.getSortPlayer(0, this.radish.position)[0];
			    this.teamWin (player.teamIndex);
                this.dispatchEventWith(EventConstant.NOTIFY, 4);
                break;
		    case RadishController.OVER_TIME_DROP_RADISH:
			    this.teamWin (this.radish.teamIndex);
                this.dispatchEventWith(EventConstant.NOTIFY, 5);
                break;
		}
		return true;
	}
    

	///当前拥有者获得加点
	public void addPoint () {
		PlayerEntity player = this.radish.ownerPlayer;
        player.fightResult.addPoint (1);
		this.addTeamPoint (this.radish.teamIndex, 1);
	}
		

	///玩家获得萝卜旗
	public void gainRadish (PlayerEntity player) {
		this.radish.gainRadish();
        this.radishCD.cd = this.radishCD.interval;

		if (this._refereeController.overTimeType == RadishController.OVER_TIME_GET_RADISH) {
			//加时赛的掉落胜出条件，掉了就输
			this.teamWin (this.radish.teamIndex);
            this.dispatchEventWith(EventConstant.NOTIFY, 6);
        }
	}
	///玩家抛弃萝卜旗
	public void dropRadish () {
		if (this._refereeController.overTimeType == RadishController.OVER_TIME_DROP_RADISH) {
            //加时赛的掉落胜出条件，掉了就输
            this.teamWin(1 - this.radish.teamIndex);
            this.dispatchEventWith(EventConstant.NOTIFY, 7);
        }
		this.radish.dropRadish ();
	}
	///判定玩家是否抛弃萝卜旗
	public bool checkDropRadish (PlayerEntity player) {
		if (-1 != this.radish.teamIndex && player == this.radish.ownerPlayer) {
			this.dropRadish ();
			return true;
		}
		return false;
	}

    public override void enterFinal() {
        this.radishCD.interval = ConfigConstant.RADISH_FINAL_POINT_CD;
    }

    public override int type { get { return ConfigConstant.RADISH_CONTROLLER; } }


    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["radish"] = this.radish.netId;
        data["radishCD"] = this.radishCD.netId;
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        this.radish = (RadishEntity)this._map.getNetObject((int)data["radish"]);
        this.radishCD = (IntervalAction)this._map.getNetObject((int)data["radishCD"]);
        this.radishCD.intervalHandler = this.addPoint;
        base.setData(data);
    }

}
