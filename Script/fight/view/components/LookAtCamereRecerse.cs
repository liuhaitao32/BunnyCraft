using UnityEngine;
using System.Collections;

///朝向摄像机的反面
public class LookAtCamereRecerse : MonoBehaviour {
	static readonly Vector3 BACK = new Vector3 (0, 180, 0);
    private Transform mainCamera;
    private Transform _transform;
    void Start() {
        this.mainCamera = GameObject.Find("Camera").GetComponent<Camera>().transform;
        this._transform = this.transform;
    }

	// Update is called once per frame
	void Update () {
		this._transform.LookAt (this.mainCamera);
		this._transform.Rotate (BACK);
	}
}
