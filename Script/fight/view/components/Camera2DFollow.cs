using System;
using UnityEngine;

public class Camera2DFollow : FightViewBase {
        
    public float positionSmooth = 0.1f;
    public float lookAtSmooth = 0.2f;
    

    private PlayerSelf _player;
    

    ///�������Ŀ��λ��(��ʱ)
    private Vector3 _aimCameraPosition;

    ///�ϴ�������۲��λ��
    private Vector3 _lookAtPosition;

    ///����ʱ��(����)
    private float _shakeTime;
    ///�����뾶
    private float _shakeRadius;

    ///�������µ�ƫ��
    private Vector3 _shakePosition;


    ///��Ҫ�����(����ƽ�� ����Ͻ���Ϊ���ú�������߶�-5�ϴ�ֵ)
    private Camera _mainCamera;
    ///Ŀ�꽹��
    private float _fieldRadius;
    /// ��Ч�����Ķ��뽹��
    private Transform _effectFocusTransform;


    
    ///���������(����ƽ�� ����Ϊ������߶�)
    private Camera _earthCamera;
    ///��������(����ƽ�� ��������)
    private Camera _skyCamera;
    ///Ѫ�������(����ƽ�� ���������һ��)
    private Camera _hpBarCamera;

    
    ///Ŀ�꽹����ٶ�
    private float fieldRadiusAcc;

    /// ��Ч�����Ķ��뽹��
    private Transform m_effectFocusTransform;
    ///��Ч�����Ķ���λ��
    private Vector3 _effectFocusPosition = new Vector3(0, 0, 32);


    public PlayerSelf player {
        set {
            this._player = value;
            if(null != this._player) this.reset();
        }
    }

    void Start() {
        this._mainCamera = this._transform.GetComponent<Camera>();
        
        this._earthCamera = this._transform.FindChild("CameraEarth").GetComponent<Camera>();        
        this._skyCamera = this._transform.FindChild("CameraSky").GetComponent<Camera>();
        this._hpBarCamera = this._transform.FindChild("CameraHpBar").GetComponent<Camera>();


        this._effectFocusTransform = this._transform.FindChild("CameraCanvasFocus");
        this._shakePosition = new Vector3();
    }


    public void reset() {
        this.followPlayerNow();
        this.endShake();

        float cameraRadius = this._player.clientRunTime.mapData.cameraRadius;
        this._mainCamera.farClipPlane = Mathf.Max(cameraRadius - 10f, 45f);
        this._earthCamera.farClipPlane = cameraRadius;
        this._hpBarCamera.farClipPlane = this._mainCamera.farClipPlane;
    }

    private void Update() {
        if(null == this._player) return;
        this.updateShake();
        this.followPlayer();
    }

    ///�������ʼ����
    public void shake(float radius, float time) {
        this._shakeTime = Math.Max(_shakeTime, time);
        this._shakeRadius = Math.Max(_shakeRadius, radius);
    }
    ///�������������
    public void endShake() {
        this._shakeTime = 0;
        this._shakeRadius = 0;
        this._shakePosition.Set(0, 0, 0);
    }
    ///���������
    public void updateShake() {
        if(this._shakeTime > 0) {
            this._shakeRadius *= 0.8f;
            Vector3 radiusV3 = new Vector3(this._shakeRadius, this._shakeRadius, this._shakeRadius);
            this._shakePosition = UnityEngine.Random.insideUnitSphere;
            this._shakePosition.Scale(radiusV3);
            this._shakeTime -= Time.deltaTime;

            if(this._shakeTime <= 0) this.endShake();
        }
    }
    ///������ң��ı����������ղ��λ�ú���ת
    private void followPlayer() {
        //������߶ȸ��������Ұ����
        float aimFieldRadius = Convert.ToSingle(this._player.playerEntity.getProperty(ConfigConstant.PROPERTY_RADAR, false)) * ViewConstant.RADAR_TO_CAMERA_FIELD;
        aimFieldRadius = Mathf.Clamp(aimFieldRadius, ViewConstant.CAMERA_FIELD_MIN, ViewConstant.CAMERA_FIELD_MAX);
        //���ı�� ��ԭ����
        if(aimFieldRadius != ViewConstant.CAMERA_FIELD) {
            this.fieldRadiusAcc = ( aimFieldRadius - this._fieldRadius ) * 0.05f;
        } else {
            this.fieldRadiusAcc = ((aimFieldRadius - this._fieldRadius) * 0.03f) * 0.1f + this.fieldRadiusAcc * 0.9f;
        }
        this._fieldRadius += fieldRadiusAcc;

        this.updeteFieldRadius();
        this.updeteAimPosition();

        Vector3 tempV3;
        tempV3 = Vector3.Lerp(this._transform.localPosition, this._aimCameraPosition, this.positionSmooth);
        tempV3 += this._shakePosition;
        this._transform.localPosition = tempV3;

        this._lookAtPosition = Vector3.Lerp(this._lookAtPosition, this._player.transform.position, this.lookAtSmooth);
        tempV3.Set(this._lookAtPosition.x, this._lookAtPosition.y, this._lookAtPosition.z);
        tempV3 += this._shakePosition;
        this._transform.LookAt(tempV3);
    }
    /// ����������ң�ʡȥ�л�����
    private void followPlayerNow() {
        this._fieldRadius = ViewConstant.CAMERA_FIELD;
        this.fieldRadiusAcc = 0;
        this.updeteFieldRadius();
        this.updeteAimPosition();

        this._lookAtPosition = this._player.transform.position;
        this._transform.localPosition = this._aimCameraPosition;
        this._transform.LookAt(this._lookAtPosition);
    }

    ///���µ�ǰ״���µ�Ŀ��������Ŀ���λ��
    private void updeteAimPosition() {
        this._aimCameraPosition = ViewUtils.logicToScene(this._player.playerEntity.position, this._player.clientRunTime.mapData.cameraRadius, this._player.clientRunTime.mapData);
        this._aimCameraPosition.y = this._player.transform.localPosition.y + ViewConstant.MAP_CAMERA_OFFSET_Y;
    }
    /// ��Ŀ�꽹��������������Ч����
    private void updeteFieldRadius() {
        this._mainCamera.fieldOfView = this._fieldRadius;
        this._effectFocusPosition.z = 58 - this._fieldRadius * 1.8f;
        this._earthCamera.fieldOfView = this._fieldRadius;
        this._skyCamera.fieldOfView = this._fieldRadius;
        this._hpBarCamera.fieldOfView = this._fieldRadius;
        this._effectFocusTransform.localPosition = this._effectFocusPosition;
    }

    public override void clear() {
        base.clear();
    }

}
