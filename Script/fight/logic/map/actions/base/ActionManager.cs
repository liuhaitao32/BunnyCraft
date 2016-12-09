using System;
using System.Collections.Generic;

public class ActionManager:MultiAction {
	/**
	 * 暂停的标志
	 */
	private bool _isPause;

	public int step = 0;

	private int _testStep = 111;


	public ActionManager (Map map, int netId = -1):base(map, netId) {
		
	}



	/**
	 * 暂停
	 */
	public void pause() {
		this._isPause = true;
	}

	/**
	 * 恢复
	 */
	public void resume() {
		this._isPause = false;
	}


	/**
		 * @inheritDoc
		 */
	override public void update() {
		if (this._isPause) return;
		this.step++;
		base.update();
	}
    
	/**
	 * 获取根节点
	 */
	override public ActionManager rootAction { get { return this; } }


    public override void setData(Dictionary<string, object> data) {
        base.setData(data);
        this._isPause = Convert.ToBoolean(data["isPause"]);
        this.step = (int)(data["step"]);
        this._testStep = (int)(data["testStep"]);
}

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData();
        data["isPause"] = this._isPause;
        data["step"] = this.step;
        data["testStep"] = this._testStep;
        return data;
    }

    public override int type { get { return ConfigConstant.ACTION_MANAGER; } }

    override public void clear() {
		base.clear();
	}
}

