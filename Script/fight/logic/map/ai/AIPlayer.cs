using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// AI控制下的Player,有额外记录值
public class AIPlayer:BaseAI
{
	public PlayerEntity player;
	///智商
	public double IQ;

	///发现控制器
	public FindController findController;
	///评估控制器
	public AssessController assessController;
	///策略控制器
	public TacticsController tacticsController;
	///情绪控制器
	public MoodController moodController;

	public AIPlayer (Map map,PlayerEntity player, double IQ):base(map) 
	{
        this.IQ = IQ;
        //this.IQ = 1;
		this.player = player;
		this.init ();
	}

	public void init ()
	{
		this.findController = new FindController (this);
		this.assessController = new AssessController (this);
		this.tacticsController = new TacticsController (this);
		this.moodController = new MoodController (this, "mood000");
		//this.resetIQ (this._map.AI_IQ);
	}
	public void update ()
	{
		if (this.player.alived) {
			this.findController.update ();
			this.assessController.update ();
			this.tacticsController.update ();
			this.moodController.update ();
		} else {
			this.hideMoveAimPosition();
		}
	}

		
	///发出摇杆
	public void sendMove(Vector2D steeringV2d) {
		NetAdapter.sendChangeMove(this.player.uid, steeringV2d.x, steeringV2d.y, this.player.netId);

		if (isSelf) {
			if (null != _playerView) {
				if (_playerView is PlayerSelf) {
					(_playerView as PlayerSelf).fakeJoystick (steeringV2d);
				}
			}
		}
	}
	///发出使用卡牌
	public void sendUseCard(int index) {
		CardData cardData = this.player.cardGroup.handCards[index];
		if(null != cardData && cardData.canUse){
			NetAdapter.sendUseCardAI(this.player.uid,index);
		}
	}
	///重置IQ，有一定的浮动值
	public void resetIQ(double value){
		this.IQ = (this.player.uid == FightMain.instance.selection.uid) ?
			value:
			Math.Min(1,this.getRandom () * AIConstant.IQ_RANDOM_RANGE + value);

		this.resetTestName ();
	}



	///取出对自身的评估
	public AssessPlayer selfAssess{
		get{ return assessController.selfAssess;}
	}

	public void clear(){
		//if (null != this._moveAimGO) {
		//	GameObject.Destroy (this._moveAimGO);
		//}
	}


















	//———————————————————————————————————————以下是对视图的调试方法———————————————————————————————————————————————
	///是否显示路径
	#if UNITY_EDITOR 
	public static bool SHOW_PATH = true;
	#else
	public static bool SHOW_PATH = false;
	#endif

	private Player _playerView;
	private GameObject _moveAimGO;

	private string _testName;
	private string _testLog;
	private Text _nameText;

	public bool initView ()
	{
        return false;
		if (null == _playerView) {
			PlayerEntity clientPlayer = FightMain.instance.selection.getPlayer(this.player.uid);
			if (null != clientPlayer) {
				if (null != clientPlayer.view) {
					this._playerView = (Player)clientPlayer.view;
					this._moveAimGO = this._playerView.clientRunTime.addEffect("hitAIMove", this.tacticsController.moveAimPosition);

					this._nameText = _playerView.nameText;
					if (null == _testName) {
						this.resetTestName ();
					}
					if (null == _testLog) {
						this.setTestLog ("");
					}
					return true;
				}
			}
			return false;
		}
		return true;
	}

	///更新当前移动目标位置
	public void showMoveAimPosition (){
        return;
		if (this.initView ()) {
			if (SHOW_PATH) {
				this._moveAimGO.SetActive (true);
				LeanTween.cancel (this._moveAimGO);
				LTDescr ltd = LeanTween.moveLocal (this._moveAimGO, ViewUtils.logicToScene (this.tacticsController.moveAimPosition, this._map.mapData.earthRadius + 1f, this._map.mapData), 0.3f);
				ltd.tweenType = LeanTweenType.easeOutSine;
			} else {
				this.hideMoveAimPosition();
			}
		}
	}
	///更新当前移动目标位置
	public void hideMoveAimPosition (){
        //throw new Exception("咩哈哈");
		//this._moveAimGO.SetActive (false);
	}


	///临时打印策略名
	private void showTest ()
	{
        return;
		if (this.initView ()) {
			this._nameText.text = _testName + "\n<color=#FFFF00FF>"+_testLog+"</color>";
		}
	}
	public void setTestLog (string log)
	{
		this._testLog = log;
		this.showTest ();
	}
	public void resetTestName ()
	{
		this._testName = this.player.name + " " + string.Format(" 【{0:F2}】",this.IQ);
		this.showTest ();
	}


	private bool isSelf 
	{
		get{return FightMain.instance.selection.uid == this.player.uid;}
	}
}


