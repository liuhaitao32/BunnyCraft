using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///播报配置
///author Biggo
public class RadioConfig {
    ///枚举的播报模式(加入时判断)
    public enum RadioTypes { Kill, GetRadish };
    public enum KillRadioTypes { FirstBlood, Combo, Life, ShutDown, Kill };

    ///播报显示基础最长时间
    public static readonly int RADIO_TIME_MAX = 3000;
    ///播报显示基础最短时间
    public static readonly int RADIO_TIME_MIN = 1000;
    ///快速连杀大于某数时都显示相同提示
    public static readonly int COMBO_MAX = 7;
    ///快速连杀最小标准
    public static readonly int COMBO_MIN = 2;
    ///单命累计大于某数时都显示相同提示
    public static readonly int LIFE_MAX = 7;
    ///单命累计最小标准
    public static readonly int LIFE_MIN = 3;


    ///引导播报显示最长时间
    public static readonly int RADIO_GUIDE_TIME_MAX = 5000;


    ///快速连杀的提示信息
    public static readonly Dictionary<int, object> comboConfig = new Dictionary<int, object>{
        {
            2,
            new Dictionary<string,object>{
                {"info","二连杀!"},
                {"infoEn","Double kill!"},
                {"voiceIDo","voiceCombo2"},
                {"time",1.1f},
                {"lightning",0.5f},
            }
        },
        {
            3,
            new Dictionary<string,object>{
                {"info","三连杀!"},
                {"infoEn","Triple kill!"},
                {"voiceIDo","voiceCombo3"},
                {"time",1.2f},
                {"lightning",0.6f},
            }
        },
        {
            4,
            new Dictionary<string,object>{
                {"info","四连杀!"},
                {"infoEn","Quadra kill!"},
                {"voiceIDo","voiceCombo4"},
                {"time",1.4f},
                {"lightning",0.7f},
            }
        },
        {
            5,
            new Dictionary<string,object>{
                {"info","五连杀!!"},
                {"infoEn","Penta kill!!"},
                {"voiceIDo","voiceCombo5"},
                {"time",1.6f},
                {"lightning",0.8f},
            }
        },
        {
            6,
            new Dictionary<string,object>{
                {"info","六连杀!!!"},
                {"infoEn","Hexa kill!!!"},
                {"voiceIDo","voiceCombo6"},
                {"time",1.8f},
                {"lightning",0.9f},
            }
        },
        {
            7,
            new Dictionary<string,object>{
                {"info","MAX连杀!!!"},
                {"infoEn","Hepta kill!!!"},
                {"voiceIDo","voiceCombo7"},
                {"time",2f},
                {"lightning",1f},
            }
        },
    };

    ///单命累计杀的提示信息
    public static readonly Dictionary<int, object> lifeConfig = new Dictionary<int, object>{
        {
            3,
            new Dictionary<string,object>{
                {"info","大杀特杀"},
                {"infoEn","Killing spree"},
                {"voiceIDo","voiceLifeKill3"},
                {"time",1.1f},
                {"lightning",0.5f},
            }
        },
        {
            4,
            new Dictionary<string,object>{
                {"info","杀人如麻"},
                {"infoEn","Rampage"},
                {"voiceIDo","voiceLifeKill4"},
                {"time",1.2f},
                {"lightning",0.6f},
            }
        },
        {
            5,
            new Dictionary<string,object>{
                {"info","无人能挡!"},
                {"infoEn","Unstoppable!"},
                {"voiceIDo","voiceLifeKill5"},
                {"time",1.4f},
                {"lightning",0.7f},
            }
        },
        {
            6,
            new Dictionary<string,object>{
                {"info","主宰比赛!"},
                {"infoEn","Godlike!"},
                {"voiceIDo","voiceLifeKill6"},
                {"time",1.6f},
                {"lightning",0.8f},
            }
        },
        {
            7,
            new Dictionary<string,object>{
                {"info","超神!!!"},
                {"infoEn","Legendary!!!"},
                {"voiceIDo","voiceLifeKill7"},
                {"time",2f},
                {"lightning",1f},
            }
        }
    };

    ///抢萝卜首杀的提示信息
    public static readonly Dictionary<string, object> firstBlood = new Dictionary<string, object> {
        { "info","第一滴血!" },
        { "infoEn","First blood!"},
        { "voiceIDo","voiceFirstBlood"},
        { "time",1.1f },
        { "lightning",0.5f},
    };

    ///终结的提示信息
    public static readonly Dictionary<string, object> shutDown = new Dictionary<string, object> {
        { "info","终结!" },
        { "infoEn","Shut down!"},
        { "voiceIDo","voiceShutDown"},
        { "time",1.5f },
        { "lightning",0.5f},
    };
    ///普通击杀的提示信息
    public static readonly Dictionary<string, object> kill = new Dictionary<string, object> {
        { "info","击杀" },
        { "infoEn","Kill"},
        { "voiceIDo","voiceIKill"},
//		{ "voiceDoMe","voiceKillMe"},
//		{ "voiceAllyDo","voiceAllyKill"},
//		{ "voiceEnermyDo","voiceEnermyKill"},
		{ "time",1f },
    };


    ///其他提示信息
    public static readonly Dictionary<string, object> othersConfig = new Dictionary<string, object> {
        {
            "eggWarmStart",
            new Dictionary<string,object> {
                { "info","魔方即将出现" },
                { "infoEn","The cube will appear"},
                { "time",2f },
                { "color","9933EE" },
                { "lightning",1f },
            }
        }, {
            "eggWarmEnd",
            new Dictionary<string,object> {
                { "info","魔方出现了!" },
                { "infoEn","The cube has appeared!"},
                { "time",2f },
                { "color","9933EE" },
                { "lightning",1f },
            }
        }, {
            "getRadish",
            new Dictionary<string,object> {
                { "info","抢到方块!" },
                { "infoEn","got the Cube!"},
                { "time",2f },
                { "lightning",0.7f },
                { "voiceIDo","voiceGetCube"},
                { "voiceAllyDo","voiceGetCube"},
            }
        },
    };
}
