using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LoopBeanEntity : BeanEntity {

    public LoopBeanEntity(Map map, int netId = -1):base(map, netId) {
        
    }
    

    public override void initConfig(Dictionary<string, object> data = null) {
        base.initConfig(data);
        this.randomPostion();
    }
    

    public void randomPostion() {
        List<Vector2D> range = new List<Vector2D>();
        List<int> expend = null;
        int extend = new List<int>() { 1, 2, 4 }[this.itemType - 1];
        switch(this.itemType) {
            case 1:
            case 2:
                range.Add(new Vector2D(this._map.mapData.gridYAreaLines[2], this._map.mapData.rowNum - this._map.mapData.gridYAreaLines[2] - 1));
                expend = new List<int> { ConfigConstant.ENTITY_LOOP_BEAN };
                break;
            case 3:
                range.Add(new Vector2D(this._map.mapData.gridYAreaLines[0], this._map.mapData.gridYAreaLines[2]));
                range.Add(new Vector2D(this._map.mapData.rowNum - this._map.mapData.gridYAreaLines[2] - 1, this._map.mapData.rowNum - this._map.mapData.gridYAreaLines[0] - 1));

                expend = new List<int> { ConfigConstant.ENTITY_PLAYER, ConfigConstant.ENTITY_LOOP_BEAN };
                break;
        }

        for(int i = 0, len = this._map.players.Count; i < len; i++) {
            PlayerEntity player = this._map.players[i];
            player.generateBirthGrids(extend);            
        }
        //萝卜模式 再加入远离萝卜
        if(this._map.mapData.isRadish && -1 != this._map.refereeController.radishController.radish.teamIndex) {
            this._map.refereeController.radishController.radish.generateBirthGrids();
        }

        Grid grid = this._map.birthGrids.getRandomBirthGrid(range, expend, true);
        if(null != grid) {
            this.birthPosition = grid.randomPosition;
            this.position = this.birthPosition.clone();
            this.generateBirthGrids();
        } else {
            this.alived = false;
        }
        this.setNewViewData();
        this.setOldViewData();
        Utils.clearList(range);
    }

    public override void useBean() {
        base.useBean();
        this.removeBirthGrids();
    }

    public void revive() {
        this._alived = true;
        this.randomPostion();
        if(this._alived) this.dispatchEventWith(EventConstant.START);
        //Debug.Log("复活" + this.itemType + " " + this._alived);
    }


    /// <summary>
    /// 只有item3是限制不让他出第二条边线。
    /// </summary>
    protected override List<List<int>> limitAreaLine {
        get {
            return this.itemType == 3 ?
                new List<List<int>> {
                    new List<int> { this._map.mapData.gridYAreaLines[0], this._map.mapData.gridYAreaLines[2] - 1 },
                    new List<int> { this._map.mapData.rowNum - this._map.mapData.gridYAreaLines[2] - 1, this._map.mapData.rowNum - this._map.mapData.gridYAreaLines[0] - 1 }
                } :
                base.limitAreaLine;
        }
    }

    public override int type { get { return ConfigConstant.ENTITY_LOOP_BEAN; } }

    public override bool alived {
        get {
            return base.alived;
        }

        set {
            base.alived = value;
            if(!value) this._map.birthBeanController.addBean(this);
        }
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        if(this._alived) this.generateBirthGrids();
    }

}
