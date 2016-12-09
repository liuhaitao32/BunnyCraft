using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using FairyGUI;

public class MediatorItemSetUp : BaseMediator {

	private GTextField l_name;
	private GLoader img_icon;
	private GButton btn_changeName;
	private GButton btn_changeGG;
	private GButton btn_changeIcon;
	private GButton btn_openZD;
	private GTextInput input_GG;
	private ComNumeric dro_level;
	private ComNumeric dro_achLevel;
	private ComNumeric dro_rank;
	private GComboBox bo_suozaidi;
	private GButton btn_changeGG2;

	private string level = "0";
	private string achlevel = "0";
	private string rank = "0";
	private string type = "1";

	private string contentText = "";
	private string icon;

	private Dictionary<string,object> attrs;
	private Dictionary<string,object> simCfg;
	private Dictionary<string,object> cfg;
	public override void Init()
	{
		base.Init ();
		this.Create (Config.VIEW_GUILDSETUP,false,Tools.GetMessageById ("19919"));

		Dictionary<string,object> data = (Dictionary<string,object>)(Tools.Clone(ModelManager.inst.guildModel.my_guild_info));
		cfg = (Dictionary<string,object>)(DataManager.inst.guild["society"]);
		simCfg = DataManager.inst.systemSimple;

		btn_changeIcon = this.GetChild ("n0").asButton;
		img_icon = btn_changeIcon.GetChild ("n0").asLoader;
		l_name = this.GetChild ("n3").asTextField;
		btn_changeName = this.GetChild ("n4").asButton;
		btn_changeGG = this.GetChild ("n8").asButton;
		input_GG = this.GetChild ("n30").asTextInput;
		bo_suozaidi = this.GetChild ("n19").asComboBox;
		btn_changeGG2 = this.GetChild ("n36").asButton;

		btn_openZD = this.GetChild ("n24").asButton;
		dro_level = this.GetChild ("n20") as ComNumeric;
		dro_achLevel = this.GetChild ("n21") as ComNumeric;
		dro_rank = this.GetChild ("n22") as ComNumeric;

		this.GetChild ("n2").asTextField.text = Tools.GetMessageById("20129");
		this.GetChild ("n18").asTextField.text = Tools.GetMessageById("20130");
		this.GetChild ("n11").asTextField.text = Tools.GetMessageById("20145");
		this.GetChild ("n15").asTextField.text = Tools.GetMessageById("20146");
		this.GetChild ("n16").asTextField.text = Tools.GetMessageById("20147");
		this.GetChild ("n17").asTextField.text = Tools.GetMessageById("20148");
		this.GetChild ("n5").asTextField.text = Tools.GetMessageById("20128");
		btn_changeGG.text = Tools.GetMessageById("20128");
		btn_changeGG2.text = Tools.GetMessageById ("20175");

		Dictionary<string,object> cc = (Dictionary<string,object>) (simCfg ["society_location"]);

		string[] str = new string[cc.Count];
		string[] va = new string[cc.Count];
		int ii = 0;
		int nowIndex = 0;
		foreach (string i in cc.Keys) {
			if (i == ModelManager.inst.guildModel.location.ToString ()) {
				nowIndex = ii;
			}
			str [ii] = Tools.GetMessageById (((object[])cc [i]) [1].ToString ());
			va[ii] = i;
			ii++;
		}
		bo_suozaidi.items = str;
		bo_suozaidi.values = va;
		bo_suozaidi.selectedIndex = nowIndex;
		bo_suozaidi.onChanged.Add (BoSuozaidiChange);


		img_icon.url = Tools.GetResourceUrl ("Icon:" + (string)(data ["icon"]));
		icon = (string)(data ["icon"]);
		l_name.text = (string)(data ["name"]);
		input_GG.maxLength = (int)cfg ["society_notice"];
		input_GG.text = (data ["content"]).ToString();
		contentText = input_GG.text;
		this.GetChild ("n7").asTextField.text = cfg ["name_change"].ToString ();
		this.GetChild ("n9").asTextField.text = Tools.GetMessageById ("20144");
//		btn_changeName.SetText (Tools.GetMessageById("20113",new string[]{cfg ["name_change"].ToString()}));
//		l_name.onFocusIn.Add(OnNameFocusIn);
//		l_name.onClick.Add(OnNameFocusIn);
		input_GG.onChanged.Add (OnInputContentChange);
//		input_GG.onFocusIn.Add (OnInputFocusIn);
//		input_GG.onFocusOut.Add (OnInputFocusInOut);

		btn_changeName.onClick.Add (OnClickChangeName);
		btn_changeGG.onClick.Add (OnClickChangeGG);
		btn_changeGG2.onClick.Add (OnClickChangeGG2);
		btn_changeIcon.onClick.Add (OnClickChangeIcon);
		btn_openZD.onClick.Add (OnClickOpenZD);
		btn_changeGG.visible = false;



		attrs = (Dictionary<string,object>)((Dictionary<string,object>)data ["attrs"])["join_condition"];
		if (attrs.ContainsKey ("lv")&&attrs["lv"] != null) {
			level = attrs ["lv"].ToString();
		}
		if (attrs.ContainsKey ("effort_lv")&&attrs["effort_lv"] != null) {
			achlevel = attrs ["effort_lv"].ToString();
		}
		if (attrs.ContainsKey ("rank_score")&&attrs["rank_score"] != null) {
			rank = attrs ["rank_score"].ToString();
		}
		if (attrs.ContainsKey ("type")&&attrs["type"] != null) {
			type = attrs ["type"].ToString();
		}
		if (type == "1") {
			btn_openZD.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_17");
			this.GetChild ("n25").asTextField.text = Tools.GetMessageById ("20111");
			this.GetChild ("n25").asTextField.strokeColor = Tools.GetColor ("52b04f");
//			btn_openZD.SetText ();
		} else {
//			btn_openZD.SetText (Tools.GetMessageById("20110"));
			btn_openZD.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_16");
			this.GetChild ("n25").asTextField.text = Tools.GetMessageById ("20110");
			this.GetChild ("n25").asTextField.strokeColor = Tools.GetColor ("d7734d");
		}
		droDownInit ();

		dro_level.OnChange = DroLevelChange;
		dro_achLevel.OnChange = DroAchLevelChange;
		dro_rank.OnChange = DroRankChange;

		this.AddGlobalListener (MainEvent.CHANGE_GUILD_ICON, OnChangeIconHandler);
		this.AddGlobalListener (MainEvent.CHANGE_GUILD_NAME, OnChangeNameHandler);
	}
	private void OnNameFocusIn()
	{
		ViewManager.inst.ShowView<MediatorChangeGuildName> ();
	}
//	private void OnInputFocusIn()
//	{
//		btn_changeGG.visible = false;
//		btn_changeGG2.visible = false;
//	}
//	private void OnInputFocusInOut()
//	{
//		btn_changeGG2.visible = true;
//	}
	private void OnInputContentChange(EventContext str)
	{
		if (contentText != (str.sender as GTextInput).text) {
			btn_changeGG.visible = true;
			btn_changeGG2.visible = false;
		} else {
			btn_changeGG.visible = false;
			btn_changeGG2.visible = true;
		}
	}
	private string funString;
	private void OnClickOpenZD()
	{
		if (type == "1") {
			type = "2";
			btn_openZD.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_16");
			this.GetChild ("n25").asTextField.text = Tools.GetMessageById ("20110");
			this.GetChild ("n25").asTextField.strokeColor = Tools.GetColor ("d7734d");
			ViewManager.inst.ShowText (Tools.GetMessageById ("20164"));
		} else {
			type = "1";
			btn_openZD.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_17");
			this.GetChild ("n25").asTextField.text = Tools.GetMessageById ("20111");
			this.GetChild ("n25").asTextField.strokeColor = Tools.GetColor ("52b04f");
			ViewManager.inst.ShowText (Tools.GetMessageById ("20165"));
		}
		setFun ();
	}
	private void OnClickChangeIcon()
	{
		ViewManager.inst.ShowView<MediatorChangeIcon> ();
	}
	private void OnClickChangeGG2()
	{
		input_GG.RequestFocus ();
	}
	private void OnClickChangeGG()
	{
		NetHttp.inst.Send (NetBase.HTTP_GUILD_SETUP_CHANGE, "content="+input_GG.text, OnChangeGGHandler);
	}
	private void OnClickChangeName()
	{
		if (ModelUser.GetCanBuy (Config.ASSET_GOLD,(int)(((Dictionary<string,object>)(DataManager.inst.guild ["society"]))["name_change"]),"20181")) {
			ViewManager.inst.ShowView<MediatorChangeGuildName> ();
		}
	}


	private void OnChangeNameHandler(MainEvent e)
	{
		ViewManager.inst.ShowText (Tools.GetMessageById("20109"));
		if(l_name != null)
		{
			l_name.text = ModelManager.inst.guildModel.my_guild_info ["name"] as string;
		}
	}
	private void OnChangeIconHandler(MainEvent e)
	{
		if (icon != MediatorChangeIcon.iconID) {
			NetHttp.inst.Send (NetBase.HTTP_GUILD_SETUP_CHANGE, "icon="+MediatorChangeIcon.iconID, OnChangeIconFunction);
		}
	}

	private void OnChangeIconFunction(VoHttp vo)
	{
		if (vo.data is bool) {
			if ((bool)vo.data == true) {
				if (img_icon != null) {
					ViewManager.inst.ShowText (Tools.GetMessageById("20109"));
					img_icon.url = Tools.GetResourceUrl ("Icon:"+MediatorChangeIcon.iconID);
					icon = MediatorChangeIcon.iconID;
					ModelManager.inst.guildModel.my_guild_info ["icon"] = MediatorChangeIcon.iconID;
					this.DispatchGlobalEvent (new MainEvent (MainEvent.GUILDICON_CHANGE));
				}
			}
		}

//		ViewManager.inst.CloseView();
	}
	private void OnChangeGGHandler(VoHttp vo)
	{
		if (vo.data is bool) {
			if ((bool)vo.data == true) {
				contentText = input_GG.text;
				ViewManager.inst.ShowText (Tools.GetMessageById("20109"));
				ModelManager.inst.guildModel.my_guild_info ["content"] = input_GG.text;
				this.DispatchGlobalEvent (new MainEvent (MainEvent.GONGGAO_CHANGE));
				btn_changeGG.visible = false;
				btn_changeGG2.visible = true;
			}
		}
	}
	private void droDownInit()
	{
		int _index = 0;
		//level
		List<string> list_level = new List<string> ();
		string[] ddd = new string[((object[])simCfg ["exp_config"]).Length + 2];
		string[] vvv = new string[((object[])simCfg ["exp_config"]).Length + 2];

		for (int i = 0; i < (((object[])simCfg["exp_config"]).Length+2); i++) {
			vvv [i] = i + "";
			ddd [i] = (i) + "";
			list_level.Add(i+"");

			if (attrs.ContainsKey ("lv")) {
				if ((i + "") == attrs ["lv"].ToString()) {
					_index = i;
				}
			} else {
				_index = 0;
			}
		}
		dro_level.SetMinMax (0, ((object[])simCfg ["exp_config"]).Length + 1);
		dro_level.SetValue ((_index));
//		dro_level.items = ddd;
//		dro_level.values = vvv;
//		dro_level.selectedIndex = _index;
		//achlevel
		_index = 0;
		list_level = new List<string> ();
		ddd = new string[((Dictionary<string,object>)DataManager.inst.effort ["effort_cond"]).Count + 2];
		vvv = new string[((Dictionary<string,object>)DataManager.inst.effort ["effort_cond"]).Count + 2];
		for (int i = 0; i < ((Dictionary<string,object>)DataManager.inst.effort["effort_cond"]).Count+2; i++) {
			list_level.Add((i)+"");
			ddd [i] = (i) + "";
			vvv [i] = i+"";
			if (attrs.ContainsKey ("effort_lv")) {
				if (((i) + "") == attrs ["effort_lv"].ToString()) {
					_index = i;
				}
			} else {
				_index = 0;

			}
		}
		dro_achLevel.SetMinMax (0, ((Dictionary<string,object>)DataManager.inst.effort ["effort_cond"]).Count + 1);
		dro_achLevel.SetValue ((_index));
//		dro_achLevel.items = ddd;
//		dro_achLevel.values = vvv;
//		dro_achLevel.selectedIndex = _index;
		//rank
		_index = 0;
		int ranks = Convert.ToInt32 (Math.Round ((int)cfg ["examine_randscore"] / 100f));
		ddd = new string[ranks+1];
		vvv = new string[ranks+1];
		list_level = new List<string> ();
		for (int i = 0; i < ranks+1; i++) {
			list_level.Add((i*100)+"");
			ddd [i] = (i * 100) + "";
			vvv [i] = i+"";
			if (attrs.ContainsKey ("rank_score")) {
				if (((i*100) + "") == attrs ["rank_score"].ToString()) {
					_index = Convert.ToInt32 (attrs ["rank_score"]);
				}
			} else {
				_index = 0;

			}
		}
		dro_rank.SetMinMax (0, (int)cfg ["examine_randscore"], 100);
		dro_rank.SetValue (_index);
		this.GetChild ("n37").asLoader.url = ModelManager.inst.userModel.GetRankImg (dro_rank.value);
//		dro_rank.items = ddd;
//		dro_rank.values = vvv;
//		dro_rank.selectedIndex = _index;
	}
	private void DroLevelChange()
	{
		level = dro_level.value + "";
		setFun();
	}
	private void DroAchLevelChange()
	{
		achlevel = dro_achLevel.value + "";
		setFun();
	}
	private void DroRankChange()
	{
		rank = dro_rank.value + "";
		this.GetChild ("n37").asLoader.url = ModelManager.inst.userModel.GetRankImg (dro_rank.value);
		setFun();
	}
	private void BoSuozaidiChange()
	{	
		Dictionary<string,object> dd = new Dictionary<string, object> ();
		dd.Add ("location",Convert.ToInt16(bo_suozaidi.values[bo_suozaidi.selectedIndex]));
		NetHttp.inst.Send (NetBase.HTTP_GUILD_SETUP_CHANGE,dd,OnChangeSuozaidi);
	}
	private void setFun()
	{
		funString = "lv=" + level + "|effort_lv=" + achlevel + "|rank_score=" + rank + "|type=" + type;
		NetHttp.inst.Send (NetBase.HTTP_GUILD_JOIN_SETUP, funString, OnChangeFunCtion,'|',onErrorFun);
	}
	private void onErrorFun(VoHttp vo)
	{
		droDownInit ();
	}
	private void OnChangeFunCtion(VoHttp vo)
	{
		Dictionary<string,object> data = ModelManager.inst.guildModel.my_guild_info;
		attrs = (Dictionary<string,object>)((Dictionary<string,object>)data ["attrs"])["join_condition"];
		attrs ["lv"] = Convert.ToInt16 (level);
		attrs ["effort_lv"] = Convert.ToInt16 (achlevel);
		attrs ["rank_score"] = Convert.ToInt16 (rank);
		attrs ["type"] = Convert.ToInt16 (type);
	}
	private void OnChangeSuozaidi(VoHttp vo)
	{
		ModelManager.inst.guildModel.location = Convert.ToInt16 (bo_suozaidi.values [bo_suozaidi.selectedIndex]);
		this.DispatchGlobalEvent (new MainEvent(MainEvent.GUILDDIZHI_CHANGE));
		ViewManager.inst.ShowText (Tools.GetMessageById ("20155"));
	}
	public override void Clear()
	{
		DispatchManager.inst.Unregister (MainEvent.CHANGE_GUILD_NAME, OnChangeNameHandler);
		DispatchManager.inst.Unregister (MainEvent.CHANGE_GUILD_ICON, OnChangeIconHandler);
		base.Clear();
	}
}
