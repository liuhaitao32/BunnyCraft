using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using System.Text;

public class ModelUser : BaseModel
{
	//获得新卡牌
	public static string RED_NEWCARD = "new_card";
	//留言板有新的留言
	public static string RED_MSGBOARD = "msg_board";
	//粉丝
	public static string RED_FANS = "fans";
	//有未领取的礼物邮件
	public static string RED_GIFTMSG = "gift_msg";
	//红包数量
	public static string RED_REDBAGNUM = "redbag_num";
	//支援数量
	public static string RED_SUPPORTNUM = "support_num";
	//请求入会数量
	public static string RED_GUILDAPPLYNUM = "guild_apply_num";
	//请求通过
	public static string RED_GUILDJOIN = "guild_join";
	//客服
	public static string RED_BUGMSG = "bug_msg";
	//关注
	public static string RED_FOLLOW = "follow";
	//踢出公会
	public static string RED_GUILDEXIT = "guild_exit";

    //是否在排行时被强制退出
    public bool isSettle = false;
	//登录天数
	public int catena;
	public string uid;
	public string tel;
	public string uname;
	public string unameTrue;
	public string pwd;
	public int gold;
	//金币
	public int coin;
	//钻石
	public int lv;
	public int effort_lv;
	//成就等级
	public object[] effort;
	public int exp;
	public Dictionary<string, object> head;
	public string sex;
	public int age;
	public Dictionary<string, object> card;
	//	public Dictionary<string,object> ship;
	public Dictionary<string, object> attrs;
	public string story;
	public Dictionary<string, object> records;
	public Dictionary<string, object> task;
	public Dictionary<string, object> notice;
	public Dictionary<string, object> season;
	public int ipList;
    //	public Dictionary<string,object> explore;
    public object[] card_group;
	public DateTime add_time;
	public DateTime login_time;
	public string pf_uid;
	public string sessionid;
	public string area;
	public string addr;
	public object[] push_server;
	public int el_score;
	public int rank_score;

	public bool isFirstShare;
	public int country;
	//是否第一次分享
	public double lon;
	public double lat;
	public string notice_version;
	//
	public string type_login;

    //背景音乐是否开启
    public bool isBGM;
    public bool isSound;
    private object ww;

    public bool isShowText = false;

    public ModelUser ()
	{
	}
	public bool isBingding(){
		return (tel != "" && tel != null);
	}
	public void SetData (Dictionary<string, object> data)
	{
        isBGM = SoundManager.inst.isMusic;
        isSound = SoundManager.inst.isSound;
		this.catena = (int)data ["catena"];
		this.notice = (Dictionary<string, object>)data ["notice"];
		this.uid = data ["uid"].ToString ();
		this.tel = (string)data ["tel"];
		if (this.tel == null) {
			this.tel = "";
		}
		this.uname = this.unameTrue = (string)data ["uname"];
		this.pwd = (string)data ["pwd"];
		this.gold = Convert.ToInt32 (data ["gold"]);
		this.coin = Convert.ToInt32 (data ["coin"]);
		this.lv = (int)data ["lv"];
		this.effort_lv = (int)data ["effort_lv"];
		this.exp = (int)data ["exp"];
		this.attrs = (Dictionary<string, object>)data ["attrs"];
		this.records = (Dictionary<string, object>)data ["records"];
		this.task = (Dictionary<string, object>)data ["task"];
		this.add_time = (DateTime)data ["add_time"];
		this.login_time = (DateTime)data ["login_time"];
		this.pf_uid = (string)data ["pf_uid"];
		this.sessionid = (string)data ["sessionid"];
        this.sex = (string)data["sex"];
        //      this.addr = (string)data["addr"];
        //this.area = (string)data["area"];
        //this.lon = data["lon"] == null ? 0 : (double)data["lon"];
        //this.lat = data["lat"] == null ? 0 : (double)data["lat"];
        this.push_server = (object[])data ["push_server"];
		this.age = data ["age"] == null ? 1 : (int)data ["age"];
		this.card = (Dictionary<string, object>)data ["card"];
		//		this.ship = (Dictionary<string,object>)data ["ship"];
		this.head = (Dictionary<string, object>)data ["head"];
		this.story = (string)data ["story"];
		this.card_group = (object[])data ["card_group"];
		this.el_score = (int)data ["el_score"];
		this.rank_score = (int)data ["rank_score"];
		this.effort = (object[])data ["effort"];
		this.country = (int)data ["country"];
		this.notice_version = (string)data ["notice_version"];
		this.season = (Dictionary<string,object>)data ["season"];
        this.ipList = ((object[])((Dictionary<string, object>)this.records["share_data"])["ip_list"]).Length;

        //		this.explore = (Dictionary<string,object>)(this.records ["explore"]);
        if (this.uname == null)
			this.uname = this.uid;
		PlatForm.inst.SERVER4 = (string)this.push_server [0];
		PlatForm.inst.PORT = Convert.ToInt32 (this.push_server [1]);

		GuideManager.inst.SetGuide ((int)this.records ["guide"]);
	}
	public bool Guide_card_lv(){
		bool b = false;
		if (this.card != null) {
			if (this.card.ContainsKey ("C020")) {
				Dictionary<string,object> c020 = (Dictionary<string,object>)this.card ["C020"];
				int clv = (int)c020 ["lv"];
				if (clv <= 1) {
					b = true;
				}
			}
		}
		return b;
	}
	public bool Guide_card_had(string id){//C603
		bool b = false;
		if (this.card != null) {
			if (this.card.ContainsKey (id)) {
				b = true;
			}
		}
		return b;
	}
	//card2.exp - card1.exp
	public int GetCardMulCard (object card1, object card2)
	{
		int count1 = Convert.ToInt32 (Tools.Analysis (card1, "exp"));
		int count2 = Convert.ToInt32 (Tools.Analysis (card2, "exp"));
		return count2 - count1;
	}

	public Dictionary<string, object> GetReward (object data)
	{
		Dictionary<string, object> user = (Dictionary<string, object>)Tools.Analysis (data, "user");
		if (user != null)
		{
			Dictionary<string, object> result = new Dictionary<string, object> ();
			if (user.ContainsKey ("exp"))
			{
				if (user.ContainsKey ("lv"))
				{
					result ["exp"] = CurLvExp ((int)user ["lv"], (int)user ["exp"]) - CurLvExp (lv, this.exp);
				}
				else
				{
					result ["exp"] = CurLvExp (lv, (int)user ["exp"]) - CurLvExp (lv, this.exp);
				}
			}
			if (user.ContainsKey ("gold"))
			{
				result ["gold"] = Convert.ToInt32 (user ["gold"]) - this.gold;
			}
			if (user.ContainsKey ("coin"))
			{
				result ["coin"] = Convert.ToInt32 (user ["coin"]) - this.coin;
			}
			if (user.ContainsKey ("card"))
			{
				Dictionary<string, object> cCard = new Dictionary<string, object> ();
				Dictionary<string,object> cUser = (Dictionary<string,object>)user ["card"];
				foreach (string cid in cUser.Keys)
				{					
					if (this.card.ContainsKey (cid))
					{
						Dictionary<string, object> cids = (Dictionary<string,object>)cUser [cid];
						int c = ((int)cids ["exp"] - (int)((Dictionary<string, object>)this.card [cid]) ["exp"]);
						if (c > 0)
							cCard.Add (cid, c);
					}
					else
					{
						cCard.Add (cid, cCard ["exp"]);
					}
				}
				if (cCard.Keys.Count > 0)
					result ["card"] = cCard;
			}
			if (user.ContainsKey ("el_score"))
			{
				result ["el_score"] = (int)user ["el_score"] - this.el_score;
			}
			if (user.ContainsKey ("rank_score"))
			{
				result ["rank_score"] = (int)user ["rank_score"] - this.rank_score;
			}
			if (user.ContainsKey ("effort_lv"))
			{
				result ["effort_lv"] = (int)user ["effort_lv"] - this.effort_lv;
			}
			return result;
		}
		return null;
	}

	//更新方法
	public void UpdateData (object data, Action fun = null)
	{
		Dictionary<string, object> user = (Dictionary<string, object>)Tools.Analysis (data, "user");
		if (user != null)
		{
			if (user.ContainsKey ("head"))
				this.head = (Dictionary<string, object>)user ["head"];
			
			if (user.ContainsKey ("exp"))
				this.exp = (int)user ["exp"];
			if (user.ContainsKey ("gold"))
				this.gold = Convert.ToInt32 (user ["gold"]);
			if (user.ContainsKey ("coin"))
				this.coin = Convert.ToInt32 (user ["coin"]);
			if (user.ContainsKey ("records"))
			{
				Dictionary<string, object> da = (Dictionary<string, object>)user ["records"];
//				if (da.ContainsKey ("redbag_coin") && (int)this.records ["redbag_coin"] != (int)da ["redbag_coin"])
//				{
//					if (((int)DataManager.inst.redbag ["exp"] * (int)da ["redbag_coin"] + exp) >= this.GetExpMax (lv))
//					{
//						ViewManager.inst.ShowAlert (Tools.GetMessageById ("13110"), (int bo) =>
//						{
//							if (bo == 1)
//								ViewManager.inst.ShowView<MediatorRedPackage> ();
//						}, true);
//					}
//				}
				this.records = da;
                this.ipList = ((object[])((Dictionary<string, object>)this.records["share_data"])["ip_list"]).Length;
				GuideManager.inst.SetGuideNormal ((int)this.records ["guide"]);
            }
			if (user.ContainsKey ("card"))
				this.card = (Dictionary<string, object>)user ["card"];
			if (user.ContainsKey ("el_score"))
			{
				ModelManager.inst.roleModel.oldEl_score = this.el_score;
				this.el_score = (int)user ["el_score"];
			}
			if (user.ContainsKey ("rank_score"))
				this.rank_score = (int)user ["rank_score"];
			if (user.ContainsKey ("effort"))
				this.effort = (object[])user ["effort"];
			if (user.ContainsKey ("effort_lv"))
				this.effort_lv = (int)user ["effort_lv"];
			if (user.ContainsKey ("lv"))
			{
				if (this.lv != (int)user ["lv"] && (int)user ["lv"] <= (((object[])DataManager.inst.systemSimple ["exp_config"]).Length + 1))
				{
					MediatorUserLevelUp.oldLv = this.lv;
					this.lv = (int)user ["lv"];
					ViewManager.inst.ShowView<MediatorUserLevelUp> (false);
				}
			}
			if (user.ContainsKey ("tel"))
				tel = (string)user["tel"];
			if (user.ContainsKey ("pwd"))
				pwd = (string)user["pwd"];
			if (user.ContainsKey ("uid"))
				uid = (string)user["uid"];

			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.USER_UPDATE));
		}
	}
	//	private void OnSendHandler (VoHttp vo)
	//	{
	//		int addExp = (int)(((Dictionary<string,object>)vo.data) ["add_exp"]);
	//		Dictionary<string,object> d = new Dictionary<string, object> ();
	//		d.Add (Config.ASSET_EXP, addExp);
	//		ViewManager.inst.ShowIcon (d,()=>{
	//			ModelManager.inst.userModel.UpdateData (vo.data);
	//			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_UPDATE));
	//		});
	//	}
	public void Login (string type, string uid, string pwd, Action fun,bool isChange = false)
	{
		string param = "pf=" + PlatForm.inst.pf;

		string loginType = type;
		if (loginType == "")
		{
//			LocalStore.SetLocal (LocalStore.LOCAL_TYPE, Ex_Local.LOGIN_TYPE_UID);//
		}
		else
		{
			if (type == Ex_Local.LOGIN_TYPE_TEL || type == Ex_Local.LOGIN_TYPE_QQ  || type == Ex_Local.LOGIN_TYPE_WEIXIN) {
				if (!isChange) {
					loginType = Ex_Local.LOGIN_TYPE_UID;
				}
			}
			param += "|utype=" + loginType;
			param += "|ustr=" + uid;
			param += "|pwd=" + pwd;
		}
		string idn;
//		if (ModelManager.inst.gameModel.loginDoubel)
//		{
//			param = "pf=" + PlatForm.inst.pf;
//			idn = LocalStore.GetLocal (LocalStore.LOCAL_IDNUM);
//			string[] ids;
//			if (idn == "")
//			{
//				idn = "0";
//			}
//			else
//			{
//				idn = idn == "0" ? "1" : "0";
//			}
//			LocalStore.SetLocal (LocalStore.LOCAL_IDNUM, idn);
//			string idse = LocalStore.GetLocal (LocalStore.LOCAL_IDS + idn);
//			if (idse != "")
//			{
//				ids = idse.Split (',');
//				uid = ids [0].ToString ();
//				pwd = ids [1].ToString ();
//				param += "|utype="+Ex_Local.LOGIN_TYPE_UID;
//				param += "|ustr=" + uid;
//				param += "|pwd=" + pwd;
//			}
//			else
//			{
//				uid = "";
//				pwd = "";
//			}
//		}
//		ViewManager.inst.ShowAlert (type+ " - Login Params - " + param);
//		Debug.LogError ("Login-----");
		NetHttp.inst.Send (NetBase.HTTP_LOGIN, param, (VoHttp v) =>
		{

			Dictionary<string, object> re = (Dictionary<string, object>)v.data;

			this.SetData (re);
            //Debug.LogError("start" + ModelManager.inst.userModel.season["start_time"] + "%%%%%%%%%%   end" + ModelManager.inst.userModel.season["end_time"]);

            if (this.country == 0)
            {
                PhoneManager.inst.GetGps(GetGps);
            }

            //
            if (type != "")
			{
				LocalStore.SetLocal (LocalStore.LOCAL_TYPE, type);
			}
			else{
				LocalStore.SetLocal (LocalStore.LOCAL_TYPE, Ex_Local.LOGIN_TYPE_UID);//
			}

			LocalStore.SetUids (this.uid,this.uname, this.pwd, Ex_Local.LOGIN_TYPE_UID,this.tel);
			//
			LocalStore.SetLocal (LocalStore.LOCAL_UID, this.uid);
			LocalStore.SetLocal (LocalStore.LOCAL_PWD, this.pwd);

			if (ModelManager.inst.gameModel.loginDoubel)
			{
				LocalStore.SetLocal (LocalStore.LOCAL_IDS + LocalStore.GetLocal (LocalStore.LOCAL_IDNUM), this.uid + "," + this.pwd);
			}
			if(!isChange){
				Main.inst.Socket_Close ();
				Main.inst.Socket_Start ();
				ModelManager.inst.Clear ();
			}
			if (fun != null)
				fun ();
		}, '|', (VoHttp vo) =>
		{
            
//            if (vo.return_code.Equals ("10001") || vo.return_code.Equals ("10002") || vo.return_code.Equals ("10003"))
//			{
//				List<string[]> pList = new List<string[]> ();
//				List<string> uList = new List<string> ();
//				foreach (string[] v in pList)
//				{
//					uList.Add (v [0]);
//				}
//				if (uList.Contains (uid))
//				LocalStore.DelUids (uid);
				if(!isChange){
					LocalStore.SetLocal(LocalStore.LOCAL_UID,"");
					LocalStore.SetLocal(LocalStore.LOCAL_TYPE,"");
					LocalStore.SetLocal(LocalStore.LOCAL_PWD,"");
				}
//			}

//            //检测头像是否不合格
//            Tools.checkHead();
        });
	}
    private void GetGps(string la, string lo)
    {
        //roleModel.longitude = lo;
        //roleModel.latitude = la;
        if (PhoneManager.inst.IsOpenGps)
        {
            string url = (string)DataManager.inst.systemSimple["gps_url"];
            Dictionary<string, object> countryData = (Dictionary<string, object>)DataManager.inst.systemSimple["society_location"];
            ww = LoaderManager.inst.Load(url + la + "," + lo, (object w) =>
            {
                WWW www = (WWW)w;
                string a = www.text.Replace("\n", "");
                a = a.Replace("\\", "");
                LitJson.JsonData jd = LitJson.JsonMapper.ToObject(a);
                LitJson.JsonData result = jd["result"];
                //string addr1 = (string)result["formatted_address"];
                //list_addr.Add(addr1);
                LitJson.JsonData addressComponent = result["addressComponent"];
                string country = (string)addressComponent["country"];
                //string city = (string)addressComponent["city"];
                int country_tag = Tools.GetCountry(country, countryData);
                string addr = "";
                LitJson.JsonData pois = result["pois"];
                //foreach (LitJson.JsonData v in pois)
                //{
                //    addr = (string)v["name"];
                //    break;
                //}
                //if(Tools.IsNullEmpty(addr) && Tools.IsNullEmpty(city))
                //{
                //    GetLocalGPS();
                //}
                //else
                //{
                //    Save(addr, roleModel.GetAddrCfg(addrDiction, city, ""), country_tag);
                //}
                Save(country_tag);
            });
        }
        else
        {
            //GetLocalGPS();
        }
#if UNITY_STANDALONE_WIN //UNITY_EDITOR_WIN || 
		//测试
		ModelManager.inst.roleModel.longitude = "0";
		ModelManager.inst.roleModel.latitude = "0";
#endif
    }
    private void Save(int country_tag)
    {
        
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["age"] = 1;
        dic["sex"] = this.sex;
        dic["country"] = country_tag;
        NetHttp.inst.Send(NetBase.HTTP_CHANGE_USERINFO, dic, (VoHttp v) =>
        {
            if ((bool)v.data)
            {
                this.age = 1;
                this.country = country_tag;
            }
        });
    }
    public string GetUName ()
	{
		if (uname == null)
		{
			return uid;
		}
		return uname;
	}

	public GComponent Add_Notice (GComponent par, Vector2 pos, int count = 0, bool isGreen = false, bool isNewBg = false,bool isScale=false)
	{
		if (par == null)
			return null;
		Remove_Notice (par);
		GComponent com;
		if (!isNewBg)
			com = Tools.GetComponent (Config.COM_RED1).asCom;
		else
			com = Tools.GetComponent (Config.COM_RED2).asCom;
		if (count != 0)
			com.GetChild ("n1").text = count.ToString ();
		if (isGreen)
		{
			com.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_tishi1");
//			ColorFilter cf = new ColorFilter ();
//			com.GetChild ("n0").filter = cf;
//			cf.AdjustHue (0.62f);
		}
		com.name = "red_update";
        if (isScale)
        {
            com.scale = new Vector2(0.6f, 0.6f);
        }
        
		par.AddChild (com);
		com.x = pos.x;
		com.y = pos.y;
		return com;
	}

	public void Remove_Notice (GComponent par)
	{
		GObject gg = par.GetChild ("red_update");
		if (gg != null)
			par.RemoveChild (gg, true);
	}

	public void Del_Notice (string key)
	{
		if (!notice.ContainsKey (key) || notice [key].ToString () == "0")
			return;
		NetHttp.inst.Send (NetBase.HTTP_DEL_NOTICE, "notice_key=" + key, (VoHttp vo) =>
		{
			if (key == ModelUser.RED_GUILDEXIT &&notice [key].ToString () == "1")//特殊
			{
				this.notice = (Dictionary<string, object>)vo.data;
				ModelManager.inst.chatModel.Clear ();
				ViewManager.inst.ShowText (Tools.GetMessageById ("22040"));
                ModelManager.inst.chatModel.Clear_RedCount();
			}else if(key == ModelUser.RED_GUILDJOIN && notice[key].ToString() == "1")//特殊
            {
                this.notice = (Dictionary<string, object>)vo.data;
                ModelManager.inst.chatModel.Clear();
                //ViewManager.inst.ShowText(Tools.GetMessageById("22040"));
                ModelManager.inst.chatModel.Clear_RedCount();
            } else
			{
				this.notice = (Dictionary<string, object>)vo.data;
                DispatchManager.inst.Dispatch(new MainEvent(MainEvent.RED_UPDATE));
            }
			//			Dictionary<string,object> no = (Dictionary<string,object>)vo.data;
			//			foreach (string n in no.Keys)
			//			{
			//				if (notice.ContainsKey (n))
			//					notice [n] = no [n];
			//			}
			
		});
	}

	public int Get_NoticeState (string key)
	{
		if (this.notice.ContainsKey (key))
		{

			if (key.Equals ("fans") || key.Equals ("follow"))
			{
				return ((object[])notice [key]).Length;
			}
			else
			{
				return Convert.ToInt32 (this.notice [key]);
			}

		}
		return 0;
	}

	//static
	public static bool GetCanBuy (string type, int buy_need, string tips = "")
	{
		if (type == Config.ASSET_GOLD)
		{
			if (ModelManager.inst.userModel.gold >= buy_need)
			{
				return true;
			}
			if (tips == "")
				ViewManager.inst.ShowText (Tools.GetMessageById ("14005"));
			else 
				ViewManager.inst.ShowText (Tools.GetMessageById (tips));
			return false;
		}
		else if (type == Config.ASSET_COIN)
		{
			if (ModelManager.inst.userModel.coin >= buy_need)
			{
				return true;
			}
			if (tips == "")
				ViewManager.inst.ShowText (Tools.GetMessageById ("14006"));
			else 
				ViewManager.inst.ShowText (Tools.GetMessageById (tips));
			return false;
		}
		if (tips == "")
			ViewManager.inst.ShowText (Tools.GetMessageById ("14007"));
		else 
			ViewManager.inst.ShowText (Tools.GetMessageById (tips));
		return false;
	}

	public bool IsSharedWeiXin ()
	{
		if (!this.records.ContainsKey ("weixin"))
			return false;
		else
		{
			if (Convert.ToInt32 (this.records ["weixin"]) == 1)
				return false;
			else
				return true;
		}
	}

	//根据成就等级获取 可请求卡牌品质和数量
	public Dictionary<string, object> GetCardRequestRarityAndCount (int effort_Lv)
	{
		Dictionary<string, object> result = new Dictionary<string, object> ();
		Dictionary<string, object> data = (Dictionary<string, object>)Tools.Analysis (DataManager.inst.guild, "support.card");
		foreach (string n in data.Keys)
		{
			Dictionary<string, object> new_Data = new Dictionary<string, object> ();
			new_Data.Add ("id", n);
			new_Data.Add ("num", Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "support.card." + n + ".num[" + (effort_Lv - 1).ToString () + "]")));
			new_Data.Add ("sup_num", Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "support.card." + n + ".support_num[" + (effort_Lv - 1).ToString () + "]")));
			result [n] = new_Data;
		}
		return result;
	}

	public int GetExpMax (int lv)
	{
		object[] exps = (object[])DataManager.inst.systemSimple ["exp_config"];
		if (lv > exps.Length)//满级
			return Convert.ToInt32 (exps [exps.Length-1]);
		return Convert.ToInt32 (exps [lv - 1]);
	}
	public bool IsMaxLv(){
		object[] exps = (object[])DataManager.inst.systemSimple ["exp_config"];

		return lv >= exps.Length;
	}
	//static
	public static string GetHeadUrl (string dc,bool isHD=false,bool isDianNao=false,bool isImg = false)
	{
		string url = "";
		if (dc == "h01" || dc == "h02" || dc == "h03" || dc == "h04" || dc == "h05" || dc == "h06")
		{
            url = isImg ? ( isHD ? "Icon:" + dc : "Icon:" + dc ) : Tools.GetResourceUrl(isHD ? "Icon:" + dc : "Icon:h01");

		}
        else if (dc == "icon_diannao")
        {
			url = isImg?("Image:" + dc):Tools.GetResourceUrl("Image:" + dc);
        }
        else
		{			
			string pfHeadImgURL = "";
//			if (!isHD) {
//				pfHeadImgURL = LocalStore.GetLocal (LocalStore.OTHER_HEADIMG);
//			}
//			url = PlatForm.inst.HTTP;
//			url = url.Replace ("gateway", "static/u") + dc + ".png";
//			if (pfHeadImgURL != "" && pfHeadImgURL.Length > 10) {
//				url = pfHeadImgURL;
//			} else {

			if (dc.IndexOf ("http") > -1) {
//				Debug.LogError (dc);
				url = dc;// + ".jpeg";
			}
			else{
                if (isDianNao)
                {

                    string dc_=Tools.UrlEncode(dc);
                    try
                    {
                        url = (string)(DataManager.inst.systemSimple["d_head_url"]) + dc_ + ".jpg";
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                   
                }
                else
                {
                    

                    url = (string)(DataManager.inst.systemSimple["head_url"]) +  dc + ".jpg";
//					Debug.LogError (url);

                }
            }
//			url = PlatForm.inst.PhotoHttp + dc + ".png";
		}
		return url;
	}


    public static string GetUname (object uid, object uname)
	{
		string msg = "";
		if (Tools.IsNullEmpty (uname))
			msg = uid.ToString ();
		else
			msg = uname.ToString ();
		return msg;
	}

	public int GetIconNumNow (string type)
	{
		switch (type)
		{
		case Config.ASSET_COIN:
			return coin;
		case Config.ASSET_GOLD:
			return gold;
		case Config.ASSET_EXP:
			return exp;
		case Config.ASSET_RANKSCORE:
			return rank_score;
		case Config.ASSET_ELSCORE:
			return el_score;
		case Config.ASSET_REDBAGCOIN:
			return (int)this.records ["redbag_coin"];
		}
		return 0;
	}

	public int GetOverEffortNum ()
	{
		int num = 0;
		Dictionary<string, object> effortCfg = (Dictionary<string, object>)(DataManager.inst.effort ["mission"]);
		List<object> lisData = new List<object> ();
		for (int i = 0; i < effort.Length; i++)
		{
			Dictionary<string, object> da = (Dictionary<string, object>)effort [i];
			Dictionary<string, object> dacfg = (Dictionary<string, object>)(effortCfg [da ["eid"].ToString ()]);
			Dictionary<string, object> ddd = new Dictionary<string, object> ();
			ddd.Add ("rate", (int)(da ["rate"]));
			ddd.Add ("need", (object[])dacfg ["need"]);
			ddd.Add ("status", (int)(da ["status"]));
			int need = (int)((object[])ddd ["need"]) [(((object[])ddd ["need"]).Length) - 1];
			if (((int)ddd ["status"] == 0) && lisData.Count < 3)
			{
				if (need <= (int)(da ["rate"]))
				{
					num++;
				}
			}
		}
		return num;
	}

	public string GetRankImg (object value, int offect = 0, bool isBig = false)
	{
		int rank = Convert.ToInt32 (value);
		Dictionary<string, object> matchCfg = DataManager.inst.match;
		Dictionary<string, object> rank_group = (Dictionary<string, object>)matchCfg ["rank_group_show"];
		string str = "";
		int zuida = 0;
		string j = "";
		foreach (string i in rank_group.Keys)
		{
			object[] obj = (object[])rank_group [i];
			object[] num = (object[])obj [0];
			if ((int)num [1] > zuida)
			{
				zuida = (int)num [1];
				j = i;
			}
			if ((int)num [0] <= rank && (int)num [1] >= rank)
			{
				if (Convert.ToInt32 (i) < 10)
				{
					str = "0" + (Convert.ToInt32 (i) + offect);
				}
				else
				{
					str = (Convert.ToInt32 (i) + offect) + "";
				}
			}
		}
		if (str == "")
			str = j;
		if (isBig)
			return Tools.GetResourceUrl ("Icon:L" + str);
		return Tools.GetResourceUrl ("Icon:r" + str);
	}

	public string[] GetNextRankExpAndName (object value)
	{
		int rank = Convert.ToInt32 (value);
		Dictionary<string, object> matchCfg = DataManager.inst.match;
		Dictionary<string, object> rank_group = (Dictionary<string, object>)matchCfg ["rank_group_show"];
		string str = "";
		int zuida = 10000000;
		//		string j = "";
		string name = "";
		string iinin = "";
		foreach (string i in rank_group.Keys)
		{
			object[] obj = (object[])rank_group [i];
			object[] num = (object[])obj [0];
			if ((int)num [0] > rank && (int)num [0] < zuida)
			{
				zuida = (int)num [0];
				name = obj [4].ToString ();
				iinin = i;
			}
		}
		str = (zuida - rank) + "";
		return new string[] { str, Tools.GetMessageById (name), iinin };
	}

	public long GetRedTime ()
	{
		long currTime = ReturnTickTime ();
		DateTime date = (DateTime)ModelManager.inst.userModel.records ["redbag_time"];
		DateTime nowDate = ModelManager.inst.gameModel.time;
		bool isToday = true;
		if (date.Year != nowDate.Year || date.Month != nowDate.Month || date.Day != nowDate.Day)
			isToday = false;

		if ((int)(ModelManager.inst.userModel.records ["redbag_times"]) >= ((object[])DataManager.inst.redbag ["redbag_pool"]).GetLength (0) && isToday)
		{
			return -1;
		}
		else
		{
			if (currTime > 0)
			{
				return currTime;
			}
			else
			{
				return -2;
			}
		}
	}
	public int GetSex{
		get{
			return  (this.sex == "m") ? 0 : 1;
		}
	}
	public int CurLvExp (int lv, int exp)
	{
		int num = 0;
		object[] exp_cfg = (object[])DataManager.inst.systemSimple ["exp_config"];
		for (int i = 0; i < exp_cfg.Length; i++)
		{
			if (lv > i + 1)
			{
				num += (int)exp_cfg [i];
			}
		}
		num += exp;
		return num;
	}

	public long ReturnTickTime ()
	{
		DateTime getRedTime = (DateTime)ModelManager.inst.userModel.records ["redbag_time"];
		long redTime = getRedTime.Ticks / 10000;
		int times = (int)ModelManager.inst.userModel.records ["redbag_times"];
		object[] cfg = (object[])DataManager.inst.redbag ["redbag_pool"];
		long nextcan = 0;
		if (times >= cfg.GetLength (0))
		{
			return -1;
		}
		else
		{
			nextcan = redTime + (int)((object[])(cfg [times])) [0] * 60 * 1000;
		}
		return (nextcan - Tools.GetSystemSecond () * 1000);
	}

	public bool GetUnlcok (string id, GObject g = null, bool isShowText = false, bool isCheckSocket = false)
	{
//		if (g == null) {
//			return false;
//		}
		if (isCheckSocket)
		{
			if (!NetSocket.inst.IsConnected ())
			{				 
				ViewManager.inst.ShowText (Tools.GetMessageById ("14004"));
				NetSocket.inst.ReConnect (0);
				return false;
			}
		}
		
		if (DataManager.inst.systemSimple == null)
			return false;
		Dictionary<string, object> cfg = (Dictionary<string, object>)DataManager.inst.systemSimple ["unlock"];
		string msg = "";
		bool b = true;
		bool v = true;
		if (cfg.ContainsKey (id))
		{
			object[] i = (object[])cfg [id];
			if (Convert.ToInt32 (i [2]) == 0)
			{
				b = false;
				v = true;
				msg = Tools.GetMessageById (i [3].ToString ());
			}
			else if (Convert.ToInt32 (i [1]) == 0)
			{
				b = false;
				v = false;
				msg = "";
			}
			else if (this.lv < Convert.ToInt32 (i [0]))
			{
				b = false;
				v = true;
				msg = Tools.GetMessageById ("14028", new string[] { i [0].ToString () });
			}
			if (i.Length == 5) {//卡牌使用，有第五位就有用，限制等级显示
				if (this.lv < Convert.ToInt32 (i [4])) {
					v = false;
					b = false;
				} else {
					
				}
			}
		}
		if (g != null)
		{
			g.visible = v;
			if (g is GButton||g is GTextField||g is GGroup)
			{
//				(g as GButton)._unlock = b;//lht change
				//(g as GButton).enabled = b;
				g.grayed = !b;
			}
        }
		if (isShowText && msg != "")
			ViewManager.inst.ShowText (msg);
		return b;
	}

	public List<Dictionary<string, object>> GetCurrCardGroup (int index = -1)
	{
		List<Dictionary<string, object>> list = new List<Dictionary<string, object>> ();
		int _index = index;
		if (index == -1)
		{
			_index = (int)records ["card_group_index"];
		}
		object[] obj = (object[])card_group [_index];
		for (int i = 0; i < obj.Length; i++)
		{
			Dictionary<string, object> da = new Dictionary<string, object> ();
			da.Add ("id", obj [i].ToString ());
			da.Add ("level", ((Dictionary<string, object>)card [obj [i].ToString ()]) ["lv"]);
			list.Add (da);
		}
		return list;
	}

	public void SetGuide (int index, Action<VoHttp> fun = null,Dictionary<string, object> sendData = null)
	{
		Dictionary<string, object> data = new Dictionary<string, object> ();
		data ["guide_num"] = index;
		NetHttp.inst.Send (NetBase.HTTP_DOGUIDE, (sendData == null) ? data : sendData, (VoHttp obj) => {
			GuideManager.inst.guide11 = index;
			GuideManager.inst.guide12 = 0;
			if (fun != null) {
				fun (obj);
			}
		});
	}

	public bool IsLove ()
	{
		string love = LocalStore.GetLocal (LocalStore.GUIDE_LOVE);
		if (love == "")
			return false;
		return true;
	}

	public void SetLove (bool love)
	{
		LocalStore.SetLocal (LocalStore.GUIDE_LOVE, "1");
	}

	public bool IsInvite ()
	{
		return Convert.ToBoolean (this.records ["invite_sign"]);
	}

	public bool IsChatVoice ()
	{
		return Convert.ToBoolean (this.records ["chat_voice_sign"]);
	}
	public void Socket_loginBack(VoSocket vo){
		if (vo.data is Boolean)
		{
			//					Debug.LogError("SOCKET_LOGIN false");
			NetSocket.inst.socketLogin = false;
		}
		else
		{
			//					Debug.LogError("SOCKET_LOGIN true");
			//				Log.debug ("Socket Login - " + vo.data.ToString ());
			NetSocket.inst.socketLogin = true;//Convert.ToBoolean (vo.data);

			Dictionary<string,object> re = (Dictionary<string,object>)vo.data;
			bool isBattle = Convert.ToBoolean (re ["in_battle"]);
			if (isBattle)
			{
				int typeId = (int)re ["type"];
				ModelFight fightModel = ModelManager.inst.fightModel;
				fightModel.isLeader = true;
				if (typeId == 1)
				{
					fightModel.fightType = ModelFight.FIGHT_MATCH;
				}
				else if (typeId == 2)
				{
					fightModel.fightType = ModelFight.FIGHT_MATCHTEAM;
				}
				else if (typeId == 3)
				{
					fightModel.fightType = ModelFight.FIGHT_FREEMATCH1;
				}
				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_ING));
			}
			else
			{

			}
		}
		Main.inst.Socket_Listen ();
	}
}