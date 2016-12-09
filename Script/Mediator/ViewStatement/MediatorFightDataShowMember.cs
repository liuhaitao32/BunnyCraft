using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorFightDataShowMember : BaseMediator {
    private ModelFight fightModel;
    private object[] dataAll;
    private object[] fightDataDetails;
    private object[] myData;
    private object[] deck;
    private ModelRole roleModel;
    private ModelUser userModel;

    public override void Init()
    {
        Create(Config.VIEW_FIGTHTDATASHOWMEMBER/*,false,Tools.GetMessageById("24242")*/);
        fightModel = ModelManager.inst.fightModel;
        if (fightModel.isOpenFromRecord)
        {
            fightDataDetails = (object[])fightModel.fightDataDetails[fightModel.recordIndex];
        }
        else
        {
            
            fightDataDetails = (object[])fightModel.fightDataDetails;
        }

        roleModel = ModelManager.inst.roleModel;
        dataAll =(object[])fightDataDetails[3];
        myData=(object[])dataAll[Convert.ToInt32(fightModel.openIndex)];
		GComponent comPhoto = this.GetChild("n2").asCom;
		GTextField uname=this.GetChild("n3").asTextField;
		GTextField guildName = this.GetChild("n4").asTextField;
		GComponent comScoreType = this.GetChild("n7").asCom;
        GLoader scoreTypeBG= comScoreType.GetChild("n1").asLoader;
        GTextField scoreTypeNum = comScoreType.GetChild("n4").asTextField;
        GGroup group = GetChild("n25").asGroup;

		GList list = this.GetChild("n22").asList;
		GButton dreamAttention = this.GetChild("n23").asButton;
        dreamAttention.text = Tools.GetMessageById("13147");
        dreamAttention.onClick.Add(() => {
            if ((int)myData[0] <= 0)
            {
                ViewManager.inst.ShowText(Tools.GetMessageById("24228"));
            }
            else
            {
                Dictionary<string, object> data1 = new Dictionary<string, object>();
                data1["fuid"] = myData[0];
                NetHttp.inst.Send(NetBase.HTTP_FOLLOW, data1, (VoHttp vo) =>
                {
                    if ((bool)vo.data == true)
                    {
                        ModelRole.attentionFight.Add(myData[0].ToString());

                        //if ((int)fightDataDetails[0] == 1)
                        //{
                        //    myData[10] = true;
                        //}
                        //else
                        //{
                        //    myData[13] = true;
                        //}


                        ViewManager.inst.ShowText(Tools.GetMessageById("13045"));
                        dreamAttention.visible = false;
                    }
                    else
                    {
                        ViewManager.inst.ShowText(Tools.GetMessageById("13120"));
                    }
                });

            }
        });
        object[] fightDta=(object[])myData[5];
        deck=(object[])myData[4];

        userModel = ModelManager.inst.userModel;
        userModel.GetUnlcok(Config.UNLOCK_GUILD, guildName);

        //                [0]	-82	System.Object
        //[1]	"勇敢的龙骑士"	System.Object
        //[2]	9	System.Object
        //[3]	5	System.Object
        //[4] System.Object[8] System.Object
        //[5] System.Object[6] System.Object
        // [6] false	System.Object
        // [7] true	System.Object
        //[8] Count= 1 System.Object
        // [9] null	System.Object
        // [10]    null	System.Object

        list.itemRenderer = OnRender;
        list.numItems = deck.Length;
        if (!Tools.IsNullEmpty(myData[1]))
        {
            uname.text = myData[1].ToString();
        }
        else
        {
            uname.text = myData[0].ToString();
        }
        comPhoto.GetChild("n2").asTextField.text = myData[2].ToString();
        GButton head = comPhoto.GetChild("n0").asButton;
        head.onClick.Add(OpenUser);

        if ((int)fightDataDetails[0] == 1)
        {
            //comScoreType.visible = false;
            group.visible = false;
            if ((int)myData[0] > 0)
            {
                Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(((Dictionary<string, object>)myData[6])["use"].ToString()));
            }
            else
            {
                Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(myData[1].ToString(), false, true));

            }
            if (!Tools.IsNullEmpty(myData[8]))
            {
                guildName.text = myData[8].ToString();
            }
            else
            {
                guildName.text = Tools.GetMessageById("19955");
            }
            scoreTypeNum.text = fightModel.recordIndex.ToString();
            Debug.Log((bool)myData[10]);
            if (!(bool)myData[10])//是否关注
            {

                if (!(bool)myData[11])
                {
                    if ((bool)myData[9])
                    {
                        if (myData[0].ToString().Equals(userModel.uid))
                        {
                            dreamAttention.visible = false;
                        }
                        else
                        {
                            if (ModelRole.attentionFight.Contains(myData[0].ToString()))
                            {
                                dreamAttention.visible = false;
                            }
                            else
                            {
                                dreamAttention.visible = true;
                            }

                            
                        }
                        
                    }
                }
            }
        }
        else
        {
            if ((bool)myData[7])
            {
                //comScoreType.visible = true;
                group.visible = true;

                //scoreTypeBG.url = Tools.GetResourceUrl("Image2:n_icon_paiming0");
                //scoreTypeNum.text = Tools.GetMessageById("24223");
                Tools.GetResourceUrlForMVP(scoreTypeBG, "mvp");
            }
            else
            {
                //comScoreType.visible = false;
                group.visible = false;

            }

            if ((int)myData[0] > 0)
            {
                Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(((Dictionary<string, object>)myData[9])["use"].ToString()));
            }
            else
            {
                Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(myData[1].ToString(), false, true));

            }
            if (!Tools.IsNullEmpty(myData[11]))
            {
                guildName.text = myData[11].ToString();
            }
            else
            {
                guildName.text = Tools.GetMessageById("19955");
            }
            Debug.Log((bool)myData[13]);
            if (!(bool)myData[13])//是否关注
            {
                if (!(bool)myData[14])
                {
                    if ((bool)myData[12])
                    {
                        if (myData[0].ToString().Equals(userModel.uid))
                        {
                            dreamAttention.visible = false;
                        }
                        else
                        {
                            if (ModelRole.attentionFight.Contains(myData[0].ToString()))
                            {
                                dreamAttention.visible = false;
                            }
                            else
                            {
                                dreamAttention.visible = true;
                            }


                        }
                    }
                }
            }

        }


    }

    private void OpenUser(EventContext context)
    {
        int uid=(int)myData[0];
        if (uid <= 0)
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("24240"));
        }
        else
        {
            if ((int)fightDataDetails[0] == 1)
            {
				this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { uid, uid, roleModel.tab_Role_CurSelect1, roleModel.tab_Role_CurSelect2, roleModel.tab_Role_CurSelect3 }));
            }
            else
            {
				this.DispatchGlobalEvent(new MainEvent(MainEvent.SHOW_USER, new object[] { uid, uid, roleModel.tab_Role_CurSelect1, roleModel.tab_Role_CurSelect2, roleModel.tab_Role_CurSelect3 }));
            }
            
        }
        
    }

    private void OnRender(int index, GObject item)
    {
        item.scale=new Vector2(0.7f, 0.7f);
        object[] dc = (object[])deck[index];
        ComCard card = item as ComCard;
       
        CardVo v = DataManager.inst.GetCardVo(dc[0].ToString(),(int)dc[1]);
        card.SetData(v.id, -1, 2,v.lv);
        card.SetText(Tools.GetMessageById(v.name));
        card.onClick.Add(() => {  
            //ViewManager.inst.ShowCardInfo(v.id, 3, v.lv);
            MediatorItemShipInfo2.CID = v.id;
            //MediatorItemShipInfo2.isKu = 3;
            MediatorItemShipInfo2.lv = v.lv;
            ViewManager.inst.ShowView<MediatorItemShipInfo2>();

        });

    }

    public override void Clear()
    {
        base.Clear();
    }
}
