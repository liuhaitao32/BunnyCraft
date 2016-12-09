using UnityEngine;
using System.Collections;

//子弹的外观配置
public class BulletView : MonoBehaviour {
	//朝向摄像机/朝向地球
	public bool lookAtCamera = false;
	//是否随速度旋转
	public bool rotationBySpeed = true;
	//是否继承材质
	public bool extendMaterial = false;
}
