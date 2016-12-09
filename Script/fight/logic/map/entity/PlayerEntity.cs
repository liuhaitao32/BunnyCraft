using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity:PersonEntity {

    public string uid = "-----";

    //固定的装备位置(相对于自身，辅助计算装备命中)
    public Vector2D partHeadOffset = Vector2D.createVector2(ConfigConstant.SHIP_RADIUS, 0);
    public Vector2D partWingOffsetR = Vector2D.createVector2(ConfigConstant.SHIP_RADIUS, -Math.PI / 2);
    public Vector2D partWingOffsetL = Vector2D.createVector2(ConfigConstant.SHIP_RADIUS, Math.PI / 2);
    public Vector2D partTailOffset = Vector2D.createVector2(ConfigConstant.SHIP_RADIUS, Math.PI);

    public CardGroupAction cardGroup;

    public PartGroupAction partGroup;

    public string shipId = "ship800";

    public int index;

    public string name;

    public TimeAction reviveAction;

    public int teamIndex = -1;

    public List<CallEntity> calls = new List<CallEntity>();

    public FightResult fightResult;

    private GeomBase _attractShape;

    public string headUrl = "";

    public PlayerEntity (Map map, int netId = -1):base(map, netId) {

    }

    public void addCallEntity(CallEntity entity) {
        this.calls.Add(entity);
        this.updateCallEntity();
    }

    public void removeCallEntity(CallEntity entity) {
        this.calls.Remove(entity);
        this.updateCallEntity();
    }

    private void updateCallEntity() {        
        int layer = 0;
        int num = 4;
        int max = this.calls.Count;
        int index = 0;
        for(int i = 0, len = this.calls.Count; i < len; i++) {
            CallEntity entity = this.calls[i];
            entity.index = i;
            if(index >= num) {
                max -= num;
                index -= num;
                num += 2;
                layer++;
            }
            double count = Math.Min(max, num);
            //最外圈要根据最后的数量进行间隔
            double interval = Math2.PI2 / count;
            double start = count % 2 == 0 ? -interval / 2 : 0;
            entity.layer = layer;
            entity.offset.length = this.getProperty(ConfigConstant.PROPERTY_RADIUS) + ConfigConstant.CALL_POS_RADIUS + layer * ConfigConstant.CALL_LAYER_RADIUS;
            entity.offset.angle = start + interval * index;
            index++;
        }
    }

    public override void initConfig(Dictionary<string, object> data = null) {
        this.teamIndex = (int)(data["teamIndex"]);
        this.shipId = data["shipId"].ToString();
        this.uid = data["uid"].ToString();
        this.name = null == data["name"] ? this.uid : data["name"].ToString();
		this.level = 1;//(data["level"]==null)?1:(int)data["level"];
        this.ownerPlayer = this;
        this.id = "player";
        this.headUrl = this.uid.IndexOf("-") == 0 ? this.name : data["head"] == null ? "" : ((Dictionary<string, object>)data["head"])["use"].ToString();

        this.fightResult = new FightResult(this._map);
        this.fightResult.init();

        this.cardGroup = new CardGroupAction(this._map);
        this.partGroup = new PartGroupAction(this._map);
        this.cardGroup.ids = new List<object>((object[])data["cardGroup"]).ConvertAll<Dictionary<string, object>>((object obj) => {
            return (Dictionary<string, object>)obj;
        });
        this.cardGroup.init(this);
        this.partGroup.init(this);

        base.initConfig((Dictionary<string, object>)DataManager.inst.beckon[this.id]);

        this._attractShape = Collision.createGeom(this, new object[] { (int)this.getProperty(ConfigConstant.PROPERTY_ATTRACT) });
        this.actionManager.addAction(this.cardGroup);
        this.actionManager.addAction(this.partGroup);
        this.updateLevel();
    }
    

    public void changeScore(int v) {
        int oldLevel = this.fightResult.scoreLevel;
        this.fightResult.changeScore(v);
        if(oldLevel == this.fightResult.scoreLevel) return;
        this.updateLevel();
        this.dispatchEventWith(EventConstant.LEVEL_CHANGE, new Dictionary<string, object> { { "oldLevel", oldLevel}, { "newLevel", this.fightResult.scoreLevel } });
    }
    

    private void updateLevel() {
        Dictionary<string, object> scoreData = (Dictionary<string, object>)ConfigConstant.scoreConfig[this.fightResult.scoreLevel];
        foreach(string key in scoreData.Keys) {
            if(this._property.ContainsKey(key)) {
                this._property[key] = Convert.ToDouble(scoreData[key]);
            }
        }
        this.changeProperty();
    }

    public override void setProperty(string type, double value) {
        base.setProperty(type, value);
        switch(type) {
            case ConfigConstant.PROPERTY_ATTRACT:
                this._attractShape.radius = (int)value;
                break;
        }
    }




    public virtual void killPlayer(PlayerEntity player) {
        /************************抢夺积分************************************/
        int temp = Math.Min(Mathf.RoundToInt(player.fightResult.score * ConfigConstant.SCORE_KILL_PLAYER_RATE), ConfigConstant.SCORE_KILL_PLAYER_MAX);
        player.changeScore(-temp);
        this.changeScore(temp + ConfigConstant.SCORE_KILL_PLAYER);
        /************************生成豆************************************/
        double angle = this._map.getRandomRange(0, Math2.PI2);

        for(int i = 0; i < ConfigConstant.DEAD_ITEM_NUM; i++) {
            PriceBeanEntity entity = this.map.createFightEntity(ConfigConstant.ENTITY_PRICE_BEAN) as PriceBeanEntity; 
            int index = i % 2 + 1;
            entity.id = "bean" + index;
            entity.position.copy(player.position);
            Dictionary<string, object> config = (Dictionary<string, object>)ConfigConstant.powerConfig[entity.id];
            entity.initConfig(config);
            entity.addForce(Vector2D.createVector2(ConfigConstant.DEAD_ITEM_FORCE, Math2.PI2 * i / ConfigConstant.DEAD_ITEM_NUM + angle));
        }

        int len = Mathf.RoundToInt(player.fightResult.score * ConfigConstant.DROP_SCORE_ITEM_RATE / ConfigConstant.SCORE_UNIT) + ConfigConstant.DROP_SCORE_ITEM_MIN;
        len = Math.Min(len, ConfigConstant.DROP_SCORE_ITEM_MAX);

        this.changeScore(-len * ConfigConstant.SCORE_UNIT);
        angle = this._map.getRandomRange(0, Math2.PI2);
        for(int i = 0; i < len; i++) {
            double force = this._map.getRandomRange(ConfigConstant.DROP_SCORE_ITEM_FORCE_MIN, ConfigConstant.DROP_SCORE_ITEM_FORCE_MAX);
            PriceBeanEntity entity = this.map.createFightEntity(ConfigConstant.ENTITY_PRICE_BEAN) as PriceBeanEntity;
            entity.id = "bean" + 4;
            entity.position.copy(player.position);
            Dictionary<string, object> config = (Dictionary<string, object>)ConfigConstant.powerConfig[entity.id];
            entity.initConfig(config);
            entity.addForce(Vector2D.createVector2(force, Math2.PI2 * i / len + angle));
        }
        /************************杀敌记录************************************/
        this.fightResult.killPerson();
        this._map.killPlayerNum++;

        //萝卜模式要扔掉萝卜。
        if(this._map.mapData.isRadish) {
            if(this._map.refereeController.checkDropRadish(player)) {
                this.fightResult.recordShot++;
                player.fightResult.recordBeShot++;
            }
        }
    }



    protected override void velocityToPosition() {
        Vector2D v = this._velocity.clone();
        this._position.add(v.multiply(this.map.speedRate));
        v.clear();
    }

    public override void update () {
		base.update ();
        MediatorSystem.timeStart("checkBea");
        this.checkBean();//检查吃豆子
        MediatorSystem.getRunTime("checkBea");
        this.fightResult.update();
    }

    public override void beHit(BulletEntity bullet, double damageRate = 1) {
        bool isDead = this._dead;
        base.beHit(bullet, damageRate);

        //被这个子弹干掉了。 算击杀。
        if(this._dead && !isDead) {
            bullet.ownerPlayer.killPlayer(this);
        }
    }

    //承受伤害，各个部件分摊伤害
    public override void hurt(BulletEntity bullet, double damageRate = 1) {
        //如果穿着了装备，先判定命中位置
        int damage = (int)(bullet.getDamage(this) * damageRate);
        //伤害加深。
        List<Buff> buffs = this.getBuffs(ConfigConstant.BUFF_HURT_SHALLOW);
        //TODO:如果都是乘以我就不用排序了！
        for(int i = 0, len = buffs.Count; i < len; i++) {
            damage = Mathf.CeilToInt(Convert.ToSingle(LogicOperation.operation(Convert.ToSingle(damage), buffs[i].value, buffs[i].operation)));
        }

        //对玩家造成了伤害，子弹的主人奖励积分
        bullet.ownerPlayer.changeScore(Mathf.CeilToInt(Convert.ToSingle(ConfigConstant.SCORE_DAMAGE * damage)));
        //护盾
        damage = this.hurtShield(damage);
        //忽略体积
        if(Utils.equal(bullet.data, "ignoreBear", 1)) {
            damage = Mathf.CeilToInt(Convert.ToSingle(damage * ( 1 - this.getProperty(ConfigConstant.PROPERTY_DEF_RATE) ))) ;
        } else {
            damage = Mathf.CeilToInt(Convert.ToSingle(damage * ( 1 - this.getProperty(ConfigConstant.PROPERTY_DEF_RATE) ) * this.getProperty(ConfigConstant.PROPERTY_BEAR_RATE)));
            //如果穿着了装备，先判定命中位置
            if(this.partGroup.hasPart) {
                Vector2D deltaP = bullet.getHitPos(this);
                //转化相对角度
                deltaP.angle -= this.angle;
                //依次判定相对命中点 距离4个装备参考点是否算命中
                //			double distance;
                int bangRadius = (int)( bullet.radiusMax + ConfigConstant.SHIP_RADIUS * 0.5f );
                List<int> hitParts = new List<int>();
                if(deltaP.dist(partHeadOffset) <= bangRadius) {
                    //前部件被命中
                    hitParts.Add(0);
                }
                if(deltaP.dist(partWingOffsetL) <= bangRadius || deltaP.dist(partWingOffsetR) <= bangRadius) {
                    //中部件被命中
                    hitParts.Add(1);
                }
                if(deltaP.dist(partTailOffset) <= bangRadius) {
                    //后部件被命中
                    hitParts.Add(2);
                }
                //根据被命中的部件个数，每个部件分摊伤害
                int hitCount = hitParts.Count;
                if(hitCount > 0) {
                    int tempDamage;
                    int damagePerPart = damage / hitCount;
                    int i;
                    for(i = 0; i < hitCount; i++) {
                        PartAction part = this.partGroup.getPart(hitParts[i], true);
                        if(part != null) {
                            //装备部件分担伤害
                            tempDamage = Mathf.CeilToInt(Convert.ToSingle(damagePerPart * part.hpRate));
                            part.beHit(tempDamage);
                            damage -= tempDamage;
                        }
                    }
                }
            }
        }
        
        this.changeHp(-damage);
    }



	private void checkBean() {
        List<FightEntity> beans = this.map.getFightEntitysByRange(this._attractShape, new List<int> { ConfigConstant.ENTITY_LOOP_BEAN, ConfigConstant.ENTITY_PRICE_BEAN }, (e1, e2)=> {
            return e2.alived && null == e2.ownerPlayer;
        }, -1);
		for (int i = 0, len = beans.Count; i < len; i++) {
            beans[i].ownerPlayer = this;
		}
	}
    
    

    /**
     * 这个初步认为是结算状态。 
     */
    public override void applyPosition() {
        base.applyPosition();
        this.checkRelive();
    }

    protected virtual void checkRelive() {
        if(this._dead) {
            this.alived = false;
            this.setOldViewData();
            this.setNewViewData();
            if(this._map.mapData.isRadish) {
                //我方持有萝卜旗，复活时间变长
                int reliveCd = this._map.refereeController.radishController.radish.teamIndex == this.teamIndex ? 
                                                                                                        ConfigConstant.RADISH_HOLD_RELIVE_TIME : 
                                                                                                        ConfigConstant.RADISH_RELIVE_TIME;
                this.reviveAction = this.delayCall(reliveCd, this.revive, ConfigConstant.MAP_CALL_BACK_REVIVE);
            } else {
                this.reviveAction = this.delayCall(ConfigConstant.RELIVE_TIME, this.revive, ConfigConstant.MAP_CALL_BACK_REVIVE);
            }
            
            this.removeGrids();
        }
    }

    public Vector2D randomBirthPosition() {
        Vector2D result;
		if (FightMain.isFixedBirth) {
			//Biggo添加修改
			result = new Vector2D(0,this._map.mapData.heightHalf);
		}
		else{
			int yMin = this._map.mapData.gridYAreaLines [2];
			int yMax = this._map.mapData.rowNum - this._map.mapData.gridYAreaLines [2] - 1;
			List<Vector2D> range = new List<Vector2D> { new Vector2D (yMin, yMax) };

            //萝卜模式 再加入远离萝卜
            if(this._map.mapData.isRadish && -1 == this._map.refereeController.radishController.radish.teamIndex) {
                this._map.refereeController.radishController.radish.generateBirthGrids();
            }

            //让他离每个人远一点。
            Grid grid = null;
			for (int i = 4; i >= 0; i--) {            
				for (int j = 0, len2 = this._map.players.Count; j < len2; j++) {
					if (this._map.players [j] != this) {
						this._map.players [j].generateBirthGrids (i);
					}                
				}
				grid = this._map.birthGrids.getRandomBirthGrid (range, new List<int> { ConfigConstant.ENTITY_PLAYER });
				if (null != grid)
					break;
			}

			//找不到 随机给一点。
			if (grid == null) {
				while (null == grid) {
					List<Grid> grids = this._map.birthGrids.birthPlayerGrids [(int)(this.birthGrids.Count * this._map.getRandom ())];
					if (grids.Count > 0) {
						grid = grids [(int)(grids.Count * this._map.getRandom ())];
					}                
				}        
			}
			result = grid.randomPosition;
			Utils.clearList(range);
		}
        //if(this.uid == "0") return new Vector2D();
        return result;
    }

    protected virtual void revive() {
        this.reviveAction = null;
        this.skillManager.removeAllAction();
        this.partGroup.removeParts();
        this.cardGroup.resetCards();
        this._force.zero();
        this._joystickForce.zero();
        this._velocity.zero();
        this.birthPosition = this.randomBirthPosition();
        this.position = this.birthPosition;
        this.angle = 0;
        this.setNewViewData();
        this.setOldViewData();
        this.removeAllBuff();
        Utils.clearList(this.calls);
        this.alived = true;
        this.changeProperty();
        this.hp = (int)this._helpProperty[ConfigConstant.PROPERTY_HP];
    }

    public override Dictionary<string, object> getData () {
		Dictionary<string, object> data = base.getData ();
        data["uid"] = this.uid;
        data["cardGroup"] = this.cardGroup.netId;
        data["partGroup"] = this.partGroup.netId;
        data["shipId"] = this.shipId;
        data["index"] = this.index;
        data["name"] = this.name;
        data["headUrl"] = this.headUrl;
        if(null != reviveAction) {
            data["reviveAction"] = this.reviveAction.netId;
        }
        data["teamIndex"] = this.teamIndex;
        data["calls"] = this.calls.ConvertAll<object>((CallEntity c) => { return c.netId; }).ToArray();
        data["fightResult"] = this.fightResult.netId;
        data["attractShape"] = this._attractShape.netId;
        return data;
	}
    

    public override void setData (Dictionary<string, object> data) {
        this.uid = data["uid"].ToString();
        if(this.uid == ( (ClientRunTime)this._map ).uid) ( (ClientRunTime)this._map ).localPlayer = (ClientPlayerEntity)this;
        this.cardGroup = (CardGroupAction)this._map.getNetObject((int)(data["cardGroup"]));
        this.partGroup = (PartGroupAction)this._map.getNetObject((int)(data["partGroup"]));
        this.shipId = data["shipId"].ToString();
        this.index = (int)(data["index"]);
        this.name = data["name"].ToString();
        if(data.ContainsKey("reviveAction")) {
            this.reviveAction = (TimeAction)this._map.getNetObject((int)(data["reviveAction"]));
            this.reviveAction.callBack = this.revive;
        }
        this.teamIndex = (int)(data["teamIndex"]);
        this.calls = new List<object>((object[])data["calls"]).ConvertAll<CallEntity>((object netId) => {
            return (CallEntity)this._map.getNetObject((int)( netId ));
        });
        this.fightResult = (FightResult)this._map.getNetObject((int)( data["fightResult"] ));
        this._attractShape = (GeomBase)this._map.getNetObject((int)( data["attractShape"] ));
        this.headUrl = data["headUrl"].ToString();
        base.setData (data);
	}

	public override int type { get {return ConfigConstant.ENTITY_PLAYER;} }

    public override void changeHp(int value) {
        base.changeHp(value);
    }

    public override bool alived {
        get { return base.alived;}
        set { this._dead = !(base.alived = value);}
    }

    public GeomBase attractShape {
        get {
            if(this._propertyChange) this.changeProperty();
            return this._attractShape;
        }
    }

    public override void generateBirthGrids(int extend = 0) {
        this.birthPosition = this._position;
        base.generateBirthGrids(extend);
    }
    

    public override void clear() {
        this.uid = null;
        Utils.clearList(this.calls);
        Utils.clearObject(this.cardGroup);
        Utils.clearObject(this.partGroup);
        Utils.clearObject(this.reviveAction);
        base.clear();
    }

}
