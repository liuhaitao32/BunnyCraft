using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorMicro:BaseMediator
{
	private ModelFight fightModel;

	private GList list;

	public MediatorMicro ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_MICRO);
		this.x = 0;
		this.y = 0;

		fightModel = ModelManager.inst.fightModel;

//		this.x = -GRoot.inst.width / 2 + this.width / 2;
//		this.y = -GRoot.inst.height / 2 + this.height / 2 + 150;

		list = this.GetChild ("n0").asList;
		list.itemRenderer = Item_Render;
		this.height = fightModel.micros.Count * 50 + (fightModel.micros.Count - 1) * 5;
		this.y = 0;
//		list.visible = false;

		this.AddGlobalListener (MainEvent.MICRO_ADD, MICRO_ADD);
	}

	public override void Clear ()
	{
		base.Clear ();
		fightModel.ClearMicro ();
	}

	private void Item_Render (int index, GObject item)
	{
		Dictionary<string,object> data = (Dictionary<string,object>)fightModel.micros [index];
		GComponent g = item.asCom;
		g.GetChild ("n1").text = ModelUser.GetUname (data ["uid"], data ["name"]);
		g.GetChild ("n3").asButton.onClick.Add (() =>
		{
			if (g.GetChild ("n3").asButton.selected)
				fightModel.AddMicro_Uid (data ["uid"].ToString ());
			else
				fightModel.RemoveMicro_Uid (data ["uid"].ToString ());
		});
	}

	private void MICRO_ADD (MainEvent e)
	{
		list.numItems = fightModel.micros.Count;
		list.visible = list.numItems == 0 ? false : true;
		this.height = fightModel.micros.Count * 50 + (fightModel.micros.Count - 1) * 5;
		this.y = 0;
	}
}