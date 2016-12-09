using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class SkillAction:QueueAction {

	private PersonEntity _player;

    private Dictionary<string, object> _data;

    private List<Buff> _buffs = new List<Buff>();

    public PersonEntity lockTarget;

    public SkillAction (Map map, int netId = -1):base(map, netId) {
        
	}

    public SkillAction init(Dictionary<string, object> data, PersonEntity player, PersonEntity lockTarget = null) {
        this._data = data;
        this._player = player;
        this.lockTarget = lockTarget;
        return this;
    }

	public override void start () {
        //效果的东西 后端不用抄写。
        if(this._data.ContainsKey("preEffect")) this._player.dispatchEventWith(EventConstant.START, this);
        if (this._data.ContainsKey ("buff")) {
			Dictionary<string, object> buffs = (Dictionary<string, object>)this._data["buff"];
            string id = buffs["id"].ToString();
            //TODO:目前是都给自己的。
            foreach(string key in buffs.Keys) {
                if(key == "id") continue;
                Buff buff = new Buff(this._map).initBuff(id, key, this._player, this._player, (Dictionary<string, object>)buffs[key]);                    
                if(buff.bind) this._buffs.Add(buff);
			}
		}
        if(this._data.ContainsKey("step")) {
            object[] steps = (object[])this._data["step"];

            for(int i = 0, len = steps.Length; i < len; i++) {
                Dictionary<string, object> step = (Dictionary<string, object>)steps[i];
                this.addAction(new SkillStepAction(this._map).init(step, this._player, this.lockTarget));
            }
        }

        base.start();

    }

    public override int type { get { return ConfigConstant.SKILL_ACTION; } }

    public Dictionary<string, object> data { get { return this._data; } }

    public override void update() {
        base.update();
    }

    public override void clear() {
        Utils.clearList(this._buffs);
        this._player = null;
        this._data = null;
        this.lockTarget = null;
        base.clear();
    }


    public override void setData(Dictionary<string, object> data) { 
        base.setData(data);
        this._player = (PersonEntity)this._map.getNetObject((int)(data["player"]));
        this._data = (Dictionary<string, object>)data["data"];
        
        this._buffs = new List<object>((object[])data["buffs"]).ConvertAll<Buff>((object buffId)=> {
            return (Buff)this._map.getNetObject((int)(buffId));
        });

        if(data.ContainsKey("lockTarget")) {
            this.lockTarget = (PersonEntity)this._map.getNetObject((int)( data["lockTarget"] ));
        }
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["player"] = this._player.netId;
        data["data"] = this._data;
        data["buffs"] = this._buffs.ConvertAll<object>((Buff b)=> { return b.netId; }).ToArray();
        if(null != this.lockTarget) {
            data["lockTarget"] = this.lockTarget.netId;
        }
        return data;
    }
}

