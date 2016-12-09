using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerSelf : Player {
	
	public Text levelText;

    private long touchTime = -1;

    public override void init () {

        if(!this.clientRunTime.mapData.isWatch) this.addJoystick();
		base.init ();
        this.scene.cameraFollow.player = this;
        this._fightEntity.addListener(EventConstant.BUFF_CHANGE, (e) => {
            //TODO:这个要根据特效里面的配置 来判断放置在舞台 还是人物身上。目前全都放在人物身上。
            Buff buff = (Buff)e.data;
            if(buff.isFinish && buff.buffType == ConfigConstant.BUFF_STUN) {
                this._joyVector.zero();
            }
        });
    }
    


	//Biggo 修改
	public void sendMove() {
		//Debug.Log("11111" + this._playerEntity.hasBuff(ConfigConstant.BUFF_STUN));
		if(this._isChange) {
			NetAdapter.sendChangeMove(this._playerEntity.uid, this._joyVector.x, this._joyVector.y, this._playerEntity.netId);
			this.fakeJoystick (this._joyVector);

			this._isChange = false;
		}
	}

	//Biggo 添加
	public void fakeJoystick(Vector2D joyVector) {
		if (!(this._playerEntity.hasBuff (ConfigConstant.BUFF_STUN))) {
			((ClientPlayerEntity)this._playerEntity).joystickAngle.setXY (Utils.toFloat (Utils.toInt (joyVector.x)), Utils.toFloat (Utils.toInt (joyVector.y)));
			if (0 < this.clientRunTime.fakeCount) {
				((ClientPlayerEntity)this._playerEntity).joystickForce.copy (joyVector);
			}
		}
	}


    public override void onUpdate(float rate) {
        base.onUpdate(rate);
        
    }

    
	private Vector2D _joyVector = new Vector2D ();

	private bool _isChange = false;

	void OnJoystickMove(Vector2 v2)
	{
        if(this.clientRunTime.mapData.isWatch) return;
        if (this.clientRunTime != FightMain.instance.selection)
			return;
        this.touchTime = -1;
        Vector2D v = Vector2D.createVector(v2.x, v2.y);
        //把遥感倍率放大 可以更快的响应移动。
        v.length = Math2.range(Math.Sqrt(v.length) * 1.5, 1, 0);

        //if (Vector2D.angleBetween(v, this._joyVector) > Math2.angleToRadian(5f) || v.distSQ(this._joyVector) > 0.05f) {
        this._joyVector.copy(v);
            //this._joyVector.setXY(1, 0);
            this._isChange = true;
            this.sendMove();
        //}
        v.clear();
        
	}

    void OnTouchStart() {
        this.touchTime = System.DateTime.Now.Ticks;
        //		Debug.Log("start");
    }

    void OnTouchUp() {
        if(touchTime != -1) {
            //点击间隔小于200毫秒，认为是单击，停止摇杆
            long temp = System.DateTime.Now.Ticks - touchTime;
            if(temp <= 2000000) {
                //				Debug.Log(temp/2000000f);
                this._joyVector.multiply(0.01f);
            }
            this._isChange = true;
            this.sendMove();
        }
        //		Debug.Log("up");
    }

    protected void addJoystick() {        
        ETCJoystick.instance.activated = true;
        ETCJoystick.instance.onMove.AddListener(OnJoystickMove);
        ETCJoystick.instance.onTouchStart.AddListener(OnTouchStart);
        ETCJoystick.instance.onTouchUp.AddListener(OnTouchUp);
    }



    protected void removeJoystick() {
        ETCJoystick.instance.onMove.RemoveAllListeners();
    }

    public override void clear() {
        this.removeJoystick();
        base.clear();
    }




}
