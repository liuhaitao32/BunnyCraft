using System;
using System.Collections.Generic;

public class NetObjectFactory {

    // 只有场景初始化的时候才会走这里！ 平时用这个生成 太耗费效率了!
    public static NetObjectBase createNetObject(int type, Map map, int netId) {
        switch(type) {
            case ConfigConstant.GEOM_CIRCLE:
                return new GeomCircle(map, netId);
            case ConfigConstant.GEOM_RECT:
                return new GeomRect(map, netId);
            case ConfigConstant.GEOM_LINE:
                return new GeomLine(map, netId);
            case ConfigConstant.GEOM_SECTOR:
                return new GeomSector(map, netId);
            case ConfigConstant.BUFF:
                return new Buff(map, netId);
            case ConfigConstant.BULLET_SHALLOW:
                return new BulletShallow(map, netId);
            case ConfigConstant.ACTION_BASE:
                throw new Exception("类型错误");
            case ConfigConstant.ACTION_MANAGER:
                return new ActionManager(map, netId);
            case ConfigConstant.MULTI_ACTION:
                return new MultiAction(map, netId);
            case ConfigConstant.QUEUE_ACTION:
                return new QueueAction(map, netId);
            case ConfigConstant.HANDLER_ACTION:
                return new HandlerAction(map, netId);
            case ConfigConstant.INTERVAL_ACTION:
                return new IntervalAction(map, netId);
            case ConfigConstant.TIME_ACTION:
                return new TimeAction(map, netId);
            case ConfigConstant.SKILL_ACTION:
                return new SkillAction(map, netId);
            case ConfigConstant.SKILL_STEP_ACTION:
                return new SkillStepAction(map, netId);
            case ConfigConstant.TRIGGER_ACTION:
                return new TriggerAction(map, netId);
            case ConfigConstant.BEAM_ACTION:
                return new BeamAction(map, netId);
            case ConfigConstant.TRIGGER_GROUP_ACTION:
                return new TriggerGroupAction(map, netId);
            case ConfigConstant.CARD_GROUP_ACTION:
                return new CardGroupAction(map, netId);
            case ConfigConstant.PART_ACTION:
                return new PartAction(map, netId);
            case ConfigConstant.PART_GROUP_ACTION:
                return new PartGroupAction(map, netId);
            case ConfigConstant.MAGIC_SKILL_ACTION:
                return new MagicSkillAction(map, netId);
            case ConfigConstant.SKILL_MANAGER_ACTION:
                return new SkillManager(map, netId);
            case ConfigConstant.ENTITY_LOOP_BEAN:
                return new ClientBeanEntity(map, netId);
            case ConfigConstant.ENTITY_PLAYER:
                return new ClientPlayerEntity(map, netId);
            case ConfigConstant.ENTITY_BULLET:
                return new ClientBulletEntity(map, netId);
            case ConfigConstant.ENTITY_PRICE_BEAN:
                return new ClientPriceBeanEntity(map, netId);
            case ConfigConstant.ENTITY_CALL:
                return new ClientCallEntity(map, netId);
            case ConfigConstant.ENTITY_BARRIER:
                return new ClientBarrierEntity(map, netId);
            case ConfigConstant.ENTITY_RADISH:
                return new ClientRadishEntity(map, netId);
            case ConfigConstant.FIGHT_RESULT:
                return new FightResult(map, netId);
            case ConfigConstant.BIRTH_BEAN_CONTROLLER:
                return new BirthBeanController(map, netId);
            case ConfigConstant.REFEREE_CONTROLLER:
                return new RefereeController(map, netId);
            case ConfigConstant.RADISH_CONTROLLER:
                return new RadishController(map, netId);
            default:
                throw new Exception("超出范围" + type);
            //case ConfigConstant.MAP = 504;
        }

        return null;
    }

}

