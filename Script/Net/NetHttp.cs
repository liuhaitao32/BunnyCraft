using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class NetHttp:MonoBehaviour
{
	//忽略loading列表
	public Dictionary<string,object> ignore;

	//	public delegate void HttpEvent (VoHttp vo);
//	public bool ConfigWasGet = false;
	private string method;
	private object data;
	private Action<VoHttp> fun;
	private Action<VoHttp> errorfun;
	private char chars;

	private static NetHttp instance;
	private WWW www;
	private VoHttp vo;
	private byte[] by1;
	private byte[] by2;
	private string timer;
	private string error;

	//timeout
	private float timeout = 0f;
	private float timeMax = PlatForm.inst.timeout;

	public NetHttp ()
	{		
		ignore = new Dictionary<string, object> ();
		ignore [NetBase.HTTP_GETEFFORT] = "1";
		ignore [NetBase.HTTP_DEL_NOTICE] = "1";
	}

	public static NetHttp inst
	{
		get
		{			
			GameObject go = new GameObject ("NetHttp_" + Tools.GetRandom ());
//			Log.debug (go.name);
			instance = go.AddComponent<NetHttp> ();
//			Debug.LogError ("-->> "+go.name);
			return instance;
		}
	}

	//sex=1|age=2
	public void Send (string method, string data, Action<VoHttp> fun, char chars = '|', Action<VoHttp> error = null, float timeout = 15f)
	{
//		Debug.LogError (this.gameObject.name+" Send ++ "+method);
//		this.gameObject.name +="method";
		if (instance == null)
			throw new UnityException ("调用Instance方法初始化本类");
		timeMax = timeout;
		this.method = method;
		this.data = data;
		this.fun = fun;
		this.chars = chars;
		this.errorfun = error;
	
		SendHttpRequest (0);
	}

	//d["sex"] = 1;
	public void Send (string method, Dictionary<string,object> data, Action<VoHttp> fun, Action<VoHttp> error = null, float timeout = 15f)
	{
//		Debug.LogError (this.gameObject.name+" Send ++ "+method);
//		this.gameObject.name +="method";
		if (instance == null)
			throw new UnityException ("调用Instance方法初始化本类");
		timeMax = timeout;
		this.method = method;
		this.data = data;
		this.fun = fun;
		this.errorfun = error;

		SendHttpRequest (1);
	}

	private void SendHttpRequest (int type = 0)
	{		
		if (!ignore.ContainsKey (this.method) && !FightMain.fightTest)
			ViewManager.inst.ShowWait ();

		vo = new VoHttp ();
		if (type == 0)
			by1 = vo.ToBytes (method, (string)data, chars);
		else
			by1 = vo.ToBytes (method, (Dictionary<string,object>)this.data);
//		Log.debug ("HttpSend:" + this.method);// + " " + JsonUtility.ToJson (vo)

		timeout = 0;
		TimerManager.inst.Add (1f, 0, CheckTime);
		this.StartCoroutine (Load ());
	}

	private void CheckTime (float time)
	{
		timeout += 1;
//		Log.debug ("timeout - " + timeout.ToString ());
		if (timeout > timeMax)
		{
			error = "Timeout";	
			Clear (0);
		}
	}

	private IEnumerator Load ()
	{	
		Dictionary<string,string> header = new Dictionary<string, string> ();
		header.Add ("Content-Type", "application/x-amf");
		www = new WWW (PlatForm.inst.HTTP, by1, header);
		yield return www;
		if (www != null && www.error != null)
		{			
			error = "HttpError:" + www.error;	
			Debug.LogError (error);
			Clear (0);
		}
		else
		{		
			error = "";
			if (www.bytes.Length != 0)
			{	
				by2 = www.bytes;		
				vo.toDatas (by2);
				if (!ModelManager.inst.gameModel.isTimerGetServer) {
					//ModelManager.inst.gameModel.isTimerGetServer = true;
					ModelManager.inst.gameModel.gameStartTimer = System.DateTime.Now.Ticks;
					ModelManager.inst.gameModel.time = vo.server_now;
				}
				// Log.debug ("HttpReceive:" + this.method);//+ " " + JsonUtility.ToJson (vo)
				Clear (1);
				int code = int.Parse (vo.return_code);
				if (code == 0)
				{					
					if (this.fun != null)
						this.fun (vo);
				}
				else
				{
					Debug.LogError ("http re back code is " + code + " -- " + this.method);
					string msg = Tools.GetMessageById (code.ToString ());
					if (msg == null)
						msg = ((Dictionary<string,object>)vo.data) ["msg"].ToString ();
					//100000 未登录 100001 版本更新 100002 配置更新
					if (code == 100000 || code == 100002)
					{							
//						DispatchManager.inst.Dispatch (new MainEvent (MainEvent.SOCKET_CLOSE));
						bool tip = true;
						if(code == 100000){
							if (ModelManager.inst.userModel.isBingding()) {
								string tel = ModelManager.inst.userModel.tel;
								if (ModelManager.inst.userModel.tel.IndexOf (Ex_Local.LOGIN_TYPE_QQ) > -1) {
									tel = Ex_Local.LOGIN_TYPE_QQ;
								} else if (ModelManager.inst.userModel.tel.IndexOf (Ex_Local.LOGIN_TYPE_WEIXIN) > -1) {
									tel = Ex_Local.LOGIN_TYPE_WEIXIN;
								} else {
									tel = "";
								}
								if (tel != "") {
//									error_code = code;
									error_pf_type = tel;
									ViewManager.inst.ShowAlert (msg, ReLoginPF);
									tip = false;
								}
							}
						}
						if (tip) {
							ViewManager.inst.ShowAlert (msg + " -- " + this.method, ReLogin);
						}
					}
					else if (code == 10000)//维护
					{
//						DispatchManager.inst.Dispatch (new MainEvent (MainEvent.SOCKET_CLOSE));
						ViewManager.inst.ShowAlert (msg, (int index) =>
						{
							Application.Quit ();
						}, false);
					}
					else
					{

                        //ViewManager.inst.ShowAlert ("Server:" + msg, Enter, false);
                        ViewManager.inst.ShowText(msg);
                    }
				}		
			}
		}
	}
//	private int error_code = 0;
	private string error_pf_type = "";
	private void ReLoginPF(int v){
		MediatorChangeAccountIcon.Login (error_pf_type);
	}
	private void Clear (int type)
	{
//		Debug.LogError (this.gameObject.name);
		this.StopCoroutine (Load ());
		TimerManager.inst.Remove (CheckTime);
		if (!FightMain.fightTest)
			ViewManager.inst.CloseWait ();
		if (www != null)
			www.Dispose ();

		if (type == 0)
		{
			string msg = Tools.GetMessageById ("14004");
			if (!DataManager.inst.ConfigWasGet)
			{
				ViewManager.inst.ShowAlert (msg, ReCall, false);
			}
			else
			{
				if (error != "")
				{
					if (error.IndexOf ("Timeout") > -1 || error.IndexOf ("Network") > -1)
						ViewManager.inst.ShowAlert (msg + " -- " + error, ReCall, false);
					else
					{
						ViewManager.inst.ShowAlert (msg + " -- " + error, null, false);
						Tools.Clear (this.gameObject);
					}
				}
				else
				{
					ViewManager.inst.ShowAlert (msg, ReCall, false);
				}
			}
		}
		else
		{
			Tools.Clear (this.gameObject);
		}
	}

	private void ReCall (int index)
	{		
		if (data is IDictionary)
			this.Send (this.method, (Dictionary<string,object>)this.data, this.fun);
		else
			this.Send (this.method, (string)this.data, this.fun);	
	}

	private void ReLogin (int index)
	{		
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RELOGIN_GAME));
	}

	private void Enter (int index)
	{
		if (errorfun != null)
			errorfun (this.vo);
	}


	public void sendHttp_wx (string url)
	{
		this.StartCoroutine ("httpTest", url);

	}

	public IEnumerator httpTest (string url)
	{
		WWW www = new WWW (url);
		yield return www;

//		Debug.Log ("--sendHttp_wx-- " + www.text);
//		Debug.Log ("--sendHttp_wx-- " + www.bytes.Length);
		//
		this.StopCoroutine ("httpTest");
		Tools.Clear (this.gameObject);
		//
		if (url.IndexOf ("grant_type=authorization_code") > -1) {
			JsonData re = JsonMapper.ToObject (www.text);
			LocalStore.SetLocal (LocalStore.WX_OPENID, (string)re ["openid"]);
			LocalStore.SetLocal (LocalStore.WX_TOKEN, (string)re ["access_token"]);
			LocalStore.SetLocal (LocalStore.WX_RE_TOKEN, (string)re ["refresh_token"]);
			//{"access_token":"Y9W9n13FGy-tquOQw22-rXhONlfcMLd_KKhnN8qxdSo9xg0w0AAaT7AsoEwUiWUG1UaM1Ahf5UwFnhUyxY7Riqsh9mj42dRNTpvJo3diN9s","expires_in":7200,"refresh_token":"i_by8R_BiTkHCdxyMIQCQZo6MgkrNMkg3AzmCfNB2PjRt1YaTcAxxT4Hhw07UpPbqgn7oFAIlW6VIhOYGfZeUT8iOftK08Gz--4CdnnTbGU","openid":"o9zsEwc8Xql8WP6QSSeB3fZC2bqs","scope":"snsapi_userinfo","unionid":"oucfGvrYR6vmnj5W-T23qwpCrPTE"}
			//https://api.weixin.qq.com/sns/oauth2/refresh_token?appid=APPID&grant_type=refresh_token&refresh_token=REFRESH_TOKEN
			//https://api.weixin.qq.com/sns/auth?access_token=ACCESS_TOKEN&openid=OPENID
			//https://api.weixin.qq.com/sns/userinfo?access_token=ACCESS_TOKEN&openid=OPENID
			PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_AUTH_INFO_WEIXIN, www.text);

		} else if (url.IndexOf ("grant_type=refresh_token") > -1) {
			PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_AUTH_RE_TOKEN_WEIXIN, www.text);
		} else if (url.IndexOf ("sns/auth?") > -1) {
			PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_AUTH_TOKEN_WEIXIN, www.text);
		} else if (url.IndexOf ("sns/userinfo?") > -1) {
			PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_AUTH_HEAD_WEIXIN, www.text);
		} else if (url.IndexOf ("graph.qq.com/user/get_user_info?") > -1) {
			JsonData re = JsonMapper.ToObject (www.text);
			string headURL = (string)re["figureurl_qq_2"];//100
			PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_LOGIN_QQ_HEAD, headURL);
		}
	}
}