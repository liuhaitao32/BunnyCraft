using UnityEngine;
using System.Collections;

///穿着装备战术
public class PartTactic : BaseTactic {

	public PartTactic (AIPlayer aiPlayer,FightEntity target, double important):base(aiPlayer,target,important) 
	{
		this.type = AIConstant.PART_TACTICS_TYPE;
		this.discoverChance = 0.8 * this.important;
		this.thinkPriority = 1;
	}
	//决策，生成该战术的细节，出牌位置和移动方案
	public override void thinkTactic(){
		this.cardPriority = this.thinkPriority;
		this.movePriority = 0;

		this.cardAimPosition = null;
//		this.life = 5000;
//		this.move = 5000;
//		this.damage = 5000;
//		this.control = 5000;
//		this.push = 5000;
	}

	//得到某张牌和当前战术的适合度 1~10000,>预期值的最高值手牌则采用
	protected override double getCardFitness(MarkVO mark){
		double fitness = 0;
		if(mark.isPart)
		{
			fitness = extraFitness(mark);
			fitness += AIConstant.MARK_PART_ADD * this.important;
		}
		return fitness;
	}
}
