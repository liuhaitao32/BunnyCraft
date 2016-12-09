using UnityEngine;
using System.Collections;
using System;
using FairyGUI;
using System.Collections.Generic;
using System.Linq;

public class MediatorFightDataStatement : BaseMediator
{
    private GList list;
    private GButton share;
    private GButton back;
    private ModelUser userModel;
    private ModelRole roleModel;
    private ModelFight fightModel;
    private ModelShare shareModel;
    private string statementType;
    private object[] result;
    private object[] myData;
    private Dictionary<string, object> myRank;
    private Dictionary<string, object> fight_tag;
    private List<object> data_all;
    private List<object> listData;
    private object[] mydata;
    public List<int> fightDataTypeChange;//变
    private object[] user;
    private string btnBgUp;
    private string btnBgDo;
    private string textAttention;
    private string textAttentiond;
    private string textShield;
    private string textShieldd;
    private string textBtnStrokeColor;
    private string textBtnColor;
    private string fontSize;



    /*-		fightModel.fightData	Count=7	System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
-		["gifts"]	{System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]}	System.Collections.DictionaryEntry
+		    ["el_score"]	400	System.Collections.DictionaryEntry
+		    ["card"]	{System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]}	System.Collections.DictionaryEntry
+		    ["gold"]	12	System.Collections.DictionaryEntry
-		["result"]	System.Object[8]	System.Collections.DictionaryEntry
-		    [0]	System.Object[3]	System.Object
[0]	1267	System.Object
-		[1]	System.Object[6]	System.Object
[0]	1209	System.Object
[1]	7	System.Object
[2]	1	System.Object
[3]	998	System.Object
[4]	21	System.Object
[5]	8	System.Object
-		        [2]	Count=15	System.Object
+		["head"]	{System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]}	System.Collections.DictionaryEntry
+		["guild_id"]	null	System.Collections.DictionaryEntry
+		["uid"]	1267	System.Collections.DictionaryEntry
+		["is_friend"]	false	System.Collections.DictionaryEntry
+		["name"]	null	System.Collections.DictionaryEntry
+		["dis"]	null	System.Collections.DictionaryEntry
+		["is_mvp"]	false	System.Collections.DictionaryEntry
+		["effort_lv"]	1	System.Collections.DictionaryEntry
+		["guild_name"]	null	System.Collections.DictionaryEntry
+		["lv"]	1	System.Collections.DictionaryEntry
+		["state"]	1	System.Collections.DictionaryEntry
+		["rank_score"]	52	System.Collections.DictionaryEntry
+		["sex"]	"f"	System.Collections.DictionaryEntry
+		["rank_lv"]	"1"	System.Collections.DictionaryEntry
+		["is_leader"]	1	System.Collections.DictionaryEntry
+		[1]	System.Object[3]	System.Object
+		[2]	System.Object[3]	System.Object
+		[3]	System.Object[3]	System.Object
+		[4]	System.Object[3]	System.Object
+		[5]	System.Object[3]	System.Object
+		[6]	System.Object[3]	System.Object
+		[7]	System.Object[3]	System.Object
-		["user"]	{System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]}	System.Collections.DictionaryEntry
+		["el_score"]	300	System.Collections.DictionaryEntry
+		["gold"]	10040	System.Collections.DictionaryEntry
+		["lv"]	1	System.Collections.DictionaryEntry
+		["records"]	{System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]}	System.Collections.DictionaryEntry
+		["exp"]	0	System.Collections.DictionaryEntry
+		["coin"]	100	System.Collections.DictionaryEntry
+		["card"]	{System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]}	System.Collections.DictionaryEntry
+		["rank_score"]	81	System.Collections.DictionaryEntry
-		["battle_result"]	1	System.Collections.DictionaryEntry
Key	"battle_result"	System.String
Value	1	System.Int32
-		["my_data"]	System.Object[6]	System.Collections.DictionaryEntry
-		[0]	System.Object[3]	System.Object
[0]	1209	System.Object
[1]	1209	System.Object
[2]	1	System.Object
+		[1]	System.Object[3]	System.Object
+		[2]	System.Object[3]	System.Object
+		[3]	System.Object[3]	System.Object
+		[4]	System.Object[3]	System.Object
+		[5]	System.Object[3]	System.Object
-		["award_id"]	"rank_win"	System.Collections.DictionaryEntry
-		["statementType"]	"match_Team"	System.Collections.DictionaryEntry
*/


    public override void Init()
    {
        base.Init();
        fightDataTypeChange = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            fightDataTypeChange.Add(i);
        }
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        fightModel = ModelManager.inst.fightModel;
        shareModel = ModelManager.inst.shareModel;
        statementType = (string)fightModel.fightData["statementType"];
       
        if (statementType.Equals(ModelFight.FIGHT_MATCHTEAM))
        {
            Create(Config.SCENE_FIGTHTDATA2);
        }
        else
        {
            Create(Config.SCENE_FIGTHTDATA);
        }

        btnBgUp = "n_btn_5";
        btnBgDo = "n_btn_6";
        textAttention = "13013";
        textAttentiond = "13018";
        textBtnColor = "FFFFFF";
        textBtnStrokeColor = "#B266E0";
        textShield = "13016";
        textShieldd = "13032";
        fontSize="";



        FindObject();
        InitData();
       
    }
    public override void Clear()
    {
        base.Clear();
        
      
        
    }
    private void InitData()
    {
        fight_tag = fightModel.GetFightTag();
        result = (object[])fightModel.fightData["result"];
        myData = (object[])fightModel.fightData["my_data"];
        data_all = new List<object>(result);

        if (statementType.Equals(ModelFight.FIGHT_MATCHTEAM) || statementType.Equals(ModelFight.FIGHT_FREEMATCH2))
        {
            for(int i = 0; i < data_all.Count; i++)
            {
                if (data_all[i]!=null) {
                    if (!((Dictionary<string, object>)((object[])data_all[i])[2]).ContainsKey("rankLevel"))
                    {
                        ((Dictionary<string, object>)((object[])data_all[i])[2]).Add("rankLevel", i);
                    }
                }
                
            }
        }
        ModelFight.Sort(data_all, 1, 1);//根据战斗积分排序
        myRank = fightModel.GetFightUser(data_all,userModel.uid);
        user = (object[])myRank["0"];
        if ((int)myRank["1"] < 3)
        {
            if (data_all.Count != 1)
            {
                Debug.Log(data_all.Count);
                listData = data_all.GetRange(0, 3);
                //list.x = 250;
            }
            else
            {
                listData = data_all.GetRange(0, 1);  
                //list.x = 250;
            }

        }
        else
        {
            listData = data_all.GetRange(0, 3);
            listData.Add(user);

        }
        //foreach(object obj in listData)
        //{
        //    if (obj == null) {
        //        listData.Remove(obj);
        //    }
        //}
        for (var i = 0; i < listData.Count; i++) {
            if (listData[i] == null) {
                listData.RemoveAt(i);
                --i;
            }
        }

        list.itemRenderer = OnRenderer;
        list.numItems = listData.Count;
        Log.debug("人数：" + listData.Count);

    }

    private void OnRenderer(int index, GObject item)
    {
        GButton btn = item.asCom.GetChild("n11").asButton;

        //Tools.Btn_Change(btn, "Image:btn_10", "13018");
        btn.touchable = false;
    //}
//            else
//            {
//                btnAtr.selectedIndex = 0;
//                ButtonChange();
//    //Tools.Btn_Change(btn, "Image:btn_16", "13013");
//}
//            if (info.ContainsKey("is_black")) {
//                if ((bool)info["is_black"])
//                {//是否拉黑
//                    //Tools.Btn_Change(btn, "Image:btn_10", "13032");



        object[] data = (object[])listData[index];
        Dictionary<string, object> info = (Dictionary<string, object>)data[2];//userinfo  【1】uid
        object[] fightData = (object[])data[1];//战斗数据
        GTextField lv = item.asCom.GetChild("n3").asCom.GetChild("n2").asTextField;
        GGroup group = item.asCom.GetChild("n3").asCom.GetChild("n3").asGroup;
        GComponent start = item.asCom.GetChild("n8").asCom;
        GButton head = item.asCom.GetChild("n3").asCom.GetChild("n0").asButton;
        GTextField uname = item.asCom.GetChild("n4").asTextField;
        GTextField guild = item.asCom.GetChild("n10").asTextField;
        GTextField core = item.asCom.GetChild("n7").asTextField;
        GTextField core_type = item.asCom.GetChild("n5").asTextField;
        GLoader fight_group = item.asCom.GetChild("n41").asLoader;
        GTextField group_name = item.asCom.GetChild("n42").asTextField;
        GComponent com = item.asCom.GetChild("n35").asCom;
        GComponent  start_= item.asCom.GetChild("n40").asCom;
        GButton  dreamAttention= item.asCom.GetChild("n45").asButton;
        dreamAttention.text = Tools.GetMessageById("13147");
        dreamAttention.onClick.Add(()=> {
            if ((int)data[0] <= 0)
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("24228"));
            }
            else
            {
                Dictionary<string, object> data1 = new Dictionary<string, object>();
                data1["fuid"] = data[0];
                NetHttp.inst.Send(NetBase.HTTP_FOLLOW, data1, (VoHttp vo) =>
                {
                    if ((bool)vo.data == true)
                    {
                        roleModel.AddAttentionFight(data[0].ToString());
                        Dictionary<string, object> dataMy = (Dictionary<string, object>)data[2];
                        dataMy["is_friend"] = true;
                        ViewManager.inst.ShowText(Tools.GetMessageById("13045"));
                        //Tools.SetButtonBgAndColor(btn, btnBgDo, textAttentiond, fontSize, textBtnColor, "", 0);

                        //btn.touchable = false;
                        dreamAttention.visible = false;
                    }
                    else
                    {
                        ViewManager.inst.ShowText(Tools.GetMessageById("13120"));
                    }
                });

            }
        });
        //GGraph icon_start = item.asCom.GetChild("n31").asGraph;

        userModel.GetUnlcok(Config.UNLOCK_GUILD, guild);


        if (info["lv"] != null)
            lv.text = info["lv"].ToString();
        if (info["name"] != null)
            uname.text = info["name"].ToString();
        else
            uname.text = info["uid"].ToString();
        if (info["guild_name"] != null)
        {
            if (info["guild_name"].ToString().Equals(""))
                guild.text = Tools.GetMessageById("19955");
            else
                guild.text = info["guild_name"].ToString();
        }
        else
        {
            guild.text = Tools.GetMessageById("19955");
        }


        object[] type = null;
        List<object> userFightData = new List<object>();
        if (data[0].ToString().Equals(userModel.uid))
        {
            mydata = (object[])fightModel.fightData["my_data"];
            List<object> userData = fightModel.GetUserData(data_all, mydata, statementType);
            for (int i = 0; i < 5; i++)
            {
                Dictionary<string, object> dd = new Dictionary<string, object>();
                dd["data"] = mydata[i];
                dd["rank"] = (int)userData[i];
                dd["index"] = i;
                userFightData.Insert(i, dd);
            }
            Tools.Sort(userFightData, new string[] { "rank:int:0" });
            Dictionary<string,object> my=(Dictionary<string,object>)userFightData[0];
            type = new object[] { my["index"],my["data"]};
        }
        else
        {
            type = fightModel.GetUserMaxData(index, data_all, info["uid"].ToString(), fightDataTypeChange);
        }


        if (statementType.Equals(ModelFight.FIGHT_MATCH) || statementType.Equals(ModelFight.FIGHT_FREEMATCH1))
        {
            fight_group.visible = false;
            group_name.visible = false;
            start.visible = true;
            start_.visible = false;
            com.visible = false;


            Tools.StartSetValue(start, (index + 1).ToString(), ((int)myRank["1"] + 1).ToString(), info["uid"].ToString());
            //if (index == 0)
            //{
            //    GameObject g = EffectManager.inst.AddPrefab(Config.EFFECT_LIGHT, icon_start);
            //    GameObjectScaler.Scale(g, 0.8f);
            //    g.transform.localScale *= 0.8f;
            //}

        }
        else
        {
            fight_group.visible = true;
            group_name.visible = true;
            start.visible = false;
            start_.visible = true;
            if (statementType.Equals(ModelFight.FIGHT_FREEMATCH2)) {
                com.visible = false;

                if ((int)info["rankLevel"] >= 0 && (int)info["rankLevel"] <= 3)
                {
                    fight_group.url = Tools.GetResourceUrl("Image2:n_icon_dui2");
                    Tools.SetRootTabTitleStrokeColor(group_name, "#424F9E", 2);
                    group_name.text = Tools.GetMessageById("24229");
                }
                else
                {
                    fight_group.url = Tools.GetResourceUrl("Image2:n_icon_dui1");
                    Tools.SetRootTabTitleStrokeColor(group_name, "#9A42AA", 2);
                    group_name.text = Tools.GetMessageById("24230");

                }
            }
            else
            {
                com.visible = true;
                GLoader rankIcon = com.GetChild("n1").asLoader;
                GTextField rankScore = com.GetChild("n2").asTextField;
                rankIcon.url = userModel.GetRankImg((int)info["rank_score"]);
                rankScore.text = info["rank_score"].ToString();

                Dictionary<string,object> userInfo=(Dictionary<string,object>)user[2];

                if ((int)userInfo["rankLevel"] <= 4)
                {
                    if ((int)info["rankLevel"] >= 0 && (int)info["rankLevel"] <= 3)
                    {
                        fight_group.url = Tools.GetResourceUrl("Image2:n_icon_dui2");
                        Tools.SetRootTabTitleStrokeColor(group_name, "#424F9E", 2);
                        group_name.text = Tools.GetMessageById("24229");
                    }
                    else
                    {
                        fight_group.url = Tools.GetResourceUrl("Image2:n_icon_dui1");
                        Tools.SetRootTabTitleStrokeColor(group_name, "#9A42AA", 2);
                        group_name.text = Tools.GetMessageById("24230");
                    }
                }
                else
                {
                    if ((int)info["rankLevel"] >= 0 && (int)info["rankLevel"] <= 3)
                    {
                        fight_group.url = Tools.GetResourceUrl("Image2:n_icon_dui1");
                        Tools.SetRootTabTitleStrokeColor(group_name, "#9A42AA", 2);
                        group_name.text = Tools.GetMessageById("24230");
                    }
                    else
                    {
                        fight_group.url = Tools.GetResourceUrl("Image2:n_icon_dui2");
                        Tools.SetRootTabTitleStrokeColor(group_name, "#424F9E", 2);
                        group_name.text = Tools.GetMessageById("24229");
                    }
                }
                
            }
            
            if ((bool)info["is_mvp"])
            {
                //Tools.StartSetValue(start_, Tools.GetMessageById("24223"));
                Tools.GetResourceUrlForMVP(start_.GetChild("n1").asLoader, "mvp");
            }
            else
            {
                start_.visible = false;
            }
        }
        

        if (index == 0)
        {
            core_type.text = fight_tag["0"].ToString();
            core.text = fightData[0].ToString();
            fightDataTypeChange.Remove(0);
        }
        else
        {
            if (!fight_tag[type[0].ToString()].ToString().Equals(fight_tag["5"].ToString()))
            {
                core_type.text = fight_tag[type[0].ToString()].ToString();
                core.text = fightData[Convert.ToInt32(type[0])].ToString();

            }
            else
            {
                core_type.text = fight_tag["0"].ToString();
                core.text = fightData[0].ToString();

            }
            fightDataTypeChange.Remove((int)type[0]);

        }
        //平均分、是否是新记录
        GComponent obj= item.asCom.GetChild("n38").asCom;
        GImage new_mark_bg = obj.GetChild("n0").asImage;
        GTextField new_mark = obj.GetChild("n1").asTextField;
        obj.visible = false;
        if (!statementType.Equals(ModelFight.FIGHT_FREEMATCH1) && !statementType.Equals(ModelFight.FIGHT_FREEMATCH2))
        {
            if (data[0].ToString().Equals(userModel.uid))
            {
                if (type.Length != 0)
                {
                    object[] userType = (object[])myData[Convert.ToUInt32(type[0])];
                    if ((int)userType[2] != 0)
                    {
                        obj.visible = true;
                        new_mark.text = Tools.GetMessageById("24218");
                    }
                }
            }
            group.visible = true;
            if ((int)info["uid"] > 0)
            {
                if (info["head"] != null)
                    Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(((Dictionary<string, object>)info["head"])["use"].ToString()));
            }
            else
            {
                Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(info["name"].ToString(),false,true));
            }
        }
        else
        {
            group.visible = false;
            if ((int)info["uid"] > 0)
            {
                if (info["head"] != null)
                    Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(((Dictionary<string, object>)info["head"])["use"].ToString()));
            }
            else
            {
                Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl("icon_diannao"));
            }

        }
        btn.touchable = true;
        btn.RemoveEventListeners();
        GLoader bg1= item.asCom.GetChild("n2").asLoader;
        bg1.url = Tools.GetResourceUrl("Image2:n_bg_tanban1");
        //GLoader bg = item.asCom.GetChild("n39").asLoader;



        if (data[0].ToString().Equals(userModel.uid))
        {
            Tools.SetButtonBgAndColor(btn, btnBgUp, "24216", "", textBtnColor,textBtnStrokeColor);
            //bg.visible = true;
            btn.visible = true;
            btn.onClick.Add(() => {
				ViewManager.inst.ShowScene<MediatorUserDataStatemnet>();
            });
        }
        else
        {
            Debug.Log(info["uid"] + "is_friend:" + info["is_friend"] + " ..........request_sign" + (bool)info["request_sign"] + ".................is_black" + (bool)info["is_black"]);
            //bg.visible = false;
            if ((bool)info["is_friend"])
            {
                //Tools.SetButtonBgAndColor(btn, btnBgDo, textAttentiond, fontSize, textBtnColor, "",0);
                //btn.touchable = false;

                
                if ((bool)info["request_sign"])
                {
                    dreamAttention.visible = false;
                }
            }
            else
            {
                if (info.ContainsKey("is_black"))
                {
                    if (!(bool)info["is_black"])
                    {
                        if ((bool)info["request_sign"])
                        {
                            dreamAttention.visible = true;
                        }

                    }
                    else
                    {
                        //Tools.SetButtonBgAndColor(btn, btnBgDo, textShieldd, fontSize, textBtnColor, "",0);
                        //btn.touchable = false;
                    }
                }

                
                //Tools.SetButtonBgAndColor(btn, btnBgUp, textAttention, fontSize, textBtnColor, textBtnStrokeColor);
            }
            
            //btn.onClick.Add(() =>
            //{
                
                
            //});
        }
    }

    private void FindObject()
    {
		list = this.GetChild("n1").asList;
		share = this.GetChild("n7").asButton;
        userModel.GetUnlcok(Config.UNLOCK_SHARD, share);
        share.GetChild("title").asTextField.textFormat.size = 30;
        share.text = Tools.GetMessageById("24219");


        if (shareModel.isShareRed())
        {
            userModel.Add_Notice(share, new Vector2(145, -10));
        }
        else
        {
            userModel.Remove_Notice(share);
        }
        share.onClick.Add(() => {
            string share_bitmap="";
            userModel.GetUnlcok(Config.UNLOCK_SHARD, null, true);
            //shareModel.viewX = this.x+this.width/2-232;
            shareModel.viewY = this.y + share.y-200;
            MediatorAd.typeAd = ModelShare.SHARE_FIGHT;
			shareModel.type = ModelShare.SHARE_FIGHT;
//			ViewManager.inst.ShowView<MediatorAd>(false,false);
			ViewManager.inst.ShowView<MediatorShareBtn>(true,false);
        });
		back = this.GetChild("n3").asButton;
        back.text = Tools.GetMessageById("24111");
        back.onClick.Add(() => {
            //DispatchManager.inst.Dispatch(new MainEvent(MainEvent.FIGHT_DATA_END, new object[] { (Dictionary<string, object>)fightModel.fightData["gifts"] }));
            ViewManager.inst.CloseScene();
        });
    }

    private void sdk_callback(object data)
    {
        throw new NotImplementedException();
    }
}
