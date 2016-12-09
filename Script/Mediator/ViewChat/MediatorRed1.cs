using System;
using System.Collections.Generic;
using FairyGUI;

public class MediatorRed1:BaseMediator
{
	private ModelUser userModel;

	private GTextField t1;
	private GTextField t3;
	private GTextField num1;
	private GTextField num3;
	private GButton btn1;
	private GButton btn3;

	private Dictionary<string,object> cfg;
	private object[] count;
	private bool isEnter = false;
    public int gold;
	public MediatorRed1 ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_RED1, false, Tools.GetMessageById ("22011"));
        
		userModel = ModelManager.inst.userModel;
		count = (object[])userModel.records ["guild_redbag_coin"];
		cfg = DataManager.inst.guild;
        gold = userModel.gold;
		t1 = this.GetChild ("n8").asTextField;
		t3 = this.GetChild ("n10").asTextField;
		btn1 = this.GetChild ("n16").asButton;
		btn3 = this.GetChild ("n18").asButton;
		num1 = this.GetChild ("n32").asTextField;
		num3 = this.GetChild ("n33").asTextField;

		this.GetChild ("n35").asTextField.text = Tools.GetMessageById ("22027");
		this.GetChild ("n36").asTextField.text = Tools.GetMessageById ("22047");

//		this.GetChild ("n1").text = Tools.GetMessageById ("22011");
		t1.text = Tools.GetMessageById ("22031")+Tools.Analysis (cfg, "redbag_society_1.exp").ToString ();
		t3.text = Tools.GetMessageById ("22031")+Tools.Analysis (cfg, "redbag_society_2.exp").ToString ();
		btn1.text = Tools.GetMessageById ("22032");
		btn3.text = Tools.GetMessageById ("22032");
		num1.text = Tools.GetMessageById ("22026", new string[]{ count [0].ToString () });
		num3.text = Tools.GetMessageById ("22026", new string[]{ count [1].ToString () });
//		this.GetChild ("n30").text = Tools.Analysis (cfg, "redbag_society_1.exp").ToString ();
//		this.GetChild ("n31").text = Tools.Analysis (cfg, "redbag_society_2.exp").ToString ();

		btn1.onClick.Add (() =>
		{
			if (Convert.ToInt32 (count [0]) <= 0)
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("22044"));
				return;
			}
			Btn_Click (0);
		});
		btn3.onClick.Add (() =>
		{
			if (Convert.ToInt32 (count [1]) <= 0)
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("22045"));
				return;
			}
			Btn_Click (1);
		});
	}

	public override void Clear ()
	{
		base.Clear ();
		if (!isEnter)
			this.DispatchGlobalEventWith (MainEvent.CHAT_SENDREDBAG);
	}

	private void Btn_Click (int index)
	{
		isEnter = true;
		Dictionary<string,object> data = new Dictionary<string, object> ();
		data ["type"] = index;
		NetHttp.inst.Send (NetBase.HTTP_SENDGUILDREDBAG, data, (VoHttp vo) =>
		{			
			GRoot.inst.touchable = false;
			ViewManager.inst.ShowIcon (userModel.GetReward (vo.data), () =>
			{
				GRoot.inst.touchable = true;
				userModel.UpdateData (vo.data);
				this.DispatchGlobalEventWith (MainEvent.CHAT_SENDREDBAG);
			});
			ViewManager.inst.CloseView (this);
		});			
	}
}