using UnityEngine;
using System.Collections;
using System;

/// 附近有敌人时的战术
public class PlayerTactic : BaseTactic {
	///放弃战术（追不到）
	public const string GIVE_UP = "GIVE_UP";

	//拉近战术系列
	///直追战术（迫切需要拉近距离）
	public const string IN_LINE = "IN_LINE";
	///侧向迂回前进战术（当前侧向对敌，侧向对敌有较大优势，不需要迫切拉近）
	public const string IN_SIDE = "IN_SIDE";

	//维持距离战术系列
	///基本不动，面朝敌人战术(无法有效远离时也采用该战术)
	public const string HOLD_HEAD = "HOLD_HEAD";
	///基本不动，侧朝敌人战术(几率极低)
	public const string HOLD_WING = "HOLD_WING";
	///基本不动，背朝敌人战术(几率极低)
	public const string HOLD_TAIL = "HOLD_TAIL";
	///侧向迂回战术（当前侧向对敌，侧向对敌有较大优势）
	public const string HOLD_SIDE = "HOLD_SIDE";

	//远离战术系列
	///逃生战术（迫切需要拉远距离）
	public const string OUT_LINE = "OUT_LINE";
	///侧向迂回后退战术（当前侧向对敌，侧向对敌有较大优势，不需要迫切拉远）
	public const string OUT_SIDE = "OUT_SIDE";

	public string subType;

	public PlayerTactic (AIPlayer aiPlayer,FightEntity target, double important):base(aiPlayer,target,important) 
	{
		this.type = AIConstant.PLAYER_TACTICS_TYPE;
		this.discoverChance = 0.2 * this.important + 0.8;
		this.thinkPriority = 1 * this.important;
	}
	///决策，生成该战术的细节，出牌位置和移动方案
	public override void thinkTactic(){
		PlayerEntity targetPlayer = (PlayerEntity)this.target;
		AssessPlayer targetAssessPlayer = this._aiPlayer.assessController.getAssessPlayer (targetPlayer.uid);

		PlayerEntity selfPlayer = this._aiPlayer.player;
		AssessPlayer selfAssessPlayer = this._aiPlayer.selfAssess;


		//评估好的理想作战距离
		SituationVO situationVO = this._aiPlayer.assessController.getSituationVO(targetAssessPlayer);
		double morale = situationVO.morale;
		int distanceMin = situationVO.distanceMin;
		int distanceMax = situationVO.distanceMax;

//		string log = string.Format ("{0:F2},[{1},{2}]", morale,distanceMin,distanceMax);
//		ownerPlayer.ShowAILog (log);
//		ownerPlayer.ShowAILog ("战意：" + morale + "，最佳作战距离[" + distanceMin + "," + distanceMax + "]");
//		Debug.Log ("战意："+morale+"，最佳作战距离[" + distanceMin+"," +distanceMax+"]");
		morale = this.randomValue (morale,AIConstant.MARK_RANDOM);



		//评估预判最佳作战距离，方向，尽可能达成
		Vector2D targetForecastPosition = targetAssessPlayer.getForecastPosition(AIConstant.FORECAST_TIME*(0.5+this._aiPlayer.IQ));
		Vector2D selfForecastPosition = selfAssessPlayer.getForecastPosition(AIConstant.FORECAST_TIME * this._aiPlayer.IQ);

		Vector2D deltaPosition = Collision.realPosition (selfForecastPosition,targetForecastPosition,this._map.mapData).deltaPos;
		double deltaDistance = deltaPosition.length;
		//对方朝向我的速度分量，威胁速度（正值为靠近，负值为远离）
		double warnSpeed = -targetPlayer.velocity.projectionOn (deltaPosition);
		//我和对方的最大最小相对速度（正值为靠近，负值为远离）
		double selfSpeedMax = selfAssessPlayer.currentSpeedMax;
		double deltaSpeedMax = warnSpeed + selfSpeedMax;
		double deltaSpeedMin = warnSpeed - selfSpeedMax;
		//手上可用卡牌有爆发移动力（非装备）
		bool canBurstMove = selfAssessPlayer.canBurstMove;

		//自身擅长的攻击角度
		double[] selfStrengthAngle = selfAssessPlayer.getMarkStrengthAngle();
		//敌方擅长的攻击角度
		double[] targetStrengthAngle = targetAssessPlayer.getMarkStrengthAngle();
		//自身擅长的攻击角度，最大值为1
		double[] selfStrengthAngleNormalize = ArrayUtils.normalize((double[])selfStrengthAngle.Clone());
		//敌方擅长的攻击角度，最大值为1
		double[] targetStrengthAngleNormalize = ArrayUtils.normalize((double[])targetStrengthAngle.Clone());

		//预处理各种需要分析的数据，我的当前角度相对于位置向量角度-180 , 180
		double selfDeltaAngle = Math2.deltaAngle(selfPlayer.angle,deltaPosition.angle);
		//敌方当前角度相对于位置向量角度 0面对我，90侧对我，180背对我
		double targetDeltaAngle = Math2.deltaAngle(targetPlayer.angle,deltaPosition.angle + Math.PI);
		double[] targetFitnessAngleMarks = MarkVO.getFitnessAngleMarks (targetDeltaAngle);

//		double absDeltaAngle = Mathf.Abs (deltaAngle);

		//追击倾向{靠近，保持，拉远}
		double[] sickMarks = new double[AIConstant.MARK_COUNT];
		//正向面对倾向{正面，侧面，背面}
		double[] faceMarks = selfStrengthAngleNormalize;
//		double[] faceMarks = ArrayUtils.SubtractArray(ownerStrengthAngleNormalize, ArrayUtils.MultiplyArray(aimStrengthAngleNormalize,0.2));
		//对手如果正背面能力超强，尽量使用迂回战术
		faceMarks[0] += -targetStrengthAngleNormalize[0]*0.4*targetFitnessAngleMarks[0] - targetStrengthAngleNormalize[2]*0.4*targetFitnessAngleMarks[2];
		faceMarks[1] += targetStrengthAngleNormalize[0]*0.3 + targetStrengthAngleNormalize[2]*0.1 - targetStrengthAngleNormalize[1]*0.5;

		double[] selfFitnessAngleMarks = MarkVO.getFitnessAngleMarks (selfDeltaAngle);
		ArrayUtils.multiplyArrayValue (selfFitnessAngleMarks, 0.1);
		ArrayUtils.addArray (faceMarks, selfFitnessAngleMarks);

		//击杀倾向，决定输出伤害是否最划算
		double attackMark = 1 - targetAssessPlayer.hpPer;
		faceMarks [0] += attackMark * 0.8;

		//速率倾向
		double speedMark = 1 + AIConstant.MARK_RANDOM;
		//放弃倾向
		double giveUpMark = 0;
		//左翼倾向
		double leftMark = MarkVO.getLeftMark (selfDeltaAngle);
		//生存倾向(越近越倾向护盾技能)
		double lifeMark = deltaDistance>=AIConstant.RANGE_DISTANCE_FAR ? 0: (double)(AIConstant.RANGE_DISTANCE_FAR-deltaDistance)/AIConstant.RANGE_DISTANCE_FAR;

		//面对倾向影响追击倾向
		sickMarks[0] += faceMarks[0]*0.5 + faceMarks[1]*0.1;
		sickMarks[1] += faceMarks[0]*0.1 + faceMarks[1]*0.4 + faceMarks[2]*0.1;
		sickMarks[2] += faceMarks[1]*0.1 + faceMarks[2]*0.5;

		//AI智商低下，不容易做出保持距离和侧面面对的决策
		sickMarks[1] -= (1-this._aiPlayer.IQ)*0.8;
		faceMarks[1] -= (1 - this._aiPlayer.IQ) * 2.5;

		//自身正面能力低下（很可能没有可用卡牌，尽可能放弃直接战斗或直接逃跑）
		if (selfStrengthAngle [0] < AIConstant.MARK_ATTACK_ANGLE_MIN) {
			double temp = (AIConstant.MARK_ATTACK_ANGLE_MIN - selfStrengthAngle[0]) / AIConstant.MARK_ATTACK_ANGLE_MIN;
			giveUpMark += temp*1.2;
			sickMarks[2] += temp*1.2;
		}

		//temp
//		sickMarks [0] += 5;
//		faceMarks [1] += 5;

		if (morale >= AIConstant.MORALE_NEAR_MIN) {
			//认真作战
			if (deltaDistance > distanceMax) {
				//应该靠近，对方正高速逃离，我的最大速度无法追击，如果存在目标距离内可秒杀或追击技能则使用，追击超出距离就放弃
				if (deltaSpeedMax < 0 && deltaDistance >= AIConstant.RANGE_DISTANCE_VERY_FAR) {
					//追不上，放弃
					giveUpMark += 0.5+(-deltaSpeedMax / selfSpeedMax)+(deltaDistance-AIConstant.RANGE_DISTANCE_VERY_FAR)/AIConstant.RANGE_DISTANCE_VERY_FAR;
				} 
				//距离远，追击动机足
				sickMarks[0] += 1;
			} else if (deltaDistance < distanceMin) {
				//应该拉远，对方正朝向我高速移动，我的最大速度无法拉开距离，放控制技能后再后撤
				if (deltaSpeedMin > 0) {
					//距离近，无法拉开距离，停下来打
					sickMarks[1] += 1;
					sickMarks[2] += 0.2;
				} else {
					//距离近，拉开距离来打
					sickMarks[1] += 0.2;
					sickMarks[2] += 1;
				}
			} else {
				//保持距离

				//理想距离中值偏移，大于0 应适当拉近，小于0 应适当拉远
				double deltaAimDistance = deltaDistance - (distanceMin + distanceMax) / 2;
				//理想速率，大于0 应适当拉近，小于0 应适当拉远
				double tempAimSpeedScale = deltaAimDistance / 5 / selfSpeedMax;
				if (tempAimSpeedScale < 0.5) {
					//距离相差不多，可选择侧翼包抄（我的侧方向可以攻击，而敌方侧方向或后方向（近战）比较薄弱）
					sickMarks[0] += 0.3;
					sickMarks[1] += 0.7;
				} else{
					//还是需要很快追逐
					sickMarks[0] += 0.7;
					sickMarks[1] += 0.3;
				} 
				speedMark = Math.Max(this.getRandom()*0.8+0.2,tempAimSpeedScale+0.2);
			}
		}  
		else if (morale >= AIConstant.MORALE_MIN) {
			//稍有信心战斗
			if (!canBurstMove && deltaSpeedMin > 0 && deltaDistance <= AIConstant.RANGE_DISTANCE_MIDDLE) {
				//逃不掉，死磕
				sickMarks[0] += 1;
				sickMarks[1] += 0.8;
				sickMarks[2] += 0.2;
				speedMark = this.getRandom()*0.8+ 0.2;
			} else {
				//不可力敌，逃跑
				sickMarks [2] += 1;
				faceMarks [2] += 0.6;
			}
		}
		else{
			//如果逃不掉，死磕
			if (!canBurstMove && deltaSpeedMin > 0 && deltaDistance <= AIConstant.RANGE_DISTANCE_MIDDLE) {
				//逃不掉，死磕
				sickMarks[0] += 1;
				sickMarks[1] += 0.6;
				speedMark = this.getRandom()*0.6+ 0.4;
			} else {
				//不可力敌，逃跑
				sickMarks [2] += 2;
				faceMarks [2] += 2;
			}
		}



		//适度的随机
		this.randomArrayValue (sickMarks,AIConstant.MARK_RANDOM);
		this.randomArrayValue (faceMarks,AIConstant.MARK_RANDOM);
		this.randomValue (attackMark, AIConstant.MARK_RANDOM);
		this.randomValue (speedMark, AIConstant.MARK_RANDOM);
		this.randomValue (giveUpMark, AIConstant.MARK_RANDOM);
		this.randomValue (leftMark, AIConstant.MARK_RANDOM);
		this.randomValue (lifeMark, AIConstant.MARK_RANDOM);

		//真正的策略

		if (giveUpMark >= 1) {
			this.subType = GIVE_UP;
		} else {
			int sickType = ArrayUtils.getMaxValueIndex (sickMarks);
			int faceType = ArrayUtils.getMaxValueIndex (faceMarks);
			if (sickType == 0) {
				//靠近
				if (faceType == 0) {
					this.subType = IN_LINE;
				}
				else{
					this.subType = IN_SIDE;
				}
			} else if (sickType == 1) {
				///保持
				if (faceType == 0) {
					this.subType = HOLD_HEAD;
				}
				else if (faceType == 1) {
					this.subType = HOLD_SIDE;
				}
				else{
					this.subType = HOLD_TAIL;
				}
			} else {
				//远离
				if (faceType == 1) {
					this.subType = OUT_SIDE;
				}
				else{
					this.subType = OUT_LINE;
				}
			}
		}


		this.cardPriority = 1 * this.important;
		this.movePriority = 1 * this.important;
		this.cardAimPosition = targetForecastPosition;
		if (morale > AIConstant.MORALE_NEAR_MAX) {
			//全力击杀
			this.cardPriority += 0.2;
		}

//		string log2 = string.Format ("{0} > {1}\n相对距离{2},头差角度{3:F1},左翼倾向{4:F2},战术{5}", ownerPlayer.name, aimPlayer.name, deltaDistance,deltaAngle,leftMark , type);
//		Debug.Log (log2);
//		Debug.Log (ownerPlayer.name + " > " + aimPlayer.name + 
//			"\n左翼倾向" + leftMark + "，相对角度" + deltaAngle+"，战术" + type);

		switch (this.subType) {
		case GIVE_UP:
			{
				this.cardPriority = 0;
				this.movePriority = 0;
				break;
			}
		
		case IN_LINE:
			{
				this.life = 500 + 500*lifeMark;
				this.move = 200 + 1000*speedMark;
				this.damage = 4000;
				this.control = 3000;
				this.push = -2000;

				Vector2D tempV2d = deltaPosition.clone ().multiply (0.5);
				this.forwardMoveAim (tempV2d, tempV2d.angle, speedMark+0.3);
				this.floatMoveAim (50);
				tempV2d.clear ();
				break;
			}
		case IN_SIDE:
			{
				this.life = 200 + 500*lifeMark;
				this.move = 500 + 500*speedMark;
				this.damage = 4000;
				this.control = 3000;
				this.push = 0;

				Vector2D tempV2d = deltaPosition.clone ().reverse();
				double offsetAngle = 2.1 / Math.Max (1, deltaDistance / AIConstant.RANGE_DISTANCE_VERY_NEAR);
				//左右翼包抄(距离越近选择角度越大) 左翼包抄是以目标的右侧作为目标点
				if (leftMark > 0) {
					tempV2d.angle -= offsetAngle;
				} else{
					tempV2d.angle += offsetAngle;
				}
				//缩短距离
				tempV2d.multiply(0.7);
				tempV2d.add (targetPlayer.position);
				this.setMoveAim (tempV2d, tempV2d.angle);
				this.floatMoveAim (50);
				tempV2d.clear ();
				break;
			}
		case HOLD_HEAD:
			{
				this.life = 500 + 500*lifeMark;
				this.move = 0;
				this.damage = 3000+1000*attackMark;
				this.control = 3000+1000*attackMark;
				this.push = 0;

				Vector2D tempV2d = deltaPosition.clone ().multiply (0.3);
				this.forwardMoveAim (tempV2d, tempV2d.angle,speedMark);
				this.floatMoveAim (20);
				tempV2d.clear ();
				break;
			}
		case HOLD_WING:
			{
				break;
			}
		case HOLD_TAIL:
			{
				this.life = 500 + 500*lifeMark;
				this.move = 0;
				this.damage = 1000+3000*attackMark;
				this.control = 3000+1000*attackMark;
				this.push = 0;

				Vector2D tempV2d = deltaPosition.clone ().reverse();
				this.forwardMoveAim (tempV2d, tempV2d.angle, speedMark);
				this.floatMoveAim (100);
				tempV2d.clear ();
				break;
			}
		case HOLD_SIDE:
			{
				this.life = 200 + 500*lifeMark;
				this.move = 500 + 500*speedMark;
				this.damage = 3000+1000*attackMark;
				this.control = 3000+1000*attackMark;
				this.push = 0;

				Vector2D tempV2d = deltaPosition.clone ().reverse();
				double offsetAngle = 2.1 / Math.Max (1, deltaDistance / AIConstant.RANGE_DISTANCE_VERY_NEAR);
				//左右翼包抄(距离越近选择角度越大)
				if (leftMark > 0) {
					tempV2d.angle -= offsetAngle;
				} else{
					tempV2d.angle += offsetAngle;
				}
				//加大距离
				tempV2d.multiply(1.2);
				tempV2d.add (targetPlayer.position);
				this.setMoveAim (tempV2d, tempV2d.angle, (speedMark+1)*0.5);
				this.floatMoveAim (100);
				tempV2d.clear ();
				break;
			}
		case OUT_LINE:
			{
				this.life = 1000+ 500*lifeMark;
				this.move = 3000-3000*attackMark + 1000*speedMark;
				this.damage = 500+4000*attackMark;
				this.control = 3000;
				this.push = 2000;

				Vector2D tempV2d = deltaPosition.clone ().reverse();
				this.forwardMoveAim (tempV2d, tempV2d.angle, (speedMark+1)*0.5);
				this.floatMoveAim (100);
				tempV2d.clear ();
				break;
			}
		case OUT_SIDE:
			{
				this.life = 1000+ 500*lifeMark;
				this.move = 1000-3000*attackMark+ 1000*speedMark;
				this.damage = 500+4000*attackMark;
				this.control = 3000;
				this.push = 0;

				Vector2D tempV2d = deltaPosition.clone ().reverse();
				double offsetAngle = 2.1 / Math.Max (1, deltaDistance / AIConstant.RANGE_DISTANCE_VERY_NEAR);
				//左右翼撤退(距离越近选择角度越大)
				if (leftMark > 0) {
					tempV2d.angle -= offsetAngle;
				} else{
					tempV2d.angle += offsetAngle;
				}
				//加大距离
				tempV2d.multiply(2);
				tempV2d.add (targetPlayer.position);
				this.setMoveAim (tempV2d, tempV2d.angle, (speedMark+1)*0.5);
				this.floatMoveAim (100);
				tempV2d.clear ();
				break;
			}
		}
	}
}
