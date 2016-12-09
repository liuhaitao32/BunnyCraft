using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;

public class MediatorSetGradation : BaseMediator {
	public static string uid;

	private GButton graUp;
	private GButton graDown;
	private GButton outGuild;

	private int num;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_MEMBERSET);

		graUp = this.GetChild ("n3").asButton;
		graDown = this.GetChild ("n4").asButton;
		outGuild = this.GetChild ("n5").asButton;
		graUp.text = Tools.GetMessageById ("20117");
		graDown.text = Tools.GetMessageById ("20118");
		outGuild.text = Tools.GetMessageById ("20119");

		graUp.onClick.Add (OnUpFunction);
		graDown.onClick.Add (OnDownFunction);
		outGuild.onClick.Add (OnOutGuildFunction);

		num = (int)(((Dictionary<string,object>)(ModelManager.inst.guildModel.my_guild_member [uid])) ["gradation"]);
		switch(num)
		{
		case 1:
			switch (ModelManager.inst.guildModel.my_guild_job) {
			case 0:
				graUp.text = Tools.GetMessageById ("20108");
				break;
			case 1:
				graUp.enabled = false;
				graDown.enabled = false;
				outGuild.enabled = false;
				break;
			case 2:
				graUp.enabled = false;
				graDown.enabled = false;
				outGuild.enabled = false;
				break;
			}
			break;
		case 2:
			switch (ModelManager.inst.guildModel.my_guild_job) {
			case 1:
				graUp.enabled = false;
				break;
			case 2:
				graUp.enabled = false;
				graDown.enabled = false;
				outGuild.enabled = false;
				break;
			}
			break;
		case 3:
			graDown.enabled = false;
			switch (ModelManager.inst.guildModel.my_guild_job) {
			case 2:
				graUp.enabled = false;
				break;
			}
			break;
		}
	}
	private int type = -1;//0委任会长1升职2降职3提出公会
	private void OnUpFunction()
	{
		if (num == 1) {
			type = 0;
			ViewManager.inst.ShowAlert(Tools.GetMessageById("20160"),(int bo)=>{
				if(bo == 1)
				{
					num = 0;
					NetHttp.inst.Send (NetBase.HTTP_GUILD_MODIFY, "uid="+uid+"|num="+num, GetGuildInfo);
				}
			},true);
			return;
		} else {
			type = 1;
		}
		if (GetCanUp ()) {
			num--;
			NetHttp.inst.Send (NetBase.HTTP_GUILD_MODIFY, "uid="+uid+"|num="+num, GetGuildInfo);
		}
	}
	private void OnDownFunction()
	{
		type = 2;
		num++;
		NetHttp.inst.Send (NetBase.HTTP_GUILD_MODIFY, "uid="+uid+"|num="+num, GetGuildInfo);
	}
	private void OnOutGuildFunction()
	{
		type = 3;
		num = -1;
		NetHttp.inst.Send (NetBase.HTTP_GUILD_MODIFY, "uid="+uid+"|num="+num, GetGuildInfo);
	}
	private void GetGuildInfo(VoHttp vo)
	{
		switch(type)
		{
		case 0:
			ViewManager.inst.ShowText (Tools.GetMessageById("20156"));
			break;
		case 1:
			ViewManager.inst.ShowText (Tools.GetMessageById("20157"));
			break;
		case 2:
			ViewManager.inst.ShowText (Tools.GetMessageById("20158"));
			break;
		case 3:
			ViewManager.inst.ShowText (Tools.GetMessageById("20159"));
			break;
		}
//		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.GUILD_ESC,1));
		ViewManager.inst.CloseView (this);
	}
	private bool GetCanUp()
	{
		Dictionary<string,object> guild_member = ModelManager.inst.guildModel.my_guild_member;
		Dictionary<string,object> cfg = (Dictionary<string,object>)DataManager.inst.guild ["society"];

		int zhanglaoxz = (int)cfg ["elder"];
		int fuhuizhangxz = (int)cfg ["vice_leadr"];
		int zhanglao = 0;
		int fuhuizhang = 0;
		foreach (string i in guild_member.Keys) {
			Dictionary<string,object> memberdata = (Dictionary<string,object>)(guild_member [i]);
			if (memberdata.ContainsKey ("gradation")) {
				if ((int)memberdata["gradation"] == 1) {
					fuhuizhang++;
				}
				if ((int)memberdata["gradation"] == 2) {
					zhanglao++;
				}
			}
		}
		if (num == 2&&fuhuizhang>=fuhuizhangxz) {
			ViewManager.inst.ShowText (Tools.GetMessageById("20177"));
			return false;
		}
		if (num == 3 && zhanglao >= zhanglaoxz) {
			ViewManager.inst.ShowText (Tools.GetMessageById("20178"));
			return false;
		}
		return true;
	}
	public override void Clear()
	{
		base.Clear();
	}
}
