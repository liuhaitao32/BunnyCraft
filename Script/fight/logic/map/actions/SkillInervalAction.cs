using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SkillInervalAction : ActionBase {


    private PersonEntity _person;

    private Dictionary<string, object> _data;

    private List<Buff> _buffs = new List<Buff>();

    public List<PersonEntity> targets;

    private TimeAction _cdTime;

    public SkillInervalAction(Map map, int netId = -1):base(map, netId){

    }

    public SkillInervalAction init(Dictionary<string, object> data, int totalTime, PersonEntity person) {
        this._data = data;
        this._person = person;
        this._cdTime = new TimeAction(this._map).init(totalTime);
        return this;
    }

    public override void start() {
        //对于魔法 有些是立刻执行的 填写skill 有些是通过触发器执行的。 在live.triggers里面。
        //if(this._data.ContainsKey("skill")) {
        //    object[] skillData = (object[])this._data["skill"];
        //    MultiAction multiAction = new MultiAction(this._map);
        //    for(int i = 0, len = skillData.Length; i < len; i++) {
        //        multiAction.addAction(new SkillAction(this._map).init((Dictionary<string, object>)skillData[i], this._player));
        //    }
        //    this.addAction(multiAction);
        //}
        base.start();
    }

    public bool isCding { get { return this._cdTime.isFinish; } }

    public override int type { get { return ConfigConstant.MAGIC_SKILL_ACTION; } }

    public void useSkill() {
        if(this.isCding) return;
        SkillAction skillAction = new SkillAction(this._map).init(this._data, this._person);
        this._person.addAction(skillAction);
        this._cdTime.reset();
    }

    public override void update() {
        base.update();
        if(!this.isCding) this._cdTime.update();
    }
}
