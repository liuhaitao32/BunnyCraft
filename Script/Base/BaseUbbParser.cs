using System;
using FairyGUI.Utils;
using System.Collections.Generic;

public class BaseUbbParser:UBBParser
{
	public static BaseUbbParser instance;
	private Dictionary<string,object> ubb;

	public static new BaseUbbParser inst
	{
		get
		{
			if (instance == null)
				instance = new BaseUbbParser ();			
			return instance;
		}
	}

	public BaseUbbParser () : base ()
	{
//		Dictionary<string,object> names = (Dictionary<string,object>)DataManager.inst.systemSimple ["ubb"];
//		string[] keys;
//		foreach (string n in names.Keys)
//		{
//			keys = names [n].ToString ().Split (',');
//			this.handlers [n] = (string tagName, bool end, string attr) =>
//			{
//				if (keys [1].ToString () != "0" && keys [2].ToString () != "0")
//					return "<img src=\"" + Tools.GetResourceUrl (keys [0].ToString ()) + "\" width=\"" + keys [1] + "\" height=\"" + keys [2] + "\"/>";
//				else
//					return "<img src=\"" + Tools.GetResourceUrl (keys [0].ToString ()) + "\"/>";
//			};
//		}
		ubb = (Dictionary<string,object>)DataManager.inst.systemSimple ["ubb"];
		this.handlers ["name"] = (string tagName, bool end, string attr) =>
		{
			return GetUbbString ("name");
		};
		this.handlers ["coin"] = (string tagName, bool end, string attr) =>
		{
			return GetUbbString ("coin");
		};
        this.handlers["fenge"] = (string tagName, bool end, string attr) =>
        {
            return GetUbbString("fenge");
        };
		this.handlers["gold2"] = (string tagName, bool end, string attr) =>
		{
			return GetUbbString("gold2");
		};
		this.handlers["card02"] = (string tagName, bool end, string attr) =>
		{
			return GetUbbString("card02");
		};
		this.handlers["card12"] = (string tagName, bool end, string attr) =>
		{
			return GetUbbString("card12");
		};
		this.handlers["card22"] = (string tagName, bool end, string attr) =>
		{
			return GetUbbString("card22");
		};
		this.handlers["card32"] = (string tagName, bool end, string attr) =>
		{
			return GetUbbString("card32");
		};
		this.handlers["coin2"] = (string tagName, bool end, string attr) =>
		{
			return GetUbbString("coin2");
		};
    }

	private string GetUbbString (string name)
	{		
		string[] keys = ubb [name].ToString ().Split (',');
		if (keys [1].ToString () != "0" && keys [2].ToString () != "0")
			return "<img src=\"" + Tools.GetResourceUrl (keys [0].ToString ()) + "\" width=\"" + keys [1] + "\" height=\"" + keys [2] + "\"/>";
		else
			return "<img src=\"" + Tools.GetResourceUrl (keys [0].ToString ()) + "\"/>";
	}


	//	protected string onTag_IMG(string tagName, bool end, string attr)
	//	{
	//		if (!end)
	//		{
	//			string src = GetTagText(true);
	//			if (src == null || src.Length == 0)
	//				return null;
	//
	//			if (defaultImgWidth != 0)
	//				return "<img src=\"" + src + "\" width=\"" + defaultImgWidth + "\" height=\"" + defaultImgHeight + "\"/>";
	//			else
	//				return "<img src=\"" + src + "\"/>";
	//		}
	//		else
	//			return null;
	//	}
}