using System;
using System.Collections.Generic;
using UnityEngine;

public class RadishEntity : FightEntity {

    public TimeAction reliveTime;

    private GeomBase _seek;

    public int teamIndex = -1;

    private List<Buff> _buffs;

    public RadishEntity(Map map, int netId = -1):base(map, netId) {
        
    }

    public override void initConfig(Dictionary<string, object> data = null) {
        this.id = "radish";
        this._shape = Collision.createGeom(this, new object[] { (int)( data["radius"] ) });

        this.birthPosition = this._position = Vector2D.createVector(0, this._map.mapData.heightHalf);
        base.initConfig(data);
        this._seek = Collision.createGeom(this, new object[] { ConfigConstant.ATTRACT });
        this.reliveTime = new TimeAction(this._map).init(ConfigConstant.RADISH_RADISH_START_TIME, this.reliveHandler);
        this._buffs = new List<Buff>();
    }

    public void reliveHandler() {
        this.dispatchEventWith(EventConstant.RELIVE);
    }

    public override void update () {
        base.update();
        if(this._alived) {
            if(this.reliveTime.isFinish) {
                this.guidePlayer();
                this.checkBarrier();
            } else {
                //TODO:这里没有给受力？
                this.position = this.limitPosition(this._position.add(this._velocity));
                this.reliveTime.update();
            }
            
        } else {
            //被玩家获得
            this._position.copy(this.ownerPlayer.position);
            this.position = this._position;
        }
        this.setNewViewData();
    }

    public void checkBarrier() {
        //相对位置
        List<FightEntity> entitys = this._map.getFightEntitysByRange(this._shape, new List<int>() { ConfigConstant.ENTITY_BARRIER }, null, -1);
        for(int i = 0, len = entitys.Count; i < len; i++) {
            BarrierEntity barrier = (BarrierEntity)entitys[i];
            CollisionInfo info = Collision.checkCollision(barrier.shape, this._shape, this._map.mapData);            
            if(info.isHit) {
                Vector2D deltaPos = info.deltaPos;
                double radius = this._shape.radius + barrier.shape.radius;
                //把萝卜向外推，越中心越强
                double rate = 1 - Math2.range(info.dist / radius, 1, 0);
                deltaPos.length = deltaPos.length * rate * 0.4;
                this.addForce(deltaPos);
                barrier.dispatchEventWith(EventConstant.HIT);
            }
        }
        
    }

    private void guidePlayer() {
        if(null != this.ownerPlayer && this.ownerPlayer.alived) {
            CollisionInfo info = Collision.checkCollision(this._seek, this.ownerPlayer.shape, this._map.mapData);
            if(!info.isHit) {
                this.ownerPlayer = null;
                return;
            }


            Vector2D delta = info.deltaPos.clone();
            double dist = info.dist;
            //追踪目标，直接改变位置，
            delta.length = Math2.range(( this.ownerPlayer.attractShape.radius - dist ) / this.ownerPlayer.attractShape.radius, 0.2, 0.1) * ConfigConstant.ITEM_MAG_SPEED;
            this.position = this.limitPosition(this._position.add(delta));

            if(Collision.checkCollision(this.ownerPlayer.shape, this._shape, this._map.mapData).isHit) {
                this._map.refereeController.gainRadish(this.ownerPlayer);
            }
        } else {
            //找到最近的目标
            List<PersonEntity> playerList = this._map.getPerson(this._seek, -1, -1, false);
            if(playerList.Count > 0) {
                this.ownerPlayer = (PlayerEntity)playerList[0];
            }
            this.applyPosition();
        }
    }

    protected override void velocityToPosition() {
        base.velocityToPosition();
        this._velocity.multiply(0.8);
    }
    

    public virtual void gainRadish() {
        this._alived = false;     
        this.removeGrids();
        //给组队赛一个萝卜进去！
        this.teamIndex = this.ownerPlayer.teamIndex;
        Dictionary<string, object> buffs = (Dictionary<string, object>)this._data["buff"];
        string id = buffs["id"].ToString();
        foreach(string key in buffs.Keys) {
            if(key == "id") continue;
            Buff buff = new Buff(this._map).initBuff(id, key, this.ownerPlayer, this.ownerPlayer, (Dictionary<string, object>)buffs[key]);
            if(buff.bind) this._buffs.Add(buff);
        }
        this.ownerPlayer.cureHpPer(Convert.ToDouble(this._data["cure"]));
        this.dispatchEventWith(EventConstant.GAIN_RADISH, this.ownerPlayer);
	}


    public void dropRadish() {
        //移除身上的buff
        for(int i = 0, len = this._buffs.Count; i < len; i++) {
            this._buffs[i].finish();
        }
        this._buffs.Clear();
        //出现在玩家阵亡的位置，并高抛出去
        this.position = this.ownerPlayer.position.clone();
        this._velocity = Vector2D.createVector2(20, this.ownerPlayer.angle);        
        this.reliveTime.totalTime = ConfigConstant.RADISH_RADISH_RELIVE_TIME;
        this.reliveTime.reset();
        this._alived = true;
        this.teamIndex = -1;
        this.dispatchEventWith(EventConstant.DROP_RADISH, this.ownerPlayer);
        this.ownerPlayer = null;
    }
    

    public override Dictionary<string, object> getData () {
		Dictionary<string, object> data = base.getData ();
        data["reliveTime"] = this.reliveTime.netId;
        data["seek"] = this._seek.netId;
        data["teamIndex"] = this.teamIndex;
        data["buffs"] = this._buffs.ConvertAll<object>((e)=> { return e.netId; }).ToArray();
        return data;
	}


    public override void setData (Dictionary<string, object> data) {
        this.reliveTime = (TimeAction)this._map.getNetObject((int)data["reliveTime"]);
        this.reliveTime.callBack = this.reliveHandler;
        this._seek = (GeomBase)this._map.getNetObject((int)data["seek"]);
        this.teamIndex = (int)data["teamIndex"];
        this._buffs = new List<object>((object[])data["buffs"]).ConvertAll<Buff>((object id) => { return (Buff)this._map.getNetObject((int)id); });
        base.setData (data);
	}

    public override void generateBirthGrids(int extend = 0) {
        this.birthPosition.copy(this._position);
        base.generateBirthGrids(extend);
    }

    public override void clear() {
        base.clear();
    }

    public override int type { get { return ConfigConstant.ENTITY_RADISH; } }

    public override bool alived {
        get {
            return base.alived;
        }

        set {
            base.alived = value;
        }
    }

}
