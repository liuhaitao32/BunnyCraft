using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorRed2:BaseMediator
{
	private ModelChat chatModel;
	private ModelUser userModel;

	private GList list;
	//	private GTextField gold;
	//	private GTextField text;
	//	private GTextField txt1;
	//	private GTextField txt2;
	//	private GTextField txt3;

	private Dictionary<string,object> cfg;
	private List<object> ld;

	public MediatorRed2 ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		chatModel = ModelManager.inst.chatModel;
		userModel = ModelManager.inst.userModel;
		cfg = DataManager.inst.guild;
       
		string title = Tools.GetMessageById ("22018", new string[] {
			chatModel.grab_List.Length.ToString (),
			Tools.Analysis (cfg, "redbag_society_" + (chatModel.grab_Type + 1).ToString () + ".times").ToString ()
		});
		this.Create (Config.VIEW_RED2, false, title);

//		gold = view.GetChild ("n6").asCom.GetChild ("n0").asTextField;
		list = this.GetChild ("n8").asList;
//		text = view.GetChild ("n2").asTextField;
//		txt1 = view.GetChild ("n25").asTextField;
//		txt2 = view.GetChild ("n26").asTextField;
//		txt3 = view.GetChild ("n27").asTextField;
//		(view.GetChild ("n6").asCom as ComGold).RemoveListener ();

		ld = chatModel.GetRedCards ();
//		gold.text = chatModel.GetRedGolds ().ToString ();
//		text.text = Tools.GetMessageById ("22018", new string[] {
//			chatModel.grab_List.Length.ToString (),
//			Tools.Analysis (cfg, "redbag_society_" + (chatModel.grab_Type + 1).ToString () + ".times").ToString ()
//		});
//		int[] counts = chatModel.GetCardRarity ();
//		txt1.text = Tools.GetMessageById ("14016", new string[]{ counts [0].ToString () });
//		txt2.text = Tools.GetMessageById ("14016", new string[]{ counts [1].ToString () });
//		txt3.text = Tools.GetMessageById ("14016", new string[]{ counts [2].ToString () });

		list.itemRenderer = Item_Render;
		list.numItems = chatModel.grab_List.Length;

    }

	public override void Clear ()
	{
		base.Clear ();
		chatModel.isRedbagGift = false;
//		this.DispatchGlobalEventWith (MainEvent.CHAT_SENDREDBAG);
	}

	//	private void Item_Render (int index, GObject go)
	//	{
	//		ComIcon ci = (go as GComponent).GetChild ("n0") as ComIcon;
	//		ci.SetData (ld [index]);
	//	}

	private void Item_Render (int index, GObject go)
	{
		Dictionary<string,object> data = (Dictionary<string,object>)chatModel.grab_List [index];

		GComponent g = go as GComponent;
		GLoader bg = g.GetChild ("n0").asLoader;
		if (index == 0 || index % 2 == 0) {
			bg.alpha = 1;
		} else {
			bg.alpha = 0;
		}
		GButton head = g.GetChild ("n1").asButton;
		GTextField name = g.GetChild ("n2").asTextField;
//		GTextField txt = g.GetChild ("n3").asCom.GetChild ("n0").asTextField;
		GList list = g.GetChild ("n4").asList;
		Controller c1 = g.GetController ("c1");
//		(g.GetChild ("n3").asCom as ComGold).RemoveListener ();
		bg.url = Tools.GetResourceUrl ("Image2:n_bg_tanban6");
		if (userModel.uid == data ["uid"].ToString ()) {
			c1.selectedIndex = 1;
			bg.alpha = 1;
			bg.url = Tools.GetResourceUrl ("Image2:n_bg_tanban6_");
		}
		else
			c1.selectedIndex = 0;
		Dictionary<string,object> use = data ["head"] as Dictionary<string,object>;
//      head.GetChild ("n2").text = data ["lv"].ToString ();
		Tools.SetLoaderButtonUrl (head.GetChild("n0").asButton, ModelUser.GetHeadUrl (use ["use"].ToString ()));
		name.text = ModelUser.GetUname (data ["uid"].ToString (), ModelUser.GetUname (data ["uid"], data ["uname"]));
//		txt.text = data ["gold"].ToString ();

//		bool isGift = chatModel.isRedbagGift;
//		chatModel.isRedbagGift = false;
//		if (isGift)
//		{
//			if (userModel.uid != data ["uid"].ToString ())
//				isGift = false;
//		}
		List<object> li = new List<object> ();
		if (data ["card"] != null)
		{
			Dictionary<string,object> da = new Dictionary<string, object> ();
			da ["card"] = data ["card"];
			li = Tools.GetReward (da);
		}
		li.Add (new Dictionary<string,object> (){ { "gold",data ["gold"] } });
		list.itemRenderer = (int i, GObject item) =>
		{
			ComBigIcon ci = item.asCom.GetChild ("n0") as ComBigIcon;
			Dictionary<string,object> dd = (Dictionary<string,object>)li [i];
			foreach (string n in dd.Keys)
			{
				ci.SetData (n, Convert.ToInt32 (dd [n]), 4);
				if (chatModel.isRedbagGift && userModel.uid == data ["uid"].ToString ())
					ci.RotateCard (1f);
				else
					ci.RotateCard (0f);
			}
		};
		list.numItems = li.Count;
		list.x = 545 - ((li.Count - 1) * 130);
	}
}