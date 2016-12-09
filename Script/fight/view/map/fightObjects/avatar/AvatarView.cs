using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AvatarView : FightViewBase, IClientView {

    protected float _lastSwingRadian = 0f;

    private PartView[] _gameObjects = new PartView[6];

    private Dictionary<string, object> _color;

    private ClientPlayerEntity _playerEntity;

    private Animator _avatarAnimator;

    public void updateView() {
        for(int i = 0, len = ConfigConstant.PART_MODEL_PREFIX.Length; i < len; i++) {
            PartAction partData = i >= ConfigConstant.PART_COUNT ? null : this._playerEntity.partGroup.getPart(i);

            string id = null != partData ?
                            partData.id.ToString() :
                            ConfigConstant.PART_MODEL_PREFIX[i] + ( i == len - 1 ? "000" : this._playerEntity.shipId.Substring(4));//avatar只有000
            this.createPart(id, i, partData);
        }
    }

    public void reset() {
        //最后一个是跟随的萝卜
        Utils.clearObject(this._gameObjects[this._gameObjects.Length - 1]);
        this._gameObjects[this._gameObjects.Length - 1] = null;
        this.updateView();
    }

    public void init(Transform parent, ClientPlayerEntity playerEntity) {
        this._transform.SetParent(parent, false);
        this._transform.localRotation = Quaternion.Euler(180, 0, 0);
        this._playerEntity = playerEntity;

        ( (ClientRunTime)this._playerEntity.map ).registerClientView(this);
        
        this._playerEntity.partGroup.addListener(EventConstant.PART_CHANGE, (e) => {
            this.updateView();
        });

    }
    

    ///兔子播放动画
    public void doAvatarAnimation(string key) {
        if(this._avatarAnimator != null) {
            this._avatarAnimator.Play(key);
        }
    }

    private void createPart(string id, int type, PartAction partData) {
        PartView result = this._gameObjects[type];
        if(result == null) result = new PartView((ClientRunTime)this._playerEntity.map);
        if(result.id == id && result.partData == partData) return;
        result.init(id, type, partData, this._transform, this._playerEntity, ( (Player)this._playerEntity.view ).mainScale);
        this._gameObjects[type] = result;
        if(4 == type && null == this._avatarAnimator) this._avatarAnimator = result.gameObject.transform.FindChild("go").GetComponent<Animator>();
    }

    public void changeFollow(string name) {
        this.createPart(name, this._gameObjects.Length - 1, null);
        this._gameObjects[this._gameObjects.Length - 1].resetPart(0, 0, 0);
    }

    public void onUpdate(float rate) {
        float temp = 1 / Mathf.Sqrt(((Player)this._playerEntity.view).mainScale);
        _lastSwingRadian += 0.15f * temp;


        float sinAngle = Mathf.Sin(this._lastSwingRadian);
        float cosAngle = Mathf.Cos(this._lastSwingRadian);
        this._transform.localRotation = Quaternion.Euler(( this._playerEntity.getLastAngle(rate) * 3f + sinAngle * 8f ) * temp + 180, cosAngle * 4f * temp, 0);
    }

    //"head", "wing", "tail", "main", "avatar"
    public void scaleShip(float scaleStart, float scaleMiddle, float scaleEnd) {
        for(int i = 0, len = ConfigConstant.PART_MODEL_PREFIX.Length; i < len; i++) {
            if(null != this._gameObjects[i]) this._gameObjects[i].resetPart(scaleStart, scaleMiddle, scaleEnd);
        }
    }
    
}

