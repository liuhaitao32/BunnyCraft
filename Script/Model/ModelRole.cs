using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using FairyGUI;

public class ModelRole :BaseModel
{
	/***********************************基本数据******************************/
	public int record;//零时用
	public string fuid;
	public long Oneday = (long)24 * 60 * 60 * 1000 * 10000;
	public long OneHour = (long)60 * 60 * 1000 * 10000;
	public long OneMili = (long)60 * 1000 * 10000;

	/***********************************人物信息界面******************************/
	public int selectIndex;//邮件的选择记录
	public  int isGuest;//1表示游客 其他表示自己
	public DateTime phototTimer;//照片更新的时间
	public string expendCoinNum;//消耗coin的数量
    public Dictionary<string, object> roleLove;
	public List<object> roleVoyage;
	public List<object> rolefigtRecord;
	public string longitude;
	public string latitude;
	public Dictionary<string, object> tempOpenData;//打开用户uid
    public Dictionary<string, object> tempData;
	public int like_Coin;//喜欢花费coin
	public bool isSave = false;
    //人物界面的tab
    public const int ROLEVIEW = 1;
    public const int OTHERVIEW = 2;
    public int tab_Role_CurSelect1 = 0;
    public int tab_Role_CurSelect2 = 0;
    public int tab_Role_CurSelect3 = 0;
    public int tab_Role_Select1 = -1;
    public int tab_Role_Select2 = -1;
    public int tab_Role_Select3 = -1;

    //其他
    public int tab_CurSelect1 = 0;
    public int tab_CurSelect2 = 0;
    public int tab_CurSelect3 = 0;
    public int tab_Select1 = -1;
    public int tab_Select2 = -1;
    public int tab_Select3 = -1;


    public Dictionary<string, object> giftData;
	public string giftIndex;
	public object[] rank_title_data;
	public Dictionary<string,object> rankData;
	public static Dictionary<string, object> fightReward = null;//战斗奖励
	public static Dictionary<string, object> fightUserData = null;
	public int oldEl_score;

    //缓存的数据
    public static Dictionary<string, object> fight1 = null;//赛季
    public static object[] fight2 = null;//比赛记录1
    public static object[] fight3 = null;//比赛记录2
    public static Dictionary<string, object> fight4 = null;//赛季旅程
    public static object[] rankTempData = null;//段位
    public static object[] loveTempData = null;//喜欢
    public static object[] killTempData = null;//杀人
    public static object[] mvpTempData = null;//mvp
    public static List<string> attentionFight = new List<string>();//战斗详情的关注关系

    public static bool isCloseMask=false;
    public static bool isUploadOver = false;


    public void AddAttentionFight(string uid)
    {
        if (!ModelRole.attentionFight.Contains(uid))
        {
            ModelRole.attentionFight.Add(uid);
        }
    }

    public void RemoveAttentionFight(string uid)
    {
        if (ModelRole.attentionFight.Contains(uid))
        {
            ModelRole.attentionFight.Remove(uid);
        }
    }



    //界面uid
    public List<Dictionary<string,object>> uids = new List<Dictionary<string, object>>();
	public Dictionary<string, object> otherInfo;
    internal static bool isHasMicro;

    public void ClearFightData()
    {
        if (fight1 != null)
        {
            fight1 = null;
        }
        if (fight2 != null)
        {
            fight2 = null;
        }
        if (fight3 != null)
        {
            fight3 = null;
        }
        if (fight4 != null)
        {
            fight4 = null;
        }

    }




    //打开用户的信息，作用是保存关注、拉黑的状态
	public void SetTempOpenData (Dictionary<string,object> d)
	{
		if (tempOpenData == null)
			tempOpenData = new Dictionary<string, object> ();
		if (!tempOpenData.ContainsKey (d ["uid"].ToString ()))
			tempOpenData [d ["uid"].ToString ()] = d;
        
	}
    //保存代开用户信息之前的数据
	public void SetTempData (List<object> data, int[] tag)
	{
		if (tempData == null)
		{
			tempData = new Dictionary<string, object> ();
		}
		tempData ["data"] = data;
		tempData ["tag"] = tag;
	}
    public void clearRole()
    {
        uids.Clear();
        tab_Select1 = -1;
        tab_Select2 = -1;
        tab_Select3 = -1;
        tab_Role_Select1 = -1;
        tab_Role_Select2 = -1;
        tab_Role_Select3 = -1;
    }
	public void SetTabSelect (object[] value)
	{
//        if (value[1] != null)
//        {
            tab_Role_Select1 = -1;
            tab_Role_Select2 = -1;
            tab_Role_Select3 = -1;
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["uid"] = value[1];
            data["tab1"] = value[2];
            data["tab2"] = value[3];
            data["tab3"] = value[4];
			data ["isShow"] = value [0];
            uids.Add(data);
//        }
//        else
//        {
            tab_Select1 = tab_CurSelect1;
            tab_Select2 = tab_CurSelect2;
            tab_Select3 = tab_CurSelect3;
//
//        }
    }
	public void SetTabSelect(string uid,Dictionary<string, object>other = null)
    {
        Dictionary<string, object> user = null;
		Dictionary<string, object> v;
		for (var i = uids.Count - 1; i >= 0; i--) {
			v = (Dictionary<string, object>)uids [i];
//		}
//        foreach (Dictionary<string, object> v in uids)
//        {
            if (v["uid"].ToString().Equals(uid))
            {
                user = v;
				break;
            }
        }

        if (user != null)
        {
			if (other != null) {
				user = other;
			}
            tab_Role_Select1 = (int)user["tab1"];
            tab_Role_Select2 = (int)user["tab2"];
            tab_Role_Select3 = (int)user["tab3"];
        }
    }


    public void CloseAllFun ()
	{
        Dictionary<string,object> data_=(Dictionary<string,object>)uids[0];
        if (data_["uid"].ToString().Equals(ModelManager.inst.userModel.uid))
        {
            
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd["fuid"] = data_["uid"];
            NetHttp.inst.Send(NetBase.HTTP_FUSERGET, dd, (VoHttp vo) =>
            {
                if (vo.data != null)
                {
                    otherInfo = (Dictionary<string, object>)vo.data;
                    Dictionary<string,object> data=(Dictionary<string,object>)uids[0];
                    SetTabSelect(data["uid"].ToString());
                    int num = uids.Count;
                    uids.Clear();
                    uids.Add(data_);
                    ViewManager.inst.CloseScene(2,true);
                }
            });
        }
        else
        {
			//data ["isShow"]
//			Dictionary<string, object> v;
//			int count = 0;
//			for(var i=0;i<this.uids.Count;i++){
//				if (this.uids [i] != null) {
//					v = (Dictionary<string, object>)uids [i];
//					if (v ["isShow"] == null) {
//						count += 1;
//					}
//				}
//			}
			ViewManager.inst.CloseScene (2,true);
            this.uids = new List<Dictionary<string,object>>();
            this.otherInfo = null;
        }
       
	}

	public string GetAddrCfg (Dictionary<string, object> dc, string area, string key)
	{
		string ar = "";
		if (!Tools.IsNullEmpty (area))
		{
			foreach (KeyValuePair<string,object> obj in dc)
			{
				if (obj.Value.Equals (area))
				{
					ar = obj.Key;
				}
			}
			if (ar.Equals (""))
			{
				ar = area;
			}
		}
		if (!Tools.IsNullEmpty (key))
		{
			foreach (KeyValuePair<string, object> obj in dc)
			{
				if (obj.Key.Equals (key))
				{
					ar = ((string)obj.Value);
				}
			}
		}
		return ar;
	}
}