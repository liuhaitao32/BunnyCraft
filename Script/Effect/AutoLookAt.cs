using UnityEngine;
using System.Collections;

///自动调整朝向的各种方法
public class AutoLookAt : MonoBehaviour
{
	public static readonly Vector3 BACK = new Vector3 (0, 180, 0);
	public static readonly float LOWER_OFFSET_Y = -12f;
	public static Vector3 helpV3 = new Vector3();

	///枚举的模式 看平行点 看平行下沉点 看摄像机所看方向 直接看摄像机镜头
	public enum LookAtTypes{Normal,Lower,Camera,CameraLens};

	public LookAtTypes lookAtType = LookAtTypes.Normal; 
	public bool isReverse = false; 
	public bool isUpdate = false;

	void OnEnable(){
		if(!isUpdate)
		{
			Look (this.transform, this.lookAtType, this.isReverse);
			this.enabled = false;
		}
	}
	void Update () {
		Look (this.transform, this.lookAtType, this.isReverse);
	}

	public static void Look (Transform transform,LookAtTypes lookAtType,bool isReverse) {
		switch (lookAtType) {
		case LookAtTypes.Normal:
			helpV3.y = transform.position.y;
			transform.LookAt (helpV3);
			break;
		case LookAtTypes.Lower:
			helpV3.y = transform.position.y + LOWER_OFFSET_Y;
			transform.LookAt (helpV3);
			break;
		case LookAtTypes.Camera:
			transform.rotation = Camera.main.transform.rotation;
			break;
		case LookAtTypes.CameraLens:
			transform.LookAt (Camera.main.transform);
			break;
		}

		if(isReverse)
			transform.Rotate (BACK);
	}

	public static void LookNormal (Transform transform) {
		helpV3.y = transform.position.y;
		transform.LookAt (helpV3);
	}
	public static void LookLower (Transform transform) {
		helpV3.y = transform.position.y + LOWER_OFFSET_Y;
		transform.LookAt (helpV3);
	}
	public static void LookCamera (Transform transform) {
		transform.rotation = Camera.main.transform.rotation;
	}
	public static void LookCameraLens (Transform transform) {
		transform.LookAt (Camera.main.transform);
	}

	public static void LookReverse (Transform transform) {
		transform.Rotate (BACK);
	}
}


