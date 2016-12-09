using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Rank {

    private List<RankItem> _rankList = new List<RankItem>();

    private int _itemCount = -1;

    private ClientPlayerEntity player {
        get { return FightMain.instance.selection.localPlayer; }
    }

    public Rank() {

    }
    

    public void init(GameObject go) {
        Utils.clearList(this._rankList);

        if(FightMain.instance.selection.mapData.isRadish) {
            this._itemCount = 1;
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10, -100);
        } else {
            this._itemCount = 6;
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10, -5);
        }

        for(int i = 0, len = this._itemCount; i < len; i++) {
            RankItem item = new RankItem();
            item.init(go, i);
            
            if(i == len - 1) {
                item.go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (i) * -26 - 2);
            } else {
                item.go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (i) * -26);
            }
            
            this._rankList.Add(item);
        }

        FightMain.instance.selection.addListener(EventConstant.LOGIC_COMPLETE, onFrameHandler);
    }
    

    private void onFrameHandler(MainEvent e) {
        if(TestValue.test1) return;
        MediatorSystem.timeStart("rank");
        List<PlayerEntity> playerList = FightMain.instance.selection.getSortPlayer(1);
        int count = Math.Min(playerList.Count, this._rankList.Count);
        for(int i = 0, len = this._rankList.Count; i < len; i++) {
            //最后一个显示自己 并且要求还得都显示的下！
            if(i >= count) {
                this._rankList[i].hide();
            } else {
                if(i == this._rankList.Count - 1) {
                    int index = playerList.IndexOf(this.player);
                    if(index >= i) {//如果自己不在之前的排行榜里 就显示。
                        this._rankList[i].changeIndexText(index, this.player);
                    } else {
                        this._rankList[i].hide();
                    }
                } else {
                    this._rankList[i].changeIndexText(i, (ClientPlayerEntity)playerList[i]);
                }
            }
            
        }
        MediatorSystem.getRunTime("rank");
    }
    

}
