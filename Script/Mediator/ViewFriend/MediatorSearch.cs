using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using System;

public class MediatorSearch : BaseMediator
{
    private List<object> listDataHttp;
    private List<object> listUid;
    private List<object> listDataSocket;
    private List<object> listStatusTemp;
    private ModelRole roleModel;
    private ModelUser userModel;
    private GTextInput input;
    private GButton btn;
    private GList list;
    private int type;
    private int effort_Num;
    private List<object> listData;
    private string btnBgUp;
    private string btnBgDo;
    private string textAttention;
    private string textAttentiond;
    private string textShield;
    private string textShieldd;
    private string textBtnStrokeColor;
    private string textBtnColor;
    private string fontSize;
    private GTextField showMyUID;

    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_SEARCH);
        FindObject();
        InitData();
    }

    public override void Clear()
    {
        base.Clear();

        NetSocket.inst.RemoveListener(NetBase.SOCKET_STATUS);
    }

    private void SearchRender(int index, GObject item)
    {
        Dictionary<string, object> dc = (Dictionary<string, object>)listData[index];
        GButton photo = item.asCom.GetChild("n1").asCom.GetChild("n0").asButton;
        GTextField name = item.asCom.GetChild("n3").asTextField;
        GTextField guild = item.asCom.GetChild("n4").asTextField;
        GLoader achieve = item.asCom.GetChild("n21").asCom.GetChild("n1").asLoader;
        GTextField status = item.asCom.GetChild("n9").asTextField;
        GButton mask_btn = item.asCom.GetChild("n8").asButton;
        GButton btn_attention = item.asCom.GetChild("n7").asButton;
        GTextField attention_text = btn_attention.GetChild("title").asTextField;
        GLoader btn_image = btn_attention.asCom.GetChild("n1").asLoader;
        //btn_image.visible = true;
        GObject reletion = item.asCom.GetChild("n2");
        GTextField rankScore = item.asCom.GetChild("n21").asCom.GetChild("n2").asTextField;
        GButton more = item.asCom.GetChild("n10").asButton;
        GTextField lv = item.asCom.GetChild("n1").asCom.GetChild("n2").asTextField;
        GLoader bg = item.asCom.GetChild("n32").asLoader;
        bg.visible = false;
        more.visible = false;
        rankScore.visible = true;
        reletion.visible = false;
        userModel.GetUnlcok(Config.UNLOCK_GUILD, guild);

        btn_attention.RemoveEventListeners();
        btn_attention.onClick.Add(() => {
            string uid_ = dc["uid"] + "";
            if (!uid_.Equals(userModel.uid))
            {
                Dictionary<string, object> dd = new Dictionary<string, object>();
                dd["fuid"] = dc["uid"];
                NetHttp.inst.Send(NetBase.HTTP_FOLLOW, dd, (VoHttp vo) =>
                {

                    if ((bool)vo.data == true)
                    {
                        roleModel.AddAttentionFight(dc["uid"].ToString());
                        ((Dictionary<string, object>)listData[index])["if_friend"] = true;
                        Tools.SetButtonBgAndColor(btn_attention, btnBgDo, textAttentiond, fontSize, textBtnColor, "", 0);
                        btn_attention.touchable = false;
                        //                        btn_image.url = Tools.GetResourceUrl("Image:btn_page3");
                        btn_attention.RemoveEventListeners();
                    }
                    else
                    {
                        ViewManager.inst.ShowText(Tools.GetMessageById("13120"));
                    }
                });
            }
            else
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13106"));
            }
        });
        if ((bool)dc["if_black"])
        {//是否拉黑
            Tools.SetButtonBgAndColor(btn_attention, btnBgDo, textShieldd, fontSize, textBtnColor, "", 0);
            type = 1;
            btn_attention.touchable = false;
            //            btn_image.url = Tools.GetResourceUrl("Image:btn_page3");

        }
        else
        if ((bool)dc["if_follow"])
        {
            //是否已关注
            Tools.SetButtonBgAndColor(btn_attention, btnBgDo, textAttentiond, fontSize, textBtnColor, "", 0);
            type = 2;
            btn_attention.touchable = false;
            //            btn_image.url = Tools.GetResourceUrl("Image:btn_page3");
        }
        else
        {
            Tools.SetButtonBgAndColor(btn_attention, btnBgUp, textAttention, fontSize, textBtnColor, textBtnStrokeColor);
            btn_attention.touchable = true;
            //            btn_image.url = Tools.GetResourceUrl("Image:btn_9");
        }

        achieve.url = userModel.GetRankImg((int)dc["rank_score"]);
        mask_btn.RemoveEventListeners();
        mask_btn.onClick.Add(() => {
            roleModel.SetTempData(listData, new int[] { index });
            string uid_ = dc["uid"] + "";
            if (!uid_.Equals(userModel.uid))
            {
                this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { null, uid_, roleModel.tab_CurSelect1, roleModel.tab_CurSelect2, roleModel.tab_CurSelect3 }));
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
        Tools.SetLoaderButtonUrl(photo, ModelUser.GetHeadUrl(dc_));
        if (dc["guild_name"] == null)
        {
            guild.text = Tools.GetMessageById("19955");
        }
        else
        {
            guild.text = dc["guild_name"] + "";
        }
        if (dc.ContainsKey("status"))
        {
            switch ((int)dc["status"])
            {
                case 0://离线
                    //					bg.url = Tools.GetResourceUrl("Image2:n_bg_zt3");
                    status.text = Tools.GetMessageById("19957");
                    Tools.SetRootTabTitleStrokeColor(status, "#646EA9", 1);
                    break;
                case 1://在线
                    //					bg.url = Tools.GetResourceUrl("Image2:n_bg_zt1");
                    status.text = Tools.GetMessageById("19958");
                    Tools.SetRootTabTitleStrokeColor(status, "#63A969", 1);
                    break;
                case 2://ing
                    //					bg.url = Tools.GetResourceUrl("Image2:n_bg_zt2");
                    Tools.SetRootTabTitleStrokeColor(status, "#A55A76", 1);
                    status.text = Tools.GetMessageById("19959");
                    break;
            }
        }
        rankScore.text = dc["rank_score"] + "";
    }



    private void InitData()
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
        listDataHttp = new List<object>();
        listUid = new List<object>();
        listDataSocket = new List<object>();
        listStatusTemp = new List<object>();
        listData = new List<object>();
        roleModel = ModelManager.inst.roleModel;
       // userModel = ModelManager.inst.userModel;
        effort_Num = ((Dictionary<string, object>)DataManager.inst.effort["effort_cond"]).Count;
        if (roleModel.isSave)
        {
            roleModel.isSave = false;
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
            list.numItems = listData.Count;
            list.ScrollToView(((int[])roleModel.tempData["tag"])[0]);
            roleModel.tempData = null;
        }
        else
        {
            list.numItems = listData.Count;
        }
    }

    private void FindObject()
    {
        userModel = ModelManager.inst.userModel;
        showMyUID = this.GetChild("n120").asTextField;
        showMyUID.text = Tools.GetMessageById("19972",new string[] { userModel.uid });
        
        input = this.GetChild("n2").asCom.GetChild("n1").asTextInput;
        input.promptText = Tools.GetMessageById("19953");
        btn = this.GetChild("n3").asButton;
        btn.text = Tools.GetMessageById("19945");
        btn.RemoveEventListeners();
        btn.onClick.Add(FindFriends);
        list = this.GetChild("n1").asList;
        list.emptyStr = Tools.GetMessageById("19933");
        list.itemRenderer = SearchRender;
        list.SetVirtual();
        
        
    }

    private void FindFriends()
    {
        Debug.Log(list.numItems);
        listDataHttp.Clear();
        listDataSocket.Clear();
        listStatusTemp.Clear();
        listUid.Clear();
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["uid"] = input.text;
        if (!input.text.Equals(""))
        {
            
            if (input.text.Equals(userModel.uid) || input.text.Equals(userModel.uname))
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("19922"));
            }
            else
            {
                try
                {
                   Convert.ToInt32(input.text);
                    NetHttp.inst.Send(NetBase.HTTP_GETFRIENS, data, (VoHttp vo) =>
                    {

                        input.text = "";
                        Dictionary<string, object> dc = (Dictionary<string, object>)vo.data;
                        if (dc != null)
                        {
                            //foreach (object v in dc)
                            //{
                            //    listDataHttp.Add(v);
                            //}
                            listDataHttp.Add(dc);
                            listUid.Clear();
                            foreach (object v in listDataHttp)
                            {
                                Dictionary<string, object> dd = (Dictionary<string, object>)v;
                                listUid.Add(dd["uid"]);
                            }
                            listData = listDataHttp;
                            list.numItems = 0;
                            list.numItems = listData.Count;
                            //                        list.numItems = listData.Count;
                            if (listDataHttp.Count != 0)
                            {
                                
                                //长联的数据
                                Dictionary<string, object> s_data = new Dictionary<string, object>();
                                s_data["uids"] = listUid;
                                NetSocket.inst.Send(NetBase.SOCKET_STATUS, s_data);
                                NetSocket.inst.AddListener(NetBase.SOCKET_STATUS, (VoSocket vo1) => {
                                    Dictionary<string, object> uids = (Dictionary<string, object>)vo1.data;
                                    //                                listDataSocket.Clear();
                                    foreach (object vData in listDataHttp)
                                    {
                                        Dictionary<string, object> v_Data = (Dictionary<string, object>)vData;
                                        foreach (string vUid in uids.Keys)
                                        {
                                            if (v_Data["uid"].ToString().Equals(vUid))
                                            {
                                                v_Data["status"] = uids[vUid];
                                                //                                            listDataSocket.Add(v_Data);
                                            }
                                        }
                                    }
                                    //                                listData = listDataSocket;
                                    list.numItems = 0;
                                    list.numItems = listData.Count;
                                   
                                });
                            }

                        }
                        else
                        {
                            list.numItems = 0;   
                            ViewManager.inst.ShowText(Tools.GetMessageById("19921"));
                        }
                    });
                }
                catch
                {
                    list.numItems = 0;
                    ViewManager.inst.ShowText(Tools.GetMessageById("13148"));
                }
               
            }
        }
        else
        {
            list.numItems = 0;
            ViewManager.inst.ShowText(Tools.GetMessageById("13062"));
        }


    }
    public  bool IsIntNum(string str)
    {
        System.Text.RegularExpressions.Regex reg1
        = new System.Text.RegularExpressions.
            Regex(@"^[-]?[1-9]{1}\d*$|^[0]{1}$");
        bool ismatch = reg1.IsMatch(str);
        return ismatch;
    }
}
