using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using FairyGUI;

public class MediatorGift : BaseMediator {
    private GList list_notice;

    private ModelRole roleModel;
    private object[] data;
    private List<object> arrData;
    private Dictionary<string, object> giftData;
    private ModelUser userModel;

    public override void Init()
    {
        roleModel = ModelManager.inst.roleModel;
        Create(Config.VIEW_GIFT,false,Tools.GetMessageById("14059"));
        AddGlobalListener(MainEvent.GIFT_UPDATE, ViewUpdata);
        FindObject();
        initData();
    }

    private void ViewUpdata(MainEvent e)
    {
        Dictionary<string,object> data=(Dictionary<string,object>)e.data;
        if ((int)data["type"] == 1) {
            arrData.RemoveAt(roleModel.selectIndex);
            roleModel.selectIndex = 0;
            list_notice.numItems = arrData.Count;
//            isFirst = false;
        }
       
    }

    private void FindObject()
    {
		list_notice=this.GetChild("n1").asList;
        
    }

    private void initData()
    {
        userModel = ModelManager.inst.userModel;
        arrData = new List<object>();
        NetHttp.inst.Send(NetBase.HTTP_GIFT, "", (VoHttp vo) =>
        {
            giftData = (Dictionary<string, object>)(vo.data);
            data = (object[])giftData["data"];
            
            for(int i = 0; i < data.Length; i++)
            {
                Dictionary<string,object> dd=(Dictionary<string,object>)data[i];
                if ((int)dd["status"] == 0|| (int)dd["status"] == 1) {
                    dd["index"] = i + "";
                    arrData.Add(dd);
                }
                

            }
            InitItem();

        });
    }

    private void InitItem()
    {
        list_notice.emptyStr = Tools.GetMessageById("21012");
        if (arrData.Count <= 4)
        {
            list_notice.scrollPane.touchEffect = false;
        }
        else
        {
            list_notice.scrollPane.touchEffect = true;

        }
        list_notice.itemRenderer = OnRender;
        list_notice.numItems = arrData.Count;
    }

    private void OnRender(int index,GObject item)
    {
        Dictionary<string,object> dd=(Dictionary<string,object>)arrData[index];
        GLoader icon= item.asCom.GetChild("n3").asLoader;
        GTextField name = item.asCom.GetChild("n2").asTextField;
        GTextField date = item.asCom.GetChild("n0").asTextField;
        GButton btn_mask = item.asCom.GetChild("n4").asButton;
        btn_mask.RemoveEventListeners();
        btn_mask.onClick.Add(()=> {
            roleModel.selectIndex = index;
            roleModel.giftIndex = (string)dd["index"];
            roleModel.giftData = dd;
            if((int)dd["status"] == 0)
            {
				Dictionary<string, object> d = new Dictionary<string, object>();
                d["index"] = dd["index"] + "";
                d["rid"] = dd["rid"] + "";
                NetHttp.inst.Send(NetBase.HTTP_ISOPEN, d, (VoHttp vo) =>
                {
                    dd["status"] = 1;
                    icon.url = Tools.GetResourceUrl("Image:icon_youjian2");
                    Dictionary<string, object> da = new Dictionary<string, object>();
                    da["gift_msg"] = vo.data.ToString();
                    userModel.notice["gift_msg"] = vo.data.ToString();
                    DispatchGlobalEvent(new MainEvent(MainEvent.RED_UPDATE, da));
                });
            }
			ViewManager.inst.ShowView<MediatorAddGift>();

        });
        if ((int)dd["status"] == 0)
        {
            icon.url = Tools.GetResourceUrl("Image:icon_youjian1");
        }
        else
        {
            icon.url = Tools.GetResourceUrl("Image:icon_youjian2");
        }
           
        name.text = (string)dd["name"];
        DateTime time=(DateTime)dd["time"];
        Tools.DataTimeFormat(date,time,0);
    }

    public override void Clear()
    {
		base.Clear();
		RemoveGlobalListener(MainEvent.GIFT_UPDATE, ViewUpdata);
    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView(this);
    }
}
