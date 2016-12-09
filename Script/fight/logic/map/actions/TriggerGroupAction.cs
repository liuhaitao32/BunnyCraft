using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TriggerGroupAction: QueueAction {


    protected PersonEntity _player;  //玩家战舰

    protected Dictionary<string, object> _data;

    protected List<Buff> _buffs = new List<Buff>();

    protected MultiAction _triggers;

    public TimeAction preTimeAction;

    public bool alived = false;

    private bool _canAction = false;

    public PersonEntity lockTarget;

    //TODO:一开始要把敌人传进来。有些召唤物从外面就锁定了唯一的敌人。
    public TriggerGroupAction(Map map, int netId = -1):base(map, netId) {
        
    }

    public virtual TriggerGroupAction init(Dictionary<string, object> data, PersonEntity player) {
        this._data = data;
        this._player = player;
        this._triggers = new MultiAction(this._map);
        return this;
    }

    public override void start() {
        //效果的东西 后端不用抄写。
        if(this._data.ContainsKey("preEffect")) this._player.dispatchEventWith(EventConstant.START, this);

        if(this._data.ContainsKey("preTime")) {
            this.preTimeAction = new TimeAction(this._map).init((int)(this._data["preTime"]));
            this.addAction(this.preTimeAction);
        }
        this.addAction(new HandlerAction(this._map).init(this.onAliveHandler, 1));

        //—————————————————————live———————————————————————
        if(this._data.ContainsKey("time")) {
            int totalTime = (int)this._data["time"];
            if(this._data.ContainsKey("live")) {

                Dictionary<string, object> live = (Dictionary<string, object>)this._data["live"];

                //buff
                if(live.ContainsKey("buff")) this.addAction(new HandlerAction(this._map).init(this.createBuff, 2));


                //triggers
                if(live.ContainsKey("triggers")) {
                    //给当前并行的触发器 加到装备里面。
                    object[] triggers = (object[])live["triggers"];
                    for(int i = 0, len = triggers.Length; i < len; i++) {
                        Dictionary<string, object> triggerData = (Dictionary<string, object>)triggers[i];
                        TriggerAction action = null;
                        int interval = (int)( triggerData["time"] );
                        if(triggerData.ContainsKey("skill")) {
                            action = new TriggerAction(this._map).initTrigger(interval, totalTime, triggerData, this._player, this.lockTarget);
                            action.breakAction = this;
                            action.breakGroup = Utils.equal(triggerData, "break", 1);
                        } else if(triggerData.ContainsKey("beam")){
                            action = new BeamAction(this._map).initTrigger(interval, totalTime, triggerData, this._player, this.lockTarget);
                            action.breakAction = this;
                        }

                        this._triggers.addAction(action);
                    }
                }
            }
            this.addAction(new TimeAction(this._map).init(totalTime));
        }
        base.start();
    }

    private void createBuff() {
        Dictionary<string, object> live = (Dictionary<string, object>)this._data["live"];
        Dictionary <string, object> buffs = (Dictionary<string, object>)live["buff"];
        string id = buffs["id"].ToString();
        foreach(string key in buffs.Keys) {
            if(key == "id") continue;
            Buff buff = new Buff(this._map).initBuff(id, key, this._player, this._player, (Dictionary<string, object>)buffs[key]);
            if(buff.bind) this._buffs.Add(buff);
        }
    }

    public Dictionary<string, object> data { get { return this._data; } }

    public override void clear() {
        //效果的东西 后端不用抄写。
        if(this._data.ContainsKey("preEffect")) this.dispatchEventWith(EventConstant.DEAD, this);
        Utils.clearList(this._buffs);
        Utils.clearObject(this._triggers);
        this._player = null;
        this._data = null;
        this._buffs = null;
        this.preTimeAction = null;
        base.clear();
    }

    public override void update() {
        base.update();
        bool newCanAction = this.canAction;
        //从不能使用到可以使用 要重置所有的触发器间隔时间。不能用派事件 所以就这么写了。
        //这个只要改变就重置一下 会调用两次 主要是为了方便客户端取值。
        if(this._canAction != newCanAction) {
            this._canAction = newCanAction;
            this.resetTriggers();
        }

        if(this._canAction) {
            this._triggers.update();
        }
    }

    private void resetTriggers() {
        for(int i = 0, len = this._triggers.actions.Count; i < len; i++) {
            ((TriggerAction)this._triggers.actions[i]).reset();
        }
    }

    public override int type { get { return ConfigConstant.TRIGGER_GROUP_ACTION; } }

    protected virtual bool canAction {
        get{ return this.alived; }
    }

    protected virtual void onAliveHandler() {
        this.preTimeAction = null;
        this._triggers.start();
        this.alived = true;
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this._player = (PersonEntity)this._map.getNetObject((int)(data["player"]));
        this._buffs = new List<object>((object[])data["buffs"]).ConvertAll<Buff>((object buffId)=> {
            return (Buff)this._map.getNetObject((int)(buffId));
        });
        this._triggers = (MultiAction)this._map.getNetObject((int)(data["triggers"]));

        if(data.ContainsKey("preTimeAction")) {
            this.preTimeAction = (TimeAction)this._map.getNetObject((int)(data["preTimeAction"]));
        }
        if(data.ContainsKey("lockTarget")) {
            this.lockTarget = (PersonEntity)this._map.getNetObject((int)( data["lockTarget"] ));
        }
        this.alived = Convert.ToBoolean(data["alived"]);
        this._canAction = Convert.ToBoolean(data["canAction"]);



        //这里要保证handler一定要初始化完毕！
        for(int i = 0, len = this._actions.Count; i < len; i++) {
            if(this._actions[i].type == ConfigConstant.HANDLER_ACTION) {
                HandlerAction action = (HandlerAction)this._actions[i];
                switch(action.handlerId) {
                    case 1:
                        action.handler = this.onAliveHandler;
                        break;
                    case 2:
                        action.handler = this.createBuff;
                        break;
                    default:
                        throw new Exception("超出范围");
                }
            }
        }

    }

    
    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["player"] = this._player.netId;
        data["buffs"] = this._buffs.ConvertAll<object>((Buff b)=> { return b.netId; }).ToArray();
        
        if(null != this.preTimeAction) {
            data["preTimeAction"] = this.preTimeAction.netId;
        }
        data["triggers"] = this._triggers.netId;

        data["alived"] = this.alived;
        data["canAction"] = this._canAction;
        if(null != this.lockTarget) {
            data["lockTarget"] = this.lockTarget.netId;
        }        
        return data;
    }
}

