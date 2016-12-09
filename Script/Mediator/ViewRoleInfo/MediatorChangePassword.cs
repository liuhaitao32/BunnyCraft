using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;

public class MediatorChangePassword : BaseMediator {
    private GButton btn_ok;
    private GButton close;
    private GTextInput input_newpwd;
    private GTextInput input_pwd;
    private ModelUser userModel;
    private GTextField title;

    public override void Init()
    {
		Create(Config.VIEW_CHANGEPASSWORD,false,Tools.GetMessageById("33104"));
        InitView();
    }

    private void InitView()
    {
//        InitTitle(Tools.GetMessageById("33104"));
        userModel = ModelManager.inst.userModel;
		input_pwd=this.GetChild("n4").asCom.GetChild("n1").asTextInput;
        //input_pwd.promptText = Tools.GetMessageById("13101");
        input_pwd.maxLength = (int)DataManager.inst.systemSimple["password_num"];
        input_pwd.restrict = Config.REG_NUMORABC;
		input_newpwd = this.GetChild("n3").asCom.GetChild("n1").asTextInput;
        //input_newpwd.promptText = Tools.GetMessageById("13099");
        input_newpwd.maxLength = (int)DataManager.inst.systemSimple["password_num"];
        input_newpwd.restrict = Config.REG_NUMORABC;

        GetChild("n8").asTextField.text = Tools.GetMessageById("13101") + "：";
        GetChild("n9").asTextField.text = Tools.GetMessageById("13099") + "：";

        btn_ok =this.GetChild("n0").asButton;
        btn_ok.text = Tools.GetMessageById("13079");
        btn_ok.onClick.Add(() =>
        {
            if (input_newpwd.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13037"));
            }
            else if (input_pwd.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13036"));
            }
            else
            {
                string pwd_=Tools.MD5(input_pwd.text);
                if (!pwd_.Equals(userModel.pwd))
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13143"));
                }
                else
                {
                    //                if (input_pwd.text.Equals(LocalStore.GetLocal(LocalStore.LOCAL_PWD)))
                    //                {
                    string param = "old_pwd=" + input_pwd.text;
                    param += "|pwd=" + input_newpwd.text;

                    NetHttp.inst.Send(NetBase.HTTP_CHANGEPWD, param, (VoHttp v) =>
                    {
                        //"25f9e794323b453885f5181f1b624d0b"
                        if ((string)v.data != string.Empty)
                        {
                            string pwd = (string)v.data;
                            LocalStore.SetLocal(LocalStore.LOCAL_PWD, pwd);
                            userModel.pwd = v.data.ToString();
//                            LocalStore.DelUids(userModel.uid);
                            LocalStore.SetUids(userModel.uid, userModel.uname, pwd, userModel.type_login, userModel.tel);
                            Dictionary<string, object> dc = new Dictionary<string, object>();
                            dc.Add("value", "");
                            dc.Add("tag", "password");
                            DispatchGlobalEvent(new MainEvent(MainEvent.ROLE_UPDATE, dc));
                            ViewManager.inst.CloseView(this);
                        }
                    });
                    //                }
                    //                else
                    //                {
                    //                    ViewManager.inst.ShowText(Tools.GetMessageById("13067"));
                    //                }
                }


            }
        });
    }

    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView();
    }
}
