using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;

public class MediatorNear : BaseMediator
{
    private GList list;
    private ModelRole roleModel;
    private List<object> listData;
    private bool isMore = true;
    private int tag_n = 1;
    private int num = 0;
    private double lon;
    private double lat;
    private int type = 0;
    private ModelUser userModel;
    private int dataPreNum;
    private string btnBgUp;
    private string btnBgDo;
    private string textAttention;
    private string textAttentiond;
    private string textShield;
    private string textShieldd;
    private string textBtnStrokeColor;
    private string textBtnColor;
    private string fontSize;

    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_NEAR);
        list = this.GetChild("n0").asList;
        list.emptyStr = Tools.GetMessageById("19932");
        list.onChangeNum.Add(this.CheckListNum);
        list.itemRenderer = SearchRander;
        list.SetVirtual();
        InitData();
    }

    public override void Clear()
    {
        base.Clear();

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
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        num = (int)DataManager.inst.systemSimple["name_near"];
        listData = new List<object>();
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
            //list.numItems = listData.Count;
            SetListCSS(list, listData.ToArray(), 6, true);
			jump1 =	TimerManager.inst.Add (0.2f, 1, (float f) => {
				int[] tag = (int[])roleModel.tempData ["tag"];
				list.ScrollToView (tag [0]);
				tag_n = tag [1];
				Debug.Log ("tag_n_CALL" + tag_n);
				dataPreNum = tag [2];
				roleModel.tempData = null;
				TimerManager.inst.Remove(jump1);
			});
        }
        else
        {
            if (!PhoneManager.inst.IsOpenGps)
            {
                //ViewManager.inst.ShowText(Tools.GetMessageById("13038"));
                ViewManager.inst.ShowAlert(Tools.GetMessageById("13038"), (int v) =>
                {
                    if (v == 0)
                    {
                        list.numItems = 0;
                    }
                    if (v == 1)
                    {
                        PhoneManager.inst.GetGps(GetGps);
                    }
                },true,false);
            }
            else
            {
                PhoneManager.inst.GetGps(GetGps);

            }
        }

    }
    private void GetGps(string la, string lo)
    {
        roleModel.longitude = lo;
        roleModel.latitude = la;
        GetNearData(2, null);
    }
    private void GetNearData(int type, GObject more)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["lon"] = lon;
        data["lat"] = lat;
        data["page_num"] = tag_n;
        // Debug.Log("tag_n_" + tag_n);
        NetHttp.inst.Send(NetBase.HTTP_GETNEAR, data, (VoHttp vo) =>
        {
            Dictionary<string, object> temp = new Dictionary<string, object>();
            temp["uid"] = -1;
            temp["type"] = 0;
            object[] data1 = (object[])vo.data;
            if (data1.Length > 0)
            {
                if (type == 2)
                {
                    foreach (object d in data1)
                    {
                        listData.Add(d);
                    }
                    if (data1.Length >= num)
                    {
                        listData.Add(temp);
                    }
                    //list.numItems = listData.Count;
                    SetListCSS(list, listData.ToArray(), 6, true);
                    list.ScrollToView(0);
                }
                else
                {
                    listData.RemoveAt(listData.Count - 1);
                    foreach (object d in data1)
                    {
                        listData.Add(d);
                    }
                    if (data1.Length >= num)
                    {
                        listData.Add(temp);
                    }
                    Debug.Log("listData.Count" + listData.Count);
                    Debug.Log("dataPreNum" + dataPreNum);
                    //list.numItems = listData.Count;
                    SetListCSS(list, listData.ToArray(), 6, true);
                    list.ScrollToView(dataPreNum - 1);
                }
                dataPreNum = listData.Count;
            }
            else
            {
                if (tag_n == 1)
                {
                    list.numItems = 0;
                }
                else
                {
                    listData.RemoveAt(listData.Count - 1);
                    //list.numItems = listData.Count;
                    SetListCSS(list, listData.ToArray(), 6, true);
                    list.ScrollToView(listData.Count - 1);
                    ViewManager.inst.ShowText(Tools.GetMessageById("19924"));
                }

            }
            tag_n += 1;
        });
    }
    private void SearchRander(int index, GObject item)
    {

        GComponent comPhoto = item.asCom.GetChild("n1").asCom;
        GButton photo = comPhoto.GetChild("n0").asButton;
        GTextField name = item.asCom.GetChild("n3").asTextField;
        GTextField guild = item.asCom.GetChild("n4").asTextField;
        GObject rank = item.asCom.GetChild("n21");
        GTextField rankScore = rank.asCom.GetChild("n2").asTextField;
        GLoader achieve = rank.asCom.GetChild("n1").asLoader;
        GTextField status = item.asCom.GetChild("n9").asTextField;
        GButton mask_btn = item.asCom.GetChild("n8").asButton;
        GButton btn_attention = item.asCom.GetChild("n7").asButton;
        GTextField attention_text = btn_attention.GetChild("title").asTextField;
        GLoader btn_image = btn_attention.asCom.GetChild("n1").asLoader;
        GObject reletion = item.asCom.GetChild("n2");
        GButton more = item.asCom.GetChild("n10").asButton;
        GTextField lv = item.asCom.GetChild("n1").asCom.GetChild("n2").asTextField;
        GGroup itemBtn = item.asCom.GetChild("item").asGroup;
        //GImage bg = item.asCom.GetChild("n0").asImage;
        more.text = Tools.GetMessageById("13078");


        bool isVisible = SetListCSS(item, listData.ToArray(), index);
        if (isVisible)
        {
            Dictionary<string, object> dc = (Dictionary<string, object>)listData[index];

            more.visible = false;
            rankScore.visible = true;
            reletion.visible = false;
            rank.visible = true;
            int uid = (int)dc["uid"];
            if (uid == -1)
            {
                //            bg.visible = false;
                lv.visible = false;
                comPhoto.visible = false;
                name.visible = false;
                guild.visible = false;
                achieve.visible = false;
                status.visible = false;
                //mask_btn.visible = false;
                more.visible = true;
                btn_attention.visible = false;
                rank.visible = false;
                more.onClick.Add(() =>
                {
                    if (Convert.ToInt32(dc["type"]) == 1)
                    {
                        ViewManager.inst.ShowText(Tools.GetMessageById("13043"));
                    }
                    else
                    {
                        GetNearData(1, item);
                    }
                });
            }
            else
            {
                lv.visible = true;
                comPhoto.visible = true;
                name.visible = true;
                guild.visible = true;
                userModel.GetUnlcok(Config.UNLOCK_GUILD, guild);
                achieve.visible = true;
                status.visible = true;
                //mask_btn.visible = true;
                btn_attention.visible = true;
                more.visible = false;
                btn_attention.RemoveEventListeners();
                btn_attention.onClick.Add(() =>
                {
                    //关注的实现
                    Dictionary<string, object> dd = new Dictionary<string, object>();
                    dd["fuid"] = dc["uid"];
                    NetHttp.inst.Send(NetBase.HTTP_FOLLOW, dd, (VoHttp vo) =>
                    {

                        if ((bool)vo.data == true)
                        {
                            roleModel.AddAttentionFight(dc["uid"].ToString());
                            //ViewManager.inst.ShowText(Tools.GetMessageById("13045"));
                            ((Dictionary<string, object>)listData[index])["if_follow"] = true;
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
                });
                if ((bool)dc["if_black"])
                {//是否拉黑
                    Tools.SetButtonBgAndColor(btn_attention, btnBgDo, textShieldd, fontSize, textBtnColor, "", 0);

                    type = 1;
                    btn_attention.touchable = false;


                }
                else if ((bool)dc["if_follow"])
                {
                    Tools.SetButtonBgAndColor(btn_attention, btnBgDo, textAttentiond, fontSize, textBtnColor, "", 0);

                    type = 2;
                    btn_attention.touchable = false;
                    //                btn_image.url = Tools.GetResourceUrl("Image:btn_page3");
                }
                else
                {
                    Tools.SetButtonBgAndColor(btn_attention, btnBgUp, textAttention, fontSize, textBtnColor, textBtnStrokeColor);

                    btn_attention.touchable = true;
                }
                achieve.url = userModel.GetRankImg((int)dc["rank_score"]);
                status.text = Tools.GetNearDistance((int)dc["dis"]);
                mask_btn.RemoveEventListeners();
                mask_btn.onClick.Add(() =>
                {
                    if (!more.visible)
                    {
                        roleModel.SetTempData(listData, new int[] { list.GetFirstChildInView(), tag_n, dataPreNum });
                        string uid_ = dc["uid"] + "";
                        if (!uid_.Equals(userModel.uid))
                        {
                            this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { null, uid_, roleModel.tab_CurSelect1, roleModel.tab_CurSelect2, roleModel.tab_CurSelect3 }));
                        }
                        else
                        {
                            ViewManager.inst.ShowText(Tools.GetMessageById("13106"));
                        }
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
                rankScore.text = dc["rank_score"] + "";
            }
        }
        else
        {
            btn_attention.visible = false;
        }


    }
}
