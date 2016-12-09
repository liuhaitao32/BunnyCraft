using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;

public class MediatorFightTip : BaseMediator
{
	private GComponent head;
	private ModelUser userModel;
	private Dictionary<string,object> cfg;

	public MediatorFightTip ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_TIP, false,Tools.GetMessageById ("24245"));


		userModel = ModelManager.inst.userModel;
		cfg = (Dictionary<string,object>)DataManager.inst.match ["custom_level"];
		head = this.GetChild ("n8").asCom;

		head.GetChild ("n2").text = cfg ["ship"].ToString ();
		Tools.SetLoaderButtonUrl (head.GetChild ("n0").asButton, ModelUser.GetHeadUrl (userModel.head ["use"].ToString ()));
		this.GetChild ("n4").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image:icon_kp1");
		this.GetChild ("n5").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image:icon_kp2");
		this.GetChild ("n6").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image:icon_kp3");
		this.GetChild ("n7").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image:icon_kp4");

		this.GetChild ("n4").asCom.GetChild ("n1").asCom.GetChild ("n1").asTextField.text = cfg ["0"].ToString ();
		this.GetChild ("n5").asCom.GetChild ("n1").asCom.GetChild ("n1").asTextField.text = cfg ["1"].ToString ();
		this.GetChild ("n6").asCom.GetChild ("n1").asCom.GetChild ("n1").asTextField.text = cfg ["2"].ToString ();
		this.GetChild ("n7").asCom.GetChild ("n1").asCom.GetChild ("n1").asTextField.text = cfg ["3"].ToString ();

		this.GetChild ("n10").asTextField.text = Tools.GetMessageById ("25060");
		this.GetChild ("n3").asTextField.text = Tools.GetMessageById ("25061");
	}

	public override void Clear ()
	{
		base.Clear ();
	}
}
