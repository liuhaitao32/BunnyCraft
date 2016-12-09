using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// 组队赛单个比分球
public class TeamBall : MonoBehaviour {
	protected Slider slider;
	protected Image bgImage;
	protected Image fgImage;

	protected GameObject textGameObject;
	protected Text text;
	protected Outline outline;

	public int team;
	/// 当前显示的缩放等级
	public int scaleLevel;

	public virtual void Awake () {
		this.slider = this.transform.GetComponent<Slider>();
        this.slider.maxValue = ConfigConstant.TEAM_SCORE_MAX / ConfigConstant.SCORE_UNIT;

		this.bgImage = this.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>();
		this.fgImage = this.transform.FindChild("Fg").GetComponent<Image>();

		Transform textTransform;
		textTransform = this.transform.FindChild ("Text");
		this.textGameObject = textTransform.gameObject;
		this.text = textTransform.GetComponent<Text>();
		this.outline = textTransform.GetComponent<Outline>();
	}
	///初始化
	public void init (Transform parentTransform, int team) {
		GameObject go = this.gameObject;
		go.transform.SetParent(parentTransform,false);
		go.GetComponent<RectTransform> ().anchoredPosition = new Vector2 ((team-1)*95,0);
		this.team = team;
		this.scaleLevel = 0;
	}

	/// 刷新颜色
	public void changeColor (int teamIndex) {
		Color teamColor = ViewConstant.teamColors [teamIndex][2];

		this.bgImage.color = teamColor;
		this.fgImage.color = teamColor;

		teamColor.a = 0.5f;
		this.outline.effectColor = teamColor;
	}
	///更新当前缩放
	public void updateScale (int scaleLevel) {
		if (this.scaleLevel == scaleLevel) {
			return;
		}
		this.scaleLevel = scaleLevel;

		LeanTween.cancel (this.gameObject, false);
		//动画缩放
		LTDescr ltd;
		float scale;
		if (scaleLevel > 0) {
			scale = 1.1f;
		}
		else if (scaleLevel < 0) {
			scale = 0.9f;
		}
		else {
			scale = 1f;
		}

		ltd = LeanTween.scale (this.gameObject, new Vector3 (scale, scale, scale), 0.3f);
		ltd.tweenType = LeanTweenType.easeOutSine;
	}
	///更新当前队伍的分数
	public virtual void updateTeamPoint (int point) {
		this.slider.value = point;
		//更新文字
		this.text.text = point.ToString ();
	}
}
