using UnityEngine;
using System.Collections.Generic;

public class LocalStore
{
	public const string LOCAL_ID = "mid";
	public const string LOCAL_TYPE = "type";
	public const string LOCAL_UID = "uid";
	public const string LOCAL_PWD = "pwd";
	public const string LOCAL_TEL = "tel";
	public const string LOCAL_UNAME = "uname";
	public const string LOCAL_UIDS = "uids_";
	public const string LOCAL_UIDS_LIST = "uids_list_";
	public const string LOCAL_SOUND = "sound";
	public const string LOCAL_MUSIC = "music";
	public const string LOCAL_VOICE = "voice";//语音
	public const string LOCAL_SHIPRED = "shipRed";//飞船显示
//    public static string LOCAL_INVITE = "invite";//邀请
	public const string LOCAL_LANGUAGE = "language";
//    public static string LOCAL_ATTENTION = "attention";
	public const string LOCAL_QRCODE = "QRcode";//二维码
	public const string LOCAL_BASEIMAGE = "baseImage";//二维码底图
	public const string LOCAL_SHOPRED = "shopRed";//商城红点
	public const string CFG_VERSION = "cfg_version";
	public const string APP_VERSION = "app_version";
	public const string NOTICE_VERSION = "notice_version";
	public const string GUIDE_LOVE = "guide_love";
	//

	public const string HEADIMG = "headimg_";
	public const string OTHER_HEADIMG = "other_headimg_";
	//
	public const string QQ_OPENID = "qq_openid";
	public const string QQ_TOKEN = "qq_token";
	public const string QQ_DATE = "qq_date";
	//
	public const string WX_OPENID = "wx_openid";
	public const string WX_TOKEN = "wx_token";
	public const string WX_RE_TOKEN = "refresh_token";
	//
	public const string UID_QQ_OPENID = "uid_qq_openid_";
	public const string UID_QQ_TOKEN = "uid_qq_token_";
	public const string UID_QQ_DATE = "uid_qq_date_";

	//
	public const string UID_WX_OPENID = "uid_wx_openid_";
	public const string UID_WX_TOKEN = "uid_wx_token_";
	public const string UID_WX_RE_TOKEN = "uid_wx_re_token_";
    //登录双帐号
	public const string LOCAL_IDS = "local_id_";
	public const string LOCAL_IDNUM = "0";

	private static string LOCAL_SIGN = PlatForm.inst.pf+PlatForm.inst.type + "_";
    private static string LOCAL_Friend = "uid_friend";

    public static string GetLocal (string name)
	{
		name = LOCAL_SIGN + name;
		if (PlayerPrefs.HasKey (name))
			return	PlayerPrefs.GetString (name);
		return "";
	}
	public static void DelLocal (string name)
	{
		name = LOCAL_SIGN + name;
		if (PlayerPrefs.HasKey (name)) {
			PlayerPrefs.DeleteKey (name);
		}
	}
    public static void SetLocal (string name, string value)
	{
		name = LOCAL_SIGN + name;
		PlayerPrefs.SetString (name, value);
        PlayerPrefs.Save ();
	}

	public static void Set_QQ_Info(string uid,string openid,string token,string date){
		SetLocal (UID_QQ_OPENID+uid,openid);
		SetLocal (UID_QQ_TOKEN+uid,token);
		SetLocal (UID_QQ_DATE+uid,date);
		//
		SetLocal (QQ_OPENID,openid);
		SetLocal (QQ_TOKEN,token);
		SetLocal (QQ_DATE,date);
	}
	public static void Set_WX_Info(string uid,string openid,string token,string reToken){
		SetLocal (UID_WX_OPENID+uid,openid);
		SetLocal (UID_WX_TOKEN+uid,token);
		SetLocal (UID_WX_RE_TOKEN+uid,reToken);
		//
		SetLocal (WX_OPENID,openid);
		SetLocal (WX_TOKEN,token);
		SetLocal (WX_RE_TOKEN,reToken);
	}
	public static void SetUids (string uid,string uname, string pwd, string type,string phone)
	{
		string nameList = DelUids(uid);
		string content = uid + "," + uname + "," + pwd + "," + type + "," + phone;
		SetLocal (LocalStore.LOCAL_UIDS + uid, content);
		SetLocal (LocalStore.LOCAL_UIDS_LIST, nameList + ";" + uid + ";");
	}
	public static string DelUids (string uid)
	{
		string nameList =  GetLocal (LocalStore.LOCAL_UIDS_LIST);
		if (nameList != "") {
			string key = ";" + uid + ";";
			if (nameList.IndexOf (key) > -1) {
				nameList = nameList.Replace (key, "");
			}
			DelLocal (LocalStore.LOCAL_UIDS + uid);
			SetLocal (LocalStore.LOCAL_UIDS_LIST, nameList);
		}
		return nameList;
	}

	// uid pwd type
	public static List<string[]> GetUids ()
	{
		List<string[]> list = new List<string[]> ();
		string name = GetLocal (LocalStore.LOCAL_UIDS_LIST);
		if (name != "")
		{
			string[] names = name.Split (';');
			string[] obj;
			string key = "";
			foreach (string n in names)
			{
				if (n != "" && n != null) {
					key = GetLocal (LocalStore.LOCAL_UIDS + n);
					if (key != null) {
						obj = key.Split (',');
						list.Add (obj);
					}
				}
			}
		}
		return list;
	}

    public static void SetFriendUID(string uid) {
        string content = GetFriendUID();
        content += uid + ",";
        SetLocal(LocalStore.LOCAL_Friend, content);
    }

    public static string  GetFriendUID() {
        return GetLocal(LocalStore.LOCAL_Friend);
    }

    //	public static void SetPwd (string value)
    //	{
    //		string name = LOCAL_SIGN + LOCAL_PWD;
    //		PlayerPrefs.SetString (name, Tools.Md5 (value));
    //		PlayerPrefs.Save ();
    //	}


    public static string getFreeWords(string name) {
        string str = LocalStore.GetLocal(name);
        if(str != "") {
            string[] arr = str.Split('_');
            for(int i = 0; i < arr.Length; i++) {
                string[] strs = arr[i].Split('-');
                if(strs[1] == "0") {
                    str = strs[0];
                    break;
                } else { 
                        str = "";
                    break;
                }
            }
        }
        return str;
    }

    public static void setFreeWords(string name, string value, string free) {
        string str = LocalStore.GetLocal(name);
        int index = str.IndexOf(value);
        if(index != -1) {
            string cc = str.Substring(index, value.Length + 2);
            str= str.Replace(cc, value + "-" + free);
        } else {
            if(str != "") str += "_";
            str += value + "-" + free;
        }
        LocalStore.SetLocal(name, str);
    }
    

    public static void Clear ()
	{
		PlayerPrefs.DeleteAll ();
	}

	//	public static bool IsID ()
	//	{
	//		return PlayerPrefs.HasKey (LOCAL_ID);
	//	}
	//
	//	public static bool IsSign ()
	//	{
	//		return PlayerPrefs.HasKey (MID + LOCAL_SIGN);
	//	}
	//
	//	public static bool IsUid ()
	//	{
	//		return PlayerPrefs.HasKey (MID + LOCAL_ID);
	//	}
	//
	//	public static bool IsPwd ()
	//	{
	//		return PlayerPrefs.HasKey (MID + LOCAL_PWD);
	//	}
}

