using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;

public class MediatorGuildMain : BaseMediator
{

	private ModelRole roleModel;
	private ModelGuild guildModel;
	private ModelChat chatModel;
	private ModelUser userModel;

	private GButton back;
//	private GComponent frind_left_tab1;
//	private GButton frind_left_tab_1_0;
//	private GButton frind_left_tab_1_1;
//	private GButton frind_left_tab_1_2;
//	private GButton frind_left_tab_1_3;
//	private Controller tabC2;
	private bool haveGuild = false;
	private BaseMediator childView;
	private GComponent n0;
	private bool jiaru = false;
	private bool tipSetUp = false;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.SCENE_GUILDMAIN);

		roleModel = ModelManager.inst.roleModel;
		guildModel = ModelManager.inst.guildModel;
		chatModel = ModelManager.inst.chatModel;
		userModel = ModelManager.inst.userModel;

		
		Init_LeftTab (new string[]{ Tools.GetMessageById ("19908") }, "n22");

//		frind_left_tab1 = (GComponent)this.GetChild ("n6");
//		frind_left_tab_1_0 = frind_left_tab1.GetChild ("n0").asButton;
//		frind_left_tab_1_1 = frind_left_tab1.GetChild ("n1").asButton;
//		frind_left_tab_1_2 = frind_left_tab1.GetChild ("n2").asButton;
//		frind_left_tab_1_3 = frind_left_tab1.GetChild ("n3").asButton;


		this.GetChild ("n9").asTextField.text = Tools.GetMessageById ("20170");
		this.GetChild ("n4").asButton.text = "";
		back = this.GetChild ("n2").asButton;
		back.onClick.Add (OnCloseHandel);

		this.AddGlobalListener (MainEvent.GUILD_ESC, OnExcFunction);
		this.GetChild ("n4").asButton.onClick.Add (()=>{
			ModelManager.inst.fightModel.OpenFreeMatchInvite();
		});
//		this.GetChild ("n3").asButton.text = Tools.GetMessageById ("19908");
		n0 = this.GetChild ("bg").asCom;
		tabC1 = this.GetController ("c2");
		tabC2 = this.GetController ("c3");
		tabC2.onChanged.Add (()=>{
			base.OnTabLeftChange();
		});
		tabC2.onChanged.Call ();
		tabC1.onChanged.Add (On_Left_Change1);
//		this.GetChild ("n8").asGroup.visible = false;
		if (ModelManager.inst.userModel.Get_NoticeState (ModelUser.RED_GUILDJOIN) != 0)
		{
			jiaru = true;
		}
		this.AddGlobalListener (MainEvent.GUILDICON_CHANGE, GUILDICON_CHANGE);
		this.AddGlobalListener (MainEvent.CHANGE_GUILD_NAME, CHANGE_GUILD_NAME);
		this.AddGlobalListener (MainEvent.GUILDDIZHI_CHANGE, GUILDDIZHI_CHANGE);
		GetGuildInfo (guildModel.viewData);

//		if (roleModel.tab_Select2 != -1 && roleModel.tab_Select2 != 0)
//		{
//			tabC2.selectedIndex = roleModel.tab_Select2;
//			roleModel.tab_Select2 = -1;
//		}
	}
	private void GUILDDIZHI_CHANGE(MainEvent e)
	{
		object[] ddd = ((object[])((Dictionary<string,object>)DataManager.inst.systemSimple ["society_location"]) [ModelManager.inst.guildModel.location + ""]);
		this.GetChild ("n16").asTextField.text = Tools.GetMessageById ("20174") + Tools.GetMessageById (ddd [1].ToString ());
	}
	private void CHANGE_GUILD_NAME(MainEvent e)
	{
		if(this.GetChild ("n14").asTextField != null)
		{
			this.GetChild ("n14").asTextField.text = ModelManager.inst.guildModel.my_guild_info ["name"] as string;
		}
	}
	private void GUILDICON_CHANGE(MainEvent e)
	{
		Dictionary<string,object> datas = (Dictionary<string,object>)(Tools.Clone (ModelManager.inst.guildModel.my_guild_info));
		this.GetChild("n11").asLoader.url = Tools.GetResourceUrl ("Icon:" + (string)(datas ["icon"]));
	}
	private void On_Left_Change1 ()
	{
		base.OnTabChange ();
		roleModel.tab_CurSelect2 = tabC1.selectedIndex;
//		frind_left_tab_1_0.GetController ("c1").selectedIndex = 0;
//		frind_left_tab_1_1.GetController ("c1").selectedIndex = 0;
//		frind_left_tab_1_2.GetController ("c1").selectedIndex = 0;
//		frind_left_tab_1_3.GetController ("c1").selectedIndex = 0;
		if (tabC1.selectedIndex == 0)
		{
			userModel.Remove_Notice (this.GetChild ("bar0").asCom);
			this.RemoveGlobalListener (MainEvent.RED_CHATUPDATE, RED_CHATUPDATE);
		}
		else
		{
			this.AddGlobalListener (MainEvent.RED_CHATUPDATE, RED_CHATUPDATE);	
			RED_CHATUPDATE (null);
		}
		switch (tabC1.selectedIndex)
		{
		case 0:
//			frind_left_tab_1_0.GetController ("c1").selectedIndex = 1;
			this.GetChild ("n28").height = 105;
			if (haveGuild) {
				this.AddChildView (new MediatorChat ());
			} else {
				this.AddChildView (new MediatorItemGuildList ());
			}
			break;
		case 1:
//			frind_left_tab_1_1.GetController ("c1").selectedIndex = 1;
			if (haveGuild) {
				this.GetChild ("n28").height = 30;
				this.AddChildView (new MediatorItemGuildMember ());
			} else {
				this.GetChild ("n28").height = 105;
				this.AddChildView (new MediatorItemCreateGuild ());
			}
				
			break;
		case 2:
//			frind_left_tab_1_2.GetController ("c1").selectedIndex = 1;

			if (haveGuild) {
				this.GetChild ("n28").height = 105;
				this.AddChildView (new MediatorItemGuildInList ());
			} else {
				this.GetChild ("n28").height = 25;
				this.AddChildView (new MediatorItemGuildSearch ());
			}
				
			break;
		case 3:
//			frind_left_tab_1_3.GetController ("c1").selectedIndex = 1;
			this.AddChildView (new MediatorItemSetUp ());
			break;
		case 4:
			break;
		}
	}

	private void Tab_1 ()
	{		
		if (haveGuild)
		{			
//			frind_left_tab_1_3.visible = false;
//			frind_left_tab_1_0.text = Tools.GetMessageById ("19916");
//			frind_left_tab_1_1.text = Tools.GetMessageById ("19917");
//			frind_left_tab_1_2.text = Tools.GetMessageById ("19918");

			InitTopBar(new string[]{ Tools.GetMessageById ("19916"), Tools.GetMessageById ("19917"), Tools.GetMessageById ("19918") });
		}
		else
		{
//			frind_left_tab_1_3.visible = false;
//			frind_left_tab_1_0.text = Tools.GetMessageById ("19918");
//			frind_left_tab_1_1.text = Tools.GetMessageById ("19915");
//			frind_left_tab_1_2.text = Tools.GetMessageById ("19920");
			InitTopBar(new string[]{ Tools.GetMessageById ("19918"), Tools.GetMessageById ("19915"), Tools.GetMessageById ("19920") });
		}
//		tabC2.onChanged.Add (On_Left_Change1);
		if (roleModel.tab_CurSelect2 != -1 && roleModel.tab_CurSelect2 != 0)
		{
			if (tabC1.selectedIndex == roleModel.tab_CurSelect2)
			{
				tabC1.onChanged.Call ();
			}
			tabC1.selectedIndex = roleModel.tab_CurSelect2;

//			roleModel.tab_CurSelect2 = -1;
		}
		else
		{
//			roleModel.tab_CurSelect2 = -1;
			if (tabC1.selectedIndex != 0)
			{
				tabC1.selectedIndex = 0;
			}
			else
			{
				On_Left_Change1 ();
			}
		}
	}

	private void OnCloseHandel (EventContext context)
	{
		ViewManager.inst.CloseScene();
	}

	private void AddChildView (BaseMediator child)
	{
		if (childView != null)
		{
			this.RemoveChild (childView.group, true);
			childView = null;
		}
		childView = child;
		this.AddChild (childView.group);
		childView.group.x = n0.x;
		childView.group.y = n0.y + 60;
	}

	private void GetGuildInfo (VoHttp vo)
	{
//		tab_root_0.selected = false;
//		tab_root_1.selected = true;
		ModelManager.inst.guildModel.viewData = vo;
		Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
		guildModel.guildList = null;
		if (data ["my_guild_info"] != null)
		{
			haveGuild = true;
            TimerManager.inst.Add(0.3f, 1, (float ff) => { tabC1.selectedIndex = 0; });
//			this.GetChild ("n8").asGroup.visible = true;
			guildModel.location = ((int)(((Dictionary<string,object>)data ["my_guild_info"]) ["location"]));
			guildModel.guildHave = true;
			guildModel.my_guild_job = (int)(((Dictionary<string,object>)(((Dictionary<string,object>)(((Dictionary<string,object>)data ["my_guild_info"]) ["member"])) [ModelManager.inst.userModel.uid + ""])) ["gradation"]);
			guildModel.my_guild_info = (Dictionary<string,object>)data ["my_guild_info"];
			guildModel.my_guild_member = ((Dictionary<string,object>)(((Dictionary<string,object>)data ["my_guild_info"]) ["member"]));

			Dictionary<string,object> datas = (Dictionary<string,object>)(Tools.Clone (ModelManager.inst.guildModel.my_guild_info));
			this.GetChild ("n14").asTextField.text = (string)(datas ["name"]);
			this.GetChild ("n15").asTextField.text = Tools.GetMessageById ("20102") + (datas ["id"]).ToString ();
			object[] ddd = ((object[])((Dictionary<string,object>)DataManager.inst.systemSimple ["society_location"]) [ModelManager.inst.guildModel.location + ""]);
			this.GetChild ("n16").asTextField.text = Tools.GetMessageById ("20174") + Tools.GetMessageById (ddd [1].ToString ());
			this.GetChild ("n17").asButton.text = Tools.GetMessageById ("19919");
			this.GetChild("n11").asLoader.url = Tools.GetResourceUrl ("Icon:" + (string)(datas ["icon"]));
			this.GetChild ("n18").visible = true;
//			this.GetChild ("n21").visible = false;
			if (guildModel.my_guild_job < 2) {
				this.GetController ("c1").selectedIndex = 1;
			} else {
				this.GetController ("c1").selectedIndex = 0;
			}
			this.GetChild ("n17").asButton.RemoveEventListeners ();
			this.GetChild ("n17").asButton.onClick.Add (OnGuildSetUp);
		}
		else
		{
			this.GetChild ("n18").visible = false;
//			this.GetChild ("n21").visible = true;
			haveGuild = false;
			guildModel.guildHave = false;
//			this.GetChild ("n8").asGroup.visible = false;
		}
		guildModel.word_guild_list = (object[])(data ["guild_rank"]);
		guildModel.SetGuildList ((object[])(data ["guild_rank"]));
		if (jiaru)
		{
            string str = "";
            str = guildModel.my_guild_info == null ? " " : guildModel.my_guild_info["name"].ToString();
			ViewManager.inst.ShowText (Tools.GetMessageById ("20168", new object[]{ str }));
			jiaru = false;
		}
		if (tipSetUp) {
			tipSetUp = false;
			OnGuildSetUp ();
		}
		Tab_1 ();
	}
	private void OnGuildSetUp()
	{
		ViewManager.inst.ShowView<MediatorItemSetUp> ();
	}
	private void RED_CHATUPDATE (MainEvent e)
	{
		///////////////////////////////////////////////////////公会
		int count = chatModel.GetChatRedCount ();
		if (count > 0)
			userModel.Add_Notice (this.GetChild("bar0").asCom, new Vector2 (150, 0), count);
		else
			userModel.Remove_Notice (this.GetChild("bar0").asCom);
	}

	private void OnExcFunction (MainEvent e)
	{
		MediatorChangeIcon.iconID = "g01";
		if (e.data != null)
		{
			if ((int)e.data == 100)
			{
				jiaru = true;
				return;
			}
			else if ((int)e.data == 10)
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("20161"));
                chatModel.Clear_RedCount();
			}
			else
			{
//				roleModel.tab_CurSelect2 = (int)e.data;
			}
			if ((int)e.data == 954) {
				tipSetUp = true;
//				roleModel.tab_CurSelect2 = -1;
			}

		}
		else
		{
//			roleModel.tab_CurSelect2 = -1;
		}
		NetHttp.inst.Send (NetBase.HTTP_GUILD_INDEX, "", GetGuildInfo);
	}

	public override void Clear ()
	{
		base.Clear ();
	}
}