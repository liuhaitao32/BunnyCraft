using System;
using System.Collections.Generic;

public class GeomBase :NetObjectBase {
    
    /**
	 * 中心点 用于旋转用的锚点
	 */
    protected Vector2D _pivot ;

	/**
	 * 旋转角度
	 */
	public double angle = 0;

	/**
	 * 初始的时候 旋转角度是0的默认的点。
	 */
	protected List<Vector2D> _sourcePoints ;

	/**
	 * 旋转后的点。
	 */
	protected List<Vector2D> _globalPoints = new List<Vector2D>();

	/**
	 * 几何的全局位置。
	 */
	public Vector2D position = Vector2D.createVector();

    /**
     * 用这个 可以简化判断。 先用园距离判断 再用 点去判断 会高效很多。
     */
    protected int _radius = 0;

    /**
     * 为了vector2D的对象池。。。 所以只能有一个变量标示了。
     */
    protected Vector2D _center = Vector2D.createVector();

    private FightEntity _entity;

    public bool applyEntity = true;

    //——————————————————————————————————————————以下是方法——————————————————————————————————————————————————————

    public GeomBase(Map map, int netId = -1):base(map, netId) {

	}



    public virtual void parseData(object[] datas, object[] pivot = null) {			
			
	}

    protected void init(List<Vector2D> sourcePoints, Vector2D pivot = null) {
		if(null == pivot) pivot = Vector2D.createVector();
        this._pivot = pivot;
		this._sourcePoints = sourcePoints;
		for (int i = 0, len = sourcePoints.Count; i < len; i++) {
			this._globalPoints.Add(sourcePoints[i].clone());
		}

        this.updatePoints();
    }
    

    protected Vector2D changePos(Vector2D v) {
		if (this.angle != 0) v.angle += this.angle;
		return v.add(this.position);
	}

    public virtual void updatePoints() {
		for (int i = 0, len = this._sourcePoints.Count; i < len; i++) {
			Vector2D v = this._sourcePoints[i].clone().add(this._pivot);
			this._globalPoints[i].copy(this.changePos(v));
			v.clear ();
		}
	}

	/**
	 * 检测是否包含点。
	 * @param	p
	 */
	public virtual bool containsPoint(Vector2D p )  {			
		return false;
	}
    

    /**
	 * 获取或设置 几个图形的中点（锚点）
	 */
    public Vector2D pivot{
		get {return this._pivot;}
		set{this._pivot = value;}
	}


    /**
	 * 相对于pivot的本地坐标。
	 */
    public List<Vector2D> sourcePoints { set { this._sourcePoints = value; } }


	/**
	 * 替换某一个点
	 * @param	point
	 * @param	index
	 */
	public virtual void repeatPoint(Vector2D point, int index) {
        Vector2D old = this._sourcePoints[index];
        if(null != old && old != point) old.clear();
		this._sourcePoints[index] = point;
	}

    /**
		 * 获取全局位置的点集合。
		 */
    public List<Vector2D> globalPoints { get { return this._globalPoints; } }


    public int radius {
        set { this._radius = value; }
        get { return this._radius; }
    }

    public virtual Vector2D center {
        get {
            this._center.copy(this._pivot);
            return this.changePos(this._center);
        }
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["pivot"] = this._pivot.getData();
        data["angle"] = this.angle;
        data["sourcePoints"] = this._sourcePoints.ConvertAll<object>((Vector2D v) => { return v.getData(); }).ToArray();
        data["position"] = this.position.getData();
        data["radius"] = this._radius;
        data["center"] = this._center.getData();
        data["entity"] = this.entity.netId;
        data["applyEntity"] = this.applyEntity;
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this._pivot = Vector2D.createVector4((Dictionary<string, object>)data["pivot"]);
        this.angle = Convert.ToDouble(data["angle"]);
        this._radius = (int)(data["radius"]);
        this.position = Vector2D.createVector4((Dictionary<string, object>) data["position"] );
        this._center = Vector2D.createVector4((Dictionary<string, object>)data["center"]);
        this.entity = (FightEntity)this._map.getNetObject((int)(data["entity"]));
        this._radius = (int)( data["radius"] );
        this.applyEntity = Convert.ToBoolean(data["applyEntity"]);

        object[] sourceDatas = (object[])data["sourcePoints"];

        this._sourcePoints = new List<Vector2D>();
        for(int i = 0, len = sourceDatas.Length; i < len; i++) {
            Dictionary<string, object> pos = (Dictionary<string, object>)sourceDatas[i];
            Vector2D v = Vector2D.createVector4(pos);
            this._sourcePoints.Add(v);
            this._globalPoints.Add(v.clone());
        }
        this.updatePoints();

    }
    

    public FightEntity entity {
        get {return this._entity;}
        set {this._entity = value;}
    }
    


    public override void clear() {
        Utils.clearList(this._sourcePoints);
        Utils.clearList(this._globalPoints);
        this._pivot.clear();
        this._pivot = null;
        this.position.clear();
        this.position = null;
        this._center.clear();
        this._center = null;
        this.entity = null;
        base.clear();
    }
}

