using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ModelShare : BaseModel {


    //微博
    public string appKeyWeiBo;
    public string content1;
    public string content2;
    public string shareImageWinBo;
    public string openUrl;

    //微信
    public string appKeyWeiXin;
    public string shareImageWeiXin;


    public string shareImage;
	public string shareImageIcon;

    public const string SHARE_FIGHT = "2";
    public const string SHARE_FREIND = "1";

	//分享界面的位置
	public float viewX;
	public float viewY;
    public string type;

    public string fightShareType;//分享的地方

    public ModelShare()
    {

    }

    public void SetData(string shareImage,string type)
    {
        this.shareImage = shareImage;
        this.type = type;
    }

    public bool isShareRed(int type=1)//i:红点 2：分享奖励次数
    {
        bool isShareRed = false;
        DateTime timeNewShare = (DateTime)((Dictionary<string, object>)ModelManager.inst.userModel.records["share_data"])["succ_time"];
        bool isNewDayShare = Tools.IsNewDay(timeNewShare);
        if (isNewDayShare)
        {
            ((Dictionary<string, object>)ModelManager.inst.userModel.records["share_data"])["succ_times"] = 0;//今日分享出去的次数
        }
        int todayShare = (int)((Dictionary<string, object>)ModelManager.inst.userModel.records["share_data"])["succ_times"];
        Dictionary<string, object> share = (Dictionary<string, object>)DataManager.inst.systemSimple["share"];
        int shareNum = (int)(((object[])share["share1"])[0]);
        if (type == 1)
        {
            if (shareNum - todayShare == shareNum)
            {
                isShareRed = true;
            }
        }
        else
        {
            if (shareNum - todayShare>0)
            {
                isShareRed = true;
            }
        }
        return isShareRed;
    }


}
