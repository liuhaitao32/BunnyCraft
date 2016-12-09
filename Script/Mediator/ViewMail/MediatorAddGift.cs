using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using FairyGUI;
using System.Linq;

public class MediatorAddGift : BaseMediator
{
	private List<object> giftDataIcon;
	private GTextField title;
	private GTextField list_content;
	private GList list_icon;
	private GButton btn_ok;
	private ModelRole roleModel;
	private Dictionary<string, object> addGift;
	private Dictionary<string, object> reward;
	private object index;
	private Dictionary<string, object> award;
	private List<object> data;
	private ModelUser userModel;
	private Dictionary<string, object> d;

	public override void Init ()
	{
		roleModel = ModelManager.inst.roleModel;
		userModel = ModelManager.inst.userModel;
		Create (Config.VIEW_ADDGIFT, false, (string)roleModel.giftData ["name"]);
		this.visible = false;
		FindObject ();
		InitData ();
		AddGlobalListener (MainEvent.EXPLORE_GIFT, OpenView);
	}

	private void OpenView (MainEvent obj)
	{
		userModel.UpdateData (d);
	}

	public void InitView ()
	{
		this.visible = true;
	}

	private void FindObject ()
	{
		list_content = this.GetChild ("n3").asTextField;
		list_icon = this.GetChild ("n4").asList;
		btn_ok = this.GetChild ("n5").asButton;
		btn_ok.text = Tools.GetMessageById ("21009");
		btn_ok.onClick.Add (OkEvent);
        
	}

	private void OkEvent ()
	{
        
		Dictionary<string,object> dd = new Dictionary<string, object> ();
		dd ["index"] = roleModel.giftIndex;
		dd ["rid"] = addGift ["rid"];
		NetHttp.inst.Send (NetBase.HTTP_ADDGIFT, dd, (VoHttp vo) =>
		{
            int gold1 = userModel.gold;
			ViewManager.inst.CloseView (this);
			d = (Dictionary<string, object>)vo.data;
            Dictionary<string, object> da = (Dictionary<string, object>)d ["award_gift"];
			roleModel.giftData ["status"] = -1;
			Dictionary<string, object> data1 = new Dictionary<string, object> ();
			data1 ["type"] = 1;
			data1 ["value"] = 0;
			DispatchGlobalEvent (new MainEvent (MainEvent.GIFT_UPDATE, data1));
			int el_score = userModel.el_score;
			userModel.UpdateData (vo.data);
            userModel.el_score = el_score;
			reward.Remove ("award");
			foreach (Dictionary<string, object> o in da.Values)
			{
				foreach (string v in o.Keys)
				{
					if (reward.ContainsKey (v))
					{
						switch (v)
						{
						case "card":
							Dictionary<string, object> o1 = (Dictionary<string, object>)o [v];
							foreach (string v1 in o1.Keys)
							{
								Dictionary<string, object> data = (Dictionary<string, object>)reward [v];
								if (data.ContainsKey (v1))
								{
									((Dictionary<string, object>)reward [v]) [v1] = (int)data [v1] + (int)o1 [v1];
								}
								else
								{
									((Dictionary<string, object>)reward [v]).Add (v1, o1 [v1]);
								}
							}

							break;
						default:
							int n1 = (int)reward [v];
							int n2 = (int)o [v];
							n1 += n2;
							reward [v] = n1;
							break;
						}
                        
					}
					else
					{
						reward.Add (v, o [v]);
					}
				}
			}
            int gold = reward.ContainsKey("gold")?Convert.ToInt32(reward["gold"]):0;
            if(gold + gold1 != userModel.gold) {
                userModel.isShowText = true;
            }
            
            ViewManager.inst.ShowGift (reward, Config.EFFECT_MAILBOX);
			MediatorGiftShow.isExplore = true;

		});
	}

	private void InitData ()
	{
        
		addGift = roleModel.giftData;
		reward = (Dictionary<string, object>)addGift ["rewards_dict"];
		data = Tools.GetReward (reward);
//        if(!isFirst)
		InitItem ();
		InitView ();
	}

	private void InitItem ()
	{
		list_content.text = (string)addGift ["info"];
		if ((int)addGift ["status"] == -1)
		{
			btn_ok.text = Tools.GetMessageById ("21010");
			btn_ok.touchable = false;
		}
		list_icon.itemRenderer = OnIconRender;
		list_icon.SetVirtual ();
		list_icon.numItems = data.Count;
	}

	private void OnIconRender (int index, GObject item)
	{
		ComIcon icon = item.asCom.GetChild ("n0") as ComIcon;
		Dictionary<string,object> dc = (Dictionary<string,object>)data [index];
		Dictionary<string, object> dd = new Dictionary<string, object> ();
		Dictionary<string, object> ds = new Dictionary<string, object> ();
        bool isCar = false;
        bool isShip = false;
        foreach (string v in dc.Keys)
		{
			if (v.ToString ().StartsWith ("C"))
			{
				isCar = true;
				dd ["name"] = v.ToString ();
				dd ["value"] = dc [v.ToString ()];
			}
          
            //			icon.SetData (v, dc [v], 1);
        }
        if (isCar)
        {

            icon.SetData(dd["name"].ToString(), dd["value"]);
        }
        else
        {
            icon.SetData(dc);
        }
	}

	public override void Clear ()
	{
		RemoveGlobalListener (MainEvent.EXPLORE_GIFT, OpenView);
	}

}
