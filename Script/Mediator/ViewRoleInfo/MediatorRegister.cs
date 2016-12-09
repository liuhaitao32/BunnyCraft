using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MediatorRegister : BaseMediator
{
	private GButton btn_ok;
	private GButton btn_send;
    private GTextInput input_code;
	private GTextInput input_password;
	private GTextInput input_phone;
    private Dictionary<string, object> otherInfo;
    private DateTime timer;
    private int timerDis;
    private ModelUser userModel;
    private GTextField title;
    private GButton close;

    public override void Init ()
	{
		base.Init ();
		Create (Config.VIEW_REGISTER,false, Tools.GetMessageById("33106"));
		userModel = ModelManager.inst.userModel;
        otherInfo = ModelManager.inst.roleModel.otherInfo;
        timerDis = (int)DataManager.inst.systemSimple["time_code"];
        InitItem ();
	}

	private void InitItem ()
	{
//        InitTitle(Tools.GetMessageById("33106"));
        input_password = this.GetChild ("n5").asCom.GetChild ("n1").asTextInput;
        input_password.promptText = "";
        input_password.maxLength = (int)DataManager.inst.systemSimple["password_num"];
        input_password.restrict = Config.REG_NUMORABC;
		input_phone = this.GetChild ("n3").asCom.GetChild ("n1").asTextInput;
        input_phone.maxLength = 11;
        input_phone.promptText = "";
        input_code = this.GetChild ("n4").asCom.GetChild ("n1").asTextInput;
        input_code.maxLength = (int)DataManager.inst.systemSimple["code_num"];
        input_code.promptText = "";
        btn_send = this.GetChild ("n6").asButton;
        GTextField phone = this.GetChild("n8").asTextField;
        phone.text = Tools.GetMessageById("13097")+"：";
        GTextField passwd = this.GetChild("n9").asTextField;
        passwd.text = Tools.GetMessageById("13098")+"：";
        GTextField code = this.GetChild("n10").asTextField; 
        code.text = Tools.GetMessageById("13084")+"：";
        btn_send.text = Tools.GetMessageById ("13077");
		btn_send.onClick.Add (() =>
		{
			if (input_phone.text == "")
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("13035"));
			}
			else
			{
                string param = "tel_num=" + input_phone.text;
                NetHttp.inst.Send(NetBase.HTTP_VALIDATE, param, (VoHttp v) =>
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

		btn_ok = this.GetChild ("n7").asButton;
		btn_ok.text = Tools.GetMessageById ("13079");
		btn_ok.onClick.Add (() =>
		{
            if (input_password.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("10011"));
            }
            else if (input_code.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13034"));
            }
            else if (input_phone.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13035"));
            }
            else
            {
                string param = "sign=" + input_code.text;
//                param += "|uname=" + userModel.uname;
                param += "|pwd=" + input_password.text;
                 NetHttp.inst.Send(NetBase.HTTP_REGIST, param, (VoHttp v) =>
                {
//                    Debug.Log(v.data);
                    if ((string)v.data != string.Empty)
                    {
						ViewManager.inst.ShowText(Tools.GetMessageById("13124"));
						//
						LocalStore.SetLocal (LocalStore.LOCAL_UID, userModel.uid);
						LocalStore.SetLocal (LocalStore.LOCAL_PWD, (string)v.data);
						LocalStore.SetLocal (LocalStore.LOCAL_TYPE, Ex_Local.LOGIN_TYPE_TEL);
                        LocalStore.SetLocal(LocalStore.LOCAL_TEL, input_phone.text);
						//
						userModel.tel = Ex_Local.LOGIN_TYPE_TEL+"|"+ input_phone.text;
						otherInfo["tel"] = userModel.tel;
								//
//						LocalStore.DelUids(userModel.uid);
						LocalStore.SetUids(userModel.uid,userModel.uname,(string)v.data,Ex_Local.LOGIN_TYPE_TEL,userModel.tel);
								//
                        Dictionary<string, object> dc = new Dictionary<string, object>();
                        dc.Add("value", "");
                        dc.Add("tag", "account");
                        DispatchGlobalEvent(new MainEvent(MainEvent.ROLE_UPDATE, dc));
                        ViewManager.inst.CloseView(this);
                    }
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
            timerDis = (int)DataManager.inst.systemSimple["time_code"];
            TimerManager.inst.Remove(Timer);
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
