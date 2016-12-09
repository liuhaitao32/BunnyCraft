using System;
using System.Collections.Generic;
using FairyGUI;

public class MediatorInvoteAlert:BaseMediator
{
	private ModelFight fightModel;

	private Dictionary<string,object> data;

	public MediatorInvoteAlert ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_INVITEALERT);
		fightModel = ModelManager.inst.fightModel;
	}

	public override void Clear ()
	{
		base.Clear ();
	}

	public void SetData (Dictionary<string,object> data)
	{
		this.data = data;
		if (data ["type"].ToString () == "0")
			this.GetChild ("n1").text = Tools.GetMessageById ("25006", new String[]{ ModelUser.GetUname (data ["uid"], data ["name"]) });
		else
			this.GetChild ("n1").text = Tools.GetMessageById ("25040", new String[]{ ModelUser.GetUname (data ["uid"], data ["name"]) });				
		this.GetChild ("n2").text = ModelUser.GetUname (data ["uid"], data ["name"]);
		GButton head = this.GetChild ("n3").asButton;
		head.GetChild ("n2").text = data ["lv"].ToString ();
		Tools.SetLoaderButtonUrl (head.GetChild ("n0").asButton, ModelUser.GetHeadUrl (Tools.Analysis (data, "head.use").ToString ()));

		GButton btn1 = this.GetChild ("n4").asButton;
        btn1.text = Tools.GetMessageById("14056");
		GButton btn2 = this.GetChild ("n5").asButton;
        btn2.text = Tools.GetMessageById("14057");
		btn1.onClick.Add (Btn1_Click);
		btn2.onClick.Add (Btn2_Click);
	}

	private void Btn1_Click ()
	{
		Dictionary<string,object> da = new Dictionary<string, object> ();
		if (data ["type"].ToString () == "0")
		{							
			da ["team_id"] = data ["team_id"].ToString ();
			NetSocket.inst.Send (NetBase.SOCKET_ACCEPTINVITE, da);
		}
		else
		{
			da ["room_id"] = data ["room_id"].ToString ();
			NetSocket.inst.Send (NetBase.SOCKET_FREEACCEPTINVITE, da);
		}
		Btn2_Click ();
	}

	private void Btn2_Click ()
	{
		ViewManager.inst.CloseInviteAlert (data ["uid"].ToString ());
	}
}