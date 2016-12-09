using UnityEngine;
using System.Collections;

///运行时缩放显示对象工具
///author Biggo
public class GameObjectScaler : MonoBehaviour {
    ///运行时缩放 显示对象及其内部所有粒子，scale >1 放大
    public static void Scale(GameObject gameObj, float scale) {
        //		ScaleParticles (gameObj, scale);
        gameObj.transform.localScale *= scale;
    }
    ///运行时缩放 显示对象所有子粒子，scale >1 放大
    public static void ScaleParticles(GameObject gameObj, float scale) {
        //不修改当前层粒子，所有子粒子仅修改发射数值，Hierarchy不修改，防止牵连其他
        ParticleSystem[] particles = gameObj.GetComponentsInChildren<ParticleSystem>(true);
        foreach(ParticleSystem p in particles) {
            if(p.scalingMode == ParticleSystemScalingMode.Shape) {
                p.startSize *= scale;
                p.startSpeed *= scale;
            } else if(p.scalingMode == ParticleSystemScalingMode.Local) {
                p.transform.localScale *= scale;
            }
        }
    }
}
