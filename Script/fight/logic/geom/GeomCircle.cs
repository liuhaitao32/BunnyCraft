using System;
using System.Collections.Generic;

public class GeomCircle:GeomBase {
	

	public GeomCircle(Map map, int netId = -1):base(map, netId) {
		
	}

    override public void parseData(object[] datas, object[] pivot = null) {
        this._radius = (int)( datas[0] );		
		this.init(new List<Vector2D>(), Vector2D.createVector3(pivot));
	}

    override public bool containsPoint(Vector2D p) {		
		return this.center.dist(p) <= this._radius;
	}


    public override int type { get { return ConfigConstant.GEOM_CIRCLE; } }
    
}

