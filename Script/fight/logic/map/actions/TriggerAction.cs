using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TriggerAction:TimeAction {

    protected Dictionary<string, object> _triggerData;

    protected MultiAction _mutilAction;

    protected PersonEntity _person;

    public ActionBase breakAction;

    /**
     * 对于冲撞技能 在技能检测成功之后 要停掉整个技能触发器！
     */
    public bool breakGroup = false;

    protected TimeAction _cdTime;

    public PersonEntity lockTarget;


    public TriggerAction(Map map, int netId = -1):base(map, netId) {
        
    }

    public virtual TriggerAction initTrigger(int interval, int totalTime, Dictionary<string, object> triggerData, PersonEntity person, PersonEntity lockTarget = null) {
        this.lockTarget = lockTarget;
        this._mutilAction = new MultiAction(this._map);
        this._triggerData = triggerData;
        this._person = person;
        this._cdTime = new TimeAction(this._map).init(interval);
        return (TriggerAction)this.init(totalTime);
    }

    public override void start() {
        base.start();
        this._cdTime.time = 0;
        this._cdTime.start();
    }

    public bool cding { get { return this._cdTime.isFinish; } }

    public override void reset() {
        base.reset();
        this._mutilAction.removeAllAction();
        this._cdTime.time = 0;
    }
    

    public override void update() {
        base.update();
        this._mutilAction.update();
        

        bool check = this._cdTime.isFinish;
        if(!check) {
            this._cdTime.update();
            return;
        }
        
        this.useSkill();
    }

    protected virtual void useSkill() {
        List<PersonEntity> targets = null;
        if(this._triggerData.ContainsKey("condition")) {
            Dictionary<string, object> condition = (Dictionary<string, object>)this._triggerData["condition"];
            GeomBase geom = Collision.createGeom(this._person, (object[])condition["range"], (object[])condition["offset"]);
            if(condition.ContainsKey("offsetType") && (int)condition["offsetType"] != 0) {
                geom.applyEntity = false;
                geom.position.copy(this._person.position);
                geom.angle = this._person.angle;
                Vector2D v = this._person.getVertexPosition((int)condition["offsetType"]).clone();
                v.angle += this._person.angle;
                geom.position.add(v);
                v.clear();
            }
            targets = this._person.map.getPerson(geom, (int)( condition["aim"] ), (int)( condition["sort"] ));
            Utils.clearObject(geom);
            if(0 == targets.Count) return;//有条件 查找不到人就return。
            
            if(null != this.lockTarget) {//锁定了人 并且能查找到人。并且能在攻击范围内使用技能
                if(!targets.Contains(this.lockTarget)) return;
            } else {//一开始没有锁定人
                this.lockTarget = targets[0];
            }
        }
        SkillAction skillAction = new SkillAction(this._map).init((Dictionary<string, object>)this._triggerData["skill"], this._person, this.lockTarget);
        this._person.addAction(skillAction);
        //skillAction.start();

        if(this.breakGroup && null != skillAction.lockTarget) {
            this.breakAction.cannel();
        }
        this._cdTime.reset();
    }

    override public int type { get { return ConfigConstant.TRIGGER_ACTION; } }

    public override void clear() {
        base.clear();
        this._triggerData = null;
        Utils.clearObject(this._mutilAction);
        this._person = null;
        this.breakAction = null;
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data); 

        this._triggerData = (Dictionary<string, object>)data["triggerData"];
        this._mutilAction = (MultiAction)this._map.getNetObject((int)(data["mutilAction"]));
        this._cdTime = (TimeAction)this._map.getNetObject((int)( data["cdTime"] ));
        this._person = (PersonEntity)this._map.getNetObject((int)(data["person"] ));
        
        if(data.ContainsKey("breakAction")) {
            this.breakAction = (ActionBase)this._map.getNetObject((int)(data["breakAction"]));
        }
        if(data.ContainsKey("lockTarget")) {
            this.lockTarget = (PersonEntity)this._map.getNetObject((int)( data["lockTarget"] ));
        }
        this.breakGroup = Convert.ToBoolean(data["breakGroup"]);
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["triggerData"] = this._triggerData;
        data["mutilAction"] = this._mutilAction.netId;
        data["person"] = this._person.netId;
        if(null != this.breakAction) {
            data["breakAction"] = this.breakAction.netId;
        }
        if(null != this.lockTarget) {
            data["lockTarget"] = this.lockTarget.netId;
        }
        data["cdTime"] = this._cdTime.netId;
        data["breakGroup"] = this.breakGroup;
        return data;
    }
}
