using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class ModelChat:BaseModel
{
	public bool isLoad = false;
	public object[] chats;
	public object[] guild;
	public object[] guild_Join;
	public object[] guild_Modify;
	public object[] reds;
	public object[] support;
	public object[] share;

	public object[] grab_List;
	public object[] card_List;
	public int grab_Type;

	public List<object> all;
	public string support_Id = "";

	//redcount
	public int redCount = -1;
	public List<List<int>> redPoint;

	//领取红包
	public bool isRedbagGift = false;

	public ModelChat ()
	{
		all = new List<object> ();
	}

	public override void Clear ()
	{
		isLoad = false;
		this.Clear_RedCount ();
		if (all != null)
			all.Clear ();
	}

	public List<object> GetAll ()
	{
		Dictionary<string,object> tem = new Dictionary<string, object> ();
		long time = Tools.GetSystemTicks ();
		long delay = 0;

		List<object> chat = new List<object> (chats);
		chat.ForEach ((object o) =>
		{
			Dictionary<string,object> oo = (Dictionary<string,object>)o;
			oo ["ctype"] = "chat";
		});

		List<object> g = new List<object> (guild);
		int job = ModelManager.inst.guildModel.my_guild_job;
		if (ModelManager.inst.guildModel.IsKick (job))
		{
			g.ForEach ((object o) =>
			{
				Dictionary<string,object> oo = (Dictionary<string,object>)o;
				oo ["ctype"] = "guild";
			});

			delay = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "society.society_player_time"));
			delay = delay * 60 * 1000 * 10000;
			for (int i = 0; i < g.Count; i++)
			{
				tem = (Dictionary<string,object>)g [i];
				if (time - ((DateTime)tem ["time"]).Ticks > delay)
				{
					g.RemoveAt (i);
					i--;
				}
			}
		}

		List<object> j = new List<object> (guild_Join);
		j.ForEach ((object o) =>
		{
			Dictionary<string,object> oo = (Dictionary<string,object>)o;
			oo ["ctype"] = "guild_join";
		});

		List<object> p = new List<object> (guild_Modify);
		p.ForEach ((object o) =>
		{
			Dictionary<string,object> oo = (Dictionary<string,object>)o;
			oo ["ctype"] = "guild_modify";
		});

		List<object> r = new List<object> (reds);
		r.ForEach ((object o) =>
		{
			Dictionary<string,object> oo = (Dictionary<string,object>)o;
			oo ["ctype"] = "send_guild_redbag";
		});			
		delay = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "society.redbag_cd"));
		delay = delay * 60 * 1000 * 10000;
		for (int i = 0; i < r.Count; i++)
		{
			tem = (Dictionary<string,object>)r [i];
			if (time - ((DateTime)tem ["time"]).Ticks > delay)
			{
				r.RemoveAt (i);
				i--;
			}
		}

		List<object> s = new List<object> (support);
		s.ForEach ((object o) =>
		{
			Dictionary<string,object> oo = (Dictionary<string,object>)o;
			oo ["ctype"] = "require_guild_support";
		});
		delay = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "support.daily"));
		delay = delay * 60 * 1000 * 10000;
		for (int i = 0; i < s.Count; i++)
		{
			tem = (Dictionary<string,object>)s [i];
			if (time - ((DateTime)tem ["time"]).Ticks > delay)
			{
				s.RemoveAt (i);
				i--;
			}
		}

		List<object> fs = new List<object> (share);
		fs.ForEach ((object o) =>
		{
			Dictionary<string,object> oo = (Dictionary<string,object>)o;
			oo ["ctype"] = "guild_fight_share";
		});

		all = new List<object> ();
		all.AddRange (chat);
		all.AddRange (g);
		all.AddRange (j);
		all.AddRange (p);
		all.AddRange (r);
		all.AddRange (s);
		all.AddRange (fs);

		Tools.Sort (all, new string[]{ "time:datetime:0" });

//		redCount = GetRedCountIndexAll ();
		return all;
	}

	public bool isSendRedBagOutTime (object data)
	{
		long time = Tools.GetSystemTicks ();
		long delay = 0;

		Dictionary<string,object> da = (Dictionary<string,object>)data;
		delay = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "society.redbag_cd"));
		delay = delay * 60 * 1000 * 10000;
		if (time - ((DateTime)da ["time"]).Ticks > delay)
		{
			return false;
		}
		return true;
	}

	public bool isSendSupportOutTime (object data)
	{
		long time = Tools.GetSystemTicks ();
		long delay = 0;

		Dictionary<string,object> da = (Dictionary<string,object>)data;
		delay = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "support.daily"));
		delay = delay * 60 * 1000 * 10000;

		if (time - ((DateTime)da ["time"]).Ticks > delay)
		{
			return false;
		}
		return true;
	}

	public List<object> CheckOutTimeData ()
	{
		Dictionary<string,object> o;
		long time = Tools.GetSystemTicks ();
		long delay1 = 0;
		long delay2 = 0;
		long delay3 = 0;
		delay1 = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "society.society_player_time"));
		delay1 = delay1 * 60 * 1000 * 10000;

		delay2 = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "society.redbag_cd"));
		delay2 = delay2 * 60 * 1000 * 10000;

		delay3 = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "support.daily"));
		delay3 = delay3 * 60 * 1000 * 10000;
		for (int i = 0; i < all.Count; i++)
		{
			o = (Dictionary<string,object>)all [i];
			if (o ["ctype"].ToString () == "guild")
			{				
				if (time - ((DateTime)o ["time"]).Ticks > delay1)
				{
					all.RemoveAt (i);
					i--;
				}
			}
			else if (o ["ctype"].ToString () == "send_guild_redbag")
			{		
				if (time - ((DateTime)o ["time"]).Ticks > delay2)
				{
					all.RemoveAt (i);
					i--;
				}
			}
			else if (o ["ctype"].ToString () == "require_guild_support")
			{		
				if (time - ((DateTime)o ["time"]).Ticks > delay3)
				{
					all.RemoveAt (i);
					i--;
				}
			}
		}
//		List<List<int>> l = GetRedCountIndexAll ();
//		redCount = l [0].Count + l [1].Count;
		return all;
	}

	public List<List<int>> GetRedCountIndexAll (int begin = 0, int end = 0)
	{
		redPoint = new List<List<int>> ();
		redPoint.Add (new List<int> ());
		redPoint.Add (new List<int> ());

		int job = ModelManager.inst.guildModel.my_guild_job;
		int ind = -1;
		int total = 0;
		Dictionary<string,object> m;
		for (int i = 0; i < all.Count; i++)
		{
			m = (Dictionary<string,object>)all [i];
			if (m ["ctype"].ToString () == "guild")
			{
				if (ModelManager.inst.guildModel.IsKick (job))
					ind = i;								
			}
			else if (m ["ctype"].ToString () == "send_guild_redbag")
			{
				if (m ["is_avable"] != null && Convert.ToBoolean (m ["is_avable"]) == true)
				{
					ind = i;
				}
			}
			else if (m ["ctype"].ToString () == "require_guild_support")
			{
				if (m ["uid"].ToString () == ModelManager.inst.userModel.uid || !IsSendCardOver (m) || !IsSendGiveCard () || !isSendSupportOutTime (m))
					continue;
				bool call = DataManager.inst.GetCardVo (m ["cid"].ToString ()).have;
				if (m ["data"] != null)
				{
					Dictionary<string,object> dd = (Dictionary<string,object>)m ["data"];
					bool ok = false;
					foreach (string userid in dd.Keys)
					{
						int used = Convert.ToInt32 (dd [userid]);
//						int call = GetCardSendCount (m ["cid"].ToString (), Convert.ToInt32 (m ["effort_lv"]));
						if (userid == ModelManager.inst.userModel.uid)
						{
							ok = true;
							break;						
						}				
					}
					if (!ok && call)
						ind = i;					
				}
				else
				{
					if (call)
						ind = i;
				}
			}
			if (ind != -1)
			{
				if (ind < begin)
					redPoint [0].Add (ind);
				else if (ind > end)
					redPoint [1].Add (ind);
				total++;
			}
			ind = -1;
		}
//		redCount = redPoint [0].Count + redPoint [1].Count;
		redCount = total;
		return redPoint;
	}

	public List<object> GetRedCards ()
	{
		List<object> ld = new List<object> ();
		object obj;
		for (int i = 0; i < grab_List.Length; i++)
		{
			obj = Tools.Analysis (grab_List [i], "card");
			if (obj != null)
				ld.Add (obj);
		}
		return ld;
	}

	//	public bool isSendRedBagZero ()
	//	{
	//		bool isOk = false;
	//		Dictionary<string,object> o;
	//		long time = Tools.GetSystemTicks ();
	//		long delay = 0;
	//		delay = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "society.redbag_cd"));
	//		delay = delay * 60 * 1000 * 10000;
	//		for (int i = 0; i < all.Count; i++)
	//		{
	//			o = (Dictionary<string,object>)all [i];
	//			if (o ["ctype"].ToString () == "send_guild_redbag")
	//			{
	//				if (time - ((DateTime)o ["time"]).Ticks > delay)
	//				{
	//					continue;
	//				}
	//			}
	//		}
	//		return isOk;
	//	}

	public int GetRedGolds ()
	{
		int gold = 0;
		for (int i = 0; i < grab_List.Length; i++)
		{
			gold += Convert.ToInt32 (Tools.Analysis (grab_List [i], "gold"));
		}
		return gold;
	}

	public int[] GetCardRarity ()
	{
		int[] ints = new int[4];
		Dictionary<string,object> dd;
		int ratity = 0;
		for (int i = 0; i < this.grab_List.Length; i++)
		{
			if (this.grab_List [i] != null)
			{
				dd = (Dictionary<string,object>)Tools.Analysis (this.grab_List [i], "card");
				if (dd != null)
				{
					foreach (string n in dd.Keys)
					{
						ratity = DataManager.inst.GetCardRarity (n);
						if (ratity != -1)
							ints [ratity] = ints [ratity] + Convert.ToInt32 (dd [n]);
					}
				}
			}
		}			
		return ints;
	}

	public int RemoveChat (string type, string uid)
	{
		int index = -1;
		if (all == null || all.Count == 0)
			return index;
		Dictionary<string,object> o;
		for (int i = 0; i < all.Count; i++)
		{
			o = (Dictionary<string,object>)all [i];
			if (o ["ctype"].ToString () == type)
			{
				if (o ["uid"].ToString () == uid)
				{
					index = i;
					all.RemoveAt (i);
					break;
				}
			}
		}
		return index;
	}

	public void UpdateRedBagData (Dictionary<string,object> data)
	{
		Dictionary<string,object> o;
		for (int i = 0; i < all.Count; i++)
		{
			o = (Dictionary<string,object>)all [i];
			if (o ["ctype"].ToString () == "send_guild_redbag" && o ["id"].ToString () == data ["id"].ToString ())
			{
				all [i] = data;
				break;
			}
		}
	}

	public void UpdateGuildSupport (Dictionary<string,object> data)
	{
		string uid = ModelManager.inst.userModel.uid;
		Dictionary<string,object> o;
//		List<int> index = new List<int> ();
		for (int i = 0; i < all.Count; i++)
		{
			o = (Dictionary<string,object>)all [i];
			if (o ["ctype"].ToString () == "require_guild_support")
			{
				if (o ["id"].ToString () == data ["id"].ToString ())
				{
					Dictionary<string,object> da = (Dictionary<string,object>)data ["data"];
					int count = 0;
					foreach (string n in da.Keys)
						count += Convert.ToInt32 (da [n]);
					int max = this.GetCardRequestCount (o ["cid"].ToString (), Convert.ToInt32 (o ["effort_lv"]));
					if (count >= max)
					{
						all.RemoveAt (i);
					}
					else
					{
						o ["data"] = data ["data"];
						all [i] = o;
					}
					break;
				}
//				index.Add (i);
			}
		}
//		return index;
	}

	//获得可以助战的卡牌 根据award_card_pool Lv 成就等级
	public List<string> GetRequestCards ()
	{
		List<string> list = new List<string> ();

		ModelUser userModel = ModelManager.inst.userModel;
		Dictionary<string,object> d = (Dictionary<string,object>)Tools.Analysis (DataManager.inst.systemSimple, "award_card_pool.Lv" + userModel.effort_lv);
		Dictionary<string,object> cards = (Dictionary<string,object>)Tools.Analysis (DataManager.inst.guild, "support.card");
//		List<string> num = new List<string> ();
//		foreach (string n in cards)
//			num.Add (n);

		Dictionary<string,object> da;
//		Dictionary<string,object> das;
		CardVo vo;
		foreach (string n in userModel.card.Keys)
		{
			vo = DataManager.inst.GetCardVo (n);
			if (!cards.ContainsKey (vo.rarity + ""))
				continue;
			da = (Dictionary<string,object>)d [vo.rarity + ""];
			if (da.ContainsKey (n))
				list.Add (n);
		}
		return list;
	}

	//请求卡牌张数
	public int GetCardRequestCount (string cid, int effort_Lv)
	{		
		Dictionary<string,object> card = (Dictionary<string,object>)DataManager.inst.card [cid];
		Dictionary<string,object> data = (Dictionary<string,object>)Tools.Analysis (DataManager.inst.guild, "support.card." + card ["rarity"].ToString ());
		return Convert.ToInt32 (Tools.Analysis (data, "num[" + (effort_Lv - 1) + "]") + "");
	}

	//可支援卡牌次数
	public int GetCardSendCount (string cid, int effort_Lv)
	{		
		Dictionary<string,object> card = (Dictionary<string,object>)DataManager.inst.card [cid];
		Dictionary<string,object> data = (Dictionary<string,object>)Tools.Analysis (DataManager.inst.guild, "support.card." + card ["rarity"].ToString ());
		return Convert.ToInt32 (Tools.Analysis (data, "support_num[" + (effort_Lv - 1) + "]") + "");
	}

	public long GetSendRequestCardTime ()
	{
		ModelUser userModel = ModelManager.inst.userModel;
		long delay = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "support.daily"));
		object time = Tools.Analysis (userModel.records, "guild_support.last_require_support");
		if (time == null)
			return 0;
		DateTime t = (DateTime)time;
		long tt = Tools.GetSystemTicks ();
		delay = delay * 60 * 1000 * 10000;
		long ts = tt - t.Ticks;
		if (ts > delay)
			return 0;
		return Math.Abs (ts - delay);
	}

	//红包cd
	public long GetSendRedBagTime ()
	{
		ModelUser userModel = ModelManager.inst.userModel;
		long delay = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "society.redbag_send_cd"));
		object time = Tools.Analysis (userModel.records, "guild_redbag_log");
		if (time == null)
			return 0;
		time = Tools.Analysis (time, "send_time");
		DateTime t = (DateTime)time;
		long tt = Tools.GetSystemTicks ();
		delay = delay * 60 * 1000 * 10000;
		long ts = tt - t.Ticks;
		if (ts > delay)
			return 0;
		return Math.Abs (ts - delay);
	}

	//红包是否领完
	public bool GetSendRedBagCount ()
	{
		ModelUser userModel = ModelManager.inst.userModel;
		long delay = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "society.redbag_send_cd"));
		object time = Tools.Analysis (userModel.records, "guild_redbag_log");
		if (time == null)
			return true;
		int min = (int)Tools.Analysis (time, "remain_num");
		if (min <= 0)
			return true;
		return false;
	}

	//支援次数今天用完
	public bool IsSendGiveCard ()
	{
		ModelUser userModel = ModelManager.inst.userModel;
		int use = Convert.ToInt32 (Tools.Analysis (userModel.records, "guild_support.used_coin"));
		int count = Convert.ToInt32 (Tools.Analysis (DataManager.inst.guild, "support.support_times[" + (userModel.effort_lv - 1) + "]"));
		if (use >= count)
			return false;
		return true;
	}

	//支援该卡用完
	public bool IsSendCardOver (object data)
	{
		Dictionary<string,object> da = (Dictionary<string,object>)data;
		if (da ["data"] != null)
		{
			int used = 0;
			int call = 0;
			Dictionary<string,object> dd = (Dictionary<string,object>)da ["data"];
			foreach (string userid in dd.Keys)
			{
				used = Convert.ToInt32 (dd [userid]);
				call = this.GetCardSendCount (da ["cid"].ToString (), Convert.ToInt32 (da ["effort_lv"]));
				if (userid == ModelManager.inst.userModel.uid && used >= call)
				{					
					return false;
				}
			}
		}
		return true;
	}

	public void IsEffect (object data, GComponent go)
	{
		Dictionary<string,object> da = (Dictionary<string,object>)data;
		if (da.ContainsKey ("effect") && Convert.ToBoolean (da ["effect"]))
		{	
			da ["effect"] = false;
			go.scale = new Vector2 (0.5f, 0.5f);
			go.TweenScale (new Vector2 (1f, 1f), 1f);
		}
//		go.x = go.width / 2;
	}

	public int GetChatRedCount ()
	{
		ModelUser userModel = ModelManager.inst.userModel;
		if (redCount == -1)
		{
			redCount = 0;
			redCount += userModel.Get_NoticeState (ModelUser.RED_REDBAGNUM);
			redCount += userModel.Get_NoticeState (ModelUser.RED_SUPPORTNUM);
			redCount += userModel.Get_NoticeState (ModelUser.RED_GUILDAPPLYNUM);
			redCount += userModel.Get_NoticeState (ModelUser.RED_GUILDJOIN);
			redCount += userModel.Get_NoticeState (ModelUser.RED_GUILDEXIT);
		}
		return redCount;
	}

	public void Add_RedCount ()
	{
		redCount++;
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_CHATUPDATE));
	}

	public void Remove_RedCount ()
	{
		redCount--;
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_CHATUPDATE));
	}

	public void Clear_RedCount ()
	{
		redCount = -1;
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_CHATUPDATE));
	}

}