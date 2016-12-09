using UnityEngine;
using System.Collections;

///情绪模型控制器
public class MoodController:BaseAIPlayer{
	///情绪模型类型
	string id;

	///当前情绪[勇气，愤怒，紧张，失误，孤独]   0~1
	double[] _currentArr;

	///情绪基值
	double[] _baseArr;

	///情绪增加系数，越大越容易0~1
	double[] _upArr;

	///情绪降低系数，越大越容易0~1
	double[] _downArr;

	///检测状态和平复情绪的CD
	public double moodCD;

	public MoodController(AIPlayer aiPlayer,string id):base(aiPlayer) 
	{
		this.id = id;
		this.init ();
	}
	public void init(){
		double[][] moodArr = (double[][])AIConstant.playerMood [this.id];

		this._baseArr = (double[])moodArr[0].Clone ();
		this._upArr = (double[])moodArr[1].Clone ();
		this._downArr = (double[])moodArr[2].Clone ();
		this._currentArr = (double[])this._baseArr.Clone ();
	}
	public void reset(){
		this.calmMood (0.5);
	}

	public void update(){
		this.moodCD -= ConfigConstant.MAP_ACT_TIME_S;
		if (this.moodCD <= 0)
			this.calmMood ();
	}

		
	///获得当前的某种情绪
	public double getMood(int index){return this._currentArr[index];}
	///强制改变某种情绪到固定值
	public double setMood(int index,double value){
		this._currentArr [index] = value;
		return this._currentArr[index];
	}


	///提升某种情绪
	///type 情绪类型
	///value 理想加值
	///minValue 最小值
	///maxValue 最大值，越接近该值增量越小，本身超出该值不增加也不减少
	public double upMood(int index,double value,double minValue=0,double maxValue=1){
		double temp = this._currentArr [index];
		if (temp < minValue)
			temp = minValue;
		if (temp < maxValue) {
			//越接近该值增量越小
			temp += (maxValue-temp)/(maxValue-minValue) * value * this._upArr [index];
		}
		this._currentArr [index] = temp;
		return temp;
	}
	///降低某种情绪
	///type 情绪类型
	///value 理想减值，填正值
	///minValue 最小值，越接近该值减量越小，本身低于该值不增加也不减少
	///maxValue 最大值
	public double downMood(int index,double value,double minValue=0,double maxValue=1){
		double temp = this._currentArr [index];
		if (temp > maxValue)
			temp = maxValue;
		if (temp > minValue) {
			//越接近该值减量越小
			temp -= (temp-minValue)/(maxValue-minValue) * value * this._downArr [index];
		}
		this._currentArr [index] = temp;
		return temp;
	}



	///平复所有情绪
	public void calmMood(double rate = 1){
		int i;
		double value;
		this.moodCD = AIConstant.MOOD_CD;

		for(i=0;i<AIConstant.MOOD_NUM;i++){
			value = this._baseArr [i]- this._currentArr [i];
			if (value>0) 
				this._currentArr [i] += value* this._upArr [i]* AIConstant.moodCalm[i] * rate;
			else if (value<0) 
				this._currentArr [i] += value* this._downArr [i]* AIConstant.moodCalm[i] * rate;
		}
	}
	///勇气
	public double courage {get{return this._currentArr[AIConstant.MOOD_COURAGE];}}
	///愤怒
	public double anger {get{return this._currentArr[AIConstant.MOOD_ANGER];}}
	///紧张
	public double nervous {get{return this._currentArr[AIConstant.MOOD_NERVOUS];}}
	///错误
	public double error {get{return this._currentArr[AIConstant.MOOD_ERROR];}}
	///孤独（团战专用）
	public double lonely {get{return this._currentArr[AIConstant.MOOD_LONELY];}}

	///触发情绪事件
	public void EventMood (string type, double value = 0) {
		switch (type) {
		case AIConstant.EVENT_BE_HIT:
			{
				//被命中
				this.upMood (AIConstant.MOOD_ANGER, 1*value, 0.3, 1);
				this.upMood (AIConstant.MOOD_NERVOUS, 0.5*value, 0.3, 1);
				this.downMood (AIConstant.MOOD_COURAGE, 0.5*value, 0.1, 0.9);
				break;
			}
		case AIConstant.EVENT_HIT:
			{
				//命中别人
				this.upMood (AIConstant.MOOD_COURAGE, 0.2*value, 0, 0.5);
				break;
			}
		case "beHit1":
			{
				this.upMood (AIConstant.MOOD_ANGER, 0.1, 0.1, 0.7);
				this.downMood (AIConstant.MOOD_COURAGE, 0.1, 0.1, 0.9);
				break;
			}
		}
	}


	public override string ToString ()
	{
		return string.Format ("{0:F2},{1:F2}", courage,nervous);
		//		return string.Format ("{0:F2},{1},{2},{3},{4}]", courage, anger, nervous, error, lonely);
	}
}
