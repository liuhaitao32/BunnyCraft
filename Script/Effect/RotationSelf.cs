using UnityEngine;
using System.Collections;

public class RotationSelf : MonoBehaviour {
	public Vector3 rotationV3;

    private Transform _transform;

    void Awake() {
        this._transform = this.transform;
    }

	// Update is called once per frame
	void Update () {
		this._transform.Rotate (rotationV3*Time.deltaTime);
	}
}
