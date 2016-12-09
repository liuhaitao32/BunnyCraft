using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorChangeAccountIcon : BaseMediator {
    private object[] loginType;
    private List<object> loginTypeList;

    public override void Init()
    {
        base.Init();
		Create(Config.VIEW_CHANGEACCOUNTICON,false,Tools.GetMessageById("13125"));
//        InitTitle(Tools.GetMessageById("13125"));
        GList list =GetChild("n1").asList;
        userModel = ModelManager.inst.userModel;
        loginType = (object[])DataManager.inst.systemSimple["login_icon"];
        //loginTypeList = Tools.ConvertDicToList(loginType, "name");
        //Tools.Sort(loginTypeList, new string[] { "name:int:0" });
        if (loginType.Length > 5)
        {
            list.scrollPane.touchEffect = true;
        }
        else
        {
            list.scrollPane.touchEffect = false;
        }
        list.itemRenderer = OnRender;
        list.numItems = loginType.Length;
    }
	private static string Type_login;
    private ModelUser userModel;

    private void OnRender(int index, GObject item)
    {
        GComponent obj = item.asCom;
        GLoader icon = obj.GetChild("n2").asLoader;
        object[] value = (object[])loginType[index];
        //object[] data = (object[])value[value["name"].ToString()];
        icon.url = Tools.GetResourceUrl("Icon:" + value[1].ToString());
        item.onClick.Add(() => {
            ViewManager.inst.CloseView();
            switch (value[2].ToString())
            {
                case "phone":
                    ViewManager.inst.ShowView<MediatorChangeAccount2>();
                    break;
                case "weixin"://微信
					Login(Ex_Local.LOGIN_TYPE_WEIXIN);
                    break;
                case "weibo"://微博
                    break;
                case "qq"://qq
					Login(Ex_Local.LOGIN_TYPE_QQ);
                    break;

            }
        });
    }
	public static void Login(string type)
	{
		Type_login = type;
		if(type == Ex_Local.LOGIN_TYPE_QQ){
			PlatForm.inst.GetSdk ().Call (Ex_Local.ERROR_NO_QQ, null, (object a) => {
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("33207"));
			});
			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_LOGIN_QQ_HEAD,null,Headimg_getQQ);
//			QQ_auth_code("F9DB32C8E692C5F636A2A9AE4CD964B0");
		}
		else if(type == Ex_Local.LOGIN_TYPE_WEIXIN){
			PlatForm.inst.GetSdk ().Call (Ex_Local.ERROR_NO_WX, null, (object a) => {
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("33208"));
			});
			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_LOGIN_WEIXIN_CODE, null, WX_auth_code);//Weixin 先绑定再拿头像
//			WX_auth_code("041J44R515P1OY1tKIQ51TC5R51J44RF");
		}
		PlatForm.inst.GetSdk().Login(type,true);
	}
	public static void WX_auth_code(object re){
		string code = (string)re;
		//		string param = "code=" + code;
		string param = "pf=" + PlatForm.inst.pf;
		param += "|utype=" + Ex_Local.LOGIN_TYPE_WEIXIN;
		param += "|ustr=" + code;
		//		param += "|pwd=" + pwd;
//		PlatForm.inst.GetSdk().Call(Ex_Local.TRACE,new string[]{"WX_auth_code :: "+param},null);
		NetHttp.inst.Send (NetBase.HTTP_LOGIN, param, (VoHttp v) => {
			Dictionary<string, object> user = (Dictionary<string, object>)v.data;
			ModelManager.inst.userModel.SetData(user);
			//
			LocalStore.SetLocal (LocalStore.LOCAL_TYPE,Ex_Local.LOGIN_TYPE_WEIXIN);
			//
			LocalStore.SetUids (ModelManager.inst.userModel.uid,ModelManager.inst.userModel.uname, ModelManager.inst.userModel.pwd, Ex_Local.LOGIN_TYPE_WEIXIN,ModelManager.inst.userModel.tel);
			//
//			LocalStore.Set_QQ_Info((string)user["uid"],LocalStore.GetLocal(LocalStore.QQ_OPENID),LocalStore.GetLocal(LocalStore.QQ_TOKEN),LocalStore.GetLocal(LocalStore.QQ_DATE));

			if(user.ContainsKey("wx_data")){
				Dictionary<string, object> pf = (Dictionary<string, object>)user["wx_data"];
				LocalStore.Set_WX_Info(ModelManager.inst.userModel.uid,(string)pf ["openid"], (string)pf ["access_token"], (string)pf ["refresh_token"]);
			}

			LocalStore.SetLocal (LocalStore.LOCAL_UID, ModelManager.inst.userModel.uid);
			LocalStore.SetLocal (LocalStore.LOCAL_PWD, ModelManager.inst.userModel.pwd);
//			LocalStore.SetLocal (LocalStore.LOCAL_UNAME,userModel.uname);

			//
			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_AUTH_HEAD_GET_WEIXIN, null, Headimg_getWX);//微信绑定之后才能拿到头像
			PlatForm.inst.GetSdk ().WX_auth_info ("");//微信 找头像
//			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RELOGIN_GAME));
		});
	}
	public static void Headimg_getWX(object re){
		
//		LocalStore.DelUids(LocalStore.GetLocal (LocalStore.LOCAL_UID));
		LocalStore.SetUids (LocalStore.GetLocal (LocalStore.LOCAL_UID), LocalStore.GetLocal (LocalStore.LOCAL_UNAME), LocalStore.GetLocal (LocalStore.LOCAL_PWD), Ex_Local.LOGIN_TYPE_WEIXIN, "");
		LocalStore.SetLocal (LocalStore.OTHER_HEADIMG + ModelManager.inst.userModel.uid, LocalStore.GetLocal (LocalStore.OTHER_HEADIMG));
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RELOGIN_GAME));
	}
	//
	public static string headstr = "";
	public static void Headimg_getQQ(object re){
		//		if (Type_Bingding == Ex_Local.LOGIN_TYPE_WEIXIN) {
		//			Update_headimg ((string)re);
		//		}
		headstr = (string)re;
		QQ_auth_code (LocalStore.GetLocal (LocalStore.QQ_TOKEN));
	}
	public static void QQ_auth_code(object re){
		string code = (string)re;
//		string param = "code=" + code;
		string param = "pf=" + PlatForm.inst.pf;
		param += "|utype=" + Ex_Local.LOGIN_TYPE_QQ;
		param += "|ustr=" + code;
//		param += "|pwd=" + pwd;
		PlatForm.inst.GetSdk().Call(Ex_Local.TRACE,new string[]{"QQ_auth_code :: "+param},null);
		NetHttp.inst.Send (NetBase.HTTP_LOGIN, param, (VoHttp v) => {
			Dictionary<string, object> user = (Dictionary<string, object>)v.data;
			ModelManager.inst.userModel.SetData(user);
			//
			LocalStore.SetLocal (LocalStore.LOCAL_TYPE,Ex_Local.LOGIN_TYPE_QQ);
			//
			LocalStore.SetUids (ModelManager.inst.userModel.uid,ModelManager.inst.userModel.uname, ModelManager.inst.userModel.pwd, Ex_Local.LOGIN_TYPE_QQ,headstr);
			//
			LocalStore.Set_QQ_Info(ModelManager.inst.userModel.uid,LocalStore.GetLocal(LocalStore.QQ_OPENID),LocalStore.GetLocal(LocalStore.QQ_TOKEN),LocalStore.GetLocal(LocalStore.QQ_DATE));

			LocalStore.SetLocal (LocalStore.LOCAL_UID, ModelManager.inst.userModel.uid);
			LocalStore.SetLocal (LocalStore.LOCAL_PWD, ModelManager.inst.userModel.pwd);
//			LocalStore.SetLocal (LocalStore.LOCAL_UNAME,userModel.uname);
			LocalStore.SetLocal(LocalStore.OTHER_HEADIMG+ModelManager.inst.userModel.uid,LocalStore.GetLocal(LocalStore.OTHER_HEADIMG));
			DispatchManager.inst.Dispatch(new MainEvent(MainEvent.RELOGIN_GAME));
		});
	}
    public override void Clear()
    {
        base.Clear();
    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView();
    }
}
