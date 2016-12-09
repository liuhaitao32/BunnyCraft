using FairyGUI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Events;


public class MediatorTaste : BaseMediator
{
	
	private GList list;
	private GButton btn_updata;
	private List<object> listdDataCopy;
    private List<object> listData;

    private ModelUser userModel;
    private ModelRole roleModel;
    private bool isMore = true;
    private int tag_n = 1;
    private double lon;
    private double lat;
    private int type;
    private int effort_Num;
    private int num = 5;
    private int tag = 0;
    private string btnBgUp;
    private string btnBgDo;
    private string textAttention;
    private string textAttentiond;
    private string textShield;
    private string textShieldd;
    private string textBtnStrokeColor;
    private string textBtnColor;
    private string fontSize;

    public override void Init ()
	{
		base.Init ();
		Create (Config.VIEW_TASTE);
		FindObject ();
		InitData ();

	}

	public override void Clear ()
	{
		base.Clear ();
		
    }
    private void InitItem ()
	{
        if (listData.Count != 0)
        {
            btn_updata.visible = true;
        }
        list.emptyStr = Tools.GetMessageById("19934");
		list.onChangeNum.Add (this.CheckListNum);
        list.itemRenderer = TasteRander;
        list.numItems = listData.Count;
        
    }

	private void TasteRander (int index, GObject item)
	{

		Dictionary<string, object> dc = (Dictionary<string, object>)listData [index];
		GButton photo = item.asCom.GetChild ("n1").asCom.GetChild("n0").asButton;
		GTextField name = item.asCom.GetChild ("n3").asTextField;
		GTextField guild = item.asCom.GetChild ("n4").asTextField;
		GLoader achieve = item.asCom.GetChild ("n21").asCom.GetChild ("n1").asLoader;
		GTextField status = item.asCom.GetChild ("n9").asTextField;
		GButton mask_btn = item.asCom.GetChild ("n8").asButton;
		GButton btn_attention = item.asCom.GetChild ("n7").asButton;
        GTextField attention_text = btn_attention.GetChild("title").asTextField;
        GLoader btn_image = btn_attention.asCom.GetChild ("n1").asLoader;
        btn_image.visible = true;
        GObject reletion = item.asCom.GetChild ("n2");
		GTextField rankScore = item.asCom.GetChild ("n21").asCom.GetChild ("n2").asTextField;
		GTextField lv = item.asCom.GetChild ("n1").asCom.GetChild("n2").asTextField;
		reletion.visible = false;
		btn_attention.visible = true;
        userModel.GetUnlcok(Config.UNLOCK_GUILD, guild);

        btn_attention.RemoveEventListeners ();
		btn_attention.onClick.Add (() =>
		{
            //关注的实现
			Dictionary<string, object> dd = new Dictionary<string, object> ();
			dd ["fuid"] = dc ["uid"];
			NetHttp.inst.Send (NetBase.HTTP_FOLLOW, dd, (VoHttp vo) =>
			{
				
				if ((bool)vo.data == true)
				{
                    roleModel.AddAttentionFight(dc["uid"].ToString());
                    ((Dictionary<string, object>)listData[index])["if_follow"] = true;
                    Tools.SetButtonBgAndColor(btn_attention, btnBgDo, textAttentiond, fontSize, textBtnColor, "", 0);
                    btn_attention.touchable = false;
					
                }
                else
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13120"));
                }
			});
		});
		if ((bool)dc ["if_black"])
		{//是否拉黑
            Tools.SetButtonBgAndColor(btn_attention, btnBgDo, textShieldd, fontSize, textBtnColor, "", 0);
            type = 1;
			btn_attention.touchable = false;
        }
        else if ((bool)dc["if_follow"])
        {
            //是否已关注
            Tools.SetButtonBgAndColor(btn_attention, btnBgDo, textAttentiond, fontSize, textBtnColor, "", 0);
            type = 2;
            btn_attention.touchable = false;

        }
        else
        {
            Tools.SetButtonBgAndColor(btn_attention, btnBgUp, textAttention, fontSize, textBtnColor, textBtnStrokeColor);
            btn_attention.touchable = true;
        }

        achieve.url = userModel.GetRankImg((int)dc["rank_score"]);
        mask_btn.RemoveEventListeners();
        mask_btn.onClick.Add (() =>
		{
            roleModel.SetTempData(listData,new int[] { index });
			string uid_ = dc ["uid"] + "";
			if (!uid_.Equals (userModel.uid))
			{
                this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { null, uid_, roleModel.tab_CurSelect1, roleModel.tab_CurSelect2, roleModel.tab_CurSelect3 }));
            }
			else
			{
				ViewManager.inst.ShowText (Tools.GetMessageById ("13106"));
			}
            
		});
		lv.text = dc ["lv"].ToString();
		if (Tools.IsNullEmpty(dc["uname"]))
		{
			name.text = dc ["uid"] + "";
		}
		else
		{
			name.text = dc["uname"].ToString();
		}
		string dc_ = (string)dc ["head_use"];
		Tools.SetLoaderButtonUrl (photo, ModelUser.GetHeadUrl (dc_));
        if (dc["guild_name"] == null)
        {
            guild.text = Tools.GetMessageById("19955");
        }
        else {
            guild.text = dc["guild_name"] + "";
        }
        switch (dc["type"].ToString())
        {
            case "redbag":
                status.text = Tools.GetMessageById("19963");
                break;
            case "battle_man":
                status.text = Tools.GetMessageById("19964");
                break;
            case "elv_lv":
                status.text = Tools.GetMessageById("19965");
                break;
        }
        rankScore.text = dc ["rank_score"] + "";
    }

	private void InitData ()
	{
        btnBgUp = "n_btn_12";
        btnBgDo = "n_btn_12_";
        textAttention = "19901";
        textAttentiond = "19912";
        textBtnColor = "FFFFFF";
        textBtnStrokeColor = "#4E6ED5";
        textShield = "13016";
        textShieldd = "13032";
        fontSize = "";
        roleModel = ModelManager.inst.roleModel;
		userModel = ModelManager.inst.userModel;
		effort_Num = ((Dictionary<string, object>)DataManager.inst.effort ["effort_cond"]).Count;
		listData = new List<object> ();
        if (roleModel.isSave)
        {
            roleModel.isSave = false;
            if (roleModel.tempData != null)
            {
                if (roleModel.tempOpenData != null)
                {
                    foreach (object v in (List<object>)roleModel.tempData["data"])
                    {
                        Dictionary<string, object> d = (Dictionary<string, object>)v;
                        if (roleModel.tempOpenData.ContainsKey(d["uid"].ToString()))
                        {
                            Dictionary<string, object> dd = (Dictionary<string, object>)roleModel.tempOpenData[d["uid"].ToString()];
                            d["if_follow"] = dd["if_follow"];
                            d["if_black"] = dd["if_black"];
                        }
                        listData.Add(d);
                    }
                    roleModel.tempOpenData = null;
                }
                else
                {
                    listData = (List<object>)roleModel.tempData["data"];
                   
                }
                InitItem();
                roleModel.tempData = null;
            }
        }
        else
            GetDataForNet();

    }

	private void FindObject ()
	{

		list = this.GetChild ("n0").asList;
		btn_updata = this.GetChild ("n1").asButton;
		btn_updata.text = Tools.GetMessageById ("19946");
		btn_updata.onClick.Add (BtnUpdata);
        btn_updata.visible = false;

    }

	private void BtnUpdata ()
	{


        if (listData.Count >= 5) {
            listData.Clear();
            GetDataForNet();
        }
        else
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("19944"));
        }
        
	}

	private void GetDataForNet ()
	{
		NetHttp.inst.Send (NetBase.HTTP_GETTASTE, "", (VoHttp vo) =>
		{
            tag += 1;
			object[] dc = (object[])vo.data;
            if (dc.Length > 0)
            {
                foreach (object v in dc)
                {
                    Dictionary<string, object> dd = (Dictionary<string, object>)v;
                    string uid = dd["uid"] + "";
                    if (!uid.Equals(userModel.uid))
                    {
                        dd["if_follow"] = false;
                        listData.Add(dd);
                        
                    }
                }
            }
            InitItem();
		});
	}
}
