using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Vector2D:IClear {
	
	public double x;

    public double y;

    private bool _cleared = false;

	public Vector2D(double x = 0, double y = 0) {
        this.x = x;
        this.y = y;
	}

	public static Vector2D lerp (Vector2D v1, Vector2D v2, double rate) {		
		return v2.clone ().subtract (v1).multiply (rate).add (v1);
	}

	public void copy(Vector2D v){
		this.setXY (v.x, v.y);
	}

    public Vector2D precise(int digits = 0) {
        this.x = Utils.precise(this.x, digits);
        this.y = Utils.precise(this.y, digits);
        return this;
    }

    public Vector2D clone() {
		return createVector(this.x, this.y);
	}

	private static List<Vector2D> pool = new List<Vector2D>();



    public static Vector2D createVector3(object[] pos) {
        if(null == pos) return Vector2D.createVector();
        return Vector2D.createVector(Convert.ToSingle(pos[0]), Convert.ToSingle(pos[1]));
    }

    public static Vector2D createVector4(Dictionary<string, object> data) {
        return Vector2D.createVector(Convert.ToSingle(data["x"]), Convert.ToSingle(data["y"]));
    }

    public static bool usePool = false;

    public static Vector2D createVector(double x = 0, double y = 0) {
		Vector2D v = null;
		//if (usePool && pool.Count > 0) {
  //          int len = pool.Count - 1;
		//	v = pool [len];
		//	v.setXY (x, y);
		//	pool.RemoveAt (len);
  //          v.cleared = false;
		//} else {
			v = new Vector2D (x, y);
		//}
		return v;
	}


	public void clear() {
		//if (FightMain.checkError && pool.Contains (this)) {
		//	throw new Exception ("重复添加到池里！");
		//}
  //      this._cleared = true;
		//if(usePool) pool.Add (this);
	}

    public static Vector2D createVector2(double radius, double angle) {
		Vector2D v2 = Vector2D.createVector(radius);
        v2.angle = angle;
        return v2;
    }

    public Vector2D zero() {
		this.x = 0;
		this.y = 0;
		return this;
	}

	public bool isZero {
		get { return this.x == 0 && this.y == 0; }
	}



	public double length {
		get {
			return Math.Sqrt(this.lengthSQ);
		}
		set {
			double a = this.angle;
			this.x = Math.Cos(a) * value;
			this.y = Math.Sin(a) * value;
		}


	}

	public double lengthSQ {
		get{return this.x * this.x + this.y * this.y;}

	}
    

	public double angle {
		set {
			double len = this.length;
			this.x = Math.Cos(value) * len;
			this.y = Math.Sin(value) * len;
		}
		get {
			return Math.Atan2(this.y, this.x);
		}
	}

	public Vector2D rotate(double value) {
		double newX = this.x * Math.Cos(value) - this.y * Math.Sin(value);
		double newY = this.x * Math.Sin(value) + this.y * Math.Cos(value);			
		return Vector2D.createVector(newX, newY);
	}



	/**
	 * 把这个向量变成单位向量 保留角度
	 * @return 当前类引用
	 */
	public Vector2D normalize() {
		double  len = this.length;
		if (0 == len) {
			this.x = 1;
			return this;
		}
		this.x /= len;
		this.y /= len;
		return this;
	}

	/**
	 * 判断长度是否为单位向量 1
	 * @return true 是 false 非
	 */
	public bool isNormalized() {
		return 1.0 == this.length;
	}

	public Vector2D truncate(double max) {
        if(this.length > max) this.length = max;
		return this;
	}

	public Vector2D reverse() {
		this.x = -this.x;
		this.y = -this.y;
		return this;
	}


	public double dotProd(Vector2D v2) {
		return this.x * v2.x + this.y * v2.y;
	}


	public static double angleBetween(Vector2D v1, Vector2D v2) {
		v1 = v1.clone().normalize();
		v2 = v2.clone().normalize();
		double result = Math.Acos(v1.dotProd(v2));
		v1.clear ();
		v2.clear ();
		return result;
	}

	/**
	 * 确定给出的向量是在右边还是在坐标
	 * 以当前向量方向上给出左右 只需判断是否与垂直同向就行！
	 * @param	v2 给出的向量
	 * @return -1 左边 +1 右边
	 */
	public int sign(Vector2D v2) {
		return this.perp.dotProd(v2) < 0 ? -1 : 1;
	}

	/**
	 * 返回垂直于当前向量的向量
	 */
	public Vector2D perp {
		get { return Vector2D.createVector(-this.y, x); }
	}

	/**
	 * 计算当前向量与给出向量的距离
	 * @param	v2 给出向量
	 * @return 根据参数返回与给出向量的距离
	 */
	public double dist(Vector2D v2) {
		return Math.Sqrt(this.distSQ(v2));
	}

	/**
		 * 计算当前向量与给出的向量距离的平方
		 * @param	v2 给出的向量
		 * @return 根据参数返回与给出向量的距离的平方
		 */
	public double distSQ(Vector2D v2) {
		double dx = v2.x - this.x;
		double dy = v2.y - this.y;
		return dx * dx + dy * dy;
	}

	/**
	 * 添加一个向量到当前向量
	 * @param	v2 给出向量
	 * @return 返回this
	 */
	public Vector2D add(Vector2D v2) {
		this.x += v2.x;
		this.y += v2.y;
		return this;
	}

	/**
	 * 当前向量减去给出向量 创建一个新的向量返回 不改变原向量
	 * @param	v2 给出向量
	 * @return 返回this
	 */
	public Vector2D subtract(Vector2D v2) {
		this.x -= v2.x;
		this.y -= v2.y;
		return this;
	}

	/**
	 * 当前向量乘以value值
	 * @param	value 
	 * @return 返回 this
	 */
	public Vector2D multiply(double value) {
		this.x *= value;
		this.y *= value;
		return this;
	}

	/**
	 * 当前向量除以value值
	 * @param	value 
	 * @return 返回 this
	 */
	public Vector2D divide(double value) {
		this.x /= value;
		this.y /= value;
		return this;
	}

	/**
	 * 判断当前向量与给出的向量是否相等
	 * @param	v2 给出的向量
	 * @return true 相等 false 不能
	 */
	public bool equals(Vector2D v2, double epsilon = 0.000001f) {
		return Math.Abs(x - v2.x) < epsilon && Math.Abs(y - v2.y) < epsilon;
	}
    

    /**
	 * toString方法
	 * @return 返回说明向量的字符串;
	 */
    public override string ToString() {
        return string.Format("x:{0}y:{1}len:{2}angle:{3}", this.x, this.y, this.length, this.angle);
        //return string.Format("x:{0:F2}y:{1:F2}len:{2:F2}angle:{3:F2}", this._x, this._y, this.length, this.angle);
    }

    /**
	 * 设置向量x y
	 * @param	x
	 * @param	y
	 */
    public void setXY(double x, double y) {
		this.x = x;
		this.y = y;
	}

	/**
	 * 投影到某向量上的长度。
	 * @param	axis
	 * @return
	 */
	public double projectionOn(Vector2D axis) {
		Vector2D v = axis.clone ().normalize();
		double result = this.dotProd(v);
		v.clear ();
		return result;
	}

	/**
	 * 投影到某向量上的然后返回此轴上的对应长度的向量。
	 * @param	axis
	 * @return 当前轴向量 大小与投影大小一致 方向与轴相同。
	 */
	public Vector2D projectionVector(Vector2D axis) {
		axis = axis.clone();
		axis.length = Math.Abs(this.projectionOn(axis));
		return axis;
	}


	/**
		 * 矢量叉积。
		 * @param	v
		 * @return 正数代表在右边 负数代表在右边。 0代表 180 与 0之间。
		 */
	public double crossProduct(Vector2D v) {
		return this.x * v.y - this.y * v.x;
	}

	public void  toFloor() {
        this.precise(0);
	}


    public bool cleared {
        get {
            return this._cleared;
        }
        set {
            this._cleared = value;
        }
    }

    public Dictionary<string, object> getData() { return new Dictionary<string, object> { { "x", this.x }, { "y", this.y } }; }

    public Dictionary<string, object> setData() {
        return new Dictionary<string, object> { { "x", this.x }, { "y", this.y } };
    }

    public static Vector2D UP = new Vector2D (0, 1);
	public static Vector2D DOWN = new Vector2D (0, -1);
	public static Vector2D RIGHT = new Vector2D (1, 0);
	public static Vector2D LEFT = new Vector2D (-1, 0);

	public static Vector2D UP_RIGHT = new Vector2D (1, 1);
	public static Vector2D UP_LEFT = new Vector2D (-1, 1);
	public static Vector2D DOWN_RIGHT = new Vector2D (1, -1);
	public static Vector2D DOWN_LEFT = new Vector2D (1, -1);
}
