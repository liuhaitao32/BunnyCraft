using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FightEntity:NetObjectBase{

    public FightObject view;

	public int velocityLimitMax = ConfigConstant.PLAYER_VELOCITY_MAX;


	protected Vector2D _position = Vector2D.createVector();

    protected Vector2D _force = Vector2D.createVector(0, 0);

    /**
	 * 用来检测碰撞的类。
	 */
    protected GeomBase _shape;
    

	public List<Grid> grids = new List<Grid>();

    public List<Grid> birthGrids = new List<Grid>();

    public string id = null;

    /**
     *  客户端保存的数据。
     */
    public FightEntityViewData viewData;

    /**
     * 当前战场的物体所属的拥有者。
     */
    public PlayerEntity ownerPlayer;

    protected double _angle = 0;

	protected Vector2D _velocity = Vector2D.createVector (0, 0);

    protected bool _alived = true;
    
    protected Dictionary<string, object> _data;

    protected double _scale = 1;

    public Vector2D _oldPosition = Vector2D.createVector(-100000, -100000);

    private int _birthAreaGrid;

    public Vector2D birthPosition;
    /// 这两个值都是用于临时变量。 不用传递。查找的时候的信息都在里面。
    public double findDist = 0;
    public Vector2D findDelta = Vector2D.createVector();

    public Dictionary<string, object> data {
		get{ return this._data;}
	}

    public virtual GeomBase shape { get { return this._shape; } }
    

    //—————————————————————————————————————————————————以下是方法—————————————————————————————————————————————————————————————————————————

    public FightEntity(Map map, int netId = -1):base(map, netId) {
        this.viewData = new FightEntityViewData(map.mapData);
    }
    

    public virtual void initConfig(Dictionary<string, object> data = null) {
        this._data = data;
        if(null != data && data.ContainsKey("velocityLimitMax")) this.velocityLimitMax = (int)(data["velocityLimitMax"]);
        if(null != data && data.ContainsKey("birthAreaGrid")) this._birthAreaGrid = (int)( data["birthAreaGrid"] );
        this._alived = true;
        this.setNewViewData();
        this.position = this._position;
        this.birthPosition = this._position.clone();
        this._map.addEntity(this);
    }


    public virtual void update() {
        this.setOldViewData();
        this.move();
        this.changeDir();
	}

	public virtual void setOldViewData() {
        this.viewData.saveOldData();
    }

	public virtual void setNewViewData() {
        this.viewData.newPosition.copy(this._position);
        this.viewData.newAngle = this._angle;
	}

    public virtual Vector2D position {
		set{
			if (value != this._position) {
				this._position.clear ();
			}
            this._position = value;
			this._position.toFloor();
            this.generateGrids ();
		}
		get{ return this._position;}
	}

	public virtual double angle {
		set{ this._angle = Utils.precise(value, 4); }
		get{ return this._angle; }
	}

	protected virtual void generateGrids() {
        if(!this._alived) {
            this.removeGrids();
            return;
        }
        if(this._oldPosition.equals(this._position)) return;
        this.removeGrids();
        this.grids = this._map.getGrids(this._shape, this.grids);

        for(int i = 0, len = this.grids.Count; i < len; i++) {
            this.grids[i].addEntity(this);
        }
        this._oldPosition.copy(this._position);
	}

	public void removeGrids() {
		for (int i = 0, len = this.grids.Count; i < len; i++) {
			this.grids [i].removeEntity (this);
		}
		this.grids.Clear ();
	}


	/**
	 * 当一切合力用过之后 就开始执行这句话！
	 */
	public virtual void applyPosition() {
        this.forceToVelocity();
        this._force.toFloor();
        this._velocity.truncate(this.velocityLimitMax);
        this.velocityToPosition();
        this._velocity.toFloor();
        this.position = this.limitPosition(this._position);
        this.angle = this._angle;
		this.setNewViewData ();
    }


    public void forceToVelocity() {
        Vector2D v = this._force.clone().multiply(ConfigConstant.PLAYER_FORCE_TO_SPEED_RATE);
        this._velocity.add(v);
        this._force.subtract(v);
        v.clear();
    }

    public Vector2D addForce(Vector2D v2d) {
        return this._force.add(v2d);
    }

    public Vector2D force {
        get { return this._force; }
        set {
            if(this._force != value) {
                this._force.clear();
            }
            this._force = value;
        }
    }


    /**
     * 人物需要复写这个 其他的在场景都不是直接变速的！
     */
    protected virtual void velocityToPosition() {
        this._position.add(this._velocity);
    }

    protected virtual double changeDir() {
        return 0;
    }

    protected virtual void move() {

    }

    public virtual Vector2D limitPosition(Vector2D pos) {
		while(pos.x < 0) pos.x += this._map.mapData.width;
		pos.x = pos.x % this._map.mapData.width;
		pos.y = Math2.range(pos.y, this._map.mapData.height, 0);
		return pos;
	}

	public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["velocityLimitMax"] = this.velocityLimitMax;
        data["position"] = this._position.getData();
        data["birthposition"] = this.birthPosition.getData();
        data["velocity"] = this._velocity.getData();
        data["shape"] = this._shape.netId;
        data["id"] = this.id;
        if(null != this.ownerPlayer) {
            data["ownerPlayer"] = this.ownerPlayer._netId;
        }        
        data["angle"] = this._angle;
        data["alived"] = this._alived;
        data["data"] = this._data;
        data["scale"] = this._scale;
        return data;
	}

	public override void setData(Dictionary<string, object> data){
        this._oldPosition.setXY(-1, -1);
        base.setData(data);
        this.velocityLimitMax = (int)(data["velocityLimitMax"]);
        this._position = Vector2D.createVector4((Dictionary<string, object>)data["position"]);
        this.birthPosition = Vector2D.createVector4((Dictionary<string, object>)data["birthposition"]);
        this._velocity = Vector2D.createVector4((Dictionary<string, object>)data["velocity"]);
        this._shape = (GeomBase)this._map.getNetObject((int)(data["shape"]));
        this._netId = (int)(data["netId"]);
        this.id = data["id"].ToString();
        if(data.ContainsKey("ownerPlayer")) {
            this.ownerPlayer = (PlayerEntity)this._map.getNetObject((int)(data["ownerPlayer"]));
        }       
        this._angle = Convert.ToDouble(data["angle"]);
        this._alived = Convert.ToBoolean(data["alived"]);
        this._data = (Dictionary<string, object>)data["data"];

        this.setNewViewData();
        this.generateGrids();//这之前 shape必须初始化完毕！
        if(this.view == null) {
            ( (ClientRunTime)this._map ).createFightObject(this);
        } else {
            this.view.reset();
        }
        this._scale = Convert.ToDouble(data["scale"]);
        if(this._data.ContainsKey("birthAreaGrid")) this._birthAreaGrid = (int)( this._data["birthAreaGrid"] );
    }


    public override void clear() {
        this._alived = false;
        this._map.removeEntity (this);
		this._position.clear ();
		this._velocity.clear ();
        this._data = null;
        this.ownerPlayer = null;
        this.removeGrids();
        this.id = null;
        this.grids = null;
        Utils.clearObject(this._shape);
        base.clear();
	}

    public TimeAction delayCall(int time, Action callBack, int handlerId) {
        return this._map.addDelayCall(time, callBack, handlerId);
    }


	public Vector2D velocity {
		get {return this._velocity;}
		set {
			if (this._velocity != value) {
				this._velocity.clear ();
			}
			this._velocity = value;
		}
	}
    

    public virtual bool alived {
        get { return this._alived; }
        set {this._alived = value;}
    }

    public Map map { get { return this._map; } }

    public double scale { get { return this._scale; } }


    public void removeBirthGrids() {
        for(int i = 0, len = this.birthGrids.Count; i < len; i++) {
            this.birthGrids[i].changeBirthEntity(this, -1);
        }
        this.birthGrids.Clear();
    }

    public virtual void generateBirthGrids(int extend = 0) {
        this.removeBirthGrids();
        int col = (int)this.birthPosition.x / ConfigConstant.MAP_GRID_SIZE;
        int row = Math.Min((int)this.birthPosition.y / ConfigConstant.MAP_GRID_SIZE, this._map.mapData.rowNum - 1);
        //try {
        this.birthGrids = Grid.getAreaGrids(this._map.birthGrids.getGrid(col, row), this._birthAreaGrid + extend, this._map, this.limitAreaLine);
        //} catch {
        //    Debug.Log(111);
        //}
        

        for(int i = 0, len = this.birthGrids.Count; i < len; i++) {
            this.birthGrids[i].changeBirthEntity(this, 1);
        }
    }
    /// <summary>
    /// 一维是限制的范围个数 二维限制范围 两个值 第一个是min 第二个是max 相当于一个钩子。只有在item type值是3的时候 复写。
    /// </summary>
    protected virtual List<List<int>> limitAreaLine { get { return new List<List<int>> { new List<int> { 0, this._map.mapData.rowNum - 1 } } ; } }


}
