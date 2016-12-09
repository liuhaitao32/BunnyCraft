using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using FairyGUI;

public class MediatorBlackList : BaseMediator
{
    private List<object> listData;
    private ModelRole roleModel;
    private int m_PageIndex;
    private int total;
    private GTextField num;
    private GButton btn_n;
    private GButton btn_p;
    private GList list;
    private int m_PageCount;
    private int m_ItemsCount;
    private int number;
    private GObject icon;
    private GLoader head;
    private ModelUser userModel;
    private int effort_Num;
    private GComponent g;
    private int m_PageIndexS;
    private int count;

    public override void Init()
    {
        base.Init();
        m_PageIndexS = 1;
        count = 5;
        m_PageIndex = 1;
        number = 6;
        this.Create(Config.VIEW_ATTENTION);
        FindObject();
        InitData();
    }

    private void FindObject()
    {
		g = this.GetChild("n2").asCom;
        g.visible = false;
        num = g.GetChild("n0").asTextField;
        btn_n = g.GetChild("n5").asButton;
        btn_p = g.GetChild("n4").asButton;
		list = this.GetChild("n1").asList;
        list.emptyStr = Tools.GetMessageById("19929");//lht change
        list.onChangeNum.Add (this.CheckListNum);
        list.itemRenderer = OnRendere;

        btn_n.onClick.Add(() =>
        {
            Next();
        });
        btn_p.onClick.Add(() =>
        {
            Previous();
        });
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
        SendNetMesage("pre");
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
        SendNetMesage("next");
    }

    private void InitData()
    {
        listData = new List<object>();
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        effort_Num = ((Dictionary<string, object>)DataManager.inst.effort["effort_cond"]).Count;
        if (roleModel.isSave)
        {
            roleModel.isSave = false;
            if (roleModel.tempData != null)
            {
                m_PageIndexS = ((int[])roleModel.tempData["tag"])[0];
                m_PageIndex = ((int[])roleModel.tempData["tag"])[1];
                SendNetMesage("current");
            }
        }
        else
        {
            SendNetMesage("");
        }

    }

    private void SendNetMesage(string type)
    {
        if (type.Equals("next"))
            m_PageIndexS += 1;
        else if (type.Equals("pre"))
            m_PageIndexS -= 1;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["page_num"] = m_PageIndexS;
        NetHttp.inst.Send(NetBase.HTTP_GETBALCKLIST, data, (VoHttp vo) =>
        {
            Dictionary<string, object> dc = (Dictionary<string, object>)vo.data;
            object[] obj = (object[])dc["black_res"];
            m_ItemsCount = (int)dc["total"];
            if (obj.Length != 0)
            {
                listData.Clear();
                total = (int)dc["total"];
                foreach (object v in obj)
                {
                    listData.Add(v);
                }

                InitItem(listData, m_PageIndex);
            }
            else
            {
                g.visible = false;
                listData.Clear();
                //list.numItems = listData.Count;
                SetListCSS(list, listData.ToArray(),6,false);
            }
            
        });
    }

    private void InitItem(List<object> listData, int index)
    {
        g.visible = true;
        m_PageCount = (m_ItemsCount % number) == 0 ? m_ItemsCount / number : (m_ItemsCount / number) + 1;
        this.listData = listData;
        list.numItems = listData.Count;
        SetListCSS(list, listData.ToArray(),6, false);
        num.text = string.Format("{0}/{1}", m_PageIndex.ToString(), m_PageCount.ToString());
    }

    private void OnRendere(int index, GObject item)
    {

        GComponent g = item.asCom;
        GObject reletion = g.GetChild("n3");
        GButton headBtn=g.GetChild("n2").asButton;
        GButton head = g.GetChild("n2").asCom.GetChild("n0").asButton;
        GTextField name = g.GetChild("n4").asTextField;
        GTextField guild = g.GetChild("n5").asTextField;
        GLoader achieve = g.GetChild("n29").asCom.GetChild("n1").asLoader;
        GTextField rankScore = g.GetChild("n29").asCom.GetChild("n2").asTextField;
        GTextField msg = g.GetChild("n6").asTextField;
        GTextField status = g.GetChild("n7").asTextField;
        GButton mask_btn = g.GetChild("n9").asButton;
        GButton btn_ = g.GetChild("n0").asButton;
        userModel.GetUnlcok(Config.UNLOCK_GUILD, guild);

        bool isVisible = SetListCSS(item, listData.ToArray(), index);
        if (isVisible)
        {
            btn_.visible = true;
            //mask_btn.SetSize(622, 71);
            btn_.text = Tools.GetMessageById("19911");
            reletion.visible = false;
            msg.visible = false;
            status.visible = false;
            GTextField lv = g.GetChild("n2").asCom.GetChild("n2").asTextField;
            Dictionary<string, object> dc = (Dictionary<string, object>)listData[index];

            btn_.GetChild("n1").asLoader.url = Tools.GetResourceUrl("Image2:n_btn_4");
            Tools.SetRootTabTitleStrokeColor(btn_.GetChild("title").asTextField, "#dd8680", 2);
            btn_.RemoveEventListeners();
            btn_.onClick.Add(() =>
            {
                Dictionary<string, object> dd = new Dictionary<string, object>();
                dd["fuid"] = dc["uid"];
                NetHttp.inst.Send(NetBase.HTTP_CANCLESHIELDING, dd, (VoHttp vo) =>
                {
                    if ((bool)vo.data == true)
                    {

                        SendNetMesage("");
                        ViewManager.inst.ShowText(Tools.GetMessageById("19913"));
                    }
                });
            });
            achieve.url = userModel.GetRankImg((int)dc["rank_score"]);
            mask_btn.RemoveEventListeners();
            mask_btn.onClick.Add(() => {
                roleModel.SetTempData(null, new int[] { m_PageIndexS, m_PageIndex });
                string uid = dc["uid"] + "";
                if (!uid.Equals(userModel.uid))
                {
                    this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { null, uid, roleModel.tab_CurSelect1, roleModel.tab_CurSelect2, roleModel.tab_CurSelect3 })); 
                }
                else
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13106"));
                }
            });
            lv.text = dc["lv"].ToString();
            string uname = dc["uname"] + "";
            if (uname.Equals(""))
            {
                name.text = dc["uid"] + "";
            }
            else
            {
                name.text = uname;
            }
            string dc_ = (string)dc["head_use"];
            Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(dc_));
            if (dc["guild_name"] == null)
            {
                guild.text = Tools.GetMessageById("19955");
            }
            else
            {
                guild.text = dc["guild_name"] + "";
            }
            //msg.text = "23小时前成就等级提升至99级";
            rankScore.text = dc["rank_score"] + "";
        }
        else
        {
            btn_.visible = false;
        }
    }
}

