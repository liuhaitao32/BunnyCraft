using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// 分析得到的战术节点。在战略下会考虑若干条战术，战术是一种计划。根据主要目标确定战术后，会根据周边其他情况对战术评分，存在多种战术执行最佳评分的战术
public class BaseTactic:BaseAIPlayer 
{	
	///战术的类别，同类的战术也可能考虑对不同目标的方案
	public int type;

	/// 战术针对的主要目标
	public FightEntity target;

	/// 战术的重要性（同类下对比，最大为1）
	protected double important;
	/// 战术的发现机会（自身）
	protected double discoverChance;
	/// 战术的考虑优先级（综合对比）
	public double thinkPriority;


	///与目标预判位置相对距离下，【近 中 远】混合系数，总共加起为1
	protected double[] deltaRangeScales;
	///与目标预判位置相对角度下，【前 侧 后】混合系数，总共加起为1
	protected double[] deltaAngleScales;



	//思考之后确定

	/// 战术的出牌优先级
	public double cardPriority;
	/// 战术的移动优先级
	public double movePriority;


	// 出牌决策的各类目标值参数，吻合程度越高越好，超出会降分

	///生存向需求
	protected double life;
	///位移向需求
	protected double move;
	///伤害向需求
	protected double damage;
	///控制向需求
	protected double control;
	///推远向需求(次要需求)
	protected double push;
	///多目标需求(次要需求)
	protected double aim;
	///生效速度需求(次要需求)
	protected double fast;
	///命中率需求（次要需求 仅限伤害控制）
	protected double hit;
	///额外优先级卡牌ID
	protected string[] bestCardIds;



	/// 最终移动目标位置
	public Vector2D moveAimPosition;
	/// 最终目标移动控速，默认1
	public double moveAimSpeedScale;
	/// 最终目标朝向角度,如果为最小值double.MinValue，则不强制在最终转向
	public double moveAimAngle;

	/// 最终使用卡牌目标位置
	public Vector2D cardAimPosition;

	/// 战术的推论价值，以之后半秒的可能状况预测，是否是好的战术。如果价值为最小值则未初始化
	//	public int value;


	public BaseTactic (AIPlayer aiPlayer,FightEntity target, double important):base(aiPlayer) 
	{
		this.important = important;
		this.target = target;
		this.init ();
	}

	public void init(){
		this.moveAimPosition = Vector2D.createVector();
//		this.cardAimPosition = Vector2D.createVector();
	}

	///判断是否重视，发现该战术
	public virtual bool checkDiscover(){
		if (this.getRandom() <= this.discoverChance)
			return true;
		else
			return false;
	}

	///思考决策，生成该战术的细节，出牌位置和移动方案
	public virtual void thinkTactic(){
	}

	///判定当前状况使用各个牌的适应度
	public double[] getCardFitnessArray(){
		List<CardData> handCards = this._aiPlayer.player.cardGroup.handCards;
		double[] fitnessList = new double[AIConstant.MARK_COUNT];
		this.updateAimDeltaScales ();
		for (int i = 0; i < ConfigConstant.CARD_HAND_MAX; i++) {
			CardData card = handCards [i];
			if (null != card) {
				if (card.canUse) {
					MarkVO mark = MarkVO.getCardMark (card.id);
					fitnessList [i] = this.getCardFitness (mark);
				} else
					fitnessList [i] = 0;
			}
		}
		return fitnessList;
	}
	///更新计算，与目标预判位置相对距离角度下，各位置的混合系数
	protected void updateAimDeltaScales(){
		//有出牌目标位置
		if (null != this.cardAimPosition) {
			//判定与目标点（已经预判）的相对方向和距离
			PlayerEntity player = this._aiPlayer.player;
			Vector2D deltaV2d = Collision.realPosition(player.position,cardAimPosition,this._map.mapData).deltaPos;
			double deltaDistance = deltaV2d.length;
			this.deltaRangeScales = MarkVO.getFitnessRangeMarks (deltaDistance);

			double deltaAngle = Math2.deltaAngle (deltaV2d.angle, player.angle);
			this.deltaAngleScales = MarkVO.getFitnessAngleMarks (deltaAngle);
		} else {
			this.deltaRangeScales = AIConstant.MARK_DEFAULT_SCALES;
			this.deltaAngleScales = AIConstant.MARK_DEFAULT_SCALES;
		}
	}

	///得到某张牌和当前战术的适合度 1~10000,>预期值的最高值手牌则采用
	protected virtual double getCardFitness(MarkVO mark){
		double fitness = 0;
		//目的是击杀对手，或消磨对手时，对手分数越多，出牌优先级越高
		fitness += this.getSingleFitness(mark.life,this.life);
		fitness += this.getSingleFitness(mark.move,this.move);
		fitness += this.getSingleFitness(mark.damage,this.damage);
		fitness += this.getSingleFitness(mark.control,this.control);
		fitness += this.getSingleFitness(mark.push,this.push);

		double rangeRate = ArrayUtils.multiplyArray (this.deltaRangeScales,mark.rangeArr);
		double angleRate = ArrayUtils.multiplyArray (this.deltaAngleScales,mark.angleArr);

		//乘以方向和距离系数(重要性系数)
		if (fitness > 0) {
			fitness = fitness * rangeRate * angleRate;
		} 

		//有明确目标的情况，增加方向和距离适应度
		if (null != this.cardAimPosition) {
			fitness += (rangeRate * AIConstant.MARK_RANGE_ADD + angleRate * AIConstant.MARK_ANGLE_ADD)*this.cardPriority;
		}

		fitness += this.extraFitness(mark);
		return fitness;
	}
	///计算单项适合度
	public double getSingleFitness(double value,double aimValue){
		if (aimValue < 0) {
			//逆向需求，如拉近需要负值才满足
			return -value;
		}else if (value <= aimValue) {
			return value;
		} else {
			//需求超出，反而不适合
			return aimValue + (aimValue - value) * AIConstant.MARK_FITNESS_OVER_RATE;
		}
	}

	///额外适合度
	public double extraFitness(MarkVO mark){
		double fitness = 0;
		double rate;
		///如果本策略出卡优先级很高，提高适应度
		fitness += this.cardPriority * AIConstant.MARK_CARD_PRIORITY_ADD;

		///如果当前血量很低，卡牌的适合度提升
		rate = AIConstant.HP_PER_MIDDLE - this._aiPlayer.selfAssess.hpPer;
		if (rate > 0) {
			fitness += rate * AIConstant.MARK_FITNESS_HP_PER_DYING * (1+this._aiPlayer.IQ);
		}
		///随机额外值
		fitness += this.getRandom()* AIConstant.MARK_FITNESS_RANDOM * ((1-this._aiPlayer.IQ)*2 +1);

		//愤怒，卡牌适应度提升
		fitness += this._aiPlayer.moodController.anger * this._aiPlayer.IQ * AIConstant.MARK_ANGER;

		return fitness;
	}



	///设置移动目标
	public void setMoveAim(Vector2D v2d, double angle = double.MinValue, double speedScale = 1){
		speedScale = Math2.range (speedScale, 1, 0);
		this.moveAimPosition.copy (v2d);
		this.limitPosition (this.moveAimPosition);
		this.moveAimAngle = angle;
		this.moveAimSpeedScale = speedScale;
	}

	///设置移动目标为当前位置基础上的偏移坐标
	public void forwardMoveAim(Vector2D v2d, double angle = double.MinValue, double speedScale = 1){
		v2d.add (this.aiPlayer.player.position);
		this.setMoveAim (v2d, angle, speedScale);
	}

	///随机浮动移动目标，不超过半径
	public void floatMoveAim(double radius){
		radius = this.getRandom () * radius;
		double angle = this.getRandom () * Math2.PI2;
		Vector2D v2d = Vector2D.createVector2 (radius, angle);
		this.moveAimPosition.add (v2d);
		v2d.clear ();
	}

	///将当前地图的随机坐标 设置为移动目标点
	public void randomMoveAim () {	
		Vector2D v2d = Vector2D.createVector(this.getRandomRange(0,this._map.mapData.width),this.getRandomRange(this._map.mapData.height*0.1,this._map.mapData.height*0.9));
		this.setMoveAim (v2d);
		v2d.clear ();
	}
}


