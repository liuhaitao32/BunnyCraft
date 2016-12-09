using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorFightWorld:BaseMediator
{
	private ModelFight fightModel;

	private GButton button;
    private ModelRole roleModel;

    public MediatorFightWorld ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_FIGHTWORLD);

		fightModel = ModelManager.inst.fightModel;
		roleModel = ModelManager.inst.roleModel;

        button = this.GetChild ("n3").asButton;
		button.onClick.Add (Result_Click);

		button.visible = fightModel.isLeader;

		NetSocket.inst.AddListener (NetBase.SOCKET_GETRESUILTTEAMPUSH, (VoSocket vo) =>
		{
			NetSocket.inst.RemoveListener (NetBase.SOCKET_GETRESUILTTEAMPUSH);

			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["data"] = vo.data;
			data ["type"] = fightModel.fightType;
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_RESULT, data));
		});

//		NetSocket.inst.AddListener (NetBase.SOCKET_GETRESUILTPUSH, (VoSocket vo) =>
//		{
//			NetSocket.inst.RemoveListener (NetBase.SOCKET_GETRESUILTPUSH);
//
//			Dictionary<string,object> data = new Dictionary<string, object> ();
//			data ["data"] = vo.data;
//			data ["type"] = fightModel.fightType;
//			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_RESULT, data));
//		});

		NetSocket.inst.AddListener (NetBase.SOCKET_GETRESULTFREEMATCHPUSH, (VoSocket vo) =>
		{
			NetSocket.inst.RemoveListener (NetBase.SOCKET_GETRESULTFREEMATCHPUSH);

			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["data"] = vo.data;
			data ["type"] = fightModel.fightType;
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.FIGHT_RESULT, data));
		});
        roleModel.ClearFightData();


    }

	public override void Clear ()
	{
		base.Clear ();
	}

	private void Result_Click ()
	{
		if (fightModel.fightType == ModelFight.FIGHT_MATCHTEAM)
		{			
			NetSocket.inst.Send (NetBase.SOCKET_GETRESUILTTEAM, null);
		}
		else if (fightModel.fightType == ModelFight.FIGHT_MATCH)
		{			
			NetSocket.inst.Send (NetBase.SOCKET_GETRESUILT, null);
		}
		else
		{			
			NetSocket.inst.Send (NetBase.SOCKET_GETRESULTFREEMATCH, null);
		}
	}
}