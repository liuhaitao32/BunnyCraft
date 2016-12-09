using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorLoginCard:BaseMediator
{
	private ModelUser userModel;
	private ModelChat chatModel;

	private ComCard card;
	private GList list;
	private ComProgressBar bar;

	private List<object[]> listData;

	public MediatorLoginCard ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_LOGINCARD, false);

		userModel = ModelManager.inst.userModel;
		chatModel = ModelManager.inst.chatModel;
		//
		this.GetChild("n17").asTextField.text = Tools.GetMessageById ("25501");
		//
		listData = new List<object[]> ();
		Dictionary<string,object> data = (Dictionary<string,object>)userModel.records ["guild_support_logs"];

		this.GetChild ("n7").text = Tools.GetMessageById ("25502");
		this.GetChild ("n9").text = Tools.GetMessageById ("25503");

		card = this.GetChild ("n8").asCom as ComCard;
		list = this.GetChild ("n12").asList;
//		bar = this.GetChild ("n4").asCom as ComProgressBar;
//		bar.skin = ComProgressBar.BAR7;

		int count = 0;
		Dictionary<string,object> users = (Dictionary<string,object>)data ["data"];
		object[] obj;
		foreach (string n in users.Keys)
		{
			obj = new object[3];
			object[] ob = (object[])users [n];
			obj [0] = n;
			obj [1] = ob [0];
			obj [2] = ob [1];
			listData.Add (obj);
			count += Convert.ToInt32 (obj [2]);
		}

		int max = chatModel.GetCardRequestCount (data ["cid"].ToString (), userModel.effort_lv);
//		bar.value = count;
//		bar.max = max;
		this.GetChild("n19").asTextField.text = count+"/"+max;
		CardVo vo = DataManager.inst.GetCardVo (data ["cid"].ToString ());
		card.SetData (vo.id, 1, 1);
		card.SetText (Tools.GetMessageById (vo.name));

		this.GetChild ("n11").text = vo.exp+"";// + "/" + vo.maxExp;
		this.GetChild ("n18").text = "/" + vo.maxExp;

		list.itemRenderer = Item_Render;
		list.SetVirtual ();
		list.numItems = listData.Count;
	}

	public override void Clear ()
	{
		base.Clear ();
	}

	private void Item_Render (int index, GObject item)
	{
		object[] obj = listData [index];
		GComponent g = item.asCom;
		string msg;
		string user;
		msg = Tools.GetMessageById ("25504", new string[] {
			obj [2].ToString ()
		});
		if (obj [1] == null) {
			user = obj [0].ToString ();
		}else{
			user = obj [1].ToString ();
		}
//		msg = Tools.GetMessageColor (msg, new string[]{ "ffffff", "bdff00" });
		g.GetChild ("n0").text = user;
		g.GetChild ("n1").text = msg;
	}
}