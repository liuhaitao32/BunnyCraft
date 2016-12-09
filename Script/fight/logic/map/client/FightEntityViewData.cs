using System;
using UnityEngine;
using System.Collections.Generic;

public class FightEntityViewData {

	public Vector2D oldPosition = new Vector2D();
    public Vector2D newPosition = new Vector2D();
    

	public double oldAngle = 0;
    public double newAngle = 0;

    public FightObject view;

    private MapData _mapData;


    public FightEntityViewData(MapData mapData) {
        this._mapData = mapData;
    }

    public void saveOldData() {
        this.oldPosition.copy(this.newPosition);
		this.oldAngle = this.newAngle;
	}

	public Vector3 getPosition(double rate) {
        Vector2D v = this.getPositionV2(rate);
        Vector3 v3 = this.view.toViewPosition(v);
        v.clear();
        return v3;
    }

    public Vector2D getPositionV2(double rate) {
        CollisionInfo info = Collision.realPosition(this.oldPosition, this.newPosition, this._mapData);
        Vector2D v = Vector2D.lerp(info.pos1, info.pos2, rate);
        return v;
    }

    public double getAngle(double rate) {
        
        this.newAngle = this.oldAngle + Math2.deltaAngle(this.oldAngle, this.newAngle);

        return Utils.transitionNum(this.oldAngle, this.newAngle, rate);
    }
}