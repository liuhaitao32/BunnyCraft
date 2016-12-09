using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using FairyGUI;
using System;

public class MediatorItemGuildInList : BaseMediator {
	private GList list;
	private List<object> lisData;

	private Controller c1;
	public override void Init()
	{
		base.Init ();
		this.Create (Config.VIEW_GUILDINLIST);

//		view.GetChild ("n2").asTextField.text = Tools.GetMessageById ("31072");
//		view.GetChild ("n3").asTextField.text = Tools.GetMessageById ("20125");
//		view.GetChild ("n4").asTextField.text = Tools.GetMessageById ("20126");
		this.GetChild ("n6").asButton.text = Tools.GetMessageById ("19926");
		this.GetChild ("n7").asButton.text = Tools.GetMessageById ("19927");
		list = this.GetChild ("n0").asList;
		list.itemRenderer = ListRander;
		list.SetVirtual ();
		c1 = this.GetController ("c1");
		c1.onChanged.Add (OnChangeC1);
		this.visible = false;
		c1.selectedIndex = 1;
//		NetHttp.inst.Send (NetBase.HTTP_GUILD_RANK, "", GetGuildIndex);
	}
	private void OnChangeC1()
	{
		pagindex = 1;
		if (c1.selectedIndex == 0) {
            //			if (this.visible) 
            //				ViewManager.inst.ShowText (Tools.GetMessageById("20166"));
            //this.GetChild("n6").asButton.GetChild("title").asTextField.strokeColor = Tools.GetColor("E08928");
            //this.GetChild("n7").asButton.GetChild("title").asTextField.strokeColor = Tools.GetColor("D36C7F");
            Tools.SetRootTabTitle(this.GetChild("n7").asButton.GetChild("title").asTextField, "", 30, "ffffff", "#E08928", new Vector2(0, 0), 2);
            Tools.SetRootTabTitle(this.GetChild("n6").asButton.GetChild("title").asTextField, "", 30, "ffffff", "#D36C7F", new Vector2(0, 0), 2);
            NetHttp.inst.Send (NetBase.HTTP_GUILD_RANK, "", GetGuildIndex);
		} else {
//			if (this.visible) 
//				ViewManager.inst.ShowText (Tools.GetMessageById("20167"));
			//this.GetChild("n6").asButton.GetChild("title").asTextField.strokeColor = Tools.GetColor("D36C7F");
			//this.GetChild("n7").asButton.GetChild("title").asTextField.strokeColor = Tools.GetColor("E08928");
            Tools.SetRootTabTitle(this.GetChild("n7").asButton.GetChild("title").asTextField, "", 30, "ffffff", "#D36C7F", new Vector2(0, 0), 2);
            Tools.SetRootTabTitle(this.GetChild("n6").asButton.GetChild("title").asTextField, "", 30, "ffffff", "#E08928", new Vector2(0, 0), 2);

            NetHttp.inst.Send (NetBase.HTTP_GUILD_LOCATION_RANK, "", GetGuildIndex);
		}
	}
	private void GetGuildIndex(VoHttp vo)
	{
//		Debug.LogError ("GetGuildIndex");
		this.visible = true;
		ModelManager.inst.guildModel.guildList = null;
		ModelManager.inst.guildModel.SetGuildList (((object[])vo.data));
		if (((object[])vo.data).GetLength (0) == 10) {
			have10 = true;
		} else {
			have10 = false;
		}

		lisData = new List<object> ();
		for (int i = 0; i < ((object[])vo.data).GetLength (0); i++)
		{
			lisData.Add (((object[])vo.data) [i]);
		}
		if (have10) {
			lisData.Add (true);
		}
		have10 = false;
		if (lisData.Count > 5) {
			list.numItems = lisData.Count;
		} else {
			list.numItems = 5;
		}
		if (lisData.Count <= 5) {
			list.scrollPane.touchEffect = false;
		}
		list.ScrollToView (0);
	}
	//分割线//////////////////////////////////////////////////////////////////////////////////////
	private int pagindex = 1;
	private bool have10 = false;
	private void ListRander(int index,GObject go)
	{
		GButton btn = go.asCom.GetChild ("n0").asButton;
		if (index == 0 || index % 2 == 0) {
			btn.alpha = 0;
		} else {
			btn.alpha = 1;
		}
//		L_Button btn_btn = Tools.FindChild<L_Button> (go, "btn_btn");
		GTextField l_mc = go.asCom.GetChild("n18").asCom.GetChild("n4").asTextField;
		GLoader img_icon = go.asCom.GetChild ("n8").asLoader;
		GTextField l_guildName = go.asCom.GetChild ("n2").asTextField;
//		GTextField l_guildID = go.asCom.GetChild ("n3").asTextField;
		GTextField l_guildNum = go.asCom.GetChild ("n4").asTextField;
		GTextField l_guildCoin = go.asCom.GetChild ("n15").asCom.GetChild ("n2").asTextField;
		GButton btn_more = go.asCom.GetChild ("n7").asButton;

		GLoader pmUpdata = go.asCom.GetChild ("n16").asLoader;
		GTextField pmText = go.asCom.GetChild ("n17").asTextField;
		GLoader pmStart = go.asCom.GetChild ("n18").asCom.GetChild("n1").asLoader;
//		GTextField no = go.asCom.GetChild ("n19").asTextField;
		if (index > lisData.Count - 1) {
			l_mc.visible = false;
			img_icon.visible = false;
			l_guildName.visible = false;
			l_guildNum.visible = false;
			go.asCom.GetChild ("n15").visible = false;
			btn_more.visible = false;
			pmUpdata.visible = false;
			pmText.visible = false;
			pmStart.visible = false;
			btn.touchable = false;
			return;
		} else {
			l_mc.visible = true;
			img_icon.visible = true;
			l_guildName.visible = true;
			l_guildNum.visible = true;
			go.asCom.GetChild ("n15").visible = true;
			btn_more.visible = true;
			pmUpdata.visible = true;
			pmText.visible = true;
			pmStart.visible = true;
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
//		btn_btn.onClick.RemoveAllListeners ();
		btn_more.RemoveEventListeners ();
		btn.visible = true;
		if (lisData[index] is bool) {
			btn.visible = false;
//			btn_btn.gameObject.SetActive (false);
			l_mc.visible = false;
			img_icon.visible = false;
			l_guildName.visible = false;
//			l_guildID.visible = false;
			l_guildNum.visible = false;
			l_guildCoin.visible = false;
			pmUpdata.visible = false;
			pmText.visible = false;
			pmStart.visible = false;
			go.asCom.GetChild ("n15").visible = false;
//			no.visible = false;
			btn_more.visible = true;
			btn_more.onClick.Add (() => {
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

//		btn_btn.gameObject.SetActive (true);
		l_mc.visible = true;
		img_icon.visible = true;
		l_guildName.visible = true;
//		l_guildID.visible = true;
		l_guildNum.visible = true;
		l_guildCoin.visible = true;
		pmUpdata.visible = true;
		pmText.visible = true;
		pmStart.visible = true;
		go.asCom.GetChild ("n15").visible = true;
//		no.visible = true;
		btn_more.visible = false;
		Dictionary<string,object> _da = (Dictionary<string,object>)lisData[index];
		if ((_da ["id"]).ToString () == ModelManager.inst.guildModel.my_guild_info ["id"].ToString ()) {
			btn.GetChild ("n2").asLoader.url = Tools.GetResourceUrl ("Image2:n_bg_tanban6_");
			btn.alpha = 1;
		} else {
			btn.GetChild ("n2").asLoader.url = Tools.GetResourceUrl ("Image2:n_bg_tanban6");
		}
		btn_more.text = Tools.GetMessageById ("20183");
		l_mc.text = Tools.StartValueTxt (index + 1);//.ToString ();//liuhaitao change
		img_icon.url = Tools.GetResourceUrl ("Icon:" + (string)(_da ["icon"]));
//		Tools.SetLoaderButtonUrl(img_icon, ModelUser.GetHeadUrl ((string)(_da ["icon"])));
		l_guildName.text = (string)(_da ["name"]);
//		l_guildID.text = Tools.GetMessageById("20102",new string[]{(_da ["id"]).ToString()});
		l_guildNum.text = Tools.GetMessageById ("20103",new string[]{(_da ["member_count"]).ToString () + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString ()});// + "[color=#ff9900]" + (_da ["member_count"]).ToString () + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString () + "[/color]";
		l_guildCoin.text = _da ["score"].ToString ();
		if ((int)(_da ["rank_diff"]) == 0) {
			pmUpdata.url = Tools.GetResourceUrl("Image:icon_bubian");
			pmText.text = "";
		} else {
			if ((int)(_da ["rank_diff"]) > 0) {
				pmUpdata.url = Tools.GetResourceUrl("Image:icon_up");
				pmText.color = Tools.GetColor ("#4DB10C");
			} else {
				pmUpdata.url = Tools.GetResourceUrl("Image:icon_down");
				pmText.color = Tools.GetColor ("#D23823");
			}
			pmText.text = Math.Abs ((int)(_da ["rank_diff"])).ToString ();
		}

		btn.onClick.Add (() => {
			NetHttp.inst.Send (NetBase.HTTP_GUILD_INFO, "gid="+(_da ["id"]).ToString(), GetGuildInfo);
		});
	}
	private void errorHttp(VoHttp vo)
	{
		switch (vo.return_code) {
		case "10112":
			break;
		}
	}
	private object curobj;
	private int curIndex;
	private GButton curbtn;
	private void OnGuildShenqing(VoHttp vo)
	{
		if (vo.data is bool) {
			curbtn.text = Tools.GetMessageById ("20106");
			((Dictionary<string,object>)curobj) ["is_apply"] = true;
//			list.numItems = lisData.Count;
		}
	}
	private void OnGuildQuxiao(VoHttp vo)
	{
		if (vo.data is bool) {
			curbtn.text = Tools.GetMessageById ("20105");
			((Dictionary<string,object>)curobj) ["is_apply"] = false;
//			list.numItems = lisData.Count;
		}
	}
	private void GetNewPageRank(VoHttp vo)
	{
		if (((object[])vo.data).GetLength (0) == 10) {
			have10 = true;
		} else {
			have10 = false;
		}
		ModelManager.inst.guildModel.SetGuildList (((object[])vo.data));
		lisData = (List<object>)(Tools.Clone (ModelManager.inst.guildModel.guildList));
		if (have10) {
			lisData.Add (true);
		}
		if (lisData.Count > 5) {
			list.numItems = lisData.Count;
		} else {
			list.numItems = 5;
		}
		if (lisData.Count <= 5) {
			list.scrollPane.touchEffect = false;
		}
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
