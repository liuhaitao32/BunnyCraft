using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Ex_Local:ISdkFuntion
{
	public Dictionary<string,Action<object>> funs;

	public const string TRACE = "traceInfo";
	//String w,String h
	public const string CALL_OPENPHOTO = "openPhoto";
	public const string CALL_COPYTO = "copyTo";
	public const string CALL_WEIBO = "share_weibo";
	//String appKey,String text,String url,String bitmap,String reURL
	public const string CALL_WECHAR = "share_wechar";
	//String appID,String text,String url,String bitmap,String type
	public const string CALL_WECHARIMG = "share_wechar_img";
	//String appID,String bitmap,String type
	public const string CALL_QQ = "share_qq";
	//
	public const string CALL_FUN = "share_fun";
	public const string CALL_SCAN = "scan";

	public const string CALL_INIT_PIDS = "init_pids";
	public const string CALL_BUY = "buy";

	public const string CALL_OPEN_WECHAR = "open_wechar";

	public const string CALL_RE_TOUCH3D = "touch3d";
	public const string CALL_RE_TOUCH3D_GET = "touch3d_get";
	public const string CALL_RE_TOUCH3D_SET = "touch3d_set";
	//
	public const string CALL_LOGIN_PF = "login_pf";
//平台账号登陆
	//
	public const string CALL_INIT_QQ = "init_qq";
	public const string CALL_LOGIN_QQ = "login_qq";
	public const string CALL_LOGIN_QQ_TOKEN = "login_qq_token";
	public const string CALL_LOGIN_QQ_HEAD = "login_qq_head";
	//
	public const string CALL_INIT_WEIXIN = "init_weixin";
	public const string CALL_LOGIN_WEIXIN = "login_weixin";
	public const string CALL_LOGIN_WEIXIN_CODE = "login_weixin_code";
	public const string CALL_AUTH_TOKEN_WEIXIN = "auth_token_weixin";
	public const string CALL_AUTH_RE_TOKEN_WEIXIN = "auth_re_token_weixin";
	public const string CALL_AUTH_INFO_WEIXIN = "auth_info_weixin";
	public const string CALL_AUTH_HEAD_WEIXIN = "auth_head_weixin";
	public const string CALL_AUTH_HEAD_GET_WEIXIN = "auth_head_weixin_get";
	//
	public const string ERROR_NO_QQ = "error_no_qq";
	public const string ERROR_NO_WX = "error_no_wx";
	//
	public const string LOGIN_TYPE_UNAME = "uname";
	public const string LOGIN_TYPE_TEL = "tel";
	public const string LOGIN_TYPE_UID = "uid";
	public const string LOGIN_TYPE_QQ = "qq";
	public const string LOGIN_TYPE_WEIXIN = "wx";

	public Ex_Local ()
	{
		this.funs = new Dictionary<string, Action<object>> ();
	}

	public virtual void Init (string type)
	{
	}

	public virtual void Init (string className, string method)
	{
	}

	public void InitPF (string type)
	{
		#if !UNITY_EDITOR
		string appId;
//		if (type == LOGIN_TYPE_QQ) {
			appId = DataManager.inst.systemSimple [DataManager.SHARE_QQ_ID].ToString ();
			string openId = LocalStore.GetLocal (LocalStore.QQ_OPENID);
			string token = LocalStore.GetLocal (LocalStore.QQ_TOKEN);
			string date = LocalStore.GetLocal (LocalStore.QQ_DATE);

			Call (CALL_INIT_QQ, new string[]{ appId, openId, token, date,"" }, null);
//		}else if(type == LOGIN_TYPE_WEIXIN){
			appId = DataManager.inst.systemSimple [DataManager.SHARE_WEIXIN_ID].ToString ();
			Call(CALL_INIT_WEIXIN,new string[]{appId},null);
//		}
		#endif
	}

	public  void Login (string type, bool change = false)
	{
		string openId;
		if (type == LOGIN_TYPE_QQ)
		{
			Call (CALL_LOGIN_QQ_TOKEN, null, (object re) =>
			{
				JsonData json = JsonMapper.ToObject ((string)re);
//				string appId = DataManager.inst.systemSimple [DataManager.SHARE_QQ_ID].ToString ();
				openId = (string)json ["openid"];
				string token = (string)json ["access_token"];
				string date = ((int)json ["expires_in"])+"";
				LocalStore.SetLocal (LocalStore.QQ_OPENID, openId);
				LocalStore.SetLocal (LocalStore.QQ_TOKEN, token);
				LocalStore.SetLocal (LocalStore.QQ_DATE, date);
				//
				//				Debug.Log("--lht--LOGIN_TYPE_QQ Login "+openId+" || "+token +" || "+date);
				//
//				QQ_head_get(openId,token);
			});
			if (change)
			{
				Call (CALL_LOGIN_QQ, new string[]{ "change" }, null);
			}
			else
			{
				Call (CALL_LOGIN_QQ, new string[]{ "" }, null);
			}
		}
		else if (type == LOGIN_TYPE_WEIXIN)
		{

			if (change)
			{
				Call (CALL_LOGIN_WEIXIN, new string[]{ "" }, null);//Weixin 授权
			}
			else
			{
				openId = LocalStore.GetLocal (LocalStore.WX_OPENID);
				string token = LocalStore.GetLocal (LocalStore.WX_TOKEN);
				string re_token = LocalStore.GetLocal (LocalStore.WX_RE_TOKEN);

				if (openId.Length > 0 && token.Length > 0)
				{
					Call (CALL_AUTH_TOKEN_WEIXIN, null, WX_auth_token);
					NetHttp.inst.sendHttp_wx ("https://api.weixin.qq.com/sns/auth?access_token=" + token + "&openid=" + openId);//weixin 验证
				}
				else if (openId == "" || token == "")
				{
					Call (CALL_LOGIN_WEIXIN_CODE, null, WX_auth_code);
					Call (CALL_LOGIN_WEIXIN, new string[]{ openId }, null);//Weixin 授权
				}
			}
		}
	}

	public virtual void Clear ()
	{
	}

	public virtual void Call (string method, Action<object> fun)
	{
		
	}

	public virtual void Call (string method, object[] args, Action<object> fun)
	{
	}

	public virtual void CallBuy (string pid, Action<object> fun)
	{
	}

	public virtual void CallInitPIDs (string[] pids, string androidKEY, Action<object> fun)
	{
		
	}

	public virtual string CallReturn (string method)
	{
		return "";
	}

	public void Dispatch (string method, object data)
	{
		if (funs.ContainsKey (method))
		{
			this.funs [method] (data);
			this.funs.Remove (method);
		}
	}

	public void QQ_head_get (string openid, string token)
	{
//		string token = "";
//		string openid = "";
//		Call (CALL_LOGIN_QQ_HEAD, null, ()=>{

		//					PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_LOGIN_QQ_HEAD, headURL);
		//					PlatForm.inst.GetSdk().Dispatch(Ex_Local.CALL_LOGIN_PF,null);//这里可能需要服务器验证
//		});
		NetHttp.inst.sendHttp_wx ("https://graph.qq.com/user/get_user_info?access_token=" + token + "&oauth_consumer_key=meng52&openid=" + openid);
	}

	public void WX_auth_code (object re)
	{
		string code = (string)re;
		Debug.Log ("--unity--WX_auth_code--" + code);
		//
		string appId = DataManager.inst.systemSimple [DataManager.SHARE_WEIXIN_ID].ToString ();
		string s = "6a84fee18c26bb4792451ac9d2748cd1";
		//
		Call (CALL_AUTH_INFO_WEIXIN, null, WX_auth_info);
		NetHttp.inst.sendHttp_wx ("https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appId + "&secret=" + s + "&code=" + code + "&grant_type=authorization_code");
	}

	public void WX_auth_token (object re)
	{
		JsonData json = JsonMapper.ToObject ((string)re);
		int code = (int)json ["errcode"];
		if (code == 0)
		{
			//
			WX_auth_info ("");//拿用户信息
		}
		else
		{
			string appId = DataManager.inst.systemSimple [DataManager.SHARE_WEIXIN_ID].ToString ();
			string re_token = LocalStore.GetLocal (LocalStore.WX_RE_TOKEN);
			Call (CALL_AUTH_RE_TOKEN_WEIXIN, null, WX_auth_token_re);
			NetHttp.inst.sendHttp_wx ("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid=" + appId + "&grant_type=refresh_token&refresh_token=" + re_token);
		}
	}

	public void WX_auth_token_re (object re)
	{
		string str = (string)re;
		JsonData json = JsonMapper.ToObject (str);
		int code = 0;
		if (str.IndexOf ("errcode") > -1)
		{
			code = (int)json ["errcode"];
			string openId = LocalStore.GetLocal (LocalStore.WX_OPENID);
			Call (CALL_LOGIN_WEIXIN_CODE, null, WX_auth_code);
			Call (CALL_LOGIN_WEIXIN, new string[]{ openId }, null);//Weixin 授权,重新授权
		}
		else
		{
			LocalStore.SetLocal (LocalStore.WX_OPENID, (string)json ["openid"]);
			LocalStore.SetLocal (LocalStore.WX_TOKEN, (string)json ["access_token"]);
			LocalStore.SetLocal (LocalStore.WX_RE_TOKEN, (string)json ["refresh_token"]);
			//
			WX_auth_info ("");//拿用户信息
		}

	}

	public void WX_auth_info (object re)
	{
//		JsonData json = JsonMapper.ToObject ((string)re);
		string openid = LocalStore.GetLocal (LocalStore.WX_OPENID);
		string token = LocalStore.GetLocal (LocalStore.WX_TOKEN);
		//
		Call (CALL_AUTH_HEAD_WEIXIN, null, WX_auth_head_get);
		NetHttp.inst.sendHttp_wx ("https://api.weixin.qq.com/sns/userinfo?access_token=" + token + "&openid=" + openid);
	}

	public void WX_auth_head_get (object re)
	{
		JsonData json = JsonMapper.ToObject ((string)re);
		Debug.Log ("--lht--headimgurl :: \n" + json.ToJson ());
		//{"openid":"o9zsEwc8Xql8WP6QSSeB3fZC2bqs","nickname":"\u5289\u6D77\u6FE4@Elvis","sex":1,"language":"zh_CN","city":"Chaoyang","province":"Beijing","country":"CN","headimgurl":"http://wx.qlogo.cn/mmopen/kZG1bNOzT8ROh4IQv8ENlicwB4dUmPObLhIuuVZibQps22JpqWOwDvrbG1HVleqCGaF2bF99KozJqaLnWfeGTHicAClY30IgD4H/0","privilege":[],"unionid":"oucfGvrYR6vmnj5W-T23qwpCrPTE"}
		string headURL = (string)json ["headimgurl"];
		//http://wx.qlogo.cn/mmopen/kZG1bNOzT8ROh4IQv8ENlicwB4dUmPObLhIuuVZibQps22JpqWOwDvrbG1HVleqCGaF2bF99KozJqaLnWfeGTHicAClY30IgD4H/0
//		http://wx.qlogo.cn/mmopen/kZG1bNOzT8ROh4IQv8ENlicwB4dUmPObLhIuuVZibQps22JpqWOwDvrbG1HVleqCGaF2bF99KozJqaLnWfeGTHicAClY396IgD4H/96
		headURL = headURL.Substring (0, headURL.LastIndexOf ("/"));
		headURL = headURL+"/96";
//		headURL = headURL.Replace (headURL.Substring (headURL.LastIndexOf ("/") + 1), "96");//微信只用 96 尺寸的
		LocalStore.SetLocal (LocalStore.OTHER_HEADIMG, headURL);
		LocalStore.DelLocal(LocalStore.HEADIMG + headURL);//删除缓存
		PlatForm.inst.GetSdk ().Dispatch (CALL_AUTH_HEAD_GET_WEIXIN, headURL);

		PlatForm.inst.GetSdk ().Dispatch (CALL_LOGIN_PF, null);//这里可能需要服务器验证
	}
}