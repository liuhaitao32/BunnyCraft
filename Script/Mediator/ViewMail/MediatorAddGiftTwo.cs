using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using FairyGUI;

public class MediatorAddGiftTwo : BaseMediator {
    private List<object> giftDataIcon;
    private GTextField title;
    private GTextField list_content;
    private GList list_icon;
    private GButton btn_ok;
    private ModelRole roleModel;
    private Dictionary<string, object> addGift;
    private Dictionary<string, object> reward;
    private List<object> data;
    private object index;

    public List<object> giftDataContent { get; private set; }

    public override void Init()
    {
        //Create(Config.VIEW_ADDGIFTTWO);
      
        initData();
        FindObject();
        InitView();
    }

    private void FindObject()
    {
		title=this.GetChild("n1").asTextField;
        title.text = Tools.GetMessageById("19008");
		list_icon = this.GetChild("n4").asList;
		btn_ok = this.GetChild("n5").asButton;
        btn_ok.text = Tools.GetMessageById("31005");
        btn_ok.onClick.Add(OkEvent);
    }

    private void OkEvent()
    {
        if (reward.ContainsKey("award"))
        {
            //打开宝箱的动作
            ViewManager.inst.CloseView(this);
            roleModel.giftData = null;
            roleModel.giftIndex = "";
        }
        else
        {
            ViewManager.inst.CloseView(this);
            roleModel.giftData = null;
            roleModel.giftIndex = "";
        }
        
        
    }

    private void initData()
    {
        roleModel = ModelManager.inst.roleModel;
        addGift=roleModel.giftData;
        reward=(Dictionary<string, object>)addGift["rewards_dict"];
        data = new List<object>();
        data= Tools.ConvertDicToList(reward, "name");
    }

    private void InitView()
    {
        list_icon.itemRenderer = OnIconRender;
        list_icon.SetVirtual();
        list_icon.numItems = data.Count;
    }

    private void OnIconRender(int index,GObject item)
    {
        
        GLoader img_1 =item.asCom.GetChild("n0").asLoader;
        GLoader img_2 = item.asCom.GetChild("n1").asLoader;
        GLoader img_3 = item.asCom.GetChild("n2").asLoader;
        GTextField num= item.asCom.GetChild("n4").asTextField;
        Dictionary<string,object> dd=(Dictionary<string,object>)data[index];
        if (dd["name"].Equals("body")) {
           
        }
        if (dd["name"].Equals("el_score"))
        {
            //img_1.url = Tools.GetResourceUrl("Image:" + "bg_kapai2");
            img_2.url = Tools.GetResourceUrl("Image:" + "bg_baoxiang");
            img_3.url = Tools.GetResourceUrl("Image:" + "icon_zs");
            num.text = "战斗积分"+ "X"+dd["el_score"];
        }
        if (dd["name"].Equals("gold"))
        {
            //img_1.url = Tools.GetResourceUrl("Image:" + "bg_kapai2");
            img_2.url = Tools.GetResourceUrl("Image:" + "bg_baoxiang");
            img_3.url = Tools.GetResourceUrl("Image:" + "icon_zs");
            num.text = "X"+dd["gold"];
        }
        if (dd["name"].Equals("award"))
        {
            object[] r=(object[])dd["redbag3"];

            //img_1.url = Tools.GetResourceUrl("Image:" + "bg_kapai2");
            img_2.url = Tools.GetResourceUrl("Image:" + "bg_baoxiang");
            img_3.url = Tools.GetResourceUrl("Image:" + r[1]);
            num.text = Tools.GetMessageById((string)r[0]);
        }
        if (dd["name"].Equals("exp"))
        {
            img_2.url = Tools.GetResourceUrl("Image:" + "bg_baoxiang2");
            //img_3.url = Tools.GetResourceUrl("Image:" + "icon_xm");
            num.text = "经验"+ "X"+dd["exp"];
        }
        if (dd["name"].Equals("coin"))
        {
            img_2.url = Tools.GetResourceUrl("Image:" + "bg_baoxiang2");
            img_3.url = Tools.GetResourceUrl("Image:" + "icon_xm");
            num.text = "X"+dd["coin"];
        }
        if (dd["name"].Equals("redbag_coin"))
        {
            img_2.url = Tools.GetResourceUrl("Image:" + "bg_baoxiang2");
            img_3.url = Tools.GetResourceUrl("Image:" + "icon_hongbaoquan");
            num.text = "X"+dd["redbag_coin"];
        }
        if (dd["name"].Equals("rank_score"))
        {
            img_2.url = Tools.GetResourceUrl("Image:" + "bg_baoxiang2");
            img_3.url = Tools.GetResourceUrl("Image:" + "icon_xm");
            num.text = "排位积分"+ "X"+dd["rank_score"];
        }
    }
    public override void Clear()
    {
        
    }

}
