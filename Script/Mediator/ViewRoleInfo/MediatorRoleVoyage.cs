using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using FairyGUI;

public class MediatorRoleVoyage : BaseMediator {
    private List<object> list;
    private GList list_voyage;
    private int rankMax;
    private ModelRole roleModel;
    private ModelUser userModel;
    //private Dictionary<string, object> voyage;
    private Dictionary<string, object> voyage_data;
    public override void Init()
    {
        Create(Config.VIEW_ROLEVOYAGE);
		list_voyage = this.GetChild("n2").asList;
        InitDate();
    }

    private void InitDate()
    {

        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        rankMax=(int)DataManager.inst.systemSimple["season_ranking"];
        if (roleModel.otherInfo["uid"].ToString().Equals(userModel.uid) && ModelRole.fight4 != null && ModelRole.fight4.Count!=0)
        {
            SetData(ModelRole.fight4);
        }
        else
        {
            GetData();
        }
    }

    private void GetData()
    {
        Dictionary<string, object> value = new Dictionary<string, object>();
        value["uid"] = roleModel.otherInfo["uid"];
        NetHttp.inst.Send(NetBase.HTTP_GET_HISTORY_SEASONS, value, (VoHttp v) =>
        {
            Dictionary < string, object> data= (Dictionary<string, object>)v.data;
            data.Remove("-1");
            voyage_data = FilterData(data);
            SetData(voyage_data);
        });
    }

    private Dictionary<string,object> FilterData(Dictionary<string, object> data)
    {
        Dictionary<string, object> voyage_data = new Dictionary<string, object>();
        foreach (KeyValuePair<string,object> v in data)
        {
            object[] obj=(object[])v.Value;
            if ((int)obj[0] != 0)
            {
                voyage_data.Add(v.Key,v.Value);
            }
        }
        return voyage_data;
    }

    private void SetData(Dictionary<string, object> data)
    {
        voyage_data = data;
        ModelRole.fight4 = data;
        list = Tools.ConvertDicToList(data, "name");
        Tools.Sort(list, new string[] { "name:int:1" });
        InitItem();
    }

    private void InitItem()
    {
        list_voyage.emptyStr = Tools.GetMessageById("13118");
        list_voyage.itemRenderer = Voyage_Render;
        list_voyage.SetVirtual();
        list_voyage.numItems = voyage_data.Count;
        if (voyage_data.Count > 4)
        {
            list_voyage.scrollPane.touchEffect = true;
            list_voyage.ScrollToView(0);
        }
        else
        {
            list_voyage.scrollPane.touchEffect = false;
        }
    }

    private void Voyage_Render(int index, GObject item)
    {
        Dictionary<string, object> data = (Dictionary<string, object>)list[index];
        int index_=Convert.ToInt32(data["name"]);
        if (index_ == -1)
            return;
        object[] dc= (object[])data[index_ + ""];

        GTextField textTitle=item.asCom.GetChild("n1").asTextField;
        GLoader load=item.asCom.GetChild("n2").asLoader;
        GTextField group=item.asCom.GetChild("n5").asTextField;
         GTextField rankScore =item.asCom.GetChild("n94").asTextField;
        GTextField rankLevel=item.asCom.GetChild("n3").asTextField;
        GTextField rankLevelValue= item.asCom.GetChild("n4").asTextField;
        if ((int)dc[1]>rankMax) {
            rankLevelValue.text = rankMax+Tools.GetMessageById("13129");
        }
        else
        {
            rankLevelValue.text = dc[1].ToString();
        }
        textTitle.text = Tools.GetMessageById("31079", new string[] { (index_ + "").ToString() });
        load.url = userModel.GetRankImg((int)dc[0], 0, true);
        group.text = Tools.getRankGroup((int)dc[0]);//大师组
        rankScore.text = dc[0].ToString();
        rankLevel.text = Tools.GetMessageById("13085");
       
    }
}
