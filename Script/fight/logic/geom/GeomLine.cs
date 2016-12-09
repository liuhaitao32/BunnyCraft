using System;
using System.Collections.Generic;

public class GeomLine:GeomBase {
	

	public GeomLine(Map map, int netId = -1):base(map, netId) {
		
	}

    override public void parseData(object[] datas, object[] pivot = null) {
        this.init(new List<Vector2D> {
                                        Vector2D.createVector3((object[])datas[0]),
                                        Vector2D.createVector3((object[])datas[1])
                                      }, Vector2D.createVector3(pivot));

        this._radius = (int)(this.point2.dist(this.point1) / 2);
    }

    public override Vector2D center {
        get {
            Vector2D v = Vector2D.lerp(this.point1, this.point2, 0.5f);
            this._center.copy(v);
            v.clear();
            return this._center;
        }
    }


    /**
	 * 线段的第一个点
	 */
    public Vector2D point1 {
        get { return this._globalPoints[0]; }
        set { this.repeatPoint(value, 0); }
    }

    /**
     * 线段的第二个点
     */
    public Vector2D point2 {
        get { return this._globalPoints[1]; }
        set { this.repeatPoint(value, 1); }
    }
    
    

    /**
     * 检查点在此线段的方向。
     * @param	vector2D
     * @return 右侧是1 左侧是-1 在线段上的是0。
     */
    public int checkPoint(Vector2D v, double epsilon = 0.00001f) {
		//点积整数代表同向 复数代表反向 0代表垂直。
		//折线段的拐向判断方法可以直接由矢量叉积的性质推出。对于有公共端点的线段p0p1和p1p2，通过计算(p2 - p0) × (p1 - p0)的符号便可以确定折线段的拐向：
	    Vector2D v1 = this.point2.clone().subtract(this.point1);
        Vector2D v2 = v.clone().subtract(this.point1);
        int result = v2.sign(v1);
        v1.clear();
        v2.clear();
        return result;
    }
    

    /**
     * 获取当前线段 可以构成的向量。
     */
    public Vector2D vector2D { get { return this.point2.clone().subtract(this.point1); } }


    public override int type { get { return ConfigConstant.GEOM_LINE; } }
    
}

