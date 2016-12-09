using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using FairyGUI;
using System;

public class MediatorChangeIcon : BaseMediator {

	public static string iconID = "g01";
	private GComponent pack;
	private GButton pl;
	private GButton pr;
	private GTextField pt;

	private List<object> lis;

	private GList list;
	private int pindex = 0;
	private int pmax = 0;
	public override void Init()
	{
		base.Init ();
		this.Create (Config.VIEW_GUILDCHANGEICON,false,Tools.GetMessageById ("20131"));
		pack = this.GetChild ("n3").asCom;
		pl = pack.GetChild ("n4").asButton;
		pr = pack.GetChild ("n5").asButton;
		pt = pack.GetChild ("n0").asTextField;

//		view.GetChild ("n1").asTextField.text = Tools.GetMessageById ("20131");
		object[] cfg = (object[])((Dictionary<string,object>)DataManager.inst.guild["society"])["society_icon"];
		pl.onClick.Add (OnLeftClick);
		pr.onClick.Add (OnRightClick);
		lis = new List<object> ();
		for (int i = 0; i < cfg.Length; i++) {
			lis.Add (cfg [i]);
		}
//		iconID = lis [0].ToString ();
		pmax = Convert.ToInt16 (Math.Floor (lis.Count / 10f) + 1);

		list = this.GetChild ("n2").asList;
		list.itemRenderer = List_Render;
		list.numItems = lis.Count;
		pt.text = (pindex + 1) + "/" + pmax;

//		view.GetChild ("n7").asButton.onClick.Add (() => {
//			ViewManager.inst.CloseView(this);
//		});
	}
	private void List_Render (int index,GObject go)
	{
		GComponent _go = go.asCom;
		string _url;
		_url = lis [index].ToString ();
		if (ModelManager.inst.guildModel.guildHave) {
			if (ModelManager.inst.guildModel.my_guild_info ["icon"].ToString () == _url) {
				_go.GetChild ("n1").visible = true;
			} else {
				_go.GetChild ("n1").visible = false;
			}
		} else {
			if (_url == iconID) {
				_go.GetChild ("n1").visible = true;
			} else {
				_go.GetChild ("n1").visible = false;
			}
		}
		_go.GetChild ("n0").asLoader.url = Tools.GetResourceUrl("Icon:"+_url);
		go.asButton.RemoveEventListeners();
		go.asButton.onClick.Add(() =>{
			iconID = _url;
			ViewManager.inst.CloseView(this);
			this.DispatchGlobalEvent(new MainEvent (MainEvent.CHANGE_GUILD_ICON));
		});
	}
	private void OnLeftClick()
	{
		pindex--;
		if (pindex < 0) {
			pindex = 0;
			return;
		}
		list.container.y = pindex * -360;
		pt.text = (pindex + 1) + "/" + pmax;
	}
	private void OnRightClick()
	{
		pindex++;
		if ((pindex + 1) > pmax) {
			pindex = pmax-1;
			return;
		}
		list.container.y = pindex * -360;
		pt.text = (pindex + 1) + "/" + pmax;
	}
	public override void Clear()
	{
		base.Clear();
	}
}
