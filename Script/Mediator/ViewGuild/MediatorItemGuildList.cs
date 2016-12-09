using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using FairyGUI;
using System;

public class MediatorItemGuildList : BaseMediator {
	private GList list;
	private List<object> lisData;
	private List<object> cloneData;
	private Controller c1;

	private GButton btn1;
	private GButton btn2;
	private ModelUser userModel;
	public override void Init()
	{
		base.Init ();
		this.Create (Config.VIEW_GUILDLIST);

		userModel = ModelManager.inst.userModel;

//		view.GetChild ("n2").asTextField.text = Tools.GetMessageById ("31072");
//		view.GetChild ("n3").asTextField.text = Tools.GetMessageById ("20125");
//		view.GetChild ("n4").asTextField.text = Tools.GetMessageById ("20126");
		btn1 = this.GetChild ("n6").asButton;
		btn2 = this.GetChild ("n7").asButton;
		btn1.text = Tools.GetMessageById ("19926");
		btn2.text = Tools.GetMessageById ("19927");
		c1 = this.GetController ("c1");
		c1.onChanged.Add (OnChangeC1);
        GObject bg = this.GetChild("n5");
        BaseMediator.emptyBg = bg;
		list = this.GetChild ("n0").asList;
		list.itemRenderer = ListRander;
        list.emptyStr = Tools.GetMessageById("19936");
        list.onChangeNum.Add(this.CheckListNum);
        list.SetVirtual ();

		this.visible = false;
		c1.selectedIndex = 1;
//		NetHttp.inst.Send (NetBase.HTTP_GUILD_RANK, "", GetGuildIndex);
		this.AddGlobalListener (MainEvent.CHAT_GUILDJOIN, CHAT_GUILDJOIN);
	}
	private void CHAT_GUILDJOIN (MainEvent e)
	{
		this.DispatchGlobalEvent (new MainEvent (MainEvent.GUILD_ESC));
	}
	private void OnChangeC1()
	{
		pagindex = 1;
		if (c1.selectedIndex == 0) {
            //			if (this.visible) 
            //				ViewManager.inst.ShowText (Tools.GetMessageById("20166"));
            Tools.SetRootTabTitle(this.GetChild("n7").asButton.GetChild("title").asTextField, "", 30, "ffffff", "#E08928", new Vector2(0, 0), 2);
            Tools.SetRootTabTitle(this.GetChild("n6").asButton.GetChild("title").asTextField, "", 30, "ffffff", "#D36C7F", new Vector2(0, 0), 2);
            NetHttp.inst.Send (NetBase.HTTP_GUILD_RANK, "", GetGuildIndex);
		} else {
            //			if (this.visible) 
            //				ViewManager.inst.ShowText (Tools.GetMessageById("20167"));
            Tools.SetRootTabTitle(this.GetChild("n7").asButton.GetChild("title").asTextField, "", 30, "ffffff", "#D36C7F", new Vector2(0, 0), 2);
            Tools.SetRootTabTitle(this.GetChild("n6").asButton.GetChild("title").asTextField, "", 30, "ffffff", "#E08928", new Vector2(0, 0), 2);
            NetHttp.inst.Send (NetBase.HTTP_GUILD_LOCATION_RANK, "", GetGuildIndex);
		}
	}
	private void GetGuildIndex(VoHttp vo)
	{
		this.visible = true;
		ModelManager.inst.guildModel.guildList = null;
		ModelManager.inst.guildModel.SetGuildList (((object[])vo.data));
		if (((object[])vo.data).GetLength (0) == 10) {
			have10 = true;
		} else {
			have10 = false;
		}
		cloneData = ModelManager.inst.guildModel.guildList;

		lisData = new List<object> ();
		for (int i = 0; i < ((object[])vo.data).GetLength (0); i++)
		{
			lisData.Add (((object[])vo.data) [i]);
		}
		if (have10) {
			lisData.Add (true);
		}
		have10 = false;
        /*
		if (lisData.Count > 1) {
			list.numItems = lisData.Count;
		} else {
			list.numItems = 0;
		}
		if (lisData.Count <= 5) {
			list.scrollPane.touchEffect = false;
		}
        */
        SetListCSS(list, lisData.ToArray(), 5, true);

		list.ScrollToView (0);
	}
	//分割线//////////////////////////////////////////////////////////////////////////////////////
	private int pagindex = 1;
	private bool have10 = false;
	private void ListRander(int index, GObject go)
	{
		GButton btn = go.asCom.GetChild ("n0").asButton;
		if (index == 0 || index % 2 == 0) {
			btn.alpha = 0;
		} else {
			btn.alpha = 1;
		}
		GButton btn_btn = go.asCom.GetChild ("n13").asButton;
		GTextField l_mc = go.asCom.GetChild("n18").asCom.GetChild("n4").asTextField;
        GLoader img_icon = go.asCom.GetChild ("n3").asLoader;
		GTextField l_guildName = go.asCom.GetChild ("n4").asTextField;
		GTextField l_guildNum = go.asCom.GetChild ("n6").asTextField;
		GButton btn_more = go.asCom.GetChild ("n14").asButton;
		GComponent rankscore = go.asCom.GetChild ("n21").asCom;

		GLoader pmUpdata = go.asCom.GetChild ("n16").asLoader;
		GTextField pmText = go.asCom.GetChild ("n17").asTextField;
		GLoader pmStart = go.asCom.GetChild("n18").asCom.GetChild("n1").asLoader;
        //		GTextField no = go.asCom.GetChild ("n19").asTextField;
        if (index > lisData.Count - 1) {
			l_mc.visible = false;
			img_icon.visible = false;
			l_guildName.visible = false;
			l_guildNum.visible = false;
			rankscore.visible = false;
			btn_more.visible = false;
			pmUpdata.visible = false;
			pmText.visible = false;
			pmStart.visible = false;
			btn_btn.visible = false;
			btn.touchable = false;
			return;
		} else {
			l_mc.visible = true;
			img_icon.visible = true;
			l_guildName.visible = true;
			l_guildNum.visible = true;
			rankscore.visible = true;
			btn_more.visible = true;
			pmUpdata.visible = true;
			pmText.visible = true;
			pmStart.visible = true;
			btn_btn.visible = true;
			btn.touchable = true;
		}


		pmStart.url = Tools.GetResourceUrl ("Image2:n_icon_paiming4");
		l_mc.strokeColor = Tools.GetColor ("426600");
		switch ((index + 1)) {
		case 1:
			l_mc.strokeColor = Tools.GetColor ("9b5c04");
			pmStart.url = Tools.GetResourceUrl ("Image2:n_icon_paiming1");
			break;
		case 2:
			l_mc.strokeColor = Tools.GetColor ("4b4b4b");
			pmStart.url = Tools.GetResourceUrl ("Image2:n_icon_paiming2");
			break;
		case 3:
			l_mc.strokeColor = Tools.GetColor ("853c1d");
			pmStart.url = Tools.GetResourceUrl ("Image2:n_icon_paiming3");
			break;
		}

		btn.RemoveEventListeners ();
		btn_btn.RemoveEventListeners ();
		btn_more.RemoveEventListeners ();
		btn.visible = true;
		if (lisData[index] is bool) {
			btn.visible = false;
			btn_btn.visible = false;
			l_mc.visible = false;
			img_icon.visible = false;
			l_guildName.visible = false;
			l_guildNum.visible = false;
			pmUpdata.visible = false;
			pmText.visible = false;
			pmStart.visible = false;
			rankscore.visible = false;
			btn_more.visible = true;
			btn_more.onClick.Add (()=>{
				pagindex++;
				if (c1.selectedIndex == 0) {
					NetHttp.inst.Send (NetBase.HTTP_GUILD_RANK, "page="+pagindex, GetNewPageRank);
				} else {
					NetHttp.inst.Send (NetBase.HTTP_GUILD_LOCATION_RANK, "page="+pagindex, GetNewPageRank);
				}
			});
			return;
		} else {
			btn.visible = true;
		}

		btn_btn.visible = true;
		l_mc.visible = true;
		img_icon.visible = true;
		l_guildName.visible = true;
		l_guildNum.visible = true;
		pmUpdata.visible = true;
		pmText.visible = true;
		pmStart.visible = true;
		rankscore.visible = true;
		btn_more.visible = false;
		Dictionary<string,object> _da = (Dictionary<string,object>)(lisData[index]);
		btn_more.text = Tools.GetMessageById ("20183");
		btn_btn.GetChild ("title").asTextField.textFormat.size = 25;
		if ((bool)_da ["is_apply"]) {
			btn_btn.text = Tools.GetMessageById ("20106");
//			btn_btn.GetController ("c1").selectedIndex = 1;
			btn_btn.GetChild("title").asTextField.strokeColor = Tools.GetColor("dd8680");
			btn_btn.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_15");
		} else {
			btn_btn.text = Tools.GetMessageById ("20105");
//			btn_btn.GetController ("c1").selectedIndex = 0;
			btn_btn.GetChild("title").asTextField.strokeColor = Tools.GetColor("52b04f");
			btn_btn.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_14");
		}

		l_mc.text = Tools.StartValueTxt ((index + 1));//(index + 1).ToString (); 

		img_icon.url = Tools.GetResourceUrl ("Icon:"+(string)(_da ["icon"]));
//		Tools.SetLoaderButtonUrl(img_icon, ModelUser.GetHeadUrl ((string)(_da ["icon"])));
		l_guildName.text = (string)(_da ["name"]);
		l_guildNum.text = Tools.GetMessageById ("20103", new string[]{ (_da ["member_count"]).ToString () + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString () });//+ "[color=#ff9900]" + (_da ["member_count"]).ToString () + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString () + "[/color]";
		rankscore.GetChild ("n2").asTextField.text = (_da ["score"]).ToString ();
		if ((int)(_da ["rank_diff"]) == 0) {
			pmUpdata.url = Tools.GetResourceUrl("Image:icon_bubian");
			pmText.text = "";
		} else {
			string color = "";
			if ((int)(_da ["rank_diff"]) > 0) {
				color = "54b116";
				pmUpdata.url = Tools.GetResourceUrl("Image:icon_up");
			} else {
				color = "d33623";
				pmUpdata.url = Tools.GetResourceUrl("Image:icon_down");
			}
			pmText.text =Tools.GetMessageColor("[0]" + Math.Abs ((int)(_da ["rank_diff"])).ToString ()+"[/0]",new string[]{color});
		}
        //btn.RemoveEventListeners();
		btn.onClick.Add (() => {
			NetHttp.inst.Send (NetBase.HTTP_GUILD_INFO, "gid="+(_da ["id"]).ToString(), GetGuildInfo);
		});

		btn_btn.onClick.Add(() => {
			if ((bool)_da ["is_apply"]) {
				NetHttp.inst.Send (NetBase.HTTP_GUILD_CANCEL_MEMBER, "gid="+(_da ["id"]).ToString(), OnGuildQuxiao,'|',errorHttp);
			} else {
				if((_da ["member_count"]).ToString() == ((int)(ModelManager.inst.guildModel.getGuildCfg("society")["society_num"])).ToString())
				{
					ViewManager.inst.ShowText(Tools.GetMessageById("20169"));
					return;
				}
				Dictionary<string,object> daAtt = (Dictionary<string,object>)_da["attrs"];
				if(daAtt.ContainsKey("join_condition"))
				{
					Dictionary<string,object> daAtt2 = (Dictionary<string,object>)daAtt["join_condition"];
					if(daAtt2.ContainsKey("type")&&((int)daAtt2["type"] == 2||(int)daAtt2["type"] == 1))
					{
						if(userModel.lv < (int)daAtt2["lv"])
						{
							ViewManager.inst.ShowText(Tools.GetMessageById("20185"));
							return;
						}else if(userModel.effort_lv<(int)daAtt2["effort_lv"])
						{
							ViewManager.inst.ShowText(Tools.GetMessageById("20184"));
							return;
						}
						else if(userModel.rank_score<(int)daAtt2["rank_score"])
						{
							ViewManager.inst.ShowText(Tools.GetMessageById("20186"));
							return;
						}
					}
				}
				NetHttp.inst.Send (NetBase.HTTP_GUILD_APPLY_MEMBER, "gid="+(_da ["id"]).ToString(), OnGuildShenqing,'|',errorHttp);
			}
			curobj = lisData[index];
			curIndex = index;
			curbtn = btn_btn;
		});
	}
	private void errorHttp(VoHttp vo)
	{
		switch (vo.return_code) {
		case "10108":
			this.DispatchGlobalEvent(new MainEvent (MainEvent.GUILD_ESC));
			break;
		}
	}
	private object curobj;
	private int curIndex;
	private GButton curbtn;
	private void OnGuildShenqing(VoHttp vo)
	{
		if (vo.data is bool) {
//			cloneData [curIndex] = true;
			curbtn.GetChild("title").asTextField.strokeColor = Tools.GetColor("dd8680");
			curbtn.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_15");
//			curbtn.GetController ("c1").selectedIndex = 1;
			curbtn.text = Tools.GetMessageById ("20106");
			((Dictionary<string,object>)curobj) ["is_apply"] = true;
		} else {
			this.DispatchGlobalEvent(new MainEvent (MainEvent.GUILD_ESC,100));
		}
	}
	private void OnGuildQuxiao(VoHttp vo)
	{
		if (vo.data is bool) {
//			cloneData [curIndex] = false;
			curbtn.GetChild("title").asTextField.strokeColor = Tools.GetColor("52b04f");
			curbtn.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_14");
//			curbtn.GetController ("c1").selectedIndex = 0;
			curbtn.text = Tools.GetMessageById ("20105");
			((Dictionary<string,object>)curobj) ["is_apply"] = false;
		}
	}
	private void GetNewPageRank(VoHttp vo)
	{
		if (((object[])vo.data).GetLength (0) == 10) {
			have10 = true;
		} else {
			have10 = false;
		}
		int num = ModelManager.inst.guildModel.guildList.Count;
		ModelManager.inst.guildModel.SetGuildList (((object[])vo.data));
		cloneData = ModelManager.inst.guildModel.guildList;

		lisData = (List<object>)(Tools.Clone (cloneData));
		if (have10) {
			lisData.Add (true);
		}
        /*
		if (lisData.Count > 5) {
			list.numItems = lisData.Count;
		} else {
			list.numItems = 5;
		}
		if (lisData.Count <= 5) {
			list.scrollPane.touchEffect = false;
		}
        */
        SetListCSS(list, lisData.ToArray(), 5, true);
		list.ScrollToView ((pagindex - 1) * 10);
	}
	private void GetGuildInfo(VoHttp vo)
	{
		MediatorGuildInfo.data = (Dictionary<string,object>)(vo.data);
		ViewManager.inst.ShowView<MediatorGuildInfo> ();
	}
	//分割线//////////////////////////////////////////////////////////////////////////////////////修改请修改两处
	public override void Clear()
	{
		base.Clear();
	}
}
