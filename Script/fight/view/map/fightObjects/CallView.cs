using System;
using UnityEngine;

public class CallView : Person {
	
	public CallView() {
		
	}

    public override FightEntity fightEntity {
        set {
            base.fightEntity = value;

            this._fightEntity.addListener(EventConstant.DEAD, (MainEvent e) => {
                Vector2D pos = this._fightEntity.position.clone();
                this._fightEntity.removeListeners(EventConstant.DEAD);
                this.nextFrameCall(() => {
                    if(null == e.data) {
                        //this.clientRunTime.addEffect("bangDead", pos);
                    }
                    pos.clear();
                    this.clear();
                });
            });
        }
    }

    public override void init() {
        base.init();
        GameObject go = ResFactory.getCacheEffect(this._fightEntity.data["resId"].ToString(), this.clientRunTime);
        //GameObject go = ResFactory.instance.bean2;
        go.transform.SetParent(skewBody.transform, false);
        //this.name = ( (CallEntity)this._fightEntity ).index.ToString();

    }
    

    public override void clear() {
        //别等destroy了！ 那个是延时执行的！太傻比！可能当前帧然后被抢走了？ call有待于测试。
        //if(this._isEnabled) {
        //    foreach(Transform item in Tools.FindChild2("personEarth/effect").transform) {
        //        ResFactory.removePool(item.gameObject);
        //    }
        //    foreach(Transform item in Tools.FindChild2("personEarth/personDirection/buff").transform) {
        //        ResFactory.removePool(item.gameObject);
        //    }
        //}
        base.clear();
    }
}

