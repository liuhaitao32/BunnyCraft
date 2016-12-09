using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Buff:IntervalAction, IDecorate {

    protected Dictionary<string, object> _data;

    protected PersonEntity _target;

    protected string _buffType;

    protected FightEntity _owner;

    public bool bind = false;

    private int _operation = 1;

    private double _value = 1;

    public string id;

    private TriggerAction triggerAction;

    //这个后端要生成一个根据每场战斗给一个buff 多个buff 会重叠。 但是我客户端就临时这么写了。
    public static int instanceId = 0;

    public Buff(Map map, int netId = -1):base(map, netId) {

    }

    public Buff initBuff(string id, string buffType, PersonEntity target, FightEntity owner, Dictionary<string, object> data) {
        this.id = id;
        this._data = data;
        this._target = target;
        this._buffType = buffType;
        this._owner = owner;

        this.bind = !this._data.ContainsKey("time");

        this._totalTime = this._data.ContainsKey("time") ? (int)(this._data["time"]) : int.MaxValue;
        if(this._data.ContainsKey("interval"))  this._interval  = (int)(this._data["interval"]);
        if(this._data.ContainsKey("operation")) this._operation = (int)(data["operation"]);
        if(this._data.ContainsKey("skill")) {
            this.triggerAction = new TriggerAction(this._map).initTrigger(this._interval, this._totalTime, (Dictionary<string, object>)this._data, this.applyTarget);
            this.triggerAction.start();
        }

        if(this._data.ContainsKey("value")) {
            this._value = this._buffType == ConfigConstant.PROPERTY_ASP && this._operation <= 2?
                            Math2.angleToRadian(Convert.ToSingle(data["value"])) :
                            Convert.ToSingle(data["value"]);
        }
        //作用在谁的身上。
        this.applyTarget.addBuff(this);

        this.applyBuff();
        this._intervalHandler = this.onIntervalHandler;
        return this;
    }
    /**
	 * 装饰属性。
	 * @param	personProperty
	 * @return
	 */
    public void decortation(Dictionary<string, double> property) {
        double value = property.ContainsKey(this._buffType) ? Convert.ToSingle(property[this._buffType]) : 0;
        this.applyTarget.setProperty(this._buffType, LogicOperation.operation(value, this._value, this._operation));
    }

    public double value {
        set { this._value = value; }
        get { return this._value; }
    }

    public int operation {
        get { return this._operation; }
    }
    

    

    private void applyBuff() {
        switch(this._buffType) {
            case ConfigConstant.BUFF_STUN:
                this.applyTarget.skillManager.removeAllAction();
                Vector2D joystick = Vector2D.createVector();
                this.applyTarget.setJoystick(joystick);
                joystick.clear();
                break;
        }
    }

    public override void finish() {
        base.finish();
        Utils.clearObject(this);
    }

    public override void update() {
        base.update();
        if(null != this.triggerAction) this.triggerAction.update();
    }
    

    public FightEntity owner { get { return this._owner; } }

    public PersonEntity applyTarget {
        get {
            return Utils.equal(this._data, "target", 1) ? this._target : (PersonEntity)this._owner;
        }
    }
    

    public override void clear() {
        this._isFinish = true;
        this.applyTarget.removeBuff(this);
        base.clear();
        this._target = null;
        this._owner = null;
        Utils.clearObject(this.triggerAction);
        this.triggerAction = null;
        this._buffType = null;
        this._data = null;
    }
    

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this._data = (Dictionary<string, object>)data["data"];
        this._target = (PersonEntity)this._map.getNetObject((int)(data["target"]));
        this._buffType = data["buffType"].ToString();
        this.id = data["id"].ToString();

        this._owner = (PersonEntity)this._map.getNetObject((int)(data["owner"]));
        this.bind = Convert.ToBoolean(data["bind"]);
        this._operation = (int)(data["operation"]);
        this._value = Convert.ToSingle(data["value"]);
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();        
        data["data"] = this._data;
        data["id"] = this.id;
        data["target"] = this._target.netId;
        data["buffType"] = this._buffType;
        data["owner"] = this._owner.netId;
        data["bind"] = this.bind;
        data["operation"] = this._operation;
        data["value"] = this.value;
        return data;
    }

    public Dictionary<string, object> data { get { return this._data; }}

    public override int type { get { return ConfigConstant.BUFF; } }

    public string buffType { get { return this._buffType; } }
}
