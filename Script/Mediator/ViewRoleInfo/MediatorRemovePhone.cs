using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorRemovePhone : BaseMediator {
    private GButton btn_ok;
    private GButton btn_send;
    private GButton close;
    private GTextInput input_code;
    private GTextField label_;
    private Dictionary<string, object> otherInfo;
    private DateTime timer;
    private ModelUser userModel;
    private int timerDis;

    public override void Init()
    {
		Create(Config.VIEW_REMOVEPHONE,false,Tools.GetMessageById("33107"));
        InitDate();
        InitItem();
    }
    private void InitDate()
    {
        userModel = ModelManager.inst.userModel;
        otherInfo = ModelManager.inst.roleModel.otherInfo;
        timerDis = (int)DataManager.inst.systemSimple["time_code"];
    }

    private void InitItem()
    {
//        InitTitle(Tools.GetMessageById("33107"));
        label_ = this.GetChild("n5").asTextField;
        label_.text = Tools.GetMessageById("13086")+ ":";
		label_ =this.GetChild("n6").asTextField;
		string tel = Tools.GetUserTel(userModel.tel)[1];
        string old = tel.Substring(3, 4);
        tel = tel.Replace(old, "****");
        label_.text = tel;

		input_code=this.GetChild("n3").asCom.GetChild("n1").asTextInput;
        input_code.maxLength = (int)DataManager.inst.systemSimple["code_num"];
        //input_code.promptText = Tools.GetMessageById("13098");
        btn_send =this.GetChild("n4").asButton;
        btn_send.text = Tools.GetMessageById("13077");

        GetChild("n9").asTextField.text= Tools.GetMessageById("13098")+ "：";
        btn_ok = this.GetChild("n0").asButton;
        btn_ok.text = Tools.GetMessageById("13080");
        btn_send.onClick.Add(() =>
        {
            if (label_.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13035"));
            }
            else {
				string param = "tel_num=" + Tools.GetUserTel(userModel.tel)[1];
                NetHttp.inst.Send(NetBase.HTTP_VALIDATE, param, (VoHttp v) =>
                {
                    if ((bool)v.data)
                    {
                        ViewManager.inst.ShowText(Tools.GetMessageById("13049"));
                        timer = ModelManager.inst.gameModel.time;
                        btn_send.touchable = false;
                        btn_send.grayed = true;
                        TimerManager.inst.Add(1F, 0, Timer);
                    }
                });
            }
        });

        //解除绑定
        btn_ok.onClick.Add(() =>
        {
            if (input_code.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13034"));
            }
            else if (label_.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13035"));
            }
            else {
                string param = "sign=" + input_code.text;
                NetHttp.inst.Send(NetBase.HTTP_REMOVETEL, param, (VoHttp v) =>
                {
                    Debug.Log(v.data);//true
                    if ((bool)v.data)
                    {
                        ViewManager.inst.CloseView(this);
						ViewManager.inst.ShowView<MediatorChangePhone>();
                    }
                });
            }


        });
    }

    private void Timer(float obj)
    {
        if (timerDis == 0)
        {
            btn_send.text = Tools.GetMessageById("13077");
            btn_send.touchable = true;
            btn_send.grayed = false;
            TimerManager.inst.Remove(Timer);
            timerDis= (int)DataManager.inst.systemSimple["time_code"];
        }
        else
        {
            timerDis -= 1;
            btn_send.text = Tools.GetMessageById("13119", new object[] { timerDis });
        }
       
        
    }
    public override void Clear()
    {
        base.Clear();
        TimerManager.inst.Remove(Timer);
    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView();
    }
}
