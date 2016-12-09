using System;

///AI评估玩家时每个局部引用的基类
public class BaseAssessPlayer :BaseAIPlayer {
	///被评估的玩家
	protected AssessPlayer _assessPlayer;
	public BaseAssessPlayer(AssessPlayer assessPlayer):base(assessPlayer.aiPlayer) 
	{
		this._assessPlayer = assessPlayer;
	}

	///重新评估前调用更新属性
//	public virtual void update()
//	{
//	}

	///传入需要评估的范畴和内容类型，返回评估结果
	public virtual double[] getTypeMark(int aspectType, int valueType) {
		return new double[AIConstant.MARK_COUNT];
	}
	///传入需要评估的内容类型，返回评估结果
	public virtual double getMark(int valueType) {
		return 0;
	}
}

