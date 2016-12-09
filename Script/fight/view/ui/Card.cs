using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Card : FightViewBase, IClientView {

    static Color colorComplete = new Color(1f, 1f, 1f);
    static Color colorUncomplete = new Color(0.7f, 0.7f, 0.7f);
    static Color colorWait = new Color(0.5f, 0.5f, 0.5f);
    //	private GUILayer testGUILayer;//定义  
    public Slider slider;
    public Image bgImg;
    public Image fillImg;
    public Image frameImg;
    public Button btn;
    //	Text costText;
    public Text timeText;

    private CardData _cardData;

    public int index;

    private bool _enabled = false;

    public CardData cardData {
        set {
            this._enabled = true;
            if(this._cardData == value) {
                return;
            }

            if(null != this._cardData) this._cardData.removeListeners(EventConstant.CHANGE);
            this._cardData = value;
            if(null != this._cardData) this._cardData.addListener(EventConstant.CHANGE, this.onComplete);


            if(null != this._cardData) {
                this.updateView();
            } else {
                this.hide();
            }
        }
        get { return this._cardData; }
    }


    

    private PlayerEntity player {
        get { return FightMain.instance.selection.localPlayer; }
    }


    private void updateView() {
        this.gameObject.SetActive(true);

        this.fillImg.sprite = this.bgImg.sprite = ResFactory.getSprite(this.cardData.id);
        this.slider.value = 1;
        this.gameObject.name = this._cardData.id;
        
        float scale1 = 0.1f;
        float scale2 = 0.67f;
        LeanTween.cancel(this.gameObject, false);
        this.gameObject.transform.localRotation = new Quaternion();
        this.gameObject.transform.localScale = new Vector3(scale1, scale1, scale1);
        LeanTween.scale(this.gameObject, new Vector3(scale2, scale2, scale2), 0.3f).tweenType = LeanTweenType.easeOutBack;
        //DOTween.To (() => this.transform.localScale, x => this.transform.localScale = x, new Vector3(scale,scale,scale), 0.3f);
    }

    protected override void preInit() {
        base.preInit();
        btn.onClick.AddListener(this.use);
    }
    

    public void onUpdate(float rate) {

        if(!this.gameObject.activeSelf) return;

        //TODO:本来人死了就退出的 但是这个人死了还要继续往前追 就临时这么写一下。
        if(null == this.player) return;

        if(null != this._cardData) {
            //手牌
            float value = (float)this._cardData.power / this._cardData.cost;
            if(value < 1) {
                this.fillImg.color = colorUncomplete;
                this.timeText.gameObject.SetActive(true);
                //通过当前自然充能速率计算，还需要几秒（向上取整）可以完成CD
                int sec = Mathf.CeilToInt((float)( this._cardData.cost - this._cardData.power ) / (FightMain.instance.selection.powerGain * 15));
                this.timeText.text = sec.ToString();
            } else {
                this.fillImg.color = colorComplete;
                this.timeText.gameObject.SetActive(false);
            }
            this.slider.value = value;
        }
        if((Input.GetKeyUp(KeyCode.J) && this.index == 0) ||
           (Input.GetKeyUp(KeyCode.K) && this.index == 1) ||
           (Input.GetKeyUp(KeyCode.L) && this.index == 2)) {
            this.use();
        }
    }

    private void onComplete(MainEvent e) {
        float scale1 = 0.65f;
        float scale2 = 0.67f;
        LeanTween.cancel(this.gameObject, false);
        LTDescr ltd1 = LeanTween.color(this.gameObject, colorComplete, 0.2f);
        ltd1.tweenType = LeanTweenType.easeOutSine;
        LTDescr ltd2 = LeanTween.scale(this.gameObject, new Vector3(scale1, scale1, scale1), 0.2f);
        ltd2.tweenType = LeanTweenType.easeOutSine;
        ltd2.onComplete = () => {
            LTDescr ltd3 = LeanTween.scale(this.gameObject, new Vector3(scale2, scale2, scale2), 0.2f);
            ltd3.tweenType = LeanTweenType.easeInSine;
        };
    }


    //使用卡牌
    public void use() {
        //player.useCard(index);
        if(this.player.map.mapData.isWatch) return;

        if(this._enabled && null != this._cardData && this._cardData.canUse) {
            this._enabled = false;
            NetAdapter.sendUseCard(this.player.uid, this._cardData.index);
        }

    }
    
	public void hide() {
        float scale1 = 0.7f;
        float scale2 = 0.1f;
        LeanTween.cancel(this.gameObject, false);
        LTDescr ltd1 = LeanTween.scale(this.gameObject, new Vector3(scale1, scale1, scale1), 0.1f);
        ltd1.tweenType = LeanTweenType.easeOutSine;
        ltd1.onComplete = () => {
            LTDescr ltd2 = LeanTween.scale(this.gameObject, new Vector3(scale2, scale2, scale2), 0.1f);
            ltd2.tweenType = LeanTweenType.easeInSine;
            ltd2.onComplete = ()=>{ this.gameObject.SetActive(false); };
        };
    }
    
    



}
