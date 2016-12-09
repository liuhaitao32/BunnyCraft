using System;
using UnityEngine;
using LitJson;
using System.Collections.Generic;

public class UnityCallBack:MonoBehaviour
{
	public const string FID_get_photo_end = "get_photo_end";
	public const string FID_get_geo = "get_geo";
	public const string FID_get_lola = "get_lola";
	public const string FID_share_ok = "share_ok";
	public const string FID_copy_ok = "copy_ok";
	public const string FID_share_status_success = "0";
	public const string FID_scan_message = "onScannerMessage";
	public const string FID_touch3d = "touch3d";

	public const string FID_LOGIN_QQ_TOKEN = "login_qq_token";
	public const string FID_LOGIN_QQ_HEAD = "login_qq_head";
	public const string FID_LOGIN_QQ = "login_qq";
	public const string FID_LOGIN_WEIXIN_CODE = "login_weixin_code";
	//
	public const string ERROR_NO_QQ = "error_no_qq";
	public const string ERROR_NO_WX = "error_no_wx";
	//
	public const string SCHEME_EVENT_JOIN = "scheme_event_join";
	public const string SCHEME_EVENT_JOIN_CHECK = "scheme_event_join_check";

	public UnityCallBack ()
	{
	}

	void Start ()
	{
		
	}
//	void Update(){
//		Debug.LogError (Time.deltaTime);
//	}
	void sdk_callback (string value)
	{
        Debug.Log("---unity--CALLbACK....."+value);
    //Log: CALLbACK.....{ "funcId":"share_ok","msg":"0","isJson":0}
        Dictionary<string, object> dd = new Dictionary<string, object>();
        if (value.IndexOf ("{") > -1)
		{
			string fid = "";
			//普通第三方json数据返回
			JsonData re = JsonMapper.ToObject (value);
			if (((int)re ["isJson"]) == 1)
			{
				//				trace.text += "\n -^-:" + re ["funcId"];
				//				trace.text += "\n -^-:" + re ["data"].ToJson ();
				fid = (string)re ["funcId"];
				if (fid == FID_LOGIN_QQ_TOKEN) {
					//QQ 登陆返回 token
					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_LOGIN_QQ_TOKEN, value);
				}
				else if(fid == FID_LOGIN_QQ_HEAD){
					//QQ 登陆返回 头像//figureurl_qq_1//figureurl_qq_2
					//{"ret":0,"msg":"","is_lost":0,"nickname":"三叔","gender":"男","province":"北京","city":"朝阳","figureurl":"http:\/\/qzapp.qlogo.cn\/qzapp\/101347709\/367F8C9525DC8690EC9DF1578F6714A1\/30","figureurl_1":"http:\/\/qzapp.qlogo.cn\/qzapp\/101347709\/367F8C9525DC8690EC9DF1578F6714A1\/50","figureurl_2":"http:\/\/qzapp.qlogo.cn\/qzapp\/101347709\/367F8C9525DC8690EC9DF1578F6714A1\/100","figureurl_qq_1":"http:\/\/q.qlogo.cn\/qqapp\/101347709\/367F8C9525DC8690EC9DF1578F6714A1\/40","figureurl_qq_2":"http:\/\/q.qlogo.cn\/qqapp\/101347709\/367F8C9525DC8690EC9DF1578F6714A1\/100","is_yellow_vip":"0","vip":"0","yellow_vip_level":"0","level":"0","is_yellow_year_vip":"0"}
					string headURL = (string)re["figureurl_qq_2"];//40
					LocalStore.DelLocal(LocalStore.HEADIMG + headURL);//删除缓存
					LocalStore.SetLocal(LocalStore.OTHER_HEADIMG,headURL);
					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_LOGIN_QQ_HEAD, headURL);
//					PlatForm.inst.GetSdk().Dispatch(Ex_Local.CALL_LOGIN_PF,null);//这里可能需要服务器验证
				}
			}
			else
			{
                PlatForm.inst.GetSdk().Dispatch("callback", value);
                //自定义 json数据 标准格式{isJson:0,function:"",msg:""};
                //				trace.text += "\n -->" + re ["funcId"] + "\n -->" + re ["other"];
				fid = re ["funcId"].ToString ();
				if (fid == FID_get_photo_end) {
					string bitmap = (string)re ["msg"];

//					byte[] b = System.Convert.FromBase64String (bitmap);

					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_OPENPHOTO, bitmap);
				} else if (fid == FID_share_ok) {

					string msg = (string)re ["msg"];
//					Debug.Log ("-->>lht 001-->>" + msg + " :: >> " + Ex_Local.CALL_FUN);
					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_FUN, msg);
				} else if (fid == FID_LOGIN_WEIXIN_CODE) {
					//微信 code 返回
					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_LOGIN_WEIXIN_CODE, (string)re ["msg"]);
				
				} else if (fid == ERROR_NO_QQ) {
					//
					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.ERROR_NO_QQ, "0");
				} else if (fid == ERROR_NO_WX) {
					//
					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.ERROR_NO_WX, "0");
				} else if (fid == SCHEME_EVENT_JOIN) {//有scheme 消息打开游戏
				}
				else if(fid == SCHEME_EVENT_JOIN_CHECK){
					
				}
				#if UNITY_IOS
				else if (fid == FID_touch3d) {
					
//					PlatForm.inst.touch3dType = (string)re ["msg"];
					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_RE_TOUCH3D, (string)re ["msg"]);
//					Ex_Ios.c_test();


				}
				else if(fid == Ex_Local.CALL_RE_TOUCH3D_GET){
					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_RE_TOUCH3D_GET, (string)re ["msg"]);
				}
				#endif
                //if (re["funcId"].ToString() == FID_copy_ok)
                //{
                //    dd["name"] = Ex_Local.CALL_COPYTO;
                //    dd["value"] = (string)re["msg"];
                //    PlatForm.inst.GetSdk().Dispatch(Ex_Local.CALL_COPYTO, dd);
                //}

            }
        }
		else
		{
//			trace.text += "\n" + str;
//			if (str.IndexOf ("GET_GEO_NELL") > -1)
//			{
//				string[] arr = str.Split (new string[]{ "," }, System.StringSplitOptions.None);
//				la = arr [1];
//				lo = arr [2];
//				sdk_callback (arr [0] + arr [1] + arr [2]);
//			}
		}
	}
}