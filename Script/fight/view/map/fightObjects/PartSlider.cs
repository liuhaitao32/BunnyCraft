using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartSlider {

    private PartAction _partData;

    private Player _person;

    private Slider _slider;

    private int type;

    private Material _material;

    public void updatePart() {
        PartAction partData = this._person.playerEntity.partGroup.getPart(type);
        if(partData == this._partData) return;

        this._partData = partData;
        if(null == this._partData) return;


        Image fg = Tools.FindChild2("Fill Area/Fill", this._slider.gameObject).GetComponent<Image>();
        /*************************禁用装备******************************/
        if(this._partData.alived) {
            //this.changeImage("Fill Area/Fill", "hpFg10");
            fg.color = this._person.playerEntity.getTeamColor(3);
        } else {
            fg.color = ViewConstant.COLOR_READY;
            /*************************启用用装备******************************/
            this._partData.addListener(EventConstant.ALIVED, (e) => {
                fg.color = this._person.playerEntity.getTeamColor(3);
            });

        }
        Tools.FindChild2("Background", this._slider.gameObject).GetComponent<Image>().color = this._person.playerEntity.getTeamColor(1);
    }


    

    public PartSlider(Player person, int type) {
        this._person = person;


        GameObject go = Tools.FindChild2("personEarth/Canvas/hpSliderPart" + type, person.gameObject);
        go.SetActive(false);


        this._slider = go.GetComponent<Slider>();

        //TODO:每个队伍不同颜色血条
        ( (PlayerEntity)this._person.fightEntity ).partGroup.addListener(EventConstant.PART_CHANGE, (e) => {
            this.updatePart();
        });
        this.type = type;
        this.updatePart();
    }
    
    

    public void onUpdate() {
        if(null == this._partData) {
            this._slider.gameObject.SetActive(false);
        } else {
            this._slider.gameObject.SetActive(true);



            if(this._partData.alived) {
                this._slider.maxValue = Convert.ToSingle(this._partData.hpMax);
                this._slider.value = Convert.ToSingle(this._partData.hp);
            } else {
                //TODO:颜色也应该改变。
                this._slider.maxValue = this._partData.preTimeAction.totalTime;
                this._slider.value = this._partData.preTimeAction.totalTime - this._partData.preTimeAction.time;
            }

        }
    }
}
