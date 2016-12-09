using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FightResult : NetObjectBase {

    public int score = 0;
    //整场总人数
    public int killNum = 0;
    //一条命杀敌最大数
    public int lifeKillNum = 0;

    public int hurtMax = 0;
    //最久的存活时间
    public int totalLifeTime = 0;
    //死亡次数
    public int deathNum = 0;
    //当前记录生存时间
    public int currLifeTime = 0;
    //当前击杀敌人的次数
    public int currLifeKillNum = 0;

    public int scoreLevel = 0;


    /// 快速连杀剩余累计秒数
    public int comboKillLastTime;

    /// 快速连杀
    public int comboKillNum;



    ///整局内击杀敌人次数
    public int recordKill;
    ///整局内被击杀次数
    public int recordBeKill;
    ///整局内击杀持有萝卜者次数
    public int recordShot;
    ///整局内持有萝卜时被击杀次数
    public int recordBeShot;
    ///整局内持有萝卜获得的总分
    public int recordPoint;



    public FightResult(Map map, int netId = -1) : base(map, netId) {

    }

    public void init() {
        this.score = ConfigConstant.SCORE_INIT;
    }

    public void update() {
        this.currLifeTime += ConfigConstant.MAP_ACT_TIME_S;
        this.comboKillNum -= ConfigConstant.MAP_ACT_TIME_S;
    }

    public void killPerson() {
        this.comboKillNum = this.comboKillLastTime <= 0 ? 1 : this.comboKillNum + 1;
        this.currLifeKillNum++;
        this.comboKillLastTime = ConfigConstant.COMBO_KILL_TIME_MAX;

        this.killNum++;
        this.lifeKillNum = Math.Max(this.lifeKillNum, this.currLifeKillNum);
    }

    public void dead() {
        this.currLifeTime = 0;
        this.currLifeKillNum = 0;
        this.comboKillLastTime = 0;
        this.comboKillNum = 0;
    }

    public override int type { get { return ConfigConstant.FIGHT_RESULT; } }

    public void changeScore(int value) {
        this.score = Math.Max(this.score + value, 0);
        //积分等级变更
        if(value > 0) {
            while(this.scoreLevel < ConfigConstant.scoreConfig.Length - 1 && this.score >= (int)((Dictionary<string, object>)ConfigConstant.scoreConfig[this.scoreLevel])["scoreMax"]) {
                this.scoreLevel++;
            }
        } else if(value < 0) {
            while(this.scoreLevel > 0 && this.score < (int)( (Dictionary<string, object>)ConfigConstant.scoreConfig[this.scoreLevel - 1] )["scoreMax"]) {
                this.scoreLevel--;
            }
        }
    }

    public override Dictionary<string, object> getData() {
        return base.getData();
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
    }

    public void addPoint(int value) {
        
    }
}

