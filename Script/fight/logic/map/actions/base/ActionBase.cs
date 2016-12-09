using System;
using System.Collections.Generic;

public class ActionBase:NetObjectBase {
		
	internal ActionManager _rootAction;
		
	/**
	* 完成标志
	*/
	protected bool _isFinish;
		
	/**
	* 父级的动作。 但是子类尽量不调用clear 因为netObject在clear的话 父级容器要是有此netId
	*/
	internal MultiAction _parentAction;	
		
	/**
	* 强制取消的标志
	*/
	protected bool _cannelFlag;
		
	/**
	* 开始的标志！
	*/
	protected bool _isStart;

    /**
     * 额外的数据 后端用不上 我前端用来临时保存一些东西用的。
     */
    public object viewData;

    public ActionBase(Map map, int netId = -1):base(map, netId) {

    }

	/**
	* 更新函数
	*/
	public virtual void update() {

	}

	/**
	* 开始执行
	*/
	public virtual void start() {
		this._isFinish = false;
		this._cannelFlag = false;
		this._isStart = true;
		this.dispatchEventWith(EventConstant.START);
	}


	/**
	* 是否结束
	*/
	public virtual bool isFinish {
		get{ 
			if (this._isFinish && !this._cannelFlag) {
				this._isStart = false;
				this.dispatchEventWith(EventConstant.COMPLETE);
			}
			return this._isFinish;
		}			
	}

	/**
	* 取消这个动作
	*/
	public virtual void cannel() {
		this._cannelFlag = true;
		this._isFinish = true;
		this._isStart = false;
		this.dispatchEventWith(EventConstant.CANCEL);
	}

	/**
	* 获取我当前的action
	* @return
	*/
	public virtual ActionBase currAction {
		get{ return this; }
	}

	public virtual ActionManager rootAction { get{ return this._rootAction; } }

	/**
	* 获取当前是否启动！
	*/
	public bool isStart { get{return this._isStart; }}


    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["isFinish"] = this._isFinish;
        data["cannelFlag"] = this._cannelFlag;
        data["isStart"] = this._isStart;
        if(null != this._rootAction) {
            data["rootAction"] = this._rootAction.netId;
        }
        if(null != this._parentAction) {
            data["parentAction"] = this._parentAction.netId;
        }
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this._isFinish = Convert.ToBoolean(data["isFinish"]);
        this._cannelFlag = Convert.ToBoolean(data["cannelFlag"]);
        this._isStart = Convert.ToBoolean(data["isStart"]);
        if(data.ContainsKey("rootAction")) {
            this._rootAction = (ActionManager)this._map.getNetObject((int)(data["rootAction"]));
        }
        if(data.ContainsKey("parentAction")) {
            this._parentAction = (MultiAction)this._map.getNetObject((int)(data["parentAction"]));
        }
    }

    public MultiAction parentAction  { get { return this._parentAction; } }
    

    /**
	* @inheritDoc
	*/
    public override void clear() {
		this._isFinish = true;
		this._rootAction = null;
		this._parentAction = null;
        base.clear();
	}
}

