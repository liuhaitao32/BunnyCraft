using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using FairyGUI;

public class MediatorItemGuildMember : BaseMediator
{
	private ModelRole roleModel;
	private ModelGuild guildModel;

	private GTextField l_name;
	private GTextField l_id;
	private GTextField l_number;
//	private GTextField l_coinNum;
	private GTextField l_gonggao;
	private GLoader l_icon;
	private GList list;
	private GButton btn_outGuild;
	private GameObject gg;

	private List<object> lis;

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_GUILDMEMBER);

		roleModel = ModelManager.inst.roleModel;
		guildModel = ModelManager.inst.guildModel;

		Dictionary<string,object> data = (Dictionary<string,object>)(Tools.Clone (ModelManager.inst.guildModel.my_guild_info));


		l_name = this.GetChild ("n4").asTextField;
		l_id = this.GetChild ("n6").asTextField;
		l_number = this.GetChild ("n5").asTextField;
//		l_coinNum = view.GetChild ("n7").asTextField;
		l_gonggao = this.GetChild ("n0").asTextField;
		l_icon = this.GetChild ("n8").asCom.GetChild("n0").asLoader;
		list = this.GetChild ("n14").asList;
		btn_outGuild = this.GetChild ("n15").asButton;
		btn_outGuild.text = Tools.GetMessageById ("20136");
		btn_outGuild.onClick.Add (OnTuiChuGH);
		if (ModelManager.inst.guildModel.my_guild_job == 0 &&	 ModelManager.inst.guildModel.my_guild_member.Count == 1)
		{
			btn_outGuild.text = Tools.GetMessageById ("20112");
		}

		l_icon.url = Tools.GetResourceUrl ("Icon:" + (string)(data ["icon"]));
		l_name.text = (string)(data ["name"]);
		l_id.text = Tools.GetMessageById ("20102");
		this.GetChild ("n18").asTextField.text = (data ["id"]).ToString ();
		l_number.text = Tools.GetMessageById ("20103",new string[]{((Dictionary<string,object>)(data ["member"])).Count + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString ()});
		this.GetChild ("n19").asTextField.text = ((Dictionary<string,object>)(data ["member"])).Count + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString ();
//		l_coinNum.text = Tools.GetMessageById ("20104");
		this.GetChild ("n21").asCom.GetChild("n2").asTextField.text = (data ["score"]).ToString ();
		l_gonggao.text = Tools.GetMessageById ("20115", new string[]{ (data ["content"]).ToString () });
		object[] ddd = ((object[])((Dictionary<string,object>)DataManager.inst.systemSimple ["society_location"]) [ModelManager.inst.guildModel.location + ""]);
		this.GetChild ("n1").asTextField.text = Tools.GetMessageById (ddd [1].ToString());
		list.itemRenderer = List_Render;
//		list.SetVirtual ();

//		view.GetChild ("n9").asTextField.text = Tools.GetMessageById("20132");
//		view.GetChild ("n10").asTextField.text = Tools.GetMessageById("20133");
//		view.GetChild ("n11").asTextField.text = Tools.GetMessageById("20134");
//		view.GetChild ("n12").asTextField.text = Tools.GetMessageById("20135");

		this.AddGlobalListener (MainEvent.CHAT_GUILDMODIFY, CHAT_GUILDJOIN);
		this.AddGlobalListener (MainEvent.CHAT_GUILDJOIN, CHAT_GUILDJOIN);

		this.visible = false;
		NetHttp.inst.Send (NetBase.HTTP_GUILD_INDEX, "", GetGuildInfo);
	}
	private void CHAT_GUILDJOIN (MainEvent e)
	{
		if (this.view != null && this.view.parent != null) {
			NetHttp.inst.Send (NetBase.HTTP_GUILD_INDEX, "", GetGuildInfo);
		}
	}
	private void GetGuildInfo (VoHttp vo)
	{
		if (this.view != null && this.view.parent != null) {
			this.visible = true;
			Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
			guildModel.guildList = null;
			if (data ["my_guild_info"] != null) {
				guildModel.location = ((int)(((Dictionary<string,object>)data ["my_guild_info"]) ["location"]));
				guildModel.guildHave = true;
				guildModel.my_guild_job = (int)(((Dictionary<string,object>)(((Dictionary<string,object>)(((Dictionary<string,object>)data ["my_guild_info"]) ["member"])) [ModelManager.inst.userModel.uid + ""])) ["gradation"]);
				guildModel.my_guild_info = (Dictionary<string,object>)data ["my_guild_info"];
				guildModel.my_guild_member = ((Dictionary<string,object>)(((Dictionary<string,object>)data ["my_guild_info"]) ["member"]));
			}
			guildModel.word_guild_list = (object[])(data ["guild_rank"]);
			guildModel.SetGuildList ((object[])(data ["guild_rank"]));

			CHAT_GUILDMODIFY ();
		}
	}
	private void CHAT_GUILDMODIFY(MainEvent e = null)
	{
		Dictionary<string,object> data = (Dictionary<string,object>)(Tools.Clone (guildModel.my_guild_info));
		lis = new List<object> ();
		foreach (string i in ((Dictionary<string,object>)data ["member"]).Keys)
		{
			Dictionary<string,object> da = (Dictionary<string,object>)((Dictionary<string,object>)data ["member"]) [i];
			da.Add ("id", i);
			if (da.ContainsKey ("rank_score")) {
				lis.Add (da);
			}
		}
		Tools.Sort (lis, new string[]{ "rank_score:int:1", "contribute:int:1", "gradation:int:1", "id:int:1" });
        
		if (lis.Count >= 6) {
			list.numItems = lis.Count;
		} else {
			list.numItems = 6;
		}
		if (lis.Count <= 5) {
			list.scrollPane.touchEffect = false;
		}
        
		l_number.text = Tools.GetMessageById ("20103", new string[]{ lis.Count + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString () });// +;
		this.GetChild ("n19").asTextField.text = lis.Count + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString ();
		if (ModelManager.inst.guildModel.my_guild_job == 0 &&	 ModelManager.inst.guildModel.my_guild_member.Count == 1)
		{
			btn_outGuild.text = Tools.GetMessageById ("20112");
		}
	}
	private void OnTuiChuGH ()
	{
		if (guildModel.my_guild_job == 0)
		{
			if (guildModel.my_guild_member.Count > 1) {
				ViewManager.inst.ShowText (Tools.GetMessageById ("20151"));
				return;
			}
			ViewManager.inst.ShowAlert (Tools.GetMessageById("20153"), (int bo) => {
				if(bo == 1)
					NetHttp.inst.Send (NetBase.HTTP_GUILD_OVER, "", GuildOver);
			}, true);

		}
		else
		{
			ViewManager.inst.ShowAlert (Tools.GetMessageById("20154"), (int bo) => {
				if(bo == 1)
					NetHttp.inst.Send (NetBase.HTTP_GUILD_EXIT, "", GuildExc);
			}, true);
		}
	}
	private void GuildOver (VoHttp vo)
	{
		ModelManager.inst.chatModel.Clear ();
		ViewManager.inst.ShowText (Tools.GetMessageById("20162"));
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.GUILD_ESC));
	}
	private void GuildExc (VoHttp vo)
	{
		ModelManager.inst.chatModel.Clear ();
		ViewManager.inst.ShowText (Tools.GetMessageById("20163"));
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.GUILD_ESC));
	}

	private void List_Render (int index, GObject go)
	{
		
		GButton btn_go = go.asCom.GetChild ("n0").asButton;
		if (index == 0 || index % 2 == 0) {
			btn_go.alpha = 0;
		} else {
			btn_go.alpha = 1;
		}


		GButton img_icon = go.asCom.GetChild ("n1").asButton;
		GLoader img_rank = go.asCom.GetChild ("n10").asCom.GetChild("n1").asLoader;
		GTextField l_name = go.asCom.GetChild ("n2").asTextField;
		GTextField l_level = go.asCom.GetChild ("n3").asCom.GetChild ("n1").asTextField;
		GTextField l_rank = go.asCom.GetChild ("n10").asCom.GetChild ("n2").asTextField;
		GTextField l_job = go.asCom.GetChild ("n6").asTextField;
		GTextField l_guildCoin = go.asCom.GetChild ("n7").asTextField;
		GButton btn_set = go.asCom.GetChild ("n8").asButton;
		GTextField ranktxt =go.asCom.GetChild("n18").asCom.GetChild("n4").asTextField;
        GLoader pmStart = go.asCom.GetChild("n18").asCom.GetChild("n1").asLoader;
        btn_go.GetChild("n2").asLoader.url = Tools.GetResourceUrl("Image2:n_bg_tanban6");
        if (index > lis.Count - 1) {
			img_icon.visible = false;
			img_rank.visible = false;
			l_name.visible = false;
			go.asCom.GetChild("n3").visible = false;
			l_rank.visible = false;
			l_job.visible = false;
			l_guildCoin.visible = false;
			btn_set.visible = false;
			ranktxt.visible = false;
			pmStart.visible = false;
			btn_go.touchable = false;
			return;
		} else {
			img_icon.visible = true;
			img_rank.visible = true;
			l_name.visible = true;
			go.asCom.GetChild("n3").visible = true;
			l_rank.visible = true;
			l_job.visible = true;
			l_guildCoin.visible = true;
			btn_set.visible = true;
			ranktxt.visible = true;
			pmStart.visible = true;
			btn_go.touchable = true;
		}

		Dictionary<string,object> _data = (Dictionary<string,object>)(lis [index]);

		ranktxt.text = Tools.StartValueTxt ((index + 1));//.ToString ();
		if (guildModel.my_guild_job == 3 || ModelManager.inst.userModel.uid == _data ["id"].ToString () || guildModel.my_guild_job >= (int)_data ["gradation"])
		{
			btn_set.visible = false;
		}
		else
		{
			btn_set.visible = true;
		}
		pmStart.url = Tools.GetResourceUrl ("Image2:n_icon_paiming4");
		ranktxt.strokeColor = Tools.GetColor ("426600");
		switch ((index + 1)) {
		case 1:
			ranktxt.strokeColor = Tools.GetColor ("9b5c04");
			pmStart.url = Tools.GetResourceUrl ("Image2:n_icon_paiming1");
			break;
		case 2:
			ranktxt.strokeColor = Tools.GetColor ("4b4b4b");
			pmStart.url = Tools.GetResourceUrl ("Image2:n_icon_paiming2");
			break;
		case 3:
			ranktxt.strokeColor = Tools.GetColor ("853c1d");
			pmStart.url = Tools.GetResourceUrl ("Image2:n_icon_paiming3");
			break;
		}
        
        if (ModelManager.inst.userModel.uid == _data ["id"].ToString ()) {
			btn_go.GetChild ("n2").asLoader.url = Tools.GetResourceUrl ("Image2:n_bg_tanban6_");
			btn_go.alpha = 1;
		} else {
			
		}
		l_rank.text = _data [Config.ASSET_RANKSCORE].ToString ();
		img_rank.url = ModelManager.inst.userModel.GetRankImg ((int)_data [Config.ASSET_RANKSCORE]);
//		img_icon.url = ModelUser.GetHeadUrl (((Dictionary<string,object>)_data ["head"]) ["use"] as string);
		Tools.SetLoaderButtonUrl(img_icon, ModelUser.GetHeadUrl (((Dictionary<string,object>)_data ["head"]) ["use"] as string));


		if (_data ["uname"] == null)
		{
			l_name.text = _data ["id"].ToString ();
		}
		else
		{
			l_name.text = _data ["uname"].ToString ();
		}

		l_level.text = _data ["lv"].ToString ();
		l_job.text = guildModel.getJob ((int)_data ["gradation"]);
		l_guildCoin.text = Tools.GetMessageColor (Tools.GetMessageById ("20124") + "[0]" + _data ["contribute"].ToString () + "[/0]", new string[]{ "e08002" });

		btn_set.RemoveEventListeners ();
		btn_set.onClick.Add (() =>
		{
			MediatorSetGradation.uid = _data ["id"].ToString ();
			removegg ();

			Vector2 v2 = Stage.inst.touchPosition;
			v2.x = Tools.offectSetX(v2.x);
//			Vector3 v3 = new Vector3 ((list.x * Tools.GetStageScale ().x) + 860, (list.y * Tools.GetStageScale ().y) + go.y + list.container.y + 134, list.z);
			MediatorSetGradation bbb = ViewManager.inst.ShowView<MediatorSetGradation>(false) as MediatorSetGradation;
			v2 = this.parent.GlobalToLocal(v2);
//			bbb.view.Center= new Vector2(0f,0f);
			bbb.group.x = v2.x-bbb.group.width-15;
			bbb.group.y = v2.y-bbb.group.height/2 + 15;
//			bbb.group.x = 0f;
//			bbb.group.y = 0f;
		});

		btn_go.RemoveEventListeners ();
		btn_go.onClick.Add (() =>
		{
			Dictionary<string, object> dd = new Dictionary<string, object> ();
			dd ["fuid"] = _data ["id"];
//			roleModel.tab_Role_Select2 = 1;
			NetHttp.inst.Send (NetBase.HTTP_FUSERGET, dd, (VoHttp vo) =>
			{
						this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] {null,_data["id"], ModelManager.inst.roleModel.tab_CurSelect1, ModelManager.inst.roleModel.tab_CurSelect2, ModelManager.inst.roleModel.tab_CurSelect3 }));
            });
		});
	}

	private void removegg ()
	{
		if (gg != null)
		{
			Tools.Clear (gg);
			gg = null;
		}
	}

	public override void Clear ()
	{
		this.RemoveGlobalListener (MainEvent.CHAT_GUILDJOIN, CHAT_GUILDJOIN);
		this.RemoveGlobalListener (MainEvent.CHAT_GUILDMODIFY, CHAT_GUILDMODIFY);
		base.Clear ();
	}
}
