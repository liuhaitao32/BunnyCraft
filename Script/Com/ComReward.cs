using System;
using System.Collections.Generic;
using FairyGUI;

public class ComReward:BaseMediator
{
	private GList list;

	private List<object> ld;

	public ComReward ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.COM_REWARD);

		list = this.GetChild ("n2").asList;
	}

	public override void Clear ()
	{
		base.Clear ();
	}

	private void Item_Render (int index, GObject go)
	{
		ComIcon vi = go.asCom.GetChild ("n0") as ComIcon;
		vi.SetData (ld [index]);
	}

	public void SetData (object data)
	{		
		list.itemRenderer = Item_Render;
		list.SetVirtual ();
		ld = Tools.GetReward (data);
		list.numItems = ld.Count;
	}
}