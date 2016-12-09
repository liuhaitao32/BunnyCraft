using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class MediatorRoleRoot : BaseMediator
{
    private GComponent role_left_tab;
    private GComponent role_left_tab_0;
    private ModelUser userModel;
    private ModelRole roleModel;
    private Dictionary<string, object> otherRole;
    private GButton backAll;
    private GButton back;
    private GButton tab_root_0;
    private GButton tab_root_1;
    private GButton tab_root_2;
    private GButton tab_root_3;
    private List<object> listTemp1 = new List<object>();
    private List<object> listTemp2 = new List<object>();
    //private Controller tabC1;
    //	private Controller tabC2;
    private BaseMediator childView;
    private GImage smilPosisition;
    private GButton btn;
    private string selectColor;
    private string normalColor;
    private ModelFight fightModel;
    private GComponent bigPosition;
    private GImage bgFengGeXin;

    //private ComTabEffect tab;

    public override void Init()
    {
        this.Create(Config.SCENE_ROLEROOT, false, null);
        InitDate();
        FindObject();
    }
   
    private void RedUpdate(MainEvent e)
    {
        if (otherRole["uid"].ToString().Equals(userModel.uid))
        {
            if (tabC2.selectedIndex == 0)
            {
                int count = userModel.Get_NoticeState(ModelUser.RED_MSGBOARD);
                if (count > 0)
                {
                    userModel.Add_Notice(btn, new UnityEngine.Vector2(145, -12), count, false);
                }
                else
                {
                    userModel.Remove_Notice(btn);
                }
            }
            else
            {
                userModel.Remove_Notice(btn);
            }
            //if(tabC2.selectedIndex == 0 && tabC1.selectedIndex == 2)
            //{
            //    userModel.Remove_Notice(btn);
            //}
        }
    }

    private void InitDate()
    {
        selectColor = "9E5600";
        normalColor = "215CB4";
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        fightModel = ModelManager.inst.fightModel;
        otherRole = roleModel.otherInfo;
    }

    private void FindObject()
    {
        //tab = this.GetChild ("n1") as ComTabEffect;
        backAll = this.GetChild("n86").asButton;
        backAll.RemoveEventListeners();
        backAll.onClick.Add(BackAll);
        back = this.GetChild("n85").asButton;
        back.GetChild("title").asTextField.textFormat.size = 30;
        back.text = Tools.GetMessageById("24111");
        back.RemoveEventListeners();
        back.onClick.Add(onCloseHandel);
        smilPosisition = this.GetChild("n93").asImage;//photo
        bigPosition = this.GetChild("bg").asCom;//info
        bgFengGeXin = GetChild("n87").asCom.GetChild("n4").asImage;
        btn = GetChild("bar2").asButton;
        tabC1 = this.GetController("c1");
        Init_LeftTab(new string[] { Tools.GetMessageById("13007"), Tools.GetMessageById("13008"), Tools.GetMessageById("13009"), Tools.GetMessageById("13010") }, "n87");
        
        tabC2.onChanged.Add(OnLeftChangeOne);
        if (roleModel.tab_Role_Select2 != -1 && roleModel.tab_Role_Select2 != 0)
        {
            if (roleModel.uids.Count < 2)
            {
                tabC2.selectedIndex = roleModel.tab_Role_Select2;
                roleModel.tab_Role_Select2 = -1;
            }
            else
            {
                this.OnLeftChangeOne();
            }
        }
        else
        {
            this.OnLeftChangeOne();
        }

        backAll.visible = roleModel.uids.Count > 1 ? true : false;

        if (!otherRole["uid"].ToString().Equals(userModel.uid))
        {
            bgFengGeXin.height = 100;
            Dictionary<string, string> locks = new Dictionary<string, string>();
            locks.Add("bar2", "false");
            locks.Add("bar3", "false");
            //
            //            Check_UnlockBar(locks);
            Check_TabLock(locks, this.GetChild("n87").asCom);
        }
    }

    private void OnRootChange()
    {
        base.OnTabChange();
        if (otherRole["uid"].ToString().Equals(userModel.uid))
        {
            AddGlobalListener(MainEvent.RED_UPDATE, RedUpdate);
            RedUpdate(null);
        }

        if (roleModel.uids.Count < 2)
        {
            roleModel.tab_Role_CurSelect1 = tabC1.selectedIndex;
        }
        switch (tabC1.selectedIndex)
        {

            case 0:
                //			role_left_tab_0.GetChild ("n1").asButton.text = Tools.GetMessageColor (title_1, new string[] { selectColor });
                if (tabC2.selectedIndex == 0)
                {
                    AddChildView(new MediatorRoleIndex(), 1);
                }
                else
                {
                    AddChildView(new MediatorDeck(), 1);
                }
                break;
            case 1:
                //			role_left_tab_0.GetChild ("n2").asButton.text = Tools.GetMessageColor (title_2, new string[] { selectColor });
                if (tabC2.selectedIndex == 0)
                {
                    AddChildView(new MediatorRolePhoto(), 1);
                }
                else
                {
                    AddChildView(new MediatorRoleFight(), 1);
                }
                break;
            case 2:
                //			role_left_tab_0.GetChild ("n3").asButton.text = Tools.GetMessageColor (title_3, new string[] { selectColor });
                if (tabC2.selectedIndex == 0)
                {
                    AddChildView(new MediatorGuestBook(), 1);
                }
                else
                {
                    AddChildView(new MediatorRoleRecord(), 1);
                }
                break;
            case 3:
                //			role_left_tab.GetChild ("n3").asButton.text = Tools.GetMessageColor (title_4, new string[] { selectColor });

                if (tabC2.selectedIndex == 1)
                {
                    AddChildView(new MediatorRoleVoyage(), 1);
                }
                break;


        }
    }
    //个人信息
    private void Open_Root_Tab_0()
    {

        if (tabC1 != null)
        {
            tabC1.RemoveEventListeners();
            tabC1.selectedIndex = 0;
        }
        tabC1.onChanged.Add(OnRootChange);
        //tab.SetCount (4, 3);
        if (roleModel.tab_Role_Select1 != -1 && roleModel.tab_Role_Select1 != 0)
        {
            if (roleModel.uids.Count < 2)
            {
                tabC1.selectedIndex = roleModel.tab_Role_Select1;
                roleModel.tab_Role_Select1 = -1;
            }
            else
            {
                this.OnRootChange();
            }

        }
        else
        {
            this.OnRootChange();
        }

    }
    private void OnLeftChangeOne()
    {
        base.OnTabLeftChange();


        if (roleModel.uids.Count < 2)
        {
            roleModel.tab_Role_CurSelect2 = tabC2.selectedIndex;
        }
        switch (tabC2.selectedIndex)
        {
            case 0:
                //Tools.SetRootTabTitle (text_1, str_1, 32, "ffffff", "#0079B8", new Vector2 (0, 0), 3);
                //			Check_left_tab(new string[]{Tools.GetMessageById ("13001"),Tools.GetMessageById ("13002"),Tools.GetMessageById ("13003")},"n87");
                InitTopBar(new string[] {
                Tools.GetMessageById ("13001"),
                Tools.GetMessageById ("13002"),
                Tools.GetMessageById ("13003"),
                "",
            });
                //			Check_lockTop (new string[]{"bar3"});
                Open_Root_Tab_0();
                //

                break;
            case 1:
                //Tools.SetRootTabTitle (text_2, str_2, 32, "ffffff", "#0079B8", new Vector2 (0, 0), 3);
                //			Check_left_tab(new string[]{Tools.GetMessageById ("13060"),Tools.GetMessageById ("13004"),Tools.GetMessageById ("13005"),Tools.GetMessageById ("13006")},"n88");
                InitTopBar(new string[] {
                Tools.GetMessageById ("13060"),
                Tools.GetMessageById ("13004"),
                Tools.GetMessageById ("13005"),
                Tools.GetMessageById ("13006")
            });
                //			Check_lockTop (null);
                Open_Root_Tab_0();
                break;
            case 2:
                //Tools.SetRootTabTitle (text_3, str_3, 32, "ffffff", "#0079B8", new Vector2 (0, 0), 3);
                //用户信息（自己）
                if (userModel.tel == "")
                {
                    roleModel.isGuest = 1;
                }
                else
                {
                    roleModel.isGuest = 2;
                }
                AddChildView(new MediatorUserInfo(), 2);
                break;
            case 3:
                //Tools.SetRootTabTitle (text_4, str_4, 32, "ffffff", "#0079B8", new Vector2 (0, 0), 3);
                //设置
                AddChildView(new MediatorGameSet(), 2);
                break;
        }

    }

    private void BackAll()
    {
        roleModel.isSave = true;
        roleModel.CloseAllFun();
    }

    private void onCloseHandel()
    {
        roleModel.isSave = true;
        if (roleModel.uids.Count > 1)
        {

            Dictionary<string, object> dd = new Dictionary<string, object>();

            string fuid = ((Dictionary<string, object>)(roleModel.uids[roleModel.uids.Count - 2]))["uid"].ToString();
            dd["fuid"] = fuid;
            NetHttp.inst.Send(NetBase.HTTP_FUSERGET, dd, (VoHttp vo) =>
            {
                if (vo.data != null)
                {
                    Dictionary<string, object> data = (Dictionary<string, object>)(roleModel.uids[roleModel.uids.Count - 1]);
                    roleModel.uids.RemoveAt(roleModel.uids.Count - 1);
                    roleModel.otherInfo = (Dictionary<string, object>)vo.data;
                    roleModel.SetTabSelect(fuid, data);
                    ViewManager.inst.CloseScene();
                }
            });
        }
        else
        {
            roleModel.uids.Clear();
            ViewManager.inst.CloseScene();
        }
    }

    private void AddChildView(BaseMediator child, int type)
    {
        if (childView != null)
        {
            this.RemoveChild(childView.group, true);
            childView = null;
        }
        childView = child;
        this.AddChild(childView.group);
        if (type == 1)
        {
            childView.group.x = smilPosisition.x;
            childView.group.y = smilPosisition.y;
        }
        else if (type == 2)
        {
            childView.group.x = bigPosition.x;
            childView.group.y = bigPosition.y;
        }
        else
        {
        }

    }


    public override void Clear()
    {
        base.Clear();
        //ViewManager.inst.CloseScene();
        if (otherRole["uid"].ToString().Equals(userModel.uid))
        {
            RemoveGlobalListener(MainEvent.RED_UPDATE, RedUpdate);

        }
    }
}
