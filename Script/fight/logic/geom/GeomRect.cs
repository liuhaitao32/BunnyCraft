using System;
using System.Collections.Generic;
using UnityEngine;

public class GeomRect : GeomBase {

    public Vector2D axisX = Vector2D.createVector();
    public Vector2D axisY = Vector2D.createVector();

    private double _width;
    private double _height;

    private double _halfWidth;
    private double _halfHeight;

    public GeomRect(Map map, int netId = -1):base(map, netId) {

    }

    override public void parseData(object[] datas, object[] pivot = null) {
        this._width = Convert.ToDouble(datas[0]);
        this._height = Convert.ToDouble(datas[1]);
        this.axisX.normalize();
        this.axisY.normalize();
        this._halfWidth = this._width / 2;
        this._halfHeight = this._height / 2;
        this._radius = (int)Math.Max(this._halfWidth, this._halfHeight);
        this.init(this.createPoints(), Vector2D.createVector3(pivot));
    }

    public double x { get {return this.center.x - this._halfWidth;} }
    public double y { get {return this.center.y - this._halfHeight;} }
    public double right { get {return this.center.x + this._halfWidth;} }
    public double bottom { get {return this.center.y + this._halfHeight;} }

    override public void updatePoints(){
		this.axisX.angle = this.angle;
		this.axisY.angle = this.angle + Math.PI / 2;
		base.updatePoints();
	}
		
		
	public double getProjectionRadius(Vector2D axis) {
		double projectionAxisX = Math.Abs(axis.dotProd(this.axisX));
        double projectionAxisY = Math.Abs(axis.dotProd(this.axisY));
		return this._halfWidth * projectionAxisX + this._halfHeight * projectionAxisY;
	}

    private List<Vector2D> createPoints() {
		return new List<Vector2D> {
                                    Vector2D.createVector(-this._halfWidth, -this._halfHeight),
                                    Vector2D.createVector(this._halfWidth, -this._halfHeight),
                                    Vector2D.createVector(-this._halfWidth, this._halfHeight),
                                    Vector2D.createVector(this._halfWidth, this._halfHeight)
                                };
	}

    

    public override int type { get { return ConfigConstant.GEOM_RECT; } }


    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this.axisX = Vector2D.createVector4((Dictionary<string, object>)data["axisX"]);
        this.axisY = Vector2D.createVector4((Dictionary<string, object>)data["axisY"]);
        this._width = Convert.ToDouble(data["width"]);
        this._height = Convert.ToDouble(data["height"]);
        this._halfWidth = this._width / 2;
        this._halfHeight = this._height / 2;
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["axisX"] = this.axisX.getData();
        data["axisY"] = this.axisY.getData();
        data["width"] = this._width;
        data["height"] = this._height;
        return data;
    }

    public override void clear() {
        base.clear();
        this.axisX.clear();
        this.axisY.clear();
        this.axisX = null;
        this.axisY = null;
    }
}

