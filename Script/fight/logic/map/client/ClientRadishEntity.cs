using System;
using System.Collections.Generic;
using System.Collections;


public class ClientRadishEntity : RadishEntity, IClientFake {

    

    public ClientRadishEntity(Map map, int netId = -1):base(map, netId) {
		
	}



    public override void update () {
        base.update ();
	}
    

    public override void clear() {
        base.clear();
    }


    public void fakeUpdate() {

    }

    public void regainFake() {

    }

    public void saveFake() {

    }
}

