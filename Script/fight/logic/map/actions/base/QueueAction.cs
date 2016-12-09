using System;

public class QueueAction:MultiAction {
	
	public QueueAction (Map map, int netId = -1):base(map, netId){
		
	}

	/**
		 * @inheritDoc
		 */
	override public void update() {
		ActionBase firstAction = this.firstAction;
		if (null != firstAction) {				
			firstAction.update();
			if (firstAction.isFinish) {
				this.nextAction();
			}
		}else {
			this._isFinish = true;
		}
	}

	/**
		 * 向下执行一个action
		 */
	public void nextAction() {
		this.removeActionAt(0);
		this.startSubAction();
	}

	/**
		 * 开始执行当前子动作
		 */
	protected void startSubAction() {
		ActionBase firstAction = this.firstAction;
		if (null != firstAction) {
			firstAction._rootAction = this.rootAction;
			firstAction.start();
			//这个是判断是否立刻完成。 如果此动作立刻完成 就进行下一个。
			if (firstAction.isFinish) {
				this.nextAction();
			}
		}else {
			this._isFinish = true;
		}
	}

	/**
		 * 获取当前执行的action
		 */
	protected ActionBase firstAction { get { return this._actions.Count > 0 ? this._actions[0] : null; }}

	/**
	 * 获取当前动作
	 */
	override public ActionBase currAction {
		get {return null != this.firstAction ? this.firstAction.currAction : null;}
	}

    public override int type { get { return ConfigConstant.QUEUE_ACTION; } }

    /**
		 * @inheritDoc
		 */
    override public void start() {
		this._isFinish = false;
		this._cannelFlag = false;
		this._isStart = true;
		this.startSubAction();

	}
}
