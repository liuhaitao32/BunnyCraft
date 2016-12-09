using UnityEngine;
using System.Collections;

///贫血濒死时的战术
public class DyingTactic : BaseTactic {

	public DyingTactic(AIPlayer aiPlayer,FightEntity target, double important):base(aiPlayer,target,important) 
	{
		this.type = AIConstant.DYING_TACTICS_TYPE;
		this.discoverChance = 0.3 * this.important + 0.8;
		this.thinkPriority = 0.9 * this.important + 0.1;
	}
	//决策，生成该战术的细节，出牌位置和移动方案
	public override void thinkTactic(){
		this.cardPriority = this.thinkPriority;
		this.movePriority = 0;

		this.life = 4000 * this.important+2000;
		this.move = 0;
		this.damage = 0;
		this.control = 0;
		this.push = 0;
		//如果附近有血包，追踪之


	}
}
