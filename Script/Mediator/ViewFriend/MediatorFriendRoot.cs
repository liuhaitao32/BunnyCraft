using UnityEngine;
using System.Collections;
using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorFriendRoot : BaseMediator
{
	private ModelRole roleModel;
	private ModelUser userModel;
	private ModelGuild guildModel;

	private GButton back;
	//private GButton tab_root_0;
	//private GButton tab_root_1;
	//private GComponent frind_left_tab;
	private GButton frind_left_tab_0;
	private GButton frind_left_tab_1;
	//private GButton frind_left_tab_2;
	//public GButton btn_search;
	//private GImage n0;

	//private GComponent frind_left_tab0;
	//private GComponent frind_left_tab1;

	//private GButton frind_left_tab_1_0;
	//private GButton frind_left_tab_1_1;
	//private GButton frind_left_tab_1_2;

//    private Controller tabC1;
//    private Controller tabC2;
//
//	private List<object> listTemp1 = new List<object> ();
//	private List<object> listTemp2 = new List<object> ();
//	private bool haveGuild = false;
	private BaseMediator childView;
    //private BaseMediator PrevchildView;
//    private GButton tab_root_2;
	private GObject bg4;
	private GObject bg3;
	private GGroup bg2;
    private string selectColor;
    private string normalColor;
    //private GComponent gold;
    //private GComponent coin;
//    private ComTabEffect tab;
    public override void Init ()
	{
		this.Create (Config.SCENE_FRIENDROOT);
		userModel = ModelManager.inst.userModel;
		roleModel = ModelManager.inst.roleModel;
		guildModel = ModelManager.inst.guildModel;
        selectColor = "9E5600";
        normalColor = "215CB4";
        FindObject();
        
    }
    private void RedUpdate(MainEvent e)
    {
        if (tabC2.selectedIndex==0)
        {
            int fans_count = userModel.Get_NoticeState(ModelUser.RED_FANS);
            if (fans_count > 0)
            {

                userModel.Add_Notice(frind_left_tab_1, new UnityEngine.Vector2(150, 0), fans_count, false);
            }
            else
            {
                userModel.Remove_Notice(frind_left_tab_1);
            }

            int follow_count = userModel.Get_NoticeState(ModelUser.RED_FOLLOW);
            if (follow_count > 0)
            {
                userModel.Add_Notice(frind_left_tab_0, new UnityEngine.Vector2(150, 0), follow_count, false);
            }
            else
            {
                userModel.Remove_Notice(frind_left_tab_0);
            }
        }
        else if(tabC2.selectedIndex==1)
        {
            userModel.Remove_Notice(frind_left_tab_1);
            userModel.Remove_Notice(frind_left_tab_0);
        }
    }

    public override void Clear ()
	{
		base.Clear ();
		BaseMediator.emptyBg = null;
        RemoveGlobalListener(MainEvent.RED_UPDATE, RedUpdate);
    }

	private void FindObject ()
	{
//		tab = this.GetChild("n1") as ComTabEffect;
		back = this.GetChild ("n2").asButton;
        back.GetChild("title").asTextField.textFormat.size = 30;
        back.text = Tools.GetMessageById("24111");
        back.onClick.Add (onCloseHandel);
        tabC1 = this.GetController("c1");
        //
        frind_left_tab_0 = this.GetChild ("bar0").asButton;
        frind_left_tab_1 = this.GetChild ("bar1").asButton;
        //		//
        //		tab_root_0 = this.GetChild ("n3").asButton;
        //		tab_root_1 = this.GetChild ("n4").asButton;

        //tab_root_2 = this.GetChild("n7").asButton;
        bg2 = this.GetChild("bg2").asGroup;
		bg3 = this.GetChild("bg3");
		bg4 = this.GetChild("bg4");

        //		bg =this.GetChild("n0");
        //        bg.visible = false;
        //tabC1 = this.GetController("c1");
        //        tabC1.onChanged.Add(On_Root_Change);
        //        tabC1.onChangeTip.Add(Tab_Change_Check);
        //        tabC1.changeObj.Add(tab_root_2);


        //        bool ok=userModel.GetUnlcok(Config.UNLOCK_SHARD, tab_root_2);
        //        if (ok)
        //        {
        //            tab.SetCount(3, 3);
        //        }
        //        else
        //        {
        //            tab.SetCount(2, 3);
        //        }
        //        if (roleModel.tab_Select1 != -1 && roleModel.tab_Select1 != 0)
        //        {
        //            tabC1.selectedIndex = roleModel.tab_Select1;
        //            roleModel.tab_Select1 = -1;
        //        }
        //        else
        //        {
        //            this.On_Root_Change();
        //        }

        Init_LeftTab (new string[]{Tools.GetMessageById("19907"),Tools.GetMessageById("19906")},"tabLeft");
		tabC2.onChanged.Add (On_Left_Change0);
		if (roleModel.tab_Select2 != -1 && roleModel.tab_Select2 != 0)
		{
			tabC2.selectedIndex = roleModel.tab_Select2;
			roleModel.tab_Select2 = -1;
		}
		else
		{
			this.On_Left_Change0 ();
		}

    }

    private void Tab_Change_Check(EventContext context)
    {
        if (Convert.ToInt32(context.data) == 2)
        {
            userModel.GetUnlcok(Config.UNLOCK_SHARD, null,true);
           
        }
    }


    private void onCloseHandel (EventContext context)
	{
        
        roleModel.clearRole();
        ViewManager.inst.CloseScene();

	}

	private void On_Root_Change ()
	{

        base.OnTabChange ();
        AddGlobalListener(MainEvent.RED_UPDATE, RedUpdate);
        RedUpdate(null);
        //        if (tabC1.selectedIndex != 0)
        //        {
        //            tab.SetIndex(tabC1.selectedIndex, false);
        //        }
        //        else
        //        {
        //            tab.SetIndex(tabC1.selectedIndex);
        //        }

        roleModel.tab_CurSelect1 = tabC1.selectedIndex;
//        string str_1 = "[0]" + Tools.GetMessageById("19907") + "[/0]";
//        string str_2 = "[0]" + Tools.GetMessageById("19906") + "[/0]";
//        string str_3 = "[0]" + Tools.GetMessageById("17008") + "[/0]";
//        GTextField text_1 = tab_root_0.GetChild("title").asTextField;
//        Tools.SetRootTabTitle(text_1, str_1, 30, "A7A5A8", "", new Vector2(0, 0), 1);
//        GTextField text_2 = tab_root_1.GetChild("title").asTextField;
//        Tools.SetRootTabTitle(text_2, str_2, 30, "A7A5A8", "", new Vector2(0, 0), 1);
        //GTextField text_3 = tab_root_2.GetChild("title").asTextField;
        //Tools.SetRootTabTitle(text_3, str_3, 30, "A7A5A8", "", new Vector2(0, 0), 1);

        switch (tabC1.selectedIndex)
        {
		case 0:
                //                Tools.SetRootTabTitle(text_1, str_1, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
                //                bg.visible = true;
                //                Tab_0();
				if (tabC2.selectedIndex == 0) {
					this.AddChildView(new MediatorAttention(),1);
                   
				} else if (tabC2.selectedIndex == 1) {
					this.AddChildView(new MediatorNear(), 2);
				} 
                break;
            case 1:
                //                Tools.SetRootTabTitle(text_2, str_2, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
                //                bg.visible = true;
                //                Tab_1();
                if (tabC2.selectedIndex == 0) {
					this.AddChildView(new MediatorFans(),1);
                   
                } else if (tabC2.selectedIndex == 1) {
					this.AddChildView(new MediatorTaste(), 3);
				} 
                break;
            case 2:
                //Tools.SetRootTabTitle(text_3, str_3, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
                //bg.visible = false;
                //this.AddChildView(new MediatorShare(),2);
                if (tabC2.selectedIndex == 0) {
					this.AddChildView(new MediatorBlackList(),1);
                    
                } else if (tabC2.selectedIndex == 1) {
					this.AddChildView(new MediatorSearch(), 4);
				} 
                break;
        }
    }

  

//    private void Tab_0 ()
//	{
//		tabC2 = frind_left_tab0.GetController ("c1");
//		frind_left_tab_0 = frind_left_tab0.GetChild ("n0").asButton;
//		frind_left_tab_1 = frind_left_tab0.GetChild ("n1").asButton;
//		frind_left_tab_2 = frind_left_tab0.GetChild ("n2").asButton;
//		
//        AddGlobalListener(MainEvent.RED_UPDATE, RedUpdate);
//        RedUpdate(null);
//        tabC2.onChanged.Add (On_Left_Change0);
//		if (roleModel.tab_Select2 != -1 && roleModel.tab_Select2 != 0)
//		{
//			tabC2.selectedIndex = roleModel.tab_Select2;
//			roleModel.tab_Select2 = -1;
//		}
//		else
//		{
//			this.On_Left_Change0 ();
//		}
//	}

	

//    private void Tab_1 ()
//	{
//		tabC2 = frind_left_tab1.GetController ("c1");
//		frind_left_tab_1_0 = frind_left_tab1.GetChild ("n0").asButton;
//		frind_left_tab_1_1 = frind_left_tab1.GetChild ("n1").asButton;
//		frind_left_tab_1_2 = frind_left_tab1.GetChild ("n2").asButton;
//       
//		tabC2.onChanged.Add (On_Left_Change1);
//		if (roleModel.tab_Select2 != -1 &&roleModel.tab_Select2 != 0)
//		{
//            tabC2.selectedIndex = roleModel.tab_Select2;
//			roleModel.tab_Select2 = -1;
//		}
//		else
//		{
//            this.On_Left_Change1();
//        }
//	}

    private void On_Left_Change0 ()
	{
		base.OnTabLeftChange ();

        roleModel.tab_CurSelect2 = tabC2.selectedIndex;
//        string title_1 = "[0]" + Tools.GetMessageById("19901") + "[/0]";
//        string title_2 = "[0]" + Tools.GetMessageById("19902") + "[/0]";
//        string title_3 = "[0]" + Tools.GetMessageById("19905") + "[/0]";
//        frind_left_tab_0.text = Tools.GetMessageColor(title_1, new string[] { normalColor });
//        frind_left_tab_1.text = Tools.GetMessageColor(title_2, new string[] { normalColor });
//        frind_left_tab_2.text = Tools.GetMessageColor(title_3, new string[] { normalColor });
        switch (tabC2.selectedIndex)
        {
            case 0:
                //                bg.visible = true;
                //                frind_left_tab_0.text = Tools.GetMessageColor(title_1, new string[] { selectColor });
                //                this.AddChildView(new MediatorAttention(),1);
                InitTopBar(new string[]{Tools.GetMessageById("19901"),Tools.GetMessageById("19902"),Tools.GetMessageById("19905")});

                break;
            case 1:
//                bg.visible = true;
//                frind_left_tab_1.text = Tools.GetMessageColor(title_2, new string[] { selectColor });
//                this.AddChildView(new MediatorFans(),1);
			InitTopBar(new string[]{Tools.GetMessageById("19910"),Tools.GetMessageById("19923"),Tools.GetMessageById("19909")});
                break;
//            case 2:
//                bg.visible = true;
//                frind_left_tab_2.text = Tools.GetMessageColor(title_3, new string[] { selectColor });
//                this.AddChildView(new MediatorBlackList(),1);
//                break;
        }
		if (tabC1 != null)
		{
			tabC1.RemoveEventListeners();
			tabC1.selectedIndex = 0;
		}
		
        tabC1.onChanged.Add(On_Root_Change);
//        tabC1.onChangeTip.Add(Tab_Change_Check);
//        tabC1.changeObj.Add(tab_root_2);


//        bool ok=userModel.GetUnlcok(Config.UNLOCK_SHARD, tab_root_2);
//        if (ok)
//        {
//            tab.SetCount(3, 3);
//        }
//        else
//        {
//            tab.SetCount(2, 3);
//        }
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
//    private void On_Left_Change1 ()
//	{
//		roleModel.tab_CurSelect2 = tabC2.selectedIndex;
//        string title_1 = "[0]" + Tools.GetMessageById("19909") + "[/0]";
//        string title_2 = "[0]" + Tools.GetMessageById("19910") + "[/0]";
//        string title_3 = "[0]" + Tools.GetMessageById("19923") + "[/0]";
//        frind_left_tab_1_0.text = Tools.GetMessageColor(title_2, new string[] { normalColor });
//        frind_left_tab_1_1.text = Tools.GetMessageColor(title_3, new string[] { normalColor });
//        frind_left_tab_1_2.text = Tools.GetMessageColor(title_1, new string[] { normalColor });
//        switch (tabC2.selectedIndex)
//		{
//		case 0:
//                bg.visible = true;
//                frind_left_tab_1_0.text = Tools.GetMessageColor(title_2, new string[] { selectColor });
//                this.AddChildView(new MediatorNear(), 1);
//			break;
//		case 1:
//                bg.visible = false;
//                frind_left_tab_1_1.text = Tools.GetMessageColor(title_3, new string[] { selectColor });
//                this.AddChildView(new MediatorTaste(), 1);
//                break;
//		case 2:
//                bg.visible = false;
//                frind_left_tab_1_2.text = Tools.GetMessageColor(title_1, new string[] { selectColor });
//                this.AddChildView(new MediatorSearch(), 1);
//                break;
//		}
//	}

    private void AddChildView(BaseMediator child,int type)
    {
        if (childView != null)
        {
			this.RemoveChild(childView.group, true);
            childView = null;
        }
        childView = child;
		this.AddChild(childView.group);
		BaseMediator.emptyBg = null;
		bg2.visible = false;
		bg3.visible = false;
		bg4.visible = false;

//        Debug.Log("dddddddddd"+type);
		if (type == 1) {
			BaseMediator.emptyBg = bg2;
			bg2.visible = true;
			childView.group.x = bg2.x;
			childView.group.y = bg2.y;
		} else if (type == 2) {
			BaseMediator.emptyBg = bg3;
			bg3.visible = true;
			childView.group.x = bg3.x;
			childView.group.y = bg3.y;
		} else if (type == 3) {
			BaseMediator.emptyBg = bg4;
			bg4.visible = true;
			childView.group.x = bg.x;
			childView.group.y = bg.y + 55;
		} else {
			childView.group.x = bg.x;
			childView.group.y = bg.y+55;
		}
       
    }
}
