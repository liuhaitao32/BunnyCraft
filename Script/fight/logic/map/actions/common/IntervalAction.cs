using System;
using System.Collections.Generic;

//间隔时间执行的行为。
public class IntervalAction:ActionBase {

    protected int _interval;

    protected int _totalTime = 0;

    protected int _cd = 0;

    protected Action _intervalHandler;

    public IntervalAction(Map map, int netId = -1):base(map, netId) {

    }

    public IntervalAction init (int interval = 0, int totalTime = int.MaxValue, Action intervalHandler = null) {
		this._interval = interval;
        this._totalTime = totalTime;
        this._intervalHandler = null != intervalHandler ? intervalHandler : this.onIntervalHandler;
        return this;
    }

    protected virtual void onIntervalHandler() {

    }

    public virtual void reset() {
        this._cd = 0;
    }

    public virtual void finish() {
        this._isFinish = true;
    }

    /**
     * cd中
     */
    public virtual bool cding { get { return this._cd > 0; } }

    public int cd { set { this._cd = value; } get { return this._cd; } }

    public override void update () {
        if(!this.cding && !this._isFinish) {
            this._intervalHandler();
            this._cd += this._interval;
        }
		this._cd -= ConfigConstant.MAP_ACT_TIME_S;
        this._totalTime -= ConfigConstant.MAP_ACT_TIME_S;
        this._isFinish = this._totalTime <= 0;
        if(this._isFinish) {
            this.finish();
        }
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this._interval = (int)(data["interval"]);
        this._totalTime = (int)(data["totalTime"]);
        this._cd = (int)(data["cd"]);

    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["interval"] = this._interval;
        data["totalTime"] = this._totalTime;
        data["cd"] = this._cd;
        return data;
    }

    override public int type { get { return ConfigConstant.INTERVAL_ACTION; } }

    public Action intervalHandler { set { this._intervalHandler = value; } }

    public int totalTime {
        get {return this._totalTime;}

        set {this._totalTime = value;}
    }

    public int interval {
        get {return this._interval;}

        set {this._interval = value;}
    }
}

