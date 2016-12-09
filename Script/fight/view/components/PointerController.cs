using UnityEngine;
using System.Collections;
using System;

///摇杆指针显示
public class PointerController : MonoBehaviour,IClientView {
	Player player;
	Transform pointerRota;
	Transform pointerPos;
	Vector3 rotationV3;
	Vector3 positionV3;
    private ClientRunTime _clientRunTime;

	// Use this for initialization
	void Start () {
		pointerRota = this.transform.FindChild ("pointerRota");
		pointerPos = pointerRota.transform.FindChild ("pointerPos");
		rotationV3 = pointerRota.localRotation.eulerAngles;
		positionV3 = pointerPos.localPosition;
	}

    public void init(ClientRunTime clientRunTime) {
        this._clientRunTime = clientRunTime;
        this._clientRunTime.registerClientView(this);
    }
    

    public void onUpdate(float rate) {
        ClientPlayerEntity player = this._clientRunTime.localPlayer;

        if(null != player) {
            if(player.alived) {
                this.showPointer();

                if(!player.joystickAngle.isZero) rotationV3.z = Mathf.LerpAngle(rotationV3.z, Convert.ToSingle(Math2.radianToAngle(player.joystickAngle.angle)), 0.5f);
                positionV3.x = Mathf.Lerp(positionV3.x, Convert.ToSingle(player.steeringForce.length) * 0.025f + Mathf.Sqrt(( (Player)player.view ).mainScale) * 0.9f + 0.08f, 0.5f);

                pointerRota.localRotation = Quaternion.Euler(rotationV3);
                pointerPos.localPosition = positionV3;
            } else {
                this.hidePointer();
            }
        } else {
            hidePointer();
        }
    }
    

	public void hidePointer () {
		pointerRota.gameObject.SetActive(false);
//		this.gameObject.SetActive(false);
	}
	public void showPointer () {
		pointerRota.gameObject.SetActive(true);
//		this.gameObject.SetActive(true);
	}
}
