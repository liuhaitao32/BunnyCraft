using UnityEngine;
using System.Collections;

///胡来战术，乱走乱出牌
public class MessTactic : BaseTactic {
	public MessTactic (AIPlayer aiPlayer,FightEntity target, double important):base(aiPlayer,target,important) 
	{
		this.type = AIConstant.MESS_TACTICS_TYPE;
		this.discoverChance = 1;
		this.thinkPriority = this.important;
	}
	//决策，生成该战术的细节，出牌位置和移动方案
	public override void thinkTactic(){
		this.cardPriority = this.thinkPriority;
		this.movePriority = this.thinkPriority;
		double rnd = this.getRandom();
		if (rnd < 1) {
			//全场随机
			this.randomMoveAim ();
		}
		else
		{
			Vector2D v2d = this._aiPlayer.player.position.clone ();
			double radius;
			double angle;
			if (rnd < 0.5) {
				//纯新手，继续前进（非满速）
				radius = this.getRandomRange (500, 1500);
				angle = this._aiPlayer.player.angle + this.getRandomRange (-0.2, 0.2);
				v2d.add (Vector2D.createVector2 (radius, angle));
				this.setMoveAim (v2d, double.MinValue, 0.6 + this.getRandom () * 0.4);
			}
			else if (rnd < 0.7) {
				//纯新手，发呆乱摇停下
				radius = this.getRandomRange (300,500);
				angle = this._aiPlayer.player.angle + this.getRandomRange (-0.4,0.4);
				v2d.add (Vector2D.createVector2 (radius, angle));
				this.setMoveAim (v2d,double.MinValue, 0.3+ this.getRandom()*0.5);
			}
			else{
				//在前方扇形随机目标
				radius = this.getRandomRange (500,1500);
				angle = this._aiPlayer.player.angle + this.getRandomRange (-0.6,0.6);
				v2d.add (Vector2D.createVector2 (radius, angle));
				this.setMoveAim (v2d,double.MinValue);
			} 
			v2d.clear();
		}
	}

	//得到某张牌和当前战术的适合度 1~10000,>预期值的最高值手牌则采用
	protected override double getCardFitness(MarkVO mark){
		double fitness = extraFitness(mark);
		return fitness;
	}
}
