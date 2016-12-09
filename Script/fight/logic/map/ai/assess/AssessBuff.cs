using System;
using System.Collections;
using System.Collections.Generic;

///被AI评估玩家 的Buff评估
public class AssessBuff:BaseAssessPlayer{
	private List<Buff> _buffs;

	public AssessBuff(AssessPlayer assessPlayer):base(assessPlayer) 
	{
		this._buffs = this._assessPlayer.targetPlayer.buffs;
	}

}

