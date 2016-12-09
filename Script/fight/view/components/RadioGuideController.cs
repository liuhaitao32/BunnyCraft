using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

///播报控制器
///author Biggo
public class RadioGuideController : MonoBehaviour {
	public static Color DEFAULT_COLOR = new Color(0.5f,0.2f,0f);

	/// 语音源
	AudioSource audioSource;

	Text radioText;
	Outline radioTextOutline;
	CanvasGroup canvasGroup;

	Vector3 textLocalScale;
	Vector3 deadLocalScale;

	List<RadioInfo> radiolist = new List<RadioInfo>();
	RadioInfo currentRadio;
	///当前广播已经停留时间
	float currentRadioTime;
	bool isSwitch = true;

	///初始化
	public void Awake () {
		this.audioSource = this.GetComponent<AudioSource> ();

		Transform tempTransform;
		tempTransform = this.transform.FindChild ("radioText");
		this.radioText = tempTransform.GetComponent<Text>();
		this.radioTextOutline = tempTransform.GetComponent<Outline>();

		this.textLocalScale = this.radioText.transform.localScale;

		this.canvasGroup = this.transform.GetComponent<CanvasGroup>();
        this.Clear();
	}
	void Update () {
		if (this.isSwitch) {
			return;
		}

		int count = this.radiolist.Count;
		//当前播报
		if (this.currentRadio != null) {
			this.currentRadioTime += Time.deltaTime * 1000;
			//后面排队的很多，就尽快播完
			if (count > 0) {
				if (this.currentRadioTime >= 500) {
					this.HideRadio ();
				}
			}
			else if (this.currentRadioTime >= RadioConfig.RADIO_GUIDE_TIME_MAX) {
				this.HideRadio ();
			}
		}
		else if (count > 0) {
			this.PopRadio ();
		}
	}

	///加入一条播报
	public void addRadio (string info , Color color) {
		if (null == color)
			color = Color.gray;
		RadioInfo radio = new RadioInfo (info,color);
		this.radiolist.Add (radio);
	}



	///弹出并显示一条播报
	public void PopRadio () {
		this.ClearTween ();
		if (this.radiolist.Count > 0) {
			LTDescr ltd;

			this.currentRadio = this.radiolist [0];
			this.radiolist.RemoveAt (0);
			this.currentRadioTime = 0;

			//首阶段显现时间
			float timeApper = 0.1f;
			//次阶段缓动动画时间
			float timeEnter = 0.1f;
			//第三阶段缓动动画时间
			float timeNext = 0.1f;

			this.radioTextOutline.effectColor = this.currentRadio.color;
			this.radioText.text = this.currentRadio.info;
			//文字缩放
			float scale1 = 1.5f;
			float scale2 = 1.2f;
			Vector3 v3_1 = new Vector3 (scale1,scale1,scale1);
			Vector3 v3_2 = new Vector3 (scale2,scale2,scale2);

			this.radioText.transform.localScale = v3_1;

			ltd = LeanTween.scale (this.radioText.gameObject,v3_2,timeEnter);
			ltd.delay = timeApper*0.5f;
			ltd.onComplete = () => {
				LTDescr ltd_= LeanTween.scale (this.radioText.gameObject,this.textLocalScale,timeNext);
				ltd_.tweenType = LeanTweenType.easeOutBack;
			};

			//整体透明显现
			ltd = LeanTween.value (this.gameObject, (float value) => 
				{
					this.canvasGroup.alpha = value;
				},
				this.canvasGroup.alpha,
				1,
				timeApper
			);
			ltd.onComplete = () => {
				this.isSwitch = false;
			};
		}
	}


	///隐藏当前播报
	public void HideRadio () {
//		ClearTween ();
		this.isSwitch = true;
		LTDescr ltd = LeanTween.value (this.gameObject, (float value) => 
			{
				this.canvasGroup.alpha = value;
			},
			this.canvasGroup.alpha,
			0,
			0.2f
		);
		ltd.onComplete = () => {
			this.isSwitch = false;
			this.currentRadio = null;
		};
	}

	public void ClearTween () {
		LeanTween.cancel (this.gameObject, false);
		LeanTween.cancel (this.radioText.gameObject, false);
		this.radioText.transform.localScale = this.textLocalScale;
	}

	public void Clear () {
		this.radiolist.Clear ();
		this.ClearTween ();
		this.canvasGroup.alpha = 0f;

		this.currentRadio = null;
		this.currentRadioTime = 0;
		this.isSwitch = false;
	}
}
