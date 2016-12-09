using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PriceBeanEntity:BeanEntity {

    public int cd;

    public PriceBeanEntity(Map map, int netId = -1):base(map, netId) {
        
    }
    

    public override void initConfig(Dictionary<string, object> data = null) {
        base.initConfig(data);
        this._alived = false;
        this.cd = ConfigConstant.DEAD_ITEM_READY_TIME;
        this.velocityLimitMax = 1000;
    }

    public override void update() {        
        if(!this._alived) {
            this.setOldViewData();
            this.cd -= ConfigConstant.MAP_ACT_TIME_S;
            this._alived = this.cd <= 0;
            this.applyPosition();
        } else {
            base.update();
        }
    }

    public override void useBean() {
        base.useBean();
        this.clear();
    }

    protected override void velocityToPosition() {
        base.velocityToPosition();
        this._velocity.multiply(0.85);
    }

    public override void applyPosition() {
        base.applyPosition();
    }





    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["cd"] = this.cd;
        return data;
    }

    public override void setData(Dictionary<string, object> data) {
        this.cd = (int)( data["cd"] );
        base.setData(data);
    }

    public override int type {
        get {
            return ConfigConstant.ENTITY_PRICE_BEAN;
        }
    }
}
