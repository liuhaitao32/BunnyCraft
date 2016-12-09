using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using FairyGUI;
using System;

public class MediatorGuildInfo : BaseMediator {

	public static Dictionary<string,object> data;
	public static int type;
	private GTextField l_name;
	private GTextField l_id;
	private GTextField l_number;
//	private GTextField l_coinNum;
	private GTextField l_gonggao;
	private GLoader l_icon;
	private GList list;

	private List<object> lis;
	public override void Init()
	{
		base.Init ();
		this.Create (Config.VIEW_GUILDINFO,false,"");

		l_name = this.GetChild ("n5").asTextField;
		l_id = this.GetChild ("n7").asTextField;
		l_number = this.GetChild ("n6").asTextField;
//		l_coinNum = view.GetChild ("n8").asTextField;
		l_gonggao = this.GetChild ("n2").asTextField;
		l_icon = this.GetChild ("n9").asCom.GetChild("n0").asLoader;
		list = this.GetChild ("n14").asList;

		l_icon.url = Tools.GetResourceUrl ("Icon:"+(string)(data ["icon"]));
		l_name.text = (string)(data ["name"]);
		l_id.text = Tools.GetMessageById ("20102");
		this.GetChild ("n18").asTextField.text = (data ["id"]).ToString ();
		l_number.text = Tools.GetMessageById ("20103",new string[] {"" });
		this.GetChild ("n19").asTextField.text = ((Dictionary<string,object>)(data ["member"])).Count + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString ();
//		l_coinNum.text = Tools.GetMessageById ("20104");
		this.GetChild ("n21").asCom.GetChild("n2").asTextField.text = (data ["score"]).ToString ();
		l_gonggao.text = Tools.GetMessageById ("20115", new string[]{ (data ["content"]).ToString()});
		object[] ddd = ((object[])((Dictionary<string,object>)DataManager.inst.systemSimple ["society_location"]) [data ["location"].ToString() + ""]);
		this.GetChild ("n4").asTextField.text = Tools.GetMessageById (ddd [1].ToString());

//		view.GetChild ("n10").asTextField.text = Tools.GetMessageById("20132");
//		view.GetChild ("n11").asTextField.text = Tools.GetMessageById("20133");
//		view.GetChild ("n12").asTextField.text = Tools.GetMessageById("20134");
//		view.GetChild ("n13").asTextField.text = Tools.GetMessageById("20135");
		  
		lis = new List<object> ();
		foreach (string i in ((Dictionary<string,object>)data ["member"]).Keys) {
			Dictionary<string,object> da = (Dictionary<string,object>)((Dictionary<string,object>)data ["member"]) [i];
			da.Add ("id", i);
			lis.Add (da);
		}
		list.itemRenderer = List_Render;
		Tools.Sort (lis, new string[]{ "rank_score:int:1", "contribute:int:1", "gradation:int:1", "id:int:1" });
		if (lis.Count > 5) {
			list.numItems = lis.Count;
		} else {
			list.numItems = 5;
		}
		if (lis.Count <= 4) {
			list.scrollPane.touchEffect = false;
		}

	}
	private void List_Render (int index,GObject go)
	{
		
		GButton bb = go.asCom.GetChild ("n0").asButton;
		if (index == 0 || index % 2 == 0) {
			bb.alpha = 0;
		} else {
			bb.alpha = 1;
		}
//		bb.enabled = false;
		GButton img_icon = go.asCom.GetChild ("n1").asButton;
		GLoader img_rank = go.asCom.GetChild ("n7").asCom.GetChild ("n1").asLoader;
		GLoader img_man = go.asCom.GetChild ("n8").asLoader;
		GTextField l_name = go.asCom.GetChild ("n2").asTextField;
		GTextField l_level = go.asCom.GetChild ("n3").asCom.GetChild("n1").asTextField;
		GTextField l_rank = go.asCom.GetChild ("n7").asCom.GetChild ("n2").asTextField;
		GTextField l_job = go.asCom.GetChild ("n5").asTextField;
		GTextField l_guildCoin = go.asCom.GetChild ("n6").asTextField;
		GTextField n9 = go.asCom.GetChild ("n9").asTextField;
		GLoader n10 = go.asCom.GetChild ("n10").asLoader;
		if (index > lis.Count - 1) {
			img_icon.visible = false;
			l_name.visible = false;
			go.asCom.GetChild ("n3").visible = false;
			l_job.visible = false;
			l_guildCoin.visible = false;
			go.asCom.GetChild ("n7").visible = false;
			n9.visible = false;
			img_man.visible = false;
			n10.visible = false;
			bb.touchable = false;
			go.touchable = false;
			return;
		} else {
			img_icon.visible = true;
			l_name.visible = true;
			go.asCom.GetChild ("n3").visible = true;
			l_job.visible = true;
			l_guildCoin.visible = true;
			go.asCom.GetChild ("n7").visible = true;
			n9.visible = true;
			img_man.visible = true;
			n10.visible = true;
			bb.touchable = true;
			go.touchable = true;
		}

		Dictionary<string,object> _data = (Dictionary<string,object>)(lis [index]);
		n9.text = Tools.StartValueTxt (index + 1);//.ToString ();

		n10.url = Tools.GetResourceUrl ("Image2:n_icon_paiming4");
		n9.strokeColor = Tools.GetColor ("426600");
		switch ((index + 1)) {
		case 1:
			n9.strokeColor = Tools.GetColor ("9b5c04");
			n10.url = Tools.GetResourceUrl ("Image2:n_icon_paiming1");
			break;
		case 2:
			n9.strokeColor = Tools.GetColor ("4b4b4b");
			n10.url = Tools.GetResourceUrl ("Image2:n_icon_paiming2");
			break;
		case 3:
			n9.strokeColor = Tools.GetColor ("853c1d");
			n10.url = Tools.GetResourceUrl ("Image2:n_icon_paiming3");
			break;
		}
		if (_data ["uname"] == null) {
			l_name.text = _data ["id"].ToString ();
		} else {
			l_name.text = _data ["uname"].ToString ();
		}
		if (ModelManager.inst.userModel.uid == _data ["id"].ToString ()) {
			bb.GetChild ("n2").asLoader.url = Tools.GetResourceUrl ("Image2:n_bg_tanban6_");
			bb.alpha = 1;
		} else {
			bb.GetChild ("n2").asLoader.url = Tools.GetResourceUrl ("Image2:n_bg_tanban6");
		}
		l_rank.text = _data [Config.ASSET_RANKSCORE].ToString ();
		img_rank.url = ModelManager.inst.userModel.GetRankImg ((int)_data [Config.ASSET_RANKSCORE]);
//		img_icon.url = ModelUser.GetHeadUrl (((Dictionary<string,object>)_data ["head"]) ["use"] as string);
		Tools.SetLoaderButtonUrl(img_icon, ModelUser.GetHeadUrl (((Dictionary<string,object>)_data ["head"]) ["use"] as string));

		l_level.text = _data ["lv"].ToString ();
		l_job.text = ModelManager.inst.guildModel.getJob ((int)_data ["gradation"]);
		l_guildCoin.text = Tools.GetMessageColor (Tools.GetMessageById ("20124") + "[0]" + _data ["contribute"].ToString () + "[/0]", new string[]{ "e08002" });

        img_man.url = Tools.GetSexUrl(_data["sex"]);
		bb.RemoveEventListeners ();
		bb.onClick.Add (() =>
			{
				Dictionary<string, object> dd = new Dictionary<string, object> ();
				dd ["fuid"] = _data ["id"];
				NetHttp.inst.Send (NetBase.HTTP_FUSERGET, dd, (VoHttp vo) =>
					{
						this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { _data["id"], _data["id"], type==1?0:ModelManager.inst.roleModel.tab_CurSelect1, type==1?0:ModelManager.inst.roleModel.tab_CurSelect2, ModelManager.inst.roleModel.tab_CurSelect3 }));

					});
            });
	}
	public override void Clear()
	{
		base.Clear();
		type = 0;
	}
}
