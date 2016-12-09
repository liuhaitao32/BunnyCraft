using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreView : FightViewBase, IClientView {
    ///仅限显示的计分球
    public List<TeamBall> ballArray = new List<TeamBall>();

    protected ClientRunTime _clientRunTime;

    protected TeamController teamController;

    protected override void preInit() {
        base.preInit();
    }

    public virtual void init(ClientRunTime clientRunTime) {
        this._clientRunTime = clientRunTime;
        this.teamController = this._clientRunTime.refereeController.teamController;
        //this.teamController.addListener
        this.resetBalls();
        this.updateBallsPoint();
        this.updateBallsScale();
        this.initColor();
    }

    public virtual void resetBalls() {
        this.ballArray.Clear();
        ViewUtils.clearChildren(this.transform);
        for(int i = 0, len = this.teamController.teamPointArray.Count; i < len; i++) {
            TeamBall ball = this.createBall();
            this.ballArray.Add(ball);
            ball.init(this.transform, i);
        }
    }

    protected virtual TeamBall createBall() {
        return ResFactory.createObject<GameObject>(ResFactory.getOther("teamBall")).GetComponent<TeamBall>();
    }

    ///更新点数
	public virtual void updateBallsPoint() {
        for(int i = 0, len = this.teamController.teamPointArray.Count; i < len; i++) {
            TeamBall ball = this.ballArray[i];
            ball.updateTeamPoint(this.teamController.teamPointArray[i]);
        }
    }

    ///按当前状况排序队伍球，令其改变顺序(缩放)
	public virtual void updateBallsScale() {
        int temp = 0;
        if(this.teamController.teamPointArray[0] > this.teamController.teamPointArray[1]) {
            temp++;
        } else if(this.teamController.teamPointArray[0] < this.teamController.teamPointArray[1]) {
            temp--;
        }
        this.ballArray[0].updateScale(temp);
        this.ballArray[1].updateScale(-temp);
    }

    ///按当前玩家视角给队伍球上色
	public void initColor() {
        if(!this._clientRunTime.mapData.isTeam) {
            return;
        }
        for(int i = 0, len = this.ballArray.Count; i < len; i++) {
            if(this._clientRunTime.teamIndex == ballArray[i].team) {
                this.ballArray[i].changeColor(1);
            } else {
                this.ballArray[i].changeColor(2);
            }
        }
    }

    public void onUpdate(float rate) {
        
    }

    public void addRadioGuide(string message, Color color) {
        this._clientRunTime.scene.addRadioGuide(message, color);
    }

    public void addRadioGuide(string message) {
        this._clientRunTime.scene.addRadioGuide(message);
    }

    ///得到我方或敌方的字符串
	public string getTeamName(int team) { return team == this._clientRunTime.teamIndex ? "我方" : "敌方"; }

    ///获得某队伍的正常颜色
    public Color getTeamColor(int team = -1) {
        if(team < 0)
            return Color.gray;
        else if(this._clientRunTime.teamIndex == team)
            return ViewConstant.teamColors[1][2];
        else
            return ViewConstant.teamColors[2][2];
    }
}
