using System;
using System.Collections.Generic;

//定时会执行的行为。
public class TimeAction:ActionBase {

	private int _time;

	private int _totalTime;

	private Action _callBack;

    public int handlerId;

	public TimeAction(Map map, int netId = -1):base(map, netId) {
	}

    public TimeAction init(int time = int.MaxValue, Action callBack = null, int handlerId = 1) {
        this._time = this._totalTime = time;
        this._callBack = callBack;
        this.handlerId = handlerId;
        return this;
    }

	override public bool isFinish { get {return this._time == 0;}}

    public int time {
        set { this._time = Math.Max(0, value); }
        get { return this._time; }
    }

    public int totalTime {
        set { this._totalTime = value; }
        get { return this._totalTime; }
    }

    public Action callBack { set { this._callBack = value; } }

    override public void start() {
		base.start();
        this.checkFinish();
	}

    public virtual void reset() {
        this._time = this._totalTime;
    }

	override public void update() {
		base.update();
		this.time -= ConfigConstant.MAP_ACT_TIME_S;
        this.checkFinish();
	}

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this._time = (int)(data["time"]);
        this.handlerId = (int)(data["handlerId"]);
        this._totalTime = (int)(data["totalTime"]);
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["time"] = this._time;
        data["handlerId"] = this.handlerId;
        data["totalTime"] = this._totalTime;
        return data;
    }

    private void checkFinish() {
        if(this.isFinish && null != this._callBack) this._callBack.Invoke();
    }

    public override void clear() {
        this._callBack = null;
        base.clear();
    }

    override public int type { get { return ConfigConstant.TIME_ACTION; } }
}

