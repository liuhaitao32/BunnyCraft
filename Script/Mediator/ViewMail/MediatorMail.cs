using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using FairyGUI;

public class MediatorMail : BaseMediator {
    private GameObject showView;
    //private GButton tab_root_0;
    private GButton tab_root_1;
	private GButton tab_root_2;
    private ModelRole roleModel;
    private List<object> listTemp1 = new List<object>();
    private GImage n1;
    private BaseMediator childView;
    private ModelUser userModel;

    public override void Init()
    {
        Create(Config.SCENE_MAIL);
        AddGlobalListener(MainEvent.RED_UPDATE, ViewUpdata);
        
        Inidata();
        FindObject();
    }

 

    private void ViewUpdata(MainEvent e)
    {
        int count = userModel.Get_NoticeState(ModelUser.RED_GIFTMSG);
        if (count > 0)
            userModel.Add_Notice(tab_root_1, new UnityEngine.Vector2(175, -15), count, false);
    }
    private void Inidata()
    {
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
    }

    private void FindObject()
    {
        GButton btn = this.GetChild("n66").asButton;
        btn.text = Tools.GetMessageById("21015");
        GButton callBack = this.GetChild("n104").asButton;
        callBack.onClick.Add(CallBack);
        //tab_root_0 = this.GetChild("n3").asButton;
        tab_root_1 = this.GetChild("bar1").asButton;
        tab_root_2 = this.GetChild("bar2").asButton;

        n1 =this.GetChild("n5").asImage;
        //userModel.GetUnlcok(Config.UNLOCK_MAIL_REWARD, tab_root_1);
        //tabC2 = this.GetController("c1");
        AddGlobalListener(MainEvent.RED_UPDATE, RedUpdate);
        RedUpdate(null);

        InitTopBar(new string[] {
                Tools.GetMessageById ("21006"),
                Tools.GetMessageById ("21007"),
                Tools.GetMessageById ("21008")
            });
        tabC2.onChanged.Add(On_Root_Change);
        //tabC1.onChangeTip.Add(On_Root_ChangeTips);
  //      GGroup gift = this.GetChild ("n63").asGroup;
		//GGroup help = this.GetChild ("n62").asGroup;





		//ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_MAIL_REWARD, gift);
		//ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_MALL_HELP, help);
		//tabC1.changeObj.Add (gift);
		//tabC1.changeObj.Add (help);
		List<GObject> gs = new List<GObject> ();
        //int[] pos = new int[2]{ 265, 471 };
        //if (gift.visible)
        //	gs.Add (gift);
        //if (help.visible)
        //	gs.Add (help);
        //for (int i = 0; i < gs.Count; i++)
        //	gs [i].x = pos [i];
        Dictionary<string, string> locks = new Dictionary<string, string>();
        locks.Add("bar1", "true");
        locks.Add("bar2", "true");
        //
        //Check_UnlockBar(locks);
        Check_TabLock(locks, null);

        if (roleModel.tab_Select1 != -1 && roleModel.tab_Select1 != 0)
        {
            tabC1.selectedIndex = roleModel.tab_Select1;
            roleModel.tab_Select1 = -1;
        }
        else
        {
            this.On_Root_Change();
        }
    }

    private void CallBack(EventContext context)
    {
        ViewManager.inst.CloseScene();
    }

    private void On_Root_ChangeTips(EventContext data)
	{
		if (Convert.ToInt32 (data.data) == 1) {
			userModel.GetUnlcok (Config.UNLOCK_MAIL_REWARD,null,true);
			
		}
		else if (Convert.ToInt32 (data.data) == 2) {
			userModel.GetUnlcok (Config.UNLOCK_MALL_HELP,null,true);
		}
	}
    private void RedUpdate(MainEvent e)
    {
        int count = userModel.Get_NoticeState(ModelUser.RED_GIFTMSG);
        if (count > 0)
        {
            userModel.Add_Notice(tab_root_1, new UnityEngine.Vector2(175, -15), count, false);
        }
        else
        {
            userModel.Remove_Notice(tab_root_1);
        }

		int kefu = userModel.Get_NoticeState(ModelUser.RED_BUGMSG);
		if (kefu > 0) {
			userModel.Add_Notice (tab_root_2, new UnityEngine.Vector2 (175, -15));
		} else {
			userModel.Remove_Notice(tab_root_2);
		}

    }
    private void On_Root_Change()
    {
        base.OnTabLeftChange();
        roleModel.tab_CurSelect1 = tabC2.selectedIndex;
        //string str_1 = "[0]" + Tools.GetMessageById("21006") + "[/0]";
        //string str_2 = "[0]" + Tools.GetMessageById("21007") + "[/0]";
        //string str_3 = "[0]" + Tools.GetMessageById("21008") + "[/0]";
        //GTextField text_1 = tab_root_0.GetChild("title").asTextField;
        //Tools.SetRootTabTitle(text_1, str_1, 30, "ffffff", "#1077BA", new Vector2(0, 0), 3);
        //GTextField text_2 = tab_root_1.GetChild("title").asTextField;
        //Tools.SetRootTabTitle(text_2, str_2, 30, "ffffff", "#1077BA", new Vector2(0, 0), 3);
        //GTextField text_3 = tab_root_2.GetChild("title").asTextField;
        //Tools.SetRootTabTitle(text_3, str_3, 30, "ffffff", "#1077BA", new Vector2(0, 0), 3);
        switch (tabC2.selectedIndex)
        {
            case 0:
                //Tools.SetRootTabTitle(text_1, str_1, 30, "ffffff", "#C16C2B", new Vector2(0, 0), 3);
                AddChildView(new MediatorNotice());
                break;
            case 1:
                //Tools.SetRootTabTitle(text_2, str_2, 30, "ffffff", "#C16C2B", new Vector2(0, 0), 3);
                //				bool isOk = userModel.GetUnlcok(Config.UNLOCK_MAIL_REWARD, tab_root_1);
                //				if (!isOk)
                //                    return;
                AddChildView(new MediatorGift());
                break;
		    case 2:
			    //Tools.SetRootTabTitle (text_3, str_3, 30, "ffffff", "#C16C2B", new Vector2 (0, 0), 3);
			    AddChildView (new MediatorCustom ());
			    //childView.group.x = 35;
			    //childView.group.y = 87;
				break;
        }
    }

    public override void Clear()
    {
        base.Clear();
        this.DispatchGlobalEvent(new MainEvent(MainEvent.RED_UPDATE));
        RemoveGlobalListener(MainEvent.RED_UPDATE, RedUpdate);
        RemoveGlobalListener(MainEvent.GIFT_UPDATE,ViewUpdata);
    }
   
    private void AddChildView(BaseMediator child)
    {
        if (childView != null)
        {
			this.RemoveChild(childView.group, true);
            childView = null;
        }
        childView = child;
		this.AddChild(childView.group);
		childView.group.x = n1.x;
		childView.group.y = n1.y;
    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView(this);
    }
}
