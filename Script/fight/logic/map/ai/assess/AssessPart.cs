using System;

///被AI评估玩家 的装备评估
public class AssessPart:BaseAssessPlayer{
	private PartGroupAction _partGroup;

	public AssessPart(AssessPlayer assessPlayer):base(assessPlayer) 
	{
		this._partGroup = this._assessPlayer.targetPlayer.partGroup;
	}
		

	///传入需要评估的范畴和内容类型，返回评估结果
	public override double[] getTypeMark(int aspectType, int valueType) {
		PartAction part;
		double[] marks = new double[AIConstant.MARK_COUNT];
		for (int i = 0; i < ConfigConstant.PART_COUNT; i++) {
			part = this._partGroup.getPart (i, true);
			if (part != null) {
				if (valueType == MarkVO.VALUE_TYPE_LIFE) {
					marks [i] = this.getPartMark (part, valueType);
				} else {
					ArrayUtils.addArray (marks, this.getPartTypeMark (part, aspectType, valueType));
				}
			}
		}
		return marks;
	}
	///传入需要评估的内容类型，返回评估结果
	public override double getMark(int valueType) {
		PartAction part;
		double markValue = 0;
		for (int i = 0; i < ConfigConstant.PART_COUNT; i++) {
			part = this._partGroup.getPart (i, true);
			if (part != null) {
				markValue += this.getPartMark (part, valueType);
			}
		}
		return markValue;
	}

	//————————————————————————————————————单个装备评估——————————————————————————————————————————————————

	///获得装备当前状态下的预装完成率
	private double getPartCompletePer(PartAction part){
		if (part.alived)
			return 1;
		else if (null == part.preTimeAction)
			return 0;
		else {
			double time = part.preTimeAction.time;
			double totalTime = part.preTimeAction.totalTime;
			return (totalTime - time) / totalTime;
		}
	}
	///获得装备当前状态下的血量存量
	private double getPartHpPer(PartAction part){
		return (double)part.hp/part.hpMax;
	}
	//获得装备当前状态下的总体状态比率
	private double getPartScale(PartAction part){
		return part.alived ? Math.Pow (this.getPartHpPer (part), 0.1) : this.getPartCompletePer (part) * 0.7;
	}

	//获得装备当前状态下的攻击力比率
	private double getPartAttackPer(PartAction part){
		if (part.alived)
			return Math.Pow (getPartHpPer(part), 0.05);
		else
			return this.getPartCompletePer (part) * 0.7;
	}

	///传入需要评估的范畴和内容，返回评估结果
	private double[] getPartTypeMark(PartAction part,int aspectType, int valueType) {
		MarkVO mark = MarkVO.getMarkByPartId (part.id);
		return ArrayUtils.multiplyArrayValue(mark.getTypeMark(aspectType,valueType) , this.getPartScale(part));
	}

	///传入需要评估的内容，返回评估结果
	private double getPartMark(PartAction part,int valueType) {
		MarkVO mark = MarkVO.getMarkByPartId (part.id);
		return mark.getMark(valueType) * this.getPartScale(part);
	}
}


