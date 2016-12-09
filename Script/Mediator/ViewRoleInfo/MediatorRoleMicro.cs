using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorRoleMicro : BaseMediator {
    private GButton btnMicroReStart;
    private GButton btnMicroStart;
    private GTextField textMicro;
    private GButton btnMicroUpload;
    private ComProgressBar progressMicro;
    private GTextField timeMicro;
    private GButton btnMicroStop;
    private int miro_time;
    private GTextField timeMicro1;
    private GTextField timeMicro2;
    private Action<float> miroAc;
    private bool isClose=false;

    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_ROLEMICRO);
        btnMicroReStart = GetChild("n7").asButton;
        btnMicroStart = GetChild("n5").asButton;
        btnMicroStop = GetChild("n8").asButton;
        textMicro = GetChild("n3").asTextField;
        textMicro.text = Tools.GetMessageById("13162");
        btnMicroUpload = GetChild("n6").asButton;
        progressMicro = this.GetChild("n4").asCom as ComProgressBar;
        progressMicro.skin = ComProgressBar.BAR12;
        progressMicro.SetTextVisible(false);
        progressMicro.value = 0;
        progressMicro.max = 100;
        timeMicro = GetChild("n2").asTextField;
        timeMicro2 = GetChild("n11").asTextField;
        timeMicro1 = GetChild("n12").asTextField;
        btnMicroStart.onClick.Add(BtnMicroStart);
        
        btnMicroStop.onClick.Add(BtnMicroStop);
        btnMicroUpload.onClick.Add(BtnMicroUpload);
        btnMicroReStart.onClick.Add(BtnMicroReStart);
        miro_time = (int)DataManager.inst.systemSimple["miro_time"]*1000;

    }

    private void BtnMicroReStart(EventContext context)
    {
        
    }

    private void BtnMicroUpload(EventContext context)
    {
        //上传成功后 isUploadOver=true
        ModelRole.isUploadOver = true;
        ModelRole.isHasMicro = true;
    }



    private void BtnMicroStop(EventContext context)
    {
        ModelRole.isCloseMask = false;
        TimerManager.inst.Remove(miroAc);
        btnMicroStart.visible = true;
        btnMicroStart.touchable = true;
        btnMicroStart.grayed = true;
        btnMicroStart.touchable = false;
        btnMicroStop.visible = false;
        btnMicroReStart.touchable = true;
        btnMicroUpload.touchable = true;
        textMicro.visible = false;



    }

    private void BtnMicroStart(EventContext context)
    {
        btnMicroStart.visible = false;
        btnMicroStop.visible = true;
        btnMicroReStart.visible = true;
        btnMicroReStart.touchable = false;
        textMicro.text = Tools.GetMessageById("13163");

        miroAc=TimerManager.inst.Add(0.1f,0,(float t)=> {
            miro_time -=100;
            if (miro_time <= 0)
            {
                ModelRole.isCloseMask =false;
                progressMicro.value = 100;
                TimerManager.inst.Remove(miroAc);
                timeMicro.text = "00" + "：";
                timeMicro1.text = "00" + "、";
                timeMicro2.text = "00";
            }
            else
            {
                Debug.Log(miro_time);
                ModelRole.isCloseMask = true;
                progressMicro.value += 1;
                string[] timerArr = Tools.miro(miro_time);
                timeMicro.text = timerArr[1] + "：";
                timeMicro1.text = timerArr[2] + "、";
                timeMicro2.text = timerArr[3];
            }

        });


    }

    public override void Clear()
    {
        base.Clear();
        Dictionary<string, object> dc = new Dictionary<string, object>();
        dc.Add("value", "");
        dc.Add("tag", "micro");
        DispatchGlobalEvent(new MainEvent(MainEvent.ROLE_UPDATE, dc));
    }

  

    public override void Close()
    {
        base.Close();
        Dictionary<string, object> dc = new Dictionary<string, object>();
        dc.Add("value", "");
        dc.Add("tag", "micro");
        DispatchGlobalEvent(new MainEvent(MainEvent.ROLE_UPDATE, dc));
    }

}
