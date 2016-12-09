using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AutoRunClient {



    private int _totalIndex = 0;

    private int _stepIndex = 0;


    public List<List<Dictionary<string, object>>> steps = new List<List<Dictionary<string, object>>>();


    public AutoRunClient(){

    }


    public void startFight(object[] keyFrame, string uid, int teamIndex, int mode) {
        this.steps.Clear();
        for(int i = 0, len = keyFrame.Length; i < len; i++) {
            object[] keyDatas = (object[])keyFrame[i];
            List<Dictionary<string, object>> keyFrameList = new List<Dictionary<string, object>>();
            this.steps.Add(keyFrameList);
            for(int j = 0, len2 = keyDatas.Length; j < len2; j++) {
                List<object> list = new List<object>((object[])keyDatas[j]);
                Dictionary<string, object> data = (Dictionary<string, object>)list[1];
                string u = null == list[2] ? "" : list[2].ToString();
                string key = list[0].ToString();
                keyFrameList.Add(new Dictionary<string, object> {
                    {"type", key },
                    {"data", data },
                    {"uid", u }
                });
            }
        }
        FightMain.instance.clearFight();
        FightMain.instance.gotoLobby();
        FightMain.instance.mode = mode;
        TimerManager.inst.Add(1f * ConfigConstant.MAP_ACT_TIME_S / 1000, 0, this.sendClient);
        FightMain.instance.addUser(uid, teamIndex);
        FightMain.instance.startFight();
        this._stepIndex = 0;
        this._totalIndex = this.steps.Count;
    }

    private void sendClient(float time) {
        //判断不是最后一帧 并且到了指定的帧 就停止。
        if(this._stepIndex == this._totalIndex && this._totalIndex < this.steps.Count) {
            FightMain.instance.selection.pause = true;
            return;
        }
        //if(FightMain.instance.selection.pause) return;
        if(this.steps.Count - 1 > this._stepIndex) {
            NetAdapter.receive(FightMain.instance.selection.uid, this.steps[this._stepIndex++]);
        } else {
            this.endFight();
            FightMain.instance.gotoLobby();
        }
    }

    public void endFight() {
        TimerManager.inst.Remove(this.sendClient);
    }


    public int totalIndex {
        get {
            return this._totalIndex;
        }

        set {
            this._totalIndex = Math.Max(Math.Min(value, this.steps.Count), FightMain.instance.selection.stepIndex);
        }
    }

}
