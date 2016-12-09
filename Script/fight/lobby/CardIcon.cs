using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardIcon:EventClass {

    public Button btn;

    public Image image;

    public string id;

    public CardIcon(GameObject go, string id) {
        this.id = id;
        this.btn = go.GetComponent<Button>();
        this.image = go.GetComponent<Image>();
        this.btn.onClick.AddListener(this.onClick);
        this.updateView();
    }

    private void onClick() {
        LobbyMain.instance.allCards.Add(this.id);
        this.id = LobbyMain.instance.allCards[0];
        LobbyMain.instance.allCards.RemoveAt(0);
        this.updateView();
        LobbyMain.instance.setCardsData();
    }

    void updateView() {
        this.image.sprite = ResFactory.getSprite(this.id);
    }
}
