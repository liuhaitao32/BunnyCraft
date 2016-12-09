using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BulletEntity : FightEntity {

	public PersonEntity owner;

    protected int _speed;
	private int _accSpeed;

    public int atk;

    //爆炸半径
    public int _aoe;
	//击退力
	public int backForce;

    public int _lifeTime = 1000;

    /**
     * 攻击的范围。
     */
    public Dictionary<string, object> seekData = null;

    public PersonEntity seekTarget;

    /**
     * 一开始查找到的敌人。 用于放到目标脚下。
     */
    public PersonEntity lockTarget;

    /**
     * 搜寻的范围。
     */
    public GeomBase seekRange;

    /**
     * 预热时间。
     */
    private int _preTime = 0;

    /**
     * 减免需要乘以的数值。同类型的子弹 打第二个人的时候要减少伤害。
     */
    public BulletShallow shallow;

    public bool checkBarrier;
    

    private bool isFree;//是否一开始锁定敌人。 如果锁定敌人的话 那就直接检测人与此子弹是否碰撞。
    private bool _repeat;




    private IntervalAction _hitAction;

    private IntervalAction _bombAction;

    public List<PersonEntity> hitList = new List<PersonEntity>();

    ///最大爆炸个数
    public int bangCount = 1;
    ///最大爆炸半径
    public int bangRadiusMax;
    ///最大距离下！！爆炸伤害比例
    public double bangRateMax = 1;
    ///最小爆炸半径
    public int bangRadiusMin = 1;
    ///最小距离下！！爆炸伤害比例
    public double bangRateMin = 1;

    ///增幅次数
	public int zoomCount;
    ///增幅缩放
    public double zoomScale;
    ///增幅半径
    public int zoomRadius;
    ///增幅伤害
    public int zoomDamage;
    ///增幅眩晕时间
    public int zoomStun;
    /// 当前累计的眩晕时间。
    public int stun;

    //——————————————————————————————————————以下是方法——————————————————————————————————————————

    public BulletEntity(Map map, int netId = -1):base(map, netId) {
        
    }

    public override void initConfig(Dictionary<string, object> data = null) {
        if(data.ContainsKey("speed"))         this._speed           = (int)(data["speed"]);
        if(data.ContainsKey("accSpeed"))      this._accSpeed        = (int)(data["accSpeed"]);
        if(data.ContainsKey("atk"))           this.atk              = (int)(data["atk"]);
        if(data.ContainsKey("aoe"))           this._aoe             = (int)( data["aoe"]);
        if(data.ContainsKey("lifeTime"))      this._lifeTime        = (int)( data["lifeTime"]);
        if(data.ContainsKey("angle"))         this._angle           = Math2.angleToRadian(Convert.ToSingle(data["angle"]));
        if(data.ContainsKey("seek"))          this.seekData         = (Dictionary<string, object>)data["seek"];
        if(data.ContainsKey("backForce"))     this.backForce        = (int)(data["backForce"] );
        if(data.ContainsKey("preTime"))       this._preTime         = (int)( data["preTime"] );
        //bang伤害。
        if(data.ContainsKey("bangCount"))     this.bangCount        = (int)( data["bangCount"] );
        if(data.ContainsKey("bangRadiusMax")) this.bangRadiusMax    = (int)( data["bangRadiusMax"] );
        if(data.ContainsKey("bangRateMax"))   this.bangRateMax      = Convert.ToDouble(data["bangRateMax"]);
        if(data.ContainsKey("bangRadiusMin")) this.bangRadiusMin    = (int)( data["bangRadiusMin"] );
        if(data.ContainsKey("bangRateMin"))   this.bangRateMin      = Convert.ToDouble( data["bangRateMin"] );
        //zoom增幅。
        if(data.ContainsKey("zoomCount"))     this.zoomCount        = (int)( data["zoomCount"] );
        if(data.ContainsKey("zoomRadius"))    this.zoomRadius       = (int)( data["zoomRadius"] );
        if(data.ContainsKey("zoomDamage"))    this.zoomDamage       = (int)( data["zoomDamage"] );
        if(data.ContainsKey("zoomStun"))      this.zoomStun         = (int)( data["zoomStun"] );

        
        //检查障碍物。
        this.checkBarrier = Utils.equal(data, "checkBarrier", 1);
        this._repeat = Utils.equal(data, "repeat", 1);

        if(null != seekData) {
            this.seekRange = Collision.createGeom(this, (object[])this.seekData["range"], (object[])this.seekData["offset"]);
        }
        this.atk = Mathf.CeilToInt(Convert.ToSingle(this.owner.getProperty(ConfigConstant.PROPERTY_ATK_RATE) * this.atk));

        this._shape = Collision.createGeom(this, (object[])data["range"]);

        if(this.zoomRadius != 0) this.zoomScale = Convert.ToDouble(this.zoomRadius) / this._shape.radius;

        if(data.ContainsKey("offset")) {
            Vector2D offset = Vector2D.createVector3((object[])data["offset"]);
            offset.angle += this.owner.angle;
            this._position.add(offset);
            offset.clear();
        }

        if(data.ContainsKey("reactionForce")) {
            this.owner.velocity.length = 1;
            this.owner.addForce(Vector2D.createVector2(Convert.ToSingle(data["reactionForce"]), Math.PI + this.owner.angle));
        }
        this.id = "";
        this.ownerPlayer = this.owner.ownerPlayer;

        this.isFree = !Utils.equal(data, "posTarget", 1);
        PersonEntity target = this.isFree ? this.owner : this.lockTarget;
        this._angle += target.angle;
        this._position.add(target.position);
        //根据飞船大小 偏移。
        if(data.ContainsKey("offsetType") && (int)data["offsetType"] != 0) {
            Vector2D v = target.getVertexPosition((int)data["offsetType"]).clone();
            v.angle += target.angle;
            this._position.add(v);
            v.clear();
        }

        this._velocity.length = this._speed;
        this._velocity.angle = this._angle;

        this.changeDir();
        //TODO:优化修改。
        this.applyPosition();
        //hitCd
        if(data.ContainsKey("hitCDMax")) {
            if(0 != (int)data["hitCDMax"]) {
                this._hitAction = new IntervalAction(this._map).init((int)data["hitCDMax"], this._lifeTime, this.checkHit);
            }            
        } else {
            this._hitAction = new IntervalAction(this._map).init(66 * 2, this._lifeTime, this.checkHit);
        }

        if(data.ContainsKey("bangCDMax")) {
            this._bombAction = new IntervalAction(this._map).init((int)data["bangCDMax"], this._lifeTime, this.bomb);
        }


        base.initConfig(data);

    }

    private void checkHit() {
        if(!this.enabled || !this._alived) return;
        PersonEntity person = null;
        if(this.isFree) {
            List<PersonEntity> persons = this.map.getPerson(this.shape);

            for(int i = 0, len = persons.Count; i < len; i++) {
                person = persons[i];
                if(!this.hitList.Contains(person)) {
                    break;
                }
            }
        } else {//一开始放到人物脚下 直接判定命中！ 不需要检测是否判定。
            person = this.lockTarget;
        }
        if(null != person) {
            this.hitPerson(person);
        }        
    }

    protected virtual void hitPerson(PersonEntity person) {
        person.beHit(this, 1);
        if(null != this.shallow) this.shallow.addPerson(person);
        if(!this._repeat) this.hitList.Add(person);
        this.bomb();
    }

    protected override double changeDir() {
        this.angle = this._velocity.angle;
        return 0;
    }

    public override void update() {
        if(this._lifeTime <= 0) {
            this.bomb();
            return;
        }
        this._lifeTime -= ConfigConstant.MAP_ACT_TIME_S;
        this._preTime -= ConfigConstant.MAP_ACT_TIME_S;

        if(0 != this._accSpeed) this.accMove();
        if(this.enabled)        this.guidePlayer();
        base.update();
        if(this.enabled && null != this._hitAction) this._hitAction.update();
        if(this.enabled && null != this._bombAction) this._bombAction.update();
        if(this.zoomCount != 0) this.zoomBullet();
        //因为子弹没有受力没必要通过力 然后 再改变位置。
        if(this._alived) this.applyPosition();

    }

    private void zoomBullet() {
        this.atk += this.zoomDamage;
        this._shape.radius += this.zoomRadius;
        if(this.bangRadiusMax > 0) {
            this.bangRadiusMax += this.zoomRadius;
            this.bangRadiusMin += this.zoomRadius;
        }

        this.stun += zoomStun;
        this._scale += this.zoomScale;
        zoomCount--;
    }

    protected void accMove() {
        this._speed += this._accSpeed;
        this._velocity.length = this._speed;
    }

    public int getDamage(PersonEntity person) {
        if(null != this.shallow) return shallow.getAtk(person, atk);
        return this.atk;
    }

    private bool enabled { get { return this._preTime <= 0; } }

    //加速度和追踪
    public void guidePlayer() {
        if(this.enabled && null != this.seekData) {
            //脱离追踪范围。
            if(null != this.seekTarget && (!this.seekTarget.alived || !Collision.checkCollision(this.seekRange, this.seekTarget.shape, this._map.mapData).isHit)) {
                this.seekTarget = null;
            }
            //没有人的时候查询范围找敌人。
            if(null == this.seekTarget) {
				List<PersonEntity> players = this.map.getPerson (this.seekRange, 1, 0);
                if(players.Count > 0) { this.seekTarget = players[0]; }
            }
            //有敌人开始追踪力
            if(null != this.seekTarget) {
                CollisionInfo info = Collision.realPosition(this.position, seekTarget.position, this._map.mapData);
                Vector2D logicDelta = info.deltaPos;
                logicDelta.length = Convert.ToSingle(this.seekData["guideForce"]);
                this._velocity.add(logicDelta);
                this._velocity.truncate(this._speed);
            }
        }
    }
    
	public override Vector2D limitPosition (Vector2D pos) {
		while(pos.x < 0) pos.x += this._map.mapData.width;
		pos.x = pos.x % this._map.mapData.width;
		return pos;
	}

    public virtual void bomb() {
        if(!this.enabled || !this._alived) return;
        if(this.bangRadiusMax > 0) {
            //爆炸范围只承受部分伤害，排除已经Hit伤害过的
            int radius = this._shape.radius;
            this._shape.radius = this.bangRadiusMax;
            //TODO:这里没有指定敌方我方。
            List<PersonEntity> personList = this._map.getPerson(this._shape);           

            for(int i = 0, len = personList.Count; i < len; i++) {                
                PersonEntity person = personList[i];
                if(!this.hitList.Contains(person)) {
                    double bangRate;
                    if(this.bangRateMin == this.bangRateMax) {
                        bangRate = this.bangRateMin;
                    } else {
                        double dist = Collision.realPosition(this._position, person.position, this._map.mapData).deltaPos.length - person.shape.radius;
                        if(dist <= this.bangRadiusMin) {
                            bangRate = this.bangRateMin;
                        } else {
                            bangRate = ( dist - this.bangRadiusMin ) / ( this.bangRadiusMax - this.bangRadiusMin ) * ( this.bangRateMax - this.bangRateMin ) + this.bangRateMin;
                        }
                    }

                    //仅爆炸的子弹，替代命中检测方案
                    if(!this._repeat && this._hitAction == null) {
                        this.hitList.Add(person);
                    }
                    person.beHit(this, bangRate);
                }
                
            }
            this._shape.radius = radius;
        }
        //这里要写碰撞的次数。
        if(this._lifeTime <= 0) {
            this.clear();
        } else {
            this.bangCount--;
            if(this.bangCount == 0) {
                this.clear();
            }
        }        
    }
    

    public Vector2D getHitPos(FightEntity fightEntity) {
        //子弹和玩家相对速度
        Vector2D deltaV = this._velocity.clone().subtract(fightEntity.velocity);
        //玩家到命中点的向量
        Vector2D deltaP = Collision.realPosition(fightEntity.position, this._position, this._map.mapData).deltaPos;
        double a, bSQ, c, l;
        double r = fightEntity.shape.radius + this._shape.radius;
        double rSQ = r * r;
        a = deltaP.projectionOn(deltaV);
        bSQ = deltaP.lengthSQ - a * a;

        if(rSQ >= bSQ) {
            //体内命中，按入射位置判定
            c = Math.Sqrt(r * r - bSQ);
            l = a + c;
            //修改deltaV含义为子弹位置指向命中点向量
            deltaV.truncate(l);
            //修改deltaP含义为玩家到命中点向量
            deltaP.subtract(deltaV);
        }
        return deltaP;
    }

    public override int type { get { return ConfigConstant.ENTITY_BULLET; } }

    /**
     * 获取最大的半径。
     */
    public int radiusMax { get { return Math.Max(this.shape.radius, this._aoe); } }


    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        if(!this.owner.cleared) {
            data["owner"] = this.owner.netId;
        }        
        data["speed"] = this._speed;
        data["accSpeed"] = this._accSpeed;
        data["atk"] = this.atk;
        data["aoe"] = this._aoe;
        data["backForce"] = this.backForce;
        data["lifeTime"] = this._lifeTime;
        if(null != this.seekData) {
            data["seekData"] = this.seekData;
        }
        if(null != this.seekTarget) {
            data["seekTarget"] = this.seekTarget.netId;
        }
        if(null != this.lockTarget) {
            data["lockTarget"] = this.lockTarget.netId;
        }
        if(null != this.seekRange) {
            data["seekRange"] = this.seekRange.netId;
        }
        data["preTime"] = this._preTime;
        if(null != this.shallow) {
            data["shallow"] = this.shallow.netId;
        }


        if(null != this._hitAction) {
            data["hitAction"] = this._hitAction.netId;
        }
        if(null != this._bombAction) {
            data["hitAction"] = this._bombAction.netId;
        }
        data["hitList"] = this.hitList.ToArray();
        data["bangCount"] = this.bangCount;
        data["bangRadiusMax"] = this.bangRadiusMax;
        data["bangRadiusMin"] = this.bangRadiusMin;
        data["zoomCount"] = this.zoomCount;
        data["zoomRadius"] = this.zoomRadius;
        data["zoomScale"] = this.zoomScale;
        data["zoomDamage"] = this.zoomDamage;
        data["zoomStun"] = this.zoomStun;
        data["stun"] = this.stun;
        return data;
    }
    

    public override void setData(Dictionary<string, object> data) {
        //this._lifeTime = Convert.ToSingle(data["lifeTime"]);
        if(data.ContainsKey("owner")) {
            this.owner = (PersonEntity)this._map.getNetObject((int)( data["owner"] ));
        }
        
        this._speed = (int)(data["speed"]);
        this._accSpeed = (int)( data["accSpeed"]);
        this.atk = (int)(data["atk"]);
        this._aoe = (int)( data["aoe"]);

        this.backForce = (int)( data["backForce"]);
        this._lifeTime = (int)( data["lifeTime"]);
        this._preTime = (int)(data["preTime"]);
        if(data.ContainsKey("seekData")) {
            this.seekData = (Dictionary<string, object>)data["seekData"];
        }
        if(data.ContainsKey("seekTarget")) {
            this.seekTarget = (PersonEntity)this._map.getNetObject((int)(data["seekTarget"]));
        }
        if(data.ContainsKey("lockTarget")) {
            this.lockTarget = (PersonEntity)this._map.getNetObject((int)( data["lockTarget"] ));
        }
        if(data.ContainsKey("seekRange")) {
            this.seekRange = (GeomBase)this._map.getNetObject((int)(data["seekRange"]));
        }
        if(data.ContainsKey("shallow")) {
            this.shallow = (BulletShallow)this._map.getNetObject((int)(data["shallow"]));
        }
        

        if(data.ContainsKey("hitAction")) {
            this._hitAction = (IntervalAction)this._map.getNetObject((int)( data["hitAction"] ));
            this._hitAction.intervalHandler = this.checkHit;
        }
        if(data.ContainsKey("bombAction")) {
            this._bombAction = (IntervalAction)this._map.getNetObject((int)( data["bombAction"] ));
            this._bombAction.intervalHandler = this.bomb;
        }
        this.hitList = new List<object>((object[])data["hitList"]).ConvertAll<PersonEntity>((id) => { return (PersonEntity)this._map.getNetObject((int)id); });
        this.bangCount = (int)data["bangCount"];

        this.bangRadiusMax = (int)data["bangRadiusMax"];
        this.bangRadiusMin = (int)data["bangRadiusMin"];
        this.zoomCount = (int)data["zoomCount"];
        this.zoomRadius = (int)data["zoomRadius"];
        this.zoomScale = Convert.ToDouble(data["zoomScale"]);
        this.zoomDamage = (int)data["zoomDamage"];
        this.zoomStun = (int)data["zoomStun"];
        this.stun = (int)data["stun"];

        base.setData(data);


        this.bangRateMax = Convert.ToDouble(this._data["bangRateMax"]);
        this.bangRateMin = Convert.ToDouble(this._data["bangRateMin"]);


        this.checkBarrier = Utils.equal(this._data, "checkBarrier", 1);
        this.isFree = !Utils.equal(this._data, "posTarget", 1);
    }
    
    

    public override void clear() {
        base.clear();
        this.shallow = null;
        Utils.clearObject(this.seekRange);
        this.seekRange = null;
        this.lockTarget = null;
        this.seekTarget = null;
        this.seekData = null;
        this.owner = null;
    }
}

