using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorChangePhone : BaseMediator {
    private GButton btn_ok;
    private GButton btn_send;
    private GButton close;
    private GTextInput input_code;
    private GTextInput input_password;
    private GTextInput input_phone;
    private GTextInput input_username;
    private Dictionary<string, object> otherInfo;
    private DateTime timer;
    private ModelUser userModel;
    private int timerDis;
    private GTextField title;

    public override void Init()
    {
        base.Init();
		Create(Config.VIEW_CHANGEPHONE,false,Tools.GetMessageById("33105"));
        userModel = ModelManager.inst.userModel;
        otherInfo = ModelManager.inst.roleModel.otherInfo;
        timerDis = (int)DataManager.inst.systemSimple["time_code"];
        InitItem();
    }
    private void InitItem()
    {
//        InitTitle(Tools.GetMessageById("33105"));
        input_phone = this.GetChild("n4").asCom.GetChild("n1").asTextInput;
        input_phone.maxLength = 11;
        //input_phone.promptText = Tools.GetMessageById("13086")+":";
        input_code = this.GetChild("n5").asCom.GetChild("n1").asTextInput;
        input_code.maxLength = (int)DataManager.inst.systemSimple["code_num"];
        //input_code.promptText = Tools.GetMessageById("13098");


        GetChild("n8").asTextField.text = Tools.GetMessageById("13140") + "：";
        GetChild("n9").asTextField.text = Tools.GetMessageById("13098") + "：";

        btn_send = this.GetChild("n6").asButton;
        btn_send.text = Tools.GetMessageById("13077");
        btn_send.onClick.Add(() =>
        {
            if (input_phone.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13035"));
            }
            else
            {
                if (input_phone.text.Equals(Tools.GetUserTel(userModel.tel)[1]))
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13142"));
                }
                else
                {
                    string param = "tel_num=" + input_phone.text;
                    NetHttp.inst.Send(NetBase.HTTP_VALIDATE, param, (VoHttp v) =>
                    {
                        //                    Debug.Log(v.data);//true
                        //                    if ((bool)v.data)
                        //                    {
                        ViewManager.inst.ShowText(Tools.GetMessageById("13049"));
                        timer = ModelManager.inst.gameModel.time;
                        btn_send.touchable = false;
                        btn_send.grayed = true;
                        TimerManager.inst.Add(1f, 0, Timer);
                        //                    }
                    });
                }

            }
        });

		btn_ok = this.GetChild("n7").asButton;
        btn_ok.text = Tools.GetMessageById("13079");
        btn_ok.onClick.Add(() =>
        {
            if (input_code.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13034"));
            }
            else if (input_phone.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13035"));
            }
            else {
                string param = "sign=" + input_code.text;
                NetHttp.inst.Send(NetBase.HTTP_REGIST, param, (VoHttp v) =>
                {
//                    Debug.Log(v.data);
//                    if ((bool)v.data)
//                    {
						LocalStore.SetLocal (LocalStore.LOCAL_UID, userModel.uid);
						LocalStore.SetLocal (LocalStore.LOCAL_PWD, (string)v.data);
						LocalStore.SetLocal (LocalStore.LOCAL_TYPE, Ex_Local.LOGIN_TYPE_TEL);
                        LocalStore.SetLocal(LocalStore.LOCAL_TEL, input_phone.text);
						//
						userModel.tel = Ex_Local.LOGIN_TYPE_TEL+"|"+input_phone.text;
//						LocalStore.DelUids(userModel.uid);
						LocalStore.SetUids(userModel.uid,userModel.uname,(string)v.data,Ex_Local.LOGIN_TYPE_TEL,userModel.tel);
							//
                        Dictionary<string, object> dc = new Dictionary<string, object>();
                        dc.Add("value", "");
                        dc.Add("tag", "chphone");
                        DispatchGlobalEvent(new MainEvent(MainEvent.ROLE_UPDATE, dc));
                        ViewManager.inst.CloseView(this);

//                    }
                });
            }
        });
    }

    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView();
    }

    private void Timer(float obj)
    {
        if (timerDis == 0)
        {
            btn_send.text = Tools.GetMessageById("13077");
            btn_send.touchable = true;
            btn_send.grayed = false;
            TimerManager.inst.Remove(Timer);
            timerDis = (int)DataManager.inst.systemSimple["time_code"];
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
}
