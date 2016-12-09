using System;

public class ClientPriceBeanEntity : PriceBeanEntity {

    public ClientPriceBeanEntity(Map map, int netId = -1):base(map, netId) {

    }
    


    public override void update() {
        base.update();
    }

    public override void clear() {
        this.dispatchEventWith(EventConstant.DEAD);
        base.clear();
    }

}

