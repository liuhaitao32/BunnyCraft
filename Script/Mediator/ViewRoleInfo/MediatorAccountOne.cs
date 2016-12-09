using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MediatorAccountOne : BaseMediator {
    private ModelUser userModel;
    private GTextInput input_phone;
    private GTextInput input_code;
    private GTextInput input_password;
    private GButton btn_send;
    private GButton btn_ok;
    private GButton close;
    private DateTime timer;
    private ModelRole roleModel;
    private int timerDis;

    public override void Init()
    {
		Create(Config.VIEW_CHANGEACCOUNT1,false,Tools.GetMessageById("33102"));
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        timerDis = (int)DataManager.inst.systemSimple["time_code"];
        InitView();
    }

    private void InitView()
    {
//        InitTitle(Tools.GetMessageById("33102"));

        GTextField text1=GetChild("n8").asTextField;
        GTextField text2=GetChild("n9").asTextField;
        GTextField text3 = GetChild("n10").asTextField;

        text1.text = Tools.GetMessageById("13097") + "：";
        text2.text = Tools.GetMessageById("13098") + "：";
        text3.text = Tools.GetMessageById("13099") + "：";


        input_phone = this.GetChild("n2").asCom.GetChild("n1").asTextInput;
        input_phone.maxLength = 11;
        //input_phone.promptText = Tools.GetMessageById("13097");
        input_code = this.GetChild("n5").asCom.GetChild("n1").asTextInput;
        input_code.maxLength = (int)DataManager.inst.systemSimple["code_num"];
        //input_code.promptText = Tools.GetMessageById("13098");
        input_password = this.GetChild("n4").asCom.GetChild("n1").asTextInput;
        //input_password.promptText = Tools.GetMessageById("13099");
        input_password.maxLength = (int)DataManager.inst.systemSimple["password_num"];
        input_password.restrict = Config.REG_NUMORABC;
		btn_send = this.GetChild("n6").asButton;
        btn_send.text = Tools.GetMessageById("13077");
        btn_send.onClick.Add(() => {
            if (input_phone.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13035"));
            }
            else
            {
                string param = "tel_num=" + input_phone.text;
                NetHttp.inst.Send(NetBase.HTTP_GETBACKSIGN, param, (VoHttp v) =>
                {
                    Debug.Log(v.data);//true
                    if ((bool)v.data)
                    {
                        ViewManager.inst.ShowText(Tools.GetMessageById("13049"));
                        timer = ModelManager.inst.gameModel.time;
                        btn_send.touchable = false;
                        btn_send.grayed = true;
                        TimerManager.inst.Add(1f, 0, Timer);
                    }
                });
            }
        });
		btn_ok = this.GetChild("n7").asButton;
        btn_ok.text = Tools.GetMessageById("13159");
        btn_ok.onClick.Add(() => {
            if (input_phone.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13035"));
            }
            else if (input_password.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("10011"));
            }
            else if (input_code.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13034"));
            }
            else {
                string param = "tel_num=" + input_phone.text;
                param += "|pwd=" + input_password.text;
                param += "|sign=" + input_code.text;
                NetHttp.inst.Send(NetBase.HTTP_CHANGEGETBACKPWD, param, (VoHttp v) =>
                {
                    Dictionary<string, object> re = (Dictionary<string, object>)v.data;
                    userModel.SetData(re);
//                    LocalStore.SetLocal(LocalStore.LOCAL_UNAME, re["uname"].ToString());
						LocalStore.SetLocal(LocalStore.LOCAL_UID, userModel.uid);
						LocalStore.SetLocal(LocalStore.LOCAL_PWD, userModel.pwd); //re["pwd"].ToString()
//                    List<string[]> plist=LocalStore.GetUids();
//                    List<string> uList = new List<string>();
//                    foreach(string[] dic in plist)
//                    {
//                        uList.Add(dic[0]);
//                    }
//					if (uList.Contains(re["uid"].ToString()))
//						LocalStore.DelUids(userModel.uid);
						LocalStore.SetUids(userModel.uid, userModel.uname,userModel.pwd, userModel.type_login,userModel.tel);
//                    ViewManager.inst.CloseView(this);
//                    roleModel.uids.Clear();
//					ViewManager.inst.ShowScene<MediatorMain>();
						DispatchManager.inst.Dispatch(new MainEvent(MainEvent.RELOGIN_GAME));
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
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView();
    }
}
