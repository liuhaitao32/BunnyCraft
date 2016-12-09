using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using FairyGUI;

public class MediatorAttention : BaseMediator {
    private List<object> listDataHttp;
    private List<object> listUid;
    private List<object> listDataSocket;
    private List<object> listStatusTemp;
    private ModelRole roleModel;
    private int m_PageIndex ;
    private GTextField num;
    private GButton btn_n;
    private GButton btn_p;
    private GList list;
    private int m_PageCount;
    private int m_PageIndexS;//获取网络数据的index
    private int m_ItemsCount;
    private int number;
    private GObject icon;
    private GLoader head;
    private ModelUser userModel;
    private int effort_Num;
    private GComponent g;
    private List<int> listIndex;
    private int index;
    private int count;
    private List<object> listData;

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
		AddGlobalListener(MainEvent.ATTENTION_UPDATE, ViewUpdata);
    }
    private void ViewUpdata(MainEvent e)
    {
        int tag=(int)e.data;
        if (tag==1) {
            if(listDataHttp!=null)
               listDataHttp.Clear();
            if(listDataSocket!=null)
               listDataSocket.Clear();
            if (listStatusTemp != null)
                listStatusTemp.Clear();
            if (listUid != null)
                listUid.Clear();
            m_PageIndex = 1;
            m_PageCount = 1;
            m_PageIndexS = 1;
            SendNetMesage("");
        }

    }
    public override void Clear()
    {
        base.Clear();
        RemoveGlobalListener(MainEvent.ATTENTION_UPDATE, ViewUpdata);
        NetSocket.inst.RemoveListener(NetBase.SOCKET_STATUS);
    }
    private void FindObject()
    {
		g =this.GetChild("n2").asCom;
        g.visible = false;
        num = g.GetChild("n0").asTextField;
        btn_n = g.GetChild("n5").asButton; 
        btn_p = g.GetChild("n4").asButton;
		list = this.GetChild("n1").asList;
        list.emptyStr = Tools.GetMessageById("19928");
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
        listDataHttp = new List<object>();
        listUid = new List<object>();
        listDataSocket = new List<object>();
        listData = new List<object>();
        listStatusTemp = new List<object>();
        listIndex = new List<int>();
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        effort_Num=((Dictionary<string,object>)DataManager.inst.effort["effort_cond"]).Count;
        
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
        NetHttp.inst.Send(NetBase.HTTP_GETATTENTION, data, (VoHttp vo) =>
        {
            Dictionary<string, object> dc = (Dictionary<string, object>)vo.data;
            if (dc.ContainsKey("notice_follow"))
            {
                object tempNotice = (object[])dc["notice_follow"];
                userModel.notice["follow"] = tempNotice;
                DispatchGlobalEvent(new MainEvent(MainEvent.RED_UPDATE, tempNotice));
            }
            else
            {
                userModel.notice["follow"] = new object[] { };
                DispatchGlobalEvent(new MainEvent(MainEvent.RED_UPDATE, new object[] { }));
            }
            object[] obj = (object[])dc["follow_res"];
            m_ItemsCount = (int)dc["total"];
            if (obj.Length != 0)
            {
                listDataHttp.Clear();
                foreach (object v in obj)
                {
                    listDataHttp.Add(v);
                }

                listUid.Clear();
                foreach (object v in listDataHttp)
                {
                    Dictionary<string, object> dd = (Dictionary<string, object>)v;
                    listUid.Add(dd["uid"]);
                }
                InitItem(listDataHttp, index, type);
                //长联的数据
                if (listDataHttp.Count != 0)
                {
                    listDataSocket.Clear();
					Dictionary<string, object> s_data = new Dictionary<string, object>();
                    s_data["uids"] = listUid;
                    NetSocket.inst.Send(NetBase.SOCKET_STATUS, s_data);
                    NetSocket.inst.AddListener(NetBase.SOCKET_STATUS, (VoSocket vo1) => {
                        NetSocket.inst.RemoveListener(NetBase.SOCKET_STATUS);
                        Dictionary<string, object> uids = (Dictionary<string, object>)vo1.data;
                        foreach (object vData in listDataHttp)
                        {
                            Dictionary<string, object> v_Data = (Dictionary<string, object>)vData;
                            foreach (string vUid in uids.Keys)
                            {
                                if (v_Data["uid"].ToString().Equals(vUid))
                                {
                                    v_Data["status"] = uids[vUid];
                                    listDataSocket.Add(v_Data);
                                }
                            }
                        }
                        InitItem(listDataSocket, index, type);
                    });
                }
            }
            else
            {
                //list.numItems = listData.Count;
                SetListCSS(list, listData.ToArray(), 6,false);
            }

        });
       
        
    }

    private void InitItem(List<object> listData, int index,string type)
    {
        g.visible = true;
        m_PageCount = (m_ItemsCount % number) == 0 ? m_ItemsCount / number : (m_ItemsCount / number) + 1;
        this.listData = listData;
        //list.numItems = listData.Count;
        SetListCSS(list, listData.ToArray(), 6, false);
        num.text = string.Format("{0}/{1}", m_PageIndex.ToString(), m_PageCount.ToString());
    }
    
    private void OnRendere(int index, GObject item)
    {

        GComponent g=item.asCom;
        GObject reletion = g.GetChild("n3");
        GButton head=g.GetChild("n2").asCom.GetChild("n0").asButton;
        GTextField name=g.GetChild("n4").asTextField;
        GTextField guild=g.GetChild("n5").asTextField;
        GLoader achieve=g.GetChild("n29").asCom.GetChild("n1").asLoader;
        GTextField rankScore = g.GetChild("n29").asCom.GetChild("n2").asTextField;
        GTextField msg=g.GetChild("n6").asTextField;
        GTextField status=g.GetChild("n7").asTextField;
        GButton mask_btn = g.GetChild("n9").asButton;
        //GGroup mask_btn = g.GetChild("item").asGroup;
        GButton btn_ = g.GetChild("n0").asButton;
        GLoader bg = g.GetChild("n32").asLoader;
        userModel.GetUnlcok(Config.UNLOCK_GUILD, guild);

        bool isVisible = SetListCSS(item, listData.ToArray(), index);
        if (isVisible)
        {
            bg.visible = false;
            btn_.visible = false;
            GTextField lv = g.GetChild("n2").asCom.GetChild("n2").asTextField;
            Dictionary<string, object> dc = (Dictionary<string, object>)listData[index];
            if ((bool)dc["is_new"])
            {
                userModel.Add_Notice(g, new UnityEngine.Vector2(72, -1), 0, false);
            }
            else
            {
                userModel.Remove_Notice(g);
            }

            int fans_count = userModel.Get_NoticeState(ModelUser.RED_FOLLOW);
            achieve.url = userModel.GetRankImg((int)dc["rank_score"]);
            rankScore.text = dc["rank_score"] + "";
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
            if ((bool)dc["if_friend"])
            {
                reletion.visible = true;
            }
            else
            {
                reletion.visible = false;
            }
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
            Dictionary<string, object> dynamic = (Dictionary<string, object>)dc["dynamic"];
            if (dynamic.Count != 0)
            {
                Tools.DataTimeFormat(msg, (DateTime)dynamic["time"], 0);
                switch (dynamic["type"].ToString())
                {
                    case "lv_up":
                        msg.text = Tools.GetMessageById("19937", new object[] { msg.text, dynamic["lv"].ToString() });
                        break;
                    case "elv_up":
                        msg.text = Tools.GetMessageById("19938", new object[] { msg.text, Tools.GetEffortName((int)dynamic["elv"]) });
                        break;
                    case "match_team":
                        if ((bool)dynamic["if_win"])
                        {
                            if ((bool)dynamic["if_mvp"])
                            {
                                msg.text = Tools.GetMessageById("19939", new object[] { msg.text });

                            }
                            else
                            {
                                msg.text = Tools.GetMessageById("19940", new object[] { msg.text });

                            }

                        }
                        else
                        {
                            if ((bool)dynamic["if_mvp"])
                            {
                                msg.text = Tools.GetMessageById("19941", new object[] { msg.text });

                            }
                            else
                            {
                                msg.text = Tools.GetMessageById("19942", new object[] { msg.text });

                            }
                        }

                        break;
                    case "match":
                        msg.text = Tools.GetMessageById("19943", new object[] { msg.text, dynamic["sort"].ToString(), dynamic["kill_num"].ToString() });
                        break;
                    case "up_head":
                        msg.text = Tools.GetMessageById("19970", new object[] { msg.text });
                        break;
                    case "choose_head":
                        msg.text = Tools.GetMessageById("19971", new object[] { msg.text });
                        break;

                }
            }
            else
            {
                msg.text = "";
            }

            if (dc.ContainsKey("status"))
            {
                switch ((int)dc["status"])
                {
                    case 0:
                        //					bg.url = Tools.GetResourceUrl("Image2:n_bg_zt3");
                        status.text = Tools.GetMessageById("19957");
                        Tools.SetRootTabTitleStrokeColor(status, "#646EA9", 1);
                        break;
                    case 1:
                        //					bg.url = Tools.GetResourceUrl("Image2:n_bg_zt1");
                        status.text = Tools.GetMessageById("19958");
                        Tools.SetRootTabTitleStrokeColor(status, "#63A969", 1);
                        break;
                    case 2:
                        //					bg.url = Tools.GetResourceUrl("Image2:n_bg_zt2");
                        Tools.SetRootTabTitleStrokeColor(status, "#A55A76", 1);
                        status.text = Tools.GetMessageById("19959");
                        break;
                }
            }
        }

        
    }
}

