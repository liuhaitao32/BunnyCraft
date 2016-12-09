using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonEntity:FightEntity {


	public Dictionary<string, double> _property = new Dictionary<string, double>();

	public Dictionary<string, double> _helpProperty = new Dictionary<string, double>();

	/**
	 * 判断当前属性是否改变了！
	 */
	protected bool _propertyChange = false;

	public int hp = 100;

    public ActionManager actionManager;


    /**
	 * 遥感力 记录一个比值 0~1之间的比值。
	 */
    protected Vector2D _joystickForce = Vector2D.createVector();

    protected Vector2D _steeringForce = Vector2D.createVector();
    public Vector2D steeringForce { get { return this._steeringForce; } }

    protected bool _dead = false;


    protected List<Buff> _buffs = new List<Buff>();
    
    /// 已buff id 为key值 里面装的是某一类卡的buff。
    protected Dictionary<string, List<Buff>> _buffMap = new Dictionary<string, List<Buff>>();

    public SkillManager skillManager;

    protected GeomBase _collisionShape;

    public int level = 1;

    //五个顶点 中心 上左下右
    protected List<Vector2D> _vertexPosion = new List<Vector2D> {
        Vector2D.createVector(),
        Vector2D.createVector2(ConfigConstant.SHIP_RADIUS, 0),
        Vector2D.createVector2(ConfigConstant.SHIP_RADIUS, Math.PI / 2),//L
        Vector2D.createVector2(ConfigConstant.SHIP_RADIUS, -Math.PI / 2),//R
        Vector2D.createVector2(ConfigConstant.SHIP_RADIUS, Math.PI)
    };

    public PersonEntity (Map map, int netId = -1):base(map, netId) {

    }
    

    public override GeomBase shape {
        get {
            if(this._propertyChange) this.changeProperty();
            return base.shape;
        }
    }

    public GeomBase collisionShape {
        get {
            if(this._propertyChange) this.changeProperty();
            return this._collisionShape;
        }
    }

    

    public virtual void addBuff(Buff buff) {
        if(FightMain.checkError && this._buffs.Contains(buff)) {
            Debug.Log("重复添加buff？？？");
        }
        this._buffs.Add(buff);
        //没一个buff 有一个自己的组。 只是用来查找与 移除用的，所以不用排序，不需要前后端对齐。
        if(!this._buffMap.ContainsKey(buff.id)) this._buffMap[buff.id] = new List<Buff>();
        this._buffMap[buff.id].Add(buff);
        this._propertyChange = true;
    }

    public virtual void removeBuff(Buff buff) {
        if(FightMain.checkError && !this._buffs.Contains(buff)) {
            Debug.Log("移除不掉？？？");
        }
        this._buffs.Remove(buff);
        this._buffMap[buff.id].Remove(buff);
        if(0 == this._buffMap[buff.id].Count) this._buffMap.Remove(buff.id);
        this._propertyChange = true;
    }

    public List<Buff> getBuffs(string type, int sort = -1) {
        List<Buff> buffs = this._buffs.FindAll((Buff buff) => { return buff.buffType == type; });
        switch(sort) {
            case 0:
                buffs.Sort((Buff b1, Buff b2) => { return b2.cd - b1.cd; });
                break;
        }
        return buffs;
    }
	///取得某类Buff当前存在的最长时间ms Biggo添加
	public int getBuffCD(string type) {
		List<Buff> buffs = getBuffs (type, 0);
		return buffs.Count > 0 ? buffs [0].cd : 0;
	}
	/// Biggo添加
	public List<Buff> buffs {
		get{ return this._buffs;}
	}

    public bool hasBuff(string name) {
        return this.getProperty(name) > 0;
    }

    public int hurtShield(int damage) {
        List<Buff> buffs = this.getBuffs(ConfigConstant.BUFF_SHIELD);

        for(int i = 0, len = buffs.Count; i < len; i++) {            
            Buff buff = buffs[i];
            int value = Math.Min((int)(buff.value), damage);
            buff.value -= value;
            damage -= value;
            if(0 == buff.value) buff.clear();
            if(damage == 0) {
                this.dispatchEventWith(EventConstant.SHIELD_HURT);
                continue;
            }
        }
        return damage;
    }

    public void addAction(ActionBase action) {
        this.skillManager.addAction(action);
        action.start();
    }
    


    /**
	 * 如果有属性改变了！ 那么 下次再重新取出来人物属性 就需要重新计算了！
	 */
    protected virtual void changeProperty() {
        this._helpProperty.Clear();
        foreach(string key in this._property.Keys) {
            this.setProperty(key, this._property[key]);
        }        
        this._buffs.Sort((Buff b1, Buff b2) => {
            if(b1.operation != b2.operation) return b1.operation - b2.operation;
            if(b1.value < b2.value) return -1;
            if(b1.value > b2.value) return 1;
            return b1.netId - b2.netId;
        });

        for(int i = 0, len = this._buffs.Count; i < len; i++) {
            this._buffs[i].decortation(this._helpProperty);
        }
        this._propertyChange = false;
    }
    

    public virtual void setProperty(string type, double value) {
        this._helpProperty[type] = value;
        //半径什么的 还需要把机体放大！
        switch(type) {
            case ConfigConstant.PROPERTY_RADIUS:
                this._shape.radius = (int)value;
                this._scale = value / ConfigConstant.SHIP_RADIUS;
                //四周的顶点也进行偏移。
                for(int i = 1, len = this._vertexPosion.Count; i < len; i++) {
                    this._vertexPosion[i].length = value;
                }
                break;
            case ConfigConstant.PROPERTY_COLLISION_RADIUS:
                this._collisionShape.radius = (int)value;
                break;
        }
    }

    public double getProperty(string type, bool decoration = true) {
        if(this._propertyChange) this.changeProperty();
        return this._helpProperty.ContainsKey(type) ? this._helpProperty[type] : 0;
    }


    ///治疗血量百分比
    public void cureHpPer(double per) {
        this.cureHp(Mathf.CeilToInt(Convert.ToSingle(per * this.getProperty(ConfigConstant.PROPERTY_HP))));
    }
    ///治疗血量
    public void cureHp(int value) {
        value = (int)( value * this.getProperty(ConfigConstant.PROPERTY_CURE_RATE) );
        this.changeHp(value);
    }

    public virtual void changeHp(int value) {
        this.hp = (int)Math2.range(this.hp + value, this.getProperty(ConfigConstant.PROPERTY_HP), 0);
        this._dead = this.hp <= 0;
    }

    public void removeAllBuff() {
        Utils.clearList(this._buffs);
    }

    public virtual void beHit(BulletEntity bullet, double damageRate = 1) {
        /***********************击退**********************************/
        Vector2D backV2d;
        if(bullet.velocity.length < 2) {//子弹速度很低时，按相对位置击退
            backV2d = Collision.realPosition(bullet.position, this._position, this._map.mapData).deltaPos;
        } else {
            backV2d = bullet.velocity.clone();
        }
        backV2d.length = bullet.backForce;
        this.addForce(backV2d);
        /**************************增加buff********************************/

        if(bullet.data.ContainsKey("buff")) {
            Dictionary<string, object> buffs = (Dictionary<string, object>)bullet.data["buff"];
            string id = buffs["id"].ToString();
            foreach(string key in buffs.Keys) {
                if(key == "id") continue;
                Buff buff = new Buff(this.map).initBuff(id, key, this, bullet.owner, (Dictionary<string, object>)buffs[key]);
                if(key == ConfigConstant.BUFF_STUN) buff.totalTime += bullet.stun;
            }
        }

        this.hurt(bullet, damageRate);
    }

    //分摊伤害
    public virtual void hurt(BulletEntity bullet, double damageRate = 1) {
        int damage = this.hurtShield((int)(bullet.getDamage(this) * damageRate));
        double rate = ( 1 - this.getProperty(ConfigConstant.PROPERTY_DEF_RATE) ) * this.getProperty(ConfigConstant.PROPERTY_BEAR_RATE);
        damage = Mathf.CeilToInt(Convert.ToSingle(damage * rate));
        this.changeHp(-damage);
    }


    protected override double changeDir() {
        //一点一点转向。
        if(this._joystickForce.isZero) return 0;

        double max = this.getProperty(ConfigConstant.PROPERTY_ASP);
        double angle2 = this._joystickForce.angle;
        angle2 = Math2.range(Math2.deltaAngle(this.angle, angle2) * this.getProperty(ConfigConstant.PROPERTY_ASP_RATE), max, -max);
        this.angle += angle2;
        return angle2;
    }

    public override void update () {
        MediatorSystem.timeStart("SinglePlayerUpdate");
        base.update();
        MediatorSystem.getRunTime("SinglePlayerUpdate");
        MediatorSystem.timeStart("actionUpdate");
		this.actionManager.update ();
		MediatorSystem.getRunTime("actionUpdate");
        for(int i = this._buffs.Count - 1; i >= 0; i--) {
            this._buffs[i].update();
        }
    }


    public void setJoystick(Vector2D v) {
        this._joystickForce.copy(v);
    }

    public Vector2D joystickForce { get { return this._joystickForce; } }

    

    protected override void move() {
        this._steeringForce.copy(this._joystickForce);
        double speedScale = this.getProperty(ConfigConstant.PROPERTY_SPEED_SCALE);
        double min = this.getProperty(ConfigConstant.PROPERTY_JOYSTICK_MIN) * speedScale;
        double max = this.getProperty(ConfigConstant.PROPERTY_JOYSTICK_MAX) * speedScale;
        this._steeringForce.length = Utils.transitionNum(min, max, this._joystickForce.length);
        if(this._joystickForce.isZero) {
            this._steeringForce.angle = this._angle;
        }
        this.addForce(this._steeringForce.clone().subtract(this._velocity).multiply(this.getProperty(ConfigConstant.PROPERTY_SP_RATE)));
    }

    
    public override void applyPosition() {
        base.applyPosition();
    }




    public override void initConfig(Dictionary<string, object> data = null) {

        Dictionary<string, object> personConfig = (Dictionary<string, object>)data["property"];

        //TODO:这里根据成长等级 计算！
        personConfig = (Dictionary<string, object>)Utils.clone(personConfig);

        foreach(string property in ConfigConstant.PROPERTY_ARRAY) {
            if(personConfig.ContainsKey(property)) {
                double value = -1;
                switch(property) {
                    case ConfigConstant.PROPERTY_ASP:
                        value = Math2.angleToRadian(Convert.ToDouble(personConfig[property]));
                        break;
                    default:
                        value = Convert.ToDouble(personConfig[property]);
                        break;
                }
                this._helpProperty[property] = this._property[property] = value;
            }
        }

        this.hp = (int)( this._property[ConfigConstant.PROPERTY_HP] );

        this._shape = Collision.createGeom(this, ( new object[] { personConfig[ConfigConstant.PROPERTY_RADIUS] } ));
        this._collisionShape = Collision.createGeom(this, ( new object[] { personConfig[ConfigConstant.PROPERTY_COLLISION_RADIUS] } ));


        base.initConfig(data);
        this.actionManager = new ActionManager(this._map);
        this.skillManager = new SkillManager(this._map);
        this.actionManager.addAction(this.skillManager);
        this.actionManager.start();
    }

    public override Dictionary<string, object> getData () {
		Dictionary<string, object> data = base.getData ();
        /***************************************************************/
        //把double 转成object类型的。
        Dictionary<string, object> property = new Dictionary<string, object>();
        foreach(string key in this._property.Keys) {
            property[key] = this._property[key];
        }
        data["property"] = property;
        /***************************************************************/
        property = new Dictionary<string, object>();
        foreach(string key in this._helpProperty.Keys) {
            property[key] = this._helpProperty[key];
        }
        data["helpProperty"] = property;
        /***************************************************************/
        data["propertyChange"] = this._propertyChange;
        data["actionManager"] = this.actionManager.netId;
        data["force"] = this._force.getData();
        data["hp"] = this.hp;
        data["joystickForce"] = this._joystickForce.getData();
        data["steeringForce"] = this._steeringForce.getData();
        data["dead"] = this._dead;
        data["level"] = this.level;
        data["buffs"] = this._buffs.ConvertAll<object>((Buff b) => { return b.netId; }).ToArray();

        data["skillManager"] = this.skillManager.netId;
        data["collisionShape"] = this._collisionShape.netId;

        return data;
	}

    public Vector2D getVertexPosition(int type) { return this._vertexPosion[type]; }


    public override void setData (Dictionary<string, object> data) {

        Dictionary<string, object> property = (Dictionary<string, object>)data["property"];
        /***************************************************************/
        foreach(string key in property.Keys) {
            this._property[key] = Convert.ToDouble(property[key]);
        }
        /***************************************************************/
        property = (Dictionary<string, object>)data["helpProperty"];
        foreach(string key in property.Keys) {
            this._helpProperty[key] = Convert.ToDouble(property[key]);
        }
        /***************************************************************/
        this._propertyChange = Convert.ToBoolean(data["propertyChange"]);
        this.actionManager = (ActionManager)this._map.getNetObject((int)(data["actionManager"]));
        this._force = Vector2D.createVector4((Dictionary<string, object>)data["force"]);
        this.hp = (int)(data["hp"]);
        this.level = (int)( data["level"] );
        this._joystickForce = Vector2D.createVector4((Dictionary<string, object>)data["joystickForce"]);
        this._steeringForce = Vector2D.createVector4((Dictionary<string, object>)data["steeringForce"]);
        this._dead = Convert.ToBoolean(data["dead"]);
        this._buffs = new List<object>((object[])data["buffs"]).ConvertAll<Buff>((object netId) => {
            return (Buff)this._map.getNetObject((int)(netId));
        });
        this.skillManager = (SkillManager)this._map.getNetObject((int)(data["skillManager"]));
        this._collisionShape = (GeomBase)this._map.getNetObject((int)(data["collisionShape"]));
        //重新组织buffMap
        this._buffMap.Clear();
        for(int i = 0, len = this._buffs.Count; i < len; i++) {
            string buffId = this._buffs[i].id;
            if(!this._buffMap.ContainsKey(buffId)) this._buffMap[buffId] = new List<Buff>();
            this._buffMap[buffId].Add(this._buffs[i]);
        }

        base.setData (data);
	}
    

    public override void clear() {
        this._dead = true;
        this._property = null;
        this._helpProperty = null;
        Utils.clearObject(this._collisionShape);
        Utils.clearObject(this.skillManager);
        Utils.clearObject(this.actionManager);
        Utils.clearObject(this._force);
        Utils.clearObject(this._joystickForce);
        Utils.clearObject(this._steeringForce);
        Utils.clearList(this._buffs);
        this._collisionShape = null;
        this.skillManager = null;
        this.actionManager = null;
        this._force = null;
        this._joystickForce = null;
        this._steeringForce = null;
        base.clear();
    }
}
