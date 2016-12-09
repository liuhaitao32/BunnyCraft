using System;

///被AI评估玩家 的触发器评估
public class AssessTrigger:BaseAssessPlayer{
	private SkillManager _skillManager;

	public AssessTrigger(AssessPlayer assessPlayer):base(assessPlayer) 
	{
		this._skillManager = this._assessPlayer.targetPlayer.skillManager;
	}

}


