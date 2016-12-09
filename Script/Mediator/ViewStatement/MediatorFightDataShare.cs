using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorFightDataShare : BaseMediator
{
    private GTextInput text;
    private GTextField title;
    private GButton ok;
    private ModelFight fightModel;
    private object[] fightDataDetails;
    private ModelUser userModel;
    private ModelRole roleModel;

    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_FIGTHTDATASHARE,false,Tools.GetMessageById("24233"));
        fightModel = ModelManager.inst.fightModel;
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        fightDataDetails = (object[])fightModel.fightDataDetails[fightModel.recordIndex];
		text =this.GetChild("n0").asTextInput;
        text.promptText=Tools.GetMessageById("24236");
        text.maxLength = (int)DataManager.inst.systemSimple["share_fight"];
		ok = this.GetChild("n4").asButton;
        ok.text = Tools.GetMessageById("24234");
        ok.RemoveEventListeners();
        ok.onClick.Add(SendShare);
    }

    private void SendShare(EventContext context)
    {


        if (roleModel.otherInfo["guild"] == null)
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("24241"));
        }
        else
        {
            if (Tools.IsNullEmpty(text.text))
            {
                text.text = Tools.GetMessageById("24236");
            }
            text.text = FilterManager.inst.Exec(text.text);
            Dictionary<string, object> value = new Dictionary<string, object>();
            value["log_id"] = fightDataDetails[1];
            value["chat_string"] = text.text;
            NetHttp.inst.Send(NetBase.HTTP_SHAREFIGHT, value, (VoHttp v) =>
            {
                if ((bool)v.data)
                {
                    ViewManager.inst.ShowText(Tools.GetMessageById("24235"));
                    ViewManager.inst.CloseView();
                }
            });
        }
        



    }

    public override void Clear()
    {
        base.Clear();
    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView(this);
    }
}