using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using FairyGUI;

public class MediatorGuestBook : BaseMediator {
    private ModelRole roleModel;
    private ModelUser userModel;
    private List<object> guestBookList;
    private int listNum;
    private GList list_guest;
    private GTextInput input_content;
    private GButton send;
    private int tag_g;
    private DateTime timer;
    private Dictionary<string, object> otherInfo;
    private int isScroll=4;
    private Dictionary<string, object> msgConfig;

    public override void Init()
    {
        base.Init();
        this.Create(Config.VIEW_ROLEGUESTBOOK);
        FindObject();
        InitDate();
        if (otherInfo["uid"].ToString().Equals(userModel.uid))
        {
            if (userModel.Get_NoticeState(ModelUser.RED_MSGBOARD) > 0)
            {
                userModel.Del_Notice(ModelUser.RED_MSGBOARD);
            }
        }


    }
    public void InitView()
    {
    }

    private void InitDate()
    {
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        guestBookList = new List<object>();
        msgConfig=(Dictionary<string,object>)DataManager.inst.systemSimple["message_config"];
		Dictionary<string, object> dc = new Dictionary<string, object>();
        otherInfo=roleModel.otherInfo;
        dc["uid"] = otherInfo["uid"];
        dc["id"] = 0;
        NetHttp.inst.Send(NetBase.HTTP_GETMSG, dc, (VoHttp vo) =>
        {
            listNum = (int)((Dictionary<string, object>)(vo.data))["msg_count"];
            Dictionary<string, object> temp = (Dictionary<string, object>)(vo.data);
            object[] obj = (object[])temp["msg_list"];
            if (obj.Length > 0)
            {
                Dictionary<string, object> dc_ = new Dictionary<string, object>();
                dc_["uid"] = "more";
                dc_["subtime"] = new DateTime();
                if (obj.Length > 4)
                {
                    guestBookList.Add(dc_);
                }
                Dictionary<string, object> temp_ = (Dictionary<string, object>)obj[obj.Length - 1];
                tag_g = (int)temp_["id"];
                foreach (object st in (object[])temp["msg_list"])
                {
                    guestBookList.Add(st);
                }
                Tools.Sort(guestBookList, new string[] { "subtime:datetime:0" });
            }
            InitItem();
        });


    }

    private void InitItem()
    {
        list_guest.itemRenderer = Guest_Render;
        list_guest.numItems = guestBookList.Count;
        //if (guestBookList.Count > isScroll)
        //{
        //    list_guest.ScrollToView(guestBookList.Count - 1);
        //}
//        Debug.Log("ssssssss"+guestBookList.Count);
        list_guest.ScrollToView(guestBookList.Count - 1);

    }

    private void Guest_Render(int index, GObject item)
    {
        GComponent go = item.asCom;
//		go.RemoveChildren ();
        if (guestBookList.Count != 0)
        {
            Dictionary<string, object> dc = (Dictionary<string, object>)guestBookList[index];
			GComponent itp;
            GComponent it;
            GButton head;
            GTextField text;
			GButton more;
			GImage bg;
            if (!dc["uid"].Equals("more"))
            {
//                go.RemoveChildren();
//				go.GetController ("c1").selectedIndex = 1;
				go.GetChild("n4").visible = true;
				go.GetChild ("n2").visible = false;
                string a = userModel.uid;
                string b;
                if (dc.ContainsKey("fuid"))
                {
                    b = dc["fuid"] + "";
                }
                else
                {
                    b = dc["uid"] + "";
                }
//                it = Tools.GetComponent(Config.ITEM_CHATROLEINFO).asCom;
				itp = go.GetChild("n4").asCom;
				Controller c1 = itp.GetController("c1");
                if (a.Equals(b))
                {
                    c1.selectedIndex = 1;
					it = itp.GetChild("n1").asCom;
                }
                else
                {
                    c1.selectedIndex = 0;
					it = itp.GetChild("n0").asCom;
                }
                head = it.GetChild("n0").asButton;
                text = it.GetChild("n2").asTextField;
                bg = it.GetChild("n5").asImage;

                if (dc.ContainsKey("fhead"))
                {
                    Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl((string)dc["fhead"]));
                    head.RemoveEventListeners();
                    head.onClick.Add(() =>
                    {
                        string uid_ = dc["fuid"] + "";
                        if (!uid_.Equals(roleModel.otherInfo["uid"]))
						{// roleModel.otherInfo["uid"].ToString()
                            this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] {null, uid_, roleModel.tab_Role_CurSelect1, roleModel.tab_Role_CurSelect2, roleModel.tab_Role_CurSelect3 }));
                        }
                        else
                        {
                            ViewManager.inst.ShowText(Tools.GetMessageById("13106"));
                        }
                    });
                }
                if (dc.ContainsKey("msg"))
                {
                    text.text = (string)dc["msg"];
                }
                if (dc.ContainsKey("funame"))
                {
                    it.GetChild("n1").text = (string)dc["funame"];
                }
                else
                {
                    it.GetChild("n1").text = dc["fuid"] + "";
                }
                if (dc.ContainsKey("subtime"))
                {
                    DateTime dt1 = Convert.ToDateTime(ModelManager.inst.gameModel.time.ToString("yyyy - MM - dd"));
                    DateTime dt2 = Convert.ToDateTime(((DateTime)dc["subtime"]).ToString("yyyy-MM-dd"));
                    TimeSpan span = dt1.Subtract(dt2);
                    if (span.Days < 1)
                        it.GetChild("n4").text = ((DateTime)dc["subtime"]).ToString("HH:mm");
                    else
                        it.GetChild("n4").text = ((DateTime)dc["subtime"]).ToString("yyyy-MM-dd HH:mm");
                }
                bg.height = text.height + 40;
                it.height = text.height + it.GetChild("n4").height + it.GetChild("n1").height+25;
                it.GetChild("n4").y = text.y + text.height+25;
                go.height = it.height+25;
//                go.AddChild(it);
            }
            else
            {
//				go.GetController ("c1").selectedIndex = 0;
				go.GetChild("n4").visible = false;
				go.GetChild ("n2").visible = true;
				more = go.GetChild("n2").asButton;
                more.text = Tools.GetMessageById("13065");
                more.onClick.Add(() =>
                {
					Dictionary<string, object> dc_more = new Dictionary<string, object>();
                    dc_more["id"] = tag_g;
                    dc_more["uid"] = otherInfo["uid"];
                    NetHttp.inst.Send(NetBase.HTTP_GETMSG, dc_more, (VoHttp vo) =>
                    {
                        Dictionary<string, object> temp = (Dictionary<string, object>)(vo.data);
                        object[] msgList = (object[])temp["msg_list"];
                        if (msgList.Length != 0)
                        {
                            tag_g = (int)((Dictionary<string, object>)msgList[msgList.Length - 1])["id"];
                            foreach (object st in (object[])temp["msg_list"])
                            {
                                guestBookList.Add(st);
                            }
                            Tools.Sort(guestBookList, new string[] { "subtime:datetime:0" });
                            list_guest.numItems = guestBookList.Count;
                            list_guest.ScrollToView(guestBookList.Count - 30);
                        }
                        else
                        {
                            ViewManager.inst.ShowText(Tools.GetMessageById("13043"));
                        }
                    });
                });
            }
        }
    }
    private void FindObject()
    {
		list_guest = this.GetChild("n2").asList;
        list_guest.SetVirtual();
		input_content = this.GetChild("n3").asTextInput;
        input_content.promptText = Tools.GetMessageById("13061");
        //input_content.maxLength = (int)DataManager.inst.systemSimple["book_num"];
        input_content.onChanged.Add(() => {
            input_content.text = Tools.GetStringByLength(input_content.text, (int)DataManager.inst.systemSimple["book_num"]);
            input_content.text = Tools.StrReplace(input_content.text);
        });
        send = this.GetChild("n4").asButton;
        send.onClick.Add(Send);
        send.text = Tools.GetMessageById("13064");
    }
    private void Send()
    {
        
        if (input_content.text != "")
        {
            input_content.text=input_content.text.Trim();
            input_content.text = FilterManager.inst.Exec(input_content.text);
            Dictionary<string, object> d = new Dictionary<string, object>();
            d["msg"] = input_content.text;
            if (otherInfo["uid"].ToString().Equals(userModel.uid))
            {
                d["uid"] = userModel.uid;
                SendMessage(d);
            }
            else
            {
                if (!(bool)otherInfo["black_me"]) //是否被拉黑
                {
                    d["uid"] = otherInfo["uid"];
                    SendMessage(d);
                }
                else
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13042"));
                }
            }
        }else{
            ViewManager.inst.ShowText(Tools.GetMessageById("13069"));
        }
        
    }

	private void SendMessage(Dictionary<string, object> d)
    {

        int timer = (int)msgConfig["cd_time"];
        if (guestBookList.Count > 0)
        {
            Dictionary<string, object> dataLate = (Dictionary<string, object>)guestBookList[guestBookList.Count - 1];
            DateTime time = (DateTime)dataLate["subtime"];
            if (!dataLate["uid"].ToString().Equals(userModel.uid))
            {
                AddMessage(d);
            }
            else
            {
                if (Tools.GetSystemMillisecond() - time.Ticks / 10000 > timer * 1000L)
                {
                    AddMessage(d);
                }
                else
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13041"));
                }

            }
        }
        else
        {
            AddMessage(d);
        }
       
    }

	private void AddMessage(Dictionary<string, object> d)
    {
        NetHttp.inst.Send(NetBase.HTTP_ADDMSG, d, (VoHttp vo) =>
        {
            //ViewManager.inst.ShowText(Tools.GetMessageById("13040"));
            Dictionary<string, object> dc_send = new Dictionary<string, object>();
            dc_send["fhead"] = userModel.head["use"];
            dc_send["funame"] = userModel.uname;
            dc_send["subtime"] = ModelManager.inst.gameModel.time;
            dc_send["msg"] = input_content.text;
            dc_send["uid"] = otherInfo["uid"];
            dc_send["fuid"] = userModel.uid;
            guestBookList.Add(dc_send);
            Tools.Sort(guestBookList, new string[] { "subtime:datetime:0" });
            input_content.text = "";
            list_guest.numItems = guestBookList.Count;
            list_guest.ScrollToView(guestBookList.Count - 1);
        });
    }

    public override void Clear()
    {
//		for (int i = 0; i < list_guest.numChildren; i++) {
//			list_guest.GetChildAt (i).asCom.RemoveChildren (0, -1, true);
//		}		
//		list_guest.itemPool.Clear ();
//		list_guest.RemoveChildren (0, -1, true);
//		list_guest.Dispose ();
//		list_guest = null;
//		Debug.LogError ("Clear");
        base.Clear();
    }
}
