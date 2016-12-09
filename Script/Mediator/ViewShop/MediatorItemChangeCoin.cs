using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;

public class MediatorItemChangeCoin : BaseMediator {
	private object[] gold_buy = (object[])DataManager.inst.systemSimple ["coin_buy_gold"];
	private GList list1;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_ITEMCHANGECOIN);
		this.x = 0;
		this.y = 84;
		List<object> list = new List<object> ();

		for (int i = 0; i < gold_buy.GetLength (0); i++) {
			list.Add (gold_buy [i]);
		}

		list1 = this.GetChild ("n0").asList;
//		list1.width = MediatorShop.thiswidth;
		list1.itemRenderer = List_Render1;
		list1.SetVirtual ();
		list1.numItems = list.Count+2;	
		if (list.Count < 4) {
//			list1.x = view.width / 2 - (246 / 2 * list.Count) +20;
			list1.scrollPane.touchEffect = false;
		}
	}
	private void List_Render1 (int _index,GObject go)
	{
		int index = _index - 1;
		if (_index == 0 || _index == list1.numItems - 1) {
			go.width = 10;//(MediatorShop.thiswidth - 960) / 2;
			go.alpha = 0;
			return;
		} else {
			go.width = 190;
			go.alpha = 1;
		}
		GButton btn = go.asButton;//.GetChild ("n0").asButton;
		btn.RemoveEventListeners ();
		btn.onClick.Add(() =>
			{
				this.onListItemClick (index);
			});
		GTextField name = go.asCom.GetChild ("n8").asTextField;
//		ComBigIcon card = go.asCom.GetChild("n10") as ComBigIcon;
		GLoader card = go.asCom.GetChild("n15").asLoader;

//		card.SetData (Config.ASSET_GOLD, ((object[])(gold_buy [index])) [1], 0, "Image2:" + ((object[])(gold_buy [index])) [2].ToString ());
//		card.SetSelectIndex (2);
//		card.GetChild ("n2").visible = false;
//		card.GetChild ("n15").visible = false;
		card.url = Tools.GetResourceUrl ("Image2:" + ((object[])(gold_buy [index])) [2].ToString ());
		go.asCom.GetChild ("n14").asTextField.text = "x" + ((object[])(gold_buy [index])) [1];//card.GetChild ("n15").asTextField.text;
		GTextField set_num = go.asCom.GetChild ("n6").asTextField;
		name.text = Tools.GetMessageById (((object[])(gold_buy [index])) [3].ToString ());
		set_num.text = ((object[])(gold_buy [index]))[0].ToString();
	}
	private void onListItemClick(int index)
	{
		MediatorGoldBuy._index = index;
		ViewManager.inst.ShowView<MediatorGoldBuy> ();
	}
	public override void Clear ()
	{
		base.Clear ();
	}
}
