using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using System;

public class MediatorRoleFight : BaseMediator {
    private Dictionary<string, object> data;
    private Dictionary<string, string> RoleFight;
    private ModelRole roleModel;
    private ModelUser userModel;
    private DateTime timerBegin;
    private int season_last_;
    private long season_last;
    private long rankTimer;
    private GTextField time;
    private GTextField rank_score;
    private GTextField fight_num;
    private GTextField win_rate;
    private GTextField mvp_num;
    private DateTime timerEnd;
    private ModelRank rankModel;
    private int season_protect;
    private int season_settle;
    private GTextField timetext;
    private GTextField fightTime;

    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_ROLEFIGHT);
//        view.SetPosition(256F, 76F, 0);
        
        InitDate();
    }
    public override void Clear()
    {
        base.Clear();
        TimerManager.inst.Remove(Time_Tick);
    }
    private void Time_Tick(float obj)
    {
        Tools.RankTimer(1, fightTime);
    }

    private void InitDate()
    {
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        rankModel = ModelManager.inst.rankModel;
        Tools.setRankData();
        if (roleModel.otherInfo["uid"].ToString().Equals(userModel.uid) && ModelRole.fight1 != null)
        {
            SetData(ModelRole.fight1);
        }
        else
        {
            GetData();
        }
    }

    private void GetData()
    {
        Dictionary<string, object> value = new Dictionary<string, object>();
        if (roleModel.otherInfo["uid"].ToString().Equals(userModel.uid))
        {
            value["uid"] = userModel.uid;
            NetHttp.inst.Send(NetBase.HTTP_GET_FIGHT_SEASON_DATA, value, (VoHttp v) =>
            {
                //+       ["mvp_num"] 2   System.Collections.DictionaryEntry
                //+       ["fight_num"]   17  System.Collections.DictionaryEntry
                //+       ["win_rate"]    1   System.Collections.DictionaryEntry
                //+       ["king_score"]  0   System.Collections.DictionaryEntry
                //+       ["rank_score"]  3069    System.Collections.DictionaryEntry

                SetData((Dictionary<string, object>)v.data);
            });
        }
        else
        {
            value["uid"] = roleModel.otherInfo["uid"];
            NetHttp.inst.Send(NetBase.HTTP_GET_FIGHT_SEASON_DATA, value, (VoHttp v) =>
            {
                SetData((Dictionary<string, object>)v.data);
            });
        }
    }

    private void SetData(Dictionary<string, object> value)
    {
        data = value;
        ModelRole.fight1 = data;
        InitView();
    }

    private void InitView()
    {
        fightTime=GetChild("n5").asTextField;
       GComponent rank=GetChild("n4").asCom;
        GComponent fightNum=GetChild("n6").asCom;
        GComponent winRate=GetChild("n0").asCom;
        GComponent mvp=GetChild("n7").asCom;
        GComponent kill=GetChild("n93").asCom;
        GTextField cupNum=GetChild("n8").asTextField;
        GTextField cupNum_=GetChild("n96").asTextField;
        GLoader loader=GetChild("n85").asLoader;
        GImage bg=GetChild("n95").asImage;
        loader.url= userModel.GetRankImg(userModel.rank_score);
        //timetext=fightTime.GetChild("n0").asTextField;
        //time = fightTime.GetChild("n1").asTextField;

        //rank.GetChild("n0").asTextField.text = Tools.GetMessageById("13022");
        //rank.GetChild("n1").asTextField.text = data["rank_score"].ToString();
        string rankText1 = "[size=15]" + Tools.GetMessageById("13022") + "[/size]";
        string rankText2 = "[size=22]" + data["rank_score"].ToString() + "[/size]";
        Debug.Log(rankText1 + rankText2);

        rank.GetChild("n0").asTextField.text = rankText1;
        rank.GetChild("n1").asTextField.text = rankText2;
        //rank.text = rankText1 + rankText2;


        fightNum.GetChild("n0").asTextField.text = Tools.GetMessageById("13023");
        fightNum.GetChild("n1").asTextField.text = data["fight_num"].ToString();


        double num = Convert.ToDouble(data["win_rate"]) * 100;

        winRate.GetChild("n0").asTextField.text = Tools.GetMessageById("13024");
        winRate.GetChild("n1").asTextField.text = (int)num + "%";


        mvp.GetChild("n0").asTextField.text = Tools.GetMessageById("13025");
        mvp.GetChild("n1").asTextField.text = data["mvp_num"].ToString();

        kill.GetChild("n0").asTextField.text = Tools.GetMessageById("13158");
        kill.GetChild("n1").asTextField.text = data["kill_num"].ToString();

        //GComponent start = GetChild("n9").asCom;
        //GTextField text = start.GetChild("n4").asTextField;
        //text.text= Tools.GetMessageById("24223");

        //GLoader mvp_ = start.GetChild("n1").asLoader;
        //Tools.GetResourceUrlForMVP(mvp_, "mvp");
        //Tools.SetTextFieldStrokeAndShadow(text, "#46149f", new Vector2(0, 0));
        //GImage cupIcon = GetChild("n88").asImage;
        if ((int)data["king_score"] == 0)
        {
            bg.visible = false;
            cupNum.visible = false;
            cupNum_.visible = false;

        }
        else
        {
            bg.visible = true;
            cupNum.visible = true;
            cupNum_.visible = true;
        }
        //bg.visible = true;
        //cupNum.visible = true;
        //cupNum_.visible = true;
        //cupNum.GetChild("n0").asTextField.text = Tools.GetMessageById("13026");
        //cupNum.GetChild("n1").asTextField.text = data["king_score"].ToString();

        //string cupNum1 = "[size=16]" + Tools.GetMessageById("13026") + "[/size]";
        //string cupNum2 = "[size=30]" + data["king_score"].ToString() + "[/size]";
        //        Debug.Log(cupNum1 + cupNum2);
        cupNum.text = Tools.GetMessageById("13026");
        cupNum_.text = data["king_score"].ToString();


        //this.GetChild("n3").asLoader.url = Tools.GetResourceUrl("Image2:n_icon_luobo_");
        Time_Tick(0);
        TimerManager.inst.Add(1f, 0, Time_Tick);
    }
}
