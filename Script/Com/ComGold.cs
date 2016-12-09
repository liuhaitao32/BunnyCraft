using UnityEngine;
using System.Collections;
using FairyGUI;

public class ComGold : BaseCom
{

	private GTextField l_num;

	public ComGold ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);

		l_num = this.GetChild ("n0").asTextField;
		l_num.text = ModelManager.inst.userModel.gold + "";
		DispatchManager.inst.Register (MainEvent.USER_UPDATE, OnFushHandler);
		ViewManager.inst.AddTouchTip (Config.TOUCH_COMGOLD, this, Config.TOUCH_COMGOLD);
	}

	private void OnFushHandler (MainEvent e)
	{
		UpdataGold ();
	}

	public void UpdataGold ()
	{
		l_num.text = ModelManager.inst.userModel.gold + "";
	}

	public void RemoveListener ()
	{
		DispatchManager.inst.Unregister (MainEvent.USER_UPDATE, OnFushHandler);
	}

	public override void Clear ()
	{
		DispatchManager.inst.Unregister (MainEvent.USER_UPDATE, OnFushHandler);
		base.Clear ();
	}
}
