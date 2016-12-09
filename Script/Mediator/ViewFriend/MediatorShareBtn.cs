using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorShareBtn : BaseMediator {
    private GList list;
    private ModelUser userModel;
    private ModelShare shareModel;
    private object[] shareIcon;
    private Dictionary<string, object> friendLike;
    private string weiXinId;
    private string weiBoId;
    private string weiBoContent;
    private string weiBoUrl;
    private string weiBoCallBack;

	private string qqId;
	private string qqURL;
	private string qqTitle;
	private string qqDes;

	private Loads lw;
	private Texture2D iconImage;
	private string iconImageStr;
    // Use this for initialization
    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_SHAREBTN);

        userModel = ModelManager.inst.userModel;
        shareModel = ModelManager.inst.shareModel;
        this.y = shareModel.viewY;
        weiXinId = DataManager.inst.systemSimple[DataManager.SHARE_WEIXIN_ID].ToString();

        weiBoId = DataManager.inst.systemSimple[DataManager.SHARE_WEIBO_ID].ToString();
        weiBoContent = DataManager.inst.systemSimple[DataManager.SHARE_WEIBO_CONTENT].ToString();
        weiBoUrl = DataManager.inst.systemSimple[DataManager.SHARE_WEIBO_URL].ToString();
        weiBoCallBack = DataManager.inst.systemSimple[DataManager.SHARE_WEIBO_CALLBACK].ToString();

		qqId = DataManager.inst.systemSimple[DataManager.SHARE_QQ_ID].ToString();
		qqURL = DataManager.inst.systemSimple[DataManager.SHARE_QQ_URL].ToString();
		qqTitle = DataManager.inst.systemSimple[DataManager.SHARE_QQ_TITLE].ToString();
		qqDes = DataManager.inst.systemSimple[DataManager.SHARE_QQ_DES].ToString();

        friendLike = (Dictionary<string, object>)DataManager.inst.systemSimple["share"];
        shareIcon = (object[])DataManager.inst.systemSimple["share_icon"];


		iconImageStr = shareModel.shareImageIcon;
		list = this.GetChild("n1").asList;
        GImage imag1 = this.GetChild("n2").asImage;
        GImage imag2 = this.GetChild("n3").asImage;
        GTextField t1 = this.GetChild("n32").asTextField;
        t1.text = Tools.GetMessageById("19439");
        if (shareModel.isShareRed(2))
        {
            t1.visible = true;
        }
        if (shareModel.type == ModelShare.SHARE_FREIND)
        {
            
            this.x = shareModel.viewX;
            imag1.visible = true;
            imag2.visible = false;
        }
        else
        {
            imag1.visible = false;
            imag2.visible = true;
        }

        //        list.fairyBatching = true;
        list.itemRenderer = OnRendere;
        list.numItems = shareIcon.Length;
		//
		AddGlobalListener(MainEvent.SHARE_DATA_EVENT,onSHARE_DATA_EVENT);
    }
	private string shareType;
    private void OnRendere(int index, GObject item)
    {
        GComponent obj=item.asCom;
        GTextField text=obj.GetChild("n1").asTextField;
        GLoader icon=obj.GetChild("n2").asLoader;

        object[] data=(object[])shareIcon[index];
        text.text = Tools.GetMessageById(data[0].ToString());
        icon.url = Tools.GetResourceUrl("Icon:"+data[1].ToString());
        item.onClick.Add(()=> {
			shareType = data[2].ToString();
			ViewManager.inst.ShowView<MediatorAd>(false,false);
			return;
        });
    }
	private void onSHARE_DATA_EVENT(MainEvent e){
//		Debug.LogError ("onSHARE_DATA_EVENT");
		switch (shareType) {
			case "weixinfriend":
				ShareWeixin (0);//微信好友
				break;
			case "weixin":
				ShareWeixin (1);//微信朋友圈
				break;
			case "weibo":
				ShareWeibo ();
				break;
			case "qq":
				ShareQQ (1);
				break;
		}
	}
    public override void Clear()
    {
        base.Clear();
		RemoveGlobalListener(MainEvent.SHARE_DATA_EVENT,onSHARE_DATA_EVENT);

    }
    private void ShareWeibo()
    {
        if (PlatForm.inst.pf != PlatForm.EX_LOCAL)
            PlatForm.inst.GetSdk().Call(Ex_Local.CALL_WEIBO, new string[] { weiBoId, 
				weiBoContent, 
				weiBoUrl, 
//				shareModel.shareImage, 
				iconImageStr,
				weiBoCallBack }, sdk_callback);

//		PlatForm.inst.GetSdk ().InitPF (Ex_Local.LOGIN_TYPE_QQ);
//		PlatForm.inst.GetSdk ().InitPF (Ex_Local.LOGIN_TYPE_WEIXIN);
//		NetHttp.inst.sendHttp_wx_code("https://api.weixin.qq.com/sns/oauth2/access_token?appid=wx2e794e0b84371812&secret=6a84fee18c26bb4792451ac9d2748cd1&code=001TWVMc16M61p0xtkMc1wLXMc1TWVMj&grant_type=authorization_code");
    }

    private void ShareWeixin(int type)
    {
		if (PlatForm.inst.pf != PlatForm.EX_LOCAL) {
			PlatForm.inst.GetSdk ().Call (Ex_Local.ERROR_NO_WX, null, (object a) => {
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("33208"));
			});
			if (shareModel.type == ModelShare.SHARE_FREIND) {
				PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_WECHAR, new string[]{ 
					weiXinId,
					weiBoContent,
					weiBoUrl,
					iconImageStr,
					type.ToString()
				}, sdk_callback);
			} else {
				PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_WECHARIMG, new string[] { weiXinId, 
					shareModel.shareImage, 
					type.ToString ()
				}, sdk_callback);
			}
		}
    }
	private void ShareQQ(int type)
	{
//		NSString *key       = [arr objectAtIndex:0];
//		NSString *url       = [arr objectAtIndex:1];
//		NSString *title     = [arr objectAtIndex:2];
//		NSString *des       = [arr objectAtIndex:3];
//		NSString *image     = [arr objectAtIndex:4];
//		NSString *type      = [arr objectAtIndex:5];

		if (PlatForm.inst.pf != PlatForm.EX_LOCAL) {
			PlatForm.inst.GetSdk ().Call (Ex_Local.ERROR_NO_QQ, null, (object a) => {
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("33207"));
			});
			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_QQ, new string[] { qqId, 
				qqURL,
				qqTitle,
				qqDes,
				shareModel.shareImage,//"http://i1.dpfile.com/groups/grouppic/2009-07-03/lingshi_4112163_1488300_l.jpg",
				type.ToString ()
			}, sdk_callback);
		}
//		PlatForm.inst.GetSdk ().Login (Ex_Local.LOGIN_TYPE_QQ);
//		PlatForm.inst.GetSdk ().Login (Ex_Local.LOGIN_TYPE_WEIXIN);

	}
	void sdk_callback(object str)
    {
		Debug.Log ("--lht-- :: "+str.ToString());
		ViewManager.inst.CloseView();
		string code = str.ToString(); 
		if (code.IndexOf("0")>-1) {
			if (shareModel.type == ModelShare.SHARE_FREIND || shareModel.type == ModelShare.SHARE_FIGHT) {
                ViewManager.inst.ShowText(Tools.GetMessageById("19960"));
                if (IsShare ()) {
					NetHttp.inst.Send (NetBase.HTTP_SHARE, "", (VoHttp vo) => {
						object[] gift_coin = (object[])friendLike ["share1"];
						Dictionary<string, object> dIcon = new Dictionary<string, object> ();
						dIcon [Config.ASSET_COIN] = gift_coin [1];
						ViewManager.inst.ShowIcon (dIcon, () => {
							userModel.UpdateData (vo.data);
							DispatchManager.inst.Dispatch (new MainEvent (MainEvent.EVENTSHARE, "2"));
						});
					});
				} else {
                    ViewManager.inst.ShowText(Tools.GetMessageById("13121"));
				}

			}
		} else {
			ViewManager.inst.ShowText (Tools.GetMessageById("13122"));
		}
    }
    private bool IsShare()
    {
        Dictionary<string,object> share = (Dictionary<string, object>)DataManager.inst.systemSimple["share"];
        int shareNum_ = (int)(((object[])share["share1"])[0]);
        int shareNum = (int)((Dictionary<string, object>)userModel.records["share_data"])["succ_times"];
        int num = shareNum_ - shareNum;
        if (num <= 0)
            return false;
        return true;

    }
}
