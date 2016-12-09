using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BeamAction:TriggerAction {

    
    private GeomBase _seekShape;

    private GeomBase _hitShape;

    public PersonEntity target;

    public int hitCount = 0;

    private double _deepA = 0;
    private double _deepB = 0;

    private int _maxHitCount;
    

    public BeamAction(Map map, int netId = -1):base(map, netId) {

    }

    public override TriggerAction initTrigger(int interval, int totalTime, Dictionary<string, object> triggerData, PersonEntity player, PersonEntity lockTarget) {
        base.initTrigger(interval, totalTime, triggerData, player, lockTarget);
        Dictionary<string, object> skillData = this.skillData;
        Dictionary<string, object> shapeData = (Dictionary<string, object>)skillData["seek"];
        this._seekShape = Collision.createGeom(this._person, (object[])shapeData["range"], (object[])shapeData["offset"]);
        shapeData = (Dictionary<string, object>)skillData["hit"];
        this._hitShape = Collision.createGeom(this._person, (object[])shapeData["range"], (object[])shapeData["offset"]);

        this._maxHitCount = (int)(skillData["count"]);

        this._deepA = Convert.ToDouble(skillData["deepA"]);
        this._deepB = Convert.ToDouble(skillData["deepB"]);
        this._person.map.specialAction(this);
        return this;
    }

    private int damage {
        get { return Mathf.CeilToInt(Convert.ToSingle(this._deepA * this.hitCount + this._deepB)); }
    }

    public override void start() {
        base.start();
    }

    //protected override void intervalHandler() {
    //    if(null != this.target) {
    //        Dictionary<string, object> bulletData = (Dictionary<string, object>)skillData["bullet"];
    //        BulletEntity bullet = this._player.map.createFightEntity(ConfigConstant.ENTITY_BULLET) as BulletEntity;
    //        bullet.targets = new List<PersonEntity>() { this.target};
    //        bullet.owner = this._player;
    //        bullet.initConfig(bulletData);
    //        bullet.atk = this.damage;
    //        this.hitCount = Math.Min(this.hitCount + 1, this._maxHitCount);
    //    }
    //}

    protected override void useSkill() {
        if(null != this.target) {
            Dictionary<string, object> bulletData = (Dictionary<string, object>)skillData["bullet"];
            BulletEntity bullet = this._person.map.createFightEntity(ConfigConstant.ENTITY_BULLET) as BulletEntity;
            bullet.lockTarget = this.target;
            bullet.owner = this._person;
            bullet.initConfig(bulletData);
            bullet.atk = this.damage;
            this.hitCount = Math.Min(this.hitCount + 1, this._maxHitCount);
            this._cdTime.reset();
        }
    }

    public override void update() {
        if(null == this.target) {
            List<PersonEntity> targets = this._person.map.getPerson(this._seekShape);
            if(targets.Count > 0) {
                this.target = targets[0];
                this.hitCount = 0;
            }
        }
        //找到敌人 判断敌人是否走出范围。
        if(null != this.target) {
            if(!this.target.alived || !Collision.checkCollision(this._hitShape, this.target.shape, this._map.mapData).isHit) {
                this.target = null;
            }
        }
        base.update();
    }

    public override void reset() {
        base.reset();
        this.target = null;
    }

    override public int type { get { return ConfigConstant.BEAM_ACTION; } }

    public PersonEntity player {
        get { return this._person; }
    }

    public Dictionary<string, object> skillData {
        get { return (Dictionary<string, object>)this._triggerData["beam"]; }
    }

    public override void clear() {
        this.dispatchEventWith(EventConstant.DEAD);
        base.clear();
        Utils.clearObject(this._seekShape);
        Utils.clearObject(this._hitShape);
        this._seekShape = null;
        this._hitShape = null;
        this.target = null;
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        
        this._seekShape = (GeomBase)this._map.getNetObject((int)(data["seekShape"]));
        
        this._hitShape = (GeomBase)this._map.getNetObject((int)(data["hitShape"]));
        if(data.ContainsKey("target")) {
            this.target = (PersonEntity)this._map.getNetObject((int)(data["target"]));
        }
        
        this._maxHitCount = (int)(data["maxHitCount"]);
        this._deepA = Convert.ToDouble(data["deepA"]);
        this._deepB = Convert.ToDouble(data["deepB"]);
        this.hitCount = (int)(data["hitCount"]);
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["seekShape"] = this._seekShape.netId;
        data["hitShape"] = this._hitShape.netId;
        if(null != this.target) {
            data["target"] = this.target.netId;
        }
        
        data["maxHitCount"] = this._maxHitCount;
        data["deepA"] = this._deepA;
        data["deepB"] = this._deepB;
        data["hitCount"] = this.hitCount;
        return data;
    }

    
}
