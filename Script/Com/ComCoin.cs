using UnityEngine;
using System.Collections;
using FairyGUI;

public class ComCoin : BaseCom
{
	private GTextField l_num;

	public ComCoin ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{		
		base.ConstructFromXML (xml);
		l_num = this.GetChild ("n0").asTextField;
		l_num.text = ModelManager.inst.userModel.coin + "";
		DispatchManager.inst.Register (MainEvent.USER_UPDATE, OnFushHandler);
		ViewManager.inst.AddTouchTip (Config.TOUCH_COMCOIN, this, Config.TOUCH_COMCOIN);
	}

	private void OnFushHandler (MainEvent e)
	{
		UpdataCoin ();
	}

	public void UpdataCoin ()
	{
		l_num.text = ModelManager.inst.userModel.coin + "";
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
