using System;
using System.Collections.Generic;

public class GeomSector:GeomBase {
    

    private GeomLine _line1;

    private GeomLine _line2;

    public int inRadius = 0;

    public GeomSector(Map map, int netId = -1):base(map, netId) {
        
    }
    

    override public void parseData(object[] datas, object[] pivot) {
        this._line1 = new GeomLine(this._map);
        this._line2 = new GeomLine(this._map);
        this._line1.entity = this._line2.entity = this.entity;
        double angle = Math2.angleToRadian(Convert.ToSingle(datas[0]));
        double angle2 = Math2.angleToRadian(Convert.ToSingle(datas[1]));
		this._radius = (int)(datas[2]);
			
		if (datas.Length > 3) this.inRadius = (int)(datas[3]);

        Vector2D v = Vector2D.createVector();
        List<Vector2D> vectors = new List<Vector2D>();
		vectors.Add(v);
		v = Vector2D.createVector(this._radius, 0);
        v.angle = angle;			
		vectors.Add(v);
		v = Vector2D.createVector(this._radius, 0);
        v.angle = angle2;
		vectors.Add(v);

       datas = new object[] {
                    new object[] { 0, 0 },//[x, y]
                    new object[] { this._radius, 0 }
              };

        this._line1.parseData(datas);
        this._line2.parseData(datas);
		this.init(vectors, Vector2D.createVector3(pivot));
    }


    private Vector2D point1 { get { return this.globalPoints[1]; } }
    private Vector2D point2 { get { return this.globalPoints[2]; } }




    public GeomLine line1 {
        get {
            this._line1.point2 = this.center;
            this._line1.point1 = this.point1;
            this._line1.updatePoints();
            return this._line1;
        }
    }

    public GeomLine line2 {
        get {
            this._line2.point2 = this.center;
            this._line2.point1 = this.point2;
            this._line2.updatePoints();
            return this._line2;
        }
    }

    public override int type { get { return ConfigConstant.GEOM_SECTOR; } }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        Dictionary<string, object> lineData = (Dictionary<string, object>)data["line1"];
        this._line1 = (GeomLine)this.entity.map.getNetObject((int)(lineData["netId"]));
        lineData = (Dictionary<string, object>)data["line2"];
        this._line1.entity = this.entity;
        this._line2 = (GeomLine)this.entity.map.getNetObject((int)(lineData["netId"]));
        this._line2.entity = this.entity;
        this.inRadius = (int)(data["inRadius"]);
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["line1"] = this._line1.getData();
        data["line2"] = this._line2.getData();
        data["inRadius"] = this.inRadius;
        return data;
    }

    public override void clear() {
        base.clear();
        Utils.clearObject(this._line1);
        Utils.clearObject(this._line2);
        this._line1 = null;
        this._line2 = null;
    }
}

