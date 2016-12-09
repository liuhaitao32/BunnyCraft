using System;
using System.Collections.Generic;
using UnityEngine;

///AI相关配置
public class AIConstant
{
	public static Dictionary<string, object> ai;
	public static Dictionary<string, object> mark;
	public static void init() {
		ai           		= (Dictionary<string, object>)DataManager.inst.ai;
		tacticsConfig   	= (Dictionary<string, object>)ai["tactics"];
		markConfig 			= (Dictionary<string, object>)ai["mark"];
		moodConfig      	= (Dictionary<string, object>)ai["mood"];

		mark           		= (Dictionary<string, object>)DataManager.inst.mark;
		markCardConfig 		= (Dictionary<string, object>)mark["card"];
		markTriggerConfig 	= (Dictionary<string, object>)mark["trigger"];
		markBuffConfig 		= (Dictionary<string, object>)mark["buff"];

		initTactics();
		initMarkConfig();
		initMoodConfig ();

		MarkVO.init ();
	}

	//———————————————————————————————————————决策相关常量———————————————————————————————————————————————
	public const int MESS_TACTICS_TYPE = 500;
	public const int DYING_TACTICS_TYPE = 501;
	public const int PART_TACTICS_TYPE = 502;

	public const int BEAN_TACTICS_TYPE = 600;
	public const int BULLET_TACTICS_TYPE = 601;
	public const int PLAYER_TACTICS_TYPE = 602;
	public const int CALL_TACTICS_TYPE = 603;

	public const int RADISH_TACTICS_TYPE = 700;


	///AI查找目标CD
	public static int FIND_CD = 500;
	///AI情绪波动CD
	public static int MOOD_CD = 1000;
	///AI重定位战术CD
	public static int TACTICS_CD = 25000;
	///AI检测使用卡牌CD
	public static int CARD_CD = 800;
	///AI认为已经到达目标点的距离
	public static int ARRIVE_RADIUS = 100;
	///预测命中的额外半径，防止不小心躲开的子弹，但又回到命中范围
	public static int FORECAST_RADIUS = 200;
	///躲避子弹额外半径
	public static int DODGE_BULLET_RADIUS = 200;
	///躲避障碍额外半径
	public static int DODGE_BARRIER_RADIUS = 150;
	///AI进场时发呆时间标值
	public static int ENTER_STAND_TIME = 4000;
	///预判短暂未来的基础时间
	public static int FORECAST_TIME = 132;

	//暂未录入配置
	///默认插入的AI人数
	#if UNITY_EDITOR 
	public static int AI_PLAYER_NUM = 0;
	#else
	public static int AI_PLAYER_NUM = 7;
	#endif
	///IQ 默认值（临时）
	public static double IQ_DEFAULT = 0.5;
	///IQ 浮动范围
	public static double IQ_RANDOM_RANGE = 0.3;
	///半血比例
	public static double HP_PER_MIDDLE = 0.5;
	///接近满血比例
	public static double HP_PER_NEAR_MAX = 0.8;
	///乱来策略的重要程度
	public static double MESS_IMPORTANT = 0.1;
	///未装备的牌标记价值比例
	public static double PART_IN_CARD_MARK_RATE= 0.5;

	///评估间隔时间超过多少，认为不再相信手牌和能量评估
	//	public static int ASSESS_INTERVAL_TIME = 5000;
	//	///评估时认为不在手牌的死牌有几张
	//	public static int ASSESS_DEAD_CARD_NUM = 4;


	public static Dictionary<string, object> tacticsConfig = new Dictionary<string, object> {
		{"findCd", 500},
		{"moodCd", 1000},
		{"tacticsCd", 500},
		{"cardCd", 300},
		{"arriveRadius", 50},
		{"forecastRadius", 200},
		{"dodgeRadius", 200},
		{"barrierRadius", 300},
		{"enterStandTime", 4000},
		{"forecastTime", 132}
	};
	public static void initTactics() {
		FIND_CD 			= (int)(tacticsConfig["findCd"]);
		MOOD_CD         	= (int)(tacticsConfig["moodCd"]);
		TACTICS_CD       	= (int)(tacticsConfig["tacticsCd"]);
		CARD_CD				= (int)(tacticsConfig["cardCd"]);
		ARRIVE_RADIUS		= (int)(tacticsConfig["arriveRadius"]);
		FORECAST_RADIUS		= (int)(tacticsConfig["forecastRadius"]);
//		DODGE_BULLET_RADIUS	= (int)(tacticsConfig["dodgeRadius"]);
//		DODGE_BARRIER_RADIUS= (int)(tacticsConfig["barrierRadius"]);
		ENTER_STAND_TIME	= (int)(tacticsConfig["enterStandTime"]);
		FORECAST_TIME		= (int)(tacticsConfig["forecastTime"]);
	}

	//———————————————————————————————————————标记相关常量———————————————————————————————————————————————

	/// 判断前中后或者正侧背时，都是3个标记位置
	public static int MARK_COUNT = 3;
	///飞船生命的满价值，相对于攻击能力
	public static double MARK_HP_MAX = 4000;

	///飞船正前方的角度定义
	public static double MARK_ANGLE_FRONT = 0.26;
	///飞船前侧方的角度定义
	public static double MARK_ANGLE_FRONT_SIDE = 0.7;
	///飞船正前和前侧方的角度差值
	public static double MARK_ANGLE_FRONT_DELTA = MARK_ANGLE_FRONT_SIDE-MARK_ANGLE_FRONT;
	///飞船后侧方的角度定义
	public static double MARK_ANGLE_BEHIND_SIDE = 2.44;
	///飞船正后方的角度定义
	public static double MARK_ANGLE_BEHIND = 2.88;
	///飞船正后方和后侧方的角度差值
	public static double MARK_ANGLE_BEHIND_DELTA = MARK_ANGLE_BEHIND-MARK_ANGLE_BEHIND_SIDE;

	///标记浮动值
	public static double MARK_RANDOM = 0.1;

	///方向适应度对应的加值
	public static double MARK_ANGLE_ADD = 600;
	///距离适应度对应的加值
	public static double MARK_RANGE_ADD = 300;
	///出卡优先级对应的加值
	public static double MARK_CARD_PRIORITY_ADD = 1500;
	///安静该出装备时，对应的加值
	public static double MARK_PART_ADD = 3500;
	///疯狂出牌的随机加值
	public static double MARK_MESS_ADD = 500;

	///手牌适应度随机加值
	public static double MARK_FITNESS_RANDOM = 500;
	///能量快满时,适应度加值
	public static double MARK_FITNESS_POWER_NEAR_MAX = 3500;
	///手牌使用后能量快空时,适应度减值
	public static double MARK_FITNESS_POWER_NEAR_MIN = 1500;
	///濒死时,适应度加值
	public static double MARK_FITNESS_HP_PER_DYING = 3000;
	///可以出牌的价值
	public static double MARK_USE_CARD = 4000;
	///手牌适应度愤怒加值
	public static double MARK_ANGER = 500;
	///卡牌效果超出预期,适应度减值
	public static double MARK_FITNESS_OVER_RATE = -0.8;

	///无卡牌使用目标情况，各位置的混合系数(适用于近中远 和正侧背)
	public static double[] MARK_DEFAULT_SCALES = {0.4,0.3,0.3};

	///爆发式移动的价值下限
	public static double MARK_BURST_MOVE = 1000;
	///爆发式防御的价值下限
	public static double MARK_BURST_SHIELD = 1000;
	///头向攻击力低下的阙值
	public static double MARK_ATTACK_ANGLE_MIN = 4000;

	public static string DEFAULT_MARK_ID = "C999";
	///装备穿着后评判威胁能力的倍数
	public static double PART_MARK_SCALE = 2;
	///战意士气 > 0.3才算是可作战范围
	public static double MORALE_MIN = 0.2;
	public static double MORALE_NEAR_MIN = 0.35;
	public static double MORALE_MIDDLE = 0.5;
	public static double MORALE_NEAR_MAX = 0.65;

	///近，中，远，分别代表距离
	public static int RANGE_DISTANCE_MIN = 200;
	public static int RANGE_DISTANCE_VERY_NEAR = 500;
	public static int RANGE_DISTANCE_NEAR = 800;
	public static int RANGE_DISTANCE_MIDDLE = 1100;
	public static int RANGE_DISTANCE_FAR = 1400;
	public static int RANGE_DISTANCE_VERY_FAR = 1700;
	public static int RANGE_DISTANCE_MAX = 2000;
	public static int RANGE_DISTANCE_AFOCAL = 3000;


	public static Dictionary<string, object> markConfig;
	 
	public static void initMarkConfig() {

		//待整理
	}


	//———————————————————————————————————————情绪相关常量———————————————————————————————————————————————

	///情绪事件，被攻击
	public const string EVENT_BE_HIT = "EVENT_BE_HIT";
	///情绪事件，攻击
	public const string EVENT_HIT = "EVENT_HIT";

	public static int MOOD_NUM = 5;
	public static int MOOD_COURAGE = 0;
	public static int MOOD_ANGER = 1;
	public static int MOOD_NERVOUS = 2;
	public static int MOOD_ERROR = 3;
	public static int MOOD_LONELY = 4;

	///AI基础情绪，第一维度：基值，增加系数，降低系数。 第二维度：勇气，愤怒，紧张，失误，孤独
	public static Dictionary<string,double[][]> playerMood = new Dictionary<string,double[][]> { 
		{
			"mood000",
			new double[][]{
				new double[]{0.5,0,0.2,0.3,0.5},
				new double[]{0.5,0.5,0.5,0.5,0.5},
				new double[]{0.5,0.5,0.5,0.5,0.5},
			}
		},
	};

	///AI情绪平复系数
	public static double[] moodCalm = new double[]{0.1,0.1,0.1,0.1,0.1};
	///情绪配置
	public static Dictionary<string, object> moodConfig;

	public static void initMoodConfig() {

		moodCalm = Array.ConvertAll<object, double>((object[])moodConfig["calm"], o => Convert.ToDouble(o));
//		moodCalm = Array.ConvertAll<object, double>((object[])moodConfig["calm"], (object o)=> { return (double)o; });
	}






	//———————————————————————————————————————标记相关常量———————————————————————————————————————————————

	///卡牌标识配置
	public static Dictionary<string,object> markCardConfig;
	///触发器标识配置
	public static Dictionary<string,object> markTriggerConfig;
	///状态标识配置
	public static Dictionary<string,object> markBuffConfig;
}


