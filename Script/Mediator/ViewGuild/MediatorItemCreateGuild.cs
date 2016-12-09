using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using FairyGUI;
using System;

public class MediatorItemCreateGuild : BaseMediator {

	private GTextInput input;           //公会名称
	private GButton btn_changeIcon; //修改图标
	private GLoader img_icon;          //图标img
	private GButton btn_create;     //创建按钮
	private GTextField l_goldNeed;         //需要钻石数量
	private GComboBox bo_suozaidi;

	private Dictionary<string,object> cfg = (Dictionary<string,object>)(DataManager.inst.guild["society"]);
	private Dictionary<string,object> simCfg;
	public override void Init()
	{
		base.Init ();
		this.Create (Config.VIEW_GUILDCREATE);

		simCfg = DataManager.inst.systemSimple;
		input = this.GetChild ("n3").asTextInput;
		btn_changeIcon = this.GetChild ("n0").asButton;
		img_icon = btn_changeIcon.GetChild ("n0").asLoader;
		btn_create = this.GetChild ("n6").asButton;
		l_goldNeed = this.GetChild ("n7").asTextField;
		bo_suozaidi = this.GetChild ("n10").asComboBox;

		this.GetChild ("n2").asTextField.text = Tools.GetMessageById ("20129");
		this.GetChild ("n9").asTextField.text = Tools.GetMessageById ("20130");
		btn_create.text = Tools.GetMessageById ("20149");

		this.GetChild ("n15").asButton.onClick.Add (OnAddClickInInput);

		Dictionary<string,object> cc = (Dictionary<string,object>) (simCfg ["society_location"]);
		object[] cfgs = (object[])((Dictionary<string,object>)DataManager.inst.guild["society"])["society_icon"];
		MediatorChangeIcon.iconID = cfgs [Tools.GetRandom (0, cfgs.Length - 1)].ToString ();
		img_icon.url = Tools.GetResourceUrl ("Icon:" + MediatorChangeIcon.iconID);
		string[] str = new string[cc.Count];
		string[] va = new string[cc.Count];
		int ii = 0;
		foreach (string i in cc.Keys) {
			str [ii] = Tools.GetMessageById (((object[])cc [i]) [1].ToString ());
			va[ii] = i;
			ii++;
		}
		bo_suozaidi.items = str;
		bo_suozaidi.values = va;
		input.touchable = false;
		l_goldNeed.text = Tools.GetMessageById ("20172", new object[]{ Convert.ToString ((int)cfg ["society_cost"]) });
		input.text = "";
		input.promptText = Tools.GetMessageById ("20150");
		input.onChanged.Add (OnInputChange);
		//input.maxLength = 8;
//		Tools.GetStringLength ();
		btn_changeIcon.onClick.Add(OnChangeIconViewShow);

		btn_create.onClick.Add(GuildCreate);

		DispatchManager.inst.Register (MainEvent.CHANGE_GUILD_ICON, OnChangeIconHandler);
		this.AddGlobalListener (MainEvent.CHAT_GUILDJOIN, CHAT_GUILDJOIN);
	}
	private void CHAT_GUILDJOIN (MainEvent e)
	{
		this.DispatchGlobalEvent (new MainEvent (MainEvent.GUILD_ESC));
	}
	private void OnAddClickInInput()
	{
		input.RequestFocus ();
	}
	private void OnInputChange()
	{

        input.text = Tools.GetStringByLength(input.text, 8);
        input.text = Tools.StrReplace(input.text);
    }
	private void OnChangeIconHandler(MainEvent e)
	{
		if (img_icon != null) {
			img_icon.url = Tools.GetResourceUrl ("Icon:"+MediatorChangeIcon.iconID);
		}
	}
	private void GuildCreate()
	{
		if (!ModelUser.GetCanBuy (Config.ASSET_GOLD, (int)(cfg ["society_cost"]),"20182"))
			return;
		if (input.text != "") {
			if (FilterManager.inst.Exec (input.text).IndexOf ('*') != -1/*||input.text.IndexOf(" ") != -1*/) {
				ViewManager.inst.ShowText (Tools.GetMessageById ("20180"));
				return;
			}
			NetHttp.inst.Send (NetBase.HTTP_GUILD_CREATE, "name=" + input.text + "|icon=" + MediatorChangeIcon.iconID + "|location=" + bo_suozaidi.values [bo_suozaidi.selectedIndex].ToString (), GuildCreateHandler);
		} else {
			ViewManager.inst.ShowText (Tools.GetMessageById ("20152"));
		}
	}
	private void GuildCreateHandler(VoHttp vo)
	{
		ModelManager.inst.userModel.gold = ModelManager.inst.userModel.gold - (int)(cfg ["society_cost"]);
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.USER_UPDATE));
		this.DispatchGlobalEvent(new MainEvent (MainEvent.GUILD_ESC,954));
	}
	private void OnChangeIconViewShow()
	{
		ViewManager.inst.ShowView<MediatorChangeIcon>();
	}
	public override void Clear()
	{
		MediatorChangeIcon.iconID = "g01";
		DispatchManager.inst.Unregister (MainEvent.CHANGE_GUILD_ICON, OnChangeIconHandler);
		base.Clear();
	}
}
