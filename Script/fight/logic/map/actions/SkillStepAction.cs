using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SkillStepAction:QueueAction {
    private Dictionary<string, object> _step;
    private PersonEntity _player;

    private PersonEntity _lockTarget;

    public SkillStepAction(Map map, int netId = -1):base(map, netId) {
        
    }

    public SkillStepAction init(Dictionary<string, object> step, PersonEntity player, PersonEntity lockTarget = null) {
        this._step = step;
        this._player = player;
        this._lockTarget = lockTarget;
        return this;
    }

    public override void start() {
        //TODO:这里要派发一个事件。 放到人身上。
        this._player.dispatchEventWith(EventConstant.SKILL_START, this._step);
		if(this._step.ContainsKey("preTime")) this.addAction(new TimeAction(this._map).init((int)(this._step["preTime"])));
        if(this._step.ContainsKey("player"))  this.addAction(new HandlerAction(this._map).init(this.changePlayer, 1));
        if(this._step.ContainsKey("bullets")) this.addAction(new HandlerAction(this._map).init(this.createBullet, 2));
        if(this._step.ContainsKey("calls")) this.addAction(new HandlerAction(this._map).init(this.createCalls, 3));
        base.start();
    }

    private void changePlayer() {
        Dictionary<string, object> playerData = (Dictionary<string, object>)this._step["player"];
        if(playerData.ContainsKey("position")) {
            object[] position = (object[])playerData["position"];
            Vector2D v = Vector2D.createVector(Convert.ToDouble(position[0]), Convert.ToDouble(position[1]));
            v.angle += this._player.angle;
            this._player.position = this._player.position.add(v);
            v.clear();
        }
    }

    private void createCalls() {
        object[] calls = (object[])this._step["calls"];

        for(int i = 0, len = calls.Length; i < len; i++) {
            Dictionary<string, object> callData = (Dictionary<string, object>)calls[i];
            CallEntity entity = (CallEntity)this._map.createFightEntity(ConfigConstant.ENTITY_CALL);
            entity.ownerPlayer = this._player.ownerPlayer;
            entity.initConfig(callData);

        }
    }

    private void createBullet() {
        object[] bullets = (object[])this._step["bullets"];
        //return;
		Dictionary<string, object> global = this._step.ContainsKey("global") ? (Dictionary<string, object>)this._step["global"] : null;
        MediatorSystem.timeStart("skillCreateBullet");
        BulletShallow shallow = null;
        if(this._step.ContainsKey("shallow")) {
            shallow = new BulletShallow(this._map).init();
            shallow.shallow = Convert.ToDouble(this._step["shallow"]);
        }

        
        for(int i = 0, len = bullets.Length; i < len; i++) {
            //MediatorSystem.timeStart("clone");
            Dictionary<string, object> bulletData = (Dictionary<string, object>)bullets[i];
            //当前的字典 合并到子弹里。
            if(null != global) {
                Dictionary<string, object> temp = (Dictionary<string, object>)Utils.clone(global);
                Utils.union1(temp, bulletData);
                bulletData = temp;
            }
            //MediatorSystem.getRunTime("clone");
            
            BulletEntity bullet = this._player.map.createFightEntity(ConfigConstant.ENTITY_BULLET) as BulletEntity;
            bullet.lockTarget = this._lockTarget;
            bullet.owner = this._player;
            bullet.shallow = shallow;
            bullet.initConfig(bulletData);
        }

        MediatorSystem.getRunTime("skillCreateBullet", bullets.Length);
    }

    public override int type { get { return ConfigConstant.SKILL_STEP_ACTION; } }

    public override void clear() {
        this._step = null;
        this._player = null;
        this._lockTarget = null;
        base.clear();
    }

    public override void update() {
        base.update();
    }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data); ;
        this._player = (PersonEntity)this._map.getNetObject((int)(data["player"]));
        this._step = (Dictionary<string, object>)data["step"];

        if(data.ContainsKey("lockTarget")) {
            this._lockTarget = (PersonEntity)this._map.getNetObject((int)( data["lockTarget"] ));
        }
        //这里要保证handler一定要初始化完毕！
        for(int i = 0, len = this._actions.Count; i < len; i++) {
            if(this._actions[i].type == ConfigConstant.HANDLER_ACTION) {
                HandlerAction action = (HandlerAction)this._actions[i];
                switch(action.handlerId) {
                    case 1:
                        action.handler = this.changePlayer;
                        break;
                    case 2:
                        action.handler = this.createBullet;
                        break;
                    case 3:
                        action.handler = this.createCalls;
                        break;
                    default:
                        throw new Exception("超出范围");
                }
            }
        }

    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["player"] = this._player.netId;
        data["step"] = this._step;
        if(null != this._lockTarget) {
            data["lockTarget"] = this._lockTarget.netId;
        }
        return data;
    }
}
