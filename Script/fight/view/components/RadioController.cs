using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

///播报控制器
///author Biggo
public class RadioController : MonoBehaviour {
	static Vector3 bgImagePosition0 = new Vector3();
	static Vector3 bgImagePosition1 = new Vector3 (125,0,0);
	static Vector2 radioTextSizeDelta0 = new Vector2(150,60) ;
	static Vector2 radioTextSizeDelta1 = new Vector2(320,60) ;
	/// 语音源
	//AudioSource audioSource;

	Image bgImage;
	Image bgInnerImage;
	Image player1FrameImage;
	Image player1FaceImage;

	Image player2FrameImage;
	Image player2FaceImage;
	Image deadImage;
	Text radioText;
	Outline radioTextOutline;
//	Image cardImage;
	CanvasGroup canvasGroup;
	///闪电动画
	GameObject lightningGo1;
	Animator lightningAnimator1;
	GameObject lightningGo2;
	Animator lightningAnimator2;

	Vector3 player1LocalPosition;
	Vector3 player2LocalPosition;
	Vector3 textLocalScale;
	Vector3 deadLocalScale;

	Color colorStart = new Color (1, 1, 1, 0);

	List<RadioInfo> radiolist;
	RadioInfo currentRadio;
	///当前广播已经停留时间
	float currentRadioTime;
	int offsetTween = 20;
	bool isSwitch = true;

	///初始化
	public void Start () {
		//this.audioSource = this.GetComponent<AudioSource> ();

		this.bgImage = this.transform.FindChild ("bgImage").GetComponent<Image>();
		this.bgInnerImage = this.bgImage.transform.FindChild ("bgImage").GetComponent<Image>();
//		lineImage = this.transform.FindChild ("lineImage").GetComponent<Image>();

		this.lightningGo1 = this.transform.FindChild ("lightning1").gameObject;
		this.lightningAnimator1 = this.lightningGo1.GetComponent<Animator> ();
		this.lightningGo2 = this.transform.FindChild ("lightning2").gameObject;
		this.lightningAnimator2 = this.lightningGo2.GetComponent<Animator> ();

		Transform tempTransform;
		tempTransform = this.transform.FindChild ("player1");
		player1FrameImage = tempTransform.GetComponent<Image>();
		player1LocalPosition = player1FrameImage.transform.localPosition;
		player1LocalPosition.x -= offsetTween;
		player1FaceImage = tempTransform.FindChild ("faceImage").GetChild(0).GetComponent<Image>();

		tempTransform = this.transform.FindChild ("player2");
		player2FrameImage = tempTransform.GetComponent<Image>();
		player2LocalPosition = player2FrameImage.transform.localPosition;
		player2LocalPosition.x += offsetTween;
		player2FaceImage = tempTransform.FindChild ("faceImage").GetChild(0).GetComponent<Image>();
		deadImage = tempTransform.FindChild ("deadImage").GetComponent<Image>();

		tempTransform = this.transform.FindChild ("radioText");
		radioText = tempTransform.GetComponent<Text>();
		radioTextOutline = tempTransform.GetComponent<Outline>();
//		cardImage = radioText.transform.FindChild ("cardImage").GetComponent<Image>();

		textLocalScale = radioText.transform.localScale;
		deadLocalScale = deadImage.transform.localScale;

		canvasGroup = this.transform.GetComponent<CanvasGroup>();

		radiolist = new List<RadioInfo> ();
		clear ();
	}
	void Update () {
		if (isSwitch) {
			return;
		}

		int count = radiolist.Count;
		//当前播报
		if (currentRadio != null) {
			currentRadioTime += Time.deltaTime * 1000;
			//后面排队的很多，就尽快播完
			if (count > 0) {
				if (currentRadioTime >= currentRadio.timeMin) {
					hideRadio ();
				}
			}
			else if (currentRadioTime >= RadioConfig.RADIO_TIME_MAX) {
				hideRadio ();
			}
		}
		else if (count > 0) {
			popRadio ();
		}
	}

	///加入一条播报
	public void addRadio (string type) {
		RadioInfo radio = new RadioInfo (type);
		this.radiolist.Add (radio);
	}
	///加入一条抢萝卜播报
    public void addRadishRadio(ClientPlayerEntity playerEntity, string type) {
        RadioInfo radio = new RadioInfo(playerEntity, type);
        radiolist.Add(radio);
    }

    ///加入一条击杀播报
    public void addKillRadio (ClientPlayerEntity player1, ClientPlayerEntity player2) {
		RadioInfo radio = new RadioInfo (player1, player2);
        this.radiolist.Add (radio);
	}


	///弹出并显示一条播报
	public void popRadio () {
		clearTween ();
		if (radiolist.Count > 0) {
			LTDescr ltd;


			currentRadio = radiolist [0];
			radiolist.RemoveAt (0);
			currentRadioTime = 0;
			//temp
//			currentRadio.scale = 2f;

			//首阶段显现时间
			float timeApper = 0.2f / currentRadio.scale;
			//次阶段缓动动画时间
			float timeEnter = 0.1f;
			//第三阶段缓动动画时间
			float timeNext = 0.2f;
			//最后阶段缓动动画时间
			float timeFinish = 0.2f;

			Color outlineColor;
			//更新双方和文字
			if (currentRadio.player1 != null) {
				player1FaceImage.gameObject.SetActive (true);
				player1FrameImage.gameObject.SetActive (true);
                ResFactory.setHeadSprite(currentRadio.player1.headUrl, player1FaceImage, currentRadio.player1.uid);
                player1FrameImage.color = currentRadio.player1.getTeamColor (2);
//				lineImage.color = currentRadio.player1.getTeamColor (2);

				//头像偏移
				ltd = LeanTween.moveLocalX (player1FrameImage.gameObject, player1LocalPosition.x + offsetTween, timeEnter);
				ltd.tweenType = LeanTweenType.easeOutSine;
				ltd.delay = timeApper * 0.5f;

				outlineColor = currentRadio.player1.getTeamColor (2);
	
				bgImage.transform.localPosition = bgImagePosition0;
				bgInnerImage.gameObject.SetActive (false);
				radioText.rectTransform.sizeDelta = radioTextSizeDelta0;
			}
			else {
				//没有主动玩家，一般性播报
				player1FaceImage.gameObject.SetActive (false);
				player1FrameImage.gameObject.SetActive (false);

				outlineColor = currentRadio.color;

				bgImage.transform.localPosition = bgImagePosition1;
				bgInnerImage.gameObject.SetActive (true);
				radioText.rectTransform.sizeDelta = radioTextSizeDelta1;
			}
	
			if (currentRadio.player2 != null) {
				player2FaceImage.gameObject.SetActive (true);
				player2FrameImage.gameObject.SetActive (true);
                ResFactory.setHeadSprite(currentRadio.player2.headUrl, player2FaceImage, currentRadio.player2.uid);
                player2FrameImage.color = currentRadio.player2.getTeamColor (2);

				ltd = LeanTween.moveLocalX(player2FrameImage.gameObject,player2LocalPosition.x - offsetTween,timeEnter);
				ltd.tweenType = LeanTweenType.easeOutSine;
				ltd.delay = timeApper*0.5f;

				//击杀红叉图片缩放，透明显现变化颜色
				float scale = 3f;
				Vector3 v3 = new Vector3 (scale,scale,scale);
				deadImage.transform.localScale = v3;
				ltd = LeanTween.scale (deadImage.gameObject,deadLocalScale,timeFinish);
				ltd.tweenType = LeanTweenType.easeOutBack;
				ltd.delay = timeApper + timeEnter + timeNext*0.5f;

				deadImage.color = colorStart;
				ltd = LeanTween.value (this.gameObject, (Color color) => 
					{
						deadImage.color = color;
					},
					colorStart,
					Color.white,
					timeFinish*0.5f
				);
				ltd.tweenType = LeanTweenType.easeOutSine;
				ltd.delay = timeApper + timeEnter + timeNext*0.5f;

			} else {
				player2FaceImage.gameObject.SetActive (false);
				player2FrameImage.gameObject.SetActive (false);
			}
				
			//文字和外发光
			radioTextOutline.effectColor = outlineColor;
			radioText.text = currentRadio.info;

			//音效
			if (currentRadio.player1 != null && currentRadio.player2 != null) {
				int player1Type = currentRadio.player1.teamType;
				int player2Type = currentRadio.player2.teamType;
				if (player1Type == 0) 
					playVoice(currentRadio.voiceIDo);
				else if (player2Type == 0) 
					playVoice(currentRadio.voiceDoMe);
				else if (player1Type == 1) 
					playVoice(currentRadio.voiceAllyDo);
				else if (player1Type == 2) 
					playVoice(currentRadio.voiceEnermyDo);
				else 
					playVoice(currentRadio.voice);
			} else {
				playVoice(currentRadio.voice);
			}

//			Vector3 position;
//			if (false && currentRadio.cardId != null) {
//				//有击杀卡片展示
//				cardImage.gameObject.SetActive (true);
//				cardImage.sprite = ViewUtils.LoadSprite(currentRadio.cardId);
//
//				position = new Vector3();
//				position.x = -radioText.preferredWidth * 0.5f - 22f;
//				cardImage.transform.localPosition = position;
//	
//				position = new Vector3();
//				position.x = 22f;
//				radioText.transform.localPosition = position;
//			} else {
				//无击杀卡片展示
//				cardImage.gameObject.SetActive (false);
//
//				position = new Vector3();
//				radioText.transform.localPosition = position;
//			}


			GameObject lightningGo = null;
			Animator lightningAnimator = null;
			if (currentRadio.lightningScale > 0) {
				if (currentRadio.player1 == null || currentRadio.player1.teamType <= 1) {
					//蓝色闪电
					lightningGo = lightningGo1;
					lightningAnimator = lightningAnimator1;
				} else {
					//红色闪电
					lightningGo = lightningGo2;
					lightningAnimator = lightningAnimator2;
				}
			}

			//击杀文字缩放
			if(currentRadio.scale > 1f)
			{
				float scale1 = (currentRadio.scale - 1f)*5 + 1;
				float scale2 = (currentRadio.scale - 1f)*2 + 1;
				Vector3 v3_1 = new Vector3 (scale1,scale1,scale1);
				Vector3 v3_2 = new Vector3 (scale2,scale2,scale2);

				radioText.transform.localScale = v3_1;

				ltd = LeanTween.scale (radioText.gameObject,v3_2,timeEnter);
				ltd.delay = timeApper*0.5f;
				ltd.onComplete = () => {
					LTDescr ltd_= LeanTween.scale (radioText.gameObject,textLocalScale,timeNext);
					ltd_.tweenType = LeanTweenType.easeOutBack;

					if(lightningGo != null)
					{
						float scale = currentRadio.lightningScale;
						Vector3 v3 = new Vector3 (scale,scale,scale);
						LeanTween.delayedCall(timeEnter*0.5f,() => {
							//闪电
							lightningAnimator.Play ("play");
						});
					}
				};
				//透明显现
				radioText.color = colorStart;
				ltd = LeanTween.value (this.gameObject, (Color color) => 
					{
						radioText.color = color;
					},
					colorStart,
					Color.white,//player1Color
					timeEnter
				);
				ltd.delay = timeApper*0.5f;
			}

			//整体透明显现
			ltd = LeanTween.value (this.gameObject, (float value) => 
				{
					canvasGroup.alpha = value;
				},
				canvasGroup.alpha,
				1,
				timeApper
			);
			ltd.onComplete = () => {
				isSwitch = false;
			};


		}
	}


	///隐藏当前播报
	public void hideRadio () {
//		ClearTween ();
		isSwitch = true;
		LTDescr ltd = LeanTween.value (this.gameObject, (float value) => 
			{
			canvasGroup.alpha = value;
			},
			canvasGroup.alpha,
			0,
			0.3f
		);
		ltd.onComplete = () => {
			isSwitch = false;
			currentRadio = null;
		};
	}
	///加入一条语音并播放，延迟播放
	public void playVoice (string voiceName) {
		if (voiceName != null && !FightMain.instance.selection.refereeController.isEnd) {
			FightMain.instance.selection.scene.playVoice (voiceName, 1.0f);
		}
	}

	public void clearTween () {
		LeanTween.cancel (this.gameObject, false);
		LeanTween.cancel (player1FrameImage.gameObject, false);
		LeanTween.cancel (player2FrameImage.gameObject, false);
		LeanTween.cancel (radioText.gameObject, false);
		LeanTween.cancel (deadImage.gameObject, false);

		player1FrameImage.transform.localPosition = player1LocalPosition;
		player2FrameImage.transform.localPosition = player2LocalPosition;
		radioText.transform.localScale = textLocalScale;
	}

	public void clear () {
		radiolist.Clear ();
		clearTween ();
		canvasGroup.alpha = 0f;

		currentRadio = null;
		currentRadioTime = 0;
		isSwitch = false;
	}
}
