using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ViewConstant {

    public static readonly Color SHADER_COLOR_STANDARD = new Color(0.2f, 0.2f, 0.2f, 0.2f);
    public static readonly Color SHADER_COLOR_ALPHA = new Color(0.2f, 0.2f, 0.2f, 0.05f);
    public static readonly Color SHADER_COLOR_WHITE = new Color(0.3f, 0.3f, 0.3f, 0.2f);
    public static readonly Color SHADER_COLOR_YELLOW = new Color(1f, 1f, 0.2f, 0.2f);
    public static readonly Color SHADER_COLOR_LIGHT = new Color(1f, 1f, 1f, 0.2f);
    public static readonly Color SHADER_COLOR_RED = new Color(1f, 0.05f, 0.05f, 0.2f);
    public static readonly Color SHADER_COLOR_WARM = new Color(0.5f, 0.5f, 0.2f, 0.2f);

    ///标准颜色，不要太亮
    public static readonly Color COLOR_STANDARD = new Color(0.9f, 0.9f, 0.9f, 1f);
    public static readonly Color COLOR_ENERGE_TIME = new Color(1f, 0.4f, 0.7f, 1f);
    public static readonly Color COLOR_ENERGE_TIME_SKY = new Color(0.9f, 0.9f, 0.9f, 1f);
    public static readonly Color COLOR_ENERGE_TIME_FOG = new Color(0.9f, 0.9f, 0.9f, 0.9f);
    public static readonly Color COLOR_READY = new Color(1f, 0.9f, 0.2f, 1f);
    public static readonly Color COLOR_EGG = new Color(0.8f, 0.3f, 1f, 1f);


    public static readonly Color COLOR_POWER_DEFAULT = new Color(0.98f, 0.88f, 0.4f, 1f);
    public static readonly Color COLOR_POWER_GLOW = new Color(1f, 1f, 1f, 1f);

    //所有飞船shader的统一名称
    public static readonly string SHIP_SHADER_NAME = "meng52/ShipShader";
    public static readonly string SHIP_SHADER_SIMPLE_NAME = "meng52/ShipSimpleShader";
    public static readonly string SHIP_SHADER_LIGHT_NAME = "meng52/ShipLightShader";
    public static readonly string EFFECT_SHADER_NAME = "meng52/EffectShader";

    public static readonly string EARTH_SHADER_NAME = "meng52/EarthShader";
    public static readonly string COLOR_SHADER_NAME = "meng52/ColorShader";
    public static readonly string HUE_SHADER_NAME = "meng52/HueShader";
    public static readonly string DOUBLE_COLOR_SHADER_NAME = "meng52/DoubleColorShader";


    //摄像机偏移
    public static float MAP_CAMERA_OFFSET_Y = 4;

    //地图物品视角(对齐到地球)Y偏移
    public static float MAP_CAMERA_OBJECT_OFFSET_Y = -12;


	public static readonly float CAMERA_FIELD_MIN = 11.9f;
                                                         ///摄像机视野焦距基础
	public static readonly float CAMERA_FIELD = 15.4f; 
                                                     ///摄像机视野焦距最大
	public static readonly float CAMERA_FIELD_MAX = 18.9f;
                                                         ///每点雷达距离，对应摄像机视野
    public static readonly float RADAR_TO_CAMERA_FIELD = CAMERA_FIELD / ConfigConstant.RADAR_RADIUS;




    public static Dictionary<string, object> mapConfig;


    ///战舰色相
	public static readonly Dictionary<string, object> shipConfig = new Dictionary<string, object>{
        {
            "team1",	//标准蓝队战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.1f},
                {"value",0.1f},
                {"passA","0024D5"},
                {"passB","00AACC"},
                {"passC","00F4FF"},
            }
        },
        {
            "team2",	//标准红队战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.1f},
                {"value",0.1f},
                {"passA","FF1000"},
                {"passB","CC9800"},
                {"passC","FFCC30"},
            }
        },
        {
            "ship800", //标准蓝色战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.1f},
                {"value",0f},
                {"passA","0024D5"},
                {"passB","FF9800"},
                {"passC","00F4FF"},
			}
        },
        {
            "ship801",	//临时红色战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.1f},
                {"value",0.1f},
                {"passA","ff1000"},
                {"passB","00ddff"},
                {"passC","ffcc30"},
            }
        },
        {
            "ship802",  //临时黄色战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.1f},
                {"value",0.15f},
                {"passA","ffaa00"},
                {"passB","c0ff40"},
                {"passC","40ff40"},
            }
        },
        {
            "ship810",  //黄色球体战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.3f},
                {"value",0.1f},
                {"passA","dd8800"},
                {"passB","ffdd00"},
                {"passC","00acfc"},
            }
        },
        {
            "ship820",  //白蓝色方战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.2f},
                {"value",0.1f},
                {"passA","bbbbbb"},
                {"passB","00ccee"},
                {"passC","ccaa00"},
            }
        },
        {
            "ship830",  //刀锋蓝紫战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.1f},
                {"value",0f},
                {"passA","1100ff"},
                {"passB","ff33cc"},
                {"passC","00ffff"},
            }
        },
        {
            "ship840",  //棕色鬼头蒸汽战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",1f},
                {"value",0.05f},
                {"passA","472D00"},
                {"passB","999999"},
                {"passC","FF0000"},
            }
        },
        {
            "ship850",  //红色鲨鱼头圆环战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.15f},
                {"value",0f},
                {"passA","FF0000"},
                {"passB","CC6600"},
                {"passC","FF6600"},
            }
        },
        {
            "ship860",  //绿黄色尖头战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.2f},
                {"value",0f},
                {"passA","00DD00"},
                {"passB","FFFF40"},
                {"passC","FFFF60"},
            }
        },
        {
            "ship870",  //粉黄色童话战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.3f},
                {"value",0.1f},
                {"passA","FF99CC"},
                {"passB","FFDD10"},
                {"passC","FFDD10"},
            }
        },
        {
            "ship999",  //粉黄色童话战机
			new Dictionary<string,object>{
                {"hue",0f},
                {"saturation",0.3f},
                {"value",0.1f},
                {"passA","FF99CC"},
                {"passB","FFDD10"},
                {"passC","FFDD10"},
            }
        },

    };



    private static void initMap() {
        MAP_CAMERA_OFFSET_Y        = Convert.ToSingle(ConfigConstant.mapConfig["cameraOffsetY"]);
        MAP_CAMERA_OBJECT_OFFSET_Y = Convert.ToSingle(ConfigConstant.mapConfig["cameraObjectOffsetY"]);
    }



    ///各个队伍的颜色配置0自己1友军（蓝队）2敌军3紫队,   内部数组分别为浅色，深色，正常色，血条色
    public static readonly string[][] teamColorStrs = {
        new string[]{"aaf8ff","0042cc","41bbff","66eeff"},
        new string[]{"7dd8ff","0d29bb","0064ee","1166ff"},
        new string[]{"ff9988","772211","ee1100","ee4433"},
        new string[]{"df8aff","3b0b67","5522aa","aa44ee"}
    };
    ///整理后各个队伍的颜色配置，第一维度为队伍编号0自己1友军（蓝队）2敌军3紫队
    public static List<List<Color>> teamColors = new List<List<Color>>();

    private static void initColor() {
        for(int i = 0, len = ViewConstant.teamColorStrs.Length; i < len; i++) {
            ViewConstant.teamColors.Add(new List<Color>());
            for(int j = 0, len2 = ViewConstant.teamColorStrs[i].Length; j < len2; j++) {
                ViewConstant.teamColors[i].Add(MaterialUtils.stringToColor(ViewConstant.teamColorStrs[i][j]));
            }
        }
    }

    public static void init() {
        initMap();
        initColor();
    }



    public static readonly float SHIP_ANGLE_SPEED_RATE = 0.3f;
}