using System;
using UnityEngine;

public class Camera2DFollow : FightViewBase {
        
    public float positionSmooth = 0.1f;
    public float lookAtSmooth = 0.2f;
    

    private PlayerSelf _player;
    

    ///摄像机的目标位置(临时)
    private Vector3 _aimCameraPosition;

    ///上次摄像机观察点位置
    private Vector3 _lookAtPosition;

    ///震屏时间(秒数)
    private float _shakeTime;
    ///震屏半径
    private float _shakeRadius;

    ///震屏导致的偏移
    private Vector3 _shakePosition;


    ///主要摄像机(剪裁平面 距离较近，为配置和摄像机高度-5较大值)
    private Camera _mainCamera;
    ///目标焦距
    private float _fieldRadius;
    /// 特效界面层的对齐焦点
    private Transform _effectFocusTransform;


    
    ///地球摄像机(剪裁平面 距离为摄像机高度)
    private Camera _earthCamera;
    ///天空摄像机(剪裁平面 距离无限)
    private Camera _skyCamera;
    ///血条摄像机(剪裁平面 与主摄像机一致)
    private Camera _hpBarCamera;

    
    ///目标焦距加速度
    private float fieldRadiusAcc;

    /// 特效界面层的对齐焦点
    private Transform m_effectFocusTransform;
    ///特效界面层的对齐位置
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

    ///摄像机开始震屏
    public void shake(float radius, float time) {
        this._shakeTime = Math.Max(_shakeTime, time);
        this._shakeRadius = Math.Max(_shakeRadius, radius);
    }
    ///摄像机结束震屏
    public void endShake() {
        this._shakeTime = 0;
        this._shakeRadius = 0;
        this._shakePosition.Set(0, 0, 0);
    }
    ///摄像机震屏
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
    ///看向玩家，改变摄像机和天空层的位置和旋转
    private void followPlayer() {
        //摄像机高度根据玩家视野调整
        float aimFieldRadius = Convert.ToSingle(this._player.playerEntity.getProperty(ConfigConstant.PROPERTY_RADAR, false)) * ViewConstant.RADAR_TO_CAMERA_FIELD;
        aimFieldRadius = Mathf.Clamp(aimFieldRadius, ViewConstant.CAMERA_FIELD_MIN, ViewConstant.CAMERA_FIELD_MAX);
        //当改变快 还原慢！
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
    /// 立即看向玩家，省去切换动画
    private void followPlayerNow() {
        this._fieldRadius = ViewConstant.CAMERA_FIELD;
        this.fieldRadiusAcc = 0;
        this.updeteFieldRadius();
        this.updeteAimPosition();

        this._lookAtPosition = this._player.transform.position;
        this._transform.localPosition = this._aimCameraPosition;
        this._transform.LookAt(this._lookAtPosition);
    }

    ///更新当前状况下的目标和摄像机目标点位置
    private void updeteAimPosition() {
        this._aimCameraPosition = ViewUtils.logicToScene(this._player.playerEntity.position, this._player.clientRunTime.mapData.cameraRadius, this._player.clientRunTime.mapData);
        this._aimCameraPosition.y = this._player.transform.localPosition.y + ViewConstant.MAP_CAMERA_OFFSET_Y;
    }
    /// 按目标焦距调整摄像机和特效焦点
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
