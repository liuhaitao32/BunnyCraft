using UnityEngine;
using System.Collections;
using System;

public class Bean : FightObject {

	public BeanEntity beanEnity;

    public override FightEntity fightEntity {
        set {
            base.fightEntity = value;
            this.beanEnity = (BeanEntity)this._fightEntity;
        }
    }

    public override void reset() {
        base.reset();
        if(this.beanEnity.type == ConfigConstant.ENTITY_LOOP_BEAN) this.gameObject.SetActive(this._fightEntity.alived);
    }

    public override void init () {
		this._transform.SetParent(this.scene.beanLayer, false);
		base.init ();
		this.rotation = UnityEngine.Random.Range (0, 360);
        this._fightEntity.addListener(EventConstant.START, this.onBirthHandler);
        this._fightEntity.addListener(EventConstant.HIT, this.onHitHander);
        this._fightEntity.addListener(EventConstant.DEAD, (e) => {
            this.nextFrameCall(() => {
                this.clear();
            });
        });
        if(this.beanEnity.type == ConfigConstant.ENTITY_LOOP_BEAN) {
            if(3 == this.beanEnity.itemType) {
                this.scene.edgeHintController.addFightObject(0, this._fightEntity);
            }
        } else {
            float[] value = new float[4];
            value[0] = UnityEngine.Random.Range(0.2f, 0.6f) + this.clientRunTime.mapData.earthRadius;
            value[1] = value[0] + UnityEngine.Random.Range(0.4f, 2f);
            value[2] = this.clientRunTime.mapData.earthRadius - UnityEngine.Random.Range(0.05f, 0.3f);
            value[3] = this.clientRunTime.mapData.earthRadius;

            viewHeight = value[0];
            float time = UnityEngine.Random.Range(0.2f, 0.8f);
            this.tweenHeightValue(this.gameObject, value[0], value[1], value[2], value[3], time);

            float rotateRnd = UnityEngine.Random.Range(500f, 200f);
            
            LTDescr ltd = LeanTween.rotateY(Tools.FindChild2("go", this.gameObject), UnityEngine.Random.value > 0.5 ? rotateRnd : -rotateRnd, time);
            ltd.tweenType = LeanTweenType.easeOutSine;
        }


    }

    private void onHitHander(MainEvent e) {
        PlayerEntity player = (PlayerEntity)e.data;
        if(this._fightEntity.data.ContainsKey("hitRes")) {
            this.clientRunTime.addEffect(this._fightEntity.data["hitRes"].ToString(), player.position);
        }
        this.nextFrameCall(() => {
            this.gameObject.SetActive(this._fightEntity.alived);
        });
    }

    private void onBirthHandler(MainEvent e) {
        LeanTween.cancel(this.gameObject, true);
        this.changeMove(1, true);
        base.changeDir(1);
        this.gameObject.SetActive(true);
        LeanTween.scale(this.gameObject, new Vector3(1, 1, 1), 0.5f);
    }

    void Update() {
        //this.rotation += 5f;
    }

    protected override void changeDir(float rate = 1) {
        //base.changeDir(rate);
    }

    public override void onUpdate(float rate) {
        if(!this.isActiveAndEnabled) return;
        if(FightMain.isTest && this.beanEnity.viewData.oldPosition.equals(this.beanEnity.viewData.newPosition)) return;
        base.onUpdate(rate);
    }
}
