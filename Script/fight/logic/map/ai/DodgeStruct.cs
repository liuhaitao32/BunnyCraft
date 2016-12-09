using System;

///躲避/迎击结构
public class DodgeStruct
{
	//躲避位置
	public Vector2D dodegePosition;
	//躲避几率
	public double dodgePer;

	public DodgeStruct (Vector2D dodegePosition, double dodgePer) {
		this.dodegePosition = dodegePosition;
		this.dodgePer = dodgePer;
	}
}


