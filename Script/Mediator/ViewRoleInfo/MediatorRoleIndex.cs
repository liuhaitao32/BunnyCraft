using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using FairyGUI;
using System.Text.RegularExpressions;
using DG.Tweening;

public class MediatorRoleIndex : BaseMediator
{
    private ModelUser userModel;
    private ModelRole roleModel;
    private Dictionary<string, object> otherInfo;
    //private Dictionary<string, object> addrDiction;
    private Dictionary<string, object> friend_like_s;
    private Dictionary<string, object> friendLike;
    private Dictionary<string, object> list_like;
    private Dictionary<string, object> userInfo;
    private string[] fans_name;
    private string[] fun2_player;
    private string[] fun2;
    private string[] fun1_player;
    private string fuid;
    private string[] arrStory;
    private string sign_text_temp;
    private int storyNum;
    private int expend_coin_type;
    private int i_like_num;
    private bool typeShield;
    private bool typeFollow;
    private bool isStory = true;
    private List<object> list;
    private List<object> arr1;
    private GButton photoHead;
    private GLoader sex;
    private GTextField name;
    private GTextInput story;
    private GTextField loveNum;
    private GTextField fansNum;
    private GTextField attenNum;
    private GButton like;
    private GList like_list;
    private GButton more;
    private GButton attention;
    private GButton watch;
    private GButton shield;
    private string btnBgUp;
    private string btnBgDo;
    private string textAttention;
    private string textAttentiond;
    private string textShield;
    private string textShieldd;
    private string textBtnStrokeColor;
    private string textBtnColor;
    //private GButton save_img;
    //private GButton save_btn;
    //private GTextField addr;
    private GTextField lv;
    //private GButton openGuild;
    private GImage like_bg;
    private GGraph graph;
    //private GImage image_hurt_;
    //private GTextField guildName;
    private GButton guild;
    private Controller attentionAtr;
    private Controller shieldAtr;
    private Controller watchAtr;
    private Controller guildAtr;
    private GComponent photoCom;
    private string textBtnStrokeColor1;
    private string fontSize;
    private string textguild;
    private bool isFirstLove = false;
    private string textGuild;
    private string textguil;
    private string textGuildd;
    private GTextField uid;
    private string btnBgDo_;
    private string textBtnStrokeColor2;
    private object[] need_coin;
    private object[] gift_coin;
    private GImage editImage;
    private GButton btnMicro;
    private GButton btnOpenMicro;
    private bool isBtnMicro;
    private bool isBtnOpenMicro;

    public override void Init()
    {
        base.Init();
        this.Create(Config.VIEW_ROLEIDEX);
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;



        photoCom = GetChild("n112").asCom;
        photoHead = photoCom.GetChild("n0").asButton;
        lv = photoCom.GetChild("n2").asTextField;
        sex = this.GetChild("n26").asLoader;
        name = this.GetChild("n10").asTextField;
        story = this.GetChild("n12").asTextInput;
        //image_hurt_ = this.GetChild ("n103").asImage;

        story.promptText = Tools.GetMessageById("13107") + Tools.GetMessageById("13105");
        loveNum = this.GetChild("n14").asTextField;
        fansNum = this.GetChild("n16").asTextField;
        attenNum = this.GetChild("n18").asTextField;
        //lv = this.GetChild ("n76").asTextField;
        like_list = this.GetChild("n21").asList;
        like = this.GetChild("n25").asButton;
        graph = this.GetChild("n105").asGraph;
        like_bg = like.asCom.GetChild("n2").asImage;
        like_bg.visible = false;
        like.onClick.Add(Like);
        more = this.GetChild("n20").asButton;
        more.onClick.Add(More);
        more.text = Tools.GetMessageById("13081");
        attention = this.GetChild("n22").asButton;
        attention.grayed = false;
        attention.onClick.Add(Attention);
        attentionAtr = attention.GetController("c1");
        attention.visible = false;
        watch = this.GetChild("n0").asButton;
        //watch.text = Tools.GetMessageById ("13014");
        watch.visible = false;
        watch.onClick.Add(Watch);
        shield = this.GetChild("n23").asButton;
        shield.onClick.Add(Shield);
        shield.visible = false;
        editImage = GetChild("n110").asImage;
        btnMicro = GetChild("n117").asButton;
        btnMicro.onClick.Add(BtnMicro);
        btnOpenMicro=GetChild("n121").asButton;
        btnOpenMicro.onClick.Add(BtnOpenMicro);

        isBtnMicro = userModel.GetUnlcok(Config.UNLOCK_PAY_MICRO, btnMicro);

        GetChild("n15").asTextField.text = Tools.GetMessageById("13011");
        GetChild("n17").asTextField.text = Tools.GetMessageById("13012");
        GetChild("n19").asTextField.text = Tools.GetMessageById("13013");
        uid = GetChild("n116").asTextField;


        //save_img = this.GetChild ("n30").asButton;
        //save_img.touchable = false;
        //save_img.visible = true;
        //save_btn = this.GetChild ("n31").asButton;
        //save_btn.visible = false;
        //save_btn.text = Tools.GetMessageById ("13017");
        //save_btn.RemoveEventListeners ();
        //save_btn.onClick.Add (() =>
        //{
        //	Save ();
        //});
        //guildName = this.GetChild ("n32").asTextField;
        //openGuild = this.GetChild ("n102").asButton;
        //openGuild.GetChild ("title").asTextField.textFormat.size = 28;
        //openGuild.text = Tools.GetMessageById ("13072");
        //openGuild.onClick.Add(OpenGuild);
        guild = this.GetChild("n111").asButton;
        guild.onClick.Add(Guild);
        guildAtr = guild.GetController("c1");
        guild.visible = false;
        btnBgUp = "n_icon_guanzhu";
        btnBgDo = "n_icon_guanzhu_";
        btnBgDo_ = "n_icon_guanzhu_2";
        textAttention = "13013";
        textAttentiond = "13018";
        textBtnColor = "FFFFFF";
        textBtnStrokeColor = "#38A9E3";
        textBtnStrokeColor1 = "#666666";
        textBtnStrokeColor2 = "#2FAC50";
        textShield = "13016";
        textShieldd = "13032";
        textGuild = "19908";
        textGuildd = "19955";
        fontSize = "";
        InitDate();
        Tools.SetButtonBgAndColor(watch, btnBgUp, "13014", fontSize, textBtnColor, textBtnStrokeColor);
        this.AddGlobalListener(MainEvent.ROLE_UPDATE, viewUpdate);
        Log.debug(like.x + " - " + this.x);
        if (!userModel.IsLove())
        {
            if (roleModel.otherInfo["uid"].ToString() != userModel.uid)
            {
                if (GuideManager.inst.Check("101:0"))
                {
                    GuideManager.inst.Show(this);
                }
            }
        }

    }

    private void BtnOpenMicro(EventContext context)
    {

    }

    private void BtnMicro(EventContext context)
    {
        ViewManager.inst.ShowView<MediatorRoleMicro>();
    }

    private void Watch(EventContext context)
    {
        ViewManager.inst.ShowText(Tools.GetMessageById("13146"));
    }

    private void viewUpdate(MainEvent e)
    {
        //Dictionary<string, object> obj = (Dictionary<string, object>)e.data;
        //string tag = (string)obj["tag"];
        //if (tag.Equals("photo"))
        //{
        //    //photoHead.visible = true;
        //    Dictionary<string, object> bean = (Dictionary<string, object>)obj["value"];
        //    Tools.SetLoaderButtonUrl(photoHead, ModelUser.GetHeadUrl((string)bean["name"]));
        //}
        //if (tag.Equals("uname"))
        //{
        //    SetName(name, userModel.uname);
        //}
        //if (tag.Equals("sex"))
        //{
        //    sex.url = Tools.GetSexUrl(userModel.sex);
        //}
        //if (tag.Equals("micro"))
        //{
        //    InitDate();
        //}
        InitDate();
    }

    private void InitDate()
    {

        Tools.UpdataHeadTime();
        //addrDiction = (Dictionary<string, object>)DataManager.inst.systemSimple["area_config"];
        storyNum = (int)DataManager.inst.systemSimple["story_num"];
        friend_like_s = (Dictionary<string, object>)DataManager.inst.systemSimple["friend_like"];
        need_coin = (object[])friend_like_s["need_coin"];
        gift_coin = (object[])friend_like_s["gift_coin"];

        fuid = roleModel.fuid;
        otherInfo = roleModel.otherInfo;
        uid.text = Tools.GetMessageById("13028") + otherInfo["uid"];

        if (otherInfo["uid"].ToString().Equals(userModel.uid))
        {
            if (isBtnMicro)
            {
                btnMicro.visible = true;
                //是否是第一次的
                if (!ModelRole.isHasMicro)
                {
                    userModel.Add_Notice(btnMicro, new Vector2(40, 0), 0, false, false, true);
                }
                else
                {
                    btnOpenMicro.visible = true;
                    userModel.Remove_Notice(btnMicro);
                }
            }
            editImage.visible = true;
            graph.visible = false;
            attention.visible = false;
            watch.visible = false;
            shield.visible = false;
            guild.visible = false;
            InitBaseView();
            InitBaseList();
        }
        else
        {
            if (isBtnMicro)
            {
                if (false)
                {

                }
                else
                {
                    btnOpenMicro.visible = true;
                    btnMicro.visible = true;
                    btnMicro.touchable = false;
                }
            }
            i_like_num = (int)otherInfo["ilike_num"];
            graph.visible = true;
            photoCom.touchable = false;
            attention.visible = true;
            watch.visible = true;
            shield.visible = true;
            guild.visible = true;
            userModel.GetUnlcok(Config.UNLOCK_GUILD, guild);
            //save_btn.visible = false;
            //save_img.visible = false;
            if ((bool)otherInfo["if_black"])
            {
                //是拉黑的关系
                Tools.SetButtonBgAndColor(attention, btnBgUp, textAttention, fontSize, textBtnColor, textBtnStrokeColor);
                Tools.SetButtonBgAndColor(shield, btnBgDo, textShieldd, fontSize, textBtnColor, textBtnStrokeColor1);
                typeShield = true;
            }
            else
            {
                Tools.SetButtonBgAndColor(shield, btnBgUp, textShield, fontSize, textBtnColor, textBtnStrokeColor);
                typeShield = false;
            }
            InitBaseView();
            InitBaseList();
        }

        fans_name = new string[] {

        };
        fun2_player = new string[] {
            Tools.GetMessageById ("13014"),
            Tools.GetMessageById ("13015"),
            Tools.GetMessageById ("13016")
        };
        fun2 = new string[] { Tools.GetMessageById("13017") };
        fun1_player = new string[] { Tools.GetMessageById("13018") };
    }

    private void Guild(EventContext context)
    {
        bool isOk = userModel.GetUnlcok(Config.UNLOCK_GUILD, null, true);
        if (!isOk)
            return;
        if (otherInfo["guild"] != null)
        {
            NetHttp.inst.Send(NetBase.HTTP_GUILD_INFO, "gid=" + (otherInfo["guild_id"]).ToString(), (VoHttp vo) =>
         {
             MediatorGuildInfo.type = 1;
             MediatorGuildInfo.data = (Dictionary<string, object>)(vo.data);
             ViewManager.inst.ShowView<MediatorGuildInfo>();
         });
        }
        else
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("13108"));
        }
    }
    //private void Guild(EventContext context)
    //{
    //    //Tools.SetButtonBgAndColor(guild, btnBgDo, textguild, fontSize, textBtnColor, textBtnStrokeColor1);
    //}
    private void Shield(EventContext context)
    {
        if (typeShield)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["fuid"] = otherInfo["uid"];
            NetHttp.inst.Send(NetBase.HTTP_CANCLESHIELDING, data, (VoHttp vo) =>
           {
               if ((bool)vo.data == true)
               {
                   typeShield = false;
                   otherInfo["if_black"] = false;
                   attention.grayed = false;
                   Tools.SetButtonBgAndColor(attention, btnBgUp, textAttention, fontSize, textBtnColor, textBtnStrokeColor);
                   Tools.SetButtonBgAndColor(shield, btnBgUp, textShield, fontSize, textBtnColor, textBtnStrokeColor);

               }
           });
        }
        else
        {  //拉黑
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["fuid"] = otherInfo["uid"];
            NetHttp.inst.Send(NetBase.HTTP_SHIELDING, data, (VoHttp vo) =>
           {
               if ((bool)vo.data == true)
               {
                   typeShield = true;
                   if (typeFollow)
                   {
                       fansNum.text = (Convert.ToInt32(fansNum.text) - 1).ToString();
                   }
                   typeFollow = false;
                   Tools.SetButtonBgAndColor(attention, btnBgUp, textAttention, fontSize, textBtnColor, textBtnStrokeColor);
                   Tools.SetButtonBgAndColor(shield, btnBgDo, textShieldd, fontSize, textBtnColor, textBtnStrokeColor1);

                   otherInfo["if_black"] = true;


               }
           });
        }
        roleModel.SetTempOpenData(otherInfo);

    }

    private void Attention(EventContext context)
    {
        if (!typeFollow)
        {
            if (attention.grayed)
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("10029"));
            }
            else
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["fuid"] = otherInfo["uid"];
                NetHttp.inst.Send(NetBase.HTTP_FOLLOW, data, (VoHttp vo) =>
               {
                   roleModel.AddAttentionFight(otherInfo["uid"].ToString());
                   if ((bool)vo.data == true)
                   {
                       typeFollow = true;
                       int num = Convert.ToInt32(fansNum.text) + 1;
                       fansNum.text = num + "";
                       otherInfo["fans_num"] = num;
                       otherInfo["if_follow"] = true;
                       Tools.SetButtonBgAndColor(attention, btnBgDo_, textAttentiond, fontSize, textBtnColor, textBtnStrokeColor2);



                   }
                   else
                   {
                       ViewManager.inst.ShowText(Tools.GetMessageById("13120"));
                   }
               });
            }
        }
        else
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["fuid"] = otherInfo["uid"];
            NetHttp.inst.Send(NetBase.HTTP_UNFOLLOW, data, (VoHttp vo) =>
           {
               //ViewManager.inst.ShowText(Tools.GetMessageById("13046"));
               roleModel.RemoveAttentionFight(otherInfo["uid"].ToString());
               typeFollow = false;
               if ((bool)vo.data == true)
               {
                   int num = Convert.ToInt32(fansNum.text) - 1;
                   fansNum.text = num + "";
                   otherInfo["fans_num"] = num;
                   otherInfo["if_follow"] = false;
                   Tools.SetButtonBgAndColor(attention, btnBgUp, textAttention, fontSize, textBtnColor, textBtnStrokeColor);

               }
           });
        }
        roleModel.SetTempOpenData(otherInfo);

    }

    private void More(EventContext context)
    {
        Dictionary<string, object> dc = new Dictionary<string, object>();
        dc["fuid"] = otherInfo["uid"];
        dc["page_num"] = 1;
        NetHttp.inst.Send(NetBase.HTTP_GETFRIENDLISK, dc, (VoHttp vo) =>
       {
           roleModel.roleLove = (Dictionary<string, object>)vo.data;
           ViewManager.inst.ShowView<MediatorRoleLove>();
       });
    }

    private void Like(EventContext context)
    {
        if (otherInfo["uid"].ToString().Equals(userModel.uid))
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("13055"));
        }
        else
        {
            SoundManager.inst.PlaySound(Config.SOUND_LIKE);
            if (i_like_num < need_coin.Length)
            {
                if (i_like_num == 0)
                {
                    SendLove();
                }
                else
                {
                    if (userModel.IsLove())
                    {
                        GuideManager.inst.Clear();
                    }
                    int coin_num = (int)need_coin[i_like_num - 1];
                    roleModel.like_Coin = coin_num;
                    expend_coin_type = coin_num;
                    ViewManager.inst.ShowAlertLike(Config.ASSET_COIN, coin_num, Callback);
                }


            }
            else
            {
                int coin_max = (int)need_coin[need_coin.Length - 1];
                roleModel.like_Coin = coin_max;
                expend_coin_type = coin_max;
                ViewManager.inst.ShowAlertLike(Config.ASSET_COIN, coin_max, Callback);
            }
        }
    }

    private void Callback(int bo)
    {
        if (bo == 1)
        {
            if (userModel.coin > roleModel.like_Coin)
            {
                Debug.Log("coin" + userModel.coin);

                Dictionary<string, object> dc = new Dictionary<string, object>();
                dc["fuid"] = otherInfo["uid"];
                NetHttp.inst.Send(NetBase.HTTP_FRIENDLIKE, dc, (VoHttp vo) =>
               {
                   Dictionary<string, object> dy = (Dictionary<string, object>)vo.data;
                   Dictionary<string, object> dd = (Dictionary<string, object>)dy["user"];
                   Dictionary<string, object> dr = (Dictionary<string, object>)dd["records"];
                   Dictionary<string, object> dl = (Dictionary<string, object>)dr["day_like"];
                   userModel.coin = userModel.coin - roleModel.like_Coin;
                   AddLikeNum(dd, dl, dy);
                   AddList();
               });
            }
            else
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13057"));
            }
        }
    }

    private void AddList()
    {
        Dictionary<string, object> dc1 = new Dictionary<string, object>();
        dc1["head_use"] = userModel.head["use"];
        dc1["uid"] = userModel.uid;
        arr1 = new List<object>();
        foreach (object data in list)
        {
            string uid = ((Dictionary<string, object>)data)["uid"].ToString();
            if (uid.Equals(userModel.uid))
            {
                list.Remove(data);
                return;
            }
        }
        list.Insert(0, dc1);
        like_list.itemRenderer = List_Render;
        like_list.numItems = list.Count;
    }

    private void SendLove()
    {
        Dictionary<string, object> dc = new Dictionary<string, object>();
        dc["fuid"] = otherInfo["uid"];
        like.touchable = false;
        NetHttp.inst.Send(NetBase.HTTP_FRIENDLIKE, dc, (VoHttp vo) =>
       {
//           Debug.Log(vo.data);
           if (!userModel.IsLove())
           {
               userModel.SetLove(true);
               if (GuideManager.inst.Check("101:1"))
               {
                   GuideManager.inst.Next(true);
                   //GuideManager.inst.Show(this);
               }
           }
           else
           {
               GuideManager.inst.Clear();
           }

           Dictionary<string, object> dy = (Dictionary<string, object>)vo.data;
           Dictionary<string, object> dd = (Dictionary<string, object>)dy["user"];
           Dictionary<string, object> dr = (Dictionary<string, object>)dd["records"];
           Dictionary<string, object> dl = (Dictionary<string, object>)dr["day_like"];
           like_bg.visible = true;
           AddLikeNum(dd, dl, dy);
           AddList();
       });
    }

    private void AddLikeNum(Dictionary<string, object> dd, Dictionary<string, object> dl, Dictionary<string, object> dy)
    {
        ComHurt hurt = new ComHurt();
        this.AddChild(hurt.group);
        Vector2 v2 = new Vector2(like.x + 4, like.y + 9);
        hurt.x = v2.x;
        hurt.y = v2.y;
        v2.y = v2.y - 100;
        hurt.TweenFade(0, 0.9f);
        //SoundManager.inst.PlaySound(Config.SOUND_LIKE);
        hurt.TweenMove(v2, 0.2F).OnComplete(() =>
        {
            this.RemoveChild(hurt.group);
            i_like_num += 1;
            like.touchable = true;
            otherInfo["ilike_num"] = i_like_num;
            int num = Convert.ToInt32(loveNum.text) + 1;
            loveNum.text = num + "";
            otherInfo["like_num"] = num;
        });
        if ((int)dl["n"] <= (int)gift_coin[0])
        {

            Dictionary<string, object> dIcon = new Dictionary<string, object>();
            dIcon[Config.ASSET_COIN] = gift_coin[1];
            ViewManager.inst.ShowIcon(dIcon, () =>
           {
               userModel.UpdateData(dy);
           },like);
        }
        else
        {
            userModel.UpdateData(dy);
        }

    }

    private void InitBaseView()
    {
        lv.text = otherInfo["lv"].ToString();
        //if (!Tools.IsNullEmpty(otherInfo["area"]))
        //{
        //    //addr.text = roleModel.GetAddrCfg (addrDiction, "", otherInfo ["area"].ToString ());
        //}
        //else
        //{
        //    //addr.text = Tools.GetMessageById ("14501");
        //}
        sex.url = Tools.GetSexUrl(otherInfo["sex"]);



        if (otherInfo["uname"] != null)
        {
            SetName(name, otherInfo["uname"].ToString());
        }
        else
        {
            SetName(name, otherInfo["uid"] + "");

        }
        if (otherInfo["guild"] == null)
        {
            guild.touchable = false;
            //guild.text = Tools.GetMessageById("19955");
            Tools.SetButtonBgAndColor(guild, btnBgDo, textGuildd, fontSize, textBtnColor, textBtnStrokeColor1);

        }
        else
        {
            //guild.text = otherInfo["guild"] + "";
            Tools.SetButtonBgAndColor(guild, btnBgUp, textGuild, fontSize, textBtnColor, textBtnStrokeColor);
            //guild.text = Tools.GetMessageById("19908");
        }
        if (otherInfo["uid"].ToString().Equals(userModel.uid))
        {
            if (Tools.checkHead())
            {
                Tools.SetLoaderButtonUrl(photoHead, ModelUser.GetHeadUrl((string)userModel.head["use"]));
            }
            else
            {
                Tools.SetLoaderButtonUrl(photoHead, ModelUser.GetHeadUrl("h01"));
                Dictionary<string, object> data1 = new Dictionary<string, object>();
                data1["head_key"] = "h01";
                NetHttp.inst.Send(NetBase.HTTP_CHOOSE_HEAD, data1, (VoHttp vo1) =>
                {
                    userModel.UpdateData(vo1.data);
                });
            }


            like_bg.visible = true;
            //string str_story = "";
            if (!Tools.IsNullEmpty(userModel.story))
            {
                story.text = userModel.story;
                sign_text_temp = userModel.story;
            }
            else
            {
                sign_text_temp = "";
            }

            story.onFocusOut.Add(() =>
            {
                  story.text = story.text.Trim();
                
                story.text = FilterManager.inst.Exec(story.text);
                  if (!story.text.Equals(sign_text_temp))
                  {
                      isStory = true;
                  }
                  sign_text_temp = story.text;
                  Save();
              });
            story.onChanged.Add(() =>
            {
                story.text = Tools.GetStringByLength(story.text, storyNum);
                story.text = Tools.StrReplace(story.text);

            });

        }
        else
        {
            Dictionary<string, object> dc = (Dictionary<string, object>)otherInfo["head"];
            Tools.SetLoaderButtonUrl(photoHead, ModelUser.GetHeadUrl((string)dc["use"]));
            if (otherInfo["story"] != null)
            {
                story.text = (string)otherInfo["story"];
                story.text = Regex.Replace(story.text, @"[/n/r]", "");
                story.text = story.text.TrimEnd((char[])"/n/r".ToCharArray());
            }
            story.touchable = false;
        }
    }

    private void SetName(GTextField name, string v)
    {
        //if (v.Length < 8)
        //{
        //    name.x = 300;
        //}
        //else
        //{
        //    name.x = 315;
        //}
        name.text = v;
    }

    private void InitBaseList()
    {
        list = new List<object>();
        if (otherInfo["fans_num"] != null)
        {
            fansNum.text = otherInfo["fans_num"] + "";
        }

        if (otherInfo["follow_num"] != null)
        {
            attenNum.text = otherInfo["follow_num"] + "";
        }
        loveNum.text = otherInfo["like_num"].ToString();
        list_like = (Dictionary<string, object>)otherInfo["friend_like"];
        if (list_like.Count > 0)
        {
            if (list.Count != 0)
            {
                list.Clear();
            }
            foreach (object v in (object[])(list_like["like_res"]))
            {
                list.Add(v);
            }
            if (list.Count != 0)
            {
                like_list.itemRenderer = List_Render;
                like_list.numItems = list.Count;
            }
        }
        photoHead.onClick.Add(() =>
       {
           ViewManager.inst.ShowView<MediatorRolePhotoHead>();
       });
        if (!otherInfo["uid"].ToString().Equals(userModel.uid))
        {
            if (i_like_num != 0)
            {
                like_bg.visible = true;
            }
            else
            {
                like_bg.visible = false;
            }

            if ((bool)otherInfo["if_follow"])
            {


                Tools.SetButtonBgAndColor(attention, btnBgDo_, textAttentiond, fontSize, textBtnColor, textBtnStrokeColor2);

                typeFollow = true;


            }
            else
            {
                Tools.SetButtonBgAndColor(attention, btnBgUp, textAttention, fontSize, textBtnColor, textBtnStrokeColor);
                typeFollow = false;
            }
        }
    }

    private void List_Render(int index, GObject item)
    {
        GButton obj = item.asCom.GetChild("n0").asButton;
        GImage bg = item.asCom.GetChild("n1").asImage;
        bg.visible = false;
        Dictionary<string, object> dc = (Dictionary<string, object>)list[index];
        string uid = dc["uid"] + "";
        if (uid.Equals(userModel.uid))
        {
            bg.visible = true;
        }
        string dc1 = (string)(dc["head_use"]);
        Tools.SetLoaderButtonUrl(obj, ModelUser.GetHeadUrl(dc1));
        obj.RemoveEventListeners();
        obj.onClick.Add(() =>
       {
           if (!uid.Equals(userModel.uid))
           {
               this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] {
                    otherInfo ["uid"].ToString (),
                    dc ["uid"].ToString (),
                    roleModel.tab_Role_CurSelect1,
                    roleModel.tab_Role_CurSelect2,
                    roleModel.tab_Role_CurSelect3
               }));
           }
           else
           {
               ViewManager.inst.ShowText(Tools.GetMessageById("13106"));
           }
       });
    }

    private void Save()
    {

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["story"] = sign_text_temp;
        NetHttp.inst.Send(NetBase.HTTP_CHANGE_STORY, dic, (VoHttp v) =>
       {
           if ((bool)v.data)
           {
               userModel.story = sign_text_temp;
               //save_img.visible = true;
               //save_btn.visible = false;
           }
       });
    }

    public override void Clear()
    {
        base.Clear();
        if (list != null)
        {
            list.Clear();
        }
        RemoveGlobalListener(MainEvent.ROLE_UPDATE, viewUpdate);
    }
}
