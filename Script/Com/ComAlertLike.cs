using UnityEngine;
using System.Collections;
using FairyGUI;
using System;

public class ComAlertLike : BaseMediator
{
    private GTextField title;
    private GTextField text_Check;
    private GTextField expendNum;
    private GLoader icon;
    private GButton btn_Check;
    private GButton btn_Cancel;
    private GButton btn_Ok;
    private ModelAlert alertModel;

    public override void Init()
    {
        base.Init();
        Create(Config.COM_ALERTLIKE);

        alertModel=ModelManager.inst.alertModel;
        title=GetChild("n2").asTextField;
        expendNum = GetChild("n4").asTextField;
        text_Check = GetChild("n8").asTextField;
        icon = GetChild("n3").asLoader ;
        btn_Check = GetChild("n7").asButton;
        btn_Cancel = GetChild("n5").asButton;
        btn_Cancel.text = Tools.GetMessageById("14025");
        btn_Ok = GetChild("n6").asButton;
        btn_Ok.text = Tools.GetMessageById("14056");
        title.text = Tools.GetMessageById("13056");
        text_Check.text = Tools.GetMessageById("13123");
        icon.url = Tools.GetResourceUrl("Image2:n_icon_zs");
        expendNum.text = alertModel.count.ToString();
        btn_Ok.onClick.Add(BtnOk);
        btn_Check.onChanged.Add(BtnCheck);
        btn_Cancel.onClick.Add(BtnCancel);

    }

    private void BtnCancel(EventContext context)
    {
        ViewManager.inst.CloseAlert();
        alertModel.execute(0);
    }

    private void BtnCheck()
    {
        if (btn_Check.selected)
        {
            alertModel.isOpen = false;
        }
        else
        {
            alertModel.isOpen = true;
        }
    }

    private void BtnOk(EventContext context)
    {
        ViewManager.inst.CloseAlert();
        alertModel.execute(1);
    }

    public override void Clear()
    {
        base.Clear();
    }
}
