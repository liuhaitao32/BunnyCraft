using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class RadishScoreView : ScoreView {
    

    public override void init(ClientRunTime clientRunTime) {
        base.init(clientRunTime);
        this.radishController.addListener(EventConstant.ADD_POINT, (e) => {
            int team = (int)e.data;
            this.ballArray[team].updateTeamPoint(this.teamController.teamPointArray[team]);
            this.updateBallsScale();
        });

        this.radishController.radish.addListener(EventConstant.GAIN_RADISH, (e) => {
            this.updateBallsGain();
            this.updateBallsScale();
        });

        this.radishController.radish.addListener(EventConstant.DROP_RADISH, (e) => {
            this.updateBallsGain();
            this.updateBallsScale();
        });
        this.updateBallsGain();
        this.radishController.addListener(EventConstant.NOTIFY, this.notityHandler);

        this.addRadioGuide("配合队友抢夺方块");
    }

    private void notityHandler(MainEvent obj) {
        int type = (int)obj.data;
        int winnerTeam = this.teamController.winnerTeam;
        switch(type) {
            case 0:
                this.addRadioGuide(string.Format("双方同分,{0}持有方块,胜！", this.getTeamName(winnerTeam)), this.getTeamColor(winnerTeam));
                break;
            case 1:
                int sec = Mathf.CeilToInt(1f * this._clientRunTime.refereeController.totalTime.time / 1000);
                this.addRadioGuide(string.Format("{0}秒内抢夺方块，即可获胜！", sec));
                break;
            case 2:
                this.addRadioGuide(string.Format("{0}分数较高,胜！", this.getTeamName(winnerTeam)), this.getTeamColor(winnerTeam));
                break;
            case 3:
                sec = Mathf.CeilToInt(1f * this._clientRunTime.refereeController.totalTime.time / 1000);
                if(this.radishController.radish.teamIndex == this._clientRunTime.teamIndex) {
                    this.addRadioGuide(string.Format("{0}秒内守护方块，即可获胜！", sec));
                } else {
                    this.addRadioGuide(string.Format("{0}秒内击落方块，即可获胜！", sec));
                }
                break;
            case 4:
                this.addRadioGuide(string.Format("{0}距离方块较近,胜！", this.getTeamName(winnerTeam)), this.getTeamColor(winnerTeam));
                break;
            case 5:
                this.addRadioGuide(string.Format("{0}分数反超,胜！", this.getTeamName(winnerTeam)), this.getTeamColor(winnerTeam));
                break;
            case 6:
                this.addRadioGuide(string.Format("{0}抢得方块,胜！", this.getTeamName(winnerTeam)), this.getTeamColor(winnerTeam));
                break;
            case 7:
                this.addRadioGuide(string.Format("{0}击落方块,胜！", this.getTeamName(winnerTeam)), this.getTeamColor(winnerTeam));
                break;
            case 8:
                this.addRadioGuide(string.Format("{0}达到满分,胜！", this.getTeamName(winnerTeam)), this.getTeamColor(winnerTeam));
                break;

        }
    }

    protected override TeamBall createBall() {
        return ResFactory.createObject<GameObject>(ResFactory.loadPrefab("radishBall")).GetComponent<RadishBall>();
    }

    private RadishController radishController { get { return ( (RadishController)this.teamController ); } }

    ///重置所有计分球持有萝卜状态
    public void updateBallsGain() {
        for(int i = 0, len = this.ballArray.Count; i < len; i++) {
            RadishBall ball = (RadishBall)this.ballArray[i];
            ball.updateGainImage(this.radishController.radish.teamIndex == ball.team);
        }
    }
    

    ///按当前状况排序队伍球，令其改变顺序(缩放)
	public override void updateBallsScale() {
        int temp = 0;
        if(this.teamController.teamPointArray[0] > this.teamController.teamPointArray[1]) {
            temp++;
        } else if(this.teamController.teamPointArray[0] < this.teamController.teamPointArray[1]) {
            temp--;
        }

        if(this.radishController.radish.teamIndex == 0) {
            temp++;
        } else if(this.radishController.radish.teamIndex == 1) {
            temp--;
        }
        this.ballArray[0].updateScale(temp);
        this.ballArray[1].updateScale(-temp);
    }
    
}
