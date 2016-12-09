using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using FairyGUI;

public class MediatorRoleRecord : BaseMediator {
    private Controller ct1;
    private GList list_record;
    private object[] roleRecord;
    private ModelRole roleModel;
    private object[] roleRecord1;
    private GList list_rank;
    private GButton rank;
    private GButton fight;
    private ModelUser userModel;
    private int type;
    private int match_type;
    private int rank_type;
    private ModelFight fightModel;
    private GImage bgItem;

    //private GList list_rank2;

    /*[0]	System.Object[4]	System.Object
[0]	2	System.Object
[1]	253	System.Object
+		[2]	"8/24/2016 4:30:51 PM"	System.Object
-		[3]	System.Object[8]	System.Object
-		[0]	System.Object[12]	System.Object
[0]	1229	System.Object
[1]	"lbzqwe"	System.Object
[2]	1	System.Object
[3]	1	System.Object
+	[4]	System.Object[8]	System.Object
-	[5]	System.Object[6]	System.Object
[0]	1207	System.Object
[1]	7	System.Object
[2]	3	System.Object
[3]	397	System.Object
[4]	18	System.Object
[5]	9	System.Object
[6]	false	System.Object
[7]	false	System.Object
[8]	36	System.Object
[9]	Count=4	System.Object

+		["1229_704"]	{System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]}	System.Collections.DictionaryEntry
+		["use"]	"h01"	System.Collections.DictionaryEntry
+		["1229_714"]	{System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]}	System.Collections.DictionaryEntry
+		["1229_695"]	{System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]}	System.Collections.DictionaryEntry
+		Raw View	Count=4	System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
[10]	null	System.Object
[11]	null	System.Object
+		[1]	System.Object[12]	System.Object
*/
    public override void Init()
    {
        Create(Config.VIEW_ROLERECORD);
        InitDate();
    }

    public override void Clear()
    {
        base.Clear();
        emptyBg = null;
        ModelRole.attentionFight.Clear();
    }
    private void InitDate()
    {
        match_type = 1;
        rank_type = 2;
		list_rank = this.GetChild("n1").asList;
		bgItem = this.GetChild("n4").asImage;
        list_rank.emptyStr = Tools.GetMessageById("24239");
        emptyBg = bgItem;
        list_rank.onChangeNum.Add(this.CheckListNum);
        list_rank.itemRenderer = Record_Render_fight;
		list_rank.SetVirtual();
        fight = this.GetChild("n2").asButton;
		rank = this.GetChild("n3").asButton;
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        fightModel = ModelManager.inst.fightModel;
		ct1 =this.GetController("c1");
        ct1.onChanged.Add(OnChange);
        if (roleModel.tab_Role_Select3 != -1 && roleModel.tab_Role_Select3 != 0)
        {
            if (roleModel.uids.Count <2)
            {
                ct1.selectedIndex = roleModel.tab_Role_Select3;
                roleModel.tab_Role_Select3 = -1;
            }
            else
            {
                OnChange();
            }
        }
        else
        {
            OnChange();

        }

    }

    private void OnChange()
    {
        if (roleModel.uids.Count <2)
        {
            roleModel.tab_Role_CurSelect3 = ct1.selectedIndex;
        }
        rank.text = Tools.GetMessageById("24227");
        fight.text = Tools.GetMessageById("14017");
        roleRecord = null;
        switch (ct1.selectedIndex)
        {
            case 0:
                Tools.SetRootTabTitle(rank.GetChild("title").asTextField, "", 30, "ffffff", "#E08928", new Vector2(0, 0), 2);
                Tools.SetRootTabTitle(fight.GetChild("title").asTextField, "", 30, "ffffff", "#D36C7F", new Vector2(0, 0), 2);
                type = match_type;
                if (roleModel.otherInfo["uid"].ToString().Equals(userModel.uid) && ModelRole.fight2 != null)
                {
                    SetData(match_type,ModelRole.fight2, list_rank);
                }
                else
                {
                    GetData(match_type, list_rank);

                }
                break;
            case 1:
                Tools.SetRootTabTitle(rank.GetChild("title").asTextField, "", 30, "ffffff", "#D36C7F", new Vector2(0, 0), 2); 
                Tools.SetRootTabTitle(fight.GetChild("title").asTextField, "", 30, "ffffff", "#E08928", new Vector2(0, 0), 2);
                type = rank_type;
                if (roleModel.otherInfo["uid"].ToString().Equals(userModel.uid) && ModelRole.fight3 != null)
                {
                    SetData(rank_type, ModelRole.fight3, list_rank);
                }
                else
                {
                    GetData(rank_type, list_rank);

                }
                break;
        }
    }

    private void GetData(int fight_type, GList list_type)
    {
        Dictionary<string, object> value = new Dictionary<string, object>();
        value["uid"] = roleModel.otherInfo["uid"];//userModel.uid;
        value["type"] = fight_type;
        NetHttp.inst.Send(NetBase.HTTP_GET_FIGHT_LOGS, value, (VoHttp v) =>
        {
            SetData(fight_type, (object[])v.data, list_type);

        });
    }

    private void SetData(int fight_type, object[] data, GList list_type)
    {
        roleRecord = data;
        if (fight_type.Equals(match_type))
        {
            ModelRole.fight2 = roleRecord;
        }
        else
        {
            ModelRole.fight3 = roleRecord;
        }
        fightModel.fightDataDetails = roleRecord;
        SetListCSS(list_rank, roleRecord, 6, true);
    }

    private void Record_Render_fight(int index, GObject item)
    {
        GComponent start = item.asCom.GetChild("n2").asCom;

        GLoader bg = start.GetChild("n1").asLoader;
        start.GetChild("n4").asTextField.text="";
        GTextField rank_lv = start.GetChild("n4").asTextField;
        GTextField killNum = item.asCom.GetChild("n4").asTextField;
        GTextField score = item.asCom.GetChild("n3").asTextField;
        GComponent rankScore = item.asCom.GetChild("n7").asCom;

        bool isVisible = SetListCSS(item, roleRecord, index);
        if (isVisible)
        {
            object[] dc = (object[])roleRecord[index];
            object[] userData = null;
            int type = (int)dc[0];
            foreach (object[] data in (object[])dc[3])
            {

                if (data[0].ToString().Equals(roleModel.otherInfo["uid"].ToString()))
                {
                    userData = data;
                }
            }

            if (type == 1)
            {
                rankScore.visible = false;
                rank_lv.visible = true;
                List<object> data_all = new List<object>((object[])dc[3]);
                ModelFight.Sort(data_all, 1, 2);//根据战斗积分排序
                Dictionary<string, object> myRank = fightModel.GetFightUser(data_all, roleModel.otherInfo["uid"].ToString());
                Tools.StartSetValue(start, ((int)myRank["1"] + 1).ToString());
                score.visible = true;
                score.asTextField.text = Tools.GetMessageById("24226") + ((object[])userData[5])[0].ToString();
            }
            else
            {
                if ((bool)userData[6])
                {
                    if ((bool)userData[7])
                    {
                        //rank_lv.text = Tools.GetMessageById("24223");
                        //bg.url = Tools.GetResourceUrl("Image:icon_starmvp");
                        //Tools.SetTextFieldStrokeAndShadow(rank_lv, "#46149f", new Vector2(0, 0));
                        Tools.GetResourceUrlForMVP(bg, "mvp");
                    }
                    else
                    {
                        Tools.GetResourceUrlForMVP(bg, "win");
                    }

                }
                else
                {
                    if ((bool)userData[7])
                    {
                        //rank_lv.text = Tools.GetMessageById("24223");
                        //bg.url = Tools.GetResourceUrl("Image:icon_starmvp");
                        //Tools.SetTextFieldStrokeAndShadow(rank_lv, "#46149f", new Vector2(0, 0));
                        Tools.GetResourceUrlForMVP(bg, "mvp");
                    }
                    else
                    {
                        Tools.GetResourceUrlForMVP(bg, "lose");
                    }
                }
                score.visible = false;
                rankScore.visible = true;
                rankScore.GetChild("n2").asTextField.text = userData[8].ToString();
                rankScore.GetChild("n1").asLoader.url = userModel.GetRankImg(userData[8]);
            }
            //string kill=Tools.GetMessageColor("[0]" + Tools.GetMessageById("24225")+"[/0]", new string[] { "0D55AA" });
            //string kill_= Tools.GetMessageColor("[0]" + ((object[])userData[5])[1].ToString() + "[/0]", new string[] { "00478a" });
            //killNum.text = kill+kill_;
            killNum.text = Tools.GetMessageById("24225") + ((object[])userData[5])[1].ToString();
            GButton watch_fight = item.asCom.GetChild("n0").asButton;
            watch_fight.text = Tools.GetMessageById("24224");
            watch_fight.visible = true;
            watch_fight.RemoveEventListeners();
            watch_fight.onClick.Add(() =>
            {
                fightModel.recordIndex = index;
                //roleModel.SetTabSelect(ModelRole.ROLEVIEW);
                if (type == 1)
                {
                    ViewManager.inst.ShowView<MediatorFightDataShowMatch>();
                }
                else
                {
                    ViewManager.inst.ShowView<MediatorFightDataShowRank>();

                }
            });
            GTextField date = item.asCom.GetChild("n6").asTextField;/*.text = ((DateTime)dc[3]).ToString("yyyy-MM-dd HH:mm");*/
            DateTime time = (DateTime)dc[2];
            Tools.DataTimeFormat(date, time, 0);
        }


    }
}
