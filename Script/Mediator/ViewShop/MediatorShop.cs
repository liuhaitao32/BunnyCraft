using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using FairyGUI;
using System;

public class MediatorShop : BaseMediator
{
	private Controller t_title;
	private GComponent item;
	//	private C_Coin c_coin;
	//	private C_Gold c_gold;
	private GButton n2;
	private GButton n3;
	private GButton n7;
//	private ComTabEffect tab;
	public static float thiswidth;

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.SCENE_SHOP);
		GButton btn_close = this.GetChild ("n1").asButton;
		btn_close.onClick.Add (onCloseHandler);
		n2 = this.GetChild ("n13").asCom.GetChild ("bar0").asButton;
		n3 = this.GetChild ("n13").asCom.GetChild ("bar1").asButton;
		n7 = this.GetChild ("n13").asCom.GetChild ("bar2").asButton;
//		tab = this.GetChild ("n11") as ComTabEffect;

		n2.text = Tools.GetMessageById ("19442");
		n3.text = Tools.GetMessageById ("19443");
		n7.text = Tools.GetMessageById ("19444");
		Init_LeftTab (new string[]{Tools.GetMessageById("19442"),Tools.GetMessageById("19443"),Tools.GetMessageById("19444")},"n13");

//		this.GetChild ("n6").asButton.text = Tools.GetMessageById ("17008");
		t_title = this.GetController ("c1");
//		t_title.onChanged.Add (Tab_Change);
//		t_title.onChangeTip.Add (Tab_Change_Check);
		tabC2.onChanged.Add(Tab_Change);
		tabC2.onChangeTip.Add (Tab_Change_Check);

		thiswidth = this.width;
		bool isOk = ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_PAY, n7);
//		t_title.changeObj.Add (n7);

//		if (isOk)
//			tab.SetCount (3, 3);
//		else
//			tab.SetCount (2, 3);
		Tab_Change ();
	}

	private void Tab_Change_Check (EventContext data)
	{
		if (Convert.ToInt32 (data.data) == 2)
		{
			ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_PAY, null, true);
		}
	}

	private void Tab_Change ()
	{		
//		tab.SetIndex (t_title.selectedIndex);
		base.OnTabLeftChange ();
		t_title.selectedIndex = tabC2.selectedIndex;
		if (item != null)
		{
			this.RemoveChild (item);
		}
		if (tabC2.selectedIndex != 0 && LocalStore.GetLocal (LocalStore.LOCAL_SHOPRED) == "1")
		{
			ModelManager.inst.userModel.Add_Notice (n2, new Vector2 (130, -10));
		}
		else
		{
			ModelManager.inst.userModel.Remove_Notice (n2);
		}
		if (tabC2.selectedIndex == 0)
		{
			item = new MediatorItemCardGet ().group;
			this.AddChild (item);
			this.GetChild ("n16").asLoader.url = Tools.GetResourceUrl ("Image2:0002");
		}
		else if (tabC2.selectedIndex == 1)
		{
			item = new MediatorItemChangeCoin ().group;
			this.AddChild (item);
			this.GetChild ("n16").asLoader.url = Tools.GetResourceUrl ("Image2:0004");
		}
		else if (tabC2.selectedIndex == 2)
		{			
			item = new MediatorItemPay ().group;
			this.AddChild (item);
			this.GetChild ("n16").asLoader.url = Tools.GetResourceUrl ("Image2:0005");
		}
		else if (tabC2.selectedIndex == 3)
		{
			item = new MediatorShare ().group;
			this.AddChild (item);
		}
		if (item != null)
		{
			item.width = this.width;
			item.x = this.GetChild ("n16").x;
			item.y = n2.y + 45;
		}
	}

	private void onCloseHandler ()
	{
		ViewManager.inst.CloseScene ();
	}

	public override void Clear ()
	{
		base.Clear ();
	}
}
