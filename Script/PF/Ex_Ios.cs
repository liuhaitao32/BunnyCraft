using System.Runtime.InteropServices;
using LitJson;
using UnityEngine;
using System;

public class Ex_Ios:Ex_Local
{
	//导入定义到.m文件中的C函数
	#if UNITY_IOS

//	[DllImport("__Internal")]
//	public static extern void c_test_test ();

	[DllImport("__Internal")]
	public static extern void c_test ();

	[DllImport("__Internal")]
	private static extern void c_getTouch3DType();

	[DllImport("__Internal")]
	private static extern void c_setTouch3DTypeNull ();

	[DllImport("__Internal")]
	public static extern void c_openPhoto(string w,string h);

	[DllImport("__Internal")]
	private static extern void c_share_wechar(string key,string text,string url,string bitmap,string type);

	[DllImport("__Internal")]
	private static extern void c_share_wechar_img(string key,string bitmap,string type);

	[DllImport("__Internal")]
	private static extern void c_share_weibo(string key,string text,string url,string bitmap,string reURI);

	[DllImport("__Internal")]
	private static extern void c_share_qq(string key,string url,string title,string des,string image,string type);

	[DllImport("__Internal")]
	private static extern void c_scan();

	[DllImport("__Internal")]
	private static extern void c_copyToPasteboard(string str);

	[DllImport("__Internal")]
	private static extern void c_init_weixin (string key);

	[DllImport("__Internal")]
	private static extern void c_login_weixin (string openid);

	[DllImport("__Internal")]
	private static extern void c_init_qq (string key,string openId,string token,string date,string url);

	[DllImport("__Internal")]
	private static extern void c_login_qq (string openid);
	#endif

	public Ex_Ios () : base ()
	{
	}

	public override void Init (string type)
	{

	}

	public override void Init (string className, string method)
	{
	}

	public override void Clear ()
	{
	}

	public override string CallReturn (string method)
	{
		#if UNITY_IOS
		 if(method == Ex_Local.CALL_RE_TOUCH3D_SET){
			c_setTouch3DTypeNull();
		}

		#endif
		return "";
	}

	public override void Call (string method, Action<object> fun)
	{
		#if UNITY_IOS
		if (method == Ex_Local.CALL_SCAN){
			c_scan ();
		}
		else if (method == Ex_Local.CALL_RE_TOUCH3D){
		}
		else if (method == Ex_Local.CALL_RE_TOUCH3D_GET){
			c_getTouch3DType();
		}
		this.funs [method] = fun;
		#endif
	}

	public override void Call (string method, object[] args, Action<object> fun)
	{
		string methodEvt = method;
		#if UNITY_IOS
		if (method == Ex_Local.CALL_WECHAR)
		{
			methodEvt = Ex_Local.CALL_FUN;
			c_share_wechar (args [0].ToString (), args [1].ToString (), args [2].ToString (), args [3].ToString (), args [4].ToString ());
		}
		else if (method == Ex_Local.CALL_WEIBO)
		{
			methodEvt = Ex_Local.CALL_FUN;
			c_share_weibo (args [0].ToString (), args [1].ToString (), args [2].ToString (), args [3].ToString (), args [4].ToString ());
		}
		else if (method == Ex_Local.CALL_OPENPHOTO)
		{
			c_openPhoto (args [0].ToString (), args [1].ToString ());
		}
		else if (method == Ex_Local.CALL_COPYTO)
		{
			c_copyToPasteboard (args [0].ToString ());
		}
		else if(method == Ex_Local.CALL_WECHARIMG){
			methodEvt = Ex_Local.CALL_FUN;
			c_share_wechar_img(args [0].ToString (),args [1].ToString (),args [2].ToString ());
		}
		//
		else if(method == Ex_Local.CALL_INIT_QQ){
			c_init_qq(args [0].ToString (), args [1].ToString (), args [2].ToString (), args [3].ToString (), args [4].ToString ());
		}
		else if (method == Ex_Local.CALL_QQ) {
			methodEvt = Ex_Local.CALL_FUN;
			c_share_qq (args [0].ToString (), args [1].ToString (), args [2].ToString (), args [3].ToString (), args [4].ToString (), args [5].ToString ());
		}
		else if (method == Ex_Local.CALL_LOGIN_QQ) {
			methodEvt = "";
			c_login_qq(args [0].ToString ());
		}
		else if(method == Ex_Local.CALL_INIT_WEIXIN){
			c_init_weixin(args [0].ToString ());
		}
		else if(method == Ex_Local.CALL_LOGIN_WEIXIN){
			c_login_weixin(args [0].ToString ());
		}

		//
		if(methodEvt!="" && methodEvt.Length>0){
			if (fun != null) {
				this.funs [methodEvt] = fun;
			}
		}
		#endif

	}
	#if UNITY_IOS
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