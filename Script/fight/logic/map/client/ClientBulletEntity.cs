using System;
using System.Collections.Generic;

public class ClientBulletEntity:BulletEntity,IClientFake {

	private Vector2D _fakePosition = Vector2D.createVector();

	private Vector2D _fakeVelocity = Vector2D.createVector();

    private int _fakeSpeed = 0;

    private double _fakeAngle = 0;

    private int _fakeNum = 0;

    public ClientBulletEntity(Map map, int netId = -1):base(map, netId) {
		
	}
    


    public override void update () {
		base.update ();
	}
    

    public override void clear() {
        this.dispatchEventWith(EventConstant.DEAD);
        base.clear();
    }

    public void fakeUpdate() {
        this._fakeNum++;
        this.setOldViewData();
        this.accMove();
        this.changeDir();
        this.applyPosition();
    }

    Dictionary<string, object> ccc;

    public void regainFake() {
        this._fakeNum = 0;
        this._position.copy(this._fakePosition);
        this._velocity.copy(this._fakeVelocity);
        this._angle = this._fakeAngle;
        this._speed = this._fakeSpeed;
        if(FightMain.equal) {
            Dictionary<string, object> equalInfo = ViewUtils.equal(this.ccc, this.getData(), "");
            if(equalInfo.Count > 0) {
                throw new Exception(ViewUtils.toString(equalInfo));
            }
        }
    }
    

    public override void bomb() {
        this.dispatchEventWith(EventConstant.BOMB);
        base.bomb();
    }

    protected override void generateGrids() {
        if(this._fakeNum > 0) return;
        base.generateGrids();
    }

    public void saveFake() {
        if(FightMain.equal) {
            this.ccc = this.getData();
        }
        this._fakePosition.copy(this._position);
        this._fakeVelocity.copy(this._velocity);
        this._fakeAngle = this._angle;
        this._fakeSpeed = this._speed;
    }
}

