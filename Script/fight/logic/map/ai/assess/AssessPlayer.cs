using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

///被AI评估的玩家
public class AssessPlayer:BaseAIPlayer {
	///被AI评估的玩家原实体
	public PlayerEntity targetPlayer;
	///装备评估
	private AssessPart _assessPart;
	///卡牌评估
	private AssessCard _assessCard;
	///触发器评估
	private AssessTrigger _assessTrigger;
	///Buff评估
	private AssessBuff _assessBuff;

	//对这个target玩家的仇恨值
	public double anger;
	//对这个target玩家评估的精准性
	public double accuracy;

	public AssessPlayer (AIPlayer aiPlayer,PlayerEntity targetPlayer):base(aiPlayer) 
	{
		this.targetPlayer = targetPlayer;
		init ();
	}
	public void init () {
		//对自己的了解精准性高，队友其次，对手未知
//		double IQ = this.aiPlayer.IQ;
		if(this.targetPlayer == this.aiPlayer.player)
			this.accuracy = 1;
		else if(this.targetPlayer.teamIndex == this.aiPlayer.player.teamIndex)
			this.accuracy = 0.5;
		else 
			this.accuracy = 0;
		this.anger = 0;
		this._assessPart = new AssessPart (this);
		this._assessCard = new AssessCard (this);
		this._assessTrigger = new AssessTrigger (this);
		this._assessBuff = new AssessBuff (this);
	}
	public void update () {
		//时间的流逝冲刷仇恨
		this.anger *= 0.985;
	}
		



	//根据信息评估分析

	///当前生命折合评分
	public double getMarkHp(){
		return this.hpPer * AIConstant.MARK_HP_MAX;
	}
	///当前承受伤害比率
	public double getBearRate(){
		return ( 1 - this.targetPlayer.getProperty(ConfigConstant.PROPERTY_DEF_RATE) ) * this.targetPlayer.getProperty(ConfigConstant.PROPERTY_BEAR_RATE);
	}

	//评估攻防综合能力[前，侧，后]
	public double[] getMarkStrengthAngle(){
		double[] marks = this.getTypeMark(MarkVO.ASPECT_TYPE_ANGLE,MarkVO.VALUE_TYPE_LIFE);
		ArrayUtils.addArrayValue (marks,this.getMarkHp());
		ArrayUtils.multiplyArrayValue (marks, 1/this.getBearRate());

		ArrayUtils.addArray (marks, this.getTypeMark(MarkVO.ASPECT_TYPE_ANGLE,MarkVO.VALUE_TYPE_ATTACK));
		return marks;
	}
	///当前状况评估综合生存能力
	public double getMarkLife(){
		double[] marks = this.getTypeMark (MarkVO.ASPECT_TYPE_ANGLE, MarkVO.VALUE_TYPE_LIFE);
		return (ArrayUtils.sumArray(marks)/AIConstant.MARK_COUNT + this.getMarkHp()) / this.getBearRate();
	}
	///传入需要评估的范畴和内容类型，返回评估结果
	public double[] getTypeMark(int aspectType, int valueType){
		double[] marks = this._assessPart.getTypeMark(aspectType,valueType);
		double[] cardMarks = this._assessCard.getTypeMark(aspectType,valueType);
		double[] triggerMarks = this._assessTrigger.getTypeMark(aspectType,valueType);
		double[] buffMarks = this._assessBuff.getTypeMark(aspectType,valueType);

		for (int i = 0; i < AIConstant.MARK_COUNT; i++) {
			marks [i] += cardMarks [i]; //+ triggerMarks[i] + buffMarks[i];
		}
		return marks;
	}
	///传入需要评估的内容类型，返回评估结果
	public double getMark(int valueType) {
		return this._assessPart.getMark (valueType) +
				this._assessCard.getMark (valueType) +
				this._assessTrigger.getMark (valueType) +
				this._assessBuff.getMark (valueType);
	}


	///位移能力1~10000
	public double markMove {
		get{return 300 * currentSpeedMax;}
	}

	///得到当前评估的最大速度
	public double currentSpeedMax{
		get{ return this.targetPlayer.getProperty(ConfigConstant.PROPERTY_JOYSTICK_MAX) * this._map.speedRate;}
	}

	///当前是否可以爆发移动
	public bool canBurstMove{
		get{return this._assessCard.canBurstMove;}
	}
	///当前是否可以爆发防御
	public bool canBurstShield{
		get{return this._assessCard.canBurstShield;}
	}

	///得到血量百分比
	public double hpPer{
		get{ 
			return this.targetPlayer.hp / this.hpMax;
		}
	}
	///得到最大血量
	public double hpMax{
		get{ 
			return this.targetPlayer.getProperty (ConfigConstant.PROPERTY_HP);
		}
	}
	///得到若干毫秒之后 会出现的位置（按当前速度估算）
	public Vector2D getForecastPosition(double ms){
		double distance = this.targetPlayer.velocity.length * ms / ConfigConstant.MAP_ACT_TIME_S;
		Vector2D v2d = Vector2D.createVector2 (distance,this.targetPlayer.velocity.angle);
		v2d.add (this.targetPlayer.position);
		this.limitPosition (v2d);
		return v2d;
	}
	///有几个卡片需要获得能量，正在CD
	public int needPowerCardCount{
		get{
			int count = 0;
			for(int i=0;i<ConfigConstant.CARD_HAND_MAX;i++)
			{
				CardData card = this.targetPlayer.cardGroup.handCards [i];
				if (card != null && card.power < card.cost) {
					count++;
				}
			}
			return count;
		}
	}
}

