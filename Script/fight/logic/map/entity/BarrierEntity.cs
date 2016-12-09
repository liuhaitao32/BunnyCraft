using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class BarrierEntity : FightEntity {

	///与子弹检测的关系 0穿透 1弹开 2消灭
	private int _checkBulletType;
    ///障碍自身被弹开的倍数
    private double _bounceSelfRate;
    ///障碍弹开person的倍数
    private double _bouncePersonRate;
    ///强行挤开力
    private double _pushValue;    

    public BarrierEntity(Map map, int netId = -1):base(map, netId) {

    }

    public override void initConfig(Dictionary<string, object> data = null) {
        //这个代表当前障碍物没法生成。 一般情况不会有。 有的话那就是后端配的太多障碍物了！
        if(!this._alived) {
            this.clear();
            return;
        }
        this._checkBulletType  = (int)data["checkBulletType"];
        this._bounceSelfRate   = Convert.ToDouble(data["bounceSelfRate"]);
        this._bouncePersonRate = Convert.ToDouble(data["bouncePersonRate"]);
        this._pushValue        = Convert.ToDouble(data["pushValue"]);
        this._shape            = Collision.createGeom(this, new object[] { (int)( data["radius"] ) });
        base.initConfig(data);
        this.generateBirthGrids();
    }

    public Vector2D createBirthPosition() {
        List<Vector2D> range = new List<Vector2D>() {
            Vector2D.createVector(this._map.mapData.gridYAreaLines[1], this._map.mapData.gridYAreaLines[3]),
            Vector2D.createVector(this._map.mapData.rowNum - this._map.mapData.gridYAreaLines[3] - 1, this._map.mapData.rowNum - this._map.mapData.gridYAreaLines[1] - 1)
        };

        Grid grid = this._map.birthGrids.getRandomBirthGrid(range, new List<int> {ConfigConstant.ENTITY_BARRIER});
        Utils.clearList(range);
        if(null == grid) {
            Debug.Log("障碍物已经没有地方了？");
            this._alived = false;
            return this._position;
        }
        return grid.randomPosition;
    }

    public override void update () {
        base.update();
        MediatorSystem.timeStart("barrierSingle");
        this.checkPerson ();
        if(this._bounceSelfRate > 0) {
            this.checkBullet();
            this.bounceBack();
            this.applyPosition();
        }
        MediatorSystem.getRunTime("barrierSingle");
    }
	///判断和玩家的碰撞
	public void checkPerson() {
        //TODO：这里检测了两次碰撞。 只用一次其实就够了。 filter传true。
        List<FightEntity> personList = this._map.getFightEntitysByRange(this._shape, new List<int> { ConfigConstant.ENTITY_PLAYER, ConfigConstant.ENTITY_CALL}, null, -1);//把敌我双方的所有人取出来，不需要排序。
        //personList = this._map.players.ConvertAll<FightEntity>((a)=> { return a; });
		for (int i = 0, len = personList.Count; i < len; i++) {
            PersonEntity person = (PersonEntity)personList[i];
			if (!person.alived) continue;
            Vector2D deltaPosition = person.findDelta;
            double dist = person.findDist;
            //把玩家向外推，越中心越强
            double rate = 1 - Math2.range(dist / (person.shape.radius + this._shape.radius), 1, 0);
			//玩家受到的推力，玩家越大越难被推动
			double value = (dist * rate * 0.4f + this._pushValue) / person.scale;
            Vector2D forcePersonPush = Vector2D.createVector2(value, deltaPosition.angle);
			person.addForce (forcePersonPush);

			if (this._bounceSelfRate > 0) {
				//相对速度
				Vector2D deltaVelocity = person.velocity.clone().subtract(this._velocity);
                //相对速度（投影到位置向量，为负时向内冲）
                double velocityValue = deltaVelocity.projectionOn(deltaPosition);
				if (velocityValue < 0) {
					//玩家向内冲，弹开，看速度冲量
					Vector2D projectionVelocity = deltaVelocity.projectionVector (deltaPosition);
                    Vector2D forcePerson = projectionVelocity.clone().multiply(this._bouncePersonRate / person.scale);
                    Vector2D forceSelf = projectionVelocity.clone().multiply(-this._bounceSelfRate * person.scale);

                    this.addForce(forceSelf);
                    person.addForce(forcePerson);

                    projectionVelocity.clear();
                    forcePerson.clear();
                    forceSelf.clear();
                }
			}
            //碰到了 给人上buff。
            if(this._data.ContainsKey("buff")) {
                Dictionary<string, object> buffs = (Dictionary<string, object>)this._data["buff"];
                string id = buffs["id"].ToString();
                foreach(string key in buffs.Keys) {
                    if(key == "id") continue;
                    new Buff(this._map).initBuff(id, key, person, this, (Dictionary<string, object>)buffs[key]);
                }
            }

            this.dispatchEventWith(EventConstant.HIT);
		}
	}
	///判断和子弹的碰撞
	public void checkBullet() {
        if(this._checkBulletType == 0) return;
        //TODO：这里检测了两次碰撞。 只用一次其实就够了。 filter传true。
		//Biggo修改
		List<FightEntity> bullets = this._map.getFightEntitysByRange(this._shape, new List<int> { ConfigConstant.ENTITY_BULLET }, (FightEntity entity1,FightEntity entity2)=> {
			return ((BulletEntity)entity2).checkBarrier;
        }, -1);
        bullets = new List<FightEntity>(bullets);//这个要副本的！因为在bomb的时候 会改变这个通用的数组的长度。
        for(int i = 0, len = bullets.Count; i < len; i++) {
            BulletEntity bullet = (BulletEntity)bullets[i];
            Vector2D deltaV2d = bullet.findDelta.clone();
            double dist = bullet.findDist;
            double radius = bullet.shape.radius + this._shape.radius;
            //障碍物到命中点的向量
            Vector2D hitDelta = bullet.getHitPos(this);
            double angleHitDelta = hitDelta.angle;
            double angleBulletBounce = bullet.velocity.angle + Math.PI;
            //子弹回弹方向如果与命中向量夹角大于90，则说明子弹本身就是向外的，不用处理
            double angleDelta = Math2.deltaAngle(angleHitDelta, angleBulletBounce);
            if(Math.Abs(angleDelta) < Math.PI / 2) {
                if(this._checkBulletType == 1) {
                    //新子弹的速度方向改变
                    bullet.velocity.angle = bullet.angle = angleHitDelta - angleDelta * 0.5f;
                } else if(this._checkBulletType == 2) {
                    //引爆消亡逻辑
                    bullet.bomb();
                }

                deltaV2d.length = bullet.backForce * this._bounceSelfRate * 0.6f;
                addForce(deltaV2d);
                this.dispatchEventWith(EventConstant.HIT);
            }
        }
    }

    public override void applyPosition() {
        base.applyPosition();
    }

    ///如果是可弹射移动型，受到力，归位
    public void bounceBack () {        
		Vector2D tempForce = Collision.realPosition(this._position, this.birthPosition, this._map.mapData).deltaPos.clone();
		this.addForce (tempForce.multiply(0.05f));
        tempForce.clear();
	}

    protected override void velocityToPosition() {
        base.velocityToPosition();
        this._velocity.multiply(0.64f);
    }

    public override int type { get { return ConfigConstant.ENTITY_BARRIER; } }


    

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this._checkBulletType = (int)this._data["checkBulletType"];
        this._bounceSelfRate = Convert.ToDouble(this._data["bounceSelfRate"]);
        this._bouncePersonRate = Convert.ToDouble(this._data["bouncePersonRate"]);
        this._pushValue = Convert.ToDouble(this._data["pushValue"]);
        this.generateBirthGrids();
    }
}
