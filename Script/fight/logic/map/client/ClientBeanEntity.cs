using System;
using System.Collections.Generic;
using System.Collections;


public class ClientBeanEntity:LoopBeanEntity,IClientFake {

    private Vector2D _fakePosition = Vector2D.createVector();

    private Vector2D _fakeVelocity = Vector2D.createVector();

    private int _fakeSpeed = 0;

    private double _fakeAngle = 0;

    private int _fakeNum = 0;


    public ClientBeanEntity(Map map, int netId = -1):base(map, netId) {
		
	}

	public override void update () {
        base.update ();
	}
    

    public void fakeUpdate() {
        this._fakeNum++;
    }
    

    public void regainFake() {
        this._fakeNum = 0;
    }

    protected override void generateGrids() {
        if(this._fakeNum > 0) return;
        base.generateGrids();
    }

    public void saveFake() { 
    }

}

