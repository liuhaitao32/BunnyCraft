using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SkillManager:MultiAction {

    public SkillManager(Map map, int netId = -1):base(map, netId) {

    }

    public override bool isFinish {
        get {
            return false;
        }
    }

    public override int type { get { return ConfigConstant.SKILL_MANAGER_ACTION; } }

    public override void update() {
        base.update();
    }
}
