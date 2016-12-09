using System;
using System.Collections.Generic;

public class MultiAction:ActionBase {
	/**
	 * 多个动作队列数组
	 */
	protected List<ActionBase> _actions = new List<ActionBase>();

	//——————————————————————————————————————以下是方法——————————————————————————————————————————————————

	public MultiAction (Map map, int netId = -1):base(map, netId) {

	}

	/**
	 * 添加一个action到末尾
	 * @param	action
	 */
	public ActionBase addAction(ActionBase action) {
		return this.addActionAt(action, this._actions.Count);
	}

	/**
		 * 添加到指定位置的action
		 * @param	action
		 */
	public ActionBase addActionAt(ActionBase action, int index) {
		if (this._actions.Contains(action)) return action;

		this._actions.Insert(index, action);			

		action._parentAction = this;
		action._rootAction = this.rootAction;
		return action;
	}

	/**
	 * 删除一个指定位置action
	 * @param	index 索引
	 */
	public virtual void removeActionAt(int index) {
		ActionBase action = this._actions[index];
        this._actions.RemoveAt(index);
        Utils.clearObject(action);
		this._isFinish = (0 == this._actions.Count);
	}

	public void removeAllAction() {
        Utils.clearList(this._actions);
        this._isFinish = true;
    }

	/**
		 * 删除一个action
		 * @param	action
		 */
	public void removeAction(ActionBase action) {
		int index = this._actions.IndexOf (action);
		if ( -1 != index) {
			this.removeActionAt(index);
		}
	}

	/**
		 * @inheritDoc
		 */
	override public void update() {

		List<ActionBase> remove = new List<ActionBase> ();

		for (int i = 0, len = this._actions.Count; i < len; i++) {
			ActionBase action = this._actions[i];
			if (!action.isFinish) {
				action.update();
			}else {
				remove.Add(action);
			}
		}

		while (0 != remove.Count) {			
			this.removeAction(remove [0]);
			remove.RemoveAt (0);
		}
	}

	/**
		 * 获取当前的队列数组
		 */
	public List<ActionBase> actions {get {return this._actions;}}

	/**
	 * @inheritDoc
	 */
	override public void start() {		
		for (int i = 0, len = this._actions.Count; i < len; i++) {
			this._actions[i].start();
		}
		this._isFinish = false;
		this._cannelFlag = false;
		this._isStart = true;
	}

	/**
	 * @inheritDoc
	 */
	override public void cannel() {
		base.cannel();
		for (int i = 0, len = this._actions.Count; i < len; i++) {
			this._actions[i].cannel();
		}
	}

    public override int type { get { return ConfigConstant.MULTI_ACTION; } }

    public override void setData(Dictionary<string, object> data) {
        base.setData(data);

        this._actions = new List<object>((object[])data["actions"]).ConvertAll<ActionBase>((object actionId) => {
            return (ActionBase)this._map.getNetObject((int)(actionId));
        });
    }

    public override Dictionary<string, object> getData() {
        Dictionary<string, object> data = base.getData(); 
        data["actions"] = this._actions.ConvertAll<object>((ActionBase a) => { return a.netId; }).ToArray();
        return data;
    }

    /**
	 * @inheritDoc
	 */
    override public void clear() {
        this.removeAllAction();
		base.clear();		
	}
}

