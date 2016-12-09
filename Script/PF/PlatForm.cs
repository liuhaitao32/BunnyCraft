using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

//modified: -  Source/Scripts/Core/DisplayObject.cs
//modified:   Source/Scripts/UI/Controller.cs
//modified: -  Source/Scripts/UI/GComponent.cs
//modified:   Source/Scripts/UI/GList.cs
//modified:   Source/Scripts/UI/GObject.cs
public class PlatForm
{
	public const string WIN = "win";
	public const string IOS = "ios";
	public const string AND = "and";

	//pf
	public const string EX_LOCAL = "ad_cn";
	//
	public const string EX_AND = "ad_qihoo";//360
//	public const string EX_AND = "ad_cstv";//chushouTV
	//
	public const string EX_IOS = "ios_cn";

	//
	public const bool local = false;
    //public const string SERVER = "140.210.4.47";

    //public const string SERVER4 = "192.168.1.118";
    //public const string SERVER6 = "192.168.1.118";

//    public string HTTP = local ? "http://192.168.1.119:8899/gateway/" : "http://140.210.4.47/gateway/";
    public string HTTP = local ? "http://192.168.1.118:8899/gateway/" : "http://140.210.4.47/gateway/";
    //    public string HTTP = "http://140.210.4.47/gateway/";
    public string SERVER4 = local ? "192.168.1.118" : "140.210.4.47";
	public string SERVER6 = local ? "192.168.1.118" : "140.210.4.47";
	public int PORT = 7777;
	public string pf;
	public string type = "";
	public string language;
	public float timeout = 5f;
	private string app_Version = "1.0.0";
	private string cfg_Version = "";
	public string wifi;
	public bool touch3dGot = false;
	private static PlatForm instance;
	private Ex_Local ex;

	public PlatForm ()
	{
	}
	public static PlatForm inst
	{
		get
		{
			if (instance == null)
				instance = new PlatForm ();
			return instance;
		}
	}

	public string App_Version
	{
		get
		{
			//			if (app_Version == "")
			//				app_Version = LocalStore.GetLocal (LocalStore.APP_VERSION);
			return app_Version;
		}
		set
		{
			app_Version = value;
//			LocalStore.SetLocal (LocalStore.APP_VERSION, value);
		}
	}

	public string Cfg_Version
	{
		get
		{
			if (cfg_Version == "")
				cfg_Version = LocalStore.GetLocal (LocalStore.CFG_VERSION);
			return cfg_Version;
		}
		set
		{
			cfg_Version = value;
			LocalStore.SetLocal (LocalStore.CFG_VERSION, value);
		}
	}

	public string GetPF_update_link(object obj){
		Dictionary<string,object> _dic = (Dictionary<string,object>)obj;
		string re = "";
		if(_dic.ContainsKey(this.pf)){
			re = (string)_dic[this.pf];
		}
		return re;
	}
	public void CheckPlatForm ()
	{
		#if UNITY_IOS
		this.pf = PlatForm.EX_IOS;
			return;
		#endif
		RuntimePlatform rp = Application.platform;
		if (rp == RuntimePlatform.WindowsPlayer || rp == RuntimePlatform.WindowsEditor || rp == RuntimePlatform.OSXEditor)
			this.pf = PlatForm.EX_LOCAL;
		else if (rp == RuntimePlatform.Android)
			this.pf = PlatForm.EX_AND;
		else if (rp == RuntimePlatform.IPhonePlayer)
			this.pf = PlatForm.EX_IOS;

		Log.debug ("Platfrom - " + this.pf);
	}

	public void CheckLanguage ()
	{
		string lan = LocalStore.GetLocal (LocalStore.LOCAL_LANGUAGE);
		if (lan != "")
		{
			language = lan;
		}
		else
		{
			lan = Application.systemLanguage.ToString ();
			if (lan == SystemLanguage.Chinese.ToString () || lan == SystemLanguage.ChineseSimplified.ToString ())
				language = "cn";
			else if (lan == SystemLanguage.ChineseTraditional.ToString ())
				language = "tw";
			else if(lan == SystemLanguage.English.ToString())
				language = "en";	
			else
				language = "cn";
			LocalStore.SetLocal (LocalStore.LOCAL_LANGUAGE, language);
		}
//		Debug.LogError ("System Language - " + lan);
	}

	public void SwitchLanguage (string name)
	{
		if (language == name)
			return;
		language = name;
		LocalStore.SetLocal (LocalStore.LOCAL_LANGUAGE, language);

		NetHttp.inst.Send (NetBase.HTTP_CHANGELAN, "lan=" + name, (VoHttp v) =>
		{			
			if (Convert.ToBoolean (v.data))
			{
				NetHttp.inst.Send (NetBase.HTTP_CONFIG, "lan=" + name + "|key=return_msg", (VoHttp vo) =>
				{			
					DataManager.inst.Decode (vo.data);
					DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RELOGIN_GAME));
				},'|',null,60);
			}
		});
	}

	public void CheckNetwork ()
	{		
		//Application.runInBackground
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			//当前网络是否wifi
			Debug.Log ("internetReachability :   wifi");
			wifi = "wifi";
		}
		else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
		{
			//当前网络是否移动2\3\4\5 G
			Debug.Log ("internetReachability :   2、3、4G");
			wifi = "3g";
		}			
	}

	public Ex_Local GetSdk ()
	{
		if (ex != null)
			return ex;
		switch (this.pf)
		{
		case EX_LOCAL:
			ex = new Ex_Local ();
			break;
		case EX_AND:
			ex = new Ex_And ();
			break;
		case EX_IOS:
			ex = new Ex_Ios ();
			break;
		}
		return ex;
	}
}
