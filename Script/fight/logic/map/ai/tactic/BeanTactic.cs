using UnityEngine;
using System.Collections;

///发现物品的战术
public class BeanTactic : BaseTactic {
	public BeanTactic (AIPlayer aiPlayer,FightEntity target, double important):base(aiPlayer,target,important) 
	{
		this.type = AIConstant.BEAN_TACTICS_TYPE;
		this.discoverChance = 0.7 * this.important + 0.3;
		this.thinkPriority = 0.7 * this.important + 0.1;
	}


	//决策，生成该战术的细节，出牌位置和移动方案
	public override void thinkTactic(){
		BeanEntity bean = (BeanEntity)this.target;
		double priority = this.important;

		this.cardPriority = priority * 0.1;
		this.movePriority = priority * 0.8;


		this.life = 0;
		this.move = 2000 * priority + 500;
		this.damage = 0;
		this.control = 0;
		this.push = 0;

		this.setMoveAim(bean.position);
		this.floatMoveAim (50);

		this.cardAimPosition = this.moveAimPosition.clone();
//		this.cardAimPosition.copy(this.moveAimPosition);
	}
}
