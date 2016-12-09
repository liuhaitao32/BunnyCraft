using UnityEngine;
using System.Collections;
using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorUserLevelUp : BaseMediator {
	private ModelUser userModel;
	public static int oldLv = -1;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_LEVELUSERUP,true);
		ViewManager.SetWidthHeight (this.GetChild ("n0"));
//		ViewManager.SetWidthHeight (this.GetChild ("n13"));
		userModel = ModelManager.inst.userModel;
		this.DispatchGlobalEvent (new MainEvent (MainEvent.LEVEL_UP_USER));
		Dictionary<string,object> cfg = (Dictionary<string,object>)((Dictionary<string,object>)((Dictionary<string,object>)DataManager.inst.beckon ["player"]) ["up"]) ["hp"];
		int hp = (int)((Dictionary<string,object>)((Dictionary<string,object>)DataManager.inst.beckon ["player"]) ["property"]) ["hp"];

        //		GameObjectScaler.Scale (EffectManager.inst.AddPrefab ("Player_lvup/player_lvup", this.GetChild ("n14").asGraph), 1.5f);
        GameObject go = EffectManager.inst.AddPrefab("Player_lvup/player_lvup", this.GetChild("n14").asGraph);
        go.transform.localScale *= 1.5f;

        go.GetComponent<AudioSource>().volume = userModel.isSound?1:0;

		this.GetChild ("n0").asButton.onClick.Add (OnCloseViewFun);
		this.GetChild ("n0").asButton.touchable = false;
		this.GetChild ("n8").asCom.GetChild ("n2").asTextField.text = userModel.lv.ToString ();
		if (userModel.uname != null) {
			this.GetChild ("n4").asTextField.text = userModel.uname;
		} else {
			this.GetChild ("n4").asTextField.text = userModel.uid;
		}
		this.GetChild ("n6").asTextField.text = Tools.GetMessageById ("24121");
		this.GetChild ("n18").asTextField.text = userModel.lv.ToString ();
		this.GetChild ("n19").asTextField.text = Tools.GetMessageById ("24135");
		this.GetChild ("n10").asTextField.text = Tools.GetMessageById (cfg ["name"].ToString ());

		GButton head = this.GetChild ("n8").asCom.GetChild ("n0").asButton;
		head.touchable = false;

		Tools.SetLoaderButtonUrl (head, ModelUser.GetHeadUrl (userModel.head ["use"].ToString ()));


		float zzz = 0f;
		float shengji = 0f;
		zzz = Convert.ToSingle (Convert.ToSingle (hp) * Math.Pow (Convert.ToSingle (cfg ["power"]), Convert.ToSingle (oldLv - 1)) + Convert.ToSingle (cfg ["add"]) * Convert.ToSingle (oldLv - 1));
		if ((oldLv) == 1) {
			zzz = Convert.ToSingle(hp);
		}
		shengji = Convert.ToSingle (Convert.ToSingle (hp) * Math.Pow (Convert.ToSingle (cfg ["power"]), Convert.ToSingle (userModel.lv - 1)) + Convert.ToSingle (cfg ["add"]) * Convert.ToSingle (userModel.lv - 1) - zzz);
//		Log.debug (Math.Pow (Convert.ToSingle (cfg ["power"]), Convert.ToSingle (userModel.lv - 1)).ToString ());
//		Log.debug (Convert.ToSingle (cfg ["power"]) + "||||" + Convert.ToSingle (userModel.lv - 1));
		this.GetChild ("n11").asTextField.text = string.Format("{0:F0}",zzz);
		this.GetChild ("n17").asTextField.text = string.Format("+{0:F0}",shengji);
//		this.GetChild ("n11").asTextField.text = Math.Floor (zzz).ToString ();
//		this.GetChild ("n17").asTextField.text = "+" + Math.Floor (shengji).ToString ();
		TimerManager.inst.Add (1.5f, 1, OnCanClose);
	}
	private void OnCanClose(float cc)
	{
		this.GetChild ("n0").asButton.touchable = true;
	}
	private void OnCloseViewFun()
	{
		ViewManager.inst.CloseView (this);
	}
	public override void Clear ()
	{
		base.Clear ();
	}
}
