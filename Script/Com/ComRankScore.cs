using System;
using FairyGUI;

public class ComRankScore:BaseCom
{
	private ModelUser userModel;

	private GLoader loader;
	private GTextField text;

	public ComRankScore ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);

		userModel = ModelManager.inst.userModel;

		loader = this.GetChild ("n1").asLoader;
		text = this.GetChild ("n2").asTextField;
	}

	public void SetData (object rankScore, string replaceName = null)
	{
		if (replaceName != null)
			loader.url = Tools.GetResourceUrl (replaceName);
		else
			loader.url = userModel.GetRankImg (Convert.ToInt32 (rankScore));
		text.text = rankScore.ToString ();
	}
}