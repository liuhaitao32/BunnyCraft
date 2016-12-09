using UnityEngine;
using System.Collections;
using System;

public class Scene : FightViewBase {

	public ClientRunTime clientRunTime;


    //——————————————————————————层级——————————————————————————————
    public Transform beanLayer;
    public Transform bulletLayer;
    public Transform personLayer;
    public Transform effectLayer;
    public Transform barrierLayer;
    public GameObject cameraCanvas;
    public Camera2DFollow cameraFollow;
    public FightAudioController audioController;
    public EdgeHintController edgeHintController;
    public PointerController pointerController;

    public GameObject energeTimeLayer;

    public Transform stageLayer;
    public Transform fogLayer;
    public RadioController radioController;
    public RadioGuideController radioGuideController;

    protected override void preInit() {
        base.preInit();
    }

    public void init() {
        this.stageLayer.localScale = new Vector3(this.clientRunTime.mapData.earthScale, this.clientRunTime.mapData.earthScale, this.clientRunTime.mapData.earthScale);
        if(FightMain.fightTest) this.audioController.Mute();
        this.pointerController.init(this.clientRunTime);
        this.energeTimeLayer.SetActive(false);
        GameObject go = Tools.FindChild2("stageLayer/earth", this.gameObject).gameObject;
        ViewUtils.tweenMaterialValue(go, "_Saturation", 1f, 0.3f);
        ViewUtils.tweenMaterialColor(go, new Color(1, 1, 1, 1), 0.3f);
    }
	

    public void setRegainMap(bool visible) {
        Tools.FindChild2("regain", this.cameraCanvas).SetActive(visible);
    }

    
    public void endFight() {
        GameObject go = Tools.FindChild2("stageLayer/earth", this.gameObject).gameObject;
        ViewUtils.tweenMaterialValue(go, "_Saturation", 0.5f, 0.3f);
        ViewUtils.tweenMaterialColor(go, ViewConstant.COLOR_STANDARD, 0.3f);
    }


    public void warning() {
        GameObject go = Tools.FindChild2("stageLayer/earth", this.gameObject).gameObject;
        ViewUtils.tweenMaterialValue(go, "_Saturation", 0.6f, 0.3f);
        ViewUtils.tweenMaterialColor(go, ViewConstant.COLOR_ENERGE_TIME, 0.3f);
        this.energeTimeLayer.SetActive(true);
        //Tools.FindChild2("warning", this.cameraCanvas).SetActive(true);
    }


    public GameObject addEffect(string res, Vector2D position, float scale = 1) {
        GameObject go = ResFactory.getCacheEffect(res, this.clientRunTime);
        if(null == go) {
            MediatorSystem.log("effectLost", res);
            Debug.Log("没有资源" + res);
            return go;
        }
        go.transform.SetParent(this.effectLayer, false);
        go.transform.localPosition = ViewUtils.logicToScene(position, this.clientRunTime.mapData.earthRadius, this.clientRunTime.mapData);

        Vector3 v = new Vector3(this.transform.position.x, go.transform.position.y, this.transform.position.z);
        go.transform.LookAt(v);
        if(1 != scale) GameObjectScaler.Scale(go, scale);
        return go;
    }

    ///广播杀人事态
	public void addKillRadio(ClientPlayerEntity player1, ClientPlayerEntity player2) {
        this.radioController.addKillRadio(player1, player2);
    }
    ///广播事态
    public void addRadio(string type) {
        this.radioController.addRadio(type);
    }
    ///广播抢萝卜事态
    public void addRadishRadio(ClientPlayerEntity playerEntity, string type) {
        this.radioController.addRadishRadio(playerEntity, type);
    }

    ///广播引导类事态
	public void addRadioGuide(string info, Color color) {
        this.radioGuideController.addRadio(info, color);
    }
    ///广播引导类事态
    public void addRadioGuide(string info) {
        this.radioGuideController.addRadio(info, RadioGuideController.DEFAULT_COLOR);
    }

    public void playVoice(string voiceName, float delay = 0f) {
        AudioClip audioClip = ResFactory.loadVoiceClip(voiceName);
        GameObject go = ResFactory.loadPrefab("audio");
        go = ResFactory.createObject<GameObject>(go);
        go.name = voiceName;
        AudioSource audioSource = go.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.PlayDelayed(delay);
        ResFactory.Destroy(go, audioClip.length + delay);
        go.transform.SetParent(this.audioController.transform);
    }

    ///清除所有正在播放和延迟播放的语音
	public void clearVoice() {
        ViewUtils.clearChildren(this.audioController.transform);
    }

    public void reset() {
        ViewUtils.clearChildren(this.beanLayer);
        ViewUtils.clearChildren(this.bulletLayer);
        ViewUtils.clearChildren(this.personLayer);
        ViewUtils.clearChildren(this.effectLayer);
        ViewUtils.clearChildren(this.barrierLayer);
        this.energeTimeLayer.SetActive(false);
        this.edgeHintController.clear();
        this.cameraFollow.player = null;
    }

}
