using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRunTime:Map {
	private int _netId = 0;
    /**
	 * 这个是系统同步，是所有人都要有！
	 */
    private Dictionary<string, List<Dictionary<string, object>>> _syncData = new Dictionary<string, List<Dictionary<string, object>>>();


	public NetWorkServer netWork = new NetWorkServer ();

    private float startTime;

    public ServerRunTime () {
		this.netWork.server = this;

        this.interval = ConfigConstant.MAP_ACT_TIME_S;
	}
    

    public override void update() {
        float interval = ( Time.time - startTime ) * 1000;
        int pos = (int)( interval / ConfigConstant.MAP_ACT_TIME_S );

        for(int i = 0, len = pos - this.stepIndex; i < len; i++) {
            this.onUpdate();
        }


        //for(int i = 0, len = 1000000; i < len; i++) {
        //    interval++;
        //}
        
    }

    public override void finish() {
        base.finish();
        this.addNetAction(ConfigConstant.NET_C_STOP_MATCH, new Dictionary<string, object> { { "id", -1 } });
    }
    
    protected override void onMapUpdate() {
        base.onMapUpdate();
    }


	public override void init () {
        this.initMap();
        //这句话后端不用写。我这边为了调试用。
        if(this.mapData.isWatch) NetWorkServer.steps.Clear();
        this.startTime = Time.time;
    }

    

    protected override void onUpdate () {
		//base.onUpdate ();
        this.stepIndex++;
		this.sendPackage ();
		this.resetPackage ();
	}


	private void sendPackage(){
		for (int i = 0, len = this.players.Count; i < len; i++) {			
			List<Dictionary<string, object>> data = null;
			if (this._syncData.ContainsKey (this.players [i].uid)) {
				data = this._syncData [this.players [i].uid];
				this._syncData.Remove (this.players [i].uid);
			}
			//合并这两个数据出去！
			if (this._syncData.ContainsKey ("all")) {
				if (null == data) {
					data = this._syncData ["all"];
				} else {
                    data.AddRange(this._syncData["all"]);
				}
            }
            //if(null == data) data = new List<Dictionary<string, object>>();
            //data.Add(this.getData());
			this.netWork.sendClient (this.players[i].uid, data);
		}
	}

	public void resetPackage() {
		this._syncData.Remove ("all");
		//this._syncData.Clear ();
	}

	public void addNetAction(string type, object data, string uid = "all") {
        if(!this._syncData.ContainsKey(uid))
            this._syncData[uid] = new List<Dictionary<string, object>>();


		this._syncData [uid].Add (new Dictionary<string, object> {
            { "type", type },
            { "data", data }
        });
	}

    

    public override FightEntity createFightEntity (int type, int netId = -1) {
		FightEntity entity = null;
		switch (type) {
		case ConfigConstant.ENTITY_LOOP_BEAN://
			entity = new LoopBeanEntity (this);
			break;
		case ConfigConstant.ENTITY_PLAYER:
			entity = new PlayerEntity (this);
			break;
		case ConfigConstant.ENTITY_BULLET:
			entity = new BulletEntity(this);
			break;
        case ConfigConstant.ENTITY_PRICE_BEAN:
            entity = new PriceBeanEntity(this);
            break;
        case ConfigConstant.ENTITY_CALL:
            entity = new CallEntity(this);
            break;
        case ConfigConstant.ENTITY_BARRIER:
            entity = new BarrierEntity(this);
            break;
        case ConfigConstant.ENTITY_RADISH:
            entity = new RadishEntity(this);
            break;
        }
		return entity;
	}


    public void nextFrame() {
        this.onMapUpdate();
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["stepIndex"] = this.stepIndex;
        return data;
    }


}
