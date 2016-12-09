using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Radish : FightObject {

	public ClientRadishEntity radishEntity;

    Color colorNormal;

    private Material material;
    private Text reliveText;
    Transform goTransform;

    protected override void preInit() {
        base.preInit();
        this.material = ViewUtils.getMaterial(this.gameObject, ViewConstant.DOUBLE_COLOR_SHADER_NAME);
        this.colorNormal = this.material.color;
        this.goTransform = this.transform.FindChild("go");
        this.reliveText = Tools.FindChild2("Canvas/reliveText", this.gameObject).GetComponent<Text>();
    }

    public override FightEntity fightEntity {
        set {
            base.fightEntity = value;
            this.radishEntity = (ClientRadishEntity)this._fightEntity;

            this.radishEntity.addListener(EventConstant.RELIVE, (e) => {
                this.reliveText.gameObject.SetActive(false);
                this.materialOn();
            });

            this.clientRunTime.addListener(EventConstant.LOGIC_COMPLETE, (e) => {
                if(!this.radishEntity.reliveTime.isFinish) {
                    this.reliveText.gameObject.SetActive(true);
                    this.reliveText.text = string.Format("{0:F1}", (float)this.radishEntity.reliveTime.time / 1000);
                }
            });

            this.radishEntity.addListener(EventConstant.GAIN_RADISH, (e) => {
                this.gameObject.SetActive(false);
                ClientPlayerEntity playerEntity = (ClientPlayerEntity)e.data;
                //如果是敌方，将远程曝光开启
                if(this.clientRunTime.teamIndex != playerEntity.teamIndex) {
                    this.scene.edgeHintController.ChangeEdgeHint(playerEntity, true);
                }

                Player view = ( (Player)playerEntity.view );
                view.doAvatarAnimation("poss");
                view.avatar.changeFollow("followRadish");
                this.scene.addRadishRadio(playerEntity, "getRadish");
            });
            GameObject go = Tools.FindChild2("go", this.gameObject);
            this.radishEntity.addListener(EventConstant.DROP_RADISH, (e) => {
                this.show();
                ClientPlayerEntity playerEntity = (ClientPlayerEntity)e.data;
                //如果是敌方，将远程曝光取消
                if(this.clientRunTime.teamIndex != playerEntity.teamIndex) {
                    this.scene.edgeHintController.ChangeEdgeHint(playerEntity, false);
                }

                float[] arr = new float[4];
                arr[0] = UnityEngine.Random.Range(0.2f, 0.6f) + this.clientRunTime.mapData.earthRadius;
                arr[1] = arr[0] + UnityEngine.Random.Range(1.5f, 1.5f);
                arr[2] = this.clientRunTime.mapData.earthRadius - UnityEngine.Random.Range(0.3f, 0.3f);
                arr[3] = this.clientRunTime.mapData.earthRadius;

                viewHeight = arr[0];
                float time = (this.radishEntity.reliveTime.time) * 1f / 1000;
                this.tweenHeightValue(this.gameObject, arr[0], arr[1], arr[2], arr[3], time);
                
                LeanTween.value(go, (c)=> {
                    go.transform.Rotate(10, 20, 30);
                }, 0, 0, time);
            });

        }
    }

    public override void reset() {
        base.reset();
        this.gameObject.SetActive(this.radishEntity.alived);
        if(!this.radishEntity.reliveTime.isFinish) {
            this.show();
        }
    }

    public override void init () {
		this._transform.SetParent(this.scene.beanLayer, false);
		base.init ();
        this.scene.edgeHintController.addFightObject(3, this.radishEntity);
    }



    /// 出现灰化版本
    private void show() {
        this.entryTween();
        AutoLookAt.LookLower(this.transform);
        this.materialOff();
        this.gameObject.SetActive(true);
    }

    /// 缓动到石头质感
    public void materialOff(float time = 0.5f) {
        ViewUtils.colorOutMaterial(this.gameObject, material, Color.black, time);
    }
    /// 缓动到正常值
    public void materialOn(float time = 0.5f) {
        ViewUtils.colorOutMaterial(this.gameObject, material, this.colorNormal, time);
        this.scene.addEffect("hitRadishReady", this.radishEntity.position);
    }

    public override void onUpdate(float rate) {
        if(!this.isActiveAndEnabled) return;
        base.onUpdate(rate);
    }
}
