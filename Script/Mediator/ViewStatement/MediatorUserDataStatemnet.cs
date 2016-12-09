using UnityEngine;
using System.Collections;
using System;
using FairyGUI;
using System.Collections.Generic;
using FairyGUI.Utils;

public class MediatorUserDataStatemnet : BaseMediator
{
    private object[] mydata;
    private GTextField title;
    private GList list;
    private Dictionary<string, object> fight_tag;
    private string statementType;
    private ModelRole roleModel;
    private object[] result;
    private Dictionary<string, object> figthUser;
    private List<object> userFightData;
    private GButton back;
    private Dictionary<string, object> fightIcon;
    private ModelFight fightModel;
    private ModelUser userModel;

    public override void Init()
    {
        base.Init();
        Create(Config.SCENE_FIGTHTUSERDATA);
        FindObject();
        InitData();
    }
    public override void Clear()
    {
        base.Clear();
    }
    private void FindObject()
    {
        this.GetChild("n1").asTextField.text = Tools.GetMessageById("24216");
        list = this.GetChild("n2").asList;
        back = this.GetChild("n30").asButton;
        //back.text = Tools.GetMessageById("24111");
        back.onClick.Add(Back);
    }
    private void InitData()
    {
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        fightModel = ModelManager.inst.fightModel;
        fight_tag = fightModel.GetFightTag();
        statementType = (string)fightModel.fightData["statementType"];
        mydata = (object[])fightModel.fightData["my_data"];
        result = (object[])fightModel.fightData["result"];
        fightIcon=(Dictionary<string,object>)DataManager.inst.match["fight_level_win"];

        List<object> data = new List<object>(result);
        ModelFight.Sort(data, 1, 1);//根据战斗积分排序
        figthUser = fightModel.GetFightUser(data,userModel.uid);
         userFightData = new List<object>();
        List<object> userData=fightModel.GetUserData(data,mydata,statementType);
        for(int i = 0; i < 6; i++)
        {
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd["data"] = mydata[i];
            dd["rank"] = (int)userData[i];
            dd["index"] = i;
            userFightData.Insert(i, dd);
        }
        //Tools.Sort(userFightData, new string[] {"rank:int:0" });
        list.itemRenderer = OnRendere;
        list.numItems = userFightData.Count;
    }

    private void OnRendere(int index, GObject item)
    {
        GLoader icon = item.asCom.GetChild("n2").asLoader;
        GTextField score = item.asCom.GetChild("n10").asTextField;
        GTextField scoreType = item.asCom.GetChild("n4").asTextField;
        GTextField avg = item.asCom.GetChild("n6").asTextField;
        GComponent obj=item.asCom.GetChild("n38").asCom;
        obj.visible = false;
        GTextField newMark = obj.GetChild("n1").asTextField;
        Dictionary<string,object> data1 = (Dictionary<string,object>)userFightData[index];
        if (statementType.Equals(ModelFight.FIGHT_FREEMATCH1) || statementType.Equals(ModelFight.FIGHT_FREEMATCH2))
        {
            int userData = (int)data1["data"];
            int rank = (int)data1["rank"] + 1;
            icon.url = Tools.GetResourceUrl("Image2:" + fightIcon[rank.ToString()]);
            score.text = userData.ToString();
            scoreType.text = fight_tag[data1["index"].ToString()].ToString();
            avg.text = Tools.GetMessageById("24220");
            avg.visible = false;
            obj.visible = false;
            
        }
        else
        {
            object[] userData = (object[])data1["data"];
            int rank = (int)data1["rank"] + 1;
            icon.url = Tools.GetResourceUrl("Image2:" + fightIcon[rank.ToString()]);
            //if (!statementType.Equals(ModelFight.FIGHT_MATCHTEAM) && !statementType.Equals(ModelFight.FIGHT_FREEMATCH2))
            //{
               
            //}
            score.text = userData[0].ToString();
            scoreType.text = fight_tag[data1["index"].ToString()].ToString();
            GTextField avgDate = item.asCom.GetChild("n5").asTextField;
            avgDate.text = userData[1].ToString();
            avg.text = Tools.GetMessageById("24220");
            obj.visible = false;
            if (!statementType.Equals(ModelFight.FIGHT_FREEMATCH1) && !statementType.Equals(ModelFight.FIGHT_FREEMATCH2))
            {
                if (index == 5)
                {
                    if ((int)userData[1] >= (int)userData[0])
                    {
                        string scoreText = "[0]" + userData[0].ToString() + "[/0]";
                        Tools.SetRootTabTitle(score, scoreText, 30, "F5E401", "#A56A30", new Vector2(1, 1), 1);
                    }
                }
                else
                {
                    if ((int)userData[1] <= (int)userData[0])
                    {
                        string scoreText = "[0]" + userData[0].ToString() + "[/0]";
                        Tools.SetRootTabTitle(score, scoreText, 30, "F5E401", "#A56A30", new Vector2(1, 1), 1);
                    }
                    newMark.text = "";
                    if ((int)userData[2] != 0&&(int)userData[0]!=0)
                    {
                        obj.visible = true;
                        newMark.text = Tools.GetMessageById("24218");
                    }
                }
            }
        }
    }



    private void Back(EventContext context)
    {
        //ViewManager.inst.ShowScene<MediatorFightDataStatement>();
        ViewManager.inst.CloseScene();
    }
}
