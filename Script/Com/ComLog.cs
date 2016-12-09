using System;
using FairyGUI;

public class ComLog:BaseMediator
{
	private GList list;

	public ComLog ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create ("Base:ComLog");
		list = this.GetChild ("n0").asList;

		list.itemRenderer = Item_Render;
		list.SetVirtual ();
		list.numItems = Log.list.Count;
		if (Log.list.Count != 0)
			list.ScrollToView (Log.list.Count - 1, true);

		this.AddGlobalListener (MainEvent.UPDATE_LOG, UPDATE_LOG);
	}

	public override void Clear ()
	{
		base.Clear ();
	}

	private void Item_Render (int index, GObject item)
	{
		item.asCom.GetChild ("n0").text = index + " - " + Log.list [index].ToString ();
	}

	private void UPDATE_LOG (MainEvent e)
	{
		list.numItems = Log.list.Count;
		list.ScrollToView (Log.list.Count - 1, true);
	}
}