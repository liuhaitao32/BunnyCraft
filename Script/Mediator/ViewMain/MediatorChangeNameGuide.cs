using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;

public class MediatorChangeNameGuide : BaseMediator
{
	private GTextInput txt;
	private Controller c1;
	private string sex;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_CHANGENAMEGUIDE);
		this.isAutoClose = false;

		this.GetChild ("n1").asTextField.text = Tools.GetMessageById ("13111");
		txt = this.GetChild ("n4").asTextInput;
		txt.promptText = Tools.GetMessageById ("13113");
//		txt.maxLength = 8;
        txt.onChanged.Add(() => {
			//		txt.maxLength = 8;
			txt.text = Tools.StrReplace(txt.text);
			txt.text =Tools.GetStringByLength(txt.text,(int)DataManager.inst.systemSimple["name_num"]);
        });
		this.GetChild ("n6").visible = false;//.asTextField.text = Tools.GetMessageById ("13112");

		GButton btn = this.GetChild ("n7").asButton;
		btn.text = Tools.GetMessageById ("20140");
		btn.onClick.Add (OnSureClick);
		//


		c1 = this.view.GetController ("c1");
		c1.selectedIndex = ModelManager.inst.userModel.GetSex;
		sex = ModelManager.inst.userModel.sex;
		c1.onChanged.Add (() => {
//			Debug.LogError(c1.selectedIndex+"");
			if(c1.selectedIndex==0){
				sex = "m";
			}
			else{
				sex = "f";
			}
		});
	}

	private void OnSureClick ()
	{
		if (txt.text != "")
		{
            txt.text=txt.text.Trim();
            if (FilterManager.inst.Exec (txt.text).IndexOf ('*') != -1) {
				ViewManager.inst.ShowText (Tools.GetMessageById ("13139"));
				return;
			}
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic["uname"] = txt.text;
			dic["sex"] = sex;
			if (GuideManager.inst.Check ("3:0")) {
				dic ["guide_num"] = 4;
				ModelManager.inst.userModel.SetGuide (4, ChangeOnHandler, dic);
				return;
			}
			NetHttp.inst.Send (NetBase.HTTP_CHANGE_NAME, dic, ChangeOnHandler);
		}
		else
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("13115"));
		}
	}

	private void ChangeOnHandler (VoHttp vo)
	{
		if ((bool)vo.data == true)
		{
//			Debug.LogError ();
//			if (GuideManager.inst.Check ("5:0")) {
//				GuideManager.inst.Next ();
//				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.GUIDE_UPDATE_OK, null));
//			}
			ViewManager.inst.ShowText (Tools.GetMessageById ("13114"));
			ModelManager.inst.userModel.uname = ModelManager.inst.userModel.unameTrue = txt.text;
			ModelManager.inst.userModel.sex = sex;
			ViewManager.inst.CloseView (this);
			LocalStore.SetUids(ModelManager.inst.userModel.uid, ModelManager.inst.userModel.uname,ModelManager.inst.userModel.pwd, ModelManager.inst.userModel.type_login,ModelManager.inst.userModel.tel);
			this.DispatchGlobalEvent (new MainEvent (MainEvent.USER_UPDATE));
			//
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.GUIDE_UPDATE_OVER, null));//引导全部结束
		}
	}

	public override void Clear ()
	{
		base.Clear ();
	}
}
