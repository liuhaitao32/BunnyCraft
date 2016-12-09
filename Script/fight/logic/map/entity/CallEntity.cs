using System;
using System.Collections.Generic;
using UnityEngine;

public class CallEntity : PersonEntity {

    private GeomBase _seekRange;

    public TimeAction _lifeTime;

    public PersonEntity seekTarget;

    public Dictionary<string, object> _seekData = null;

    private GeomBase _attackRange;

    public Dictionary<string, object> _attackData = null;

    private MagicSkillAction _attackAction;

    private GeomBase _fakeShape;

    public Vector2D offset = Vector2D.createVector();

    public int index;

    private int _layer;

    private int _seekRadius;

    public CallEntity(Map map, int netId = -1) : base(map, netId) {

    }

    public override void initConfig(Dictionary<string, object> data = null) {
        //this.scale = 0.5;
        this.level = (int)data["base_level"];
        this.id = data["name"].ToString();
        data = (Dictionary<string, object>)DataManager.inst.beckon[this.id];
        this._seekData = (Dictionary<string, object>)data["seekCondition"];
        this._seekRange = Collision.createGeom(this.ownerPlayer, (object[])this._seekData["range"], (object[])this._seekData["offset"]);
        this._seekRadius = this._seekRange.radius;

        this._attackData = (Dictionary<string, object>)data["attackCondition"];
        this._attackRange = Collision.createGeom(this, (object[])this._attackData["range"], (object[])this._attackData["offset"]);
        
        this._position = Vector2D.createVector2(600, this.ownerPlayer.angle + Math.PI).add(this.ownerPlayer.position);
        this._lifeTime = new TimeAction(this._map).init((int)data["time"]);
        base.initConfig(data);
        this._fakeShape = Collision.createGeom(this, ( new object[] { this._shape.radius * 3 } ));
        this._fakeShape.applyEntity = false;
        this._attackRange.applyEntity = false;
        this._angle = this.ownerPlayer.angle;
        this.ownerPlayer.addCallEntity(this);
        this._joystickForce.length = 1;
    }
    

    public override void update() {
        if(this._lifeTime.isFinish) {
            this.clear();
            return;
        }
        this._lifeTime.update();
        base.update();
        if(this.hasBuff(ConfigConstant.BUFF_STUN)) return;
        this.seekPlayer();
        this.checkAttack();
    }

    private void checkAttack() {
        if(null != this._attackAction) {
            if(!this._attackAction.isFinish) {
                this._attackAction.update();
            }

            if(this._attackAction.isFinish) {
                this.dispatchEventWith(EventConstant.DEAD, this);//带这个参数 就代表我是被爆炸 特殊移除的。
                this.clear();
            }
        }
    }

    private void checkCollision() {
        if(this._joystickForce.length <= 0.2f) return;

        //检查碰撞 提前给召唤物转向。
        Vector2D v = Vector2D.createVector2(this.getProperty(ConfigConstant.PROPERTY_JOYSTICK_MAX) * 3f, this._angle);
        this._fakeShape.radius = this._shape.radius * 3;
        this._fakeShape.position.copy(v.add(this._position));
        v.clear();
        Vector2D vv = this._joystickForce.clone();
        //绕过障碍物和人。
        List<FightEntity> entitys = this._map.getFightEntitysByRange(this._fakeShape, new List<int>{ ConfigConstant.ENTITY_BARRIER, ConfigConstant.ENTITY_PLAYER }, null, -1);

        for(int i = 0, len = entitys.Count; i < len; i++) {
            FightEntity entity = entitys[i];
            if(entity == this.seekTarget) continue;
            int dir = entity.findDelta.sign(vv);
            Vector2D v2 = dir < 0 ? vv.reverse().perp : vv.perp;
            double overlapRate = ( ( this._fakeShape.radius + entity.shape.radius ) - entity.findDist ) / entity.findDist;
            v2.length = Math.Min(overlapRate * 0.5f, 0.5f);//越近给的转向力越大。
            this._joystickForce.add(v2);
            this._joystickForce.truncate(1f);
            v2.length = overlapRate * 10f;//越近给的转向力越大。        
            //v2.length = 5f;//越近给的转向力越大。   
            this.addForce(v2);
            v2.clear();
        }
        vv.clear();
    }

    public override void applyPosition() { 
        base.applyPosition();
    }
    

    protected override double changeDir() {
        if(this.hasBuff(ConfigConstant.BUFF_STUN)) return 0;
        if(null == this.seekTarget) {
            Vector2D offset = this.offset.clone();
            offset.angle += this.ownerPlayer.angle;
            double expect = 10;
            Vector2D targetPos = this.ownerPlayer.velocity.clone().multiply(expect).add(offset).add(this.ownerPlayer.position);
            CollisionInfo info = Collision.realPosition(this._position, targetPos, this._map.mapData);
            if(info.deltaPos.lengthSQ <= 2000 /*&& Math.Abs(Vector2D.angleBetween(info.deltaPos, this.ownerPlayer.velocity)) >= Math.PI / 2*/) {
                this._joystickForce.length = 0.01f;
                double angle = this._joystickForce.angle;                
                this._joystickForce.angle = angle + Math2.deltaAngle(angle, this.ownerPlayer.angle) * 0.3f;
            } else {
                this._joystickForce.angle = info.deltaPos.divide(expect).angle;
                this._joystickForce.length = Math.Min(1, info.deltaPos.length / this.getProperty(ConfigConstant.PROPERTY_JOYSTICK_MAX));
                this.checkCollision();
            }

        } else {
            double expect = this.getProperty(ConfigConstant.PROPERTY_ATTACK_EXPECT);
            Vector2D targetPos = this.seekTarget.position;
            //面向敌人。
            CollisionInfo info = Collision.realPosition(this._position, targetPos, this._map.mapData);            
            //角度 直接面向敌人。
            this._joystickForce.angle = info.deltaPos.angle;
            //开始预估敌人位置。
            targetPos = this.seekTarget.velocity.clone().multiply(expect).add(this.seekTarget.position);
            info = Collision.realPosition(this._position, targetPos, this._map.mapData);
            //攻击预估的位置。
            this._attackRange.angle = info.deltaPos.angle;
            this._attackRange.position.copy(this._position);
            //敌人的位置。
            this._fakeShape.radius = this.seekTarget.shape.radius;
            this._fakeShape.position.copy(targetPos);
            Vector2D deltaPos = info.deltaPos.clone();
            double radius = this._attackRange.radius + this._fakeShape.radius;
            if(deltaPos.length >= radius) {
                deltaPos.length = radius;
                targetPos.subtract(deltaPos);
                //到目标点的距离。
                info = Collision.realPosition(this._position, targetPos, this._map.mapData);

                double len = Math.Min(1, info.deltaPos.divide(expect).length / this.getProperty(ConfigConstant.PROPERTY_JOYSTICK_MAX));
                //不给0 给0会导致不能转向了。
                this._joystickForce.length = len < 0.2f ? 0.00001f : len;
            } else {
                this._joystickForce.length = 0.00001f;
            }
            
            this.checkCollision();
        }
        return base.changeDir();
    }

    private void seekPlayer() {
        //脱离追踪范围。
        if(null != this.seekTarget && ( !this.seekTarget.alived || !Collision.checkCollision(this._seekRange, this.seekTarget.shape, this._map.mapData).isHit )) {
            this.seekTarget = null;
            Utils.clearObject(this._attackAction);
            this._attackAction = null;
        }
        //没有人的时候查询范围找敌人。
        if(null == this.seekTarget) {
            List<PersonEntity> players = this.map.getPerson(this._seekRange, (int)this._seekData["aim"], (int)this._seekData["sort"], false);

            if(players.Count > 0) {
                this.seekTarget = players[0];
                this._attackAction = new MagicSkillAction(this._map).init((Dictionary<string, object>)this._data["attack"], this) as MagicSkillAction;
                this._attackAction.start();
            }
        }
    }
    
    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["seekRange"] = this._seekRange.netId;
        data["lifeTime"] = this._lifeTime.netId;
        data["seekData"] = this._seekData;
        if(null != this.seekTarget) {
            data["seekTarget"] = this.seekTarget.netId;
        }        
        data["attackRange"] = this._attackRange.netId;
        data["attackData"] = this._attackData;
        if(null != this._attackAction) {
            data["attackAction"] = this._attackAction.netId;
        }
        data["fakeShape"] = this._fakeShape.netId;

        data["index"] = this.index;
        data["layer"] = this._layer;
        data["offset"] = this.offset.getData();
        return data;
    }


    public override void setData(Dictionary<string, object> data) {
        this._seekRange = (GeomBase)this._map.getNetObject((int)( data["seekRange"] ));
        this._lifeTime = (TimeAction)this._map.getNetObject((int)( data["lifeTime"] ));
        this._seekData = (Dictionary<string, object>)( data["seekData"] );

        if(data.ContainsKey("seekTarget")) {
            this.seekTarget = (PersonEntity)this._map.getNetObject((int)( data["seekTarget"] ));
        }
        this._attackRange = (GeomBase)this._map.getNetObject((int)( data["attackRange"] ));
        this._attackData = (Dictionary<string, object>)( data["attackData"] );
        if(data.ContainsKey("attackAction")) {
            this._attackAction = (MagicSkillAction)this._map.getNetObject((int)( data["attackAction"] ));
        }
        this._fakeShape = (GeomBase)this._map.getNetObject((int)( data["fakeShape"] ));
        this.index = (int)data["index"];
        this._layer = (int)data["layer"];
        this.offset = Vector2D.createVector4((Dictionary<string, object>)data["offset"]);
        base.setData(data);
    }

    public override int type { get { return ConfigConstant.ENTITY_CALL; } }

    public int layer {
        set {
            this._layer = value;
            this._seekRange.radius = this._seekRadius + this._layer * ConfigConstant.CALL_LAYER_RADIUS;
        }
    }
    

    public override void changeHp(int value) {
        base.changeHp(value);
        if(this.hp <= 0) this.clear();
    }

    public override void clear() {
        this.dispatchEventWith(EventConstant.DEAD);
        this.ownerPlayer.removeCallEntity(this);
        Utils.clearObject(this._seekRange);
        Utils.clearObject(this._lifeTime);
        this.seekTarget = null;
        this._seekData = null;

        Utils.clearObject(this._attackRange);
        Utils.clearObject(this._attackAction);
        Utils.clearObject(this._fakeShape);
        this.offset.clear();
        this.offset = null;
        base.clear();

    }
}
