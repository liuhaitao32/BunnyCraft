using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MagicSkillAction:TriggerGroupAction {


    public string cardId = null;

    public MagicSkillAction(Map map, int netId = -1):base(map, netId){

    }
    

    public override void start() {
        //对于魔法 有些是立刻执行的 填写skill 有些是通过触发器执行的。 在live.triggers里面。
        if(this._data.ContainsKey("skill")) {
            object[] skillData = (object[])this._data["skill"];
            MultiAction multiAction = new MultiAction(this._map);
            for(int i = 0, len = skillData.Length; i < len; i++) {
                multiAction.addAction(new SkillAction(this._map).init((Dictionary<string, object>)skillData[i], this._player));
            }
            this.addAction(multiAction);
        }
        base.start();
    }
    

    public override int type { get { return ConfigConstant.MAGIC_SKILL_ACTION; } }

    public override void update() {
        base.update();
    }


    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this.cardId = data["cardId"].ToString();
    }


    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["cardId"] = this.cardId;
        return data;
    }
}
