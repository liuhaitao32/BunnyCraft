using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;

public class MediatorShare : BaseMediator {
    
    private GButton btn_share;
    private GButton btn_yaoqing;
    private GButton btn;
    private GTextField title1;
    private GTextField title2;
    private GTextField text1;
    private GTextField text2;
    private GTextField text3;
    private GTextField text4;
    private GList list;
    private GTextInput url;
    private GButton btn_Copy;
    private GLoader erBg;
    private GImage fenGeXian;
    private Loads lw;
    private Texture2D topImage;
    private int urlNum;
    private int urlNum_;
    private string share_bitmap;
    private string isUid;
    private string qrCode_str;
    private string yaoQingText;
    private string shareUrl;
    private string baseUrl;
    private ModelShare shareModel;
    private ModelUser userModel;
    private Dictionary<string, object> share;
    private int sumClick;
    private int sumShare;
    private int todaySumClick;
    private int todaySumShare;
    private string qrType;
    //private Controller tabC1;
    //private GButton btn1;
    //private GButton btn2;
    private List<object> data;
    private string shareUrl_;
    private string shareM;
    private GTextField num1;
    private GTextField num2;
    private int clickNum;
    private int clickGetCoin;
    private int shareNum;
    private int shareGetCoin;
    //private GButton btn3;
    private GTextField textContext;
    private GTextInput input;
    private GButton btn_ok;
    private GImage image;
    private GButton btn_weixin;
    private int weekClike;
    private DateTime timeNewClick;
    private GGroup weiChatGroup;
    private DateTime timeNewShare;
    private GButton tabShare;

    public int WeekClickNum { get; private set; }

    public override void Init()
    {
        this.Create(Config.SCENE_SHARE);
        InitData();
        FindObject();
        DispatchManager.inst.Register(MainEvent.EVENTSHARE, ShareResult);
    }
    private void InitData()
    {

        userModel = ModelManager.inst.userModel;
        shareModel = ModelManager.inst.shareModel;
        sumClick = (int)((Dictionary<string, object>)userModel.records["share_data"])["sum_n"];//被点击的总数
        sumShare = (int)((Dictionary<string, object>)userModel.records["share_data"])["sum_num"];//分享出去的总次数
        todaySumClick = userModel.ipList;//今日被点击的数
        todaySumShare = (int)((Dictionary<string, object>)userModel.records["share_data"])["succ_times"];//分享出去的次数
        weekClike = (int)((Dictionary<string, object>)userModel.records["share_data"])["week_n"];
        timeNewClick =(DateTime)((Dictionary<string, object>)userModel.records["share_data"])["t"];
        timeNewShare =(DateTime)((Dictionary<string, object>)userModel.records["share_data"])["succ_time"];
        qrType = "**%%_%%**";
        Dictionary<string, object> dd = (Dictionary<string, object>)DataManager.inst.systemSimple["weixin_gift"];
        string shareM = dd["key"].ToString();

        string md5Result=userModel.uid + shareM + string.Format("{0:s}", ModelManager.inst.gameModel.time).Split('T')[0];
        string byte2String = Tools.MD5(md5Result);
        Debug.Log(byte2String);
        shareUrl = DataManager.inst.systemSimple["share_url"].ToString()+userModel.uid+"&code=" +byte2String;
        shareUrl_=DataManager.inst.systemSimple["share_url_"].ToString()+ userModel.uid + "&code=" + byte2String;
        baseUrl = DataManager.inst.systemSimple["share_base_url"].ToString();
        share = (Dictionary<string, object>)DataManager.inst.systemSimple["share"];
        isUid = userModel.records["invite_uid"] + "";
        qrCode_str = LocalStore.GetLocal(LocalStore.LOCAL_QRCODE+userModel.uid);

        Dictionary<string, object> inviteList = (Dictionary<string, object>)share["invite"];
        List<object> arr1 = Tools.ConvertDicToList(inviteList, "name");
        Tools.Sort(arr1, new string[] { "name:int:0" });
        data = new List<object>();
        foreach (object v in arr1)
        {
            string name = ((Dictionary<string, object>)v)["name"].ToString();
            string value = ((Dictionary<string, object>)v)[name].ToString();
            data.Add(name);
            data.Add(value);
        }
        yaoQingText = Tools.GetMessageById("19441", new string[] {
            data[0]+"",
            data[1]+"",
            data[2]+"",
            data[3]+"",
            data[4]+"",
            data[5]+"",

        });
        clickNum = (int)(((object[])share["share2"])[0]);
        clickGetCoin = (int)(((object[])share["share2"])[1]);
        WeekClickNum = (int)(((object[])share["share2"])[2]);
        shareNum = (int)(((object[])share["share1"])[0]);
        shareGetCoin = (int)(((object[])share["share1"])[1]);
    }

    private bool IsNewWeek(DateTime time)
    {
        string timeClick=string.Format("{0:s}", time).Split('T')[0];
        string now=string.Format("{0:s}", ModelManager.inst.gameModel.time).Split('T')[0];
        string regist=string.Format("{0:s}", userModel.add_time).Split('T')[0];

        DateTime clickTimer=Convert.ToDateTime(timeClick);
        DateTime nowTime = Convert.ToDateTime(now);
        DateTime registTime=Convert.ToDateTime(regist);
        long oneDay = ModelManager.inst.roleModel.Oneday;

        long distance1=(clickTimer.Ticks - registTime.Ticks)/(oneDay*7);
        long distance2= (nowTime.Ticks - registTime.Ticks)/(oneDay*7);
        if (distance1 < distance2) {
            //新的一周
            return true;      
        }
        else
        {
            //是本周
            return false;

        }
    }
    

    private void FindObject()
    {
        GButton btn = this.GetChild("n100").asButton;
        btn.text = Tools.GetMessageById("13132");
        GButton callBack = this.GetChild("n104").asButton;
        callBack.onClick.Add(CallBack);
        title1 = this.GetChild("n2").asTextField;
		title2 = this.GetChild("n13").asTextField;
		text1 = this.GetChild("n10").asTextField;
		text2 = this.GetChild("n11").asTextField;
		text3 = this.GetChild("n14").asTextField;
		text4 = this.GetChild("n15").asTextField;
		url = this.GetChild("n8").asTextInput;
		btn_Copy = this.GetChild("n110").asButton;
		erBg = this.GetChild("n3").asLoader;
		btn_share = this.GetChild("n5").asButton;
        tabShare =GetChild("bar0").asButton;
        if (shareModel.isShareRed())
        {
            userModel.Add_Notice(btn_share, new Vector2(145, -10));
        }
        else
        {
            userModel.Remove_Notice(btn_share);
        }
        
        btn_yaoqing = this.GetChild("n12").asButton;
        btn_yaoqing.GetChild("title").asTextField.textFormat.size = 25;
        //fenGeXian = this.GetChild("n9").asImage;
        list = GetChild("n60").asList;
        num1 = GetChild("n66").asTextField;
        num2 = GetChild("n64").asTextField;
        tabC1 = GetController("c1");
        textContext = this.GetChild("n90").asTextField;
        input = this.GetChild("n0").asCom.GetChild("n1").asTextInput;
        btn_ok = this.GetChild("n84").asButton;
        image = this.GetChild("n99").asImage;
        btn_weixin = this.GetChild("n91").asButton;
        weiChatGroup = this.GetChild("n106").asGroup;
        btn_weixin.text = "";
        bool ok=userModel.GetUnlcok(Config.UNLOCK_WEICHAT,weiChatGroup);
        if (!ok)
        {
            btn_ok.x = 527;
        }
        GetChild("n72").asTextField.text = Tools.GetMessageById("13136");
        btn_ok.text = Tools.GetMessageById("31002");

        //GetChild("n105").asTextField.text=Tools.GetMessageById("13145",new string[] { userModel.uid});

        InitTopBar(new string[] {
                Tools.GetMessageById ("13134"),
                Tools.GetMessageById ("13133"),
                Tools.GetMessageById ("13127")
            });

        tabC1.onChanged.Add(OnChange);
        OnChange();

        if (!Tools.IsNullEmpty(isUid))
        {
            btn_yaoqing.touchable = false;
            btn_yaoqing.grayed = true;
        }
        else
        {
            btn_yaoqing.visible = true;
        }
		share_bitmap = "";
        GetER();//获取二维码
        SetData();

    }

    private void CallBack(EventContext context)
    {
        ViewManager.inst.CloseScene();
    }

    private void SetData()
    {
        

        SetShareData();
        SetInvitData();
        SetActivatData();
    }

    private void SetActivatData()
    {
        textContext.text = BaseUbbParser.inst.Parse(Tools.GetMessageById("31006"));
        input.promptText = Tools.GetMessageColor("[0]" + Tools.GetMessageById("31004") + "[/0]", new string[] { "ffffff" });

        btn_ok.onClick.Add(() => {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["code"] = input.text;
            if (Tools.IsNullEmpty(input.text))
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("31007"));
            }
            else
            {
                NetHttp.inst.Send(NetBase.HTTP_ACTIVATION, dic, (VoHttp vo) =>
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("19008"));
                    Dictionary<string, object> data = (Dictionary<string, object>)vo.data;
                    Dictionary<string, object> user = (Dictionary<string, object>)data["user"];

                    Dictionary<string, object> reward = null;
                    Dictionary<string, object> da = null;

                    Dictionary<string, object> records = (Dictionary<string, object>)user["records"];
                    if (data.ContainsKey("rewards_dict"))
                    {
                        reward = (Dictionary<string, object>)data["rewards_dict"];

                    }
                    if (data.ContainsKey("award_gift"))
                    {
                         da = (Dictionary<string, object>)data["award_gift"];
                    }

                    int el_score = userModel.el_score;
                    int gold = userModel.gold;
                    userModel.UpdateData(data);
                    Tools.FullCard(( (Dictionary<string, object>)data ), gold,"award_gift");
                    userModel.el_score = el_score;
                    if (reward!=null && da!=null)
                    {
                        reward.Remove("award");
                        foreach (Dictionary<string, object> o in da.Values)
                        {
                            foreach (string v in o.Keys)
                            {
                                if (reward.ContainsKey(v))
                                {
                                    switch (v)
                                    {
                                        case "card":
                                            Dictionary<string, object> o1 = (Dictionary<string, object>)o[v];
                                            foreach (string v1 in o1.Keys)
                                            {
                                                Dictionary<string, object> data_ = (Dictionary<string, object>)reward[v];
                                                if (data_.ContainsKey(v1))
                                                {
                                                    ((Dictionary<string, object>)reward[v])[v1] = (int)data_[v1] + (int)o1[v1];
                                                }
                                                else
                                                {
                                                    ((Dictionary<string, object>)reward[v]).Add(v1, o1[v1]);
                                                }
                                            }

                                            break;
                                        default:
                                            int n1 = (int)reward[v];
                                            int n2 = (int)o[v];
                                            n1 += n2;
                                            reward[v] = n1;
                                            break;
                                    }
                                }
                                else
                                {
                                    reward.Add(v, o[v]);
                                }
                            }
                        }
                        ViewManager.inst.ShowGift(reward, Config.EFFECT_MAILBOX);
                        MediatorGiftShow.isExplore = true;
                    }

                    DispatchManager.inst.Dispatch(new MainEvent(MainEvent.RED_UPDATE));
                    weiChatGroup.visible = false;
                    image.visible = false;
                    btn_ok.x = 532;
                    ViewManager.inst.CloseView(this);
                });
            }

        });

        btn_weixin.onClick.Add(() => {
            ViewManager.inst.ShowView<MediatorWeinXinAc>();
        });
        if ((int)ModelManager.inst.userModel.records["weixin"] == 1)
        {
            weiChatGroup.visible = false;
            image.visible = false;
            btn_ok.x = 527;
        }
    }

    private void SetInvitData()
    {
        title1.text = Tools.GetMessageById("19951");
        title2.text = Tools.GetMessageById("19952");
        text3.text = Tools.GetMessageById("19440",new string[] { userModel.uid});
        string str2 = Tools.GetMessageById("19441", new string[] { "[0]" + share["invited"] + "[/0]" });
        text4.text = Tools.GetMessageColor(str2, new string[] { "DE8006" });
        btn_yaoqing.GetChild("title").asTextField.textFormat.size = 30;
        btn_yaoqing.text = Tools.GetMessageById("19948");
        btn_yaoqing.RemoveEventListeners();
        btn_yaoqing.onClick.Add(() =>
        {
            ViewManager.inst.ShowView<MediatorGetWin>();
        });
        list.itemRenderer = Onrender;
        list.numItems = 3;
    }

    private void SetShareData()
    {
        bool isNewsWeek = IsNewWeek(timeNewClick);
        bool isNewDayClick = Tools.IsNewDay(timeNewClick);
        string str1;
        if (isNewDayClick)
        {
            Debug.Log("sssssssss"+((object[])((Dictionary<string, object>)userModel.records["share_data"])["ip_list"]).Length);
            userModel.ipList=0;//今日被点击的数
            todaySumClick = 0;
        }
        bool isNewDayShare = Tools.IsNewDay(timeNewShare);
        if (isNewDayShare)
        {

            ((Dictionary<string, object>)userModel.records["share_data"])["succ_times"] = 0;//今日分享出去的次数
            //sumShare = 0;
            todaySumShare = 0;
        }


        int clickNum = GetRShareNum();
        if (isNewsWeek)//新的一周
        {
            ((Dictionary<string, object>)userModel.records["share_data"])["week_n"]=0;
            weekClike = 0;
        }
        int shareNum = GetShareNum();
        string str0 = Tools.GetMessageById("19438", new string[] { "[0]" + shareGetCoin + "[/0]", "[1]" + shareNum + "[/1]", "[2]" + clickGetCoin + "[/2]", "[3]" + clickNum + "[/3]", "[4]" + (WeekClickNum - weekClike) + "[/4]" });
        //str1 = Tools.GetMessageById("19439", new string[] { "[0]" + clickGetCoin + "[/0]", "[1]" + clickNum + "[/1]", "[2]" + (WeekClickNum - weekClike) + "[/2]" });
        text1.text = Tools.GetMessageColor(str0, new string[] { "666666", "DE8006", "666666", "DE8006", "DE8006" });
        //text2.text = Tools.GetMessageColor(str1, new string[] { "DE8006", "DE8006", "DE8006" });
        url.promptText = shareUrl_;
        url.touchable = false;
        //btn_Copy.GetChild("title").asTextField.textFormat.size = 28;
        btn_Copy.text = Tools.GetMessageById("19949");
        btn_Copy.RemoveEventListeners();
        btn_Copy.onClick.Add(() =>
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("19950"));
            if (PlatForm.inst.pf != PlatForm.EX_LOCAL)
                PlatForm.inst.GetSdk().Call(Ex_Local.CALL_COPYTO, new string[] { shareUrl_ }, sdk_callback);
            else
            {
                TextEditor te = new TextEditor();
                te.content = new GUIContent(shareUrl_);
                te.OnFocus();
                te.Copy();
            }
        });
        btn_share.GetChild("title").asTextField.textFormat.size = 30;
        btn_share.text = Tools.GetMessageById("19947");
        btn_share.RemoveEventListeners();
        btn_share.onClick.Add(() => {
            userModel.GetUnlcok(Config.UNLOCK_SHARD, null, true);
            if (share_bitmap != "")
            {
                shareModel.viewX = this.x+180;
                shareModel.viewY = this.y + btn_share.y + 75;
                MediatorAd.typeAd = ModelShare.SHARE_FREIND;
				shareModel.type = ModelShare.SHARE_FREIND;
//                ViewManager.inst.ShowView<MediatorAd>(false,false);
				ViewManager.inst.ShowView<MediatorShareBtn>(true,false);
            }
        });

        string t1 = Tools.GetMessageById("13130", new string[] { "[0]" + (todaySumShare * shareGetCoin + todaySumClick * clickGetCoin) + "[/0]" });
        string t2 = Tools.GetMessageById("13131", new string[] { "[0]" + (sumShare * shareGetCoin + sumClick * clickGetCoin) + "[/0]" });

        num1.text = Tools.GetMessageColor(t1, new string[] { "DE7E03"});
        num2.text = Tools.GetMessageColor(t2, new string[] { "DE7E03" });
    }

    private void GetER()
    {
        if (!Tools.IsNullEmpty(qrCode_str) && qrCode_str.Length > 0)
        {
            share_bitmap = qrCode_str;
            topImage = PhoneManager.inst.Base64ToTexter2d(share_bitmap);
            erBg.texture = new NTexture(topImage);
        }
        else
        {
			lw = LoaderManager.inst.Load(shareUrl + ".jpg", (object w) => {
                if (this.group == null) return;
                topImage = (Texture2D)w;
                if (topImage != null)
                {
                    if (this.group == null) return;
                    erBg.texture = new NTexture(topImage);
					share_bitmap = Convert.ToBase64String(topImage.EncodeToJPG());
//                    Log.debug("topImage:" + topImage);
//                    Log.debug("erBg" + erBg);
                    LocalStore.SetLocal(LocalStore.LOCAL_QRCODE + userModel.uid, share_bitmap);

                }
            });
        }
    }

    private void OnChange()
    {
        base.OnTabChange();

        //string title_1 = "[0]" + Tools.GetMessageById("13134") + "[/0]";
        //string title_2 = "[0]" + Tools.GetMessageById("13133") + "[/0]";
        //string title_3 = "[0]" + Tools.GetMessageById("13127") + "[/0]";



        //GTextField text_1 = btn1.GetChild("title").asTextField;
        //text_1.text = Tools.GetMessageById("13134");
        //Tools.SetRootTabTitle(text_1, "", 30, "ffffff", "#1077BA", new Vector2(0, 0), 3);
        //GTextField text_2 = btn2.GetChild("title").asTextField;
        //text_2.text = Tools.GetMessageById("13133");
        //Tools.SetRootTabTitle(text_2, "", 30, "ffffff", "#1077BA", new Vector2(0, 0), 3);
        //GTextField text_3 = btn3.GetChild("title").asTextField;
        //text_3.text = Tools.GetMessageById("13127");
        //Tools.SetRootTabTitle(text_3, "", 30, "ffffff", "#1077BA", new Vector2(0, 0), 3);
        switch (tabC1.selectedIndex)
        {
            case 0:
                //Tools.SetRootTabTitle(text_1, "", 30, "ffffff", "#C16C2B", new Vector2(0, 0), 3);
                userModel.Remove_Notice(tabShare);
                break;
            case 1:
                if (shareModel.isShareRed())
                {
                    userModel.Add_Notice(tabShare, new Vector2(145, -10));
                }
                else
                {
                    userModel.Remove_Notice(tabShare);
                }
                break;
            case 2:
                if (shareModel.isShareRed())
                {
                    userModel.Add_Notice(tabShare, new Vector2(145, -10));
                }
                else
                {
                    userModel.Remove_Notice(tabShare);
                }
                //Tools.SetRootTabTitle(text_3, "", 30, "ffffff", "#C16C2B", new Vector2(0, 0), 3);
                break;
        }
    }

    private void Onrender(int index, GObject item)
    {
        GComponent obj=item.asCom;
        GTextField text=obj.GetChild("n0").asTextField;
        GTextField text1=obj.GetChild("n5").asTextField;
        GTextField text2=obj.GetChild("n6").asTextField;
        text2.text = Tools.GetMessageById("14062");
        text.text = Tools.GetMessageById("13135");
        text1.text = data[index * 2].ToString();
        //string str= "[0]" + data[index * 2].ToString() + "[/0]";
        //Tools.SetTextFieldStrokeAndShadow(text1, "#C16C2B", new Vector2(0, 0), 3);
        obj.GetChild("n2").asTextField.text = "x"+data[index * 2+1].ToString();
    }

    public override void Clear()
    {
        base.Clear();
		if(lw!=null)
        Tools.Clear(lw);
        DispatchManager.inst.Unregister(MainEvent.EVENTSHARE, ShareResult);
    }
	

    private void ShareResult(MainEvent e)
    {
        string a = e.data + "";
        switch (a)
        {
            case "1":
                btn_yaoqing.touchable = false;
                btn_yaoqing.grayed = true;
                break;
            case "2":
                //string str0 = Tools.GetMessageById("19438", new string[] { "[0]" + GetShareNum() + "[/0]" });
                //text1.text = BaseUbbParser.inst.Parse(Tools.GetMessageColor(str0, new string[] { "DE8006" }));
                //((Dictionary<string, object>)userModel.records["share_data"])["sum_num"] = (int)((Dictionary<string, object>)userModel.records["share_data"])["sum_num"] + 1;
                //((Dictionary<string, object>)userModel.records["share_data"])["succ_times"] = (int)((Dictionary<string, object>)userModel.records["share_data"])["succ_times"] + 1;
                //((Dictionary<string, object>)userModel.records["share_data"])["succ_time"] = ModelManager.inst.gameModel.time;
                InitData();
                FindObject();
                SetData();
                break;
            case "3":
                ((Dictionary<string, object>)userModel.records["share_data"])["sum_n"] = (int)((Dictionary<string, object>)userModel.records["share_data"])["sum_n"] +1;
                userModel.ipList += 1; ;
                ((Dictionary<string, object>)userModel.records["share_data"])["week_n"]= (int)((Dictionary<string, object>)userModel.records["share_data"])["week_n"]+1;
                ((Dictionary<string, object>)userModel.records["share_data"])["t"] = ModelManager.inst.gameModel.time;
                InitData();
                SetData();
                break;

        }
    }

   
    void sdk_callback(object str)
    {
    }

    //点击链接次数
    private int GetRShareNum()
    {
        urlNum_ = clickNum;
        urlNum = todaySumClick;
        int num = urlNum_ - urlNum;

        if (num < 0)
            num = 0;
        return num;
    }
    //分享次数
    private int GetShareNum()
    {
        int shareNum_ = shareNum;
        int shareNum__ = todaySumShare;//+		["sum_num"]	0	System.Collections.DictionaryEntry
        int num = shareNum_ - shareNum__;
        if (num < 0)
            num = 0;
        return num;
    }

    private string ComposeImage(Texture2D baseImage, Texture2D topImage)
    {
        Vector2 v2 = new Vector2();
        v2.x = baseImage.width - topImage.width;
        v2.y = 0;
        Texture2D cc = PhoneManager.inst.GetTexture2d(baseImage, topImage, v2);
		return System.Convert.ToBase64String(cc.EncodeToJPG());
    }
}
