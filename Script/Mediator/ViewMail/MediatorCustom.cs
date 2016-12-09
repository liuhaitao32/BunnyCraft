using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;

public class MediatorCustom : BaseMediator {
	private GTextInput tt;
	private GList list;
	private object[] dic;
	private ModelUser userModel;
	private object[] language;
	private string kefuName  = "sever";
	public override void Init()
	{
		base.Init ();
		this.Create (Config.VIEW_CUSTOM,false,Tools.GetMessageById("21008"));

		userModel = ModelManager.inst.userModel;

		tt = this.GetChild ("n5").asTextInput;
		//tt.maxLength = 40;
		tt.promptText = Tools.GetMessageById ("21014");
        tt.onChanged.Add(() => {
            tt.text = Tools.GetStringByLength(tt.text, 40);
            tt.text = Tools.StrReplace(tt.text);
        });

        GButton btn = this.GetChild ("n4").asButton;
        btn.text = Tools.GetMessageById("13064");
		btn.onClick.Add (OnClickBtn);

		NetHttp.inst.Send (NetBase.HTTP_GET_BUG_MSG, "", GetMsg);

		list = this.GetChild ("n1").asList;
        list.SetVirtual();
		list.itemRenderer = OnRander;

		language = (object[])DataManager.inst.systemSimple["language"];
		List<object[]> arr=new List<object[]>();
		foreach (object[] v in language)
		{
			arr.Add(v);
		}
		if (PlatForm.inst.language != "") {
			foreach (object[] v in arr) {
				if (v [0].ToString ().Equals (PlatForm.inst.language)) {
					kefuName = v [2].ToString ();
				}
			}
		}
	}
	private void OnRander(int index,GObject go)
	{
		Dictionary<string,object> _data = (Dictionary<string,object>)dic [index];
		go.asCom.GetController ("c1").selectedIndex = (int)_data ["st"] == 0 ? 1 : 0;
		GComponent nm;
		GTextField txt;
		GTextField name;
		GTextField time;
		DateTime times = (DateTime)_data["time"];
		GButton head;
		if ((int)_data ["st"] == 0) {
			nm = go.asCom.GetChild ("n1").asCom;
			txt = nm.GetChild ("n2").asTextField;
			name = nm.GetChild ("n1").asTextField;
			time = nm.GetChild ("n4").asTextField;
			head = nm.GetChild ("n0").asButton;

			name.text = userModel.GetUName ();
			Tools.SetLoaderButtonUrl (head, ModelUser.GetHeadUrl (userModel.head ["use"].ToString ()));
			txt.text = _data ["content"].ToString ();
		} else {
			nm = go.asCom.GetChild ("n0").asCom;
			txt = nm.GetChild ("n2").asTextField;
			name = nm.GetChild ("n1").asTextField;
			time = nm.GetChild ("n4").asTextField;
			head = nm.GetChild ("n0").asButton;
			Tools.SetLoaderButtonUrl (head, ModelUser.GetHeadUrl (DataManager.inst.systemSimple ["service_icon"].ToString (),true));
			txt.text = _data ["content"].ToString ().Split ('|') [1];
			name.text = kefuName;
		}
		Tools.DataTimeFormat(time,times,0);
        //		time.text = times.ToString ();
        //if (txt.textHeight > 31)
        //{
            //nm.GetChild("n5").height = 49;
            //float c = txt.textHeight - 31;
            //nm.GetChild("n5").height = nm.GetChild("n5").height + c;
            //go.height = (90 - 31) + txt.textHeight + 10;
            //			it.GetChild ("n4").y = it.GetChild ("n4").y + c;
            go.height = txt.y + txt.textHeight + 40 + 30;

            nm.GetChild("n5").height = txt.textHeight + 33;

            time.y = nm.GetChild("n5").y + nm.GetChild("n5").height + 10;
        //}
        //else
        //{
        //    go.height = 115 + 10;
        //    //			it.GetChild ("n4").y = 90;
        //}



    }
	private void GetMsg(VoHttp vo)
	{
		int kefu = userModel.Get_NoticeState(ModelUser.RED_BUGMSG);
		if (kefu > 0) {
			userModel.Del_Notice (ModelUser.RED_BUGMSG);
		}
		dic = new object[100];
		int index = 0;
		object[] ddd = (object[])vo.data;
		for (int i = 0; i < ddd.Length; i++) {
			if ((int)((Dictionary<string,object>)ddd [i]) ["st"] < 2) {
				dic [index] = ddd [i];
				index++;
			}
		}
		list.numItems = index;
		if (index>= 1) {
			list.ScrollToView (index-1);
		}
	}
	private void OnClickBtn()
	{
        if (tt.text != "") {
            tt.text=tt.text.Trim();
			NetHttp.inst.Send (NetBase.HTTP_SEND_BUG_MSG, "content="+tt.text, OnSendMsgFunction);
		}
	}
	private void OnSendMsgFunction(VoHttp vo)
	{
		tt.text = "";
		NetHttp.inst.Send (NetBase.HTTP_GET_BUG_MSG, "", GetMsg);
	}
	public override void Clear()
	{
		base.Clear ();
	}
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView(this);
    }
}
