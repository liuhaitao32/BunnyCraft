using UnityEngine;
using System.Collections;
using System;

public class BeamView : FightViewBase, IClientView {

	private BeamAction _beamAction;

    public Vector2 offset = new Vector2();

    private ClientRunTime clientRunTime;

    private FightObject _player;

    private F3DBeam _f3dBeam;

    private LineRenderer _lineRender;

    private GameObject _body;

    public BeamAction beamAction {
        set {
            this._beamAction = value;
            this._beamAction.addListener(EventConstant.DEAD, (e)=> {
                this.clear();
            });
            this.clientRunTime = this._beamAction.player.view.clientRunTime;
            this.clientRunTime.registerClientView(this);

            this.transform.localPosition = new Vector3(0, 0, 0);
            Transform childTransform = this.gameObject.transform.FindChild("go");
            this._f3dBeam = childTransform.GetComponent<F3DBeam>();
            this._lineRender = childTransform.GetComponent<LineRenderer>();
            this._body = childTransform.gameObject;
        }
    }
    

    void Start() {
        if(!this._beamAction.isFinish) this.init();
    }

    public void init () {
        

    }
    
    
	
	// Update is called once per frame
	

    public void onUpdate(float rate) {
        //base.onUpdate(rate);
        if(null != this._beamAction.target) {

            Vector2D v = Vector2D.createVector2(offset.magnitude, this._beamAction.player.viewData.getAngle(rate));
            this._transform.localPosition = new Vector3(Convert.ToSingle(v.x), Convert.ToSingle(v.y), 0);


            v.length = ConfigConstant.SHIP_RADIUS;
            Vector2D v1 = this._beamAction.player.viewData.getPositionV2(rate);
            Vector2D v2 = this._beamAction.target.viewData.getPositionV2(rate);

            //			float angle = Mathf.DeltaAngle(_person.angle,deltaV2d.angle);
            Vector2D deltaV2d = Collision.realPosition(v1.add(v), v2, this.clientRunTime.mapData).deltaPos;
            double angle = deltaV2d.angle;
            this._transform.localRotation = Quaternion.Euler(-30, 0, Convert.ToSingle(Math2.radianToAngle(angle)));
            v.clear();
            v1.clear();
            v2.clear();

            Vector3 deltaV3 = this._beamAction.target.view.transform.position - this._transform.position;
            float dis = deltaV3.magnitude;
            this._f3dBeam.MaxBeamLength = dis;
            float width = this._beamAction.hitCount * 0.1f + 0.3f;
            this._lineRender.SetWidth(width, width);

            this._body.SetActive(true);
        } else {
            this._body.SetActive(false);
        }
    }

    public override void clear() {
        this.clientRunTime.removeClientView(this);
        this.gameObject.SetActive(false);
    }

    void onDestroy(){
		
	}
}
