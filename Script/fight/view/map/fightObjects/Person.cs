using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Person : FightObject {
    
    public Slider hpBar;
    
    //受个体移动方向倾斜
    protected Transform skewBody;

    public int team = 0;

    private PersonEntity _personEntity;

    protected float fakeRate = 1;

    protected override void preInit() {
        base.preInit();
        this.hpBar = Tools.FindChild2("personEarth/Canvas/hpSlider", this.gameObject).GetComponent<Slider>();
        this.mainBody = Tools.FindChild2("personEarth/personDirection", this.gameObject).transform;
        this.skewBody = Tools.FindChild2("personSkew", this.mainBody.gameObject).transform;
    }

    public override FightEntity fightEntity {
        set {
            base.fightEntity = value;
            this._personEntity = (PersonEntity)this._fightEntity;

        }
    }

    public GameObject addEffect(string effect, bool sync = true) {
        GameObject go = ResFactory.getCacheEffect(effect, this.clientRunTime);
        go.transform.SetParent(Tools.FindChild2(sync ? "personEarth/personDirection/buff" : "personEarth/effect", this.gameObject).transform, false);
        return go;
    }

    public override void reset() {
        base.reset();
        this.hpBar.value = this.hpBar.maxValue = Convert.ToSingle(this._personEntity.getProperty(ConfigConstant.PROPERTY_HP));
        this.gameObject.SetActive(this._fightEntity.alived);
        this.fakeRate = 1;
    }
    
    /// 人物还要进行一个补件 卡与不卡之间需要一个补件。
    protected override void changeMove(float rate = 1, bool now = false) {
        Vector3 v3 = this._transform.localPosition;
        Vector3 targetPos = this._fightEntity.viewData.getPosition(rate);
        this.fakeRate = Math.Min(this.fakeRate + 0.05f, 1f);
        this._transform.localPosition = Vector3.Lerp(this._transform.localPosition, targetPos, now ? 1f : this.fakeRate);
    }

    public override void init() {
        base.init();
        this._transform.SetParent(this.scene.personLayer, false);
        this._fightEntity.addListener(EventConstant.BUFF_CHANGE, (e) => {
            //TODO:这个要根据特效里面的配置 来判断放置在舞台 还是人物身上。目前全都放在人物身上。
            Buff buff = (Buff)e.data;
            //TODO:是否要下一帧判断？ 就看实际效果了。 
            if(buff.data.ContainsKey("effect")) {
                if(!buff.isFinish) {
                    buff.viewData = this.addEffect(buff.data["effect"].ToString());
                } else {
                    if(null != buff.viewData) {
                        //if(buff.data["effect"].ToString() == "fire018") {
                        //    Debug.Log("!!!!!!!!!!!!!!");
                        //}
                        ((GameObject)buff.viewData).SetActive(false);
                        buff.viewData = null;
                    }
                }
            }
        });

        this._fightEntity.addListener(EventConstant.START, (e) => {
            //TODO:这个要根据特效里面的配置 来判断放置在舞台 还是人物身上。目前全都放在人物身上。
            Buff buff = (Buff)e.data;
            //TODO:是否要下一帧判断？ 就看实际效果了。 
            if(buff.data.ContainsKey("effect")) {
                if(!buff.isFinish) {
                    buff.viewData = this.addEffect(buff.data["effect"].ToString());
                } else {
                    if(null != buff.viewData) {
                        //if(buff.data["effect"].ToString() == "fire018") {
                        //    Debug.Log("!!!!!!!!!!!!!!");
                        //}
                        ( (GameObject)buff.viewData ).SetActive(false);
                        buff.viewData = null;
                    }
                }
            }
        });

        this._fightEntity.addListener(EventConstant.SHIELD_HURT, (e) => {
            this.addEffect("hitArmor", false);
        });

        this.clientRunTime.addListener(EventConstant.FAKE_REAGIAN, onFakeReagain);
        Tools.FindChild2("Background", this.hpBar.gameObject).GetComponent<Image>().color = ( (ClientPlayerEntity)this._personEntity.ownerPlayer ).getTeamColor(1);
        Tools.FindChild2("Fill Area/Fill", this.hpBar.gameObject).GetComponent<Image>().color = ( (ClientPlayerEntity)this._personEntity.ownerPlayer ).getTeamColor(3);
    }


    protected void onFakeReagain(MainEvent e) {
        this.fakeRate = 0.2f;// Math.Min(this.clientRunTime.fakeCount * 1f / 30, 1f);
    }

    protected override void changeDir(float rate) {
        //TODO:先临时这么写一下吧。
        Vector3 v = new Vector3(this.scene.transform.position.x, this._transform.position.y + ViewConstant.MAP_CAMERA_OBJECT_OFFSET_Y, this.scene.transform.position.z);
        this._transform.LookAt(v);
        this.rotation = Convert.ToSingle(Math2.radianToAngle(this._fightEntity.viewData.getAngle(rate)));

        //this.warship.transform.localPosition = new Vector3();
        //this.warship.transform.position = new Vector3();

        //平滑的转上个角速度
        //_lastAngleSpeed = _lastAngleSpeed * 0.8f + angle2 * 0.2f;
        //warshipBody.transform.localRotation = Quaternion.Euler(_lastAngleSpeed * 3f, 0, 0);

    }

    public override void onUpdate(float rate) {
        base.onUpdate(rate);
        
        this.hpBar.value = this._personEntity.hp;
    }

    public override void clear() {
        this.clientRunTime.removeClientView(this);
        base.clear();
    }
}

