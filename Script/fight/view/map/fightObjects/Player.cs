using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : Person {
    
    protected ClientPlayerEntity _playerEntity;
    public ClientPlayerEntity playerEntity { get { return this._playerEntity; } }

    public AvatarView avatar;
	//Biggo添加
	public Text nameText;

    public List<PartSlider> parts = new List<PartSlider>();

    public float mainScale = 1;

    public override FightEntity fightEntity {
        set {
            base.fightEntity = value;
            this._playerEntity = (ClientPlayerEntity)this._fightEntity;
        }
    }

    protected override void preInit() {
        base.preInit();
        this.nameText = Tools.FindChild2("personEarth/Canvas/nameText", this.gameObject).GetComponent<Text>();
    }


    public void shipEntryTween(bool isReset) {
        this.gameObject.SetActive(true);
        float scaleStart;
        float scaleMiddle;
        float scaleEnd = Convert.ToSingle(((Dictionary<string, object>)ConfigConstant.scoreConfig[this._playerEntity.fightResult.scoreLevel])["mainScale"]);
        if(isReset)
            scaleStart = scaleEnd * 0.1f;
        else
            scaleStart = scaleEnd + 0.8f;

        scaleMiddle = scaleEnd * 0.9f;

        this.avatar.scaleShip(scaleStart, scaleMiddle, scaleEnd);
    }

    public override void reset() {
        base.reset();
        this.avatar.reset();
    }

    public override void init () {

        this.mainScale = Convert.ToSingle(( (Dictionary<string, object>)ConfigConstant.scoreConfig[this._playerEntity.fightResult.scoreLevel] )["mainScale"]);
        this.avatar = ResFactory.createObject<AvatarView>(ResFactory.instance.avatar);
        this.avatar.init(Tools.FindChild2("ship", this.skewBody.gameObject).transform, this._playerEntity);
        base.init ();


        /****************************加入边界************************/
        this.scene.edgeHintController.addFightObject(this._playerEntity.teamIndex == this.clientRunTime.teamIndex ? 1 : 2, this._fightEntity);
        /****************************名字************************/
		//Biggo修改
		this.nameText.text = this._playerEntity.name;
		this.nameText.color = this._playerEntity.getTeamColor(0);
        ResFactory.setHeadSprite(this._playerEntity.headUrl, Tools.FindChild2("personEarth/Canvas/faceImage/Image", this.gameObject).GetComponent<Image>(), this._playerEntity.uid);
        /****************************outLine************************/
        Tools.FindChild2("personEarth/Canvas/nameText", this.gameObject).GetComponent<Outline>().effectColor = this._playerEntity.getTeamColor(1);
        /****************************frameImage************************/
        Tools.FindChild2("personEarth/Canvas/frameImage", this.gameObject).GetComponent<Image>().color = this._playerEntity.getTeamColor(2);        
        /****************************装备************************/
        for(int i = 0, len = ConfigConstant.PART_COUNT; i < len; i++) {
            this.parts.Add(new PartSlider(this, i));
        }
        this.shipEntryTween(true);
        /****************************事件************************/
        //TODO:这个是否要与callView  dead进行融合？？？
        this._fightEntity.addListener(EventConstant.ALIVED, (e) => {
            this.gameObject.SetActive(this._fightEntity.alived);
            this.changeMove(1);
            if(!this._fightEntity.alived) {
                //死亡了。。。
                this.clientRunTime.addEffect("bangDead", this._fightEntity.position);
            } else {
                this.avatar.reset();
                this.shipEntryTween(true);
                this.clientRunTime.addEffect("bangBorn", this._fightEntity.position);
                for(int i = 0; i < ConfigConstant.PART_COUNT; i++) {
                    this.parts[i].updatePart();
                }
            }
        });

        this._playerEntity.addListener(EventConstant.LEVEL_CHANGE, (MainEvent e) => {
            this.mainScale = Convert.ToSingle(( (Dictionary<string, object>)ConfigConstant.scoreConfig[this._playerEntity.fightResult.scoreLevel] )["mainScale"]);
            this.shipEntryTween(false);
            Dictionary<string, object> scoreDic = (Dictionary<string, object>)e.data;
            if((int)scoreDic["newLevel"] > (int)scoreDic["oldLevel"]) {
                this.addEffect("bangScore");
                this.doAvatarAnimation("score");
            }
            
        });

        this._playerEntity.addListener(EventConstant.KILL_PLAYER, (MainEvent e) => {
            List<ClientPlayerEntity> players = (List<ClientPlayerEntity>)e.data;
            this.scene.addKillRadio(players[0], players[1]);

        });

        this.gameObject.SetActive(this._fightEntity.alived);
    }

    public void doAvatarAnimation(string key) {
        this.avatar.doAvatarAnimation(key);
        if(key == "poss") {
            this.addEffect("bangKillHappy");
        }
    }

    private Vector3 v3 = new Vector3();

    void Update() {
        //Debug.Log(Vector3.Distance(v3, this.transform.localPosition));
        //v3 = this.transform.localPosition;
    }


    public override void onUpdate (float rate) {
		base.onUpdate (rate);
        for(int i = 0; i < ConfigConstant.PART_COUNT; i++) {
            this.parts[i].onUpdate();
        }
        
    }
    
}

