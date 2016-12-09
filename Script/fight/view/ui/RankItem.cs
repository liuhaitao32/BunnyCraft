using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RankItem:IClear {
    static Color COLOR_SELF = new Color(1, 1, 1, 1f);
    static Color COLOR_OTHER = new Color(0, 0, 0, 0.6f);
    static Color COLOR_FONT = new Color(1, 1, 1, 0.9f);

    public int index;  //0自身
    private PlayerEntity _player;


    private Image bg;
    private Text nameText;
    private Text scoreText;
    private Text indexText;

    public GameObject go;

    private bool _cleared = false;

    public bool cleared {
        get {
            return this._cleared;
        }
    }

    public RankItem() {

    }

    public void init(GameObject parent, int index) {        
        this.go = ResFactory.loadPrefab("scoreRankItem");
        this.go = ResFactory.createObject<GameObject>(this.go);
        this.bg = this.go.transform.FindChild("bgImage").GetComponent<Image>();
        this.nameText = this.go.transform.FindChild("nameText").GetComponent<Text>();
        this.scoreText = this.go.transform.FindChild("scoreText").GetComponent<Text>();
        this.indexText = this.go.transform.FindChild("indexText").GetComponent<Text>();
        this.go.transform.SetParent(parent.transform, false);
    }
    

    public void hide() {
        this.go.SetActive(false);
        //		indexText.text = (i+1).ToString();
    }
    public void changeIndexText(int i, ClientPlayerEntity player) {
        this.go.SetActive(true);
        this.indexText.text = ( i + 1 ).ToString();
        

        this.nameText.text = player.name;
        this.nameText.color = player.getTeamColor(0);
        this.scoreText.text = ( player.fightResult.score / ConfigConstant.SCORE_UNIT ).ToString();

        int colorIndex = player.teamIndex;

        if(player.isLocalPlayer) {
            bg.color = COLOR_SELF;
        } else {
            bg.color = COLOR_OTHER;
        }
    }

    public void clear() {
        this._cleared = true;
        ResFactory.Destroy(this.go);
    }
}
