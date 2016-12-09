using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class PartGroupAction:ActionBase {

	private PlayerEntity _player;

    private List<PartAction> _parts;

    public PartGroupAction(Map map, int netId = -1):base(map, netId) {

    }

    //初始化加载到player中
    public void init (PlayerEntity player) {
		this._player = player;
        this._parts = new List<PartAction>();

        for(int i = 0, len = ConfigConstant.PART_COUNT; i < len; i++) {
            this._parts.Add(null);
        }
	}

    public override bool isFinish {
        get {
            return false;
        }
    }

    //添加一个部件
    public void addPart (Dictionary<string, object> partData) {
        int type = (int)(partData["position"]);
        //查找到 前缀名字 来判断类型 0 head 1 wing 2 tail
		this.removePart (type);
        PartAction part = (PartAction)(new PartAction(this._map).init(partData, this._player));
        part.start();
        this._parts[type] = part;
        this.dispatchEventWith(EventConstant.PART_CHANGE);
    }

    public bool hasPart { get { return this._parts.Exists((e) => { return null != e; }); } }

    public void removeParts () {
		for (int i = 0; i < ConfigConstant.PART_COUNT; i++) {
			this.removePart(i);
		}
    }

    public PartAction getPart(int type, bool alive = false) {
        PartAction part = this._parts[type];
        if(null == part) return null;
        if(alive && part.alived) return part;
        return part;
    }

    public List<PartAction> parts {
        get {
            return this._parts.FindAll((e) => { return null != e; });
        }
    }

    //移除一个部件，并还原默认位置的模型
    public void removePart (int type) {
        PartAction part = this.getPart(type);
        Utils.clearObject(part);
        this._parts[type] = null;
    }

    public override int type { get { return ConfigConstant.PART_GROUP_ACTION; } }

    override public void update(){
        base.update();
        for(int i = 0, len = this._parts.Count; i < len; i++) {
            PartAction part = this._parts[i];
            if(null != part) {
                if(part.isFinish) {
                    part.checkDead();
                    this.removePart(part.position);
                    this.dispatchEventWith(EventConstant.PART_CHANGE);
                } else {
                    part.update();
                }
                
            }
        }
	}

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this._player = (PlayerEntity)this._map.getNetObject((int)(data["player"]));
        this._parts = new List<object>((object[])data["parts"]).ConvertAll<PartAction>((e)=> { return null == e ? null : (PartAction)this._map.getNetObject((int)e); });

    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["player"] = this._player.netId;
        data["parts"] = this._parts.ConvertAll<object>((e) => {
            if(null == e) return null;
            return e.netId;
        }).ToArray();
        return data;
    }

    public override void clear() {
        base.clear();
        this._player = null;
    }
}
