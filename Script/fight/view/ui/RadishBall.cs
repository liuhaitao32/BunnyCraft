using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// 抢萝卜单个比分球
public class RadishBall : TeamBall {
	private Transform gainImageTransform;
	private GameObject gainImageGameObject;

	public override void Awake () {
		base.Awake ();
		this.slider.maxValue = ConfigConstant.RADISH_POINT_MAX;

		this.gainImageTransform = this.transform.FindChild ("gainImage");
		this.gainImageGameObject = this.gainImageTransform.gameObject;
	}
		
	///更新当前队伍的分数
	public override void updateTeamPoint (int point) {
		base.updateTeamPoint (point);

		//动画缩放
		LTDescr ltd;
		float scale;
		float time = 0.7f;

		if (this.gainImageGameObject.activeSelf) {
			scale = 1.02f;
			this.gainImageTransform.localScale = new Vector3 (scale, scale, scale);
			scale = 1f;
			ltd = LeanTween.scale (this.gainImageGameObject, new Vector3 (scale, scale, scale), time);
			ltd.tweenType = LeanTweenType.easeOutBack;
		}

		//缩放文字
		scale = 1f;
		Vector3 v3 = new Vector3 (scale, scale, scale);
		LeanTween.cancel (this.textGameObject, false);
		this.textGameObject.transform.localScale = v3;

		scale = 0.5f;
		ltd = LeanTween.scale (this.textGameObject, new Vector3 (scale, scale, scale), time);
		ltd.tweenType = LeanTweenType.easeOutBack;
	}

	/// 刷新持有萝卜状态
	public void updateGainImage (bool isShow) {
		LeanTween.cancel (this.gainImageGameObject, false);
		if (isShow) {
			this.ShowGainImage ();
		} else {
			this.HideGainImage ();
		}
	}

	/// 显示持有萝卜
	void ShowGainImage () {
		if (this.gainImageGameObject.activeSelf)
			return;

		//动画缩放
		float scale = 1.9f;
		this.gainImageTransform.localScale = new Vector3(scale,scale,scale);
		this.gainImageGameObject.SetActive (true);
		scale = 1f;
		LTDescr ltd = LeanTween.scale (this.gainImageGameObject, new Vector3 (scale, scale, scale), 0.3f);
		ltd.tweenType = LeanTweenType.easeOutBack;
	}

	/// 隐藏持有萝卜
	void HideGainImage () {
		this.gainImageGameObject.SetActive (false);
	}
}
