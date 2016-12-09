using System;
using UnityEngine;
using System.Collections.Generic;
using LitJson;

public class Ex_And:Ex_Local
{
	private AndroidJavaClass jClass;
	private AndroidJavaObject jObject;
	private AndroidJavaClass activityClass;

	public Ex_And () : base ()
	{
		
	}

	public override void Init (string type)
	{		
		#if UNITY_ANDROID
		activityClass = new AndroidJavaClass ("com.meng52.unity.sdk.UnityContext");
		jObject = activityClass.GetStatic<AndroidJavaObject> ("self");
		#endif
	}

	public override void Init (string className, string method)
	{
//		SdkFunction.getInstance ().Init ("com.meng52.unity.platform.qihoo.QihooSDK", "self");
//		#if UNITY_ANDROID
		//		jClass = new AndroidJavaClass (className);
		//		jObject = jClass.GetStatic<AndroidJavaObject> (method);
//		jObject.Call ("setPlatformClass", new string[]{ method });
//		#endif
	}

	public override void Clear ()
	{		
	}

	public override void Call (string method, Action<object> fun)
	{
		if (jObject != null)
		{
			jObject.Call (method);
			if (fun != null) {
				this.funs [method] = fun;
			}
		}
	}

	public override void Call (string method, object[] args, Action<object> fun)
	{
		if (jObject != null)
		{
			if (
				method != Ex_Local.CALL_LOGIN_PF
				&& method != Ex_Local.CALL_LOGIN_WEIXIN_CODE 
				&& method != Ex_Local.CALL_LOGIN_QQ_HEAD
				&& method != Ex_Local.CALL_LOGIN_QQ_TOKEN
				&& method != Ex_Local.CALL_AUTH_HEAD_WEIXIN 
				&& method != Ex_Local.CALL_AUTH_INFO_WEIXIN 
				&& method != Ex_Local.CALL_AUTH_RE_TOKEN_WEIXIN 
				&& method != Ex_Local.CALL_AUTH_HEAD_GET_WEIXIN 
				&& method != Ex_Local.CALL_AUTH_TOKEN_WEIXIN
				&& method != Ex_Local.ERROR_NO_WX
				&& method != Ex_Local.ERROR_NO_QQ
			) {
				jObject.Call (method, args);
			}
			string methodEvt = method;
			if (method == Ex_Local.CALL_QQ || method == Ex_Local.CALL_WECHARIMG || method == Ex_Local.CALL_WECHAR || method == Ex_Local.CALL_WEIBO) {
				methodEvt = Ex_Local.CALL_FUN;
			}
			if (fun != null) {
				this.funs [methodEvt] = fun;
			}
		}
	}

	#if UNITY_ANDROID
	public override void CallBuy(string pid,Action<object> fun)
	{
		this.funs [Ex_Local.CALL_BUY] = fun;
		IAP.inst.Buy (pid);
	}
	public override void CallInitPIDs(string[] pids,string androidKEY, Action<object> fun)
	{
		this.funs [Ex_Local.CALL_INIT_PIDS] = fun;
		IAP.inst.InitProduct (pids,androidKEY);

	}
	#endif

}