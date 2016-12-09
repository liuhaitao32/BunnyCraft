using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using FairyGUI;
using DG.Tweening;

public class MediatorItemColor : BaseMediator
{
	private GList l_list;
	private List<object> list;

	private Dictionary<string,object> userData;
	private Dictionary<string,object> cfgDa;
	private int moveToIndex = 0;

	//	private GButton btn_move;
	private GameObject fly001;
	private GLoader bigImage;
	private GTextField shipInfo;

	private string clickID = "";
	private bool canPanel = false;
    private int MaxNum = 0;
    private GButton Btn_new;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_ITEMCOLOR);
//		this.x = 0;
//		this.y = 70;
        
		userData = Tools.Clone ((Dictionary<string,object>)ModelManager.inst.userModel.records ["body"]) as Dictionary<string,object>;
		cfgDa = Tools.Clone (DataManager.inst.body) as Dictionary<string,object>;
		object[] ship_show = (object[])DataManager.inst.systemSimple ["ship_show"];
		bigImage = this.GetChild ("n8").asLoader;
		shipInfo = this.GetChild("n33").asCom.GetChild ("n0").asTextField;

		EffectManager.inst.EffectAlpha (this.GetChild ("n0").asImage, 0);
		EffectManager.inst.EffectAlpha (this.GetChild ("n1").asImage, 0);
		list = new List<object> ();
//		int index = 0;
//		foreach (string i in userData.Keys)
//		{
//			Dictionary<string,object> da = (Dictionary<string,object>)userData [i];
//			da.Add ("id", i);
//			list.Add (da);
//			if (i == (string)ModelManager.inst.userModel.records ["use_body"])
//			{
//				moveToIndex = index;
//			}
//			index++;
//		}
		this.GetChild ("n18").asButton.text = Tools.GetMessageById ("19891");
		this.GetChild ("n12").asTextField.text = Tools.GetMessageById ("19892");
		/* "s:int:0", */
//		Tools.Sort (list, new string[]{"t:datetime:0" });

		List<object> listt = new List<object> ();
		for (int j = 0; j < ship_show.Length; j++) {
			if (!userData.ContainsKey (ship_show [j].ToString ())) {
				Dictionary<string,object> da = (Dictionary<string,object>)cfgDa [ship_show [j].ToString ()];
				da.Add ("id", ship_show [j].ToString ());
				listt.Add (da);
			} else {
				Dictionary<string,object> das = (Dictionary<string,object>)userData [ship_show [j].ToString ()];
				das.Add ("id", ship_show [j].ToString ());
				listt.Add (das);
				if (ship_show [j].ToString () == (string)ModelManager.inst.userModel.records ["use_body"])
				{
					moveToIndex = j;
				}
			}
		}
//		Tools.Sort (listt, new string[]{ "id:string:0" });
		for (int ii = 0; ii < listt.Count; ii++)
		{
			list.Add (listt [ii]);
		}
		SetUrlShow (list);
		l_list = this.GetChild ("n2").asList;
		l_list.itemRenderer = List_Render;
		l_list.SetVirtual ();
		l_list.numItems = list.Count;

        l_list.onTouchEnd.Add(() => {
            int scrollIndex = l_list.GetFirstChildInView();
            //Debug.Log(scrollIndex+"      "+MaxNum);
            if(scrollIndex + 6 >= MaxNum||GetIndexNum()=="") {
                this.GetChild("n24").visible = false;
            } else {
                //Debug.Log("222222222222222");
                //this.GetChild("n24").visible = true;
            }
        });

        

        for(int iii=0; iii<7;iii++) {
            if(GetIndexNum().IndexOf(iii.ToString()) != -1) {
                this.GetChild("n24").visible = false;
           }
        }

		this.GetChild ("n5").asButton.onTouchBegin.Add (OnTouchBegin);
		this.GetChild ("n5").asButton.onTouchEnd.Add (OnTouchEnd);
//		TimerManager.inst.Add (0.01f, 0, OnTimerFunction);
		Stage.inst.onTouchMove.Add (OnTouchMove);

		this.GetChild ("n11").asButton.onClick.Add (OnUnLockShip);
		this.GetChild ("n18").asButton.touchable = false;
		this.GetChild ("n19").asLoader.onClick.Add (()=>{
			canPanel = true;
			SetRed(this.GetChild ("n19").asLoader.data.ToString());
			_changeID = this.GetChild ("n19").asLoader.data.ToString();
			SetUrlShow(list);
			listUpdate();
		});
		this.GetChild ("n20").asLoader.onClick.Add (()=>{
			canPanel = true;
			SetRed(this.GetChild ("n20").asLoader.data.ToString());
			_changeID = this.GetChild ("n20").asLoader.data.ToString();
			SetUrlShow(list);
			listUpdate();
		});
		this.GetChild ("n21").asLoader.onClick.Add (()=>{
			canPanel = true;
			SetRed(this.GetChild ("n21").asLoader.data.ToString());
			_changeID = this.GetChild ("n21").asLoader.data.ToString();
			SetUrlShow(list);
			listUpdate();
		});

        Btn_new = this.GetChild("n34").asButton;
        Btn_new.onClick.Add(()=> {
            canPanel = true;
            //SetRed(this.GetChild("n21").asLoader.data.ToString());
            //_changeID = this.GetChild("n21").asLoader.data.ToString();
            l_list.ScrollToView(MaxNum);
            this.GetChild("n24").visible = false;
            listUpdate();
        });

        //AddGlobalListener(MainEvent.VIEWCOLOR,ViewColor);
	}
    private void showRed() {
    }
	private bool canMove = false;
	private float mouseX;
	private float mouseY;
	Vector3 StartPosition;
	Vector3 previousPosition;
	Vector3 offset;
	Vector3 finalOffset;
	Vector3 eulerAngle;

	bool isSlide = true;
	float angle;
		private void OnTimerFunction(float time)
		{
		
			if (isSlide)
			{
			fly001.transform.Rotate (new Vector3 (0f, 0f, 0.2f));
//				fly001.transform.Rotate(Vector3.Cross(finalOffset, Vector3.forward).normalized, angle * 2 * Time.deltaTime, Space.World);
//				if (angle > 0)
//				{
//					angle -= 5;
//				}
//				else
//				{
//					angle = 0;
//				}
			}
		}
	private void OnTouchBegin ()
	{
		canMove = true;
		StartPosition = Input.mousePosition;  
		previousPosition = Input.mousePosition; 
	}

	private void OnTouchEnd ()
	{
		canMove = false;
		finalOffset = Input.mousePosition - StartPosition;
		isSlide = true;  
		angle = finalOffset.magnitude;  
	}

	private void OnTouchMove ()
	{
		if (canMove && fly001 != null)
		{
			isSlide = false;
			offset = (Input.mousePosition - previousPosition)* 0.1f; 
			previousPosition = Input.mousePosition;

			Vector3 v3 = Vector3.Cross (offset, Vector3.forward).normalized ;
			fly001.transform.Rotate (v3, offset.magnitude, Space.World);
			Vector3 rev= fly001.transform.rotation.eulerAngles;
			if (fly001.transform.rotation.eulerAngles.x < 90 || fly001.transform.rotation.eulerAngles.x > 300) {
				fly001.transform.Rotate (-v3, offset.magnitude, Space.World);
			}
		}
	}


	private void SetUrlShow(List<object> shipList)
	{
		int nu = 21;
		for (int i = 0; i < shipList.Count; i++) {
			Dictionary<string,object> data = (Dictionary<string,object>)shipList [i];
			string id = ((Dictionary<string,object>)data) ["id"] as string;
			if (data.ContainsKey ("show")&&(int)data["show"] == 1&&!GetCanRed(id)) {
				if (nu < 19)
					break;
				this.GetChild ("n" + nu).asLoader.url = Tools.GetResourceUrl ("Icon:" + id);
				this.GetChild ("n" + nu).asLoader.data = id;
				nu--;
			}
		}
		this.GetChild ("n26").x = 684;
		this.GetChild ("n29").x = 733;
		this.GetChild ("n31").x = 784;

		this.GetChild ("n31").visible = false;
		this.GetChild ("n29").visible = false;
		this.GetChild ("n26").visible = false;

		if (nu == 18) {
//			this.GetChild ("n24").x = 684f + 60f;
		}
		else if (nu == 19) {
			this.GetChild ("n31").visible = false;
			this.GetChild ("n29").x = 784f;
			this.GetChild ("n26").x = 733f;
//			this.GetChild ("n24").x = 733f + 60f;
		}
		else if (nu == 20) {
			this.GetChild ("n31").visible = false;
			this.GetChild ("n29").visible = false;
			this.GetChild ("n26").x = 784f;
//			this.GetChild ("n24").x = 784f + 60f;
		}
		if (nu == 21) {
			this.GetChild ("n31").visible = false;
			this.GetChild ("n29").visible = false;
			this.GetChild ("n26").visible = false;
			this.GetChild ("n24").visible = false;
		} else {
			//this.GetChild ("n24").visible = true;
		}
	}

	private void SetRed(string sid)
	{
		string str = LocalStore.GetLocal (LocalStore.LOCAL_SHIPRED);
		if (str == null) {
			str = "";
		}
		if (str.IndexOf (sid) != -1)
			return;
		str+= sid+"|";
		LocalStore.SetLocal (LocalStore.LOCAL_SHIPRED, str);
	}
	private bool GetCanRed(string sid)
	{
		string str = LocalStore.GetLocal (LocalStore.LOCAL_SHIPRED);
		if (str == null) {
			return false;
		} else {
			if (str.IndexOf (sid) != -1)
				return true;
			else
				return false;
		}
	}
	private void OnUseShip()
	{
//		NetHttp.inst.Send (NetBase.HTTP_USE_BODY, "body_id=" + _changeID, SetBodyNow);
//		EffectManager.inst.AddPrefab (Config.EFFECT_UNLOCKSKIN, this.GetChild ("n25").asGraph);
	}
	private void OnUnLockShip()
	{
//		EffectManager.inst.AddPrefab (Config.EFFECT_UNLOCKSKIN, this.GetChild ("n25").asGraph);
//		return;
		if(userData.ContainsKey(_changeID))
		{
			NetHttp.inst.Send (NetBase.HTTP_USE_BODY, "body_id=" + _changeID, SetBodyNow);
		}else{
			Dictionary<string,object> shipData = (Dictionary<string,object>)(DataManager.inst.body [_changeID]);
			if (shipData.ContainsKey(Config.ASSET_GOLD)) {
				if (!ModelUser.GetCanBuy (Config.ASSET_GOLD, (int)shipData [Config.ASSET_GOLD]))
					return;
			}
			if (shipData.ContainsKey(Config.ASSET_COIN)) {
				if (!ModelUser.GetCanBuy (Config.ASSET_COIN, (int)shipData [Config.ASSET_COIN]))
					return;
			}
			NetHttp.inst.Send (NetBase.HTTP_BUY_BODY, "body_id=" + _changeID, OnUnlockBodyHandler);
		}
	}
	private void OnUnlockBodyHandler(VoHttp vo)
	{
		ModelManager.inst.userModel.UpdateData (vo.data);
		canPanel = false;
		listUpdate ();
		if (obj != null) {
            obj.asButton.GetChild("n1").visible = false;
            ColorFilter cf = new ColorFilter ();
			obj.asButton.GetChild ("n5").asLoader.filter = cf;
			cf.AdjustSaturation (-1);
			float c = -1;
			DOTween.To (() => c, x => c = x, 0, 1f).OnUpdate (() =>
			{
				cf = new ColorFilter ();
				cf.AdjustSaturation(c);
				obj.asButton.GetChild ("n5").asLoader.filter = cf;
				
				}).OnComplete(()=>{
				cf = new ColorFilter ();
//				cf.AdjustSaturation(c);
				cf.AdjustSaturation(0);
                
             }).OnComplete(()=> { obj.asButton.GetChild("n1").visible = true; });

		}
		EffectManager.inst.AddPrefab (Config.EFFECT_UNLOCKSKIN, this.GetChild ("n25").asGraph);

		if (vo.data != null) {
			
		}
	}
	private void listUpdate ()
	{

		userData = Tools.Clone ((Dictionary<string,object>)ModelManager.inst.userModel.records ["body"]) as Dictionary<string,object>;
		cfgDa = Tools.Clone (DataManager.inst.body) as Dictionary<string,object>;
		object[] ship_show = (object[])DataManager.inst.systemSimple ["ship_show"];

		list = new List<object> ();
//		int index = 0;
//		foreach (string i in userData.Keys)
//		{
//			Dictionary<string,object> da = (Dictionary<string,object>)userData [i];
//			da.Add ("id", i);
//			list.Add (da);
//			if (i == (string)ModelManager.inst.userModel.records ["use_body"])
//			{
//				moveToIndex = index;
//			}
//			index++;
//		}
		/*"s:int:0",*/
//		Tools.Sort (list, new string[]{"t:datetime:0" });

		List<object> listt = new List<object> ();
		for (int j = 0; j < ship_show.Length; j++) {
			if (!userData.ContainsKey (ship_show [j].ToString ())) {
				Dictionary<string,object> da = (Dictionary<string,object>)cfgDa [ship_show [j].ToString ()];
				da.Add ("id", ship_show [j].ToString ());
				listt.Add (da);
			} else {
				Dictionary<string,object> das = (Dictionary<string,object>)userData [ship_show [j].ToString ()];
				das.Add ("id", ship_show [j].ToString ());
				listt.Add (das);
				if (ship_show [j].ToString () == (string)ModelManager.inst.userModel.records ["use_body"])
				{
					moveToIndex = j;
				}
			}
		}
		//		Tools.Sort (listt, new string[]{ "id:string:0" });
		for (int ii = 0; ii < listt.Count; ii++)
		{
			list.Add (listt [ii]);
		}
		SetUrlShow (list);
		l_list.numItems = list.Count;
	}

	private void List_Render (int index, GObject go)
	{
		Dictionary<string,object> data = (Dictionary<string,object>)list [index];
		string id = ((Dictionary<string,object>)data) ["id"] as string;
//		GImage ii = go.asCom.GetChild ("n3").asImage;
		if (id.IndexOf ("team") != -1)
			return;
		Dictionary<string,object> shipData = (Dictionary<string,object>)(DataManager.inst.body [id]);
		string nameid = shipData ["name"].ToString ();
		string infoid = "";
		if (shipData.ContainsKey ("info")) {
			infoid = Tools.GetMessageById (shipData ["info"].ToString ());
		}
        if(shipData.ContainsKey("show")&&(int)shipData["show"]==1&&!GetCanRed(nameid)) {
            go.asCom.GetChild("n14").visible = true;
            MaxNum = index;
            //this.DispatchGlobalEvent(new MainEvent(MainEvent.VIEWCOLOR));
            if(GetIndexNum().IndexOf(index.ToString())==-1) {
                SetIndexNum(index.ToString());
            }
        } else {
            go.asCom.GetChild("n14").visible = false;
        }

        GLoader img = go.asCom.GetChild("n1").asLoader;
		GLoader icon = go.asCom.GetChild ("n5").asLoader;
		icon.url = Tools.GetResourceUrl ("Icon:" + id);
		Controller c1 = this.GetController ("c1");
        if((string)ModelManager.inst.userModel.records["use_body"] == id) {
            go.asCom.GetChild ("n10").asImage.visible = true;
            go.asCom.GetChild("n9").asTextField.visible = true;
        } else {
            go.asCom.GetChild ("n10").asImage.visible = false;
            go.asCom.GetChild("n9").asTextField.visible = false;
        }
            go.asCom.GetChild("n8").visible = false;
		this.GetChild ("n11").asButton.text = "";
		if (_changeID != "") {
			Dictionary<string,object> shipData1 = (Dictionary<string,object>)(DataManager.inst.body [_changeID]);
			if (shipData1.ContainsKey ("info")) {
				infoid = Tools.GetMessageById (shipData1 ["info"].ToString ());
			}
			bigImage.url = Tools.GetResourceUrl ("Icon:" + "ship_" + _changeID.Split ('p') [1]);
			shipInfo.text = infoid;
			this.GetChild ("n4").asTextField.text = Tools.GetMessageById (shipData1 ["name"].ToString ());

			if (userData.ContainsKey (_changeID)) {
				
				if ((string)ModelManager.inst.userModel.records ["use_body"] != _changeID) {
					c1.selectedIndex = 4;
					this.GetChild ("n11").asButton.text = Tools.GetMessageById ("19890");
				} else {
					c1.selectedIndex = 3;
				}
			}else if(!userData.ContainsKey (_changeID))
			{
				if (shipData1.ContainsKey (Config.ASSET_COIN) && shipData1.ContainsKey (Config.ASSET_GOLD)) {
					c1.selectedIndex = 2;
					this.GetChild ("n13").asTextField.text = ((int)shipData1 [Config.ASSET_GOLD]).ToString ();
					this.GetChild ("n15").asTextField.text = ((int)shipData1 [Config.ASSET_COIN]).ToString ();
				} else if (shipData1.ContainsKey (Config.ASSET_GOLD)) {
					c1.selectedIndex = 1;
					this.GetChild ("n13").asTextField.text = ((int)shipData1 [Config.ASSET_GOLD]).ToString ();
				} else {
					c1.selectedIndex = 0;
					this.GetChild ("n15").asTextField.text = ((int)shipData1 [Config.ASSET_COIN]).ToString ();
				}
			}
			if (_changeID == id) {
				if(canPanel)
					l_list.ScrollToView (index);
				this.GetChild ("n25").x = l_list.x + go.x + l_list.container.x + 62f;
				go.asCom.GetChild ("n8").visible = true;
			}
		} else {
			if ((string)ModelManager.inst.userModel.records ["use_body"] == id) {
				this.GetChild ("n25").x = l_list.x + go.x + l_list.container.x + 62f;
				go.asCom.GetChild ("n8").visible = true;
				bigImage.url = Tools.GetResourceUrl ("Icon:" + "ship_" + id.Split ('p') [1]);
				shipInfo.text = infoid;
				this.GetChild ("n4").asTextField.text = Tools.GetMessageById (shipData ["name"].ToString ());
				c1.selectedIndex = 3;
			}
		}
		if (userData.ContainsKey (id))
		{
//			if ((int)(((Dictionary<string,object>)(userData [id])) ["s"]) != 0)
//			{
//				ii.visible = false;
//			}
//			else
//			{
//				ii.visible = true;
//			}
			icon.enabled = true;
			go.asCom.GetChild ("n7").visible = false;
			img.url = Tools.GetResourceUrl ("Image2:n_bg_jiti1");
//			img.enabled = true;
		}
		else
		{
//			icon.url = Tools.GetResourceUrl ("Image:bg_kapai5");
//			img.enabled = false;
			icon.enabled = false;
			go.asCom.GetChild ("n7").visible = true;
			img.url = Tools.GetResourceUrl ("Image2:n_bg_jiti3");
//			ii.visible = false;
		}

		GButton btn = go.asButton;
		btn.RemoveEventListeners ();
		btn.onClick.Add (() =>
		{
			_changeID = id;
			obj= go;
			if(!GetCanRed(id))
			{
				SetRed(id);
				SetUrlShow(list);
			}
			canPanel = false;
			listUpdate ();
            go.asCom.GetChild("n14").visible = false;
            DelIndexNum(index.ToString());
            SetRed(nameid);
//			if (icon.enabled)
//			{
//				NetHttp.inst.Send (NetBase.HTTP_USE_BODY, "body_id=" + id, SetBodyNow);
//			}
		});
	}
    //private void ViewColor(MainEvent e) {
    //    Debug.Log("aaaaaaaaaaaa");
    //    this.GetChild("n24").visible = false;

    //}
	private GObject obj;
	private string _changeID = "";

	private void SetBodyNow (VoHttp vo)
	{
		if ((bool)vo.data)
		{
			ModelManager.inst.userModel.records ["use_body"] = _changeID;
			((Dictionary<string,object>)(((Dictionary<string,object>)(ModelManager.inst.userModel.records ["body"])) [_changeID])) ["s"] = 1;
			_changeID = "";
			this.DispatchGlobalEvent (new MainEvent (MainEvent.COLOR_CHANGE));
		}
		canPanel = false;
		listUpdate ();
	}

    private void SetIndexNum(string index) {
        string content = GetIndexNum();
        content += index + ",";
        LocalStore.SetLocal("IndexNum",content);
    }
    private string GetIndexNum() {
        return LocalStore.GetLocal("IndexNum");
    }
    private void DelIndexNum(string index) {
        string context = GetIndexNum();
        if(context != "") {
            string key =  index + ",";
            if(context.IndexOf(key) != -1) {
                context = context.Replace(key, "");
            }
        }
        LocalStore.DelLocal("IndexNum");
        LocalStore.SetLocal("IndexNum",context);
    }

	public override void Clear ()
	{
//		TimerManager.inst.Remove (OnTimerFunction);
		base.Clear ();
	}
}
