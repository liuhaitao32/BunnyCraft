using UnityEngine;
using System.Collections;
using System;
using FairyGUI;
using System.Collections.Generic;
using DG.Tweening;
using System.Threading;

public class MediatorFightBoxStatement : BaseMediator
{
    private ModelUser userModel;
    private ModelRole roleModel;
    private ModelFight fightModel;

    private GComponent viewFight;
    private GComponent rankScore;
    private GLoader rankImg;
    private GGraph mask;
    private GGraph g1;
    private GGraph g2;
    private GGraph g3;
    private GGraph g4;
    private GGraph g5;
    private GTextField addRankScore;
    private FunQueue initQueue;
    private GTextField rankScoreValue;
    private GTextField openTitle;
    private GTextField tip;
    private GLoader rankScoreBg;
    private GComponent mvp;

    private double rankScoreStart;
    private double rankScoreEnd;
    private double rankScoreDis;
    private string rewardName;
    private string effectType1;
    private string animateType1;
    private string animateDir1;
    private string effectType2;
    private string animateType2;
    private string animateDir2;
    private float timeTotal;
    private float time;
    private double dis;
    private double num;
    private bool isMvp = false;
    public GameObject animateType1_go = null;

    public override void Init()
    {
        base.Init();
        Create(Config.SCENE_FIGTHTBOX);
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        fightModel = ModelManager.inst.fightModel;
        
        initQueue = new FunQueue();
        viewFight = this.GetChild("n0").asCom;
        viewFight.scale = new Vector2(0.8f, 0.8f);
        rankScore = viewFight.GetChild("n2").asCom;
        openTitle = viewFight.GetChild("n3").asTextField;
        rankImg = this.GetChild("n2").asLoader;
        mask = this.GetChild("n1").asGraph;
        g1 = this.GetChild("n5").asGraph;
        g2 = this.GetChild("n3").asGraph;
        g3 = this.GetChild("n4").asGraph;
        g4 = this.GetChild("n7").asGraph;
        g5 = this.GetChild("n10").asGraph;
        g6 = this.GetChild("n37").asGraph;
        AddGlobalListener(MainEvent.EXPLORE_GIFT, OpenView);
        openTitle.text = Tools.GetMessageById("24209");
        rankScoreBg = rankScore.GetChild("n1").asLoader;
        rankScoreValue = rankScore.GetChild("n2").asTextField;
        addRankScore = viewFight.GetChild("n4").asTextField;
        tip = this.GetChild("n8").asTextField;
        mvp = this.GetChild("n9").asCom;
        //mvp.GetChild("n4").asTextField.text = Tools.GetMessageById("24223");
        GLoader mvp_ = mvp.GetChild("n1").asLoader;
        Tools.GetResourceUrlForMVP(mvp_, "mvp");
        mvp.visible = false;

        //effectType1 = Config.EFFECT_LOSE;
        //animateType1 = "mvp";
        //animateDir1 = "los/lose_";
        //animateType1_go = EffectManager.inst.AddEffect(effectType1, animateType1, g1, null, false, 75, animateDir1);
        //GameObject go = Tools.GetPrefab("Effect/" + effectType1);
        //GameObject go1 = go.transform.Find(animateDir1 + "/pPlane11" + "/pPlane12").gameObject;
        //GameObject go2 = go1.transform.Find("text").gameObject;
        //GTextField text = new GTextField();
        //text.x = go2.transform.position.x;
        //text.y = go2.transform.position.y;
        //text.text = "mvp";
        //go2.AddComponent<MeshFilter>();
        //go2.AddComponent<MeshRenderer>();
        //view.AddChild(text);



        //rankScoreBg = rankScore.GetChild("n1").asLoader;
        //rankScoreValue = rankScore.GetChild("n2").asTextField;
        //addRankScore = viewFight.GetChild("n4").asTextField;
        //rankScoreValue.text = "0";
        //rankScoreStart = 150;
        //rankScoreEnd = 50 /*(int)info["rank_score"]*/;
        //num = Math.Abs(rankScoreStart - rankScoreEnd);
        //addRankScore.text = "+" + 100;
        //rankScoreBg.url = userModel.GetRankImg(rankScoreStart);
        //rankImg.url = userModel.GetRankImg(rankScoreStart);
        //rankScoreValue.text = "100";
        //switch (isRankUp(rankScoreEnd))
        //{
        //    case 0:
        //        addRankScore.text = "+" + num;
        //        break;
        //    case 1:
        //        addRankScore.text = "-" + num;
        //        break;
        //}

        //effectType1 = Config.EFFECT_WIN;

        //animateType1 = null;
        //animateDir1 = null;
        //isMvp = true;

        //rewardName = "Bag102/bag102";
        //effectType2 = "Bag102/bag102";
        //animateType2 = "102";
        //animateDir2 = null;

        //initQueue.Init(new List<Action> {
        //    Effect1,
        //    Effect2,
        //    Effect3,
        //    Effect4,
        //    Effect5,
        //    Effect6
        //});
        InitData();

    }

    public override void Clear()
    {
        base.Clear();
        TimerManager.inst.Remove(eff);
        TimerManager.inst.Remove(Time_Tick_1);

    }

    private void InitData()
    {
        rank_reward = (Dictionary<string, object>)DataManager.inst.match["rank_reward_title"];
        match_reward = (Dictionary<string, object>)DataManager.inst.match["match_reward_title"];
        statementType = fightModel.fightData["statementType"].ToString();

        if (statementType.Equals(ModelFight.FIGHT_MATCHGUIDE))
        {
            //新手引导
        }
        else
        {
            if (statementType.Equals(ModelFight.FIGHT_MATCHTEAM) || statementType.Equals(ModelFight.FIGHT_MATCH))
            {
                user = (Dictionary<string, object>)fightModel.fightData["user"];
            }
            object[] result = (object[])fightModel.fightData["result"];
            List<object> data = new List<object>(result);
            ModelFight.Sort(data, 1, 1);
            figthUserInfo = fightModel.GetFightUser(data, userModel.uid);
        }
        Match();
        MatchGuild();
        MatchTeam();
    }

    private void MatchTeam()
    {
        if (statementType.Equals(ModelFight.FIGHT_MATCHTEAM) || statementType.Equals(ModelFight.FIGHT_FREEMATCH2))//排位
        {
            addRankScore.visible = true;
            rankScoreBg.visible = true;
            rankScore.visible = true;
            if (statementType.Equals(ModelFight.FIGHT_MATCHTEAM))
            {
                Dictionary<string, object> info = (Dictionary<string, object>)((object[])figthUserInfo["0"])[2];
                object[] matchReward = (object[])rank_reward[(Convert.ToInt32(info["rank_lv"])).ToString()];
                rankScoreValue.text = userModel.rank_score.ToString();
                rankScoreBg.url = userModel.GetRankImg(userModel.rank_score);
                rankImg.url = userModel.GetRankImg(userModel.rank_score);
                rankScoreStart = userModel.rank_score;
                rankScoreEnd = (int)user["rank_score"];
                num = Math.Abs(rankScoreStart - rankScoreEnd);
                if (rankScoreStart > rankScoreEnd)
                {
                    addRankScore.text = "-" + num;
                }
                else if (rankScoreStart > rankScoreEnd)
                {
                    addRankScore.text = "+" + num;
                }
                else
                {
                    if ((int)fightModel.fightData["battle_result"] == 1)//胜利
                    {
                        addRankScore.text = "+" + num;
                    }
                    else
                    {
                        addRankScore.text = "-" + num;
                    }
                }
                if ((int)fightModel.fightData["battle_result"] == 1)//胜利
                {

                    if ((bool)info["is_mvp"])
                    {
                        effectType1 = Config.EFFECT_WIN;
                        animateType1 = null;
                        animateDir1 = null;

                        isMvp = true;

                        rewardName = Tools.GetFightRewardID(matchReward[2].ToString());
                        effectType2 = rewardName;
                        //animateType2 = matchReward[2].ToString();
                        animateDir2 = null;


                    }
                    else
                    {

                        effectType1 = Config.EFFECT_WIN;
                        animateType1 = "mvp1";
                        animateDir1 = "win_/icon";

                        rewardName = Tools.GetFightRewardID(matchReward[0].ToString());
                        effectType2 = rewardName;
                        //animateType2 = matchReward[0].ToString();
                        animateDir2 = null;
                    }
                }
                else
                {
                    //userModel.rank_score = (int)info["rank_score"];
                    if ((bool)info["is_mvp"])
                    {

                        effectType1 = Config.EFFECT_LOSE;
                        animateType1 = null;
                        animateDir1 = null;
                        isMvp = true;
                        rewardName = Tools.GetFightRewardID(matchReward[2].ToString());
                        effectType2 = rewardName;
                        //animateType2 = matchReward[2].ToString();
                        animateDir2 = null;
                    }
                    else
                    {

                        effectType1 = Config.EFFECT_LOSE;
                        animateType1 = "normal";
                        animateDir1 = "los/lose_";

                        rewardName = Tools.GetFightRewardID(matchReward[1].ToString());
                        effectType2 = rewardName;
                        //animateType2 = matchReward[1].ToString();
                        animateDir2 = null;
                    }

                }
                initQueue.Init(new List<Action> {
                    Effect1,
                    Effect2,
                    Effect3,
                    Effect4,
                    Effect5,
                    Effect6
                });
            }
            else
            {
                viewFight.visible = false;
                Dictionary<string, object> info = (Dictionary<string, object>)((object[])figthUserInfo["0"])[2];
                if ((int)fightModel.fightData["battle_result"] == 1)//胜利
                {

                    if ((bool)info["is_mvp"])
                    {
                        effectType1 = Config.EFFECT_WIN;
                        animateType1 = null;
                        animateDir1 = null;
                        isMvp = true;
                    }
                    else
                    {

                        effectType1 = Config.EFFECT_WIN;
                        animateType1 = "mvp1";
                        animateDir1 = "win_/icon";
                    }
                }
                else
                {
                    //userModel.rank_score = (int)info["rank_score"];
                    if ((bool)info["is_mvp"])
                    {

                        effectType1 = Config.EFFECT_LOSE;
                        animateType1 = null;
                        animateDir1 = null;
                        isMvp = true;
                    }
                    else
                    {

                        effectType1 = Config.EFFECT_LOSE;
                        animateType1 = "normal";
                        animateDir1 = "los/lose_";
                    }

                }
                initQueue.Init(new List<Action> {
                    Effect1,
                    Effect7
                });
            }
        }
    }

    private void MatchGuild()
    {
        if (statementType.Equals(ModelFight.FIGHT_MATCHGUIDE))//新手引导
        {
            addRankScore.visible = false;
            rankScoreBg.visible = false;
            rankScore.visible = false;
            g2.y = g2.y - 40;
			int level_ = fightModel.fightData.ContainsKey ("rank") ? (int)fightModel.fightData ["rank"] : 1;

            //int level_ = 1;//测试
            object[] matchReward_ = (object[])match_reward[level_.ToString()];

            switch (level_)
            {
                case 1:
                    effectType1 = Config.EFFECT_NO1;
                    animateType1 = null;
                    animateDir1 = null;
                    break;
                case 2:
                    effectType1 = Config.EFFECT_NO2;
                    animateType1 = null;
                    animateDir1 = null;
                    break;
                case 3:
                    effectType1 = Config.EFFECT_NO3;
                    animateType1 = null;
                    animateDir1 = null;
                    break;
                default:
                    effectType1 = Config.EFFECT_FINISH;
                    animateType1 = null;
                    animateDir1 = null;
                    break;
            }
            rewardName = Tools.GetFightRewardID(matchReward_[1].ToString());
            effectType2 = rewardName;
            //animateType2 = matchReward_[1].ToString();
            animateDir2 = null;
            initQueue.Init(new List<Action> {
                Effect1,
                Effect2,
                Effect6
            });
        }
    }

    private void Match()
    {
        if (statementType.Equals(ModelFight.FIGHT_MATCH) || statementType.Equals(ModelFight.FIGHT_FREEMATCH1))//乱斗
        {
            addRankScore.visible = false;
            rankScoreBg.visible = false;
            rankScore.visible = false;
            g2.y = g2.y - 40;
            g2.x = g2.x - 15;
            int level = (int)figthUserInfo["1"] + 1;
            object[] matchReward = (object[])match_reward[level.ToString()];

            switch (level)
            {
                case 1:
                    effectType1 = Config.EFFECT_NO1;
                    animateType1 = null;
                    animateDir1 = null;
                    break;
                case 2:
                    effectType1 = Config.EFFECT_NO2;
                    animateType1 = null;
                    animateDir1 = null;
                    break;
                case 3:
                    effectType1 = Config.EFFECT_NO3;
                    animateType1 = null;
                    animateDir1 = null;
                    break;
                default:
                    effectType1 = Config.EFFECT_FINISH;
                    animateType1 = null;
                    animateDir1 = null;
                    break;
            }
            rewardName = Tools.GetFightRewardID(matchReward[1].ToString());
            effectType2 = rewardName;
            //animateType2 = matchReward[1].ToString();
            animateDir2 = null;
            if (statementType.Equals(ModelFight.FIGHT_MATCH))
            {
                initQueue.Init(new List<Action> {
                    Effect1,
                    Effect2,
                    Effect6
                });
            }
            else
            {
                viewFight.visible = false;
                initQueue.Init(new List<Action> {
                    Effect1,
                    Effect7
                });

            }

        }
    }

    private void Effect1()
    {
        
        //上边的动画控制
        if (isMvp)
        {
            animateType1_go = EffectManager.inst.AddEffect(effectType1, animateType1, g1, null, false, 50, animateDir1);
            animateType1_go.GetComponent<AudioSource>().volume = userModel.isSound? 1:0;
            //GameObjectScaler.Scale(animateType1_go, 1.5f);
            //            GameObjectScaler.ScaleParticles (animateType1_go, 1f);
            animateType1_go.transform.localScale *= 1.5f;
            GameObject g = EffectManager.inst.AddPrefab(Config.EFFECT_BACKLIGHT, g5);
            //			GameObjectScaler.Scale (g, 0.3f);
            g.transform.localScale *= 0.3f;
            mvp.visible = true;
        }
        else
        {
            animateType1_go = EffectManager.inst.AddEffect(effectType1, animateType1, g1, null, false, 50, animateDir1);
            animateType1_go.GetComponent<AudioSource>().volume = userModel.isSound? 1:0;
            //            GameObjectScaler.Scale(animateType1_go, 1.5f);
            animateType1_go.transform.localScale *= 1.5f;

            //            GameObjectScaler.ScaleParticles (animateType1_go, 1f);
        }

       
        TimerManager.inst.Add(0.5f, 1, (float t) =>
       {
           kaPao = EffectManager.inst.AddEffect(effectType2,"stand", g2, null, false, 50, animateDir2);
           kaPao.transform.localScale *= 0.5f;
           kaPao.GetComponent<AudioSource>().volume = 0;
           GameObject g3 = EffectManager.inst.AddPrefab(Config.EFFECT_HAPPY, g6);
           g3.transform.localScale *= 0.8f;
           this.initQueue.Next();
       });

       

    }

    

    void animateType1_end(object c)
    {
        animateType1_go.transform.Find(animateDir1).gameObject.GetComponent<Animator>().Stop();
    }
    //面板的缩放
    private void Effect2()
    {
        viewFight.TweenScale(new Vector2(1.2f, 1.2f), 0.3f).OnComplete(() =>
     {
         viewFight.TweenScale(new Vector2(1f, 1f), 0.3f).OnComplete(() =>
         {
             TimerManager.inst.Add(0.2f, 1, (float t) =>
               {
                 this.initQueue.Next();
             });
         });
     });
    }
    //萝卜的缩放和旋转
    private void Effect3()
    {
        


        //rankImg.visible = true;
        //        Debug.LogError("3333333333333333333");
        rankScoreBg.visible = true;
        timeTotal = 1f;
        time = 0.1f;
        dis = Math.Round(num / (timeTotal / time), 3);
        TimerManager.inst.Add(time, 0, Time_Tick_1);
    }

    private void Time_Tick_1(float obj)
    {
        if (rankScoreStart < rankScoreEnd)
        {
            //rankImg.TweenScale (new Vector2 (1.2f, 1.2f), 0.1f).OnComplete (() =>
            //{
            //	rankImg.TweenScale (new Vector2 (1f, 1f), 0.1f).OnComplete (() =>
            //	{

            //	});
            //});
            rankScoreStart += dis;
            rankScoreValue.text = Math.Ceiling(rankScoreStart).ToString();
            if (rankScoreStart >= rankScoreEnd)
            {
                rankScoreValue.text = rankScoreEnd.ToString();

                TimerManager.inst.Remove(Time_Tick_1);
                TimerManager.inst.Add(0.2f, 1, (float t) =>
               {
                   this.initQueue.Next();
               });
            }
        }
        else if (rankScoreStart > rankScoreEnd)
        {
            //rankImg.scale = new Vector2(0.8f, 0.8f);
            //tw_a_b = false;
            //tw_a ();
            rankScoreStart -= dis;
            rankScoreValue.text = Math.Ceiling(rankScoreStart).ToString();
            if (rankScoreStart <= rankScoreEnd)
            {
                rankScoreValue.text = rankScoreEnd.ToString();

                TimerManager.inst.Remove(Time_Tick_1);
                TimerManager.inst.Add(0.2f, 1, (float t) =>
               {
                   tw_a_b = true;
                   this.initQueue.Next();
               });
            }
        }
        else
        {
            rankScoreValue.text = rankScoreEnd.ToString();
            TimerManager.inst.Remove(Time_Tick_1);
            TimerManager.inst.Add(0.2f, 1, (float t) =>
           {
               this.initQueue.Next();
           });
        }
    }

    bool tw_a_b = false;
    int tw_a_n = 0;
    private Dictionary<string, object> figthUserInfo;
    private Dictionary<string, object> user;
    private string statementType;
    private Dictionary<string, object> match_reward;
    private Dictionary<string, object> rank_reward;
    private GGraph g6;
    private GameObject kaPao;
    private Action<float> eff;

    //   void tw_a ()
    //{

    //	TimerManager.inst.Add (0.040f, 0, tw_b);
    //}

    //void tw_b (float f)
    //{
    //	tw_a_n += 2;
    //	if (tw_a_n > 360)
    //	{
    //		tw_a_n = 0;
    //	}
    //	rankImg.rotation += (float)Math.Cos ((double)(tw_a_n)) * 20;
    //       tw_a_n -= 4;
    //       if (tw_a_b)
    //	{
    //		rankImg.rotation = 0;
    //		TimerManager.inst.Remove (tw_b);
    //	}
    //}

    private void Effect4()
    {
        rankScoreBg.visible = true;
        switch (isRankUp(rankScoreEnd))
        {
            case 0:
                rankImg.visible = true;
                
                mask.visible = true;
                rankImg.TweenMoveX(g3.x + 40, 0.5f).OnComplete(() =>
                {

                });
                //rankImg.TweenMoveY(g3.y + 20, 0.5f).OnComplete(() =>
                // {

                // });
                
                rankImg.TweenScale(new Vector2(2.418f, 2.4358f), 0.2f).OnComplete(() =>
             {
                 TimerManager.inst.Add(0.2f, 1, (float t1) =>
                 {
                     
                     Tools.AddColorFilter(rankImg);
                     EffectManager.inst.SetFilterAdjustBrightness(rankImg, 1f, 0f, 1f);
                    
                     TimerManager.inst.Add(0.5f, 1, (float t) =>
                     {
                         GameObject g1 = EffectManager.inst.AddPrefab(Config.EFFECT_HAPPY, g3);
                         //				GameObjectScaler.Scale (g1, 0.5f);
                         g1.transform.localScale *= 0.5f;
                         TimerManager.inst.Add(1f, 1, (float t2) =>
                         {
                             this.initQueue.Next();

                         });
                         

                     });
                 });
             });
                break;
            case 1:
                rankImg.visible = true;
                mask.visible = true;
                rankImg.TweenMoveX(g3.x + 40, 0.5f).OnComplete(() =>
                {

                });
                //rankImg.TweenMoveY(g3.y + 20, 0.5f).OnComplete(() =>
                //{

                //});
                rankImg.TweenScale(new Vector2(2.418f, 2.4358f), 0.2f).OnComplete(() =>
             {

                 TimerManager.inst.Add(0.2f, 1, (float t1) =>
                 {
                    
                     Tools.AddColorFilter(rankImg);
                     EffectManager.inst.SetFilterAdjustBrightness(rankImg, 1f, 0f, 1f);
                     TimerManager.inst.Add(0.5f, 1, (float t) =>
                     {
                         GameObject g = EffectManager.inst.AddPrefab(Config.EFFECT_GET_LOW, g3);
                         g.transform.localScale *= 0.5f;
                         TimerManager.inst.Add(1f, 1, (float t2) =>
                         {
                             this.initQueue.Next();
                         });
                     });
                    
                 });
             });
                break;
            case 2:
                TimerManager.inst.Add(0.2f, 1, (float t) =>
               {
                   this.initQueue.Next();
               });
                break;

        }
    }

    private void Effect5()
    {
        int isRankImgBack = isRankUp(rankScoreEnd);
        switch (isRankImgBack)
        {
            case 0:
                rankImg.scale = new Vector2(1f, 1f);
              
                rankImg.url = userModel.GetRankImg(rankScoreEnd, 0, true);
                rankScoreBg.url = userModel.GetRankImg(rankScoreEnd);

                
                rankImg.TweenScale(new Vector2(1.2f, 1.2f), 0.1f).OnComplete(() =>
             {
                 Tools.RemoveColorFilter(rankImg);
                 Tools.AddColorFilter(rankImg).AdjustBrightness(0);
                 rankImg.TweenScale(new Vector2(0.9f, 0.9f), 0.2f).OnComplete(() =>
             {
                
                 rankImg.TweenScale(new Vector2(1f, 1f), 0.1f).OnComplete(() =>
                 {
                     
                     TimerManager.inst.Add(0.5f, 1, (float t) =>
                     {
                         //GameObject g = EffectManager.inst.AddPrefab(Config.EFFECT_LIGHT, g4);
                         GetChild("n7").asImage.visible = true;
                          eff = EffectManager.inst.RotationLight(this.GetChild("n7").asImage);
                         tip.text = Tools.GetMessageById("24243", new object[] { Tools.getRankGroup((int)rankScoreEnd) });
                         mask.onClick.Add(() =>
                         {
                             GetChild("n7").asImage.visible = false;
                             //Tools.Clear(g);
                             TimerManager.inst.Remove(eff);
                             this.initQueue.Next();
                         });

                     });
                 });

             });
             });
                break;
            case 1:
                rankImg.scale = new Vector2(1F, 1F);
                
                rankImg.url = userModel.GetRankImg(rankScoreEnd, 0, true);
                rankScoreBg.url = userModel.GetRankImg(rankScoreEnd);
                rankImg.TweenScale(new Vector2(1.2f, 1.2f), 0.1f).OnComplete(() =>
             {
                 Tools.RemoveColorFilter(rankImg);
                 Tools.AddColorFilter(rankImg).AdjustBrightness(0);
                 rankImg.TweenScale(new Vector2(0.9f, 0.9f), 0.2f).OnComplete(() =>
             {

                 rankImg.TweenScale(new Vector2(1f, 1f), 0.1f).OnComplete(() =>
                 {
                     TimerManager.inst.Add(0.5f, 1, (float t) =>
                     {
                         tip.text = Tools.GetMessageById("24244", new object[] { Tools.getRankGroup((int)rankScoreEnd) });
                         mask.onClick.Add(() =>
                         {
                             this.initQueue.Next();
                         });

                     });
                 });
                
             });
             });
                
                break;
            case 2:
                TimerManager.inst.Add(0.2f, 1, (float t) =>
               {
                   this.initQueue.Next();
               });
                break;
        }
    }

    //卡包
    private void Effect6()
    {
        rankImg.visible = false;
        mask.visible = false;
        tip.visible = false;
        this.view.InvalidateBatchingState(true);
        //GameObject obj = EffectManager.inst.AddEffect(effectType2, animateType2, g2, null, false, 50, animateDir2);
        //obj.transform.localScale *= 0.5f;
        //obj.GetComponent<AudioSource>().volume = 0;
        //GameObject g1 = EffectManager.inst.AddPrefab(Config.EFFECT_HAPPY, g6);
        //g1.transform.localScale *= 0.8f;
        group.onTouchBegin.Add(Open);

    }

    private void Effect7()
    {
//        Debug.LogError("777777777777777777777");
        this.group.onClick.Add(() =>
       {
           ViewManager.inst.ShowScene<MediatorFightDataStatement>();
       });
    }

    private int isRankUp(double numChangefinsh1)
    {
        double numChangestart1 = userModel.rank_score;
        //double numChangestart1 = 300;
        //numChangefinsh1 = 10;
        //double numChangestart1 = 150;

        int a = 0;
        int b = 0;
        Dictionary<string, object> group = (Dictionary<string, object>)DataManager.inst.match["rank_group_show"];
        foreach (KeyValuePair<string, object> data in group)
        {
            object[] data1 = (object[])data.Value;
            object[] data2 = (object[])data1[0];
            if (numChangestart1 >= (int)data2[0] && numChangestart1 <= (int)data2[1])
            {
                a = Convert.ToInt32(data.Key);
            }
            if (numChangefinsh1 >= (int)data2[0] && numChangefinsh1 <= (int)data2[1])
            {
                b = Convert.ToInt32(data.Key);
            }
        }
        if (a < b)
        {
            return 0;//升
        }
        else if (a > b)
        {
            return 1;//降
        }
        else
        {
            return 2;
        }

    }

    private void Open(EventContext context)
    {
        int el_score = userModel.el_score;
        int gold = userModel.gold;
        fightModel.el_score = userModel.el_score.ToString();
        ModelManager.inst.userModel.UpdateData(fightModel.fightData);
        Tools.FullCard(( (Dictionary<string, object>)fightModel.fightData ), gold,"gifts");
        userModel.el_score = el_score;
        ViewManager.inst.ShowGift((Dictionary<string, object>)fightModel.fightData["gifts"], rewardName);
        MediatorGiftShow.isExplore = true;
    }

    private void OpenView(MainEvent e)
    {
        if (statementType.Equals(ModelFight.FIGHT_MATCHGUIDE))
        {
            ViewManager.inst.ShowScene<MediatorMain>();
            MediatorGiftShow.isExplore = false;
        }
        else
        {
            ViewManager.inst.ShowScene<MediatorFightDataStatement>();
            MediatorGiftShow.isExplore = false;
        }

    }
}
