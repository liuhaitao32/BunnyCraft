using UnityEngine;
using System.Collections;
using FairyGUI;

public class ComExp : BaseCom {

	private GTextField l_num;

	public ComExp ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);

		l_num = this.GetChild ("n0").asTextField;
		l_num.text = ModelManager.inst.userModel.exp + "/" + ModelManager.inst.userModel.GetExpMax (ModelManager.inst.userModel.lv);
		DispatchManager.inst.Register (MainEvent.USER_UPDATE, OnFushHandler);
	}

	private void OnFushHandler (MainEvent e)
	{
		UpdataExp ();
	}

	public void UpdataExp ()
	{
		l_num.text = ModelManager.inst.userModel.exp + "/" + ModelManager.inst.userModel.GetExpMax (ModelManager.inst.userModel.lv);
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
