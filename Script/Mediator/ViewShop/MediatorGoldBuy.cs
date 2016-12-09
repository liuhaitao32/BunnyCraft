using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FairyGUI;

public class MediatorGoldBuy : BaseMediator {//兑换小面板

	public static int _index;
	private object[] gold_buy = (object[])DataManager.inst.systemSimple ["coin_buy_gold"];
	private object[] da;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_GOLDBUY);

		da = (object[])gold_buy [_index];

		GTextField l_coin = this.GetChild ("n3").asTextField;
		GTextField l_gold = this.GetChild ("n4").asTextField;
		l_coin.text = da [0].ToString();
		l_gold.text = da [1].ToString();

		this.GetChild ("n2").asTextField.text = Tools.GetMessageById ("17018");

		GButton btn_sure = this.GetChild ("n7").asButton;
		btn_sure.text = Tools.GetMessageById ("14056");
		btn_sure.onClick.Add (OnSureHandler);

		GButton btn_no = this.GetChild ("n9").asButton;
		btn_no.text = Tools.GetMessageById ("14057");
		btn_no.onClick.Add (OnNoHandler);

		this.GetChild ("n11").asButton.onClick.Add (() => {
			ViewManager.inst.CloseView(this);
		});
	}
	private void OnSureHandler()
	{
		if (ModelUser.GetCanBuy (Config.ASSET_COIN,(int)da [0],"17026")) {
			NetHttp.inst.Send (NetBase.HTTP_COIN_BUY_GOLD, "config_id="+_index,OnBuyGoldHandler);
		}
	}
	private void OnNoHandler()
	{
		ViewManager.inst.CloseView (this);
	}
	private void OnBuyGoldHandler(VoHttp vo)
	{
		ModelManager.inst.userModel.UpdateData (vo.data);
		ViewManager.inst.ShowText(Tools.GetMessageById("19500"));
		ViewManager.inst.CloseView (this);
	}
	public override void Clear ()
	{
		base.Clear ();
	}
}
