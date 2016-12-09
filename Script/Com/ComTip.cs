using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;

public class ComTip : BaseMediator
{
	private object data;

	private List<object> datas;
	private GTextField txt;
	private GImage bg;
	private GList list;
	private GImage n0;

	private string type;
	private string temp = null;
	string msg;

	public ComTip ()
	{
		
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.COM_TIP);
		data = ModelManager.inst.alertModel.data;

		n0 = this.GetChild ("n0").asImage;
		bg = this.GetChild ("n1").asImage;
		txt = this.GetChild ("n2").asTextField;
		list = this.GetChild ("n3").asList;
		float textH = 29;
		if (data is string)
			msg = (string)data;
		else if (data is IDictionary)
		{
			Dictionary<string,object> d = (Dictionary<string,object>)data;
			foreach (string n in d.Keys)
			{
				msg = n;
				temp = d [n].ToString ();
			}
		}
		switch (msg)
		{
		case Config.TOUCH_COMGOLD:
			txt.text = Tools.GetMessageById ("14036");
			bg.height = bg.height + txt.textHeight - textH;
			break;
		case Config.TOUCH_COMCOIN:
			txt.text = Tools.GetMessageById ("14037");
			bg.height = bg.height + txt.textHeight - textH;
			break;
		case Config.TOUCH_COMEXP:
			txt.text = Tools.GetMessageById ("14038");
			bg.height = bg.height + txt.textHeight - textH;
			break;
		case Config.TOUCH_COMELSCORE:
			txt.text = Tools.GetMessageById ("14039");
			bg.height = bg.height + txt.textHeight - textH;
			n0.x = 220;
			break;
		case Config.ASSET_REDBAGCOIN:
			txt.text = Tools.GetMessageById ("14040");
			bg.height = bg.height + txt.textHeight - textH;
			this.GetController ("c1").selectedIndex = 2;
			break;
		case "sRed":
			txt.text = Tools.GetMessageById ("14052");
//			txt.align = AlignType.Center;
//			n0.x += 47;
//			n0.TweenRotate (-90, 0);
//			n0.y += 70;
			bg.height = bg.height + txt.textHeight - textH;
			this.GetController ("c1").selectedIndex = 3;
//			this.y -= this.GetChild ("n10").y;
//			bg.y -= 90;
//			txt.y -= 90;
			break;
		case "bRed":
			txt.text = Tools.GetMessageById ("14053");
//			txt.align = AlignType.Center;
//			n0.x += 47;
//			n0.TweenRotate (-90, 0);
//			n0.y += 70;
			bg.height = bg.height + txt.textHeight - textH;
			this.GetController ("c1").selectedIndex = 3;
//			this.y -= this.GetChild ("n10").y;
//			bg.y -= 90;
//			txt.y -= 90;
			break;
		case Config.ASSET_RANKSCORE:
			datas = new List<object> ();
			Dictionary<string,object> matchCfg = DataManager.inst.match;
			Dictionary<string,object> rank_group = (Dictionary<string,object>)matchCfg ["rank_group_show"];

			foreach (string i in rank_group.Keys) {
				Dictionary<string,object> one = new Dictionary<string, object> ();
				one.Add (i, rank_group [i]);
				one.Add ("index", Convert.ToInt32 (i));
				datas.Add (one);
			}

			string[] staasd = ModelManager.inst.userModel.GetNextRankExpAndName (ModelManager.inst.userModel.rank_score);
			if (rank_group.Count == Convert.ToInt32 (staasd [2])) {
				this.GetChild ("n7").asTextField.text = Tools.GetMessageById ("14054");
				this.GetChild ("n8").asLoader.url = ModelManager.inst.userModel.GetRankImg (ModelManager.inst.userModel.rank_score);
			} else {
				this.GetChild ("n7").asTextField.text = Tools.GetMessageById ("14051",staasd);
				this.GetChild ("n8").asLoader.url = ModelManager.inst.userModel.GetRankImg (ModelManager.inst.userModel.rank_score, 1);
			}
			Tools.Sort (datas, new string[]{ "index:int:0" });
			this.GetController ("c1").selectedIndex = 1;
			list.itemRenderer = OnRender;
			list.numItems = datas.Count;
//			list.ResizeToFit (datas.Count);
			bg.height = 110;
			break;
		}
		group.height = bg.height;
	}

	public void UpdatePos ()
	{
		switch (msg)
		{
		case Config.ASSET_RANKSCORE:
			group.x += (ModelManager.inst.gameModel.width - 960) / 2 + 20;
			group.y -= 30;
			break;
		case Config.ASSET_REDBAGCOIN:			
			if (temp != null)
			{//红包
				group.x += 50;
				group.y -= 140;
			}
			else
			{
				//card
				group.x += 50;
				group.y -= 130;
			}
			this.GetChild ("n0").y = bg.height - 40;
			break;
		case "sRed":
//			group.y -= 70;
			group.y -= this.GetChild ("n10").y;
			group.x -= 137;
			break;
		case "bRed":
//			group.y -= 70;
			group.y -= this.GetChild ("n10").y;
			group.x -= 137;
			break;
		default:
//			if (group.x + group.height < GRoot.inst.height / 2)
//				group.y += 30;
//			else
//				group.y -= 30;
			group.y += 10;
			group.x -= 50;
			break;
		}
	}

	private void OnRender (int index, GObject go)
	{
		object[] da = (object[])((Dictionary<string,object>)datas [index]) [(index + 1).ToString ()];

		go.asCom.GetChild ("n2").asTextField.text = (int)((object[])da [0]) [0] + "-" + (int)((object[])da [0]) [1];
		go.asCom.GetChild ("n0").asTextField.text = Tools.GetMessageById ((14041 + index).ToString ()) + ":";
		go.asCom.GetChild ("n1").asLoader.url = ModelManager.inst.userModel.GetRankImg ((int)((object[])da [0]) [1]);
	}

	public override void Clear ()
	{
		base.Clear ();
	}
}
