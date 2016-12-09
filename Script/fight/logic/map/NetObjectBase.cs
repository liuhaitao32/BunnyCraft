using System;
using System.Collections.Generic;

public class NetObjectBase : EventClass, INetObject {    

    protected int _netId = 0;

    protected Map _map;

    public NetObjectBase(Map map, int netId = -1) {
        this._map = map;
        if(-1 == netId) {
            this._netId = this._map.netId++;
        } else {
            this._netId = netId;
        }
        //if(this._netId == 1785) {
        //    Log.debug("df");
        //}
        if(this._map is ClientRunTime) {
            if(((ClientRunTime)this._map).notSetData && netId == -1) {
//                if(!FightMain.tempFlag) throw new Exception(this + "不能直接初始化！");
            }
        }

		if(null != this._map)this._map.addNetObject(this);
    }


    public virtual Dictionary<string, object> getData() {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["netId"] = this._netId;
        data["type"] = this.type;
        return data;
    }

    public virtual void setData(Dictionary<string, object> data) {
        this._netId = (int)(data["netId"]);
    }


    public virtual int type { get { throw new Exception(this + "没有复写此方法！"); return -1; } }

    public int netId {
        get {
            if(this._cleared) {
                throw new Exception("已经清除掉了！但是外部还有调取！");
            }
            return this._netId;
        }
    }


    /**
	* @inheritDoc
	*/
    public override void clear() {
		if(null != this._map) this._map.removeNetObject(this);
        this._map = null;
        base.clear();
    }
}

