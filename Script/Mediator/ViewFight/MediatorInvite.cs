using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorInvite:BaseMediator
{
	private ModelFight fightModel;
	private ModelUser userModel;

	private GList list;
	private GButton btn_Invite;
	private Controller c1;
	private GButton check;
    private object[] obj;

    public MediatorInvite ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_INVITE, false);//Tools.GetMessageById ("25032")

		fightModel = ModelManager.inst.fightModel;
		userModel = ModelManager.inst.userModel;

		this.GetChild ("n7").text = Tools.GetMessageById ("25036", new string[]{ "0" });
		check = this.GetChild ("n6").asButton;
		check.onClick.Add (Check_Click);
		//
		btn_Close = this.GetChild ("close").asButton;
		btn_Close.onClick.Add (this.Close);
		BaseMediator.emptyBg = this.GetChild ("n26");
//		this.GetChild ("n1").text = Tools.GetMessageById ("25041");
//		this.GetChild ("n3").text = Tools.GetMessageById ("25042");
//		this.GetChild ("n2").text = Tools.GetMessageById ("25043");
//		this.GetChild ("n4").text = Tools.GetMessageById ("25044");
		this.GetChild ("n10").text = Tools.GetMessageById ("25045");

		btn_Invite = this.GetChild ("n8").asButton;
		btn_Invite.onClick.Add (Invite_Click);
		btn_Invite.text = Tools.GetMessageById ("25063");
//		this.GetChild ("n20").asButton.onClick.Add (BtnCheck_Click);
		list = this.GetChild ("n9").asList;
		list.onChangeNum.Add (this.CheckListNum);
		list.itemRenderer = ItemRender;
		list.SetVirtual ();

        //InitTopBar (new string[]{Tools.GetMessageById ("25041"),Tools.GetMessageById ("25042"),Tools.GetMessageById ("25043"),Tools.GetMessageById ("25044")});

       if(!userModel.GetUnlcok(Config.UNLOCK_GUILD)) {
            InitTopBar(new string[] { Tools.GetMessageById("25041"), Tools.GetMessageById("25042"),"", Tools.GetMessageById("25044") });
        } else {
            InitTopBar(new string[] { Tools.GetMessageById("25041"), Tools.GetMessageById("25042"), Tools.GetMessageById("25043"),Tools.GetMessageById("25044") });
        }
        tabC1 = this.GetController ("c1");
//		tabC2 = c1;
		tabC1.onChanged.Add (C1_Change);
		if (fightModel.inviteIndex != -1)
		{
			tabC1.selectedIndex = fightModel.inviteIndex;
			fightModel.inviteIndex = -1;
		}
		else
		{
			C1_Change ();
		}

		this.AddGlobalListener (MainEvent.SOCKET_CLOSE, (MainEvent e) =>
		{
			this.Close ();
		});

	}

	public override void Clear ()
	{
		base.Clear ();
		BaseMediator.emptyBg = null;
	}

	private void ItemRender (int index, GObject item)
	{
		GComponent g = item.asCom;
        GLoader img = g.GetChild("n14").asCom.GetChild("n1").asLoader;
        GTextField txt = g.GetChild("n14").asCom.GetChild("n2").asTextField;
        GButton head = g.GetChild("n1").asButton;
      //  head.GetChild("n0").asButton.GetChild("n2").asImage.visible = false;//隐藏邀请界面头像圈
        GButton btn = g.GetChild("n6").asButton;
        GButton btn_mask = g.GetChild("n9").asButton;


        bool IsVisible = SetListCSS(item, fightModel.curInvite, index);
        Log.debug("IsVisible"+IsVisible+","+fightModel.curInvite.Length+"ddd"+index);
        if (IsVisible)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)fightModel.curInvite[index];
            g.GetChild("n3").text = ModelUser.GetUname(data["uid"], data["name"]);
            //		g.GetChild ("n4").text = data ["guild_name"] == null ? Tools.GetMessageById ("25034") : data ["guild_name"].ToString ();
            //		g.GetChild ("n13").text = data ["lv"].ToString ();
            //		g.GetChild ("n2").asLoader.url = Tools.GetSexUrl (data ["sex"]);
            //		Controller c1 = g.GetController ("c1");
            //g.GetChild("n0").visible = ((index % 2) != 0);
            if (tabC1.selectedIndex == 0)
            {
                g.GetChild("n7").text = Tools.GetNearDistance(data["dis"]);
            }
            else
            {
                string state = data["state"].ToString();
                if (state == "0")
                    g.GetChild("n7").text = Tools.GetMessageById("25029");
                else if (state == "1")
                {
                    if (fightModel.isTeam(data["uid"].ToString()))
                        g.GetChild("n7").text = Tools.GetMessageById("25052");
                    else
                        g.GetChild("n7").text = Tools.GetMessageById("25030");
                }
                else if (state == "2")
                    g.GetChild("n7").text = Tools.GetMessageById("25051");
            }
           
            img.url = userModel.GetRankImg(data["rank_score"]);
            txt.text = data["rank_score"].ToString();
           
            head.GetChild("n2").text = data["lv"].ToString();
            Tools.SetLoaderButtonUrl(head.GetChild("n0").asButton, ModelUser.GetHeadUrl(Tools.Analysis(data, "head.use").ToString()));
            btn.selected = Convert.ToBoolean(data["check"]);
            btn.RemoveEventListeners();
            btn.onClick.Add(() =>
            {
                btn.selected = !btn.selected;
                data["check"] = btn.selected;
                this.GetChild("n7").text = Tools.GetMessageById("25036", new string[] { fightModel.GetCheck().ToString() });
                bool isok = fightModel.GetInviteSelectAll(tabC1.selectedIndex);
                check.selected = isok;

            });

            btn_mask.RemoveEventListeners();
            btn_mask.onClick.Add(() =>
            {
                btn.selected = !btn.selected;
                data["check"] = btn.selected;
                this.GetChild("n7").text = Tools.GetMessageById("25036", new string[] { fightModel.GetCheck().ToString() });
                bool isok = fightModel.GetInviteSelectAll(tabC1.selectedIndex);
                check.selected = isok;
            });
        }
		
	}

	private void C1_Change ()
	{				
		base.OnTabChange ();
        int count = 25001 + tabC1.selectedIndex;
        list.emptyStr = Tools.GetMessageById(count.ToString());
        //list.numItems = fightModel.GetInviteData (tabC1.selectedIndex).Length;
        SetListCSS(list, fightModel.GetInviteData(tabC1.selectedIndex), 6, false);
		fightModel.SetCheck (false);
		check.selected = false;
		this.GetChild ("n7").text = Tools.GetMessageById ("25036", new string[]{ fightModel.GetCheck ().ToString () });
	}

	private void Check_Click ()
	{
		fightModel.SetCheck (check.selected);
  //      list.numItems = 0;
		//list.numItems = fightModel.curInvite.Length;
        SetListCSS(list, fightModel.curInvite, 6, false);
        this.GetChild ("n7").text = Tools.GetMessageById ("25036", new string[]{ fightModel.GetCheck ().ToString () });
	}

	private void BtnCheck_Click ()
	{
		check.selected = !check.selected;
		Check_Click ();
    }

	private void Invite_Click ()
	{
		string[] count = fightModel.GetCurrentInviteUid (false);
		if (count.Length <= 0)
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("25009"));
			return;
		}
		string[] uid = fightModel.GetCurrentInviteUid (true);
		if (uid.Length == 0)
		{
			this.Close ();
			return;
		}
		if (fightModel.fightType == ModelFight.FIGHT_MATCHTEAM)
		{
			NetSocket.inst.AddListener (NetBase.SOCKET_SENDINVITE, (VoSocket vo) =>
			{
				NetSocket.inst.RemoveListener (NetBase.SOCKET_SENDINVITE);
				Log.debug ("邀请好友 - " + vo.data.ToString ());
//				ViewManager.inst.ShowText (Tools.GetMessageById ("25010"));
				this.Close ();
			});
			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["uids"] = uid;
			NetSocket.inst.Send (NetBase.SOCKET_SENDINVITE, data);
		}
		else
		{
			NetSocket.inst.AddListener (NetBase.SOCKET_FREESENDINVITE, (VoSocket vo) =>
			{
				NetSocket.inst.RemoveListener (NetBase.SOCKET_FREESENDINVITE);
				Log.debug ("邀请好友 - " + vo.data.ToString ());
				ViewManager.inst.ShowText (Tools.GetMessageById ("25010"));
				this.Close ();
			});
			Dictionary<string,object> data = new Dictionary<string, object> ();
			data ["uids"] = uid;
			NetSocket.inst.Send (NetBase.SOCKET_FREESENDINVITE, data);
		}
	}
}