using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FollowObject : MonoBehaviour {
	public Vector3 offsetV3;
	public Transform aimTransform;

	public Camera mainCamera;

    void Start() {

    }

	// Update is called once per frame
	void LateUpdate () {
		this.transform.position = aimTransform.position + offsetV3;
		this.transform.rotation = this.mainCamera.transform.rotation;
	}
}
