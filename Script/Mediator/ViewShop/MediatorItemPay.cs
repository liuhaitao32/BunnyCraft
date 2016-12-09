using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using FairyGUI;

public class MediatorItemPay : BaseMediator {//兑换界面
	
	private Dictionary<string,object> pay_config;
	private List<object> _data;
	private GList list2;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_ITEMPAY);
//		view.x = 0;
//		view.y = 84;

		pay_config = DataManager.inst.pay_config;
		_data = new List<object> ();
		foreach (string i in pay_config.Keys) {
			if (i != "pay") {
				object[] obj = (object[])pay_config [i];
				Dictionary<string,object> _dic = new Dictionary<string, object> ();
				_dic.Add ("id", i);
				_dic.Add ("money", obj [0]);
				_dic.Add ("coin", obj [1]);
				_dic.Add ("icon", obj [2]);
				_dic.Add ("sRed", obj [3]);
				_dic.Add ("bRed", obj [4]);
				_dic.Add ("index", obj [5]);
				_dic.Add ("name", obj [6]);
				_dic.Add ("iap", obj [7]);
				_data.Add (_dic);
			}
		}
		Tools.Sort (_data, new string[]{ "index:int:0" });
		#if UNITY_IOS
		string[] iap = new string[_data.Count];
		for(var i = 0;i<_data.Count;i++){
			iap [i] = ((Dictionary<string,object>)_data [i]) ["iap"].ToString();
		}
		PlatForm.inst.GetSdk().CallInitPIDs(iap,"",(object s)=>{
			if(s.ToString()=="0"){
				Set_listData (_data);
			}
			else{
//				Debug.LogError("-->"+s.ToString());
				ViewManager.inst.ShowText(s.ToString());
			}
		});
		#endif

		#if !UNITY_IOS
		Set_listData (_data);
		#endif
	}
	private void Set_listData(List<object> _data){
		list2 = this.GetChild ("n0").asList;
//		list2.width = MediatorShop.thiswidth;
		list2.itemRenderer = List_Render2;
		list2.SetVirtual ();
		list2.numItems = _data.Count+2;
		if (_data.Count < 4) {
			list2.scrollPane.touchEffect = false;
		}
	}
	private void List_Render2 (int _index,GObject go)
	{
		int index = _index - 1;
		if (_index == 0 || _index == list2.numItems - 1) {
			go.width = 10;//(MediatorShop.thiswidth - 960) / 2;
			go.alpha = 0;
			return;
		} else {
			go.width = 190;
			go.alpha = 1;
		}
		Dictionary<string,object> data = (Dictionary<string,object>)_data [index];
//		go.asCom.GetChild ("n2").asLoader.url = Tools.GetResourceUrl ("Image:" + data ["icon"].ToString ());
//		ComBigIcon card = go.asCom.GetChild("n11") as ComBigIcon;
		GLoader card  = go.asCom.GetChild("n17").asLoader;
//		card.SetData (Config.ASSET_COIN, data ["coin"].ToString (), 0, "Image2:" + data ["icon"].ToString ());
//		card.SetSelectIndex (2);
//		card.GetChild ("n2").visible = false;
//		go.asButton.GetChild("15").touchable = false;
		card.url = Tools.GetResourceUrl ("Image2:" + data ["icon"].ToString ());
//		go.asCom.GetChild ("n14").asTextField.text = "x" + ((object[])(gold_buy [index])) [1];
		go.asCom.GetChild ("n16").asTextField.text = "x" + data ["coin"].ToString ();//card.GetChild ("n15").asTextField.text;
		GButton btn = go.asCom.GetChild ("n0").asButton;

		btn.asButton.RemoveEventListeners ();
		btn.asButton.onClick.Add(() =>
		{
			this.onListItemClick2 (index);
		});
		GButton red = go.asCom.GetChild ("n8").asButton;
		GLoader redimg = red.GetChild ("n8").asLoader;
		GTextField t1 = go.asCom.GetChild ("n9").asTextField;
		GTextField t2 = go.asCom.GetChild ("n12").asTextField;
		go.asCom.GetChild ("n15").asImage.touchable = false;
		red.visible = true;
		t1.text = "";
		t2.text = "";
		ModelManager.inst.userModel.GetUnlcok(Config.UNLOCK_PAY_RED, red);
		if ((int)data ["sRed"] != 0) {
			red.scale = new Vector2 (0.35f, 0.35f);
			t1.text = Tools.GetMessageById ("17022");
			t2.text = Tools.GetMessageById ("17023", new string[]{ ((int)data ["sRed"]).ToString () });
			redimg.url = Tools.GetResourceUrl ("Image2:n_icon_bthb2");
			ViewManager.inst.AddTouchTip ("BaseLoader", redimg, "sRed");
		} else if ((int)data ["bRed"] != 0) {
			red.scale = new Vector2 (0.35f, 0.35f);
			t1.text = Tools.GetMessageById ("17022");
			t2.text = Tools.GetMessageById ("17023", new string[]{ ((int)data ["bRed"]).ToString () });
			redimg.url = Tools.GetResourceUrl ("Image2:n_icon_bthb1");
			ViewManager.inst.AddTouchTip ("BaseLoader", redimg, "bRed");
		} else {
			red.visible = false;
		}
		red.scale *= 0.8f;
		GTextField name = go.asCom.GetChild ("n7").asTextField;
		GTextField set_num = go.asCom.GetChild ("n5").asTextField;
		name.text = Tools.GetMessageById (data ["name"].ToString ());
		set_num.text = Tools.GetMessageById ("33051", new object[]{ data ["money"].ToString () });

		t1.visible = red.visible;
		t2.visible = red.visible;
	}
	private int currNum;
	private int currsRedNum;
	private int currbRedNum;
	private void onListItemClick2(int index)
	{
		Dictionary<string,object> data = (Dictionary<string,object>)_data [index];
		currNum = (int)data ["coin"];
		currsRedNum = (int)data ["sRed"];
		currbRedNum = (int)data ["bRed"];
		#if !UNITY_IOS
			NetHttp.inst.Send (NetBase.HTTP_TEST_COIN, "coin_key="+data ["id"].ToString (),OnPayFunOver);
		#endif
		#if UNITY_IOS
		PlatForm.inst.GetSdk().CallBuy(data["iap"].ToString(),(object s)=>{
			
			if(s is string[]){
				//{"Store":"fake","TransactionID":"e9e3d15a-beb3-45c1-ae2e-e36ebd745c65","Payload":"{ \"this\" : \"is a fake receipt\" }"}
			}
			else{
				ViewManager.inst.ShowText(s.ToString());
			}

		});
		#endif
	}
	private void OnPayFunOver(VoHttp vo)
	{
//		Dictionary<string,object> dic = new Dictionary<string, object> ();
//		Dictionary<string,object> dic2 = new Dictionary<string, object> ();
//		Dictionary<string,object> dic3 = new Dictionary<string, object> ();
//		dic2.Add ("coin", ModelManager.inst.userModel.coin + currNum);
//		dic.Add ("user", dic2);
		ModelManager.inst.userModel.UpdateData (vo.data);
		ViewManager.inst.ShowText (Tools.GetMessageById ("33050", new string[]{ currNum.ToString() }));
	}
	public override void Clear ()
	{
		base.Clear ();
	}
}
