using UnityEngine;
using System.Collections;

///朝向地球的反面，排除Y
public class LookAtCamereOfEarth : MonoBehaviour {
	static readonly Vector3 BACK = new Vector3 (0, 180, 0);

	Vector3 v3;
    private Transform _transform;
    void Awake () {
		v3 = new Vector3 ();
        this._transform = this.transform;
    }
	void Start () {
		LookUpdate ();
	}

	void Update () {
		LookUpdate ();
	}
	void LookUpdate () {
		v3.y = this._transform.position.y;
		this._transform.LookAt (v3);
		this._transform.Rotate (BACK);
//		this.transform.localRotation = quatBack;
	}
}
