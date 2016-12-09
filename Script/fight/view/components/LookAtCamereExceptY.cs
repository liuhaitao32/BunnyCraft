using UnityEngine;
using System.Collections;

///朝向摄像机,但Y轴不动
public class LookAtCamereExceptY : MonoBehaviour {
	Vector3 v3;

    private Transform mainCamera;
    private Transform _transform;

    void Start() {
        this.mainCamera = GameObject.Find("Camera").GetComponent<Camera>().transform;
        this._transform = this.transform;
    }

    void Update () {
		v3 = this.mainCamera.position;
		v3.y = 0;
		this._transform.LookAt (v3);
	}
}
