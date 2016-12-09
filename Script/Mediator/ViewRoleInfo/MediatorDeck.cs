using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using FairyGUI;
public class MediatorDeck : BaseMediator {
    private List<object> data;
    private GList list_record;
    private ModelRole roleModel;
    private List<object> roleRecord;
    private ModelUser userModel;

    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_ROLEDECK);
        InitItem();
        InitDate();

    }
    
    private void InitDate()
    {
        userModel=ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        object[] nameArr = (object[])roleModel.otherInfo["card_group"];
        List<object> carArr = Tools.ConvertDicToList((Dictionary<string,object>)roleModel.otherInfo["card"], "name");
        data = new List<object>();
        foreach (object v in nameArr)
        {
            foreach (object v1 in carArr)
            {
                if (v.ToString().Equals(((Dictionary<string, object>)v1)["name"].ToString()))
                {
                    data.Add(v1);
                }
            }
        }
        list_record.itemRenderer = Record_Render;
        list_record.numItems = data.Count;
        
    }

    private void InitItem()
    {
		list_record = this.GetChild("n2").asList;
        
    }

    private void Record_Render(int index, GObject item)
    {
        ///		["lv"]	1	System.Collections.DictionaryEntry
        //+       ["new"] 0   System.Collections.DictionaryEntry
        //+       ["exp"] 1   System.Collections.DictionaryEntry
        //+       ["name"]    "C007"  System.Collections.DictionaryEntry
        Dictionary<string, object> dc = (Dictionary<string, object>)data[index];
        //if (roleModel.otherInfo["uid"].Equals(userModel.uid))
        //{
        //    ComCard card = item as ComCard;
        //    CardVo v = DataManager.inst.GetCardVo(dc["name"].ToString());
        //    card.SetData(v.id, -1, 2);
        //    card.SetText(Tools.GetMessageById(v.name));
        //    item.onClick.Add(() =>
        //    {
        //        ViewManager.inst.ShowCardInfo(v.id, 4);
        //    });
        //}
        //else
        //{
        ComCard card = item as ComCard;
        CardVo v = DataManager.inst.GetCardVo(dc["name"].ToString(), (int)dc["lv"]);
        card.SetData(v.id, -1, 2, v.lv);
        card.SetText(Tools.GetMessageById(v.name));
        item.onClick.Add(() =>
        {
            //ViewManager.inst.ShowCardInfo(v.id, 3, v.lv);
            MediatorItemShipInfo2.CID = v.id;
            //MediatorItemShipInfo2.isKu = 3;
            MediatorItemShipInfo2.lv = v.lv;
            ViewManager.inst.ShowView<MediatorItemShipInfo2>();
        });

        //}

    }
}
