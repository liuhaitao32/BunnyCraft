using System;
using System.Collections.Generic;
using System.Collections;


public class ClientCallEntity : CallEntity, IClientFake {



    private Vector2D _fakePosition = Vector2D.createVector();

    private Vector2D _fakeVelocity = Vector2D.createVector();

    private Vector2D _fakeForce = Vector2D.createVector();

    private Vector2D _fakeJoystickForce = Vector2D.createVector();

    private double _fakeAngle = 0;

    private int _fakeNum = 0;

    public ClientCallEntity(Map map, int netId = -1):base(map, netId) {
		
	}



    public override void update () {
        base.update ();
	}

    public override void addBuff(Buff buff) {
        base.addBuff(buff);
        this.dispatchEventWith(EventConstant.BUFF_CHANGE, buff);
    }

    public override void removeBuff(Buff buff) {
        base.removeBuff(buff);
        this.dispatchEventWith(EventConstant.BUFF_CHANGE, buff);
    }

    public override void clear() {
        base.clear();
    }


    public void fakeUpdate() {
        this.setOldViewData();
        this.move();
        this.changeDir();
        this.applyPosition();
        this._fakeNum++;
    }

    public void regainFake() {
        this._joystickForce.copy(this._fakeJoystickForce);
        this._position.copy(this._fakePosition);
        this._velocity.copy(this._fakeVelocity);
        this._force.copy(this._fakeForce);
        this._angle = this._fakeAngle;
        this.viewData.newAngle = this._angle;
        this.viewData.newPosition.copy(this._position);
    }

    public void saveFake() {
        this._fakeJoystickForce.copy(this._joystickForce);
        this._fakePosition.copy(this._position);
        this._fakeVelocity.copy(this._velocity);
        this._fakeForce.copy(this._force);
        this._fakeAngle = this._angle;
        this._fakeNum = 0;
    }
}

