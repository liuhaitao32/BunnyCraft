using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;

public class MediatorEffortXX : BaseMediator {
	private ModelUser userModel;

	private MediatorExploreBox item;
	private GComponent qqbox1;
	private GComponent qqbox2;
	private GComponent gybox1;
	private GComponent gybox2;
//	private GTextField title;

	private List<GButton> boxList;

	public static int curEffort = 1;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_EFFORTXX,false,Tools.GetEffortName ((curEffort)));

		userModel = ModelManager.inst.userModel;

		qqbox1 = this.GetChild ("n10").asCom;
		qqbox2 = this.GetChild ("n11").asCom;
		gybox1 = this.GetChild ("n8").asCom;
		gybox2 = this.GetChild ("n9").asCom;
//		title = view.GetChild ("n2").asTextField;

		Dictionary<string,object> _data = userModel.GetCardRequestRarityAndCount (curEffort);
		Dictionary<string,object> cfg_Explore = DataManager.inst.explore;

		List<object> lis = Tools.ConvertDicToList (_data, "id");
		Tools.Sort (lis,new string[]{"id:int:0"});

		Dictionary<string,object> fafaf = (Dictionary<string,object>)lis [0];
		qqbox1.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image:icon_kp" + Convert.ToString(Convert.ToInt16(fafaf["id"])+1));
		gybox1.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image:icon_kp" + Convert.ToString(Convert.ToInt16(fafaf["id"])+1));
		qqbox1.GetChild ("n2").asTextField.text = fafaf ["num"].ToString ();
		gybox1.GetChild ("n2").asTextField.text = fafaf ["sup_num"].ToString ();
		fafaf = (Dictionary<string,object>)lis [1];
		qqbox2.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image:icon_kp" + Convert.ToString(Convert.ToInt16(fafaf["id"])+1));
		gybox2.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image:icon_kp" + Convert.ToString(Convert.ToInt16(fafaf["id"])+1));
		qqbox2.GetChild ("n2").asTextField.text = fafaf ["num"].ToString ();
		gybox2.GetChild ("n2").asTextField.text = fafaf ["sup_num"].ToString ();

//		title.text = Tools.GetEffortName ((curEffort));
//		this.GetChild ("n3").asTextField.text = Tools.GetMessageById ("23008");
		this.GetChild ("n19").asTextField.text = Tools.GetMessageById ("23009");
		this.GetChild ("n20").asTextField.text = Tools.GetMessageById ("23010");
		this.GetChild ("n21").asTextField.text = Tools.GetMessageById ("23011");
		this.GetChild ("n22").asTextField.text = Tools.GetMessageById ("23011");

		boxList = new List<GButton> ();
		int index = 1;
		List<object> list = new List<object> ();
		foreach (string j in ((Dictionary<string,object>)cfg_Explore["box"]).Keys) {
			if (j.IndexOf ("R") != -1) {
				Dictionary<string,object> dicto = new Dictionary<string, object> ();
				dicto.Add ("id", j);
				list.Add (dicto);
			}
		}
		Tools.Sort (list, new string[]{ "id:string:0" });
		for (int i = 0; i < list.Count; i++) {
			Dictionary<string,object> dic = ((Dictionary<string,object>)cfg_Explore ["box"]) [((Dictionary<string,object>)list[i])["id"].ToString()] as Dictionary<string,object>;
			GComponent com = this.GetChild ("b" + index).asCom;
			GButton btn = com.asButton;
			GGraph icon = com.GetChild ("n2").asGraph;
			GTextField num = com.GetChild ("n3").asTextField;

			Dictionary<string,object> curReward = (Dictionary<string,object>)DataManager.inst.award [dic ["reward"].ToString ()];
			float[] numArr = Tools.NumSection ((object[])((Dictionary<string,object>)curReward ["card"])["num"], MediatorEffortXX.curEffort);
			num.text = Tools.GetMessageById ("14016", new object[]{ Math.Floor (numArr [0]) + "" });
			string boxname = Tools.GetExploreBoxID (((Dictionary<string,object>)(((Dictionary<string,object>)cfg_Explore ["box"]) [((Dictionary<string,object>)list [i]) ["id"].ToString ()])) ["icon"].ToString ());
//			GameObjectScaler.Scale (EffectManager.inst.AddEffect (boxname,boxname, icon,null,true,50,null,true), 0.4f);
			EffectManager.inst.AddEffect (boxname,boxname, icon,null,true,50,null,true).transform.localScale *= 0.4f;
			//				EffectManager.inst.AddPrefab (Tools.GetExploreBoxID (((Dictionary<string,object>)(((Dictionary<string,object>)cfg_Explore ["box"]) [j])) ["icon"].ToString ()), icon).transform.localScale *= 0.4f;
//			btn.name = index.ToString();
			btn.data = ((Dictionary<string,object>)list[i])["id"].ToString();
			boxList.Add (btn);
			btn.RemoveEventListeners ();
			btn.onTouchBegin.Add ((EventContext ev) =>{
				if(item != null)
				{
					this.RemoveChild(item.group,true);
				}
				MediatorExploreBox.eid = btn.data.ToString();
				item = new MediatorExploreBox();
				item.group.x = (ev.sender as GButton).x - 20;
				item.group.y = (ev.sender as GButton).y;
				item.ChangY();
				if(item.y<-100)
				{
					item.y = -100;
				}
				this.AddChild(item.group);
			});
			btn.onTouchEnd.Add(() =>{
				if(item != null)
				{
					this.RemoveChild(item.group,true);
				}
			});
			index++;
		}

//		view.GetChild ("n24").asButton.onClick.Add (() => {
//			ViewManager.inst.CloseView(this);
//		});
	}
	public override void Clear ()
	{
		base.Clear ();
	}
}
