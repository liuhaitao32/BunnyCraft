using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PartView :IClear, IClientView {

    public PartAction partData;

    public string id;

    public int type;

    public GameObject gameObject;

    private Material _material;

    private Color _color = new Color();

    protected bool _cleared = false;


    private float _colorTempFloat;

    private ClientPlayerEntity _playerEntity;

    private ClientRunTime _map;

    public PartView(ClientRunTime map) {
        this._map = map;
        this._map.registerClientView(this);
    }
    

    public void init(string id, int type, PartAction partData, Transform parent, ClientPlayerEntity playerEntity, float scoreScale) {
        this.type = type;
        this._playerEntity = playerEntity;
        this._map = (ClientRunTime)this._playerEntity.map;
        //不同id 更换
        if(this.id != id) {
            this.id = id;
            if(null != this.gameObject) {
                LeanTween.cancel(this.gameObject);
                GameObject.Destroy(this.gameObject);
            }
            this.gameObject = ResFactory.getShip(id);
            //翅膀
            if(this.type == 1) {
                GameObject goLeft = ResFactory.createObject<GameObject>(this.gameObject);
                goLeft.name = "left";
                //右
                GameObject goRight = ResFactory.createObject<GameObject>(this.gameObject);
                goRight.transform.localScale = new Vector3(1, -1, 1);
                goRight.name = "right";

                this.gameObject = new GameObject();
                goLeft.transform.SetParent(this.gameObject.transform, false);
                goRight.transform.SetParent(this.gameObject.transform, false);
            } else {
                this.gameObject = ResFactory.createObject<GameObject>(this.gameObject);
            }
            this.gameObject.name = id;

            this.gameObject.transform.SetParent(parent, false);
            if(type == 5 - 1) {
                this.gameObject.transform.localPosition = new Vector3(0, 0, 1);
            } else {
                this.gameObject.transform.localPosition = new Vector3();
            }
            this._material = ViewUtils.getMaterial(this.gameObject, ViewConstant.SHIP_SHADER_NAME);
            ViewUtils.changeColor(this._material, (Dictionary<string, object>)ViewConstant.shipConfig[this._playerEntity.shipColorId]);
        }
        
        if(partData != this.partData) {
            this.resetPart(scoreScale, scoreScale, scoreScale);
        }
        //两个装备不等 直接重置装备。可以是同id替换

        
        if(null != partData && this.partData != partData) {
            //如果装备立刻启用的话 在一进入可能瞬间装备就生效了！
            if(partData.alived) {
                this.partStartHandler(null);
            } else {
                this._color = ViewUtils.cloneColor(ViewConstant.SHADER_COLOR_WARM);
                this._material.color = this._color;
                ViewUtils.setEnable(this.gameObject, false);
                /*************************启用用装备******************************/
                partData.addListener(EventConstant.ALIVED, partStartHandler);
            }

            /*************************受伤闪红******************************/
            partData.addListener(EventConstant.HURT, (e) => {
                ViewUtils.colorInOutMaterial(this.gameObject, this._material, ViewConstant.SHADER_COLOR_RED, ViewConstant.SHADER_COLOR_STANDARD, 0.05f, 0.3f);
            });
        }
        this.partData = partData;
    }


    private void partStartHandler(MainEvent e) {
        this._color = ViewConstant.SHADER_COLOR_STANDARD;
        this._material.color = this._color;
        ViewUtils.setEnable(this.gameObject, true);
        ViewUtils.colorInOutMaterial(this.gameObject, this._material, ViewConstant.SHADER_COLOR_YELLOW, ViewConstant.SHADER_COLOR_STANDARD, 0.05f, 0.2f);
    }

    public void onUpdate(float rate) {
        if(null != this.partData) {
            if(!this.partData.alived) {
                this._color.a = ( this.partData.preTimeAction.totalTime - this.partData.preTimeAction.time ) * 1f / this.partData.preTimeAction.totalTime * ViewConstant.SHADER_COLOR_WARM.a;
                this._material.color = this._color;
            } else {
                //判断是否闪红
                float hpRate = Convert.ToSingle(this.partData.hp / this.partData.hpMax);
                if(hpRate < 0.2f) {
                    //振频 强度
                    float temp = ( 0.2f - hpRate ) * 4 + 0.1f;
                    _colorTempFloat += temp;
                    //振幅偏移
                    float offset = ( Mathf.Sin(_colorTempFloat) + 1 ) / 2;
                    offset *= 1 + temp;
                    _color.r = ( offset * 0.3f + 1f + temp * 0.4f ) * ViewConstant.SHADER_COLOR_STANDARD.r;
                    _color.g = ( -offset * 0.2f + 1f - temp * 0.3f ) * ViewConstant.SHADER_COLOR_STANDARD.g;
                    _color.b = _color.g;
                    this._material.color = _color;
                }

            }
            
        }
    }


    public void resetPart(float scaleStart, float scaleMiddle, float scaleEnd) {
        float time = 0.5f;
        GameObject go = this.gameObject;
        float offsetStart;
        float offsetMiddle;
        float offsetEnd;
        float temp;

        if(this.type == 3) {
            //main放大,向下偏移
            if(go != null) {
                ViewUtils.tweenScale(go, scaleStart, scaleMiddle, scaleEnd, time);
                temp = -0.7f;
                offsetStart = ( scaleStart - 1f ) * temp;
                offsetMiddle = ( scaleMiddle - 1f ) * temp * 2f;
                offsetEnd = ( scaleEnd - 1f ) * temp;
                ViewUtils.tweenMove(go, new Vector3(0, 0, offsetStart), new Vector3(0, 0, offsetMiddle), new Vector3(0, 0, offsetEnd), time);
            }
        } else if(this.type == 4) {
            //TODO:兔子播动画。

        } else if(this.type == 0) {
            if(go != null) {
                temp = 0.6f;
                offsetStart = ( scaleStart - 1f ) * temp;
                offsetMiddle = ( scaleMiddle - 1f ) * temp + 1f;
                offsetEnd = ( scaleEnd - 1f ) * temp;
                ViewUtils.tweenScale(go, new Vector3(0.95f, 1.1f, 0.95f), new Vector3(1, 1, 1), 0.3f, 0.3f);
                ViewUtils.tweenMove(go, new Vector3(offsetStart, 0, 0), new Vector3(offsetMiddle, 0, 0), new Vector3(offsetEnd, 0, 0), time);
            }
        } else if(this.type == 1) {
            //wing内部位置偏移
            if(go != null) {
                GameObject goLeft = go.transform.FindChild("left").gameObject;
                GameObject goRight = go.transform.FindChild("right").gameObject;
                temp = 0.5f;
                offsetStart = ( scaleStart - 1f ) * temp;
                offsetMiddle = ( scaleMiddle - 1f ) * temp + 1f;
                offsetEnd = ( scaleEnd - 1f ) * temp;

                ViewUtils.tweenScale(go, new Vector3(1.1f, 0.95f, 0.95f), new Vector3(1, 1, 1), 0.3f, 0.3f);

                ViewUtils.tweenMove(goLeft, new Vector3(0, offsetStart, 0), new Vector3(0, offsetMiddle, 0), new Vector3(0, offsetEnd, 0), time);
                ViewUtils.tweenMove(goRight, new Vector3(0, -offsetStart, 0), new Vector3(0, -offsetMiddle, 0), new Vector3(0, -offsetEnd, 0), time);
            }
        } else if(this.type == 2) {
            //tail位置偏移
            if(go != null) {
                temp = -0.6f;
                offsetStart = ( scaleStart - 1f ) * temp;
                offsetMiddle = ( scaleMiddle - 1f ) * temp - 1f;
                offsetEnd = ( scaleEnd - 1f ) * temp;
                ViewUtils.tweenScale(go, new Vector3(0.95f, 1.1f, 0.95f), new Vector3(1, 1, 1), 0.3f, 0.3f);
                ViewUtils.tweenMove(go, new Vector3(offsetStart, 0, 0), new Vector3(offsetMiddle, 0, 0), new Vector3(offsetEnd, 0, 0), time);
            }
        } else if(this.type == 5) {
            go.transform.localPosition = new Vector3(-1.8f, 0, 0);
        }
    }


    public bool cleared {get { return this._cleared;}}

    public virtual void clear() {
        this._map.removeClientView(this);
        ResFactory.Destroy(this.gameObject);
        this._cleared = true;
    }
}
