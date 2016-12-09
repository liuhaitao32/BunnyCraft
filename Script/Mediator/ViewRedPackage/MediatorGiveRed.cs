using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using System;

public class MediatorGiveRed : BaseMediator//发红包界面
{
	private Dictionary<string,object> msgData;
	private GTextField noGive;
    private ModelRole roleModel;
    private ModelUser userModel;
    private GList list;
    private List<object> listData;
    //private ComNumeric L_num;
	private Dictionary<string,object> cfg;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_GIVERED,false, Tools.GetMessageById("12018"));
		cfg = ModelManager.inst.userModel.records;
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        //		view.GetChild ("n2").asTextField.text = Tools.GetMessageById ("12015");
        this.GetChild ("n11").asTextField.text = Tools.GetMessageById ("12019");

        NetHttp.inst.Send (NetBase.HTTP_GET_REDBAG_MSG, "", OnGetMsgHandler);
        //		view.GetChild ("n14").asButton.onClick.Add (() => {
        //			ViewManager.inst.CloseView(this);
        //		});
        //GameObject asd = EffectManager.inst.AddEffect (Config.EFFECT_MAILBOX,"stand", this.GetChild ("n1").asGraph);
        //GameObjectScaler.ScaleParticles (asd, 0);
        //EffectManager.inst.StopAnimation (asd);
        //		TimerManager.inst.Add (0.01f, 1, (float obj) => {
        //			
        //		});

        //GTextField title = this.GetChild("n18").asTextField;
        //title.text = Tools.GetMessageById("12018");
        //SetListCSS(list, listItem, 5, true);
	}
	private void OnGetMsgHandler (VoHttp vo)
	{
		msgData = new Dictionary<string, object> ();
		Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
		object[] da = data ["data"] as object[];
		int currNum = 0;
		for (int i = 0; i < da.GetLength (0); i++)
		{
//			if ((((Dictionary<string,object>)da [i]) ["mtype"] + "") == "redbag")
//			{
				msgData [currNum + ""] = da [i];
				currNum++;
//			}
		}
        GObject bg = this.GetChild("n14").asImage;
        listData =Tools.ConvertDicToList(msgData, "aaa");
        list = this.GetChild ("n7").asList;
        list.itemRenderer = List_Render;
        
        list.SetVirtual ();
		list.numItems = currNum;
		noGive = this.GetChild ("n11").asTextField;
		if (currNum == 0) {
			noGive.visible = true;
            bg.visible = false;
		} else {
			list.visible = true;
            bg.visible = true;
		}

        SetListCSS(list, listData.ToArray(), 5, true);

    }

    private void List_Render(int index, GObject go) {
        bool isVisible = SetListCSS(go, listData.ToArray(), index);
        if(isVisible) {
            Dictionary<string, object> d = (Dictionary<string, object>)( listData[index] );
            string name = (string)d["name"];
            string head_use = (string)d["head_use"];
            GTextField lv = go.asCom.GetChild("n4").asCom.GetChild("n2").asTextField;
            GButton photo = go.asCom.GetChild("n4").asCom.GetChild("n0").asButton;
            go.asCom.GetChild("n10").asTextField.visible = false;
            Dictionary<string, object> gift = (Dictionary<string, object>)d["content"];
            Dictionary<string, object> cardCfg = DataManager.inst.card;
            // Debug.LogError("name  "+d["name"].ToString()+" || "+"uid  "+d["uid"].ToString()+" || "+"head_use "+ d["head_use"].ToString());
            GButton head = go.asCom.GetChild("n4").asCom.GetChild("n0").asButton;
            GButton btn_att = go.asCom.GetChild("n5").asButton;
            btn_att.text = Tools.GetMessageById("13147");
            string giftName = "";
            string gifts = "";
            //if(index == 0 || index % 2 == 0) {
            //    go.asCom.GetChild("itemBg").visible = false;
            //} else {
            //    go.asCom.GetChild("itemBg").visible = true;
            //}

            lv.text = d["lv"].ToString();
            Tools.SetLoaderButtonUrl(photo, ModelUser.GetHeadUrl(head_use));
            //contecnt
            foreach(string s in gift.Keys) {
                giftName = s;
                if(giftName == Config.ASSET_CARD) {
                    foreach(string cid in ( (Dictionary<string, object>)( gift[Config.ASSET_CARD] ) ).Keys) {
                        Dictionary<string, object> cardidcfg = (Dictionary<string, object>)cardCfg[cid];
                        if(gifts == "") {
                            gifts += "  [" + giftName + (int)cardidcfg["rarity"] + "2]" + " x " + "[color=#f38917]" + (int)( ( (Dictionary<string, object>)gift[giftName] )[cid] ) + "[/color]";
                        } else {
                            gifts += "  [" + giftName + (int)cardidcfg["rarity"] + "2]" + " x " + "[color=#f38917]" + (int)( ( (Dictionary<string, object>)gift[giftName] )[cid] ) + "[/color]";
                        }
                    }
                } else {
                    if(gifts == "") {
                        gifts = "[" + giftName + "2]" + " x " + "[color=#f38917]" + (int)gift[giftName] + "[/color]  " + gifts;
                    } else {
                        gifts = "[" + giftName + "2]" + " x " + "[color=#f38917]" + (int)gift[giftName] + "[/color]  " + gifts;
                    }
                }
            }

            if(name == null) {
                name = d["uid"] + "";
            }
            string str = Tools.GetMessageById("12002", new string[] {
            "[color=#f38917]" + name + "[/color]  ",
            gifts
        });
            go.asCom.GetChild("n0").asRichTextField.text = BaseUbbParser.inst.Parse(str);

            //AttentionBtn
            //if(!(bool)d["if_black"] && !(bool)d["if_friend"] && !(bool)d["request_sign"] || d["uid"].ToString() == userModel.uid) {
            //    btn_att.visible = false;
            //} else {
            //    btn_att.visible = true;
            //}
            if((bool)d["request_sign"]) {
                if((bool)d["if_black"] || (bool)d["if_friend"]) {
                    btn_att.visible = false;
                } else {
                    btn_att.visible = true;
                }
            } else {
                btn_att.visible = false;
            }

            if( d["uid"].ToString() == userModel.uid ){
                btn_att.visible = false;
            }

            if((bool)d["if_friend"]) {
                btn_att.visible = false;
                go.asCom.GetChild("n10").asTextField.visible = true;
                go.asCom.GetChild("n10").asTextField.text = Tools.GetMessageById("13018");
            }

            if(btn_att.visible) {
                if(LocalStore.GetFriendUID().IndexOf(d["uid"].ToString() + ",") > -1) {
                    btn_att.visible = false;
                    go.asCom.GetChild("n10").asTextField.visible = true;
                    go.asCom.GetChild("n10").asTextField.text = Tools.GetMessageById("13018");
                } else {
                    btn_att.visible = true;
                }
            }



            btn_att.onClick.Add(() => {
                Dictionary<string, object> dataAtt = new Dictionary<string, object>();
                dataAtt["fuid"] = d["uid"];
                NetHttp.inst.Send(NetBase.HTTP_FOLLOW, dataAtt, (VoHttp vo) => {
                    if((bool)vo.data == true) {
                        roleModel.AddAttentionFight(d["uid"].ToString());
                        btn_att.visible = false;
                        go.asCom.GetChild("n10").asTextField.visible = true;
                        go.asCom.GetChild("n10").asTextField.text = Tools.GetMessageById("13018");
                        LocalStore.SetFriendUID(d["uid"].ToString());
                    }
                });

            });
        }
    }

	public override void Clear ()
	{
		base.Clear ();
//		ViewManager.inst.ShowView<MediatorRedPackage> ();
	}
}
