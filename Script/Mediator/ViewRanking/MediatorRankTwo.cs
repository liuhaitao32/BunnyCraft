using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorRankTwo : BaseMediator
{
    private GList list;
    private Dictionary<string, object> data;
    private ModelRole roleModel;
    private List<object> listData;
    private ModelUser userModel;
    private GButton search_user;
    private GButton top;
    private ModelRank rankModel;
    private int index;
    private GButton search_user_top;
    private GButton search_user_botton;
    private GLoader head1;
    private GLoader head2;

    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_RANK2);
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        this.visible = false;
        FindObject();
        InitData();
    }

    public void InitView()
    {
        this.visible = true;
    }
    private void FindObject()
    {
        list = this.GetChild("n2").asList;
        top = this.GetChild("n3").asButton;
        top.visible = false;
        search_user_top = this.GetChild("n4").asButton;
        head1 = search_user_top.GetChild("n1").asCom.GetChild("n0").asLoader;
        search_user_top.visible = false;
        search_user_botton = this.GetChild("n5").asButton;
        head2 = search_user_botton.GetChild("n1").asCom.GetChild("n0").asLoader;
        search_user_botton.visible = false;
        search_user_botton.onClick.Add(() => {
            if ((bool)data["isContain"])
            {
                list.ScrollToView((int)data["index"]);
                search_user_botton.visible = false;
                if (index == 0)
                {
                    top.visible = false;
                }
                else
                {
                    top.visible = true;

                }
            }
            else
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("31077"));
            }
        });
        search_user_top.onClick.Add(() => {
            if ((bool)data["isContain"])
            {
                list.ScrollToView((int)data["index"]);
                search_user_top.visible = false;
                if (index == 0)
                {
                    top.visible = false;
                }
                else
                {
                    top.visible = true;

                }
            }
            else
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("31077"));
            }
        });
        top.onClick.Add(() =>
        {
            list.ScrollToView(0);
            top.visible = false;
            if (index >= 0 && index <= 4)
            {
                search_user_top.visible = false;
                search_user_botton.visible = false;
            }
            else
            {
                search_user_botton.visible = true;
                search_user_top.visible = false;

            }

        });
    }

    private void InitItem()
    {
        if (roleModel.rankData["type"] != null)
        {
            if ((int)roleModel.rankData["type"] == 3)
            {
                list.emptyStr = Tools.GetMessageById("31081");
            }
            else
            {
                list.emptyStr = Tools.GetMessageById("31073");
            }
        }
        list.itemRenderer = OnRender;
        list.SetVirtual();
        list.onChangeNum.Add(this.CheckListNum);
        if (listData != null)
        {
            data = ModelManager.inst.rankModel.IsContainUser(listData, userModel.uid);
            index = (int)data["index"];
            if (data["data"] != null)
            {
                Dictionary<string, object> data1 = (Dictionary<string, object>)data["data"];
//                head1.url = ModelUser.GetHeadUrl(((Dictionary<string, object>)data1["head"])["use"].ToString());
//                head2.url = ModelUser.GetHeadUrl(((Dictionary<string, object>)data1["head"])["use"].ToString());
				Tools.SetLoaderButtonUrl(null,ModelUser.GetHeadUrl(((Dictionary<string, object>)data1["head"])["use"].ToString()),head1);
				Tools.SetLoaderButtonUrl(null,ModelUser.GetHeadUrl(((Dictionary<string, object>)data1["head"])["use"].ToString()),head2);

            }
            list.scrollPane.onScroll.Add(() =>
            {
                if (list.scrollPane.posY <= 0)
                {
                    search_user_top.visible = false;
                    top.visible = false;
                }
                else
                {
                    top.visible = true;
                    //search_user_botton.visible = false;

                }
            });
            list.onTouchEnd.Add(() => {
                int scrollIndex = list.GetFirstChildInView();
                if ((bool)data["isContain"])
                {
                    if (scrollIndex > 0)
                    {
                        top.visible = true;
                        if (scrollIndex + 5 < index)
                        {
                            search_user_top.visible = false;
                            search_user_botton.visible = true;
                        }else if(scrollIndex + 5>=index)
                        {
                            if(scrollIndex <= index)
                            {
                                search_user_top.visible = false;
                                search_user_botton.visible = false;
                            }
                            else
                            {
                                search_user_top.visible = true;
                                search_user_botton.visible = false;
                            }

                        }

                    }
                    else
                    {
                        top.visible = false;
                        if(index>-1 && index < 6)
                        {
                            search_user_top.visible = false;
                            search_user_botton.visible = false;
                        }
                        else
                        {
                            search_user_top.visible = false;
                            search_user_botton.visible = true;
                        }
                    }
                }
                else
                {
                    search_user_top.visible = false;
                    search_user_botton.visible = false;
                    if (scrollIndex > 0)
                        top.visible = true;
                    else
                        top.visible = false;
                }
            });
            if (listData.Count <= 5)
            {
                list.scrollPane.touchEffect = false;
            }
            else
            {
                if ((bool)data["isContain"])
                {
                    if (index > 6)
                    {
                        search_user_botton.visible = true;
                        search_user_top.visible = false;
                    }
                }
            }
            //list.itemRenderer = OnRender;
            //list.SetVirtual();
            //list.numItems = listData.Count;
        }
        //SetListCSS(list, listData.ToArray(), 6, true);
//        Debug.Log(listData.ToArray().Length);
        SetListCSS(list, listData.ToArray(), 6, true);
        //else
        //{
        //    list.itemRenderer = OnRender;
        //    list.SetVirtual();
        //    list.numItems = 0;
        //}

    }

    private void InitData()
    {
        rankModel = ModelManager.inst.rankModel;
        listData = new List<object>();
        if (roleModel.isSave)
        {
            roleModel.isSave = false;
            if (roleModel.tempData != null)
            {
                InitView();
                listData = (List<object>)roleModel.tempData["data"];
                InitItem();
				jump1 =	TimerManager.inst.Add (0.2f, 1, (float f) => {
					list.ScrollToView (((int[])roleModel.tempData ["tag"]) [0]);
					roleModel.tempData = null;
					TimerManager.inst.Remove(jump1);
				});
            }
        }
        else
        {

            if (roleModel.rankData != null)
            {
                if (roleModel.rankData["data"] != null)
                {
                    foreach (object v in (object[])roleModel.rankData["data"])
                    {
                        listData.Add(v);
                    }
                    //                if (!isFirst)
                    InitItem();
                    InitView();
                }

            }
        }

    }

    private void OnRender(int index, GObject item)
    {

        GComponent start = item.asCom.GetChild("n8").asCom;
        GLoader change = item.asCom.GetChild("n9").asLoader;
        GTextField num = item.asCom.GetChild("n1").asTextField;
        GButton head = item.asCom.GetChild("n2").asCom.GetChild("n0").asButton;
        GTextField name = item.asCom.GetChild("n4").asTextField;
        GTextField guild = item.asCom.GetChild("n5").asTextField;
        GTextField lv = item.asCom.GetChild("n2").asCom.GetChild("n2").asTextField;
        GTextField group = item.asCom.GetChild("n6").asTextField;
        GButton mask = item.asCom.GetChild("n11").asButton;
        //GLoader item_bg = item.asCom.GetChild("n0").asLoader;

        GComponent mvpIcon = item.asCom.GetChild("n12").asCom;
        GImage killIcon = item.asCom.GetChild("n13").asImage;
        GImage hurtIcon = item.asCom.GetChild("n14").asImage;
        GImage bg_ = item.asCom.GetChild("itemBg_").asImage;
        bg_.visible = false;

        userModel.GetUnlcok(Config.UNLOCK_GUILD, guild);

        bool isVisible = SetListCSS(item, listData.ToArray(), index);
        if (isVisible)
        {
            switch (rankModel.type)
            {
                case "like":
                    mvpIcon.visible = false;
                    killIcon.visible = false;
                    hurtIcon.visible = true;
                    break;
                case "kill":
                    mvpIcon.visible = false;
                    killIcon.visible = true;
                    hurtIcon.visible = false;
                    break;
                case "mvp":
                    mvpIcon.visible = true;
                    killIcon.visible = false;
                    hurtIcon.visible = false;
                    GLoader mvp= mvpIcon.GetChild("n1").asLoader;
                    Tools.GetResourceUrlForMVP(mvp, "mvp");
                    //mvpIcon.GetChild("n4").asTextField.text = Tools.GetMessageById("24223");
                    break;
            }



            Dictionary<string, object> d1 = (Dictionary<string, object>)listData[index];
            //        if (d1["uid"].ToString().Equals(userModel.uid))
            //        {
            //            item_bg.url = Tools.GetResourceUrl("Image:bg_zidichen7");
            //        }
            //        else
            //        {
            //            item_bg.url = Tools.GetResourceUrl("Image:bg_zidichen3");
            //        }
            //if (index % 2 == 0)
            //{
            //    item_bg.url = "";//Tools.GetResourceUrl("Image2:n_bg_tanban6");
            //}
            //else
            //{
            //    item_bg.url = Tools.GetResourceUrl("Image2:n_bg_tanban6");
            //}

            if (d1["uid"].ToString().Equals(userModel.uid))
            {
                bg_.visible = true;
            }
            
            Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(((Dictionary<string, object>)d1["head"])["use"].ToString()));
            mask.RemoveEventListeners();
            mask.onClick.Add(() => {
                roleModel.SetTempData(listData, new int[] { list.GetFirstChildInView() });
                string fuid = d1["uid"] + "";
                string uid = userModel.uid;
                if (fuid != uid)
                {
                    this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { null, d1["uid"], roleModel.tab_CurSelect1, roleModel.tab_CurSelect2, roleModel.tab_CurSelect3 }));
                }
                else
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13106"));
                }


            });
            if ((int)d1["rank_diff"] == 0)
            {
                change.url = Tools.GetResourceUrl("Image2:n_icon_bianhua3");
                num.text = "";
            }
            else if ((int)d1["rank_diff"] > 0)
            {
                change.url = Tools.GetResourceUrl("Image2:n_icon_bianhua1");
                num.text = Tools.GetMessageColor("[0]" + d1["rank_diff"] + "[/0]", new string[] { "4db10c" });
            }
            else
            {
                change.url = Tools.GetResourceUrl("Image2:n_icon_bianhua2");
                num.text = Tools.GetMessageColor("[0]" + d1["rank_diff"] + "[/0]", new string[] { "d23823" });
            }
            Tools.StartSetValue(start, (index + 1).ToString(), "");
            if (d1["uname"] != null)
            {
                name.text = (string)d1["uname"];
            }
            else
            {
                name.text = d1["uid"] + "";
            }

            if (d1["guild_name"] == null)
            {
                guild.text = Tools.GetMessageById("19955");
            }
            else
            {
                guild.text = d1["guild_name"] + "";
            }
            lv.text = d1["lv"].ToString();
            group.text = d1["like_num"] + "";
        }

    }
}
