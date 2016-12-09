using System;
using System.Collections.Generic;
using UnityEngine;

public class BeanEntity :FightEntity {
    
    public int itemType = -1;
    public BeanVO beanVO;

    public BeanEntity (Map map, int netId = -1):base(map, netId) {
        
    }

    public override void initConfig(Dictionary<string, object> data = null) {
        this.itemType = (int)(data["itemType"]);
        this._shape = Collision.createGeom(this, new object[] { (int)(data["radius"]) });
        this.beanVO = BeanVO.getVOByType(this.itemType);
        base.initConfig(data);
    }
    


	public override void update () {
        base.update ();
        this.guidePlayer();
    }


    private void guidePlayer() {
        if(null == this.ownerPlayer) return;
        if(!this.ownerPlayer.alived || !Collision.checkCollision(this.ownerPlayer.attractShape, this._shape, this._map.mapData).isHit) {
            this.ownerPlayer = null;
            return;
        }
        Vector2D delta = Collision.realPosition(this._position, this.ownerPlayer.position, this._map.mapData).deltaPos.clone();

        //追踪目标，直接改变位置，
        delta.length = Math2.range(1 - delta.length / this.ownerPlayer.attractShape.radius, 0.5, 0.2) * ConfigConstant.ITEM_MAG_SPEED;
        this.position = this.limitPosition(this._position.add(delta));

        if(Collision.checkCollision(this.ownerPlayer.shape, this._shape, this._map.mapData).isHit) {
            this.useBean();
        }
        this.setNewViewData();
    }

	//是否可以被AI发现去吃 Biggo添加
	public bool canUse{
		get{
			return this.alived &&  this.ownerPlayer == null;
		}
	}
		
    public virtual void useBean() {
        //hp
        if(0 != this.beanVO.cure) {//太操蛋的。 先是double类型 乘之后还得转成float 然后还得转int。破数据类型。
            this.ownerPlayer.cureHpPer(this.beanVO.cure);
        }
        //power
        if(0 != this.beanVO.power) this.ownerPlayer.cardGroup.changePower(this.beanVO.power);
        //score
        if(0 != this.beanVO.score) this.ownerPlayer.changeScore(this.beanVO.score);
        //冲力
        if(0 != this.beanVO.push) {
            Vector2D force = Vector2D.createVector2(this.beanVO.push, this.ownerPlayer.angle);
            this.ownerPlayer.addForce(force);
        }
        this.dispatchEventWith(EventConstant.HIT, this.ownerPlayer);
        this.removeGrids();
        this.ownerPlayer = null;
        this.alived = false;
	}


    public override Dictionary<string, object> getData () {
		Dictionary<string, object> data = base.getData ();
        data["itemType"] = this.itemType;
        return data;
	}


    public override void setData (Dictionary<string, object> data) {
        this.itemType = (int)(data["itemType"]);
        this.beanVO = BeanVO.getVOByType(this.itemType);
        base.setData (data);
	}

    public override void clear() {
        this.dispatchEventWith(EventConstant.DEAD);
        base.clear();
    }

}
