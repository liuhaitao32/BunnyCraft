using System;
using System.Collections.Generic;

//定时会执行的行为。
public class HandlerAction:ActionBase {

    public int handlerId = -1;

    public Action handler;

	public HandlerAction(Map map, int netId = -1):base(map, netId) {
        
	}

    public HandlerAction init(Action handler, int handlerId) {
        this.handler = handler;
        this.handlerId = handlerId;
        return this;
    }

	override public void start() {
		base.start();
        this.handler.Invoke();
        this._isFinish = true;
	}

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this.handlerId = (int)(data["handlerId"]);
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["handlerId"] = this.handlerId;
        return data;
    }

    public override int type { get { return ConfigConstant.HANDLER_ACTION; } }

    public override void clear() {
        base.clear();
        this.handler = null;
    }

}

