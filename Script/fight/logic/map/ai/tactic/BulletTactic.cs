using UnityEngine;
using System.Collections;

///附近有敌对子弹时的战术
public class BulletTactic : BaseTactic {
	public BulletTactic (AIPlayer aiPlayer,FightEntity target, double important):base(aiPlayer,target,important) 
	{
		this.type = AIConstant.BULLET_TACTICS_TYPE;
		this.discoverChance = 0.7 * this.important + 0.5;
		this.thinkPriority = 0.95 * this.important;
	}
	//决策，生成该战术的细节，出牌位置和移动方案
	public override void thinkTactic(){
//		Bullet bullet = (Bullet)this.aimFightObject;
//		DodgeStruct dodgeStruct = bullet.ForecastDodgePosition (this.ownerPlayer);
//		if (null != dodgeStruct) {
//			//需要应对，变为出牌优先级提高
//			if (dodgeStruct.dodgePer > 0.6f) {
//				//可以移动躲避
////				ownerPlayer.ShowAILog (important+"躲避"+dodgeStruct.dodgePer);
//				this.cardPriority = 0;
//				this.movePriority = 1.6f * this.important;
//
//				this.moveAimPosition = dodgeStruct.dodegePosition;
//				this.moveAimSpeedScale = 1;
//				this.moveAimAngle = double.MinValue;
////				moveAimAngle = LogicUtils.getLogicDelta (ownerPlayer.logicPosition, moveAimPosition).angle;
//			}
//			else if (ownerPlayer.CanBurstMove() && dodgeStruct.dodgePer + this.getRandom() > 1f) {
//				//尝试位移卡躲避
//				this.cardPriority = 0.8f * this.important;
//				this.movePriority = 1.6f * this.important;
//
//				this.moveAimPosition = dodgeStruct.dodegePosition;
//				this.moveAimSpeedScale = 1;
//				this.moveAimAngle = double.MinValue;//LogicUtils.getLogicDelta (ownerPlayer.logicPosition, moveAimPosition).angle;
//
//				this.cardAimPosition = moveAimPosition.clone();
//				this.life = 500;
//				this.move = 3500;
//				this.damage = 0;
//				this.control = 0;
//				this.buff = 0;
//			}
//			else if (ownerPlayer.CanBurstShield()){
//				//护盾技抵挡
//				this.cardPriority = 0.8f * this.important;
//				this.movePriority = 0f;
//
//				this.cardAimPosition = bullet.logicPosition.clone();
//				this.life = 4500;
//				this.move = 0;
//				this.damage = 0;
//				this.control = 0;
//				this.buff = 0;
//			}
//			else {
//				//无法应对无视
//				this.cardPriority = 0;
//				this.movePriority = 0;
//			}
//		} else {
//			//无关子弹，可以无视
//			this.cardPriority = 0;
//			this.movePriority = 0;
//		}
	}
}
