using UnityEngine;
using System.Collections;
using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorRoleLove : BaseMediator {
    private ModelRole roleModel;
    private ModelUser userModel;
    private int total;
    private int m_PageIndex=1;
    private object[] roleLove = new object[] { };
    private GList photoList;
    private GTextField m_PanelText;
    private GButton m_BtnPrevious;
    private GButton m_BtnNext;
    private int m_PageCount;
    private int m_ItemsCount;
    private int index;
    private int number=10;
    private GTextField title;
    private GComponent g;
    private Dictionary<string, object> otherInfo;
    private GButton close;

    public override void Init()
    {
        base.Init();
		this.Create(Config.VIEW_ROLELOVE,false,Tools.GetMessageById("13087"));
        InitViewOne();
        InitData();
    }
    private void InitItems()
    {
        m_ItemsCount = total;
        if (m_ItemsCount > 0)
        {
            g.visible = true;
            m_PageCount = (m_ItemsCount % number) == 0 ? m_ItemsCount / number : (m_ItemsCount / number) + 1;
            m_PanelText.text =Tools.SetFontColor(m_PageIndex.ToString(), m_PageCount.ToString(),null);
        }
        else
        {
            g.visible = false;
        }
        BindPage(m_PageIndex);

    }
    private void InitData()
    {
        
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        otherInfo = roleModel.otherInfo;
        if (roleModel.roleLove != null)
        {
            roleLove = (object[])(roleModel.roleLove["like_res"]);
            total = (int)roleModel.roleLove["total"];
            if (roleLove.Length == 0)
            {
                m_PageIndex = 0;
            }
        }
        InitItems();

    }

    private void InitViewOne()
    {
//        InitTitle(Tools.GetMessageById("13087"));
		photoList = this.GetChild("n3").asList;
		g= this.GetChild("n2").asCom;
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
        roleLove = null;
		Dictionary<string, object> dc = new Dictionary<string, object>();
        dc["fuid"] = otherInfo["uid"];
        dc["page_num"] = m_PageIndex;
        NetHttp.inst.Send(NetBase.HTTP_GETFRIENDLISK, dc, (VoHttp vo) =>
        {
            Dictionary<string, object> temp = (Dictionary<string, object>)(vo.data);
            roleLove = (object[])temp["like_res"];
            BindPage(m_PageIndex);
            m_PanelText.text = Tools.SetFontColor(m_PageIndex.ToString(), m_PageCount.ToString(), null);
        });
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

        roleLove = null;
		Dictionary<string, object> dc = new Dictionary<string, object>();
        dc["fuid"] = otherInfo["uid"];
        dc["page_num"] = m_PageIndex;
        NetHttp.inst.Send(NetBase.HTTP_GETFRIENDLISK, dc, (VoHttp vo) =>
        {
            Dictionary<string, object> temp = (Dictionary<string, object>)(vo.data);
            roleLove = (object[])temp["like_res"];
            BindPage(m_PageIndex);
            m_PanelText.text = Tools.SetFontColor(m_PageIndex.ToString(), m_PageCount.ToString(), null);
        });
    }

    private void BindPage(int m_PageIndex)
    {
        photoList.emptyStr = Tools.GetMessageById("13068");
        photoList.itemRenderer = ListRendere;
        photoList.numItems = roleLove.Length;

    }

    private void ListRendere(int index, GObject item)
    {
        GButton head = item.asCom.GetChild("n3").asButton;
        GTextField num=item.asCom.GetChild("n2").asTextField;
        GImage bg = item.asCom.GetChild("n4").asImage;
        GTextField name = item.asCom.GetChild("n15").asTextField;
        Dictionary<string,object> d=(Dictionary<string, object>)roleLove[index];
        bg.visible = false;
        if (Tools.IsNullEmpty(d["uname"]))
        {
            name.text = d["uid"].ToString();
        }
        else
        {
            name.text = d["uname"].ToString();
        }
        if (d["uid"].ToString().Equals(userModel.uid))
        {
            bg.visible = true;
        }
        Tools.SetLoaderButtonUrl(head,ModelUser.GetHeadUrl((string)d["head_use"]));
        num.text =d["like_num"] +"";
        head.RemoveEventListeners();
        head.onClick.Add(()=> {
            Dictionary<object, object> dc_ = new Dictionary<object, object>();
            dc_["fuid"] = d["uid"];
            string uid = ModelManager.inst.userModel.uid;
            string fuid = dc_["fuid"] + "";
            if (!fuid.Equals(uid))
            {
                this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { otherInfo["uid"].ToString(), dc_["fuid"].ToString(), roleModel.tab_Role_CurSelect1, roleModel.tab_Role_CurSelect2, roleModel.tab_Role_CurSelect3 }));
            }
            else
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13106"));
            }
        });

    }
    public override void Clear()
    {
        base.Clear();
        if (m_BtnNext != null)
        {
            m_BtnNext.onClick.Remove(Next);
        }
        if (m_BtnPrevious != null)
        {
            m_BtnPrevious.onClick.Remove(Previous);
        }
    }
}
