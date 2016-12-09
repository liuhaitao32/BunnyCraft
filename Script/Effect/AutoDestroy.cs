using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {
    ParticleSystem[] particleSystems;

    ///只是关闭活动，不删除gameObject
    public bool onlyDeactivate;
    ///移除时间，填0的话自动按粒子发射结束为标志
    public float destroyTime = 0f;
    ///缩放粒子发射后消失
    public bool scaleParticles = true;

    public void StopParticleSystems() {
        foreach(ParticleSystem particleSystem in particleSystems) {
            //新的粒子停止发射
            particleSystem.Stop();
        }
    }

    void Awake() {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    void Update() {
        if(scaleParticles) {
            GameObjectScaler.Scale(this.gameObject, 0.9f);
        }
    }
    void OnEnable() {
        //		this.enabled = true;
        if(destroyTime > 0) {
            this.Invoke("DestroySelf", destroyTime);
            //			StartCoroutine ("DestroyTime");
        } else {
            if(particleSystems.Length > 0) {
                StartCoroutine("CheckIfAlive");
            }
        }
    }
    

    void OnDisable() {
        this.CancelInvoke();
        StopAllCoroutines();
    }
    void DestroySelf() {
        if(onlyDeactivate) {
            this.gameObject.transform.localScale = new Vector3(1, 1, 1);
            this.gameObject.SetActive(false);
        }
        else {
            //			foreach (ParticleSystem particleSystem in particleSystems) {
            //				particleSystem.loop =true;
            //				particleSystem.Play ();
            //			}
            Destroy(this.gameObject);
        }
    }

    IEnumerator DestroyTime() {
        //		return ;

        yield return new WaitForSeconds(destroyTime);
        if(this.enabled)
            Destroy(this.gameObject);
    }

    IEnumerator CheckIfAlive() {
        while(true) {
            yield return new WaitForSeconds(0.2f);

            foreach(ParticleSystem particleSystem in particleSystems) {
                if(particleSystem) {
                    if(particleSystem.IsAlive(true)) {
                    } else {
                        //任意一个粒子消亡了
                        yield return new WaitForSeconds(0.1f);
                        if(onlyDeactivate)
                            this.gameObject.SetActive(false);
                        else
                            Destroy(this.gameObject);
                    }
                }
            }
        }
    }
    //	bool isAlive;
    //	while(true)
    //	{
    //		yield return new WaitForSeconds(0.2f);
    //		isAlive = false;
    //		foreach (ParticleSystem particleSystem in particleSystems) {
    //			if (!particleSystem.IsAlive (true)) {
    //				//有存活者，还不要删除
    //				isAlive = true;
    //				break;
    //			}
    //		}
    //		//全都不存活了，删除
    //		if (!isAlive) {
    //			Destroy (this.gameObject);
    //			//				yield break;
    //		}
    //	}

}
