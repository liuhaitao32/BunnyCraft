using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorFreeMatch:BaseMediator
{
	private ModelFight fightModel;
	private ModelUser userModel;

	//	private GTextField title;
	private GButton btn_Change;
	private GButton btn_Start;
	private GButton btn_Invite;
	private GButton btn_Quit;
	private GButton btn_Clos;
	private GButton btn_Bot;
	private GButton btn_Chat;
	private GComponent item0;
	private GComponent item1;
	private GComponent item2;
	private GComponent item3;
	private GComponent item4;
	private GComponent item5;
	private GComponent item6;
	private GComponent item7;
	private Controller c1;
	private GButton btn_Mil;
	private BaseMediator micro;
	private ComProgressBar bar;
	private GButton help;

	private GComponent[] items;
	private int time;
	private Micro mi;

	public MediatorFreeMatch ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.isAutoClose = false;
		this.Create (Config.VIEW_FREEMATCH, false, Tools.GetMessageById ("25024"));

		fightModel = ModelManager.inst.fightModel;
		userModel = ModelManager.inst.userModel;
		fightModel.fightType = ModelFight.FIGHT_FREEMATCH1;

//		PlatForm.inst.timeout = 2f;

//		title = view.GetChild ("n1").asTextField;
		btn_Change = this.GetChild ("n2").asButton;
		btn_Start = this.GetChild ("n12").asButton;
		btn_Invite = this.GetChild ("n13").asButton;

        btn_Change.text = Tools.GetMessageById("33209");
        btn_Start.text = Tools.GetMessageById("33210");
        btn_Invite.text = Tools.GetMessageById("33211");

		btn_Quit = this.GetChild ("n16").asButton;
		btn_Clos = this.GetChild ("n31").asButton;
		btn_Bot = this.GetChild ("n11").asButton;
		btn_Chat = this.GetChild ("n19").asButton;
		c1 = this.GetController ("c1");
		btn_Mil = this.GetChild ("n28").asButton;
		bar = this.GetChild ("n29").asCom as ComProgressBar;
		bar.skin = ComProgressBar.BAR10;
		bar.SetTextVisible (false);
		bar.value = 0;
		bar.max = 100;

		item0 = this.GetChild ("n3").asCom;
		item1 = this.GetChild ("n4").asCom;
		item2 = this.GetChild ("n5").asCom;
		item3 = this.GetChild ("n6").asCom;
		item4 = this.GetChild ("n7").asCom;
		item5 = this.GetChild ("n8").asCom;
		item6 = this.GetChild ("n9").asCom;
		item7 = this.GetChild ("n10").asCom;
		help = this.GetChild ("n30").asButton;

		btn_Invite.onClick.Add (Invite_Click);
		btn_Clos.onClick.Add (Close_Click);
		btn_Change.onClick.Add (Change_Click);
		btn_Start.onClick.Add (Start_Click);
		btn_Bot.onClick.Add (Bot_Click);
		btn_Chat.onClick.Add (Chat_Click);
		btn_Quit.onClick.Add (Close_Click);
		help.onClick.Add (Help_Click);

		userModel.GetUnlcok (Config.UNLOCK_FREEMATCH2, btn_Change);

//		public int[] freeType1 = new int[]{ 0, 1, 2, 3, 4, 5, 6, 7 };
//		public int[] freeType2 = new int[]{ 0, 1, 4, 5, 2, 3, 6, 7 };
		if (fightModel.freeType == 1)
			items = new GComponent[8]{ item0, item1, item2, item3, item4, item5, item6, item7 };
		else
			items = new GComponent[8]{ item0, item1, item4, item5, item2, item3, item6, item7 };
		this.SetDefault();
		if (fightModel.isRequest)
		{
			NetSocket.inst.AddListener (NetBase.SOCKET_NEWROOM, (VoSocket vo) =>
			{				
				NetSocket.inst.RemoveListener (NetBase.SOCKET_NEWROOM);
				fightModel.CreateFreeTeam (vo.data);
				UpdateItems ();
				CheckMode ();
				btn_Bot.visible = fightModel.isLeader;
			});

			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["mode"] = fightModel.freeType;
			NetSocket.inst.Send (NetBase.SOCKET_NEWROOM, data);
		}
		else
		{
			CheckMode ();
			UpdateItems ();
			btn_Bot.visible = fightModel.isLeader;
		}

		micro = new MediatorMicro ();
		ViewManager.inst.AddTopView (micro);
//		micro.visible = false;

		NetSocket.inst.AddListener (NetBase.SOCKET_ADDUSER, (VoSocket vo) =>
		{
			fightModel.ChangeFreeTeam (vo.data);
			UpdateItems ();
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_REMOVEUSER, (VoSocket vo) =>
		{
			bool b = fightModel.DelFreeTeam (vo.data);
			if (b)
			{
				fightModel.Clear ();
				ViewManager.inst.CloseView (this);
				ViewManager.inst.ShowText (Tools.GetMessageById ("25011"));
			}
			else
			{
				UpdateItems ();
			}
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_KILLROOMPUSH, (VoSocket vo) =>
		{
			fightModel.Clear ();
			ViewManager.inst.CloseView (this);
			ViewManager.inst.ShowText (Tools.GetMessageById ("25008"));
			Log.debug ("解散队伍");
		});

		NetSocket.inst.AddListener (NetBase.SOCKET_FREESTARTMATCHPUSH, (VoSocket vo) =>
		{
			if (fightModel.freeType == 1)
				fightModel.fightType = ModelFight.FIGHT_FREEMATCH1;
			else
				fightModel.fightType = ModelFight.FIGHT_FREEMATCH2;
			Log.debug ("进入战斗");
			ViewManager.inst.ShowScene <MediatorFightWorld> (true);
		});			

		NetSocket.inst.AddListener (NetBase.SOCKET_CHANGEMODEPUSH, (VoSocket vo) =>
		{
			Dictionary<string,object> da = (Dictionary<string,object>)vo.data;
			fightModel.freeType = Convert.ToInt32 (da ["new_mode"]);
			CheckMode (true);
			Log.debug ("更换模式");
//			ViewManager.inst.ShowText (Tools.GetMessageById ("25021"));
		});			

		NetSocket.inst.AddListener (NetBase.SOCKET_CHANGEPOSPUSH, (VoSocket vo) =>
		{			
			fightModel.RepalceFreeTeam (vo.data);
			CheckMode (true);
			Log.debug ("更换位置");
//			ViewManager.inst.ShowText (Tools.GetMessageById ("25021"));
		});
			
		NetSocket.inst.AddListener (NetBase.SOCKET_FREECHATPUSH, (VoSocket vo) =>
		{
			if (userModel.IsChatVoice ())
				fightModel.AddMicro (vo.data);
		});

		this.AddGlobalListener (MainEvent.SOCKET_CLOSE, (MainEvent e) =>
		{
			this.Close ();
		});
	}
	public void InitData(){
		
	}
	public override void Clear ()
	{
		base.Clear ();
//		PlatForm.inst.timeout = 5f;
		TimerManager.inst.Remove (Time_Tick);
		fightModel.Clear ();
		ViewManager.inst.RemoveTopView (micro);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_ADDUSER);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_REMOVEUSER);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_KILLROOMPUSH);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_FREESTARTMATCHPUSH);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_CHANGEMODEPUSH);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_CHANGEPOSPUSH);
		NetSocket.inst.RemoveListener (NetBase.SOCKET_FREECHATPUSH);
	}

	private void CheckMode (bool isCheck = false)
	{		
		if (fightModel.freeType == 1)
			this.text = Tools.GetMessageById ("25024");
		else
			this.text = Tools.GetMessageById ("25025");
		if (fightModel.isLeader)
		{
			if (fightModel.freeType == 1)
				c1.selectedIndex = 0;
			else
				c1.selectedIndex = 2;
		}
		else
		{
			if (fightModel.freeType == 1)
				c1.selectedIndex = 1;
			else
				c1.selectedIndex = 3;
		}

		if (isCheck)
		{
			if (fightModel.freeType == 1)
				items = new GComponent[8]{ item0, item1, item2, item3, item4, item5, item6, item7 };
			else
				items = new GComponent[8]{ item0, item1, item4, item5, item2, item3, item6, item7 };
//			fightModel.ChangeModel();
			UpdateItems ();
		}
	}

	private void ClearItem (GComponent g)
	{
		if (fightModel.freeType == 1)
		{
			g.GetController ("c1").selectedIndex = 2;
			g.touchable = false;
		}
		else
		{
			g.GetController ("c1").selectedIndex = 1;
		}
	}

	private void SetDefault ()
	{
		foreach (GComponent g in items)
		{
			g.GetController ("c1").selectedIndex = 2;
		}
	}

	private void UpdateItem (int index, object data)
	{
		Dictionary<string,object> da = (Dictionary<string,object>)data;
		GComponent g = items [index];
//		g.GetController ("c1").selectedIndex = 2;
		g.GetChild ("n14").visible = false;
		g.GetChild ("n0").asCom.GetChild ("n3").visible = false;
		g.RemoveEventListeners ();
		g.onClick.Add (() =>
		{
			if (fightModel.isLeader && (da != null && Convert.ToInt32 (da ["uid"]) <= 0))
			{
				NetSocket.inst.AddListener (NetBase.SOCKET_REMOVEBOT, (VoSocket vo) =>
				{
					NetSocket.inst.RemoveListener (NetBase.SOCKET_REMOVEBOT);
					if (Convert.ToBoolean (vo.data))
						ViewManager.inst.ShowText (Tools.GetMessageById ("25019"));
					else
						ViewManager.inst.ShowText (Tools.GetMessageById ("25020"));
				});
				Dictionary<string,object> d = new Dictionary<string, object> ();
				d ["index"] = index;
				NetSocket.inst.Send (NetBase.SOCKET_REMOVEBOT, d);
			}
			else if (da == null || (da != null && Convert.ToInt32 (da ["uid"]) <= 0))
			{
				if (fightModel.freeType == 2)
				{
					NetSocket.inst.AddListener (NetBase.SOCKET_CHANGEPOS, (VoSocket vo) =>
					{
						NetSocket.inst.RemoveListener (NetBase.SOCKET_CHANGEPOS);
						Log.debug ("交换位置 - " + vo.data.ToString ());
//						if (Convert.ToBoolean (vo.data))
//							ViewManager.inst.ShowText (Tools.GetMessageById ("25022"));
//						else
//							ViewManager.inst.ShowText (Tools.GetMessageById ("25023"));
					});
					Dictionary<string,object> d = new Dictionary<string, object> ();
					d ["pos"] = index;
					NetSocket.inst.Send (NetBase.SOCKET_CHANGEPOS, d);
				}
			}
		});
		g.touchable = true;
		if (data == null)
		{
			ClearItem (g);
			return;		
		}
		if (Convert.ToInt32 (da ["uid"]) < 0)
		{
//			if (fightModel.isLeader)
//			{
//				g.GetController ("c1").selectedIndex = 3;
//				g.GetChild ("n4").text = Tools.GetMessageById ("25027");
//				g.GetChild ("n4").text = Tools.GetMessageById ("25028", new string[]{ da ["uid"].ToString () });
//				g.GetChild ("n4").text = da ["name"].ToString ();
//			}
//			else
//			{
			g.GetController ("c1").selectedIndex = 3;
			g.GetChild ("n4").text = da ["name"].ToString ();
//			}
		}
		else
		{
			g.GetController ("c1").selectedIndex = 0;
			GButton head = g.GetChild ("n0").asButton;
			head.GetChild ("n2").text = da ["lv"].ToString ();
			Tools.SetLoaderButtonUrl (head.GetChild ("n0").asButton, ModelUser.GetHeadUrl (Tools.Analysis (da, "head.use").ToString ()));
			g.GetChild ("n1").asTextField.text = ModelUser.GetUname (da ["uid"], da ["name"]);
			g.GetChild ("n14").visible = da ["uid"].ToString () == userModel.uid ? true : false;
		}
	}

	private void UpdateItems ()
	{
		for (int i = 0; i < items.Length; i++)
		{
			UpdateItem (i, fightModel.team3 [i]);
		}
	}

	private void Invite_Click ()
	{
		bool isOK = false;
		for (int i = 0; i < fightModel.team3.Length; i++)
		{
			if (fightModel.team3 [i] == null)
			{
				isOK = true;
				break;
			}
		}
		if (!isOK)
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("25048"));
			return;
		}
		fightModel.InviteRequest ();
	}

	public override void Close ()
	{
		Close_Click ();
	}

	private void Close_Click ()
	{
		if (fightModel.isLeader)
		{
//			NetSocket.inst.AddListener (NetBase.SOCKET_KILLROOM, (VoSocket vo) =>
//			{
//				NetSocket.inst.RemoveListener (NetBase.SOCKET_KILLROOM);
//				Log.debug ("退出或解散 - " + vo.data.ToString ());
//			});
			int count = fightModel.GetFreeTeamCount ();
			if (count <= 1)
			{
				NetSocket.inst.Send (NetBase.SOCKET_KILLROOM, null);									
				ViewManager.inst.ShowText (Tools.GetMessageById ("25008"));
				base.Close ();
			}
			else
			{
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("25050"), (int index) =>
				{
					if (index == 1)
					{
						NetSocket.inst.Send (NetBase.SOCKET_KILLROOM, null);									
						ViewManager.inst.ShowText (Tools.GetMessageById ("25008"));
						base.Close ();
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
					NetSocket.inst.Send (NetBase.SOCKET_QUIT, null);
					base.Close ();
				}
			}, true);
		}
	}

	private void Help_Click ()
	{
		ViewManager.inst.ShowView<MediatorFightTip> ();
	}

	private void Change_Click ()
	{
		bool isOk = userModel.GetUnlcok (Config.UNLOCK_FREEMATCH2, null, true);
		if (!isOk)
			return;
		NetSocket.inst.AddListener (NetBase.SOCKET_CHANGEMODE, (VoSocket vo) =>
		{
			NetSocket.inst.RemoveListener (NetBase.SOCKET_CHANGEMODE);
			Log.debug ("模式更换 - " + vo.data.ToString ());
		});

		Dictionary<string,object> data = new Dictionary<string, object> ();
		data ["new_mode"] = fightModel.freeType == 1 ? 2 : 1;
		NetSocket.inst.Send (NetBase.SOCKET_CHANGEMODE, data);
	}

	private void Start_Click ()
	{		
		if (!fightModel.IsFreeFightStart ())
			return;
		NetSocket.inst.AddListener (NetBase.SOCKET_FREESTARTMATCH, (VoSocket vo) =>
		{			
			NetSocket.inst.RemoveListener (NetBase.SOCKET_FREESTARTMATCH);
			Log.debug ("组队开始 - " + vo.data.ToString ());
		});
		NetSocket.inst.Send (NetBase.SOCKET_FREESTARTMATCH, null);
	}

	private void Bot_Click ()
	{
		NetSocket.inst.AddListener (NetBase.SOCKET_FREEADDBOT, (VoSocket vo) =>
		{
			NetSocket.inst.RemoveListener (NetBase.SOCKET_FREEADDBOT);
			if (!Convert.ToBoolean (vo.data))
//				ViewManager.inst.ShowText (Tools.GetMessageById ("25017"));
//			else
				ViewManager.inst.ShowText (Tools.GetMessageById ("25018"));
		});
		NetSocket.inst.Send (NetBase.SOCKET_FREEADDBOT, null);
	}

	private void Chat_Click ()
	{
		btn_Mil.visible = true;
		time = 5;
		btn_Mil.text = time.ToString ();
		bar.visible = true;
		TimerManager.inst.Add (1f, 5, Time_Tick);
		mi = MicroManager.inst.Start ((byte[] obj) =>
		{
			if (obj == null || obj.Length == 0)
			{
				ClearChat ();
				return;
			}
			NetSocket.inst.AddListener (NetBase.SOCKET_FREECHAT, (VoSocket vo) =>
			{
				NetSocket.inst.RemoveListener (NetBase.SOCKET_FREECHAT);
				Log.debug ("Free Chat - " + vo.data.ToString ());
				ClearChat ();
			});
			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["content"] = obj;
			NetSocket.inst.Send (NetBase.SOCKET_FREECHAT, data);
		}, (int obj) =>
		{
			bar.value = obj;
		});			
	}

	private void ClearChat ()
	{
		if (mi != null)
			mi.Clear ();
		btn_Mil.visible = false;
		TimerManager.inst.Remove (Time_Tick);
		bar.value = 0;
		bar.visible = false;
	}

	private void Time_Tick (float t)
	{
		time--;
		btn_Mil.text = time.ToString ();
//		if (time <= 0)
//		{
//			ClearChat ();
//		}
	}
   
}