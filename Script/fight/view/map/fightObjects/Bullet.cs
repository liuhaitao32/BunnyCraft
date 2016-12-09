using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Bullet : FightObject {
	
    public ClientBulletEntity bulletEntity;

    public string bulletName;

    /// <summary>
    /// 用于缓存bullet的 类。但是粒子要进行恢复。
    /// </summary>
    private List<ParticleReset> _particles;

    protected override void preInit() {
        base.preInit();
        this.mainBody = this.gameObject.transform;
        if(this.isNullBullet) {
            AutoDestroy auto = this.gameObject.GetComponent<AutoDestroy>();
            if(null != auto) {
                ResFactory.Destroy(auto);
            }
        } else {
            this._particles = new List<ParticleSystem>(this.GetComponentsInChildren<ParticleSystem>()).ConvertAll<ParticleReset>((p) => { return new ParticleReset(p); });
        }
        
    }
    /// <summary>
    /// 基类的不要了！ 不用清理！只有一个地方可以清理！
    /// </summary>
    void Start() {
        if(!this._fightEntity.cleared) {
            init();
        } else {
            this._transform.SetParent(this.scene.bulletLayer, false);
        }
    }

    void OnEnable() {
        AutoDestroy auto = this.gameObject.GetComponent<AutoDestroy>();
        if(null != auto) {
            auto.enabled = false;
        }
    }

    public override FightEntity fightEntity {
        set {
            base.fightEntity = value;
            this.bulletEntity = (ClientBulletEntity)this._fightEntity;
            
            this.bulletEntity.addListener(EventConstant.HIT, (MainEvent e) => {
                string hitRes = this.bulletEntity.data.ContainsKey("hitRes") ? this.bulletEntity.data["hitRes"].ToString() : null;
                if(hitRes != null) {
                    PersonEntity target = e.data as PersonEntity;
                    CollisionInfo info = Collision.realPosition(this._fightEntity.position, target.position, this.clientRunTime.mapData);
                    Vector2D v = Vector2D.lerp(info.pos1, info.pos2, 0.5f);
                    this.clientRunTime.addEffect(hitRes, v);
                }
            });

            this.bulletEntity.addListener(EventConstant.BOMB, (MainEvent e) => {
                //因为子弹下一刻会被清掉！所以记录下！
                string bangRes = this.bulletEntity.data.ContainsKey("bangRes") ? this.bulletEntity.data["bangRes"].ToString() : null;
                float bangScale = this.bulletEntity.data.ContainsKey("bangScale") ? Convert.ToSingle(this.bulletEntity.data["bangScale"]) : 1;
                Vector2D pos = this.bulletEntity.position.clone();
                //this.nextFrameCall(() => {
                    //this.DispatchEventWith(EventConstant.DEAD);
                    //爆炸特效                    
                    if(bangRes != null) {
                        this.clientRunTime.addEffect(bangRes, pos, bangScale);
                    }
                //});
            });

            this.bulletEntity.addListener(EventConstant.DEAD, (MainEvent e) => {
                this.nextFrameCall(() => {
                    Utils.clearObject(this);
                });
            });
        }
    }

    protected override void entryTween() {
        if(false) {
            base.entryTween();
        }        
    }

    public override void reset() {
        if(this.isNullBullet) return;
        base.reset();
    }

    private bool isNullBullet { get { return this._transform.localScale.x == 0; } }

    public override void init () {
        //空子弹可以优化的 不用加到client数组里！
        if(this.isNullBullet) {
            this._transform.SetParent(this.scene.bulletLayer, false);
            return;
        }
        if(this._fightEntity.data.ContainsKey("scale")) this.scale = Convert.ToSingle(this._fightEntity.data["scale"]);
        base.init ();
        this._transform.SetParent(this.scene.bulletLayer, false);
        ViewUtils.changeColor(this.gameObject, ViewConstant.SHIP_SHADER_NAME, (Dictionary<string, object>)ViewConstant.shipConfig[((ClientPlayerEntity)this.bulletEntity.ownerPlayer).shipId]);
        for(int i = 0, len = this._particles.Count; i < len; i++) {
            this._particles[i].reset();
        }
        this.changeView(true);

        Color color = MaterialUtils.cloneColor(((ClientPlayerEntity)this.bulletEntity.ownerPlayer).getTeamColor(2));
        color.a = 0.5f;
        ViewUtils.changeColorMaterial(this.gameObject, ViewConstant.EFFECT_SHADER_NAME, color);
    }




	public override void onUpdate (float rate) {
        try {
            base.onUpdate(rate);
            if(1 != this.bulletEntity.scale) this.scale = Convert.ToSingle(this.bulletEntity.scale);
        } catch {
            throw new Exception("有问题的子弹是" + this.fightEntity.cleared + "  " + this.gameObject.name);
        }
		
    }
    

    void OnDestory() {
        this.removeListenerAll();
        //子弹直接通过销毁game来处理。 clear并不管！
    }

    private void changeView(bool visible) {
        if(null != this._particles) {
            if(visible) {
                for(int i = 0, len = this._particles.Count; i < len; i++) {
                    this._particles[i].reset();
                }
            } else {
                for(int i = 0, len = this._particles.Count; i < len; i++) {
                    this._particles[i].particle.loop = false;
                }
            }
        }
        foreach(MeshRenderer meshRenderer in this.GetComponentsInChildren<MeshRenderer>()) {
            meshRenderer.enabled = visible;
        }
    }

    //这个调用clear无效！子弹是走对象池的！
    public override void clear() {
        this.bulletEntity = null;
        this.clientRunTime.removeClientView(this);
        AutoDestroy auto = this.GetComponent<AutoDestroy>();
        if(!this._isEnabled) return;
        if(this.gameObject.activeSelf) {
            if(null != auto) {
                auto.enabled = true;
                this.changeView(false);
            } else {
                this.gameObject.SetActive(false);
            }
        } else {
            //就为了触发那个OnDisable 有可能因为追时间或者子弹瞬间消失
            this.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}

class ParticleReset {
    private bool _loop = false;

    public ParticleSystem particle;

    public ParticleReset(ParticleSystem p) {
        this.particle = p;
        this._loop = p.loop;
    }

    public void reset() {        
        this.particle.loop = this._loop;
    }


}

