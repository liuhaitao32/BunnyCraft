using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;

public class MediatorChangeGuildName : BaseMediator {

	private GButton L_Button;
	private GTextInput L_Input;
	Dictionary<string,object> cfg;
	public override void Init()
	{
		base.Init ();
		this.Create (Config.VIEW_GUILDCHANGENAME,false,Tools.GetMessageById ("20127"));

		cfg = (Dictionary<string,object>)(DataManager.inst.guild["society"]);
		L_Button = this.GetChild ("n5").asButton;
		L_Input = this.GetChild ("n4").asTextInput;
		L_Input.text = ModelManager.inst.guildModel.my_guild_info ["name"].ToString();
//		L_Input.maxLength = 8;
//		this.GetChild ("n2").asTextField.text = Tools.GetMessageById ("20127");
		L_Input.onChanged.Add(()=>{
			L_Input.text = Tools.StrReplace(L_Input.text);
			L_Input.text = Tools.GetStringByLength(L_Input.text,(int)DataManager.inst.systemSimple["name_num"]);

		});
		this.GetChild ("n6").asTextField.text = Tools.GetMessageById ("20176");

		this.GetChild ("n8").asTextField.text = cfg ["name_change"].ToString ();
		L_Button.onClick.Add (OnBtnClickHandler);
	}
	private void OnBtnClickHandler()
	{
		if (!ModelUser.GetCanBuy (Config.ASSET_GOLD, (int)(((Dictionary<string,object>)(DataManager.inst.guild ["society"])) ["name_change"]),"20181")) {
			return;
		}
		if (ModelManager.inst.guildModel.my_guild_info ["name"].ToString () == L_Input.text) {
			ViewManager.inst.ShowText (Tools.GetMessageById ("20173"));
			return;
		}
		if (L_Input.text != "") {
			if (FilterManager.inst.Exec (L_Input.text).IndexOf ('*') != -1) {
				ViewManager.inst.ShowText (Tools.GetMessageById ("20180"));
				return;
			}
			NetHttp.inst.Send (NetBase.HTTP_GUILD_SETUP_CHANGE, "name="+L_Input.text, OnChangeNameFunction);
		}
	}
	private void OnChangeNameFunction(VoHttp vo)
	{
		if (vo.data is bool) {
			if ((bool)vo.data == true) {
				ModelManager.inst.userModel.gold = ModelManager.inst.userModel.gold - (int)cfg ["name_change"];
				ModelManager.inst.guildModel.my_guild_info ["name"] = L_Input.text;
				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CHANGE_GUILD_NAME));
				ViewManager.inst.CloseView (this);
			}
		}
	}
	public override void Clear()
	{
		base.Clear();
	}
}
