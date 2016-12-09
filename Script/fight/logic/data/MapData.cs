using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapData {
    //逻辑属性

    ///场景类型 1乱斗 2组队战 3观战
    public int fightMode;

    ///玩家总人数
    public int playerCount;
    ///地图尺寸
    public string id;

    ///战斗总时间
    public int fightTime;
    ///战斗最终时间
    public int finalTime;
    ///加时赛时间
    public int overTime;


    ///横向格子数量
    public int colNum;
    ///纵向格子数量
    public int rowNum;

    ///场景逻辑宽(计算得出)
    public int width;
    ///场景逻辑高(计算得出)
    public int height;
    ///场景逻辑宽的一半(计算得出)
    public int widthHalf;
    ///场景逻辑高的一半(计算得出)
    public int heightHalf;


    ///地图里同时拥有的item0数量
    public int bean1Count;
    ///地图里同时拥有的item1数量
    public int bean2Count;
    ///地图里同时拥有的item2数量
    public int bean3Count;
    ///地图里同时拥有的障碍物总数
    public int barrierCount;
    ///从外边开始计算，Y轴区域线[A,B,C,D]   【真空区域】 A 【加血道具区域】 B 【障碍物,加血道具区域】 C 【障碍物,玩家和能量道具区域】 D 【玩家和能量道具区域】
    public int[] gridYAreaLines;

    //public object[][] item0s;
    ///地图首次初始化时大水晶的锁定位置【新增】
    //public object[][] item1s;
    ///地图首次初始化时障碍物的锁定位置【新增】
    public object[] barriers;
    ///地图到达指定时间产生的地图事件描述【新增】
    //public Dictionary<int, object> events;

    public List<object> playerInitData;

    public MapData(string id, int fightMode) {
        this.fightMode = fightMode;
        this.id = id;
        
        //		string id = stageSizeMode.ToString();

        Dictionary<string, object> data;
        data = (Dictionary<string, object>)ConfigConstant.mapConfig[id];
        this.initConfig(data);
        this.initViewConfig(data);//这个函数后端不用抄。
        //显示属性
    }


    private void initConfig(Dictionary<string, object> data) {
        //逻辑属性
        this.playerCount    = (int)data["playerCount"];
        this.fightTime      = (int)data["fightTime"];
        this.finalTime      = (int)data["finalTime"];
        this.colNum         = (int)data["colNum"];
        this.rowNum         = (int)data["rowNum"];
        this.bean1Count     = (int)data["bean1Count"];
        this.bean2Count     = (int)data["bean2Count"];
        this.bean3Count     = (int)data["bean3Count"];
        this.barrierCount   = (int)data["barrierCount"];
        this.gridYAreaLines = Array.ConvertAll<object, int>((object[])data["gridYAreaLines"], (object o)=> { return (int)o; });//把里面的object类型转化成int 后端应该直接赋值就行。
        this.barriers = data.ContainsKey("barriers") ? (object[])data["barriers"] : null;
        this.playerInitData = data.ContainsKey("players") ? new List<object>((object[])data["players"]) : null;
        if(data.ContainsKey("overTime")) this.overTime = (int)data["overTime"];
        //this.fightTime = 20;
        //this.finalTime = 10;
        //this.overTime = 30;

        this.width          = this.colNum * ConfigConstant.MAP_GRID_SIZE;
        this.height         = this.rowNum * ConfigConstant.MAP_GRID_SIZE;
        this.widthHalf      = this.width / 2;
        this.heightHalf     = this.height / 2;
    }

    public bool isChaos { get { return this.fightMode == ConfigConstant.MODE_CHAOS || this.fightMode == ConfigConstant.MODE_TEST_CHAOS; } }
    public bool isWatch { get { return this.fightMode == ConfigConstant.MODE_WATCH || this.fightMode == ConfigConstant.MODE_TEST_WATCH; } }
    public bool isTeam { get { return this.isRadish; } }
    public bool isRadish { get { return this.fightMode == ConfigConstant.MODE_RADISH || this.fightMode == ConfigConstant.MODE_TEST_RADISH ; } }
    //——————————————————————————————以下后端不用记，这个是视图用的——————————————————————————————————————————

    ///地球缩放
    public float earthScale;

    ///场景圆筒/地图物品显示半径（计算得出）
    public float earthRadius;
    ///场景圆筒可用高度（双向总高度）（计算得出）
    public float earthYLength;
    ///摄像机高度半径（计算得出）
    public float cameraRadius;
    ///雾层缩放（计算得出）
    public float fogScale;

    ///地图物品摆放位置Y偏移(包含逻辑y偏移)（计算得出）
    public float objectOffsetY;
    ///每点逻辑宽对应的弧度（计算得出）
    public float xToRadian;
    ///每点逻辑高对应的实际高度（计算得出）
    public float yToLength;

    private void initViewConfig(Dictionary<string, object> data) {
        //显示属性
        this.earthScale    = Convert.ToSingle(data["earthScale"]);
        this.earthRadius   = this.earthScale * 12f;
        this.earthYLength  = this.earthScale * 21f;
        this.cameraRadius  = this.earthRadius + 38f;
        this.fogScale      = 454f - this.earthScale * 24f;

        this.objectOffsetY = 1f * this.earthScale - this.earthYLength / 2;
        this.xToRadian     = 1f / this.width * Mathf.PI * 2;
        this.yToLength     = this.earthYLength / this.height;
    }
}
