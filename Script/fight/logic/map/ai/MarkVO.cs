using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

///卡牌标记 数据结构
public class MarkVO
{
	public static MarkVO defaultMarkCard;
	private static Dictionary<string,MarkVO> _markCardDic;
	private static Dictionary<string,MarkVO> _markTriggerDic;
	private static Dictionary<string,MarkVO> _markBuffDic;

	///评估范畴类型  不同射程[0近程 1中程 2远程]
	public static int ASPECT_TYPE_RANGE = 0;
	///评估范畴类型  不同方向[0正对目标 1侧对目标 2背对目标]
	public static int ASPECT_TYPE_ANGLE = 1;


	///评估内容类型  生存向能力评估
	public static int VALUE_TYPE_LIFE = 0;
	///评估内容类型  移动能力评估
	public static int VALUE_TYPE_MOVE = 1;
	///评估内容类型  伤害强度评估
	public static int VALUE_TYPE_DAMAGE = 2;
	///评估内容类型  控制强度评估
	public static int VALUE_TYPE_CONTROL = 3;
	///评估内容类型  推远强度评估
	public static int VALUE_TYPE_PUSH = 4;
	///评估内容类型  多目标评估
	public static int VALUE_TYPE_AIM = 5;
	///评估内容类型  生效速度评估
	public static int VALUE_TYPE_FAST = 6;

	///评估内容类型  攻击力（伤害+控制）评估
	public static int VALUE_TYPE_ATTACK = 100;


	public string id;

	///施展距离适应性 0近程 1中程 2远程
	public double[] rangeArr;
	///施展角度适应性 0正对目标 1侧对目标 2背对目标
	public double[] angleArr;
	///各项适应性指数 0生存 1位移 2伤害 3控制 4推远 5多目标 6生效速度
	public double[] markArr;

	public double life{get{return this.markArr[VALUE_TYPE_LIFE];}}
	public double move{get{return this.markArr[VALUE_TYPE_MOVE];}}
	public double damage{get{return this.markArr[VALUE_TYPE_DAMAGE];}}
	public double control{get{return this.markArr[VALUE_TYPE_CONTROL];}}
	public double push{get{return this.markArr[VALUE_TYPE_PUSH];}}
	public double aim{get{return this.markArr[VALUE_TYPE_AIM];}}
	public double fast{get{return this.markArr[VALUE_TYPE_FAST];}}

	///是否是装备
	public bool isPart;

	public MarkVO (string id)
	{
		Dictionary<string,object> data;
		this.id = AIConstant.markCardConfig.ContainsKey (id)? id : AIConstant.DEFAULT_MARK_ID;

		data = (Dictionary<string,object>)AIConstant.markCardConfig [id];

		this.rangeArr = Array.ConvertAll<object, double>((object[])data["range"], o => Convert.ToDouble(o));
		this.angleArr = Array.ConvertAll<object, double>((object[])data["angle"], o => Convert.ToDouble(o));
		this.markArr = Array.ConvertAll<object, double>((object[])data["mark"], o => Convert.ToDouble(o));

		this.isPart = MarkVO.cardIsPart(this.id);
	}


	public static bool cardIsPart(string cardId){
		if (cardId.Length == 4 && cardId.Substring (0, 1) == "C") {
			int type = int.Parse (cardId.Substring (1, 1));
			switch (type) {
			case 5:
			case 6:
			case 7:
				return true;
			default:
				break;
			}
		}
		return false;
	}

	///传入需要评估的范畴和内容，返回评估结果
	public double[] getTypeMark(int aspectType, int valueType) {
		double[] typeArr = aspectType == ASPECT_TYPE_RANGE ? this.rangeArr : this.angleArr;
		double[] mark = new double[AIConstant.MARK_COUNT];
		for (int i = 0; i < AIConstant.MARK_COUNT; i++) {
			mark [i] = typeArr[i] * (valueType == VALUE_TYPE_ATTACK ? (this.markArr [VALUE_TYPE_DAMAGE] + this.markArr [VALUE_TYPE_CONTROL]) :this.markArr [valueType] );
		}
		return mark;
	}

	///传入需要评估的内容，返回评估结果
	public double getMark(int valueType) {
		return this.markArr[valueType];
	}





	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~以下为静态方法

	public static void init () {
		MarkVO._markCardDic = new Dictionary<string, MarkVO> ();
		MarkVO._markTriggerDic = new Dictionary<string, MarkVO> ();
		MarkVO._markBuffDic = new Dictionary<string, MarkVO> ();
		MarkVO mark;

		foreach (KeyValuePair<string, object> kv in AIConstant.markCardConfig) {
			string id = kv.Key;
			mark = new MarkVO (id);
			MarkVO._markCardDic [id] = mark;
		}
		MarkVO.defaultMarkCard = MarkVO._markCardDic[AIConstant.DEFAULT_MARK_ID];

		foreach (KeyValuePair<string, object> kv in AIConstant.markTriggerConfig) {
			string id = kv.Key;
			mark = new MarkVO (id);
			MarkVO._markTriggerDic [id] = mark;
		}
		foreach (KeyValuePair<string, object> kv in AIConstant.markBuffConfig) {
			string id = kv.Key;
			mark = new MarkVO (id);
			MarkVO._markBuffDic [id] = mark;
		}
	}
	public static MarkVO getTriggerMark (string id) {
		//优先选择触发器的标记
		MarkVO mark;
		if (MarkVO._markTriggerDic.ContainsKey (id)) {
			mark = MarkVO._markTriggerDic [id];
		} else {
			mark = MarkVO.getCardMark (id);
		}
		return mark;
	}
	public static MarkVO getCardMark (string id) {
		MarkVO mark;
		if (MarkVO._markCardDic.ContainsKey (id)) {
			mark = MarkVO._markCardDic [id];
		} else {
			Debug.Log ("调用了不存在的Mark:"+id);
			return MarkVO.defaultMarkCard;
		}
		return mark;
	}
	public static MarkVO getMarkByPartId (string id) {
		id = partIdToCardId (id);
		return getCardMark(id);
	}

	///从装备id得到卡牌id
	public static string partIdToCardId(string partId){
		return "C" + partId.Substring (4);
	}

	///传入距离,返回对应距离的适应度数组
	public static double[] getFitnessRangeMarks(double distance) {
		double[] marks = new double[AIConstant.MARK_COUNT];
		double temp;
		if (distance > AIConstant.RANGE_DISTANCE_AFOCAL) {
			marks [0] = 0f;
			marks [1] = 0f;
			marks [2] = 0f;
		} else if (distance > AIConstant.RANGE_DISTANCE_MAX) {
			temp = (double)(distance - AIConstant.RANGE_DISTANCE_MAX) / (AIConstant.RANGE_DISTANCE_AFOCAL - AIConstant.RANGE_DISTANCE_MAX);
			marks [0] = 0f;
			marks [1] = 0f;
			marks [2] = temp;
		} else if (distance > AIConstant.RANGE_DISTANCE_VERY_FAR) {
			marks [0] = 0f;
			marks [1] = 0f;
			marks [2] = 1f;
		} else if (distance > AIConstant.RANGE_DISTANCE_FAR) {
			temp = (double)(distance - AIConstant.RANGE_DISTANCE_FAR) / (AIConstant.RANGE_DISTANCE_VERY_FAR - AIConstant.RANGE_DISTANCE_FAR);
			marks [0] = 0f;
			marks [1] = temp;
			marks [2] = 1f - temp;
		} else if (distance > AIConstant.RANGE_DISTANCE_MIDDLE) {
			marks [0] = 0f;
			marks [1] = 1f;
			marks [2] = 0f;
		} else if (distance > AIConstant.RANGE_DISTANCE_NEAR) {
			temp = (double)(distance - AIConstant.RANGE_DISTANCE_NEAR) / (AIConstant.RANGE_DISTANCE_NEAR - AIConstant.RANGE_DISTANCE_MIDDLE);
			marks [0] = temp;
			marks [1] = 1f - temp;
			marks [2] = 0f;
		} else {
			marks [0] = 1f;
			marks [1] = 0f;
			marks [2] = 0f;
		}
		return marks;
	}
	///传入角度(-Math.PI,Math.PI] ,返回对应朝向的适应度数组
	public static double[] getFitnessAngleMarks(double angle) {
		double[] marks = new double[AIConstant.MARK_COUNT];
		double absAngle = Math.Abs (angle);
		double temp;
		if (absAngle <= AIConstant.MARK_ANGLE_FRONT) {
			marks [0] = 1;
			marks [1] = 0;
			marks [2] = 0;
		} else if (absAngle <= AIConstant.MARK_ANGLE_FRONT_SIDE) {
			temp = (absAngle - AIConstant.MARK_ANGLE_FRONT) / AIConstant.MARK_ANGLE_FRONT_DELTA;
			marks [0] = 1 - temp;
			marks [1] = temp;
			marks [2] = 0;
		} else if (absAngle <= AIConstant.MARK_ANGLE_BEHIND_SIDE) {
			marks [0] = 0;
			marks [1] = 1;
			marks [2] = 0;
		} else if (absAngle <= AIConstant.MARK_ANGLE_BEHIND) {
			temp = (absAngle - AIConstant.MARK_ANGLE_BEHIND_SIDE) / AIConstant.MARK_ANGLE_BEHIND_DELTA;
			marks [0] = 0;
			marks [1] = 1 - temp;
			marks [2] = temp;
		} else {
			marks [0] = 0;
			marks [1] = 0;
			marks [2] = 1;
		}
		return marks;
	}
	///传入角度(-Math.PI,Math.PI] ,返回对应左倾系数。返回   >0为向左转
	public static double getLeftMark(double angle) {
		return -Math.Sin (angle) * 0.1;
//		double temp = 1- Math.Abs (Math2.PIHalf - Math.Abs(angle)) / Math2.PIHalf;
//		return angle > 0 ? temp : -temp;
	}
}


