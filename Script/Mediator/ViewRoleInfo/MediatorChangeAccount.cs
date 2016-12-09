using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MediatorChangeAccount : BaseMediator
{
    private ModelUser userModel;
    private ModelRole roleModel;
    private GComboBox userName;
    private string uname;
    private GTextInput input_password;
    private string password;
    private GButton btn_forgetpass;
    private GButton btn_ok;
    private string utype = "";
//    private string pwd = "";
    private string[] uInfo;
    private List<string> ulist;
    private List<string> uidlist;
    //private bool addAccount = false;
    private GTextInput userName2;
    private GButton close;
    private ModelAlert alertModel;
    private GGroup group1;
    private GGroup group2;
    private GTextField textTitle;
    private GButton btnOk;
    private GButton btnCancel;

    public override void Init()
    {
        Create(Config.VIEW_CHANGEACCOUNT, false, Tools.GetMessageById("33101"));
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        alertModel = ModelManager.inst.alertModel;
        ulist = new List<string>();
        //		uidlist = new List<string>();
        InitItem();
    }

    public override void Clear()
    {
        base.Clear();
    }

    private void InitItem()
    {
        //        InitTitle(Tools.GetMessageById("33101"));

        group1 = GetChild("n9").asGroup;
        group2 = GetChild("n10").asGroup;
        textTitle = GetChild("n6").asTextField;
        textTitle.text = Tools.GetMessageById("13128");
        btnOk = GetChild("n7").asButton;
        btnOk.text = Tools.GetMessageById("13079");
        btnCancel = GetChild("n8").asButton;
        btnCancel.text = Tools.GetMessageById("14025");
        group2.visible = true;
        userName = this.GetChild("n5").asComboBox;
        //		userName.onChanged.Add (() =>
        //		{
        //			uname = userName.text;
        //		});
        //GObject obj= this.GetChild("n3");
        //obj.visible = false;
        //        input_password = obj.asCom.GetChild ("n1").asTextInput;
        //		input_password.promptText = Tools.GetMessageById ("13084");
        //        input_password.maxLength = (int)DataManager.inst.systemSimple["password_num"];
        //        input_password.restrict = Config.REG_NUMORABC;
        //        input_password.displayAsPassword = true;
        List<string[]> pList = LocalStore.GetUids();
        string[] myItem = new string[] { };

        foreach (string[] dic in pList)
        {
            if (dic[0] == userModel.uid)
            {
                myItem = dic;
				pList.Remove(dic);
				break;
            }
        }

//        string[] tempItem= pList[0];
//        pList[pList.IndexOf(myItem)] = tempItem;
//        pList[0] = myItem; 
//        if (pList.Count != 0)
//        {
            foreach (string[] dic in pList)
            {
                ulist.Add(dic[1]);
                //            if (dic [0] != userModel.uid) {
                //	ulist.Add (dic [1]);
                //}
            }

            ulist.Add(Tools.GetMessageById("13066"));
            userName.items = ulist.ToArray();

            //			uid = ((object[])pList[0])[0].ToString();
            //			uname = ((object[])pList[0])[1].ToString();
//			pwd = myItem[2];
            //            utype = ((object[])pList[0])[3].ToString();
			uInfo = myItem;
            userName.onChanged.Add(() =>
           {

               if (userName.selectedIndex == ulist.Count - 1)
               {
                   ViewManager.inst.CloseView();
                   ViewManager.inst.ShowView<MediatorChangeAccountIcon>();
               }
               else
               {

                   uInfo = (string[])pList[userName.selectedIndex];

                   if (uInfo[0].ToString()==userModel.uid)
                   {
                       ViewManager.inst.CloseView(this);
                   }
                   else
                   {
                       group1.visible = true;
                       group2.visible = false;
//                       pwd = uInfo[2].ToString();
                       //						utype = uInfo[3].ToString();

                   }

               }

           });
//        }
//        else
//        {
//            //userName.text = Tools.GetMessageById ("13102");
		userName.text = userModel.uname;
//        }
        btn_ok = this.GetChild("n0").asButton;
        btn_ok.text = Tools.GetMessageById("13076");

        btnCancel.onClick.Add(() =>
        {
            ViewManager.inst.CloseView();
        });


        btnOk.onClick.Add(() =>
        {
            if (userName.text == "")
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13033"));
            }
            else
            {
					if(uInfo!=null && uInfo.Length>0){
						Send(uInfo[0],uInfo[2], uInfo[3]);
					}
            }
        });


        btn_ok.onClick.Add(() =>
       {
           if (userName.text == "")
           {
               ViewManager.inst.ShowText(Tools.GetMessageById("13033"));
           }
           else
           {
					if(uInfo!=null && uInfo.Length>0){
						Send(uInfo[0],uInfo[2], uInfo[3]);
					}
           }
       });


    }
    private string Type_login;
	private void Send(string uid,string pwd , string type)
    {
        //		if (pwd == "")
        //        {
        //            ViewManager.inst.ShowText(Tools.GetMessageById("10011"));
        //        }
        //        else
        //        {
        Type_login = type;
        //		if(type == Ex_Local.LOGIN_TYPE_QQ){
        //			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_LOGIN_QQ_HEAD,null,Headimg_get);
        //			PlatForm.inst.GetSdk().Login(type,true);
        //		}
        //		else if(type == Ex_Local.LOGIN_TYPE_WEIXIN){
        //			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_AUTH_HEAD_GET_WEIXIN, null, Headimg_get);
        //			PlatForm.inst.GetSdk().Login(type,true);
        //		}
        //		else{
//		ViewManager.inst.ShowText(uInfo[1] + " || "+uInfo[0]+" || "+uInfo[2]);
//		userModel.Login(Ex_Local.LOGIN_TYPE_UID, uInfo[0], uInfo[2], () =>
//        {
                //				if (ulist.Contains(userModel.uid))
                //					LocalStore.DelUids(userModel.uid);
                //                if (utype.Equals(Ex_Local.LOGIN_TYPE_UNAME))
                //                {
                //					LocalStore.SetUids(userModel.uid,userModel.uname, userModel.pwd,userModel.type_login,userModel.tel);
                //                }
                //                else
                //                {
                //                    LocalStore.SetUids(uname, input_password.text, Ex_Local.LOGIN_TYPE_UID);
                //                }
				LocalStore.SetLocal(LocalStore.LOCAL_TYPE, Ex_Local.LOGIN_TYPE_UID);
                //
                //				LocalStore.SetLocal(LocalStore.LOCAL_TYPE, userModel.type_login);
                //
                //                ViewManager.inst.CloseView();
                //                roleModel.otherInfo = null;
                //                alertModel.isOpen = true;
                //                roleModel.uids.Clear();
                //                ViewManager.inst.CloseScene();
		LocalStore.SetLocal (LocalStore.LOCAL_UID, uid);
		LocalStore.SetLocal (LocalStore.LOCAL_PWD, pwd);
                //
                DispatchManager.inst.Dispatch(new MainEvent(MainEvent.RELOGIN_GAME));
                //this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { userModel.uid, userModel.uid, roleModel.tab_Role_CurSelect1,roleModel.tab_Role_CurSelect2,roleModel.tab_Role_CurSelect3 }));
                //                Dictionary<string, object> dd = new Dictionary<string, object>();
                //                dd["fuid"] = userModel.uid;
                //                NetHttp.inst.Send(NetBase.HTTP_FUSERGET, dd, (VoHttp vo) =>
                //                {
                //                    if (vo.data != null)
                //                    {
                //                        roleModel.tab_Role_Select1 = roleModel.tab_Role_CurSelect1;
                //                        roleModel.tab_Role_Select2 = roleModel.tab_Role_CurSelect2;
                //                        roleModel.tab_Role_Select3 = roleModel.tab_Role_CurSelect3;
                //                        roleModel.otherInfo = (Dictionary<string, object>)vo.data;
                //                        ViewManager.inst.ShowScene<MediatorRoleRoot>();
                //                    }
                //                });
//            });

        //        }
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
