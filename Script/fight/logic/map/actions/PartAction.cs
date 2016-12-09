using System;
using System.Collections.Generic;
using UnityEngine;

public class PartAction: TriggerGroupAction {

    public string id;
    public int position;

    public double hp;  //耐久
    public double hpMax;  //耐久
    private double _downHp;

    public double hpRate = 0.1f;

    public PartAction(Map map, int netId = -1):base(map, netId) {

    }
    
    public override TriggerGroupAction init(Dictionary<string, object> data, PersonEntity player) {
        base.init(data, player);
        this.position = (int)(this._data["position"]);
        this.id = this._data["id"].ToString();
        this.hpRate = Convert.ToDouble(this._data["hpRate"]);
        this.hp = this.hpMax = Convert.ToDouble(this._data["hp"]);
        this._downHp = this.hpMax * ConfigConstant.MAP_ACT_TIME_S / (int)(this._data["time"]);
        return this;
    }

    public override void start() {
        base.start();
    }

    protected override void onAliveHandler() {
        base.onAliveHandler();
        this.dispatchEventWith(EventConstant.ALIVED);
    }



    public override bool isFinish {
        get {
            return base.isFinish || this.hp <= 0;
        }
    }

    override public void update() {
        base.update();
        if(this.alived) {
            this.hp -= this._downHp;
        }
    }

    public void checkDead() {
        if(this._data.ContainsKey("dead")) {
            MultiAction deadAction = new MultiAction(this._map);
            object[] deadData = (object[])this._data["dead"];
            for(int i = 0, len = deadData.Length; i < len; i++) {
                deadAction.addAction(new SkillAction(this._map).init((Dictionary<string, object>)deadData[i], this._player));
            }
            this._player.addAction(deadAction);
        }        
    }

    protected override bool canAction {
        get {
            return base.canAction && !this._player.hasBuff(ConfigConstant.BUFF_STUN);
        }
    }

    public void beHit(int damage) {
        this.hp -= damage;
        this.dispatchEventWith(EventConstant.HURT);
    }

    public override int type { get { return ConfigConstant.PART_ACTION; } }

    public override void clear() {
        base.clear();
    }


    public override void setData(Dictionary<string, object> data) {
        base.setData(data);;


        this.id = data["id"].ToString();
        this.position = (int)(data["position"]);
        this.hp = Convert.ToDouble(data["hp"]);
        this._downHp = Convert.ToDouble(data["downHp"]);
        this.hpMax = Convert.ToDouble(data["hpMax"]);
        this.hpRate = Convert.ToDouble(data["hpRate"]);

    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["id"] = this.id;
        data["position"] = this.position;
        data["hp"] = this.hp;
        data["downHp"] = this._downHp;
        data["hpMax"] = this.hpMax;
        data["hpRate"] = this.hpRate;
        return data;
    }
}
