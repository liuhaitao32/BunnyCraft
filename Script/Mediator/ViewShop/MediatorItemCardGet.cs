using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using FairyGUI;

public class MediatorItemCardGet : BaseMediator
{
	//抽将界面
	private Dictionary<string,object> cfg = DataManager.inst.random_award;
	Dictionary<string,object> con;
	Dictionary<string,object> listData;
	int haveSuo = 0;
	public static int _index = 0;
	private Dictionary<string,object> user;
	private GList list1;
	private GComponent item0;

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_ITEMCADRGET);

//		view.x = view.GetChild("n2").x;
//		view.y = view.GetChild ("n2").y + 50;
		user = ModelManager.inst.userModel.records ["daily_box"] as Dictionary<string,object>;

		if (user ["refresh_time"] != null)
		{
			DateTime date = (DateTime)user ["refresh_time"];
			if ((date.Ticks / 10000) < Tools.GetSystemSecond ())
			{
				LocalStore.SetLocal (LocalStore.LOCAL_SHOPRED, "1");
				NetHttp.inst.Send (NetBase.HTTP_REFRESH_DAILY_BOX, "", OnRefreshBox);
			}
			else
			{
				TimerManager.inst.Add (1f, 0, Time_Tick);
			}
		}
		else
		{
			LocalStore.SetLocal (LocalStore.LOCAL_SHOPRED, "1");
			NetHttp.inst.Send (NetBase.HTTP_REFRESH_DAILY_BOX, "", OnRefreshBox);
		}


		con = (Dictionary<string,object>)cfg ["box"];
		listData = new Dictionary<string, object> ();
		listData ["0"] = (Dictionary<string,object>)cfg ["daily_box"];
		int listIndex = 1;
		Dictionary<string,object> effcfg = (Dictionary<string,object>)DataManager.inst.effort ["effort_cond"];
		for (int i = con.Keys.Count; i >= 1; i--)
		{
			if (i >= effcfg.Count+2)
				continue;
			Dictionary<string,object> data = (Dictionary<string,object>)con ["Lv" + i];
			if (ModelManager.inst.userModel.effort_lv >= (int)data ["effort_lv"])
			{
				listData [listIndex + ""] = con ["Lv" + i];
				(listData [listIndex + ""] as Dictionary<string,object>) ["id"] = i;
				listIndex++;
			}
			else
			{
				haveSuo = i;
			}
		}
		if (haveSuo != 0)
		{
			listData [listIndex + ""] = con ["Lv" + haveSuo];
		}
		list1 = this.GetChild ("n0").asList;
		List<object> list = new List<object> (listData.Values);
//		list1.width = MediatorShop.thiswidth;
		list1.itemRenderer = List_Render1;
//		list1.SetVirtual ();
		ViewManager.inst.ShowViewCallLaterRequest (() =>
		{
				list1.numItems = list.Count+2;
		});
		if (list.Count < 4)
		{
			list1.scrollPane.touchEffect = false;
		}

		DispatchManager.inst.Register (MainEvent.DAY_BOX_BUY, OnBoxBuyFun);
	}

	private void OnBoxBuyFun (MainEvent e)
	{
		user = ModelManager.inst.userModel.records ["daily_box"] as Dictionary<string,object>;
		List<object> list = new List<object> (listData.Values);
		list1.itemRenderer = List_Render1;
		list1.numItems = list.Count+2;
	}

	private void OnRefreshBox (VoHttp vo)
	{
		ModelManager.inst.userModel.records ["daily_box"] = vo.data;
		this.DispatchGlobalEvent (new MainEvent (MainEvent.DAY_BOX_FRUSH));
		user = ModelManager.inst.userModel.records ["daily_box"] as Dictionary<string,object>;
		List<object> list = new List<object> (listData.Values);
		list1.numItems = list.Count+2;
		TimerManager.inst.Add (1f, 0, Time_Tick);
		Time_Tick (0f);
	}
	private void Time_Tick (float time)
	{
		if (item0 != null && item0.name == "0")
		{
			GTextField l_time = item0.GetChild ("n2").asTextField;
			if (user ["refresh_time"] == null)
			{
				l_time.text = "";
				TimerManager.inst.Remove (Time_Tick);
				NetHttp.inst.Send (NetBase.HTTP_REFRESH_DAILY_BOX, "", OnRefreshBox);
				return;
			}
			DateTime date = (DateTime)user ["refresh_time"];
			if ((date.Ticks - Tools.GetSystemTicks ()) < 0)
			{
				l_time.text = "";
				TimerManager.inst.Remove (Time_Tick);
				NetHttp.inst.Send (NetBase.HTTP_REFRESH_DAILY_BOX, "", OnRefreshBox);
//				ViewManager.inst.ShowText (Tools.GetMessageById ("17020"));
				return;
			}
			l_time.text = Tools.GetMessageById ("17012", new string[]{ Tools.TimeFormat ((date.Ticks - Tools.GetSystemTicks ())) });
		}
	}

	private void List_Render1 (int _index, GObject go)
	{
		GGraph btn_get = go.asCom.GetChild ("n10").asGraph;
		int index = _index - 1;
		if (_index == 0 || _index == list1.numItems - 1)
		{
			go.width = 10;
			go.alpha = 0;
			return;
		}
		else
		{
			go.width = 190;
			go.alpha = 1;
		}
		go.name = index + "";
		GButton btn = go.asButton;
		btn.RemoveEventListeners ();
		btn.onClick.Add (() =>
		{
			if (index == (listData.Keys.Count - 1) && haveSuo != 0)
			{
					ViewManager.inst.ShowText (Tools.GetMessageById ("17014",new string[]{Tools.GetEffortName(index)}));
			}
			else
			{
				this.onListItemClick (index);
			}
		});
		
		GLoader img_coin = go.asCom.GetChild ("n6").asLoader;
		GTextField l_coin = go.asCom.GetChild ("n7").asTextField;
		GTextField l_time = go.asCom.GetChild ("n2").asTextField;
		GTextField l_level = go.asCom.GetChild ("n4").asTextField;
		GImage img2 = go.asCom.GetChild ("n1").asImage;

//		GLoader l_imag = go.asCom.GetChild ("n9").asLoader;
		btn.enabled = true;
		btn_get.visible = false;
		GameObject asd;
		if (index == 0)
		{
			item0 = go.asCom;
			Time_Tick (0f);
//			l_imag.url = Tools.GetResourceUrl ("Image:bg_baoxiang");
			Dictionary<string,object> cfg1 = (Dictionary<string,object>)cfg ["daily_box"];
			img_coin.url = Tools.GetResourceUrl ("Image2:n_icon_xm");
			l_level.text = Tools.GetMessageById (cfg1 ["name"].ToString ());
			if (LocalStore.GetLocal (LocalStore.LOCAL_SHOPRED) == "1") {
				ModelManager.inst.userModel.Add_Notice (go.asCom, new Vector2 (155, 5));
			} else {
				ModelManager.inst.userModel.Remove_Notice (go.asCom);
			}
			object[] price = (object[])cfg1 ["price"];
			int len = price.GetLength (0) - 3;
			int axb = ((int)price [len + 1]) * (int)user ["num"] + ((int)price [len + 2]);
			int pri = (int)user ["num"] > len ? axb : (int)price [(int)user ["num"]];
			l_coin.text = pri + "";
//			img_coin.y = 363;
//			img_coin.x = 69;
			img2.visible = true;
			go.asCom.GetChild ("n13").visible = true;
			asd = EffectManager.inst.AddEffect (Tools.GetExploreBoxID (cfg1 ["picture"].ToString ()), "stand", btn_get);
			GameObjectScaler.Scale (asd, 0.7f);
//			asd.transform.localScale *= 0.7f;
		}
		else
		{
			Dictionary<string,object> dic = (Dictionary<string,object>)listData [index + ""];
			if (index == (listData.Keys.Count - 1) && haveSuo != 0)
			{
				go.grayed = true;
				btn_get.grayed = true;
				asd = EffectManager.inst.AddEffect (Tools.GetEggName (dic ["picture"].ToString ()), "end", btn_get, null, true);
				EffectManager.inst.SetShaderSaturation (asd, -1);
				GameObjectScaler.ScaleParticles (asd, 0);
				EffectManager.inst.StopAnimation (asd);
//				Tools.StopAnimation (asd);
			}
			else
			{
				asd = EffectManager.inst.AddEffect (Tools.GetEggName (dic ["picture"].ToString ()), "stand", btn_get);
			}
//			l_imag.url = Tools.GetResourceUrl ("Image:bg_baoxiang2");
			img_coin.url = Tools.GetResourceUrl ("Image2:n_icon_zs");
//			img_coin.y = 363 - 25;
//			img_coin.x = 69 - 50;
			l_coin.text = (int)dic ["price"] + "";
			l_time.text = "";
			img2.visible = false;
			go.asCom.GetChild ("n13").visible = false;
			l_level.text = Tools.GetMessageById (dic ["name"].ToString ());
		}
		GoWrapperCheck goc = go.displayObject.gameObject.AddComponent<GoWrapperCheck>();
		goc.setItem (go, Tools.offectSetX(160f),Tools.offectSetX(160f+720f),asd,GRoot.inst.scale.x);
		btn_get.visible = true;
	}

	private void onListItemClick (int index)
	{
		if (index == 0)
		{
			_index = 0;
			LocalStore.SetLocal (LocalStore.LOCAL_SHOPRED, "0");
			list1.numItems = listData.Count + 2;
			ViewManager.inst.ShowView<MediatorGetDayCardBox> ();
		}
		else
		{
			Dictionary<string,object> dic = (Dictionary<string,object>)listData [index + ""];
			_index = (int)dic ["id"];
			ViewManager.inst.ShowView<MediatorGetCardBox> ();
		}
	}

	public override void Clear ()
	{
		DispatchManager.inst.Unregister (MainEvent.DAY_BOX_BUY, OnBoxBuyFun);
		TimerManager.inst.Remove (Time_Tick);
		base.Clear ();
	}
}
