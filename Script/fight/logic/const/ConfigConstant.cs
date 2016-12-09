using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigConstant {

    //——————————————————————————————————回复单位———————————————————————————————————————————
    //初始的时候power值。
    public static int POWER_INIT = 6000;
    //power最大值
    public static int POWER_MAX = 10000;
    //每个power的回复单位
    public static int POWER_UNIT = 1000;
    //每次执行一次服务器计算回复恢复单位
    public static int POWER_GAIN = 33;
    //结束魔法回复量
    public static int LAST_POWER_GAIN = 50;
    

    public static Dictionary<string, object> powerConfig = new Dictionary<string, object> {
        {"init", 6000},
        {"max", 10000},
        {"unit", 1000},
        {"gain", 20}
    };
    public static void initPower() {
        POWER_INIT = (int)(powerConfig["init"]);
        POWER_MAX  = (int)(powerConfig["max"]);
        POWER_UNIT = (int)(powerConfig["unit"]);
        POWER_GAIN = (int)(powerConfig["gain"]);
        LAST_POWER_GAIN = (int)( powerConfig["lastGain"] );
        BeanVO.init();
    }



    //—————————————————————————————牌信息———————————————————————————————————————————

    //手牌上限
    public static int CARD_HAND_MAX = 3;
    //连续出牌CD毫秒
    public static int CARD_USE_CD = 50;
    //NEXT到手牌CD毫秒
    public static int CARD_FILL_HAND_CD = 50;
    //补充NEXT牌CD毫秒
    public static int CARD_FILL_NEXT_CD = 50;
    //待抽牌少于几张开始洗牌
    public static int CARD_WAIT_MIN = 0;
    public static Dictionary<string, object> cardConfig = new Dictionary<string, object> {
        {"handMax", 3},
        {"useCD", 50},
        {"fillHandCD", 50},
        {"fillNextCD", 50},
        {"waitMin", 0}
    };
    public static void initCard() {
        CARD_HAND_MAX     = (int)(cardConfig["handMax"]);
        CARD_USE_CD       = (int)(cardConfig["useCD"]);
        CARD_FILL_HAND_CD = (int)(cardConfig["fillHandCD"]);
        CARD_FILL_NEXT_CD = (int)(cardConfig["fillNextCD"]);
        CARD_WAIT_MIN     = (int)(cardConfig["waitMin"]);
    }

    //—————————————————————————————地图信息———————————————————————————————————————————
    

   
    //地图格子 用于生成地图的大小
    public static int MAP_GRID_SIZE = 500;

    //客户端执行时间
    public static int MAP_ACT_TIME_C = 33;
    //服务器执行时间
    public static int MAP_ACT_TIME_S = 66;

    public static Dictionary<string, object> mapConfig = new Dictionary<string, object> {
        {"earthRadius", 12},
        {"earthHeight", 20},
        {"cameraOffsetY", 4},
        {"cameraRadius", 50},
        {"cameraObjectOffsetY", -8},
        {"objectOffsetY", 4},
        {"gridSize", 150},
        {"atcTimeC", 33},
        {"atcTimeS", 66}
    };
    public static void initMap() {        
        MAP_GRID_SIZE         = (int)(mapConfig["gridSize"]);
        MAP_ACT_TIME_C             = (int)(mapConfig["atcTimeC"]);
        MAP_ACT_TIME_S             = (int)(mapConfig["atcTimeS"]);
    }

    //—————————————————————————————玩家信息———————————————————————————————————————————
    //力转到速度的比率
    public static double PLAYER_FORCE_TO_SPEED_RATE = 0.5f;
    //靠近的速度冲量推力系数(越大越有碰碰车感)
    public static double PLAYER_FORCE_PUSH = 0.3f;
    //靠近的速度冲量推力系数(越大越有碰碰车感) 人物之间互相碰撞的系数。
    public static double PLAYER_FORCE_PUSH2 = 3f;
    //靠近的斥力系数(越大越难以重合)
    public static double PLAYER_FORCE_REPULSIVE = 0.15f;
    //所有物体 运动 最大的速度 基本没有到达这个值的。
    public static int PLAYER_VELOCITY_MAX = 800;
    //飞船的默认半径。
    public static int SHIP_RADIUS = 800;


    public static Dictionary<string, object> playerConfig = new Dictionary<string, object> {
        {"forceToSpeedRate", 0.5f},
        {"forcePush", 0.3f},
        {"forceRepulsive", 0.15f},
        {"velocityMax", 800f},
        //属性
        {"hp", 5000},
        {"joystickMax", 30},
        {"joystickMin", 0},
        {"spRate", 0.5f},
        {"aspRate", 0.5f},
        {"asp", 20f},
        {"radius", 130}
    };
    public static void initPlayerConfig() {
        PLAYER_FORCE_TO_SPEED_RATE = Convert.ToDouble(playerConfig["forceToSpeedRate"]);
        PLAYER_FORCE_PUSH          = Convert.ToDouble(playerConfig["forcePush"]);
        PLAYER_FORCE_PUSH2         = Convert.ToDouble(playerConfig["forcePush2"]);
        PLAYER_FORCE_REPULSIVE     = Convert.ToDouble(playerConfig["forceRepulsive"]);
        PLAYER_VELOCITY_MAX        = (int)(playerConfig["velocityMax"]);
        SHIP_RADIUS                = (int)(playerConfig["radius"]);
    }

    //——————————————————————————————————————const无需赋值通用的常量——————————————————————————————————————————

    //——————————————————————————————————————网络实体的唯一类型id——————————————————————————————————————————

    public const int GEOM_CIRCLE = 1;
    public const int GEOM_RECT = 2;
    public const int GEOM_LINE = 4;
    public const int GEOM_SECTOR = 8;


    public const int BUFF = 50;
    public const int BULLET_SHALLOW = 51;

    public const int ACTION_BASE = 100;
    public const int ACTION_MANAGER = 101;
    public const int MULTI_ACTION = 102;
    public const int QUEUE_ACTION = 103;

    public const int HANDLER_ACTION = 110;
    public const int INTERVAL_ACTION = 111;
    public const int TIME_ACTION = 112;

    public const int SKILL_ACTION = 120;
    public const int SKILL_STEP_ACTION = 121;
    public const int TRIGGER_ACTION = 122;
    public const int BEAM_ACTION = 123;
    public const int TRIGGER_GROUP_ACTION = 124;
    public const int CARD_GROUP_ACTION = 125;
    public const int PART_ACTION = 126;
    public const int PART_GROUP_ACTION = 127;
    public const int MAGIC_SKILL_ACTION = 128;
    public const int SKILL_MANAGER_ACTION = 129;


    public const int ENTITY_LOOP_BEAN = 500;
	public const int ENTITY_PLAYER = 501;
	public const int ENTITY_BULLET = 502;
    public const int ENTITY_CALL = 503;
    public const int ENTITY_PRICE_BEAN = 504;
    public const int ENTITY_BARRIER = 505;
    public const int ENTITY_RADISH = 506;



    public const int FIGHT_RESULT = 1000;
    public const int BIRTH_BEAN_CONTROLLER = 1001;
    public const int REFEREE_CONTROLLER = 1002;
    public const int RADISH_CONTROLLER = 1003;
    //public const int MAP = 1000;





    //——————————————————————————————————————map的delay的回调函数id——————————————————————————————————————————
    //这里区分的只是同一个类的函数里面的。 外面的是什么类型的timeAction还需要本身自己类里面去判定
    public const int MAP_CALL_BACK_REVIVE = 1;
    public const int MAP_CALL_BACK_WARNING = 2;
    public const int MAP_CALL_BACK_TOTAL_TIME = 3;
    public const int MAP_CALL_BACK_TOTAL_SHALLOW = 4;

    //———————————————————————————————————————战斗模式的定义———————————————————————————————————————————————
    //乱战
    public static int MODE_CHAOS = 1;
    //抢萝卜
    public static int MODE_RADISH = 2;

    public static int MODE_GUIDE = 8;
    //观战
    public static int MODE_WATCH = 9;


    //TODO:这两个以后不用了！ 先留着。 周末来给干掉！
    public static int MODE_TEST_WATCH = 10;
    //测试 乱斗大家一起进入的模式。
    public static int MODE_TEST_CHAOS = 12;
    public static int MODE_TEST_RADISH = 11;
    //———————————————————————————————————————网络的定义———————————————————————————————————————————————

    public const string NET_C_CREATE_USER = "netCreatePlayer";
	public const string NET_C_CHANGE_MOVE = "changeMove";
    public const string NET_C_USE_CARD = "useCard";
    public const string NET_C_INIT_MAP = "initMap";
    public const string NET_C_FIGHT_EQUAL = "fight_equal";

    
    public const string NET_C_STOP_MATCH = "stop_match";
    public const string NET_C_QUIT_MATCH = "quit_match";
    public const string NET_C_START_MATCH = "start_match";
    public const string NET_C_START_MATCH2 = "start_match2";
    public const string NET_C_USER_JOIN = "user_join";
    public const string NET_C_GET_ROOM_INFO = "get_room_info";

    public const string NET_S_ADD_USER = "addUser";
	public const string NET_S_CHANGE_MOVE = "changeMove";
    public const string NET_S_USE_CARD = "useCard";
    public const string NET_S_QUIT_MATCH = "quit_match";
    //——————————————————————————————部件——————————————————————————————————————————
    public static  int PART_COUNT = 3;
    public static  string[] PART_MODEL_PREFIX = { "head", "wing", "tail", "main", "avatar" };
    //——————————————————————————————属性——————————————————————————————————————————

    public const string PROPERTY_HP = "hp";

	/**
	 * 攻击
	 */
	public const string PROPERTY_ATK_RATE = "atkRate";

    public const string PROPERTY_BEAR_RATE = "bearRate";

    public const string PROPERTY_DEF_RATE = "defRate";

    public const string PROPERTY_CURE_RATE = "cureRate";

    public const string PROPERTY_SPEED_SCALE = "speedScale";	

	/**
	 * 转向角速度。
	 */
	public const string PROPERTY_ASP = "asp";
    
	/**
	 * 速度比率。
	 */
	public const string PROPERTY_SP_RATE = "spRate";

	public const string PROPERTY_ASP_RATE = "aspRate";

	public const string PROPERTY_JOYSTICK_MIN = "joystickMin";

	public const string PROPERTY_JOYSTICK_MAX = "joystickMax";

    public const string PROPERTY_RADIUS = "radius";

    public const string PROPERTY_COLLISION_RADIUS = "collisionRadius";
    
    public const string PROPERTY_ATTACK_EXPECT = "attackExpect";

    public const string PROPERTY_RADAR = "radar";

    public const string PROPERTY_ATTRACT = "attract";


    public static  List<string> PROPERTY_ARRAY = new List<string>(){
						PROPERTY_HP,
                        PROPERTY_ATK_RATE, PROPERTY_BEAR_RATE, PROPERTY_DEF_RATE, PROPERTY_CURE_RATE,
                        PROPERTY_SPEED_SCALE, PROPERTY_ASP, PROPERTY_SP_RATE, PROPERTY_ASP_RATE,
                        PROPERTY_JOYSTICK_MIN, PROPERTY_JOYSTICK_MAX,PROPERTY_RADIUS, PROPERTY_COLLISION_RADIUS,
                        PROPERTY_ATTACK_EXPECT,PROPERTY_RADAR, PROPERTY_ATTRACT
    };
    //————————————————————————————————————积分配置——————————————————————————————————————————————————
    ///分数分档
	public static object[] scoreConfig = {
        new Dictionary<string,object>{
            {"scoreMax",100000},
            {"mainScale",0.6f},
            {"radiusExtra",-40},
            {"bearRate",1.25f},
            {"speedScale",1.05f}
        },
        new Dictionary<string,object>{
            {"scoreMax",400000},
            {"mainScale",1.15f},
            {"radiusExtra",15},
            {"bearRate",1f},
            {"speedScale",1f}
        },
        new Dictionary<string,object>{
            {"scoreMax",1000000},
            {"mainScale",1.7f},
            {"radiusExtra",70},
            {"bearRate",0.75f},
            {"speedScale",0.95f}
        },
        new Dictionary<string,object>{
            {"scoreMax",-1},
            {"mainScale",2.25f},
            {"radiusExtra",125},
            {"bearRate",0.5f},
            {"speedScale",0.9f}
        },
    };

    //————————————————————————————————————Buff——————————————————————————————————————————————————
    public const string BUFF_CHANGE_PROPERTY = "changeProperty";
    public const string BUFF_STUN = "stun";
    public const string BUFF_SHIELD = "shield";
    public const string BUFF_HURT_SHALLOW = "hurtShallow";


    //——————————————————————————————————————————————————————————————————————————————————————
    public static  string[] CARD_IDS = {
        "C400","C401","C403", "C402","C501","C603","C701"
    };

    //———————————————————————————————————————普遍的一些常量———————————————————————————————————————————————

    public static Dictionary<string, object> combat;

    

    public static void init() {
        combat       = (Dictionary<string, object>)DataManager.inst.combat;
        cardConfig   = (Dictionary<string, object>)DataManager.inst.combat["card"];
        powerConfig  = (Dictionary<string, object>)DataManager.inst.combat["power"];
        playerConfig = (Dictionary<string, object>)DataManager.inst.combat["player"];
        mapConfig    = (Dictionary<string, object>)DataManager.inst.combat["map"];
        scoreConfig  = (object[])DataManager.inst.combat["scoreConfig"];

        initCard();
        initPower();
        initPlayerConfig();
        initMap();
        AIConstant.AI_PLAYER_NUM = ( (object[])ConfigConstant.combat["testAI"] ).Length;
        AIConstant.AI_PLAYER_NUM = 0;
    }



    //———————————————————————————————————————还没配成常量的———————————————————————————————————————————————
    ///积分显示单位
    public static  int SCORE_UNIT = 1000;
    ///组队赛专用，任意一方分数到达上限，及提前结束比赛
	public static readonly int TEAM_SCORE_MAX = 3000 * SCORE_UNIT;

    ///积分转移规则，玩家初始积分
    public static  int SCORE_INIT = 0 * SCORE_UNIT; //100000
                                                            ///每获得1点能量获得积分
    //	public static  float SCORE_POWER= 4;	//1
    ///每造成1点伤害获得积分
    public static  float SCORE_DAMAGE = 5f;     //1
                                                        ///每击杀1玩家获得积分
    public static  int SCORE_KILL_PLAYER = 0;   //20000
                                                        ///击杀玩家抢夺积分比例
    public static  float SCORE_KILL_PLAYER_RATE = 0.15f;
    ///击杀玩家抢夺积分最大值
    public static  int SCORE_KILL_PLAYER_MAX = 150 * SCORE_UNIT;

    ///死亡掉落积分蓝水晶占原积分比例
    public static  float DROP_SCORE_ITEM_RATE = 0.05f;
    ///死亡掉落蓝水晶额外值
    public static  int DROP_SCORE_ITEM_MIN = 1;
    ///死亡掉落蓝水晶上限
    public static  int DROP_SCORE_ITEM_MAX = 50;
    ///死亡掉落蓝水晶爆开力量下限
    public static  int DROP_SCORE_ITEM_FORCE_MIN = 60;
    ///死亡掉落蓝水晶爆开力量上限
    public static  int DROP_SCORE_ITEM_FORCE_MAX = 120;

    ///死亡掉落item多久后才可拾取
    public static  int DEAD_ITEM_READY_TIME = 500;
    ///死亡掉落能量珠个数（间隔一大一小）
    public static  int DEAD_ITEM_NUM = 6;
    ///死亡掉落能量珠爆开力量
    public static  int DEAD_ITEM_FORCE = 70;


    public static int RELIVE_TIME = 3000;

    public static float SPEED_RATE = 1f;
    public static float LAST_SPEED_RATE = 1.10f;

    public static int CALL_POS_RADIUS = 300;
    public static int CALL_LAYER_RADIUS = 200;
    

    //大概 * 0.66时间接收不到包。。。 就重连。
    public static  int NETWORK_DELAY = 150;

    public static  float FAKE_RATE = 0.9f;
    ///不同手牌位置对应的初始CD比例
    public static  float[] HAND_CARD_RESET_CD_PER = { 0f, 0.5f, 1f };

    ///飞船雷达距离
    public static  int RADAR_RADIUS = 2500;

    public static  int ATTRACT = 500;

    public static  int ITEM_MAG_SPEED = 400;
    ///对撞造成的伤害比例
    public static readonly float PUSH_DAMAGE_RATE = 3f;

    ///对撞造成的基础伤害
    public static  int PUSH_DAMAGE = 80;
    ///对撞造成的击退力
    public static  int PUSH_FORCE_BACK = 20;


    ///快速连杀连贯基础时间
    public static readonly int COMBO_KILL_TIME_MAX = 9000;



    ///抢萝卜专用，战斗开始准备时间
    public static readonly int RADISH_READY_TIME = 3000;
    ///抢萝卜专用，正常持旗获得1点数时间
    public static readonly int RADISH_POINT_CD = 2000;
    ///抢萝卜专用，战斗末尾持旗获得1点数时间
    public static readonly int RADISH_FINAL_POINT_CD = 1000;
    ///抢萝卜专用，任意一方点数到达上限，及提前结束比赛
    public static readonly int RADISH_POINT_MAX = 100;

    ///抢萝卜专用，玩家复活时间
    public static readonly int RADISH_RELIVE_TIME = 3000;
    ///抢萝卜专用，持有萝卜的队伍复活时间
    public static readonly int RADISH_HOLD_RELIVE_TIME = 10000;
    ///抢萝卜专用，队友参考距离
    //	public static readonly int RADISH_TEAM_DISTANCE = 3000;
    ///抢萝卜专用，萝卜开场复活时间
    public static readonly int RADISH_RADISH_START_TIME = 7000;
    ///抢萝卜专用，萝卜复活时间
    public static readonly int RADISH_RADISH_RELIVE_TIME = 2000;




}

