using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClientRunTime:Map {

    public ClientPlayerEntity localPlayer;

	public Scene scene;

    private List<List<Dictionary<string, object>>> _steps = new List<List<Dictionary<string, object>>>();

    public string uid;

    public int teamIndex = 0;

    public int index = 0;


	public float startTime = 0;
    

    public List<IClientView> clientViews = new List<IClientView>();


    public int totalIndex = 0;

    /// <summary>
    /// 代表是否网络延迟 客户端自己向前惯性走。
    /// </summary>
    public int fakeCount = 0;

    public bool isStack = false;

	public int startStepIndex;

    public bool isComplete = false;

    public ClientRunTime () {
		this.interval = ConfigConstant.MAP_ACT_TIME_C;
        
	}

	public override void init () {
        this.scene.clientRunTime = this;
        if(FightMain.instance.isInitMap) {
            this.initMap();
            this.scene.init();
        }
        this.pause = true;

    }
    

    public void endFight() {
        this.scene.endFight();
        this.pause = true;
        this.dispatchEventWith(EventConstant.END);
        TimerManager.inst.Add(FightMain.fightEndWaitTime, 1, (e) => {
            if(FightMain.fightTest) {
                FightMain.instance.onStart();
            } else {
                FightMain.instance.clear();
            }                    
        });
    }

    protected override void warningTimeHandler() {
        base.warningTimeHandler();
        this.dispatchEventWith(EventConstant.WARNING);
        this.scene.warning();
    }
    
    public override void finish() {
        //TODO:等上线的时候要比对错误！
        if(false) this.equalPlayers();

        base.finish();
        this.scene.clearVoice();
        if(this.mapData.isChaos) {
            List<PlayerEntity> scoreRank = this.getSortPlayer(1);
            for(int i = 0, len = this.players.Count; i < len; i++) {
                PlayerEntity player = this.players[i];
                int scoreIndex = scoreRank.IndexOf(player);
                //前三名胜利动作。
                if(scoreIndex <= 2) ( (Player)player.view ).doAvatarAnimation("cheer");

                if(player == this.localPlayer) {
                    //根据名次播放声音
                    string voiceName;
                    switch(scoreIndex) {
                        case 0:
                            voiceName = "voiceFightEnd1";
                            break;
                        case 1:
                            voiceName = "voiceFightEnd2";
                            break;
                        case 2:
                            voiceName = "voiceFightEnd3";
                            break;
                        case 3:
                        case 4:
                            voiceName = "voiceFightEnd4";
                            break;
                        default:
                            voiceName = "voiceFightEnd5";
                            break;
                    }
                    this.scene.playVoice(voiceName, 0.5f);
                }
            }
        } else if(this.mapData.isTeam) {
            ///胜出的team
            int winnerTeam = this.refereeController.teamController.winnerTeam;
            if(winnerTeam >= 0) {
                for(int i = 0, len = this.refereeController.teamController.playerListArray[winnerTeam].Count; i < len; i++) {
                    PlayerEntity player = this.refereeController.teamController.playerListArray[winnerTeam][i];
                    ( (Player)player.view ).doAvatarAnimation("cheer");
                }

                this.scene.playVoice(this.localPlayer.teamIndex == winnerTeam ? "voiceWin" : "voiceLose", 0.5f);
            }
        }

        this.endFight();
    }


    public override void specialAction(ActionBase action) {
        if(action is BeamAction) {
            BeamAction beamAction = (BeamAction)action;
            GameObject go = ( (Player)beamAction.player.view ).addEffect(beamAction.skillData["resId"].ToString(), false);
            go.GetComponent<BeamView>().beamAction = beamAction;
        }
    }

    public void registerClientView(IClientView clientView) {
        if(this.clientViews.Contains(clientView)) {
            throw new Exception("重复添加！" + clientView);
        }
        this.clientViews.Add(clientView);
    }

    public void removeClientView(IClientView clientView) {
        if(0 == this.clientViews.Count) return;
        int index = this.clientViews.LastIndexOf(clientView);
        if(-1 != index) this.clientViews.RemoveAt(index);
    }

    

    public bool pause = false;
    internal bool notSetData = true;

    private float intervaldd = 0;

    private float netOffset = 0;

    protected override void onUpdate () {
        if(this.pause) return;
        //MediatorSystem.log("stepCount", this._steps.Count - this.stepIndex);

        float viewActTime = (Time.time - this.startTime ) * 1000;
        // 有新包的 并且之前有假的 那么要重新开始播。
        int startIndex = (this.isStack && this.fakeCount != 0 ? this.stepIndex : this.totalIndex );
        if(Time.deltaTime * 1000 > 40f || Time.deltaTime * 1000 < 25f) {
            //MediatorSystem.log("deltaTime", Time.deltaTime * 1000);
            intervaldd += Time.deltaTime * 1000 - 33f;
        }

        if(this.isStack) {
            this.isStack = false;
            float lastStepTime = ( this._steps.Count) * ConfigConstant.MAP_ACT_TIME_S - 66f;
            if(viewActTime < lastStepTime) {
                this.startTime -= ( lastStepTime - viewActTime ) / 1000;
                viewActTime = lastStepTime;
                Debug.Log("更新到服务器最新时间");
            }
        }
        viewActTime -= intervaldd;
        float cc = intervaldd * 0.05f;
        intervaldd -= cc;
        //MediatorSystem.log("intervaldd", intervaldd);

        float pos = viewActTime / ConfigConstant.MAP_ACT_TIME_S;
        //Debug.Log(( Time.time - this.startTime ) * 1000 + "  " + ( viewActTime / ConfigConstant.MAP_ACT_TIME_S ) + "   " + ( ( this._steps.Count ) * ConfigConstant.MAP_ACT_TIME_S - 66f ));
        MediatorSystem.timeStart("nextFrame");
        int len = (int)pos - startIndex;
        if(len > 0) {
            for(int i = 0; i < len; i++) {
                this.nextFrame();
            }
            if(this.fakeCount > 0) {
                this.netOffset += 2;                
                if(this.netOffset > 5) {
                    this.startTime += 66f / 1000;
                    this.netOffset = 0;
                    Debug.Log("延伸startTime时间");
                }
            } else {
                this.netOffset *= 0.95f;
            }
            this.dispatchEventWith(EventConstant.LOGIC_COMPLETE);
        }
        
        MediatorSystem.getRunTime("nextFrame", "num:" + ((int)pos - startIndex));
        this.totalIndex = (int)pos;

        if(!TestValue.test8) {
            MediatorSystem.timeStart("clientAtc");
            this.mapViewUpdate(pos - this.totalIndex);
            MediatorSystem.getRunTime("clientAtc");
        }

        if(this.isComplete) {
            this.finish();
        }
    }

    private void equalPlayers() {
        if(0 == NetAdapter.mapData.Count) return;
        //try {
            object[] sPlayers = (object[])NetAdapter.mapData[0];
            object[] cPlayers = this.players.ConvertAll<object>((PlayerEntity p) => {
                return new Dictionary<string, object> {
                { "netId", p.netId},
                { "position", p.position.getData()},
                { "angle", p.angle},
                { "hp", p.hp}
            };
            }).ToArray();

            Dictionary<string, object> equalInfo = ViewUtils.equal(cPlayers, sPlayers, "");
            NetAdapter.sendFightResult(equalInfo.Count != 0);
            FightMain.instance.onStart();
        //} catch(Exception) {
        //    Debug.Log("2323");
        //}
        
        this.pause = true;
    }

    private Dictionary<string, object> ccc;

    private void nextFrame() {
        if(this.stepIndex < this._steps.Count) {
            MediatorSystem.timeStart("serverAtc");
            this.dispatchEventWith(EventConstant.START);
            if(0 != this.fakeCount) this.regainFake();//先回复到正常的状态。

            object mapData = null;
            MediatorSystem.timeStart("parseData");
            if(null != this._steps[this.stepIndex]) {

                List<Dictionary<string, object>> steps = this._steps[this.stepIndex];
                for(int i = 0, len = steps.Count; i < len; i++) {
                    this.parseData(steps[i]);
                }
                //上线的时候 不要保存数据 所以就扔了吧。 只保留数组长度。
                if(UnityEngine.Random.value > 100) {
                    this._steps[this.stepIndex] = null;
                }
                //mapData = steps[steps.Count - 1];
            }
            MediatorSystem.getRunTime("parseData");

            MediatorSystem.timeStart("onMapUpdate");
            this.onMapUpdate();
            MediatorSystem.getRunTime("onMapUpdate");

            if(FightMain.equal) {
                //if(( this.startStepIndex + this.stepIndex - 1) % 500 == 0 && NetAdapter.mapData.Count > 0) {
                    //try {
                        //object mapData = NetAdapter.mapData[0];
                        //NetAdapter.mapData.RemoveAt(0);
                        Dictionary<string, object> equalInfo = ViewUtils.equal(this.getData(), mapData, "");
                        if(equalInfo.Count != 0) {
                            this.pause = true;
                            FightMain.instance.equalPanel.show(equalInfo);
                        }
                    //} catch(Exception e) {
                    //    Debug.Log("equal出错了");
                    //}
                }
            //}
			
            MediatorSystem.getRunTime("serverAtc");
        } else {
            if(0 == this.fakeCount) this.saveFake();//记录当前虚假运行的。
            this.fakeUpdate();
            MediatorSystem.log("fakeCount", this.stepIndex + "  " + this.fakeCount);
        }
    }

    private void regainFake() {
        Debug.Log("regainFake");
        for(int i = 0, len = this.persons.Count; i < len; i++) {
            ( (IClientFake)this.persons[i] ).regainFake();
        }

        for(int i = 0, len = this.bullets.Count; i < len; i++) {
            ( (IClientFake)this.bullets[i] ).regainFake();
        }

        for(int i = 0, len = this.beans.Count; i < len; i++) {
            ( (IClientFake)this.beans[i] ).regainFake();
        }
        this.dispatchEventWith(EventConstant.FAKE_REAGIAN);
        this.fakeCount = 0;
        if(FightMain.equal) {
            Dictionary<string, object> equalInfo = ViewUtils.equal(this.ccc, this.getData(), "");
            if(equalInfo.Count > 0) {
                string str = ViewUtils.toString(equalInfo);
                throw new Exception(str);
            }
        }
        
    }

    private void saveFake() {
        Debug.Log("saveFake");
        for(int i = 0, len = this.persons.Count; i < len; i++) {                
            ( (IClientFake)this.persons[i] ).saveFake();
        }

        for(int i = 0, len = this.bullets.Count; i < len; i++) {
            ( (IClientFake)this.bullets[i] ).saveFake();
        }
        for(int i = 0, len = this.beans.Count; i < len; i++) {
            ( (IClientFake)this.beans[i] ).saveFake();
        }
        if(FightMain.equal) {
            this.ccc = this.getData();
        }
    }

    private void fakeUpdate() {
        //if(this.mode == ConfigConstant.MODE_WATCH) return;
        for (int i = 0, len = this.persons.Count; i < len; i++) {
            //TODO:这里预估的 还有callEntity.
			((IClientFake)this.persons [i]).fakeUpdate ();
		}

		for (int i = 0, len = this.bullets.Count; i < len; i++) {
			((IClientFake)this.bullets [i]).fakeUpdate ();
		}
        for(int i = 0, len = this.beans.Count; i < len; i++) {
            ( (IClientFake)this.beans[i] ).fakeUpdate();
        }
        this.fakeCount++;
        if(this.fakeCount >= ConfigConstant.NETWORK_DELAY) {
            this.regainMap();
        }
	}

    public void regainMap() {
        this.pause = true;
        //TODO:替换新的动画。
        this.scene.setRegainMap(true);
        this._steps.Clear();
        this.stepIndex = 0;
        NetAdapter.init(() => {
            NetAdapter.sendStartMatch();
            this.scene.setRegainMap(false);
        });
    }
    
    private void mapViewUpdate(float rate) {
        for(int i = this.clientViews.Count - 1; i >= 0; i--) {
            this.clientViews[i].onUpdate(rate);
        }        
    }
    

	private void parseData(Dictionary<string, object> stepItem) {
        string type = stepItem["type"].ToString();
        object data = stepItem["data"];
		//Log.debug ("客户端执行" + type);
		switch (type) {
			case ConfigConstant.NET_C_INIT_MAP:
                this.setData((Dictionary<string, object>)data);
				break;
			case ConfigConstant.NET_C_CHANGE_MOVE:
				this.changeMove ((Dictionary<string, object>)data);
				break;
			case ConfigConstant.NET_C_CREATE_USER:
                this.createUser((Dictionary<string, object>)data);			
				break;
            case ConfigConstant.NET_C_USE_CARD:
                this.useCards((Dictionary<string, object>)data);
                break;
            case ConfigConstant.NET_C_STOP_MATCH:
                this.isComplete = true;
                FightMain.instance.id = ( (Dictionary<string, object>)data )["id"].ToString();
                break;
            default:
                Log.debug(type + "超出范围");
                break;
        }
	}

    public override PlayerEntity createUser(Dictionary<string, object> data) {
        PlayerEntity player = base.createUser(data);
        if(player.uid == this.uid) {
            this.localPlayer = player as ClientPlayerEntity;
            if(this == FightMain.instance.selection) FightMain.instance.DispatchEventWith(EventConstant.PLAYER_IN);
        }

        return player;
    }

    public void ccccc() {
        this.stepIndex = 0;
        this._steps.Clear();
        this.pause = true;
    }
    

    protected override void setData(Dictionary<string, object> data) {
        base.setData(data);
		this.startStepIndex = (int)data["stepIndex"];
        this.notSetData = false;
        this.scene.init();
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["stepIndex"] = this.startStepIndex + this.stepIndex;
        return data;
    }

    private void useCards(Dictionary<string, object> data) {
        this.getPlayer(data["uid"].ToString()).cardGroup.expendCard((int)(data["index"]));
    }

    private void changeMove(Dictionary<string, object> data) {
        string uid = data["uid"].ToString();
		PlayerEntity player = this.getPlayer (uid);
        if(player.hasBuff(ConfigConstant.BUFF_STUN)) return;

        Vector2D joystick = Vector2D.createVector (Utils.toFloat((int)data["axisX"]), Utils.toFloat((int)data["axisY"]));
		player.setJoystick(joystick);
		joystick.clear ();
	}
    

    public override FightEntity createFightEntity (int type, int netId = -1) {
		FightEntity entity = null;
		switch (type) {
			case ConfigConstant.ENTITY_LOOP_BEAN://
				entity = new ClientBeanEntity (this, netId);
				break;
			case ConfigConstant.ENTITY_PLAYER:
				entity = new ClientPlayerEntity (this, netId);
				break;
            case ConfigConstant.ENTITY_BULLET:
				entity = new ClientBulletEntity(this, netId);
				break;
            case ConfigConstant.ENTITY_PRICE_BEAN:
                entity = new ClientPriceBeanEntity(this, netId);
                break;
            case ConfigConstant.ENTITY_CALL:
                entity = new ClientCallEntity(this, netId);
                break;
            case ConfigConstant.ENTITY_BARRIER:
                entity = new ClientBarrierEntity(this, netId);
                break;
            case ConfigConstant.ENTITY_RADISH:
                entity = new ClientRadishEntity(this, netId);
                break;
        }
		return entity;
	}

    public FightObject createFightObject(FightEntity entity) {
        GameObject go = null;
        FightObject fightObject = null;
        switch(entity.type) {
            case ConfigConstant.ENTITY_LOOP_BEAN://
                go = ResFactory.instance.getBean(((LoopBeanEntity)entity).itemType);
                fightObject = go.GetComponent<Bean>();
                break;
            case ConfigConstant.ENTITY_PLAYER:
                if(((PlayerEntity)entity).uid == this.uid) {
                    go = ResFactory.createObject<GameObject>(ResFactory.instance.player);
                    fightObject = go.AddComponent<PlayerSelf>();
                } else {
                    go = ResFactory.createObject<GameObject>(ResFactory.instance.player);
                    fightObject = go.AddComponent<Enemy>();
                }
                break;
		    case ConfigConstant.ENTITY_BULLET:
                fightObject = ResFactory.getBullet(entity.data["resId"].ToString(), this);
                break;
            case ConfigConstant.ENTITY_PRICE_BEAN:
                go = ResFactory.instance.getBean(( (PriceBeanEntity)entity ).itemType);
                fightObject = go.GetComponent<Bean>();
                break;
            case ConfigConstant.ENTITY_CALL:
                go = ResFactory.createObject<GameObject>(ResFactory.instance.call);
                fightObject = go.GetComponent<CallView>();
                break;
            case ConfigConstant.ENTITY_BARRIER:
                go = ResFactory.loadPrefab(entity.data["resId"].ToString());
                go = ResFactory.createObject<GameObject>(go);
                fightObject = go.GetComponent<Barrier>();
                break;
            case ConfigConstant.ENTITY_RADISH:
                go = ResFactory.loadPrefab("radish");
                go = ResFactory.createObject<GameObject>(go);
                fightObject = go.AddComponent<Radish>();
                break;

        }
        entity.viewData.view = entity.view = fightObject;
        fightObject.fightEntity = entity;

        return fightObject;
    }

    public GameObject addEffect(string res, Vector2D position, float scale = 1) {
        return this.scene.addEffect(res, position);
    }

    
    public override void addEntity(FightEntity entity) {
        base.addEntity(entity);
        if(!entity.cleared) this.createFightObject(entity);
    }

    public void receive(List<Dictionary<string, object>> actions) {
        if(0 == this._steps.Count) {
            //this.startTime = Time.time + 33f;
            this.startTime = Time.time + 66f;
            this.pause = false;
            Debug.Log("初始化sync！");
        }
		this._steps.Add(actions);
        this.isStack = true;
	}


    ///视图方法，播放加时赛信息
    public void viewOverTime() {
        this.scene.playVoice("voiceOverTime");
    }
    
    public override void clear() {
        base.clear();
        this.scene.reset();
        Utils.clearList(this.clientViews);
    }

}
