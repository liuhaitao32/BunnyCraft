using System;
using System.Collections.Generic;
using UnityEngine;

public class Map : EventClass {

    public BirthGridManager birthGrids;

    public FindEntityManager findEntity;

    public int netId = 0;

    public List<LoopBeanEntity> beans = new List<LoopBeanEntity>();

    public List<PersonEntity> persons = new List<PersonEntity>();

    public List<PlayerEntity> players = new List<PlayerEntity>();


    public List<BulletEntity> bullets = new List<BulletEntity>();

    public List<FightEntity> others = new List<FightEntity>();

    protected double _fps = 1;

    protected int interval = 0;

    public MapData mapData;

    public FightRandom random = new FightRandom();

    protected MultiAction _delayCalls;

	///AI IQ Biggo添加，应该通过比赛段位来初始化
	public double AI_IQ;

    public double speedRate;

    public int powerGain;

    protected TimeAction warnning;

    public RefereeController refereeController;


    public BirthBeanController birthBeanController;
	///Biggo添加
	public AIController aiController;

    public int stepIndex = 0;

    public int killPlayerNum;

    public Map() {
        this.powerGain = ConfigConstant.POWER_GAIN;
        this.speedRate = ConfigConstant.SPEED_RATE;
		this.AI_IQ = AIConstant.IQ_DEFAULT;
    }

    public virtual void finish() {

    }


    public virtual void specialAction(ActionBase action) {

    }

    
	///取得其他玩家，Biggo添加
	public List<PlayerEntity> findPlayer(GeomBase geom, int aim = 1, int sort = 0, bool checkCollision = true) {
        return this.findEntity.findPlayer(geom, aim, sort, checkCollision);
	}

    public List<PersonEntity> getPerson(GeomBase geom, int aim = 1, int sort = 0, bool call = true, bool checkCollision = true) {
        return this.findEntity.getPerson(geom, aim, sort, call, checkCollision);
    }

    public List<PlayerEntity> getSortPlayer(int sort, object value = null) {
        List<PlayerEntity> result = new List<PlayerEntity>(this.players);
        switch(sort) {
            case 0://距离排序
                result.Sort((e1, e2) => {
                    double dist1 = e1.position.distSQ((Vector2D)value);
                    double dist2 = e2.position.distSQ((Vector2D)value);
                    if(dist1 - dist2 < 0) {
                        return -1;
                    } else if(dist1 - dist2 > 0) {
                        return 1;
                    } else {
                        return e1.netId - e2.netId;
                    }
                });
                break;
            case 1://分数排序
                result.Sort((p1, p2) => {
                    if(p1.fightResult.score != p2.fightResult.score) return p2.fightResult.score - p1.fightResult.score;
                    return p1.index - p2.index;
                });
                break;
        }
        return result;
    }


    //这个只会在初始化调用 server需要手动调用。 但是客户端之后播放录像的时候调用初始化。
    protected void initMap() {
        //这两个参数需要后端自己赋值。 先临时这么写一下。
        if(FightMain.instance.mode == ConfigConstant.MODE_CHAOS || FightMain.instance.mode == ConfigConstant.MODE_TEST_CHAOS) {
            FightMain.instance.mapId = "map01";
        } else {
            FightMain.instance.mapId = "map02";
        }
        this.mapData = new MapData(FightMain.instance.mapId, FightMain.instance.mode);
        this._delayCalls = new MultiAction(this);
        this.birthBeanController = new BirthBeanController(this);
        this.birthBeanController.init();
		//Biggo添加
		//if (FightMain.isLocal) 
		this.aiController = new AIController (this);


        
        //警告之后地图里的要加速。
        this.warnning = this.addDelayCall(( this.mapData.fightTime - this.mapData.finalTime ) * 1000, this.warningTimeHandler, ConfigConstant.MAP_CALL_BACK_WARNING);
        this.birthGrids = new BirthGridManager(this);
        this.findEntity = new FindEntityManager(this);
        this.createBarrier();
        this.birthGrids.createBirthGrids();
        //这一步如果有萝卜 就创建萝卜。
        this.refereeController = new RefereeController(this);
        this.refereeController.init();
        this.createPlayers();
        FightMain.isTest = true;
        this.createBean();
        this.birthBeanController.removeWaitBean();
        //this.printGrid();
    }

    private void createPlayers() {
        //这个先生成下面的假数据， 这个后端先用我这边注释的 保证前后端一致。 卡牌就是join 我发给后端的（PlayerData.instance.cardGroup）
        //object[] users = new object[8];
        //PlayerData.instance.data["users"] = users;


        //for(int i = 0, len = users.Length; i < len; i++) {
        //    //[users]:uid lv effor_lv name head["use"] cards[id, lv];
        //    users[i] = new object[] {
        //                                    (i), 1, 1, "name" + (i), new Dictionary<string, object>() { { "use", ""} }, PlayerData.instance.cardGroup.ConvertAll<object>((e)=> {return new object[] { e["id"], e["level"]}; }).ToArray(), "ship800"
        //                                    };
        //}

        //这个后端也需要用自己生成的。
        if(PlayerData.instance.data.ContainsKey("users")) {
            object[] users = (object[])PlayerData.instance.data["users"];
            object[] teamDatas = new object[users.Length];
            for(int i = 0, len = users.Length; i < len; i++) {
                object[] userData = (object[])users[i];
                Dictionary<string, object> playerData = new Dictionary<string, object> {
                                    { "name", userData[3]},
                                    { "uid", userData[0]},
                                    { "cardGroup", Array.ConvertAll<object, object>((object[])userData[5], (o)=> {
                                        return new Dictionary<string, object>() {
                                            { "level", ((object[])o)[1] },
                                            { "id", ((object[])o)[0] }
                                        };
                                    })},
                                    { "shipId", userData[6]},
                                    { "level", userData[1]},
                                    { "teamIndex", i < 4 ? 0 : 1},
                                    { "head", userData[4]},
                                    { "iq", userData.Length > 6 ? userData[7] : 0}
                                };
                teamDatas[i] = playerData;
            }
            for(int i = 0, len = teamDatas.Length; i < len; i++) {
                this.createUser((Dictionary<string, object>)teamDatas[i]);
            }
        }
    }


    protected void createBarrier() {
        object[] barriers = (object[])ConfigConstant.combat["barrier"];
        int fixedNum = null == this.mapData.barriers ? 0 : this.mapData.barriers.Length;
        for(int i = 0, len = this.mapData.barrierCount; i < len; i++) {
            BarrierEntity barrier = (BarrierEntity)this.createFightEntity(ConfigConstant.ENTITY_BARRIER);

            int index = -1;
            if(i < fixedNum) {
                object[] barrierInfo = (object[])this.mapData.barriers[i];
                index = (int)barrierInfo[2];
                barrier.position.copy(Vector2D.createVector((int)barrierInfo[0], (int)barrierInfo[1]));
            } else {
                index = (int)this.getRandomRange(0, barriers.Length);
                barrier.position.copy(barrier.createBirthPosition());
            }

            barrier.id = "barrier" + index;            
            barrier.initConfig((Dictionary<string, object>)barriers[index]);
        }
    }
    private void createBean() {
        //TODO:powerConfig的beanNum 要干掉！

        Dictionary<string, object> config = null;
                    
        config = (Dictionary<string, object>)ConfigConstant.powerConfig["bean2"];
        for(int i = 0, len = this.mapData.bean2Count; i < len; i++) {
            LoopBeanEntity bean = this.createFightEntity(ConfigConstant.ENTITY_LOOP_BEAN) as LoopBeanEntity;
            bean.id = "bean2";
            bean.initConfig(config);
        }


        config = (Dictionary<string, object>)ConfigConstant.powerConfig["bean1"];
        for(int i = 0, len = this.mapData.bean1Count; i < len; i++) {
            LoopBeanEntity bean = this.createFightEntity(ConfigConstant.ENTITY_LOOP_BEAN) as LoopBeanEntity;
            bean.id = "bean1";
            bean.initConfig(config);
        }


        config = (Dictionary<string, object>)ConfigConstant.powerConfig["bean3"];
        for(int i = 0, len = this.mapData.bean3Count; i < len; i++) {
            LoopBeanEntity bean = this.createFightEntity(ConfigConstant.ENTITY_LOOP_BEAN) as LoopBeanEntity;
            bean.id = "bean3";
            bean.initConfig(config);
        }
    }

    public TimeAction addDelayCall(int time, Action callBack, int handlerId) {
        TimeAction timeAction = new TimeAction(this).init(time, callBack, handlerId);
        this._delayCalls.addAction(timeAction);
        timeAction.start();
        return timeAction;
    }

    public TimeAction removeDelayCall(TimeAction timeAction) {
        this._delayCalls.removeAction(timeAction);
        return timeAction;
    }


    public virtual PlayerEntity createUser(Dictionary<string, object> data) {
        string uid = data["uid"].ToString();
        PlayerEntity player = this.getPlayer(uid);
        Vector2D pos = null;
        if(null == player) {
            player = (PlayerEntity)this.createFightEntity(ConfigConstant.ENTITY_PLAYER);
            if(null != this.mapData.playerInitData) {
                for(int i = 0, len = this.mapData.playerInitData.Count; i < len; i++) {
                    Dictionary<string, object> playerData = (Dictionary<string, object>)this.mapData.playerInitData[i];
                    if((int)playerData["team"] == (int)data["teamIndex"]) {
                        pos = Vector2D.createVector3((object[])playerData["birthPos"]);
                        this.mapData.playerInitData.RemoveAt(i);
                        break;
                    }
                }
            }
            if(pos == null) pos = player.randomBirthPosition();
            player.position.copy(pos);
            pos.clear();
            player.initConfig(data);
            //下面这段是ai的 先别写
            if(0 == uid.IndexOf("-")) {
                this.aiController.openAI(player, Convert.ToDouble(data["iq"]) / 100);
            }
        }
        return player;
    }

    public virtual void init() {

    }


    protected virtual void warningTimeHandler() {
        this.speedRate = ConfigConstant.LAST_SPEED_RATE;
        this.powerGain = ConfigConstant.LAST_POWER_GAIN;
        this.warnning = null;
        this.refereeController.enterFinal();
    }
    


    protected Dictionary<int, INetObject> _netObjects = new Dictionary<int, INetObject>();
    public INetObject getNetObject(int netId) {
        if(!this._netObjects.ContainsKey(netId)) {
            throw new Exception("找不到此网络对象");
        }
        return _netObjects[netId];
    }
    public virtual void addNetObject(INetObject netObject) {
        if(this._netObjects.ContainsKey(netObject.netId)) {
            throw new Exception("id重复！" + netObject + "  " + netObject.netId);
        }
        this._netObjects[netObject.netId] = netObject;
    }
    public virtual void removeNetObject(INetObject netObject) {
        if(this._netObjects.ContainsKey(netObject.netId)) this._netObjects.Remove(netObject.netId);
    }

    public virtual void addEntity(FightEntity entity) {
        switch(entity.type) {
            case ConfigConstant.ENTITY_LOOP_BEAN:
                this.beans.Add((LoopBeanEntity)entity);
                break;
            case ConfigConstant.ENTITY_PLAYER:
                ( (PlayerEntity)entity ).index = this.persons.Count;
                this.persons.Add((PlayerEntity)entity);
                this.players.Add((PlayerEntity)entity);
                break;
            case ConfigConstant.ENTITY_BULLET:
                this.bullets.Add((BulletEntity)entity);
                break;
            case ConfigConstant.ENTITY_PRICE_BEAN:
                this.others.Add((PriceBeanEntity)entity);
                break;
            case ConfigConstant.ENTITY_BARRIER:
                this.others.Add((BarrierEntity)entity);
                break;
            case ConfigConstant.ENTITY_CALL:
                this.persons.Add((CallEntity)entity);
                break;
            case ConfigConstant.ENTITY_RADISH:
                this.others.Add((RadishEntity)entity);
                break;
        }
    }

    public virtual void removeEntity(FightEntity entity) {
        switch(entity.type) {
            case ConfigConstant.ENTITY_LOOP_BEAN:
                this.beans.Remove((LoopBeanEntity)entity);
                break;
            case ConfigConstant.ENTITY_PLAYER:
                this.persons.Remove((PlayerEntity)entity);
                this.players.Remove((PlayerEntity)entity);
                break;
            case ConfigConstant.ENTITY_BULLET:
                this.bullets.Remove((BulletEntity)entity);
                break;
            case ConfigConstant.ENTITY_PRICE_BEAN:
                this.others.Remove((PriceBeanEntity)entity);
                break;
            case ConfigConstant.ENTITY_CALL:
                this.persons.Remove((CallEntity)entity);
                break;
            case ConfigConstant.ENTITY_RADISH:
                this.others.Remove((RadishEntity)entity);
                break;
        }
    }

    public virtual FightEntity createFightEntity(int type, int netId = -1) {
        return null;
    }

    public PlayerEntity getPlayer(string uid) {
        PlayerEntity result = null;
        for(int i = 0, len = this.players.Count; i < len; i++) {
            if(this.players[i].uid == uid) {
                result = (PlayerEntity)this.players[i];
                break;
            }
        }

        return result;
    }


    public double getRandom() {
        return this.random.getRandom();
    }

    public virtual double getRandomRange(double min, double max) {
        return min + this.getRandom() * ( max - min );
    }
		

    public virtual void update() {
        this.onUpdate();
    }

    protected virtual void onUpdate() {
        this.onMapUpdate();
    }

    protected virtual void onMapUpdate() {
        if(this.refereeController.isEnd) return;
        this.stepIndex++;
        MediatorSystem.timeStart("hitUpdate");
        //检查碰撞。。。
        for(int i = 0, len = this.persons.Count - 1; i < len; i++) {
            if(!this.persons[i].alived || this.persons[i].collisionShape.radius <= 0) continue;
            for(int j = i + 1, len2 = this.persons.Count; j < len2; j++) {
                if(!this.persons[j].alived || this.persons[j].collisionShape.radius <= 0) continue;
                this.checkPlayerHit2(this.persons[i], this.persons[j]);
            }
        }
        MediatorSystem.getRunTime("hitUpdate");


		//ai计算 Biggo添加
		MediatorSystem.timeStart("aiUpdate");
		if (!TestValue.test4 && null != this.aiController) {
			this.aiController.update();
		}
		MediatorSystem.getRunTime("aiUpdate");

        //if(!TestValue.test2) {
            MediatorSystem.timeStart("playerUpdate");
            for(int i = this.persons.Count - 1; i >= 0; i--) {
                if(this.persons[i].alived) this.persons[i].update();
            }
            MediatorSystem.getRunTime("playerUpdate", this.persons.Count);
        //}


        //if(!TestValue.test3) {
            MediatorSystem.timeStart("beasUpdate");
            for(int i = this.beans.Count - 1; i >= 0; i--) {
                if(this.beans[i].alived) this.beans[i].update();
            }
            MediatorSystem.getRunTime("beasUpdate", this.beans.Count);
        //}
        //if(!TestValue.test4) {
            MediatorSystem.timeStart("othersUpdate");
            for(int i = this.others.Count - 1; i >= 0; i--) {
                this.others[i].update();
            }
            MediatorSystem.getRunTime("othersUpdate", this.others.Count);
        //}

        //if(!TestValue.test5) {
            MediatorSystem.timeStart("bulletsUpdate");
        //Debug.Log("update");
            for(int i = this.bullets.Count - 1; i >= 0; i--) {
                this.bullets[i].update();
            }
            MediatorSystem.getRunTime("bulletsUpdate", this.bullets.Count);
        //}

        //if(!TestValue.test6) {
            MediatorSystem.timeStart("applyPosition");
            for(int i = this.persons.Count - 1; i >= 0; i--) {
                if(this.persons[i].alived) this.persons[i].applyPosition();
            }
            MediatorSystem.getRunTime("applyPosition", this.persons.Count);

        //}

        this._delayCalls.update();
        //if(!TestValue.test7) {
            this.birthBeanController.update();
        this.refereeController.update();
        //}
    }

    
    public List<Grid> getGrids(GeomBase geom, List<Grid> grids = null) {        
        return this.findEntity.getGrids(geom, grids);
    }
    
    /// 这个里面的entitys 会被复用的 所以 取出来人物的时候 如果 里面还要调用此方法的话，请使用副本。
	public List<FightEntity> getFightEntitysByRange(GeomBase shape, List<int> types, Func<FightEntity,FightEntity, bool> filter = null, int sort = 0, bool checkCollision = true) {
        return this.findEntity.getFightEntitysByRange(shape, types, filter, sort, checkCollision);
    }
    
    public List<FightEntity> getFightEntitysByGrids(List<Grid> grids, List<int> types) {
        return this.findEntity.getFightEntitysByGrids(grids, types);
    }


    private void checkPlayerHit2(PersonEntity person1, PersonEntity person2) {
        if(person1.type == ConfigConstant.ENTITY_CALL && person1.ownerPlayer.teamIndex == person2.ownerPlayer.teamIndex) return;
        if(person2.type == ConfigConstant.ENTITY_CALL && person1.ownerPlayer.teamIndex == person2.ownerPlayer.teamIndex) return;

        CollisionInfo info = Collision.checkCollision(person1.collisionShape, person2.collisionShape, this.mapData);

        if(info.isHit) {
            //相对速度冲量(投影到斥力方向的分力)
            Vector2D velocityDeltaV2d = person1.velocity.clone().subtract(person2.velocity);
            Vector2D positionDeltaV2d = info.deltaPos.clone();
            double len = velocityDeltaV2d.projectionOn(positionDeltaV2d);
            if(len > 0) {
                Vector2D pushForceV2d = positionDeltaV2d.clone();
                bool isPlayer = person1.type == ConfigConstant.ENTITY_PLAYER && person2.type == ConfigConstant.ENTITY_PLAYER;
                bool hurtCollision = isPlayer && person1.ownerPlayer.teamIndex != person2.ownerPlayer.teamIndex;
                double forcePush = hurtCollision ? ConfigConstant.PLAYER_FORCE_PUSH2 : ConfigConstant.PLAYER_FORCE_PUSH;
                forcePush *= len;
                int pushBack = isPlayer ? ConfigConstant.PUSH_FORCE_BACK : 0;
                pushForceV2d.length = forcePush * person1.scale / person2.scale + pushBack;
                person2.addForce(pushForceV2d);
                pushForceV2d.length = forcePush * person2.scale / person1.scale + pushBack;
                pushForceV2d.multiply(-1);
                person1.addForce(pushForceV2d);

                pushForceV2d.clear();
                //都是人  并且队伍不同。 碰撞伤害！
                if(hurtCollision) {
                    //子弹的配置。也确实 写死就好了。
                    Dictionary<string, object> bulletData = new Dictionary<string, object> {
                        {"resId", "bulletNull"},
                        {"speed", 1},
                        {"lifeTime", 30},
                        {"atk", (int)(forcePush * ConfigConstant.PUSH_DAMAGE_RATE) + ConfigConstant.PUSH_DAMAGE},
                        {"range", new object[] { ConfigConstant.SHIP_RADIUS } },
                        {"posTarget", 1},
                        {"bangRes", "hitCollide"},
                        {"bangScale", Math.Sqrt(forcePush * 0.02)},
                        { "buff", new Dictionary<string, object>() {
                            { "spRate", new Dictionary<string, object>() {
                                //'joystickMin': { 'value':60, 'operation':1, 'effect':'buff002' },   #遥感最小值
                                            { "value", 0.4f},
                                            { "operation", 2},
                                            { "time", 500},
                            }},
                            { "id", "spRate"}
                        } }
                    };
                    Vector2D pos = positionDeltaV2d.clone().multiply(0.5);
                    BulletEntity bullet = this.createFightEntity(ConfigConstant.ENTITY_BULLET) as BulletEntity;
                    bullet.lockTarget = person1;
                    bullet.owner = person2;
                    bullet.position.copy(pos);
                    bullet.initConfig(bulletData);

                    bullet = this.createFightEntity(ConfigConstant.ENTITY_BULLET) as BulletEntity;
                    bullet.lockTarget = person2;
                    bullet.owner = person1;
                    bullet.position.copy(pos.reverse());
                    bullet.initConfig(bulletData);
                }

            }
            //近距离排斥力，越近越大
            double dist = ( person1.collisionShape.radius + person2.collisionShape.radius) - positionDeltaV2d.length;
            positionDeltaV2d.length = dist * ConfigConstant.PLAYER_FORCE_REPULSIVE;
            person2.addForce(positionDeltaV2d);
            positionDeltaV2d.multiply(-1);
            person1.addForce(positionDeltaV2d);
            velocityDeltaV2d.clear();
        }
    }
    

    protected virtual void setData(Dictionary<string, object> data) {
        this.mapData = new MapData(data["id"].ToString(), (int)( data["fightMode"] ));
        if(data.ContainsKey("playerInitData")) {
            this.mapData.playerInitData = new List<object>((object[])data["playerInitData"]);
        }
        this.birthGrids = new BirthGridManager(this);
        this.findEntity = new FindEntityManager(this);
        //TODO:写在这里 容易对比。 后端不用抄setData的方法。
        List<Dictionary<string, object>> netObjectDatas = new List<object>((object[])data["entitys"]).ConvertAll<Dictionary<string, object>>((object obj) => {
            return (Dictionary<string, object>)obj;
        });
        netObjectDatas.Sort((Dictionary<string, object> obj1, Dictionary<string, object> obj2) => {
            return (int)( obj1["type"] ) - (int)( obj2["type"] );
        });

        Dictionary<int, INetObject> netObjects = this._netObjects;
        this._netObjects = new Dictionary<int, INetObject>();


        //先生成出netObject
        for(int i = 0, len = netObjectDatas.Count; i < len; i++) {
            int netId = (int)( netObjectDatas[i]["netId"] );
            INetObject obj = netObjects.ContainsKey(netId) ? netObjects[netId] : null;
            int type = (int)( netObjectDatas[i]["type"] );
            if(null != obj && obj.type != type) {
                throw new Exception("初始化类型不对！");
            }
            if(null == obj) {
                NetObjectFactory.createNetObject(type, this, netId);
            } else {
                netObjects.Remove(netId);
                this.addNetObject(obj);
            }
        }

        //netObject已经被移除掉了。 此时如果再有就说明恢复过程中有被删除的。这里手动的把它们删除掉！
        foreach(INetObject obj in netObjects.Values) {
            Utils.clearObject(obj);
        }


        this.netId = (int)( data["netId"] );
        this.random.seed = long.Parse(( data["randomSeed"] ).ToString());
        this.random.seedNum = (int)( data["randomSeedNum"] );
        this._delayCalls = (MultiAction)this.getNetObject((int)( data["delayCalls"] ));
        this.speedRate = Convert.ToSingle(data["speedRate"]);
        this.powerGain = (int)( data["powerGain"] );
        this.killPlayerNum = (int)( data["killPlayerNum"] );
        if(data.ContainsKey("warnning")) {
            this.warnning = (TimeAction)this.getNetObject((int)( data["warnning"] ));
            this.warnning.callBack = this.warningTimeHandler;
        } else {
            this.warningTimeHandler();
        }
        this.birthBeanController = (BirthBeanController)this.getNetObject((int)( data["birthBeanController"] ));
        this.refereeController = (RefereeController)this.getNetObject((int)( data["refereeController"] ));

        this.beans = new List<object>((object[])data["beans"]).ConvertAll<LoopBeanEntity>((object netId) => {
            return (LoopBeanEntity)this.getNetObject((int)( netId ));
        });
        this.persons = new List<object>((object[])data["persons"]).ConvertAll<PersonEntity>((object netId) => {
            return (PersonEntity)this.getNetObject((int)( netId ));
        });
        this.players = new List<object>((object[])data["players"]).ConvertAll<PlayerEntity>((object netId) => {
            return (PlayerEntity)this.getNetObject((int)( netId ));
        });

        this.bullets = new List<object>((object[])data["bullets"]).ConvertAll<BulletEntity>((object netId) => {
            return (BulletEntity)this.getNetObject((int)( netId ));
        });
        this.others = new List<object>((object[])data["others"]).ConvertAll<FightEntity>((object netId) => {
            return (FightEntity)this.getNetObject((int)( netId ));
        });

        
        //按照type顺序 进行初始化。 优先级 gemo->buff->actionBase->actionCommon->action->entity
        for(int i = 0, len = netObjectDatas.Count; i < len; i++) {
            int netId = (int)( netObjectDatas[i]["netId"] );
            this.getNetObject(netId).setData(netObjectDatas[i]);
        }
        this.birthGrids.createBirthGrids();
        ( (ClientRunTime)this ).localPlayer = (ClientPlayerEntity)this.getPlayer(( (ClientRunTime)this ).uid);
        //for(int i = 0, len = this.players.Count; i < len; i++) {
        //    ( (ClientPlayerEntity)this.players[i] ).build();
        //}

        //Biggo添加
        if(FightMain.isLocal)
            this.aiController = new AIController(this);


        for(int i = 0, len = this.players.Count; i < len; i++) {
            if(this.players[i] != ( (ClientRunTime)this ).localPlayer) {
                this.aiController.openAI(this.players[i], 1);
            }
        }
    }


    public virtual Dictionary<string, object> getData() {
        Dictionary<string, object> data = new Dictionary<string, object>();

        data["fightMode"] = this.mapData.fightMode;
        data["id"] = this.mapData.id;

        List<Dictionary<string, object>> entitys = new List<Dictionary<string, object>>();
        foreach(int key in this._netObjects.Keys) {
            entitys.Add(this._netObjects[key].getData());
        }
        entitys.Sort((obj1, obj2) => {
            return (int)obj1["netId"] - (int)obj2["netId"];
        });
        data["entitys"] = entitys.ToArray();
        data["netId"] = this.netId;
        data["randomSeed"] = this.random.seed;
        data["randomSeedNum"] = this.random.seedNum;
        data["birthBeanController"] = this.birthBeanController.netId;
        data["refereeController"] = this.refereeController.netId;
        data["delayCalls"] = this._delayCalls.netId;
        data["speedRate"] = this.speedRate;
        data["powerGain"] = this.powerGain;
        if(null != this.warnning) data["warnning"] = this.warnning.netId;
        data["beans"] = this.beans.ConvertAll<object>((LoopBeanEntity b) => { return b.netId; }).ToArray();
        data["persons"] = this.persons.ConvertAll<object>((PersonEntity p) => { return p.netId; }).ToArray();
        data["players"] = this.players.ConvertAll<object>((PlayerEntity p) => { return p.netId; }).ToArray();
        data["bullets"] = this.bullets.ConvertAll<object>((BulletEntity b) => { return b.netId; }).ToArray();
        data["others"] = this.others.ConvertAll<object>((FightEntity f) => { return f.netId; }).ToArray();
        data["killPlayerNum"] = this.killPlayerNum;
        if(null != this.mapData.playerInitData) {
            data["playerInitData"] = this.mapData.playerInitData.ToArray();
        }        
        return data;
    }

    public override void clear() {
        this.players.Clear();
        Utils.clearList(beans);
        Utils.clearList(persons);
        Utils.clearList(bullets);
        this._netObjects.Clear();
        this.random = null;
        base.clear();
    }
}
