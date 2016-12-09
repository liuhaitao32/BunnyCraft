using System;
using FairyGUI;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MediatorChat:BaseMediator
{
	private ModelChat chatModel;
	private ModelUser userModel;
	private ModelRole roleModel;

	private GList list;
	private GTextInput txt;
	private GButton btn_Send;
	private GButton btn_Red;
	private GButton btn_Req;
	private Dictionary<string,object> cfg;
	private List<object> ld;
	private GComponent gTop;
	private GComponent gBottom;
	private GGroup g1;
	private GGroup g2;
	private bool isRequestEffect = false;

	private long time0;
	private long time1;
	private long time2;
	Action<float>	rem = null;
	public MediatorChat ()
	{
		
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_CHAT);

		chatModel = ModelManager.inst.chatModel;
		userModel = ModelManager.inst.userModel;
		roleModel = ModelManager.inst.roleModel;
		cfg = (Dictionary<string,object>)DataManager.inst.guild ["chat"];

		list = this.GetChild ("n2").asList;
		txt = this.GetChild ("n11").asTextInput;
		btn_Send = this.GetChild ("n9").asButton;
		btn_Send.text = Tools.GetMessageById ("13064");
		btn_Red = this.GetChild ("n7").asButton;
		btn_Req = this.GetChild ("n8").asButton;
		btn_Req.text = Tools.GetMessageById ("22048");
		g1 = this.GetChild ("n21").asGroup;
		g2 = this.GetChild ("n22").asGroup;

		gTop = new GComponent ();
		this.GetChild ("n12").asGraph.ReplaceMe (gTop);
		gBottom = new GComponent ();
		this.GetChild ("n13").asGraph.ReplaceMe (gBottom);
		txt.promptText = Tools.GetMessageById ("22001");
		txt.maxLength = Convert.ToInt32 (cfg ["speak_length"]);
		if (!chatModel.isLoad)
		{
			NetHttp.inst.Send (NetBase.HTTP_CHATS, "", (VoHttp vo) =>
			{
				chatModel.isLoad = true;
				Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
				chatModel.chats = (object[])data ["chat_list"];
				chatModel.guild = (object[])data ["guild_apply_list"];
				chatModel.guild_Join = (object[])data ["guild_join_list"];
				chatModel.guild_Modify = (object[])data ["guild_modify_list"];
				chatModel.reds = (object[])data ["redbag_list"];
				chatModel.support = (object[])data ["support_list"];
				chatModel.share = (object[])data ["guild_fight_share"];

				ld = chatModel.GetAll ();
				list.itemRenderer = List_Render;
				list.SetVirtual();
				list.numItems = ld.Count;
				//
				btn_Send.onClick.Add (BtnSend_Click);
				btn_Red.onClick.Add (BtnRed_Click);
				btn_Req.onClick.Add (BtnReq_Click);
				this.AddListen ();
				list.scrollPane.onScroll.Add (Scorll_Change);
                list.scrollPane.ScrollBottom();
//				Scorll_Change ();
//				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_CHATUPDATE));
				rem = TimerManager.inst.Add(0.05f,1,(float t)=>{
					
					TimerManager.inst.Remove(rem);
					if (ld.Count != 0)
						list.ScrollToView (ld.Count - 1);
					rem = TimerManager.inst.Add(0.1f,1,(float f)=>{
						TimerManager.inst.Remove(rem);
//						Scorll_Change ();
//						DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_CHATUPDATE));
					});
				});
			});
		}
		else
		{
			ld = chatModel.CheckOutTimeData ();
			list.itemRenderer = List_Render;
			list.SetVirtual();
			list.numItems = ld.Count;
			btn_Send.onClick.Add (BtnSend_Click);
			btn_Red.onClick.Add (BtnRed_Click);
			btn_Req.onClick.Add (BtnReq_Click);
			this.AddListen ();
			list.scrollPane.onScroll.Add (Scorll_Change);
//			if (ld.Count != 0)
//				list.ScrollToView (ld.Count - 1, true);
//			Scorll_Change ();
//			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_CHATUPDATE));
			rem = TimerManager.inst.Add(0.05f,1,(float t)=>{

				TimerManager.inst.Remove(rem);
				if (ld.Count != 0)
					list.ScrollToView (ld.Count - 1);
				rem = TimerManager.inst.Add(0.1f,1,(float f)=>{
					TimerManager.inst.Remove(rem);
//					Scorll_Change ();
//					DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RED_CHATUPDATE));
				});
			});
		}			
		this.CheckRequestCard ();
		this.CheckSendRedbag ();
		this.AddGlobalListener (MainEvent.RED_CHATUPDATE, RED_CHATUPDATE);
		this.AddGlobalListener (MainEvent.CHAT_SENDREQUESTCARD, CHAT_SENDREQUESTCARD);
		this.AddGlobalListener (MainEvent.CHAT_SENDREDBAG, CHAT_SENDREDBAG);
		this.AddGlobalListener (MainEvent.CHAT_ISSENDREDBAG, CHAT_ISSENDREDBAG);
		this.AddGlobalListener (MainEvent.GONGGAO_CHANGE, GONGGAO_CHANGE);

		Dictionary<string,object> datas = (Dictionary<string,object>)(Tools.Clone (ModelManager.inst.guildModel.my_guild_info));
		this.GetChild ("n23").text = Tools.GetMessageById ("20115", new string[]{ (datas ["content"]).ToString () });
	}

	private void CheckRequestCard ()
	{
		time2 = chatModel.GetSendRequestCardTime ();
		btn_Req.grayed = time2 > 0 ? true : false;
		if (time2 > 0)
		{	
			btn_Req.grayed = true;
			g2.visible = true;
			this.GetChild ("n19").text = Tools.TimeFormat (time2, 0);
			TimerManager.inst.Add (1f, 0, Time_Tick2);
		}
		else
		{
			btn_Req.grayed = false;
			g2.visible = false;
			TimerManager.inst.Remove (Time_Tick2);
		}
	}

	private void CheckSendRedbag ()
	{
		bool isOk = chatModel.GetSendRedBagCount ();
//		Log.debug ("remain_num - check " + Tools.Analysis (userModel.records ["guild_redbag_log"], "remain_num").ToString ());
		if (isOk)
		{
			btn_Red.grayed = false;
			g1.visible = false;
			TimerManager.inst.Remove (Time_Tick1);
			return;			
		}
		time1 = chatModel.GetSendRedBagTime ();
		btn_Red.grayed = time1 > 0 ? true : false;
		if (time1 > 0)
		{	
			btn_Red.grayed = true;
			g1.visible = true;
			this.GetChild ("n16").text = Tools.TimeFormat (time1, 0);
			TimerManager.inst.Add (1f, 0, Time_Tick1);
		}
		else
		{
			btn_Red.grayed = false;
			g1.visible = false;
			TimerManager.inst.Remove (Time_Tick1);
		}
	}

	private void GONGGAO_CHANGE (MainEvent e)
	{
		Dictionary<string,object> datas = (Dictionary<string,object>)(Tools.Clone (ModelManager.inst.guildModel.my_guild_info));
		this.GetChild ("n23").text = Tools.GetMessageById ("20115", new string[]{ (datas ["content"]).ToString () });
	}

	private void CHAT_ISSENDREDBAG (MainEvent e)
	{
		this.CheckSendRedbag ();
	}

	private void Time_Tick1 (float t)
	{
		time1 -= 10000000;
		if (time1 <= 0)
		{
			TimerManager.inst.Remove (Time_Tick1);
			this.GetChild ("n16").text = Tools.TimeFormat (time1, 0);
			btn_Red.grayed = false;
			g1.visible = false;
		}
		else
		{
			this.GetChild ("n16").text = Tools.TimeFormat (time1, 0);
		}
	}

	private void Time_Tick2 (float t)
	{
		time2 -= 10000000;
		if (time2 < 0)
		{
			TimerManager.inst.Remove (Time_Tick2);
			this.GetChild ("n19").text = Tools.TimeFormat (time2, 0);
			btn_Req.grayed = false;
			g2.visible = false;
		}
		else
		{
			this.GetChild ("n19").text = Tools.TimeFormat (time2, 0);
		}
	}

	private void CHAT_SENDREQUESTCARD (MainEvent e)
	{
		this.CheckRequestCard ();
	}

	private void CHAT_SENDREDBAG (MainEvent e)
	{
		this.btn_Red.touchable = true;
		this.CheckSendRedbag ();
	}

	private void Scorll_Change ()
	{
//		Log.debug (list.scrollPane.posY.ToString () + "|" + list.scrollPane.contentHeight.ToString ());
//		try{
			float yy = list.scrollPane.posY;
			float hh = list.scrollPane.contentHeight;
			float listH = list.viewHeight;//391
			int begin = -1;// = (int)Math.Round (yy / hh);
			int end = 0;
			GObject o;
			float oh = 0;
			List<int> show = new List<int>();
			for (int i = 0; i < ld.Count; i++)
			{
				o = list.GetChild(i+"");//_isRed
				if(o!=null){
					show.Add(i);
				}
//				o = list.GetChildAt (i);
//				o = list.GetChildAt (i);
//				if (o == null)
//					break;
//				if (o.y < yy)
//					continue;
//				else
//				{
//					if (begin == -1)
//						begin = i;
//					oh = o.y - yy;
//				}
//				oh += o.height + list.lineGap;
//				if (oh >= listH)
//				{
//					end = i;
//					break;
//				}
			}
		begin = show.Count > 0 ? show [0] : -1;
		end = show.Count > 0 ? show [show.Count-1] : 0;
		chatModel.GetRedCountIndexAll (begin, end);
			RED_CHATUPDATE (null);
//		}catch(Exception exc){
//			
//		}
	}

	private void RED_CHATUPDATE (MainEvent e)
	{
		///////////////////////////////////////////////////////公会
		int top = chatModel.redPoint [0].Count;
		int bottom = chatModel.redPoint [1].Count;
		if (top > 0)
		{
			GComponent com = userModel.Add_Notice (this.gTop, new Vector2 (-10, 0), top, false, true);
			com.GetController ("c1").selectedIndex = 1;
			com.onClick.Add (() =>
			{
				List<int> red = chatModel.redPoint [0];
				if (red.Count != 0)
				{
					int index = red [red.Count - 1];
					list.ScrollToView (index, true);
				}
			});
		}
		else
			userModel.Remove_Notice (this.gTop);

		if (bottom > 0)
		{
			GComponent com = userModel.Add_Notice (this.gBottom, new Vector2 (-10, 0), bottom, false, true);
			com.onClick.Add (() =>
			{
				List<int> red = chatModel.redPoint [1];
				if (red.Count != 0)
				{
					int index = red [0];
					list.ScrollToView (index, true);
				}
			});
		}
		else
			userModel.Remove_Notice (this.gBottom);
	}

	public override void Clear ()
	{
		base.Clear ();
		TimerManager.inst.Remove (Time_Tick1);
		TimerManager.inst.Remove (Time_Tick2);
		this.RemoveGlobalListener (MainEvent.CHAT_SEND, CHAT_SEND);
		this.RemoveGlobalListener (MainEvent.CHAT_APPLEJOINGUILD, CHAT_APPLEJOINGUILD);
		this.RemoveGlobalListener (MainEvent.CHAT_GUILDJOIN, CHAT_GUILDJOIN);
		this.RemoveGlobalListener (MainEvent.CHAT_GUILDMODIFY, CHAT_GUILDMODIFY);
		this.RemoveGlobalListener (MainEvent.CHAT_SENDGUILDREDBAG, CHAT_SENDGUILDREDBAG);
		this.RemoveGlobalListener (MainEvent.CHAT_REQUIREGUILDSUPPORT, CHAT_REQUIREGUILDSUPPORT);
		this.RemoveGlobalListener (MainEvent.CHAT_SENDGUILDSUPPORT, CHAT_SENDGUILDSUPPORT);
		this.RemoveGlobalListener (MainEvent.RED_CHATUPDATE, RED_CHATUPDATE);
	}

	private void CHAT_SEND (MainEvent e)
	{
		list.numItems = ld.Count;
		if (ld.Count != 0)
			list.ScrollToView (ld.Count - 1, true);
	}

	private void CHAT_APPLEJOINGUILD (MainEvent e)
	{
		list.numItems = ld.Count;
		if (ld.Count != 0)
			list.ScrollToView (ld.Count - 1, true);
	}

	private void CHAT_GUILDJOIN (MainEvent e)
	{
		list.numItems = ld.Count;
		if (ld.Count != 0)
			list.ScrollToView (ld.Count - 1, true);
	}

	private void CHAT_GUILDMODIFY (MainEvent e)
	{
		list.numItems = ld.Count;
		if (ld.Count != 0)
			list.ScrollToView (ld.Count - 1, true);
	}

	private void CHAT_SENDGUILDREDBAG (MainEvent e)
	{
		list.numItems = ld.Count;
		if (ld.Count != 0)
			list.ScrollToView (ld.Count - 1, true);
	}

	private void CHAT_REQUIREGUILDSUPPORT (MainEvent e)
	{
		list.numItems = ld.Count;
		if (ld.Count != 0)
			list.ScrollToView (ld.Count - 1, true);
	}

	private void CHAT_SENDGUILDSUPPORT (MainEvent e)
	{
//		List<int> index = (List<int>)e.data;
//		for (int i = 0; i < index.Count; i++)
//		{
//			GObject obj = list.GetChildAt ((int)index [i]);
//			if (obj != null)
//			{
//				List_Render ((int)index [i], obj);
//			}		
//		}

		float index = list.scrollPane.posY;
		list.numItems = ld.Count;
		list.scrollPane.posY = index;
//		if (ld.Count != 0)
//			list.ScrollToView (ld.Count - 1, true);
	}

	private void CHAT_FIGHTSHARE (MainEvent e)
	{
		list.numItems = ld.Count;
		if (ld.Count != 0)
			list.ScrollToView (ld.Count - 1, true);
	}

	private void AddListen ()
	{		
		this.AddGlobalListener (MainEvent.CHAT_SEND, CHAT_SEND);			
		this.AddGlobalListener (MainEvent.CHAT_APPLEJOINGUILD, CHAT_APPLEJOINGUILD);
		this.AddGlobalListener (MainEvent.CHAT_GUILDJOIN, CHAT_GUILDJOIN);
		this.AddGlobalListener (MainEvent.CHAT_GUILDMODIFY, CHAT_GUILDMODIFY);
		this.AddGlobalListener (MainEvent.CHAT_SENDGUILDREDBAG, CHAT_SENDGUILDREDBAG);
		this.AddGlobalListener (MainEvent.CHAT_REQUIREGUILDSUPPORT, CHAT_REQUIREGUILDSUPPORT);
		this.AddGlobalListener (MainEvent.CHAT_SENDGUILDSUPPORT, CHAT_SENDGUILDSUPPORT);
		this.AddGlobalListener (MainEvent.CHAT_FIGHTSHARE, CHAT_FIGHTSHARE);
	}

	private void List_Render (int index, GObject item)
	{
//		Debug.LogError("Render Index:" + item.parent.width);
		GComponent go = item.asCom;
		go.name = index + "";
		go.RemoveChildren ();
//        go.x = GRoot.inst.width/9;
//		go.x = item.parent.width - go.width;
        Dictionary<string,object> data = (Dictionary<string,object>)ld [index];
		GComponent it;
		GButton head;
		GTextField text;
		GButton btn1;
		GButton btn2;
		ComProgressBar bar;
		if (data ["ctype"].ToString () == "chat")
		{						
			GImage bg;
			it = Tools.GetComponent (Config.ITEM_CHAT).asCom;
			Controller c1 = it.GetController ("c1");
			if (data ["uid"].ToString () == userModel.uid)
			{
				c1.selectedIndex = 1;
				it = it.GetChild ("n1").asCom;
			}
			else
			{
				c1.selectedIndex = 0;			
				it = it.GetChild ("n0").asCom;
			}
			head = it.GetChild ("n0").asButton;
			text = it.GetChild ("n2").asTextField;
			bg = it.GetChild ("n5").asImage;
           

			it.GetChild ("n1").text = ModelUser.GetUname (data ["uid"], data ["uname"]);
			Dictionary<string,object> use = data ["head"] as Dictionary<string,object>;
			Tools.SetLoaderButtonUrl (head, ModelUser.GetHeadUrl (use ["use"].ToString ()));
			text.text = data ["chat"].ToString ();
			it.GetChild ("n4").text = ((DateTime)data ["time"]).ToString ("HH:mm:ss");
			if (text.textHeight > 29)
			{
				float c = text.textHeight - 29;
				bg.height = bg.height + c;
				go.height = (115 - 29) + text.textHeight;
				it.GetChild ("n4").y = it.GetChild ("n4").y + c;
			}
			else
			{
				go.height = 115;
				it.GetChild ("n4").y = 90;
			}
			head.onClick.Add (() =>
			{
				this.DispatchGlobalEvent (new MainEvent (MainEvent.SHOW_USER, new object[] {
					null,
					data ["uid"].ToString (),
					roleModel.tab_CurSelect1,
					roleModel.tab_CurSelect2,
					roleModel.tab_CurSelect3
				}));
			});						
			go.AddChild (it);
			chatModel.IsEffect (data, go);
		}
		else if (data ["ctype"].ToString () == "guild")
		{
			GTextField rank;
			it = Tools.GetComponent (Config.ITEM_GUILD).asCom;
			head = it.GetChild ("n3").asButton;
			rank = it.GetChild ("n6").asTextField;
			btn1 = it.GetChild ("n0").asButton;
			btn2 = it.GetChild ("n7").asButton;

			it.GetChild ("n4").text = ModelUser.GetUname (data ["uid"], data ["uname"]);
			it.GetChild ("n2").text = Tools.GetMessageById ("22014");
			Dictionary<string,object> use = data ["head"] as Dictionary<string,object>;
			head.GetChild ("n2").text = data ["lv"].ToString ();
			Tools.SetLoaderButtonUrl (head.GetChild ("n0").asButton, ModelUser.GetHeadUrl (use ["use"].ToString ()));
			it.GetChild ("n9").text = ((DateTime)data ["time"]).ToString ("HH:mm:ss");
			rank.text = Tools.GetMessageById ("22002", new string[]{ data ["rank_score"].ToString () });
			go.height = 280;
			go.AddChild (it);
			chatModel.IsEffect (data, go);
			btn1.onClick.Add (() =>
			{	
				Dictionary<string,object> d1 = new Dictionary<string, object> ();
				d1 ["uid"] = data ["uid"].ToString ();
				d1 ["flag"] = 1;
				NetHttp.inst.Send (NetBase.HTTP_GUILD_JOIN, d1, (VoHttp vo) =>
				{
//					Log.debug (vo.data.ToString ());
					if (!Convert.ToBoolean (vo.data))
					{						
						chatModel.RemoveChat ("guild", data ["uid"].ToString ());
						ViewManager.inst.ShowText (Tools.GetMessageById ("22029"));
						list.RemoveChildAt (index);
					}
				});
			});
			btn2.onClick.Add (() =>
			{	
				Dictionary<string,object> d1 = new Dictionary<string, object> ();
				d1 ["uid"] = data ["uid"].ToString ();
				d1 ["flag"] = 2;
				NetHttp.inst.Send (NetBase.HTTP_GUILD_JOIN, d1, null);
			});
		}
		else if (data ["ctype"].ToString () == "guild_join")
		{
			it = Tools.GetComponent (Config.ITEM_TEXT).asCom;
			text = it.GetChild ("n0").asTextField;
			Dictionary<string,object> user1 = (Dictionary<string,object>)data ["user1"];
			Dictionary<string,object> user2 = (Dictionary<string,object>)data ["user2"];
			string name2 = ModelUser.GetUname (user2 ["uid"], user2 ["uname"]);
			if (user1 == null)
			{
				text.text = Tools.GetMessageById ("22028", new string[]{ name2 });
			}
			else
			{
				string name1 = ModelUser.GetUname (user1 ["uid"], user1 ["uname"]);
				if (data ["flag"].ToString () == "1")
					text.text = Tools.GetMessageById ("22004", new string[]{ name1, name2 });
				else
					text.text = Tools.GetMessageById ("22005", new string[]{ name1, name2 });
			}
			go.height = 70f;
//			try{
				go.AddChild (it);
//			}catch(Exception ee){
//			}
			chatModel.IsEffect (data, go);
		}
		else if (data ["ctype"].ToString () == "guild_modify")
		{
			it = Tools.GetComponent (Config.ITEM_TEXT).asCom;
			text = it.GetChild ("n0").asTextField;
			if (data.ContainsKey ("user"))
			{				
				Dictionary<string,object> user = (Dictionary<string,object>)data ["user"];
				string name0 = ModelUser.GetUname (user ["uid"], user ["uname"]);
				text.text = Tools.GetMessageById ("22030", new string[]{ name0 });
			}
			else
			{
				Dictionary<string,object> user1 = (Dictionary<string,object>)data ["user1"];
				Dictionary<string,object> user2 = (Dictionary<string,object>)data ["user2"];
				string name1 = ModelUser.GetUname (user1 ["uid"], user1 ["uname"]);
				string name2 = ModelUser.GetUname (user2 ["uid"], user2 ["uname"]);
				int num = Convert.ToInt32 (data ["num"]);
				int old_num = Convert.ToInt32 (data ["old_num"]);
				if (num == -1)
					text.text = Tools.GetMessageById ("22008", new string[]{ name1, name2 });
				else if (num == 0)
					text.text = Tools.GetMessageById ("22009", new string[]{ name1, name2 });
				else if (num < old_num)
					text.text = Tools.GetMessageById ("22006", new string[]{ name1, name2 });
				else
					text.text = Tools.GetMessageById ("22007", new string[]{ name1, name2 });
			}
			go.height = 70;
			go.AddChild (it);
			chatModel.IsEffect (data, go);
		}
		else if (data ["ctype"].ToString () == "send_guild_redbag")
		{
//			go.name = go.name+"_isRed";
			it = Tools.GetComponent (Config.ITEM_RED1).asCom;
			GLoader load = it.GetChild ("n6").asLoader;
			load.url = data ["type"].ToString () == "0" ? Tools.GetResourceUrl ("Image2:n_icon_bthb2") : Tools.GetResourceUrl ("Image2:n_icon_bthb1");
//			it.GetChild ("n1").text = ModelUser.GetUname (data ["uid"], data ["uname"]);
			string ti = Tools.GetMessageById ("22017", new string[]{ ModelUser.GetUname (data ["uid"], data ["uname"]) });
			ti = Tools.GetMessageColor (ti, new string[]{ "dc8100" });
			it.GetChild ("n2").text = ti;
			it.GetChild ("n4").text = ((DateTime)data ["time"]).ToString ("HH:mm:ss");
			it.GetChild ("n11").text = Tools.GetMessageById ("22011");
			it.GetChild ("n12").visible = true;
			if (Convert.ToBoolean (data ["is_my_grab"]))
				it.GetChild ("n12").text = Tools.GetMessageById ("22034");
			else if (Convert.ToBoolean (data ["is_grab_all"]))
				it.GetChild ("n12").text = Tools.GetMessageById ("22046");
			else
				it.GetChild ("n12").visible = false;
			load.onClick.Add (() =>
			{
				if (!chatModel.isSendRedBagOutTime (data))
				{
					ViewManager.inst.ShowText (Tools.GetMessageById ("22039"));
					return;
				}
				Dictionary<string,object> d = new Dictionary<string, object> ();
				d ["redbag_id"] = data ["id"].ToString ();
				NetHttp.inst.Send (NetBase.HTTP_GRABGUILDREDBAG, d, (VoHttp vo) =>
				{
					Dictionary<string,object> da = (Dictionary<string,object>)vo.data;
					Dictionary<string,object> gift = (Dictionary<string,object>)da ["gift"];
					chatModel.grab_List = (object[])da ["grab_list"];
					chatModel.card_List = (object[])da ["card_list"];
					chatModel.grab_Type = Convert.ToInt32 (data ["type"]);
					if (gift == null)
					{					
						if (!Convert.ToBoolean (data ["is_my_grab"]))
						{
							data ["is_avable"] = false;
							data ["is_my_grab"] = true;
							chatModel.UpdateRedBagData (data);						
							chatModel.Remove_RedCount ();
							this.List_Render (index, item);
						}
						ViewManager.inst.ShowView<MediatorRed2> ();
					}
					else
					{
						data ["is_avable"] = false;
						data ["is_my_grab"] = true;
						chatModel.UpdateRedBagData (data);						
						chatModel.Remove_RedCount ();
						this.List_Render (index, item);
						chatModel.isRedbagGift = true;
                        //ViewManager.inst.ShowIcon (gift, () =>
                        //{
                        int gold = userModel.gold;
						userModel.UpdateData (vo.data);
                        int gold2 = gift.ContainsKey("gold") ? Convert.ToInt32(gift["gold"]) : 0;
                        if(gold+gold2!=userModel.gold) {
                            ViewManager.inst.ShowText(Tools.GetMessageById("33212"));
                        }
						userModel.records ["guild_redbag_log"] = da ["guild_redbag_log"];
						ViewManager.inst.ShowView<MediatorRed2> ();
//						});
//						ViewManager.inst.ShowReward (gift);
					}
				});				                 
			});
			go.height = 280;
			go.AddChild (it);
			chatModel.IsEffect (data, go);
		}
		else if (data ["ctype"].ToString () == "require_guild_support")
		{
//			go.name = go.name+"_isRed";
			it = Tools.GetComponent (Config.ITEM_REQUEST).asCom;
			text = it.GetChild ("n2").asTextField;
			bar = it.GetChild ("n9") as ComProgressBar;
			bar.SetTextSize (28);
			bar.offsetY = 8;
			ComCard ci = it.GetChild ("n8") as ComCard;
			btn1 = it.GetChild ("n5").asButton;
			btn1.text = Tools.GetMessageById ("22049");
			bar.skin = ComProgressBar.BAR7;

			CardVo vo = DataManager.inst.GetCardVo (data ["cid"].ToString ());
			text.text = Tools.GetMessageById ("22019");
			it.GetChild ("n16").text = Tools.GetMessageById ("22012");
			it.GetChild ("n10").text = Tools.GetMessageById ("22033");
			it.GetChild ("n18").text = Tools.GetMessageColor ("[0]" + vo.exp + "[/0]/" + vo.maxExp, new string[]{ "a6fb30" });
			it.GetChild ("n1").text = ModelUser.GetUname (data ["uid"], data ["uname"]);
			it.GetChild ("n4").text = ((DateTime)data ["time"]).ToString ("HH:mm:ss");
			Dictionary<string,object> da = (Dictionary<string,object>)data ["data"];
			int count = 0;
			foreach (string n in da.Keys)
				count += Convert.ToInt32 (da [n]);
			int max = chatModel.GetCardRequestCount (data ["cid"].ToString (), Convert.ToInt32 (data ["effort_lv"]));
			bar.value = count;
			bar.max = max;
			it.GetChild ("n20").asTextField.text = bar.text;
			ci.SetData (vo.id, 1, 1);
			ci.SetText (Tools.GetMessageById (vo.name));
			if (data ["uid"].ToString () == userModel.uid || vo.exp == 0 || !chatModel.IsSendGiveCard ()
			    || !chatModel.IsSendCardOver (data) || !chatModel.isSendSupportOutTime (data) || count >= max)
			{
				go.name = go.name + "_isExc";
				btn1.grayed = true;
			}
			btn1.onClick.Add (() =>
			{				
				if (isRequestEffect)
					return;
				if (count >= max)
				{
					ViewManager.inst.ShowText (Tools.GetMessageById ("22038"));
					return;
				}
				else if (data ["uid"].ToString () == userModel.uid)
				{
					ViewManager.inst.ShowText (Tools.GetMessageById ("22035"));
					return;
				}
				else if (vo.exp == 0)
				{
					ViewManager.inst.ShowText (Tools.GetMessageById ("22036"));
					return;
				}
				else if (!chatModel.IsSendGiveCard ())
				{
					ViewManager.inst.ShowText (Tools.GetMessageById ("22022"));
					return;
				}
				else if (!chatModel.IsSendCardOver (data))
				{
					ViewManager.inst.ShowText (Tools.GetMessageById ("22024"));
					return;
				}
				else if (!chatModel.isSendSupportOutTime (data))
				{
					ViewManager.inst.ShowText (Tools.GetMessageById ("22037"));
					return;
				}
				Dictionary<string,object> d = new Dictionary<string, object> ();
				d ["support_id"] = data ["id"].ToString ();
				NetHttp.inst.Send (NetBase.HTPP_SENDGUILDSUPPORT, d, (VoHttp v) =>
				{					
					chatModel.Remove_RedCount ();
					isRequestEffect = true;
					ViewManager.inst.ShowIcon (userModel.GetReward (v.data), () =>
					{						
						userModel.UpdateData (v.data);
						chatModel.UpdateGuildSupport (data);
						DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHAT_SENDGUILDSUPPORT));
						isRequestEffect = false;
					});
				});
			});
			go.height = 300;
			go.AddChild (it);
			chatModel.IsEffect (data, go);
		}
		else if (data ["ctype"].ToString () == "guild_fight_share")
		{
			it = Tools.GetComponent (Config.ITEM_SHARE).asCom;
			object[] fight = (object[])data ["fight_data"];
//			it.GetChild ("n4").text = ModelUser.GetUname (data ["uid"], data ["uname"]);
			string na = "[0]" + ModelUser.GetUname (data ["uid"], data ["uname"]) + "[/0]";
			na = Tools.GetMessageColor (na, new string[]{ "dc8100" });
			if (fight [0].ToString () == "1")
				it.GetChild ("n2").text = na + Tools.GetMessageById ("22042");
			else
				it.GetChild ("n2").text = na + Tools.GetMessageById ("22043");
			it.GetChild ("n6").text = data ["chat"].ToString ();
			it.GetChild ("n9").text = ((DateTime)data ["time"]).ToString ("HH:mm:ss");
			it.GetChild ("n0").asButton.onClick.Add (() =>
			{
				this.DispatchGlobalEventWith (MainEvent.SHOW_FIGHTDETAIL, data);
			});
			go.height = 255;
			go.AddChild (it);
			chatModel.IsEffect (data, go);
		}
//		go.Center ();
	}

	private void BtnSend_Click ()
	{
		txt.text = Tools.StrReplace (txt.text);
		if (txt.text != "")
		{	
			txt.text=txt.text.Trim();
			long t = Tools.GetSystemSecond ();
			if (t - time0 < Convert.ToInt32 (cfg ["speak_cd"]))
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("22003"));
				return;
			}

			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["chat_string"] = FilterManager.inst.Exec (txt.text);

			NetHttp.inst.Send (NetBase.HTTP_CHAT, data, (VoHttp vo) =>
			{				
				time0 = t;
//				Log.debug ("发送 " + vo.data.ToString ());
				txt.text = "";
			});
		}
    }

    private void BtnRed_Click ()
	{
		if (chatModel.GetSendRedBagTime () > 0 && !chatModel.GetSendRedBagCount ())
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("22041"));
			return;
		}
		btn_Red.touchable = false;
		ViewManager.inst.ShowView<MediatorRed1> ();
	}

	private void BtnReq_Click ()
	{
		if (chatModel.GetSendRequestCardTime () > 0)
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("22020"));
			return;
		}
		ViewManager.inst.ShowView<MediatorRequest> ();
	}

}