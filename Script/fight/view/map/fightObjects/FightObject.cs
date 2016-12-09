using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FightObject : FightViewBase, IClientView {

	protected FightEntity _fightEntity;

    public Scene scene;

    public ClientRunTime clientRunTime;

    private List<Action> _nextCall = new List<Action>();    


    public float viewHeight = -1;

    protected float _cameraObjectOffsetY = 0;

    public virtual FightEntity fightEntity {
        set {
            this._fightEntity = value;
            this.clientRunTime = (ClientRunTime)this._fightEntity.map;
            this.scene = this.clientRunTime.scene;
        }
        get { return this._fightEntity; }
    }

    //整体控制受重力方向旋转
	protected Transform mainBody;

	public float scale {
		set{
//			GameObjectScaler.Scale (this.mainBody,value);
			this.mainBody.localScale = new Vector3(value, value, value);
		}
		get{
			return this.mainBody.localScale.z;
		}
	}

    public float rotation {
		set{
			this.mainBody.localRotation = Quaternion.Euler(0, 0, value);
        }
        get{
			return this.mainBody.localRotation.eulerAngles.z;

        }
	}

    ///缓动值到,表现抛高落地
	public GameObject tweenHeightValue(GameObject go, float valueStart, float value1, float value2, float valueEnd, float time) {
        LTDescr ltd;
        LTDescr ltd2;
        LTDescr ltd3;
        float scaleTemp;
        float rate = 0.5f;
        ltd = LeanTween.value(go, (float temp) =>
        {
            viewHeight = temp;
            scaleTemp = 1f + ( viewHeight - this.clientRunTime.mapData.earthRadius ) * rate;
            go.transform.localScale = new Vector3(scaleTemp, scaleTemp, scaleTemp);
        },
            valueStart,
            value1,
            time * 0.3f
        );
        ltd.tweenType = LeanTweenType.easeOutSine;
        ltd.onComplete = () => {
            ltd2 = LeanTween.value(go, (float temp) =>
            {
                viewHeight = temp;
                scaleTemp = 1f + ( viewHeight - this.clientRunTime.mapData.earthRadius ) * rate;
                go.transform.localScale = new Vector3(scaleTemp, scaleTemp, scaleTemp);
            },
                value1,
                value2,
                time * 0.5f
            );
            ltd2.tweenType = LeanTweenType.easeInOutSine;
            ltd2.onComplete = () => {
                ltd3 = LeanTween.value(go, (float temp) =>
                {
                    viewHeight = temp;
                    scaleTemp = 1f + ( viewHeight - this.clientRunTime.mapData.earthRadius ) * rate;
                    go.transform.localScale = new Vector3(scaleTemp, scaleTemp, scaleTemp);
                },
                    value2,
                    valueEnd,
                    time * 0.2f
                );
                ltd3.tweenType = LeanTweenType.easeInSine;
            };
        };

        return go;
    }


    public virtual void reset() {
        if(this.mainBody == null) this.mainBody = this._transform;
        this._fightEntity.setOldViewData();
        this.changeMove(1, true);
        this.changeDir();
    }

    // Use this for initialization
    void Start() {
        if(!this._fightEntity.cleared) {
            init();
        } else {
            Utils.clearObject(this);
        }
        
    }

    public Vector3 toViewPosition(Vector2D position) {
        return ViewUtils.logicToScene(position, this.viewHeight, this.clientRunTime.mapData);
    }

    /**
     * 视图在下一个服务端计算时间片的时候 该执行的操作。
     */
    public void nextFrameCall(Action callBack) {
        if(!this._nextCall.Contains(callBack)) this._nextCall.Add(callBack);
        //说明以前已经监听过了。 放弃吧。
        if(this._nextCall.Count > 1) return;
        this.clientRunTime.addListener(EventConstant.START, this.nextCallHandler);
    }

    private void nextCallHandler(MainEvent e) {
        this.clientRunTime.removeListener(EventConstant.START, this.nextCallHandler);
        for(int i = 0, len = this._nextCall.Count; i < len; i++) {
            this._nextCall[i].Invoke();
        }
        this._nextCall.Clear();
    }

    public static int aaa = 0;

    public virtual void init() {
        this.viewHeight = this.clientRunTime.mapData.earthRadius;
        this._fightEntity.viewData.view = this;
        this.reset();
        this.clientRunTime.registerClientView(this);
        this._cameraObjectOffsetY = ViewConstant.MAP_CAMERA_OBJECT_OFFSET_Y;
    }

    protected virtual void entryTween() {
        float scale = this.scale;
        this.scale = 0.2f;
        LeanTween.scale(this.mainBody.gameObject, new Vector3(scale, scale, scale), 0.3f);
    }



    // Update is called once per frame
    //	void FixedUpdate ()
    //	{
    //		this.FightUpdate ();
    //	}


    protected virtual void changeMove(float rate = 1, bool now = false) {
		Vector3 v3 = this._transform.localPosition;
        Vector3 targetPos = this._fightEntity.viewData.getPosition(rate);
        this._transform.localPosition = targetPos;
        //Debug.Log(fakeRate);
	}

	protected virtual void changeDir(float rate = 1) {
        Vector3 v = new Vector3(this.scene.transform.position.x, this._transform.position.y + this._cameraObjectOffsetY, this.scene.transform.position.z);
        this.mainBody.LookAt(v);
        this.mainBody.Rotate(0, 0, Convert.ToSingle(Math2.radianToAngle(this._fightEntity.viewData.getAngle(rate))));
        //Quaternion target = this.mainBody.transform.localRotation * Quaternion.Euler(0, 0, Convert.ToSingle(Math2.radianToAngle(this._fightEntity.viewData.getAngle(rate))));
        //this.mainBody.transform.localRotation = Quaternion.Lerp(this.mainBody.transform.localRotation, target, 1 - this.fakeRate);
    }

	public virtual void onUpdate(float rate) {		
		this.changeMove (rate);
		this.changeDir (rate);
    }

    public override void clear() {
        this.clientRunTime.removeClientView(this);
        base.clear();
    }
}

