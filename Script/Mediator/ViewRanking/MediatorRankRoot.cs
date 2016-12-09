using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorRankRoot : BaseMediator {
    private Controller c1;
    private ModelUser userModel;
    private ModelRole roleModel;
//    private Dictionary<string, object> otherRole;
    private GButton back;
    private GButton tab_root_0;
    private GButton tab_root_1;
    private GButton tab_root_2;
    private GButton tab_root_3;
    private GComponent rank_left_tab;
//    private List<object> listTemp1=new List<object>();
//    private List<object> listTemp2=new List<object>();
    private GTextField timer;
    private DateTime now;
    private long rankTimer;
    private DateTime timerBegin;
    private long season_last;
//    private Controller tabC1;
//    private Controller tabC2;
	private GObject n0;
    private BaseMediator childView;
    private int season_last_;
//    private string selectColor;
//    private string normalColor;
    private ModelRank rankModel;
//    private ComTabEffect tab;
    private DateTime timerEnd;
    private int season_protect;
    private int season_settle;

    public override void Init()
    {
		this.Create (Config.SCENE_RANKROOT, false, null);
        
        InitDate();
        InitItem();
    }

    private void InitDate()
    {
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        rankModel = ModelManager.inst.rankModel;
        Tools.setRankData();
    }

	private void Time_Tick(float time)
    {
        bool[] type = Tools.RankTimer(2, timer);
        if (type.Length > 0)
        {
            if ((bool)type[0])
            {
                TimerManager.inst.Remove(Time_Tick);
                userModel.isSettle = true;
                onCloseHandel();
            }
        }
        
        //if(timer.text == Tools.GetMessageById("13151"))//结算时 强制退出该界面
        //    onCloseHandel();
    }

    private void InitItem()
	{
//		Dictionary<string,string> unlock = new Dictionary<string, string> ();
//		unlock.Add ("bar1", "false");
//		unlock.Add ("bar0", "false");
//		unlock.Add ("bar3", "false");
//		unlock.Add ("bar0", "false");
		
//		Check_lockLeft(unlock,this.GetChild("tabLeft").asCom);
		//, new string[]{Tools.GetMessageById("32001"),Tools.GetMessageById("32002"),Tools.GetMessageById("32003"),Tools.GetMessageById("32004")}
		//
		//
		back = this.GetChild ("n6").asButton;
		back.GetChild ("title").asTextField.textFormat.size = 30;
		back.text = Tools.GetMessageById ("24111");
        
		back.onClick.Add (onCloseHandel);
		timer = this.GetChild ("n12").asTextField;
        
		BaseMediator.emptyBg =  n0 = this.GetChild ("n11");
        tabC1 = this.GetController("c1");

        //		n0.visible = false;
        //		tab = this.GetChild("n1") as ComTabEffect;
        //		ta	b_root_0 = this.GetChild("bar0").asButton;
        //		tab_root_0.GetChild ("title").asTextField.color = Color.blue;
        //		tab_root_1 = this.GetChild("n3").asButton;
        //		tab_root_2 = this.GetChild("n4").asButton;
        //		tab_root_3 = this.GetChild("n5").asButton;
        //
        //        
        //        tabC2 = rank_left_tab.GetController("c1");


        //        else
        //        {
        //            this.OnLeftChangeOne();
        //        }
        // new string[]{Tools.GetMessageById("19927"),Tools.GetMessageById("19926"),Tools.GetMessageById("32007")}
        Init_LeftTab(new string[]{Tools.GetMessageById("32001"),Tools.GetMessageById("32002"),Tools.GetMessageById("32003"),Tools.GetMessageById("32004")},"tabLeft");
		//
		tabC2.onChanged.Add(OnLeftChangeOne);
		if (roleModel.tab_Select2 != -1 && roleModel.tab_Select2 != 0)
		{
			tabC2.selectedIndex = roleModel.tab_Select2;
			roleModel.tab_Select2 = -1;
		}
		else
		{
			this.OnLeftChangeOne();
		}

//		//
//		tabC1 = this.GetController("c1");
//        tabC1.onChanged.Add(OnRootChange);
////        tab.SetCount(4, 0);
////
//        if (roleModel.tab_Select1 != -1 && roleModel.tab_Select1 != 0)
//        {
//            tabC1.selectedIndex = roleModel.tab_Select1;
//            roleModel.tab_Select1 = -1;
//        }
//        else
//        {
//            this.OnRootChange();
//        }
        TimerManager.inst.Add(1f, 0, Time_Tick);
    }

    private void OnRootChange()
    {
		base.OnTabChange ();
//        tabC2.selectedIndex = 0;
        //        if (tabC1.selectedIndex != 0)
        //        {
        //            tab.SetIndex(tabC1.selectedIndex, false);
        //        }
        //        else
        //        {
        //            tab.SetIndex(tabC1.selectedIndex);
        //        }
        roleModel.tab_CurSelect1 = tabC1.selectedIndex;

        //        string str_1 = "[0]" + Tools.GetMessageById("32001") + "[/0]";
        //        string str_2 = "[0]" + Tools.GetMessageById("32002") + "[/0]";
        //        string str_3 = "[0]" + Tools.GetMessageById("32003") + "[/0]";
        //        string str_4 = "[0]" + Tools.GetMessageById("32004") + "[/0]";
        //        GTextField text_1 = tab_root_0.GetChild("title").asTextField;
        //        GTextField text_2 = tab_root_1.GetChild("title").asTextField;
        //        GTextField text_3 = tab_root_2.GetChild("title").asTextField;
        //        GTextField text_4 = tab_root_3.GetChild("title").asTextField;
        //
        //        Tools.SetRootTabTitle(text_1, str_1, 30, "A7A5A8", "", new Vector2(0, 0), 1);
        //        Tools.SetRootTabTitle(text_2, str_2, 30, "A7A5A8", "", new Vector2(0, 0), 1);
        //        Tools.SetRootTabTitle(text_3, str_3, 30, "A7A5A8", "", new Vector2(0, 0), 1);
        //        Tools.SetRootTabTitle(text_4, str_4, 30, "A7A5A8", "", new Vector2(0, 0), 1);
        //		text_1.color = new Color(

        //        if (tabC2 != null)
        //        {
        //            tabC2.RemoveEventListeners();
        ////            tabC2.selectedIndex = 0;
        //        }
        //        rankModel.type = null;
        //switch (tabC1.selectedIndex)
        //{
        //    case 0:
        //        //                Tools.SetRootTabTitle(text_1, str_1, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
        //        //段位
        //        //                OpenRootTab0();
        //        break;
        //    case 1:
        //        //                Tools.SetRootTabTitle(text_2, str_2, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
        //        //rankModel.type = "like";
        //        //喜欢
        //        //                OpenRootTab1();
        //        break;
        //    case 2:
        //        //                Tools.SetRootTabTitle(text_3, str_3, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
        //        //rankModel.type = "kill";
        //        //击杀
        //        //                OpenRootTab2();

        //        break;
        //    case 3:
        //        //                Tools.SetRootTabTitle(text_4, str_4, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
        //        //rankModel.type = "mvp";
        //        //MVP
        //        //                OpenRootTab3();
        //        break;
        //}
        GetRankData (tabC2.selectedIndex + 1, tabC1.selectedIndex + 1);

    }

//    private void OpenRootTab0()
//    {
//		rank_left_tab = (GComponent)this.GetChild("tabLeft");
//        tabC2 = rank_left_tab.GetController("c1");
//        tabC2.onChanged.Add(OnLeftChangeOne);
//        if (roleModel.tab_Select2 != -1 && roleModel.tab_Select2 != 0)
//        {
//            tabC2.selectedIndex = roleModel.tab_Select2;
//            roleModel.tab_Select2 = -1;
//        }
//        else
//        {
//            this.OnLeftChangeOne();
//        }
//
//    }
//    private void OpenRootTab1()
//    {
//
//		rank_left_tab = (GComponent)this.GetChild("n8");
//        
//        tabC2 = rank_left_tab.GetController("c1");
//        tabC2.onChanged.Add(OnLeftChangeTwo);
//        if (roleModel.tab_Select2 != -1 && roleModel.tab_Select2 != 0)
//        {
//            tabC2.selectedIndex = roleModel.tab_Select2;
//            roleModel.tab_Select2 = -1;
//        }
//        else
//        {
//            this.OnLeftChangeTwo();
//        }
//
//    }
//    private void OpenRootTab2()
//    {
//
//		rank_left_tab = (GComponent)this.GetChild("n9");
//        tabC2 = rank_left_tab.GetController("c1");
//        tabC2.onChanged.Add(OnLeftChangeThree);
//        if (roleModel.tab_Select2 != -1 && roleModel.tab_Select2 != 0)
//        {
//            tabC2.selectedIndex = roleModel.tab_Select2;
//            roleModel.tab_Select2 = -1;
//        }
//        else
//        {
//            this.OnLeftChangeThree();
//        }
//    }
//    private void OpenRootTab3()
//    {
//
//		rank_left_tab = (GComponent)this.GetChild("n10");
//        
//        tabC2 = rank_left_tab.GetController("c1");
//        tabC2.onChanged.Add(OnLeftChangeFour);
//        if (roleModel.tab_Select2 != -1 && roleModel.tab_Select2 != 0)
//        {
//            tabC2.selectedIndex = roleModel.tab_Select2;
//            roleModel.tab_Select2 = -1;
//        }
//        else
//        {
//            this.OnLeftChangeFour();
//        }
//    }
//    
   
   
    
    private void GetRankData(int type,int subType)
    {

        if (subType != 3)
        {
            getHttpData(type, subType);

        }
        else
        {
            switch (type)
            {
                case 1:
                    if (ModelRole.rankTempData != null)
                    {
                        GetTmpData(type, subType);
                    }
                    else
                    {
                        getHttpData(type, subType);
                    }
                    break;
                case 2:
                    if (ModelRole.loveTempData != null)
                    {
                        GetTmpData(type, subType);
                    }
                    else
                    {
                        getHttpData(type, subType);
                    }
                    break;
                case 3:
                    if (ModelRole.killTempData != null)
                    {
                        GetTmpData(type, subType);
                    }
                    else
                    {
                        getHttpData(type, subType);
                    }
                    break;
                case 4:
                    if (ModelRole.mvpTempData != null)
                    {
                        GetTmpData(type, subType);
                    }
                    else
                    {
                        getHttpData(type, subType);
                    }
                    break;

            }
            
        }
    }

    private void getHttpData(int type, int subType)
    {
        roleModel.rankData = null;
        Dictionary<string, object> d = new Dictionary<string, object>();
        d["type"] = type;
        d["subtype"] = subType;

        NetHttp.inst.Send(NetBase.HTTP_RANK_LIKE, d, (VoHttp vo) =>
        {
            roleModel.rankData = new Dictionary<string, object>();
            roleModel.rankData["data"] = (object[])(vo.data);
            roleModel.rankData["type"] = subType;
            if (subType == 3)
            {
                switch (type)
                {
                    case 1:
                        ModelRole.rankTempData = (object[])(vo.data);
                        break;
                    case 2:
                        ModelRole.loveTempData = (object[])(vo.data);
                        break;
                    case 3:
                        ModelRole.killTempData = (object[])(vo.data);
                        break;
                    case 4:
                        ModelRole.mvpTempData = (object[])(vo.data);
                        break;
                }
            }
            if (type == 1)
            {
                AddChildView(new MediatorRank());
            }
            else
            {
                AddChildView(new MediatorRankTwo());
            }

        });
    }

    private void GetTmpData(int type, int subType)
    {
        roleModel.rankData["type"] = subType;
        if (subType == 3)
        {
            switch (type)
            {
                case 1:
                    roleModel.rankData["data"] = ModelRole.rankTempData;
                    break;
                case 2:
                    roleModel.rankData["data"] = ModelRole.loveTempData;
                    break;
                case 3:
                    roleModel.rankData["data"] = ModelRole.killTempData;
                    break;
                case 4:
                    roleModel.rankData["data"] = ModelRole.mvpTempData;
                    break;
            }
        }
        if (type == 1)
        {
            AddChildView(new MediatorRank());
        }
        else
        {
            AddChildView(new MediatorRankTwo());
        }
    }

    //    private void OnLeftChangeTwo()
    //    {
    //        roleModel.tab_CurSelect2 = tabC2.selectedIndex;
    //        string title_1 = "[0]" + Tools.GetMessageById("19927") + "[/0]";
    //        string title_2 = "[0]" + Tools.GetMessageById("19926") + "[/0]";
    //        string title_3 = "[0]" + Tools.GetMessageById("32007") + "[/0]";
    //        rank_left_tab.GetChild("n1").asButton.text = Tools.GetMessageColor(title_1, new string[] { normalColor });
    //        rank_left_tab.GetChild("n2").asButton.text = Tools.GetMessageColor(title_2, new string[] { normalColor });
    //        rank_left_tab.GetChild("n3").asButton.text = Tools.GetMessageColor(title_3, new string[] { normalColor });
    //        switch (tabC2.selectedIndex)
    //        {
    //            case 0:
    //                rank_left_tab.GetChild("n1").asButton.text = Tools.GetMessageColor(title_1, new string[] { selectColor });
    //                GetRankData(2,1);
    //                break;
    //            case 1:
    //                rank_left_tab.GetChild("n2").asButton.text = Tools.GetMessageColor(title_2, new string[] { selectColor });
    //                GetRankData(2, 2);
    //                break;
    //            case 2:
    //                rank_left_tab.GetChild("n3").asButton.text = Tools.GetMessageColor(title_3, new string[] { selectColor });
    //                GetRankData(2, 3);
    //                break;
    //        }
    //    }
    //    private void OnLeftChangeThree()
    //    {
    //        roleModel.tab_CurSelect2 = tabC2.selectedIndex;
    //        string title_1 = "[0]" + Tools.GetMessageById("19927") + "[/0]";
    //        string title_2 = "[0]" + Tools.GetMessageById("19926") + "[/0]";
    //        string title_3 = "[0]" + Tools.GetMessageById("32007") + "[/0]";
    //        rank_left_tab.GetChild("n1").asButton.text = Tools.GetMessageColor(title_1, new string[] { normalColor });
    //        rank_left_tab.GetChild("n2").asButton.text = Tools.GetMessageColor(title_2, new string[] { normalColor });
    //        rank_left_tab.GetChild("n3").asButton.text = Tools.GetMessageColor(title_3, new string[] { normalColor });
    //        switch (tabC2.selectedIndex)
    //        {
    //
    //            case 0:
    //                rank_left_tab.GetChild("n1").asButton.text = Tools.GetMessageColor(title_1, new string[] { selectColor });
    //                GetRankData(3, 1);
    //                break;
    //            case 1:
    //                rank_left_tab.GetChild("n2").asButton.text = Tools.GetMessageColor(title_2, new string[] { selectColor });
    //                GetRankData(3, 2);
    //                break;
    //            case 2:
    //                rank_left_tab.GetChild("n3").asButton.text = Tools.GetMessageColor(title_3, new string[] { selectColor });
    //                GetRankData(3, 3);
    //                break;
    //        }
    //    }
    //    private void OnLeftChangeFour()
    //    {
    //        roleModel.tab_CurSelect2 = tabC2.selectedIndex;
    //        string title_1 = "[0]" + Tools.GetMessageById("19927") + "[/0]";
    //        string title_2 = "[0]" + Tools.GetMessageById("19926") + "[/0]";
    //        string title_3 = "[0]" + Tools.GetMessageById("32007") + "[/0]";
    //        rank_left_tab.GetChild("n1").asButton.text = Tools.GetMessageColor(title_1, new string[] { normalColor });
    //        rank_left_tab.GetChild("n2").asButton.text = Tools.GetMessageColor(title_2, new string[] { normalColor });
    //        rank_left_tab.GetChild("n3").asButton.text = Tools.GetMessageColor(title_3, new string[] { normalColor });
    //        switch (tabC2.selectedIndex)
    //        {
    //
    //            case 0:
    //                rank_left_tab.GetChild("n1").asButton.text = Tools.GetMessageColor(title_1, new string[] { selectColor });
    //                GetRankData(4, 1);
    //                break;
    //            case 1:
    //                rank_left_tab.GetChild("n2").asButton.text = Tools.GetMessageColor(title_2, new string[] { selectColor });
    //                GetRankData(4, 2);
    //                break;
    //            case 2:
    //                rank_left_tab.GetChild("n3").asButton.text = Tools.GetMessageColor(title_3, new string[] { selectColor });
    //                GetRankData(4, 3);
    //                break;
    //        }
    //    }
    private void OnLeftChangeOne()
    {
//		Debug.LogError ("OnLeftChangeOne   "+tabC2.selectedIndex);
		base.OnTabLeftChange ();
        roleModel.tab_CurSelect2 = tabC2.selectedIndex;



//        string title_1 = "[0]" + Tools.GetMessageById("19927") + "[/0]";
//        string title_2 = "[0]" + Tools.GetMessageById("19926") + "[/0]";
//        string title_3 = "[0]" + Tools.GetMessageById("32007") + "[/0]";
        //string title_4 = "[0]" + Tools.GetMessageById("31071") + "[/0]";
//        rank_left_tab.GetChild("n1").asButton.text = Tools.GetMessageColor(title_1, new string[] { normalColor });
//        rank_left_tab.GetChild("n2").asButton.text = Tools.GetMessageColor(title_2, new string[] { normalColor });
//        rank_left_tab.GetChild("n3").asButton.text = Tools.GetMessageColor(title_3, new string[] { normalColor });
        //rank_left_tab.GetChild("n4").asButton.text = Tools.GetMessageColor(title_4, new string[] { normalColor });
//        switch (tabC2.selectedIndex)
//        {
//            case 0:
////                rank_left_tab.GetChild("n1").asButton.text = Tools.GetMessageColor(title_1, new string[] { selectColor });
//                GetRankData(1, 1);
//                break;
//            case 1:
////                rank_left_tab.GetChild("n2").asButton.text = Tools.GetMessageColor(title_2, new string[] { selectColor });
//                GetRankData(1, 2);
//                break;
//            case 2:
////                rank_left_tab.GetChild("n3").asButton.text = Tools.GetMessageColor(title_3, new string[] { selectColor });
//                GetRankData(1, 3);
//                break;
//
//        }
//		tabC2.onChanged.Add(OnLeftChangeOne);
		if (tabC1 != null)
		{
			tabC1.RemoveEventListeners();
			tabC1.selectedIndex = 0;
		}
		rankModel.type = null;
		switch (tabC2.selectedIndex)
		{
		case 0:
			//                Tools.SetRootTabTitle(text_1, str_1, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
			//段位
			//                OpenRootTab0();
			break;
		case 1:
			//                Tools.SetRootTabTitle(text_2, str_2, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
			rankModel.type = "like";
			//喜欢
			//                OpenRootTab1();
			break;
		case 2:
			//                Tools.SetRootTabTitle(text_3, str_3, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
			rankModel.type = "kill";
			//击杀
			//                OpenRootTab2();

			break;
		case 3:
			//                Tools.SetRootTabTitle(text_4, str_4, 32, "ffffff", "#0079B8", new Vector2(0, 0), 3);
			rankModel.type = "mvp";
			//MVP
			//                OpenRootTab3();
			break;
		}
		InitTopBar (new string[]{Tools.GetMessageById("19927"),Tools.GetMessageById("19926"),Tools.GetMessageById("32007")});
		//
		tabC1.onChanged.Add(OnRootChange);
		//        tab.SetCount(4, 0);
		//
		if (roleModel.tab_Select1 != -1 && roleModel.tab_Select1 != 0)
		{
			tabC1.selectedIndex = roleModel.tab_Select1;
			roleModel.tab_Select1 = -1;
		}
		else
		{
			this.OnRootChange();
		}
//		TimerManager.inst.Add(0.01f, 0, Time_Tick);

    }
    private void onCloseHandel()
    {

        ViewManager.inst.CloseScene();

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
		childView.group.x = n0.x;
		childView.group.y = n0.y;
    }

    public override void Clear()
    {
        base.Clear();
		BaseMediator.emptyBg = null;
		TimerManager.inst.Remove(Time_Tick);
        

    }
}
