using System;
using FairyGUI;
using UnityEngine;
using System.Runtime.CompilerServices;

public class BaseLoader:GLoader
{
	private Loads w;

	public BaseLoader ()
	{
	}

	protected override void LoadExternal ()
	{
//		base.LoadExternal ();
		Texture2D tex = (Texture2D)Resources.Load (this.url, typeof(Texture2D));
		if (tex != null)
			onExternalLoadSuccess (new NTexture (tex));
		else
			w = LoaderManager.inst.Load (this.url, Load_Complete, Load_Error);
	}

	private void Load_Complete (object ws)
	{
		if (this.parent == null)
			return;
		Texture tss = null;
		if (ws is WWW){
			tss = ((ws as WWW).texture);
		} else if(ws is Texture){
			tss = ws as Texture;
		}
		onExternalLoadSuccess (new NTexture (tss));
	}

	private void Load_Error (object ws)
	{
		if (this.parent == null)
			return;
		if (this != null && this.parent != null && this.parent.parent != null)
			this.url = Tools.GetResourceUrl ("Icon:error");
	}

}
