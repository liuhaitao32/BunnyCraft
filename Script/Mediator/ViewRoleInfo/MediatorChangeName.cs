using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;

public class MediatorChangeName : BaseMediator
{
	private ModelUser userModel;
	private GTextInput input_name;
	//private GTextInput input_code;
	//private GButton btn_send;
	private GButton btn_ok;
	private Dictionary<string, object> otherInfo;
	private DateTime timer;
	private GButton close;
	private int timerDis;
    private GTextField title;

	public override void Init ()
	{
		Create (Config.VIEW_CHANGENAME,false,Tools.GetMessageById("33103"));
		InitDate ();
		InitItem ();
	}

	public override void Clear ()
	{
		base.Clear ();
		//TimerManager.inst.Remove (Timer);
	}

	private void InitDate ()
	{
		userModel = ModelManager.inst.userModel;
		otherInfo = ModelManager.inst.roleModel.otherInfo;
		timerDis = (int)DataManager.inst.systemSimple ["time_code"];
	}

	private void InitItem ()
	{
		FindObject ();
		string tel = userModel.tel;
		string old = tel.Substring (3, 4);
		tel = tel.Replace (old, "****");
	}

	private void FindObject ()
	{
        input_name = this.GetChild ("n5").asCom.GetChild ("n1").asTextInput;
		//input_name.maxLength = (int)DataManager.inst.systemSimple ["name_num"];
        input_name.onChanged.Add(() => {
            input_name.text = Tools.GetStringByLength(input_name.text, (int)DataManager.inst.systemSimple["name_num"]);
            input_name.text = Tools.StrReplace(input_name.text);
        });
        GetChild("n8").asTextField.text = Tools.GetMessageById("13100")+ "：";

		btn_ok = this.GetChild ("n0").asButton;
		btn_ok.text = Tools.GetMessageById ("13079");
		btn_ok.onClick.Add (() =>
		{
			if (input_name.text == "")
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("13033"));
			}
			//else if (input_code.text == "")
			//{
			//	ViewManager.inst.ShowText (Tools.GetMessageById ("13034"));
			//}
			else if (userModel.tel == "")
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("13035"));
			}
			else
			{
                input_name.text=input_name.text.Trim();
                if (FilterManager.inst.Exec(input_name.text).IndexOf('*') != -1)
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13139"));
                }
                else
                {
                    input_name.text = input_name.text;
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic["uname"] = input_name.text;
					dic["sex"] = ModelManager.inst.userModel.sex;
                    //dic ["sign"] = input_code.text;
                    NetHttp.inst.Send(NetBase.HTTP_CHANGE_NAME, dic, (VoHttp v) =>
                    {
                        if ((bool)v.data)
                        {
                            ViewManager.inst.ShowText(Tools.GetMessageById("13144"));
                            //                        string passWord = "";
                            //						List<string[]> list = LocalStore.GetUids ();
                            //						foreach (string[] arr in list)
                            //						{
                            //							if (arr [0].ToString ()==userModel.uid)
                            //							{
                            //								passWord = arr [2].ToString ();
                            //								break;
                            //							}
                            //						}

                            userModel.uname = userModel.unameTrue = input_name.text;
                            otherInfo["uname"] = input_name.text;
                            //						LocalStore.DelUids (userModel.uid);
                            //						if (!Tools.IsNullEmpty (passWord))
                            //						{
                            LocalStore.SetUids(userModel.uid, input_name.text, userModel.pwd, userModel.type_login, userModel.tel);
                            //						}
                            //						LocalStore.SetLocal (LocalStore.LOCAL_UNAME, input_name.text);

                            Dictionary<string, object> dc = new Dictionary<string, object>();
                            dc.Add("value", "");
                            dc.Add("tag", "uname");
                            DispatchGlobalEvent(new MainEvent(MainEvent.ROLE_UPDATE, dc));
                            ViewManager.inst.CloseView(this);

                        }
                    });
                }




             
			}
		});
	}

    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView();
    }

 //   private void Timer (float obj)
	//{
	//	if (timerDis == 0)
	//	{
	//		btn_send.text = Tools.GetMessageById ("13077");
	//		btn_send.touchable = true;
	//		TimerManager.inst.Remove (Timer);
	//		timerDis = (int)DataManager.inst.systemSimple ["time_code"];
	//	}
	//	else
	//	{
	//		timerDis -= 1;
	//		btn_send.text = Tools.GetMessageById ("13119", new object[] { timerDis });
	//	}
	//}

		
}
