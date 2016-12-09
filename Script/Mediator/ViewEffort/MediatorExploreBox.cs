using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using System;

public class MediatorExploreBox : BaseMediator {
	public static string eid;
	private Dictionary<string,object> exCfg;
	private Dictionary<string,object> curCfg;
	private Dictionary<string,object> curReward;

	private GLoader lo1;
	private GLoader lo2;
	private GTextField tlo1;
	private GTextField tlo2;

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_EXPLOREBOX);

		exCfg = DataManager.inst.explore;
		curCfg = (Dictionary<string,object>)(((Dictionary<string,object>)exCfg ["box"]) [eid]);
		curReward = (Dictionary<string,object>)DataManager.inst.award [curCfg ["reward"].ToString ()];

		GTextField time = this.GetChild ("n14").asTextField;
		GTextField gold = this.GetChild ("n18").asTextField;
		GTextField cardNum = this.GetChild ("n9").asTextField;
		lo1 = this.GetChild ("n12").asLoader;
		lo2 = this.GetChild ("n13").asLoader;
		tlo1 = this.GetChild ("n10").asTextField;
		tlo2 = this.GetChild ("n11").asTextField;

		long _time = (long)(Convert.ToSingle ((int)curCfg ["time"]) * 10000000);
		time.text = Tools.TimeFormat2 (_time,1);


		float[] numArr = Tools.NumSection ((object[])curReward ["gold"], MediatorEffortXX.curEffort);
		gold.text = Math.Floor (numArr [0]) + "-" + Math.Floor (numArr [1]);

		numArr = Tools.NumSection ((object[])((Dictionary<string,object>)curReward ["card"])["num"], MediatorEffortXX.curEffort);
		cardNum.text = Math.Floor (numArr [0])+"";
		this.GetChild ("n15").asTextField.text = Tools.GetMessageById ("19017");

		showCardBDNum ();
	}
	private void showCardBDNum()
	{
		lo1.visible = false;
		lo2.visible = false;
		tlo1.visible = false;
		tlo2.visible = false;
		int cardindex = 0;
		string nums = "";
		int ra = 0;
		object[] obj = (object[])(((Dictionary<string,object>)curReward ["card"])["steps"]);
		for (int i = 0; i < obj.GetLength (0); i++) {
			Dictionary<string,object> dic = obj [i] as Dictionary<string,object>;
			if ((int)dic ["group"] == 0)
				break;
			if (Tools.NumSectionOne ((object[])dic ["open"], ModelManager.inst.userModel.effort_lv) < 1f)
				continue;
			nums = Math.Floor (Tools.NumSection ((object[])dic ["num"], ModelManager.inst.userModel.effort_lv) [0]) + "";
			ra = (int)dic ["group"];
			showCard (cardindex,ra,nums);
			cardindex++;
		}
		if (cardindex == 0) {
			this.GetController ("c1").selectedIndex = 1;
		}
		if (cardindex == 1) {
			lo1.x += 60;
			tlo1.x += 60;
		}
	}
	public void ChangY()
	{
		if (this.GetController ("c1").selectedIndex == 1) {
			this.y -= 158 - 60;
		} else {
			this.y -= 235 - 60;
		}
	}
	private void showCard(int index,int ra,string num)
	{
		if (index == 0) {
			lo1.visible = true;
			tlo1.visible = true;
			lo1.url = Tools.GetResourceUrl ("Image:icon_kp" + (ra + 1));
			tlo1.text = "x"+num;
			setColor (tlo1, ra);
		} else {
			lo2.visible = true;
			tlo2.visible = true;
			lo2.url = Tools.GetResourceUrl ("Image:icon_kp" + (ra + 1));
			tlo2.text = "x"+num;
			setColor (tlo2, ra);
		}

	}
	private void setColor(GTextField l,int ra)
	{
		switch (ra) {
		case 1:
			l.color = Color.green;
			break;
		case 2:
			l.color = new Color(170f/255f,0,1,1);
			break;
		case 3:
			l.color = new Color(1,140f/255f,0,1);
			break;
		}
	}
	public override void Clear ()
	{
		base.Clear ();
	}

}
