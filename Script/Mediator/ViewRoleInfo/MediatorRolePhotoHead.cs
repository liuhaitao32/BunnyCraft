using UnityEngine;
using System.Collections;
using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorRolePhotoHead : BaseMediator
{
    private ModelRole roleModel;
    private ModelUser userModel;
    private int total;
    private int m_PageIndex = 1;
    private object[] roleLove;
    private GList photoList;
    private GTextField m_PanelText;
    private GButton m_BtnPrevious;
    private GButton m_BtnNext;
    private int m_PageCount;
    private int m_ItemsCount;
    private int index;
    private int number = 6;
    private GTextField title;
    private List<object> dataList;
    private GComponent g;
    private List<object> itemData;
    private string defauHead = "";
    private GButton close;
    private GImage bg_;

    public override void Init()
    {
        base.Init();
        this.Create(Config.VIEW_ROLEPHOTOHEAD, false, Tools.GetMessageById("13088"));
        InitView();
        InitData();
    }

    private void InitItems()
    {
        m_ItemsCount = dataList.Count;
        if (m_ItemsCount > 0)
        {
            g.visible = true;
            m_PageCount = (m_ItemsCount % number) == 0 ? m_ItemsCount / number : (m_ItemsCount / number) + 1;
            m_PanelText.text = Tools.SetFontColor(m_PageIndex.ToString(), m_PageCount.ToString(), null);
        }
        else
        {
            g.visible = false;
        }
        BindPage(m_PageIndex);
    }

    private void InitData()
    {
        dataList = new List<object>();
        itemData = new List<object>();
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        Tools.UpdataHeadTime();
        //是否是绑定过的账号、并且不是手机绑定、才会加入 平台头像
        if (userModel.isBingding() && userModel.type_login != Ex_Local.LOGIN_TYPE_TEL)
        {
            //
			string headHttp = LocalStore.GetLocal(LocalStore.OTHER_HEADIMG+userModel.uid);
            //
            if (headHttp != "")
            {
                Dictionary<string, object> dc = new Dictionary<string, object>();
                dc.Add("name", headHttp);
                dc.Add("status", "1");
                dataList.Add(dc);
            }
        };

        //加载上传的头像
        foreach (KeyValuePair<string, object> str in userModel.head)
        {
            if (!str.Key.Equals("use"))
            {
                Dictionary<string, object> dc1 = (Dictionary<string, object>)str.Value;
                Dictionary<string, object> dc = new Dictionary<string, object>();
                dc.Add("name", str.Key);
                dc.Add("status", dc1["status"]);
                dataList.Add(dc);
            }
        }

        if (dataList.Count > 0)
        {
            Dictionary<string, object> dc_ = new Dictionary<string, object>();
            dc_.Add("name", "h01");
            dc_.Add("status", "-2");
            dataList.Add(dc_);
        }

        InitItems();


    }

    private void InitView()
    {
        //        InitTitle(Tools.GetMessageById("13088"));
        photoList = this.GetChild("n5").asList;
        photoList.emptyStr = Tools.GetMessageById("13153");
        g = this.GetChild("n4").asCom;
        bg_ = this.GetChild("n2").asImage;
        

        m_BtnPrevious = g.GetChild("n4").asButton;
        m_BtnNext = g.GetChild("n5").asButton;
        m_PanelText = g.GetChild("n0").asTextField;
        m_BtnNext.onClick.Add(Next);
        m_BtnPrevious.onClick.Add(Previous);

    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView();
    }

    private void Previous()
    {
        if (m_PageCount <= 0)
            return;
        if (m_PageIndex <= 1)
            return;
        m_PageIndex -= 1;
        if (m_PageIndex < 1)
            m_PageIndex = 1;
        BindPage(m_PageIndex);

        m_PanelText.text = Tools.SetFontColor(m_PageIndex.ToString(), m_PageCount.ToString(), null);

    }

    private void Next()
    {
        if (m_PageCount <= 0)
            return;
        if (m_PageIndex >= m_PageCount)
            return;
        m_PageIndex += 1;
        if (m_PageIndex >= m_PageCount)
            m_PageIndex = m_PageCount;
        BindPage(m_PageIndex);
        m_PanelText.text = Tools.SetFontColor(m_PageIndex.ToString(), m_PageCount.ToString(), null);
    }

    private void BindPage(int m_PageIndex)
    {
        //if (dataList.Count == 0)
        //{
        //    bg_.visible = false;
        //}
        if (itemData.Count != 0)
            itemData.Clear();
        itemData = Tools.GetSubListPage(dataList, m_PageIndex - 1, number);
        photoList.itemRenderer = ListRendere;
        photoList.numItems = itemData.Count;

    }

    private void ListRendere(int index, GObject item)
    {
        GButton head = item.asCom.GetChild("n3").asButton;
        GLoader bg = item.asCom.GetChild("n0").asLoader;
        Dictionary<string, object> d = (Dictionary<string, object>)itemData[index];
        if (d["name"].ToString().Equals(userModel.head["use"].ToString()))
        {
            bg.url = Tools.GetResourceUrl("Image2:n_bg_touxiang2");
        }
        else
        {
        }
        Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl((string)d["name"]));
        head.RemoveEventListeners();
        head.onClick.Add(() =>
        {
//            if ((string)d["name"] == "h01")
//            {
//                return;
//            }
        //    if (!userModel.head["use"].ToString().Equals(d["name"]))
        //{
            Dictionary<string, object> data = new Dictionary<string, object>();
                data["head_key"] = d["name"];
				int statusInt = Convert.ToInt32(d["status"]);
				if (statusInt == -1)
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13150"));
                }
//				else if(statusInt == -2){
//					ViewManager.inst.ShowText(Tools.GetMessageById("13154"));
//				}
                else
                {
                    NetHttp.inst.Send(NetBase.HTTP_CHOOSE_HEAD, data, (VoHttp vo) =>
                    {
                        userModel.UpdateData(vo.data);
                        Dictionary<string, object> dc = new Dictionary<string, object>();
                        dc.Add("value", d);
                        dc.Add("tag", "photo");
                        this.DispatchGlobalEvent(new MainEvent(MainEvent.ROLE_UPDATE, dc));
                        ViewManager.inst.CloseView(this);

                    });
                }
            //}
        });



    }

    public override void Clear()
    {
        base.Clear();
    }
}
