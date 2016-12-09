using UnityEngine;
using System.Collections;
using FairyGUI;
using System;
using System.Collections.Generic;

public class MediatorFightDataShowMatch : BaseMediator {
    private GList list1;
    private List<object[]> listData;
    private object[] fightDataDetails;
    private object[] dataMe;
    private ModelFight fightModel;
    private ModelRole roleModel;
    private ModelUser userModel;
    private int playbackfailure_time;
    private GButton share;
    private GButton watch;
    private GTextField timeFail;
    private DateTime video_deadline;

    public override void Init()
    {
        base.Init();
        string myUid = "";
        fightModel = ModelManager.inst.fightModel;
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        playbackfailure_time = (int)DataManager.inst.systemSimple["playbackfailure_time"];
        video_deadline = (DateTime)DataManager.inst.systemSimple["video_deadline"];
        if (fightModel.isOpenFromRecord)
        {
            myUid = roleModel.otherInfo["uid"].ToString();
            fightDataDetails = (object[])fightModel.fightDataDetails[fightModel.recordIndex];
            Create(Config.VIEW_FIGTHTDATASHOWMATCH, false,(string)roleModel.otherInfo["uname"]+ Tools.GetMessageById("24224"));
        }
        else
        {
            myUid = fightModel.myUid;
            fightDataDetails = (object[])fightModel.fightDataDetails;
            Create(Config.VIEW_FIGTHTDATASHOWMATCH, false, fightDataDetails[fightDataDetails.Length-1].ToString()+ Tools.GetMessageById("24224"));
        }
		GTextField text1 = this.GetChild("n5").asTextField;
		GTextField text2 = this.GetChild("n7").asTextField;
		GTextField text3 = this.GetChild("n4").asTextField;
		GTextField text4 = this.GetChild("n6").asTextField;
	    timeFail = this.GetChild("n10").asTextField;
        GTextField value1 = this.GetChild("value1").asTextField;
        GTextField value2 = this.GetChild("value2").asTextField;
        GTextField value3 = this.GetChild("value3").asTextField;
        GTextField value4 = this.GetChild("value4").asTextField;
        list1 = this.GetChild("n3").asList;
        listData = new List<object[]>();
       
	    share=this.GetChild("n9").asButton;
        watch = this.GetChild("n0").asButton;
        bool ok = userModel.GetUnlcok(Config.UNLOCK_FIGHTSHARE, share);
        if (!ok)
        {
            watch.x = 377;
        }
        //share.visible = true;
        share.text = Tools.GetMessageById("24219");
        //share.asCom.GetChild("n1").asLoader.url = Tools.GetResourceUrl("Image2:n_icon_you1");
        share.RemoveEventListeners();
        share.onClick.Add(ShareOnclick);
		
        watch.text = Tools.GetMessageById("24248");
        //watch.asCom.GetChild("n1").asLoader.url = Tools.GetResourceUrl("Image2:n_icon_you2");
        watch.RemoveEventListeners();
        watch.onClick.Add(WatchOnclick);

        if (fightModel.isOpenFromRecord)
        {
            if (!myUid.Equals(userModel.uid))
            {
                share.visible = false;
                watch.visible = false;
            }
            else
            {
                if (!TimeFail())
                {
                    share.visible = false;
                    watch.visible = false;
                    timeFail.visible = true;
                    timeFail.text = Tools.GetMessageById("10038");
                }
            }
        }
        else
        {
            share.visible = false;
        }

        object[] dataAll = (object[])fightDataDetails[3];
        for (int i = 0; i < dataAll.Length; i++)
        {
            object[] data = (object[])dataAll[i];
            if (data[0].ToString().Equals(myUid))
            {
                dataMe = (object[])data[5];
            }
            listData.Add(data);
        }
        string str1 = Tools.GetMessageById("24231") + ":";
        string str1_1 = dataMe[0].ToString();
        string str2 = Tools.GetMessageById("24213") + ":";
        string str2_1 = dataMe[3].ToString();
        string str3 = Tools.GetMessageById("24211") + ":";
        string str3_1 = dataMe[1].ToString();
        string str4 = Tools.GetMessageById("24212") + ":";
        string str4_1 = dataMe[2].ToString();
        text1.text = str1;
        text2.text = str2;
        text3.text = str3;
        text4.text = str4;
        value1.text = str1_1;
        value2.text = str2_1;
        value3.text = str3_1;
        value4.text = str4_1;
        list1.itemRenderer = OnRander;
        list1.numItems = listData.Count;
    }

    private bool TimeFail()
    {
        bool isVisible = true;
        DateTime time = (DateTime)fightDataDetails[2];
        float disTime = roleModel.OneMili * playbackfailure_time;
        if (ModelManager.inst.gameModel.time.Ticks - time.Ticks > disTime)
        {
            isVisible = false;
        }
        if (ModelManager.inst.gameModel.time.Ticks >= video_deadline.Ticks)
        {
            isVisible = false;
        }
        return isVisible;
    }

    private void OnRander(int index, GObject item)
    {
        object[] data = (object[])listData[index];
        GComponent obj = item.asCom;
        obj.GetChild("n0").asCom.GetChild("n2").text = data[2].ToString();
        GButton head=obj.GetChild("n0").asCom.GetChild("n0").asButton;
        if (Tools.IsNullEmpty(data[1]))
        {
            obj.GetChild("n2").asTextField.text = data[0].ToString();
        }
        else
        {
            obj.GetChild("n2").asTextField.text = data[1].ToString();
        }
        if ((int)data[0] > 0)
        {
            Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(((Dictionary<string, object>)data[6])["use"].ToString()));

        }
        else
        {
            Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(data[1].ToString(), false, true));

        }
        
        obj.GetChild("n3").asButton.onClick.Add(()=> {
            fightModel.openIndex = index.ToString();
			ViewManager.inst.ShowView<MediatorFightDataShowMember>();
        });
    }


    private void WatchOnclick(EventContext context)
    {
        if (!TimeFail())
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("10038"));
        }
        else
        {

        }
    }

    private void ShareOnclick(EventContext context)
    {
        userModel.GetUnlcok(Config.UNLOCK_SHARD, null, true);
        ViewManager.inst.ShowView<MediatorFightDataShare>();
    }

    public override void Clear()
    {

        base.Clear();
        fightModel.isOpenFromRecord = true;


    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView(this);
    }
}
