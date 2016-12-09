using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;
using System.IO;


public class EqualPanel : MonoBehaviour {

    public Text fightId;

    public Button close_btn;

    public Text info;

    void Start() {
        this.close_btn.onClick.AddListener(this.hide);
    }

    public void show(Dictionary<string, object> data) {
        this.gameObject.SetActive(true);
        string str = "用户：" + FightMain.instance.selection.uid + "  关键帧：" + (FightMain.instance.selection.startStepIndex + FightMain.instance.selection.stepIndex) +  "\n";
        this.info.text = ViewUtils.toString(data);
        NetAdapter.sendQuitFight();
    }


    public void hide() {
        this.gameObject.SetActive(false);
    }
    
}
