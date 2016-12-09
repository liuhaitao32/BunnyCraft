using UnityEngine;
using System.Collections;

///朝向摄像机
public class LookAtCamera : MonoBehaviour {
    // Update is called once per frame

    private Transform mainCamera;
    private Transform _transform;

    void Start() {
        this.mainCamera = GameObject.Find("Camera").GetComponent<Camera>().transform;
        this._transform = this.transform;
    }

	void Update () {
		this._transform.LookAt (this.mainCamera);
	}
}
