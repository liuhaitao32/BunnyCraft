using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

///评估控制器（用于记录对手信息，评估局势和决策）
public class AssessController:BaseAIPlayer {
	///本场内玩家(也包含自己)的评估（包含威胁性和仇恨性，各个威胁性）列表
	private Dictionary<string,AssessPlayer> _assessPlayerDic;
	///对自己的评估
	public AssessPlayer selfAssess{
		get{ return this._assessPlayerDic [this._aiPlayer.player.uid];}
	}


	public AssessController (AIPlayer aiPlayer):base(aiPlayer) 
	{
		this.init ();
	}
	public void init () {
		this._assessPlayerDic = new Dictionary<string, AssessPlayer> ();
		this.findAllyPlayer (this._aiPlayer.player.uid);
		//		assessPlayerDic.Add (ownerPlayer, new AssessPlayer (ownerPlayer));
	}


	public void reset () {
		//重置评估

	}

	public void update () {
		foreach (KeyValuePair<string,AssessPlayer> pair in this._assessPlayerDic) {
			AssessPlayer assessPlayer = pair.Value;
			assessPlayer.update ();
		}
	}

	///发现了敌对玩家
	public void findEnemyPlayer (string uid)
	{
		AssessPlayer assessPlayer = this.getAssessPlayer (uid);
		this.angerAssessPlayer (assessPlayer,0.1);
	}
	///发现了盟友玩家
	public void findAllyPlayer (string uid)
	{
		this.getAssessPlayer (uid);
	}


	///得到对某个玩家的评估，如果没有则新建
	public AssessPlayer getAssessPlayer (string uid)
	{
		AssessPlayer assessPlayer;
		if (this._assessPlayerDic.ContainsKey (uid)) {
			assessPlayer = this._assessPlayerDic [uid];
		} else {
			PlayerEntity player = this._map.getPlayer(uid);
			assessPlayer = new AssessPlayer (this._aiPlayer,player);
			this._assessPlayerDic.Add (uid, assessPlayer);
		}
		return assessPlayer;
	}
	///仇视某个玩家，最大值到1
	public void angerPlayer (string uid,double value)
	{
		this.angerAssessPlayer(this.getAssessPlayer (uid),value);
	}
	///仇视某个玩家，最大值到1
	public void angerAssessPlayer (AssessPlayer assessPlayer,double value)
	{
		if (assessPlayer.anger < value)
			assessPlayer.anger = value;

		assessPlayer.anger += value*0.5;
		assessPlayer.anger = Math.Min (1, assessPlayer.anger);
	}
		
	///评估按当前状况，面对指定对手时，战意值最佳距离，以及位移的能力评估
	public SituationVO getSituationVO (AssessPlayer assessPlayer)
	{
		//战意
		double[] moraleRange = this.getMoraleToPlayer (assessPlayer);
		//找到最高值
		double moraleMax = 0;
		double moraleAverage = 0;
		int moraleMaxIndex = 0;
		int distanceMin = 0;
		int distanceMax = 10000;


		for (int i = 0; i < AIConstant.MARK_COUNT; i++) {
			double morale = moraleRange [i];
			moraleAverage += morale;
			if (moraleRange [i] > moraleMax) {
					moraleMax = morale;
				moraleMaxIndex = i;
			}
		}
		moraleAverage /= AIConstant.MARK_COUNT;

		if (moraleAverage < AIConstant.MORALE_NEAR_MIN) {
			//如果平均值很低，逃跑保命吧还是
			distanceMin = AIConstant.RANGE_DISTANCE_MAX;
			distanceMax = AIConstant.RANGE_DISTANCE_AFOCAL;
		} 
		else if (moraleAverage >= AIConstant.MORALE_NEAR_MAX) {
			//如果战力差距巨大，任意距离都是ok的，尽量靠近些
			distanceMin = AIConstant.RANGE_DISTANCE_MIN;
			distanceMax = AIConstant.RANGE_DISTANCE_MIDDLE;
		}
		else{
			if (moraleMaxIndex == 0) {
				//近距离最优
				distanceMin = AIConstant.RANGE_DISTANCE_MIN;
				if (moraleRange [1] >= AIConstant.MORALE_MIDDLE) {
					//中距离也不错，中近战位
					if (moraleRange [2] >= AIConstant.MORALE_MIDDLE) {
						//远距离都还行
						distanceMax = AIConstant.RANGE_DISTANCE_MIDDLE;
					} else {
						distanceMax = AIConstant.RANGE_DISTANCE_NEAR;
					}
				} else {
					//只在近战位
					distanceMax = AIConstant.RANGE_DISTANCE_VERY_NEAR;
				}
			} else if (moraleMaxIndex == 1) {
				//中距离最优
				if (moraleRange [0] >= AIConstant.MORALE_MIDDLE) {
					//近距离也不错
					distanceMin = AIConstant.RANGE_DISTANCE_VERY_NEAR;
					if (moraleRange [2] >= AIConstant.MORALE_MIDDLE) {
						//远距离也不错
						distanceMax = AIConstant.RANGE_DISTANCE_VERY_FAR;
					} else {
						//不要远距离
						distanceMax = AIConstant.RANGE_DISTANCE_FAR;
					}
				} 
				else
				{
					distanceMin = AIConstant.RANGE_DISTANCE_NEAR;
					if (moraleRange [2] >= AIConstant.MORALE_MIDDLE) {
						//近距离不行，远距离不错
						distanceMax = AIConstant.RANGE_DISTANCE_VERY_FAR;
					} else {
						//只在中位
						distanceMax = AIConstant.RANGE_DISTANCE_FAR;
					}
				}
			} else {
				//远距离最优
				distanceMax = AIConstant.RANGE_DISTANCE_MAX;
				if (moraleRange [1] >= AIConstant.MORALE_MIDDLE) {
					//中距离也不错，中远位
					if (moraleRange [0] >= AIConstant.MORALE_MIDDLE) {
						//近距离都还行
						distanceMin = AIConstant.RANGE_DISTANCE_VERY_NEAR;
					} else {
						distanceMin = AIConstant.RANGE_DISTANCE_MIDDLE;
					}
				} else {
					//只在远程位
					distanceMin = AIConstant.RANGE_DISTANCE_FAR;
				}
			}

			//战意越高，越倾向于靠近，反之保持距离，幅度
			double rate;
			if (moraleAverage > AIConstant.MORALE_MIDDLE) {
				//自身越强越靠近
				rate = (moraleAverage - AIConstant.MORALE_MIDDLE) / (AIConstant.MORALE_NEAR_MAX - AIConstant.MORALE_MIDDLE);
				distanceMin = (int)(distanceMin * (1 - rate) + AIConstant.RANGE_DISTANCE_MIN * rate);
				distanceMax = (int)(distanceMax * (1 - rate) + AIConstant.RANGE_DISTANCE_NEAR * rate);
			} else if (moraleAverage < AIConstant.MORALE_MIDDLE) {
				//自身越弱越拉远
				rate = (AIConstant.MORALE_MIDDLE- moraleAverage) / (AIConstant.MORALE_MIDDLE - AIConstant.MORALE_NEAR_MIN);
				distanceMin = (int)(distanceMin * (1 - rate) + AIConstant.RANGE_DISTANCE_FAR * rate);
				distanceMax = (int)(distanceMax * (1 - rate) + AIConstant.RANGE_DISTANCE_AFOCAL * rate);
			}
		}
		return new SituationVO (moraleAverage,distanceMax,distanceMin);
	}


	///评估不同距离的战意值
	private double[] getMoraleToPlayer (AssessPlayer assessPlayer)
	{
		//攻击防御分开，如果自身攻击无法压住对方生命，战意将大幅下降
		double[] selfAttackRange = this.selfAssess.getTypeMark(MarkVO.ASPECT_TYPE_RANGE,MarkVO.VALUE_TYPE_ATTACK);
		double selfLife = this.selfAssess.getMarkLife();
		double[] assessAttackRange = assessPlayer.getTypeMark(MarkVO.ASPECT_TYPE_RANGE,MarkVO.VALUE_TYPE_ATTACK);
		double assessLife = assessPlayer.getMarkLife();

		//一般性愤怒和针对性仇恨
		double anger = this._aiPlayer.moodController.anger + assessPlayer.anger;
		double courage = this._aiPlayer.moodController.courage;

		//		string log = string.Format ("{0}({1:F2})对{2} 仇恨 {3:F2}", ownerPlayer.name, ai.moodController.anger, assessPlayer.player.name, assessPlayer.anger);
		//		Debug.Log (log);

		double[] moraleRange = new double[AIConstant.MARK_COUNT];

		for (int i = 0; i < AIConstant.MARK_COUNT; i++) {
			moraleRange [i] = (Math.Pow (selfAttackRange [i], 1.3) + selfLife) / (Math.Pow (assessLife, 1.3) + assessAttackRange [i]);
			//仇恨情绪够高，战意高昂，勇气不足，战意低下
			moraleRange [i] += anger * 0.4 + (courage-0.5) *0.6;
		}
		return moraleRange;
	}
}


