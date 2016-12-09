using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// 障碍物
public class Barrier : FightObject {

	///被撞击动画
	private Animator _animator;

    public override void init() {
        base.init();
        this._animator = this.gameObject.GetComponent<Animator>();
        this._transform.SetParent(this.scene.barrierLayer, false);
        this._cameraObjectOffsetY = 0;
    }

    public override FightEntity fightEntity {
        get {
            return base.fightEntity;
        }

        set {
            base.fightEntity = value;
            this._fightEntity.addListener(EventConstant.HIT, (e) => {
                if(this._animator != null) {
                    this._animator.Play("hit", -1, 0);
                }
            });
        }
    }
}
