using System;
using UnityEngine;

public class Math2 {
	

	private static readonly double rToA = (1f / Math.PI * 180f);		
	private static readonly double aToR = (1f /  180f) * Math.PI;

	//Biggo添加
    public static readonly double PIHalf = Math.PI * 0.5;
	public static readonly double PI2 = Math.PI * 2;

	/**
	 * 弧度转化角度。
	 * @param	radian 弧度
	 * @return 角度
	 */
	public static double radianToAngle(double radian) { return radian * rToA; }
	/**
	 * 角度转弧度。
	 * @param	angle 角度
	 * @return 弧度。
	 */
	public static double angleToRadian(double angle) { return angle * aToR; }

	/**
	 * 获取范围内的数字 如果超出 取临界值。
	 * @param	num 
	 * @param	max
	 * @param	min
	 * @return
	 */
	public static double range(double num, double max, double min) {
		return Math.Max(min, Math.Min(max, num));
	}
	/// 返回夹角(-Math.PI,Math.PI]
    public static double deltaAngle(double angle1, double angle2) {
        angle1 = angle2 - angle1;
		//Biggo修改 ，返回(-Math.PI,Math.PI]
		while(angle1 <= -Math.PI) angle1 += PI2;
        while(angle1 > Math.PI) angle1 -= PI2;
        return angle1;
    }
}
