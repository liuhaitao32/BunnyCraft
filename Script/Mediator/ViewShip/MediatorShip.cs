using System;
using UnityEngine;
using UnityEngine.UI;
using FairyGUI;

public class MediatorShip:BaseMediator
{
	private ModelCard cardModel;

	private GObject item;
	private Controller c1;
	private ComTabEffect tab;

	private GGroup red;
	private GTextField redTxt;
	private int rednum = 0;
	private int oldRednum;
    private float redPosX;
    private float redPosY;
	public MediatorShip ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.SCENE_SHIP, true, null);
        redPosX = 155;
        redPosY = 0;
		cardModel = ModelManager.inst.cardModel;
		InitTopBar(new string[]{ Tools.GetMessageById ("24109"), Tools.GetMessageById ("24110") });
		c1 = this.GetController ("c1");
		tabC1 = c1;
		c1.onChanged.Add (Tab_Change);
		GButton btn_esc = this.GetChild ("n1").asButton;
		red = this.GetChild ("n12").asGroup;
		redTxt = this.GetChild ("n11").asTextField;
		red.alpha = 0;
//		tab = this.GetChild ("barClip") as ComTabEffect;
		btn_esc.onClick.Add (onCloseHandler);
//		tab.SetCount (2, 3);
		Tab_Change ();
		this.AddGlobalListener (MainEvent.COLOR_CHANGE, OnFunction);
		this.AddGlobalListener (MainEvent.CARD_LEVELUP, OnlevelUp);
		this.AddGlobalListener (MainEvent.JUMP_COINGOLDEXPGET, OnDongHuaFun);
		this.AddGlobalListener (MainEvent.LEVEL_UP_USER, OnUserLevelup);
		OnFunction ();
		oldRednum = (int)ModelManager.inst.userModel.records [Config.ASSET_REDBAGCOIN] < 0 ? 0 : (int)ModelManager.inst.userModel.records [Config.ASSET_REDBAGCOIN];
	}
	private void OnUserLevelup(MainEvent e)
	{
		oldRednum = (int)ModelManager.inst.userModel.records [Config.ASSET_REDBAGCOIN] < 0 ? 0 : (int)ModelManager.inst.userModel.records [Config.ASSET_REDBAGCOIN];
	}
	private void OnlevelUp (MainEvent e)
	{
        red.TweenFade (1f, 0.5f);
//		oldRednum = (int)ModelManager.inst.userModel.records [Config.ASSET_REDBAGCOIN] < 0 ? 0 : (int)ModelManager.inst.userModel.records [Config.ASSET_REDBAGCOIN];
		redTxt.text = Tools.GetMessageById ("12023", new string[]{ oldRednum.ToString () });
		if (GuideManager.inst.Check ("2:0"))//引导 返回到主界面，开始第二场引导战斗
		{
			
			GuideManager.inst.Show (this);
		}
	}

	private void OnDongHuaFun (MainEvent e)
	{
		ComGoldCoinExp.Redcount--;
		oldRednum += Convert.ToInt32 (((object[])e.data) [1]);
		if (ComGoldCoinExp.Redcount <= 0)
		{
			ComGoldCoinExp.Redcount = 0;
			red.TweenFade (0f, 2f);
			oldRednum = (int)ModelManager.inst.userModel.records [Config.ASSET_REDBAGCOIN];
		}
		redTxt.text = Tools.GetMessageById ("12023", new string[]{ oldRednum.ToString () });
	}

	private void OnFunction (MainEvent e = null)
	{
		ModelCard cardModel = ModelManager.inst.cardModel;
		int num = cardModel.GetNewCardNum ();
		int num2 = cardModel.GetLevelUpCardNum ();
		int num3 = ModelManager.inst.cardModel.GetShipNum ();
		if (c1.selectedIndex == 0)
		{
			ModelManager.inst.userModel.Remove_Notice (this.GetChild ("bar0").asCom);
			if (num3 != 0)
			{
				ModelManager.inst.userModel.Add_Notice (this.GetChild ("bar1").asCom, new Vector2 (redPosX,redPosY), num3);
			}
			else
			{
				ModelManager.inst.userModel.Remove_Notice (this.GetChild ("bar1").asCom);
			}
		}
		else
		{
			ModelManager.inst.userModel.Remove_Notice (this.GetChild ("bar1").asCom);
			if (num != 0)
			{
				ModelManager.inst.userModel.Add_Notice (this.GetChild ("bar0").asCom, new Vector2 (redPosX,redPosY), num);
			}
			else if (num2 != 0)
			{
				ModelManager.inst.userModel.Add_Notice (this.GetChild ("bar0").asCom, new Vector2 (redPosX, redPosY), num2, true);
			}
			else
			{
				ModelManager.inst.userModel.Remove_Notice (this.GetChild ("bar0").asCom);
			}
		}

	}

	private void Tab_Change ()
	{
		base.OnTabChange ();
//		tab.SetIndex (c1.selectedIndex);
		OnFunction ();
		this.RemoveChild (item);
		if (c1.selectedIndex == 0)
		{
			float xx = this.GetChild ("bar0").x;
			float yy = this.GetChild ("bar0").y + 62;
			cardModel.v2 = new Vector2 (xx, yy);
			item = new MediatorItemShip ().group;
			this.AddChildAt (item, 1);
		}
		else if (c1.selectedIndex == 1)
		{
			cardModel.v2 = new Vector2 (this.GetChild ("bar0").x, this.GetChild ("bar0").y + 70);
			item = new MediatorItemColor ().group;
//			item.x = cardModel.v2.x;
			item.y = cardModel.v2.y - 10;
			this.AddChildAt (item, 1);
		}
	}

	private void onCloseHandler ()
	{
		ViewManager.inst.CloseScene ();
//		if (GuideManager.inst.Check ("2:0")) {
//
//			Debug.LogError ("");
//		} else if (GuideManager.inst.Check ("2:1")) {
//			Debug.LogError ("");
//		}
	}

	public override void Clear ()
	{
		this.RemoveGlobalListener (MainEvent.COLOR_CHANGE, OnFunction);
		base.Clear ();
	}
}

