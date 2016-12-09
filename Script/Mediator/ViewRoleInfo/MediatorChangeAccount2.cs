using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MediatorChangeAccount2 : BaseMediator
{
    private ModelUser userModel;
    private ModelRole roleModel;
    private GTextInput input_password;
    private GButton btn_forgetpass;
    private GButton btn_ok;
    private List<string> ulist;
    private GTextInput txetPhone;
    private ModelAlert alertModel;

    public override void Init()
    {
		Create(Config.VIEW_CHANGEACCOUNT2,false,Tools.GetMessageById("13125"));
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        alertModel = ModelManager.inst.alertModel;
        ulist = new List<string>();
        InitItem();
    }

    public override void Clear()
    {
        base.Clear();
    }

    private void InitItem()
    {
//        InitTitle(Tools.GetMessageById("13125"));

        GObject obj = this.GetChild("n3");
        input_password = obj.asCom.GetChild("n1").asTextInput;
        input_password.maxLength = (int)DataManager.inst.systemSimple["password_num"];
        input_password.restrict = Config.REG_NUMORABC;

        input_password.onFocusIn.Add(()=> {
            input_password.displayAsPassword = true;
        });
        GObject obj2 = this.GetChild("n6");
        txetPhone = obj2.asCom.GetChild("n1").asTextInput;
        txetPhone.maxLength = 11;
        btn_forgetpass = this.GetChild("n4").asButton;
        btn_forgetpass.text = "[u]" + Tools.GetMessageById("13075") + "[/u]?";
        btn_forgetpass.onClick.Add(() =>
        {
            ViewManager.inst.ShowView<MediatorAccountOne>();
        });
        btn_ok = this.GetChild("n0").asButton;
        GTextField phone = this.GetChild("n8").asTextField;
        phone.text= Tools.GetMessageById("13102") + "：";
        GTextField passwd = this.GetChild("n9").asTextField;
        passwd.text= Tools.GetMessageById("13084") + "：";
        btn_ok.text = Tools.GetMessageById("13126");
        btn_ok.onClick.Add(() =>
        {
            if (txetPhone.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13033"));
            }
            else
            {
                Send(txetPhone.text);

            }
        });
    }
    private void Send(string phone)
    {
        if (input_password.text == "")
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("10011"));
        }
        else
        {
            if (phone.Equals(userModel.tel))
            {
                ViewManager.inst.CloseView(this);
            }
            else
            {
                //userModel.Login(Ex_Local.LOGIN_TYPE_TEL, phone, input_password.text, () =>
				userModel.Login(Ex_Local.LOGIN_TYPE_TEL, phone, input_password.text, () =>
                {
					LocalStore.SetLocal(LocalStore.LOCAL_TYPE, Ex_Local.LOGIN_TYPE_TEL);
//					LocalStore.DelUids(userModel.uid);
					LocalStore.SetUids(userModel.uid,userModel.uname, userModel.pwd, Ex_Local.LOGIN_TYPE_TEL, phone);
					LocalStore.SetLocal (LocalStore.LOCAL_UID, userModel.uid);
					LocalStore.SetLocal (LocalStore.LOCAL_PWD, userModel.pwd);

//                    LocalStore.SetLocal(LocalStore.LOCAL_UNAME, phone);
//                    LocalStore.SetLocal(LocalStore.LOCAL_PWD, input_password.text);

                    //ViewManager.inst.CloseView(this);
                    //roleModel.otherInfo = null;
                    //alertModel.isOpen = true;
                    //roleModel.uids.Clear();
                    //ViewManager.inst.CloseScene();
                    //Dictionary<string, object> dd = new Dictionary<string, object>();
                    //dd["fuid"] = userModel.uid;
//                    NetHttp.inst.Send(NetBase.HTTP_FUSERGET, dd, (VoHttp vo) =>
                    //{
                    //    if (vo.data != null)
                    //    {
                    //        roleModel.tab_Role_Select1 = roleModel.tab_Role_CurSelect1;
                    //        roleModel.tab_Role_Select2 = roleModel.tab_Role_CurSelect2;
                    //        roleModel.tab_Role_Select3 = roleModel.tab_Role_CurSelect3;
                    //        roleModel.otherInfo = (Dictionary<string, object>)vo.data;
                    //        ViewManager.inst.ShowScene<MediatorRoleRoot>();
                    //    }
                    //});

                    DispatchManager.inst.Dispatch(new MainEvent(MainEvent.RELOGIN_GAME));
                },true);
            }
        }
    }



    public bool IsHandset(string str_handset)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^[1]+[3,5]+\d{9}");
    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView();
    }
}
