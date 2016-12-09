using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayerEntity:PlayerEntity, IClientFake {
    

    public Vector2D joystickAngle = Vector2D.createVector();

    public double angle2 = 0;

    private Vector2D _fakePosition = Vector2D.createVector();

	private Vector2D _fakeVelocity = Vector2D.createVector();

    private Vector2D _fakeForce = Vector2D.createVector();

    private Vector2D _fakeJoystickForce = Vector2D.createVector();

    private Vector2D _fakeSteeringForce = Vector2D.createVector();

    private double _fakeAngle = 0;

    private int _fakeNum = 0;

    private bool _fakePropertyChange = false;

    private List<Color> _myColors;


    ///上一帧角速度（仅限显示）
    private double _lastAngleSpeedOld;

    private double _lastAngleSpeed;


    public ClientPlayerEntity(Map map, int netId = -1):base(map, netId) {
        

    }

    public bool isLocalPlayer {
        get { return ( (ClientRunTime)this._map ).uid == this.uid; }
    }

    public override void initConfig(Dictionary<string, object> data = null) {
        base.initConfig(data);
        this.build();
    }

    public void regainFake() {
        this._fakeNum = 0;
        this._joystickForce.copy(this._fakeJoystickForce);
        this._position.copy(this._fakePosition);
        this._velocity.copy(this._fakeVelocity);
        this._force.copy(this._fakeForce);
        this._angle = this._fakeAngle;
        this.viewData.newAngle = this._angle;
        this.viewData.newPosition.copy(this._position);
        this._propertyChange = this._fakePropertyChange;
        this._steeringForce.copy(this._fakeSteeringForce);

        if(FightMain.equal) {
            Dictionary<string, object> equalInfo = ViewUtils.equal(this.ccc, this.getData(), "");
            if(equalInfo.Count > 0) {
                equalInfo = ViewUtils.equal(this.ccc, this.getData(), "");
                throw new Exception(ViewUtils.toString(equalInfo));
            }
        }
    }

    public override void setNewViewData() {
        base.setNewViewData();
        if(this.isLocalPlayer && !this._map.mapData.isWatch) this.viewData.newAngle = this.angle2;
    }


    protected double changeDir2() {
        //一点一点转向。
        if(this.joystickAngle.isZero) {
            //this.angle2 = this._angle;
            return 0;
        }
        double max = this.getProperty(ConfigConstant.PROPERTY_ASP);
        double angle2 = this.joystickAngle.angle;
        angle2 = Math2.range(Math2.deltaAngle(this.angle2, angle2) * this.getProperty(ConfigConstant.PROPERTY_ASP_RATE), max, -max);
        this.angle2 += angle2;
        return angle2;

    }

    public override void update () {
		base.update ();
	}

    public override void beHit(BulletEntity bullet, double damageRate = 1) {
        base.beHit(bullet, damageRate);
        bullet.dispatchEventWith(EventConstant.HIT, this);
    }

    protected override void revive() {
        base.revive();
        if(this.isLocalPlayer) {
            this.joystickAngle.copy(this._joystickForce);
            this.angle2 = this._angle;
            this.setOldViewData();
            this.setNewViewData();
        }
    }

    public override void killPlayer(PlayerEntity player) {
        base.killPlayer(player);
        this.dispatchEventWith(EventConstant.KILL_PLAYER, new List<ClientPlayerEntity> { this, (ClientPlayerEntity)player });
    }

    protected override double changeDir() {
        double intervalAngle = base.changeDir();

        if(this.isLocalPlayer) {
            intervalAngle = this.changeDir2();
        }
        intervalAngle /= 2;
        this._lastAngleSpeedOld = this._lastAngleSpeed;
        this._lastAngleSpeed += ( intervalAngle - this._lastAngleSpeed ) * ViewConstant.SHIP_ANGLE_SPEED_RATE;
        return intervalAngle;
    }

    public float getLastAngle(float rate) {
        return Convert.ToSingle(Math2.radianToAngle(Utils.transitionNum(this._lastAngleSpeedOld, this._lastAngleSpeed, rate)));
    }

    public override void addBuff(Buff buff) {
        base.addBuff(buff);
        this.dispatchEventWith(EventConstant.BUFF_CHANGE, buff);
    }

    public override void removeBuff(Buff buff) {
        base.removeBuff(buff);
        this.dispatchEventWith(EventConstant.BUFF_CHANGE, buff);
    }

    Dictionary<string, object> ccc;

    public void saveFake() {
        this.ccc = this.getData();
        this._fakeJoystickForce.copy(this._joystickForce);
        this._fakePosition.copy (this._position);		
		this._fakeVelocity.copy (this._velocity);
        this._fakeForce.copy(this._force);
        this._fakeAngle = this._angle;
        this._fakePropertyChange = this._propertyChange;
        this._propertyChange = false;
        this._fakeSteeringForce.copy(this._steeringForce);
	}

    private List<Vector2D> list = new List<Vector2D>();

	public void fakeUpdate() {
        this._fakeNum++;
        this.setOldViewData();
        this.move();
        this.changeDir();
        this.applyPosition();
        this.list.Add(this.joystickForce.clone());
	}


    protected override void generateGrids() {
        if(this._fakeNum > 0) return;
        base.generateGrids();
    }

    protected override void changeProperty() {
        if(this._fakeNum > 0) return;
        base.changeProperty();
    }

    protected override void checkRelive() {
        if(this._fakeNum > 0) return;
        base.checkRelive();
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this.angle2 = this.angle;
        this.joystickAngle.copy(this._joystickForce);
        this.build();
    }

    public override bool alived {
        get {
            return base.alived;
        }

        set {
            base.alived = value;
            this.dispatchEventWith(EventConstant.ALIVED);
        }
    }

    public Color getTeamColor(int index) {
        return this._myColors[index];
    }

    public int teamType { get { return this.isLocalPlayer ? 0 : ( (ClientRunTime)this._map ).teamIndex == this.teamIndex ? 1 : 2; } }

    public string shipColorId {
        get {
            return this._map.mapData.isTeam ?
                       ( (ClientRunTime)this._map ).teamIndex == this.teamIndex ?
                            "team1" :
                            "team2" :
                       this.shipId;
                }
    }

    public void build() {
        this._myColors = ViewConstant.teamColors[this.teamType];
    }
}

