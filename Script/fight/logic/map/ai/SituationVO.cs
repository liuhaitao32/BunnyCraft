using System;

///玩家对玩家的估算，战意，最大最小作战距离结构
public class SituationVO
{
	///战意值(表示击杀对手难度比值)
	public double morale;	
	///最佳战斗距离（最远）
	public int distanceMax;
	///最佳战斗距离（最近）
	public int distanceMin;


	public SituationVO(double morale, int distanceMax, int distanceMin) {
		this.morale = morale;
		this.distanceMax = distanceMax;
		this.distanceMin = distanceMin;
	}
}


