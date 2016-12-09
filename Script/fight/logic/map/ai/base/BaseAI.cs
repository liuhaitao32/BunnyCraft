using System;

///AI基类，必须要有地图，所有AI工具方法在此计算
public class BaseAI
{
	///地图数据
	protected Map _map;
	public Map map{get{return this._map;}}

	public BaseAI(Map map){
		this._map = map;
	}
		
	///数组每个元素随机上下浮动值
	public double[] randomArrayValue(double[] arr, double radius){
		int i;
		int len = arr.Length;
		for (i = 0; i < len; i++) {
			double rnd = getRandom();
			arr [i] += (rnd-0.5)*radius*2;
		}
		return arr;
	}
	///得到一个值在上下范围随机
	public double randomValue(double value, double radius){
		double rnd = getRandom();
		return value + (rnd-0.5)*radius*2;
	}

	///得到0~1随机数
	public double getRandom(){
		return this._map.random.getRandomAI();
	}
	///得到范围随机数
	public double getRandomRange(double min, double max){
		return min + this.getRandom() * ( max - min );
	}


	///返回位置+向量后的新坐标，不修改原坐标
//	public Vector2D getForwardPosition(Vector2D pos,Vector2D delta) {
//		return pos.clone ().add (delta);
//	}
	///直接限制位置不能超出地图
	public Vector2D limitPosition(Vector2D pos) {
		while(pos.x < 0) pos.x += this._map.mapData.width;
		pos.x = pos.x % this._map.mapData.width;
		pos.y = Math2.range(pos.y, this._map.mapData.height, 0);
		return pos;
	}
}


