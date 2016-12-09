using System;
using System.Collections.Generic;


public class BirthBeanController:NetObjectBase {

    private List<List<LoopBeanEntity>> _beans;

    private List<int> _currCd;

    private List<int> _removeCount;

    public BirthBeanController(Map map, int netId = -1):base(map, netId) {
        
    }

    public void init() {
        this._beans = new List<List<LoopBeanEntity>>();
        this._currCd = new List<int>();
        this._removeCount = new List<int>();
        for(int i = 0; i < 3; i++) {
            this._beans.Add(new List<LoopBeanEntity>());
            this._currCd.Add(this.getBeanTotalTime(i + 1));
            this._removeCount.Add(0);
        }
    }

    public void update() {
        for(int i = 0, len = this._currCd.Count; i < len; i++) {
            MediatorSystem.timeStart("birth" + i);
            this._currCd[i] -= ConfigConstant.MAP_ACT_TIME_S * this._beans[i].Count;
            if(this._currCd[i] <= 0) {
                this._currCd[i] = this.getBeanTotalTime(i + 1);
                if(this._beans[i].Count > 0) {
                    LoopBeanEntity bean = this._beans[i][0];
                    this._beans[i].RemoveAt(0);
                    bean.revive();
                    if(!bean.alived) {
                        this._removeCount[i]++;
                        if(3 == this._removeCount[i]) {
                            Utils.clearList(this._beans[i]);
                            this._removeCount[i] = 0;
                        }
                    } else {
                        this._removeCount[i] = 0;
                    }
                }
            }
            MediatorSystem.getRunTime("birth" + i, this._beans[i].Count);
        }
    }

    private int getBeanTotalTime(int type) {
        Dictionary<string, object> beanConfig = (Dictionary<string, object>)ConfigConstant.powerConfig["bean" + type];
        return (int)this._map.getRandomRange(Convert.ToDouble(beanConfig["cdMin"]), Convert.ToDouble(beanConfig["cdMax"]));
    }

    public void addBean(LoopBeanEntity bean) {
        this._beans[bean.itemType - 1].Add(bean);
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["currCd"] = this._currCd.ConvertAll<object>((o)=> { return o; }).ToArray();
        data["removeCount"] = this._removeCount.ConvertAll<object>((o) => { return o; }).ToArray();
        data["beans"] = this._beans.ConvertAll<object>((o)=> { return o.ConvertAll<object>((LoopBeanEntity b)=> { return b.netId; }).ToArray(); }).ToArray();
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        this._currCd = new List<object>((object[])data["currCd"]).ConvertAll<int>((object o)=> { return (int)o; });
        this._removeCount = new List<object>((object[])data["removeCount"]).ConvertAll<int>((object o) => { return (int)o; });
        this._beans = new List<object>((object[])data["beans"]).ConvertAll<List<LoopBeanEntity>>(
            (object o) => {
                return new List<object>((object[])o).ConvertAll<LoopBeanEntity>(
                    (object oo) => {
                        return (LoopBeanEntity)this._map.getNetObject((int)oo);
                    });
            });
        base.setData(data);
    }

    public override int type { get { return ConfigConstant.BIRTH_BEAN_CONTROLLER; } }

    public void removeWaitBean() {
        for(int i = 0, len = this._beans.Count; i < len; i++) {
            while(0 != this._beans[i].Count) {
                Utils.clearList(this._beans[i]);
            }
        }

    }
}
