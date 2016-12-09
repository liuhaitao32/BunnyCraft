using System;
using FairyGUI;
using System.Collections.Generic;

public class ComIcon:BaseCom
{
	private GLoader loader;
	private GTextField txt;
	private GLoader side;

	public ComIcon ()
	{
	}


	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);

		loader = this.GetChild ("n2").asLoader;
		txt = this.GetChild ("n3").asTextField;
		side = this.GetChild ("n0").asLoader;
	}

	//	public override void Init ()
	//	{
	//		base.Init ();
	//		this.Create (Config.COM_ICON);
	//
	//		loader = view.GetChild ("n1").asLoader;
	//		text = view.GetChild ("n3").asTextField;
	//	}

	public override void Clear ()
	{
		base.Clear ();
	}

	private void CheckSide (string name)
	{
		if (name.StartsWith ("C"))
		{
			Dictionary<string, object> o = (Dictionary<string, object>)DataManager.inst.card [name];
			side.url = Tools.GetResourceUrl ("Image2:k0" + ((int)o ["rarity"]+1).ToString ());
			side.visible = true;
		}
		else
		{
			side.visible = false;
		}
	}

	public void SetData (string name, object count)
	{
		CheckSide (name);
        if (name == Config.ASSET_AWARD)
		{
           
			object[] o = (object[])count;
			loader.url = Tools.GetResourceUrl ("Image2:" + o [1].ToString ());
			txt.text = Tools.GetMessageById (o [0].ToString ());
		}
		else
		{

			loader.url = Tools.GetResourceUrl ("Icon:" + name);
			txt.text = Tools.GetMessageById ("14016", new string[]{ count.ToString () });
		}
	}

	public void SetData (object dic)
	{
		Dictionary<string,object> dd = (Dictionary<string,object>)dic;
		foreach (string n in dd.Keys)
		{			
			CheckSide (n);
			if (n == Config.ASSET_AWARD)
			{
				object[] o = (object[])dd [n];
				loader.url = Tools.GetResourceUrl ("Image2:" + o [1].ToString ());
//				txt.text = Tools.GetMessageById ("14016", new string[]{ Tools.GetMessageById (o [0].ToString ()) });
				txt.text = Tools.GetMessageById (o [0].ToString ());
			}
			else if (n.Contains("ship") || n.Contains("team"))
            {
                loader.url = Tools.GetResourceUrl("Icon:"+n);
                txt.text = Tools.GetMessageById("14016", new string[] { dd[n].ToString() });
            }
            else
			{
				loader.url = Tools.GetIconUrl (n);
				txt.text = Tools.GetMessageById ("14016", new string[]{ dd [n].ToString () });
			}
		}
	}

}