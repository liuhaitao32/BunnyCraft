using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletShallow:NetObjectBase {

    public List<PersonEntity> persons = new List<PersonEntity>();

    private TimeAction _timeAction;

    public double shallow = 0;

    public BulletShallow(Map map , int netId = -1):base(map, netId) {
        
	}

    public BulletShallow init() {
        this._timeAction = this._map.addDelayCall(10000, this.clear, ConfigConstant.MAP_CALL_BACK_TOTAL_SHALLOW);
        return this;
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["persons"] = this.persons.FindAll((PersonEntity p)=> { return !p.cleared; }).ConvertAll<object>((PersonEntity p) => { return p.netId; }).ToArray();
        data["timeAction"] = this._timeAction.netId;
        data["shallow"] = this.shallow;
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this.persons = new List<object>((object[])data["persons"]).ConvertAll<PersonEntity>((object netId) => {
            return (PersonEntity)this._map.getNetObject((int)(netId));
        });
        this._timeAction = (TimeAction)this._map.getNetObject((int)(data["timeAction"]));

        this.shallow = Convert.ToDouble(data["shallow"]);
    }

    public void addPerson(PersonEntity player) {
        if(!this.persons.Contains(player)) this.persons.Add(player);
    }

    public int getAtk(PersonEntity person, int atk) {
        return this.persons.Contains(person) ? Mathf.CeilToInt(Convert.ToSingle(atk * this.shallow)) : atk;
    }

    public override void clear() {
        this.persons = null;
        this._timeAction = null;
        base.clear();
    }

    public override int type { get { return ConfigConstant.BULLET_SHALLOW; } }
}

