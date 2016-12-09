using System;
using FairyGUI;
using System.Collections.Generic;
using UnityEngine;

public class MediatorTeamMatch:BaseMediator
{
	private ModelFight fightModel;
	private ModelRole roleModel;
	private ModelUser userModel;

	private GList list;
	private GButton btn_Enter;
	private GButton btn_Invite;
	private GButton btn_Cancel;
	private GTextField btn_Quit;
	private GTextField text1;
	private GTextField text2;
	private GTextField text3;
	private Controller c1;
	private GButton btn_Chat;
	private GButton btn_Mil;
	private BaseMediator micro;
	private ComProgressBar bar;

	private int time;
	private Micro mi;

	public MediatorTeamMatch ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.isAutoClose = false;
		this.Create (Config.VIEW_TEAMMATCH, false);

//		PlatForm.inst.timeout = 20000f;

		fightModel = ModelManager.inst.fightModel;
		roleModel = ModelManager.inst.roleModel;
		userModel = ModelManager.inst.userModel;

		fightModel.fightType = ModelFight.FIGHT_MATCHTEAM;

		list = this.GetChild ("n2").asList;
		btn_Enter = this.GetChild ("n3").asButton;
        btn_Enter.text = Tools.GetMessageById("25062");
		btn_Invite = this.GetChild ("n4").asButton;
		c1 = this.GetController ("c1");
		text1 = this.GetChild ("n7").asTextField;
		text2 = this.GetChild ("n8").asTextField;

		text3 = this.GetChild ("n9").asTextField;
		//
		btn_Cancel = this.GetChild ("n6").asButton;
        btn_Cancel.text = Tools.GetMessageById("14025");
		btn_Quit = this.GetChild ("n29").asTextField;
		btn_Chat = this.GetChild ("n18").asButton;
        //btn_Chat.text = Tools.GetMessageById("25067");
		btn_Mil = this.GetChild ("n19").asButton;
        btn_Mil.GetChild("n2").asTextField.text = Tools.GetMessageById("25067");
		bar = this.GetChild ("n20").asCom as ComProgressBar;
		bar.skin = ComProgressBar.BAR10;
		bar.SetTextVisible (false);
		bar.value = 0;
		bar.max = 100;

		btn_Close = this.GetChild ("close").asButton;
		btn_Close.onClick.Add (this.Close);
		//
		this.GetChild("title").asTextField.text = Tools.GetMessageById ("25033");
		//
		btn_Enter.onClick.Add (Enter_Click);
		btn_Invite.onClick.Add (Invite_Click);
		btn_Cancel.onClick.Add (Cancel_Click);
//		btn_Quit.onClick.Add (Quit_Click);
		btn_Quit.text = Tools.GetMessageById ("25058");
		btn_Chat.onClick.Add (Chat_Click);
		btn_Mil.onClick.Add (Chat_Click_Over);
        fightModel.isMatch = false;

		//麦克风功能
		micro = new MediatorMicro ();
		ViewManager.inst.AddTopView (micro);
		//房间内部操作
		NetSocket.inst.AddListener (NetBase.SOCKET_TEAMMATCH, (VoSocket vo) =>
		{
//				Debug.LogError("NetSocket.inst.AddListener");
			if (vo.data is Boolean)
			{
				bool isOk = Convert.ToBoolean (vo.data);
				if (!isOk)
				{
					ViewManager.inst.ShowText (Tools.GetMessageById ("25026"));
					TimerManager.inst.Remove (Time_Tick);
					fightModel.isMatch = false;
					list.numItems = 4;
					if (fightModel.isLeader)
						c1.selectedIndex = 0;
					else
						c1.selectedIndex = 1;					
				}
			}
			else
			{
				Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
				fightModel.team1 = (object[])data ["team"];
				list.numItems = 4;//fightModel.team1.Length;

				c1.selectedIndex = 4;
				this.closeEnable = false;
				text3.text = Tools.GetMessageById("25064");
				TimerManager.inst.Add (5f, 1, (float t) =>
				{				
					TimerManager.inst.Remove (Time_Tick);
					c1.selectedIndex = 4;
                    //					Log.debug ("准备开始 - " + vo.data.ToString ());

                    DispatchManager.inst.Dispatch(new MainEvent(MainEvent.START_FIGHT, data));
                });
			}
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_STARTMATCHPUSH, (VoSocket vo) =>
		{			
			fightModel.isMatch = true;
			list.numItems = 4;//fightModel.team1.Length;

//			NetSocket.inst.RemoveListener (NetBase.SOCKET_STARTMATCH);
			if (fightModel.isLeader)
				c1.selectedIndex = 2;
			else
				c1.selectedIndex = 3;

			fightModel.preTime = Convert.ToInt32 (vo.data);
			fightModel.matchTime = 0;
//			Log.debug ("开始匹配 - " + fightModel.preTime.ToString ());
			if (fightModel.isLeader)
				text1.text = Tools.GetMessageById ("25005", new string[] {
					fightModel.preTime.ToString (),
					fightModel.matchTime.ToString ()
				});
			else
				text2.text = Tools.GetMessageById ("25005", new string[] {
					fightModel.preTime.ToString (),
					fightModel.matchTime.ToString ()
				});
			TimerManager.inst.Add (1f, 0, Time_Tick);
//			ViewManager.inst.ShowText (Tools.GetMessageById ("25013"));
		});			

		NetSocket.inst.AddListener (NetBase.SOCKET_ACCEPTINVITEOTHER, (VoSocket vo) =>
		{			
			string uid = fightModel.AddTeam (vo.data);
			list.numItems = 4;//fightModel.team1.Length;
			ViewManager.inst.ShowText (Tools.GetMessageById ("25012", new String[]{ uid }));
		});
			
		NetSocket.inst.AddListener (NetBase.SOCKET_KILLTEAMPUSH, (VoSocket vo) =>
		{	
			fightModel.Clear ();
			ViewManager.inst.CloseView (this);
			ViewManager.inst.ShowText (Tools.GetMessageById ("25008"));
//			Log.debug ("解散队伍 - " + vo.data.ToString ());
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_QUITTEAMPUSH, (VoSocket vo) =>
		{
//			if (vo.data is Boolean && !Convert.ToBoolean (vo.data))
//			{
//				ViewManager.inst.ShowText (Tools.GetMessageById ("25016"));
//				return;
//			}
			string uid = vo.data.ToString ();
			if (userModel.uid == uid)
			{
				ViewManager.inst.CloseView (this);
				ViewManager.inst.ShowText (Tools.GetMessageById ("25011"));
//				Log.debug ("退出队伍 - " + vo.data.ToString ());
			}
			else
			{				
				fightModel.ChangeTeam (vo.data);
				list.numItems = 4;
				if (fightModel.isLeader)
					c1.selectedIndex = 0;
				else
					c1.selectedIndex = 1;
				this.btn_Enter.enabled = false;
				TimerManager.inst.Add (2f, 1, (float t) =>
				{
					this.btn_Enter.enabled = true;	
				});
			}
		});
			
		NetSocket.inst.AddListener (NetBase.SOCKET_CANCELMATCHPUSH, (VoSocket vo) =>
		{
			if (!Convert.ToBoolean (vo.data))
				return;
			fightModel.isMatch = false;
			if (fightModel.isLeader)
				c1.selectedIndex = 0;
			else
				c1.selectedIndex = 1;
			TimerManager.inst.Remove (Time_Tick);
			ViewManager.inst.ShowText (Tools.GetMessageById ("25015"));
		});
			
		NetSocket.inst.AddListener (NetBase.SOCKET_MATCHTEAMCHATPUSH, (VoSocket vo) =>
		{
			if (userModel.IsChatVoice ())
				fightModel.AddMicro (vo.data);
		});

		this.AddGlobalListener (MainEvent.SOCKET_CLOSE, (MainEvent e) =>
		{
			this.Close ();
		});
		//
		this.InitData();
	}
	public void InitData(){
		if (fightModel.isRequest) {
			c1.selectedIndex = 0;
		} else {
			c1.selectedIndex = 1;
		}
		list.itemRenderer = ItemRender;
		list.numItems = 4;
	}
	public override void Clear ()
	{
//		Debug.LogError ("clear");
		base.Clear ();
		ClearChat ();
//		if(mi != null){
//			mi.Clear();
//		}
//		PlatForm.inst.timeout = 5f;
//		TimerManager.inst.Remove (TimeChat_Tick);
		TimerManager.inst.Remove (Time_Tick);
		ViewManager.inst.RemoveTopView (micro);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_TEAMMATCH);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_STARTMATCHPUSH);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_ACCEPTINVITEOTHER);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_KILLTEAMPUSH);//SOCKET_KILLTEAM
		NetSocket.inst.RemoveListener (NetBase.SOCKET_KILLTEAM);//SOCKET_KILLTEAM
		NetSocket.inst.RemoveListener (NetBase.SOCKET_QUITTEAMPUSH);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_CANCELMATCHPUSH);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_FREESTARTMATCHPUSH);
		fightModel.Clear ();
		fightModel.ClearMicro ();
	}

	//	private void Quit_Click ()
	//	{
	//		Quit_Team ();
	//	}

	private void ItemRender (int index, GObject item)
	{
		GComponent g = item.asCom;
		Controller c = g.GetController ("c1");
		GComponent bg = g.GetChild ("n0").asCom;
		bg.RemoveEventListeners ();
		//lht
		g.GetChild ("n13").visible = false;
		g.GetChild ("n17").visible = false;
		//		if (index % 2 == 0) {
		bg.GetChild("n0").visible = ((index % 2) != 0);
//		Debug.LogError (bg.GetChild ("n0").visible);
		if (index > fightModel.team1.Length - 1)
		{			
//			g.GetChild ("n13").visible = false;
			c.selectedIndex = 2;					
			if (fightModel.isLeader)
			{
				g.GetChild ("n12").text = Tools.GetMessageById ("25035");
				bg.onClick.Add (() =>
				{
					if (fightModel.team1.Length == 4)
					{
						ViewManager.inst.ShowText (Tools.GetMessageById ("25048"));
						return;
					}
					fightModel.InviteRequest ();
				});
			}
			else
			{
				g.GetChild ("n12").text = Tools.GetMessageById ("25047");
			}
			if (fightModel.isMatch)
				g.GetChild ("n12").text = Tools.GetMessageById ("25057");
			return;
		}	
		Dictionary<string,object> data = (Dictionary<string,object>)fightModel.team1 [index];
//		if (fightModel.isLeader)
//		{
//		if (data ["uid"].ToString () == userModel.uid)
//			g.GetChild ("n13").visible = true;
//		else
//			g.GetChild ("n13").visible = false;
////		}
//		else
//			g.GetChild ("n13").visible = false;
//		g.GetChild ("n4").asLoader.url = Tools.GetSexUrl (data ["sex"]);
//		}

		g.GetChild ("n5").text = ModelUser.GetUname (data ["uid"], data ["name"]);
		g.GetChild ("n6").text = data ["guild_name"] == null ? Tools.GetMessageById ("25034") : data ["guild_name"].ToString ();
        GLoader img = g.GetChild ("n11").asCom.GetChild ("n1").asLoader;
		GTextField txt = g.GetChild ("n11").asCom.GetChild ("n2").asTextField;
		img.url = userModel.GetRankImg (data ["rank_score"]);
		txt.text = data ["rank_score"].ToString ();
		GButton head = g.GetChild ("n3").asButton;
		head.GetChild ("n2").text = data ["lv"].ToString ();
        //Tools.SetLoaderButtonUrl (head.GetChild ("n0").asButton, ModelUser.GetHeadUrl (Tools.Analysis (data, "head.use").ToString ()));
        if ((int)data["uid"] > 0)
        {
            Tools.SetLoaderButtonUrl(head.GetChild("n0").asButton, ModelUser.GetHeadUrl(Tools.Analysis(data, "head.use").ToString()));
        }
        else
        {
            Tools.SetLoaderButtonUrl(head.GetChild("n0").asButton, ModelUser.GetHeadUrl(data["name"].ToString(), false, true));

        }

        GButton btn = g.GetChild ("n8").asButton;
		btn.text = Tools.GetMessageById ("20119");
		if (fightModel.isLeader)
		{
			if (data ["uid"].ToString () == userModel.uid)
				c.selectedIndex = 0;
			else
				c.selectedIndex = 1;
		}
		else
		{
			c.selectedIndex = 0;
		}

		btn.RemoveEventListeners ();
		if (fightModel.isMatch)
			btn.enabled = false;
		else
			btn.enabled = true;
		btn.onClick.Add (() =>
		{
			ViewManager.inst.ShowAlert (Tools.GetMessageById ("25049"), (int id) =>
			{
				if (id == 1)
				{
					NetSocket.inst.AddListener (NetBase.SOCKET_QUITTEAM, (VoSocket vo) =>
					{
									//踢出队员成功提示，未配置上  25014
						ViewManager.inst.ShowText (Tools.GetMessageById ("25014"));
						NetSocket.inst.RemoveListener (NetBase.SOCKET_QUITTEAM);
						Log.debug ("踢出队员 - " + vo.data.ToString ());
//						ViewManager.inst.ShowText (Tools.GetMessageById ("25014"));
					});
					Dictionary<string,object> da = new Dictionary<string, object> ();
					da ["uid"] = data ["uid"].ToString ();
					NetSocket.inst.Send (NetBase.SOCKET_QUITTEAM, da);
				}
			}, true);
		});
	}

	private void Enter_Click ()
	{			
		NetSocket.inst.AddListener (NetBase.SOCKET_STARTMATCH, (VoSocket vo) =>
		{
			NetSocket.inst.RemoveListener (NetBase.SOCKET_STARTMATCH);
//			Log.debug ("开始匹配 - " + vo.data.ToString ());
//			ViewManager.inst.ShowText (Tools.GetMessageById ("25013"));
		});	
		NetSocket.inst.Send (NetBase.SOCKET_STARTMATCH, null);
	}

	private void Time_Tick (float t)
	{
		fightModel.matchTime += 1;
		if (fightModel.isLeader) {
			text1.text = Tools.GetMessageById ("25005", new string[] {
				fightModel.preTime.ToString (),
				fightModel.matchTime.ToString ()
			});
		}
		else {
			text2.text = Tools.GetMessageById ("25005", new string[] {
				fightModel.preTime.ToString (),
				fightModel.matchTime.ToString ()
			});
		}
	}

	private void Invite_Click ()
	{
		if (fightModel.team1.Length == 4)
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("25048"));
			return;
		}
		fightModel.InviteRequest ();
	}

	public override void Close ()
	{
//        ClearChat();
		if (fightModel.isLeader)
		{			
			//			NetSocket.inst.AddListener (NetBase.SOCKET_KILLTEAM, (VoSocket vo) =>
			//			{
			//				NetSocket.inst.RemoveListener (NetBase.SOCKET_KILLTEAM);
			//				if (vo.data is Boolean && Convert.ToBoolean (vo.data))
			//					ViewManager.inst.CloseView (this);		
			//			});
			if (fightModel.team1.Length <= 1)
			{
				Kill_Team();
			}
			else 
			{
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("25050"), (int index) =>
				{
					if (index == 1)
					{
						Kill_Team ();
					}
				}, true);
			}
		}
		else
		{
			ViewManager.inst.ShowAlert (Tools.GetMessageById ("25059"), (int index) =>
			{
				if (index == 1)
				{
					Quit_Team ();
				}
			}, true);
		}
	}

	private void Kill_Team ()
	{	
		NetSocket.inst.AddListener (NetBase.SOCKET_KILLTEAM, (VoSocket vo) => {
//			Debug.LogError ("SOCKET_KILLTEAM");
			fightModel.Clear ();
		});
//		NetSocket.inst.AddListener (NetBase.SOCKET_KILLTEAMPUSH, (VoSocket vo) => {
//			Debug.LogError ("SOCKET_KILLTEAMPUSH");
//		});
		NetSocket.inst.Send (NetBase.SOCKET_KILLTEAM, null);
		base.Close ();
	}

	private void Quit_Team ()
	{		
//		NetSocket.inst.AddListener (NetBase.SOCKET_QUITTEAM, (VoSocket vo) =>
//		{
//			NetSocket.inst.RemoveListener (NetBase.SOCKET_QUITTEAM);
//			if (vo.data is Boolean && Convert.ToBoolean (vo.data))
//				ViewManager.inst.CloseView (this);
//		});
		NetSocket.inst.Send (NetBase.SOCKET_QUITTEAM, null);
		base.Close ();
	}

	private void Cancel_Click ()
	{
        NetSocket.inst.AddListener (NetBase.SOCKET_CANCELMATCH, (VoSocket vo) =>
		{
			fightModel.isMatch = false;
			list.numItems = 4;//fightModel.team1.Length;

			NetSocket.inst.RemoveListener (NetBase.SOCKET_CANCELMATCH);
//			Log.debug ("取消匹配 - " + vo.data.ToString ());
			if (fightModel.isLeader)
				c1.selectedIndex = 0;
			else
				c1.selectedIndex = 1;
//			ViewManager.inst.ShowText (Tools.GetMessageById ("25015"));
		});
		NetSocket.inst.Send (NetBase.SOCKET_CANCELMATCH, null);
	}
	private void Chat_Click_Over(){
		if (mi != null) {
			mi.Over ();
		}
	}
	private void Chat_Click ()
	{

        btn_Chat.visible = false;
        btn_Mil.visible = true;
		time = 5;
		btn_Mil.text = time.ToString ();
		bar.visible = true;
//		TimerManager.inst.Add (1f, 5, TimeChat_Tick);
		mi = MicroManager.inst.Start ((byte[] obj) => {
			if (obj == null || obj.Length == 0) {
//				Debug.LogError("MicroManager.inst.Start 1");
				ClearChat ();
				return;
			}
			NetSocket.inst.AddListener (NetBase.SOCKET_MATCHTEAMCHAT, (VoSocket vo) => {
				NetSocket.inst.RemoveListener (NetBase.SOCKET_MATCHTEAMCHAT);
//				Log.debug ("Free Chat - " + vo.data.ToString ());
//				Debug.LogError("MicroManager.inst.Start 2");
				ClearChat ();
			});
			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["content"] = obj;
			NetSocket.inst.Send (NetBase.SOCKET_MATCHTEAMCHAT, data);
		}, (int obj) => {
//			Debug.LogError("MicroManager.inst.Start 3");
			bar.value = obj;
		}, btn_Mil);	
	}

	private void ClearChat ()
	{
        try {
			if(mi != null){
                mi.Clear();
			}
            if(btn_Mil != null)
            {
				btn_Chat.visible = true;
                btn_Mil.visible = false;
            }
//            TimerManager.inst.Remove(TimeChat_Tick);
            if(bar != null) {
                bar.value = 0;
                bar.visible = false;
            }

//			Debug.LogError("ClearChat");
        } catch(Exception e) { }
	}

//	private void TimeChat_Tick (float t)
//	{
//		time--;
//		btn_Mil.text = time.ToString ();
//		if (time <= 0)
//		{
//			ClearChat ();
//			if(mi!=null){
//				mi.
//			}
//		}
//	}
}