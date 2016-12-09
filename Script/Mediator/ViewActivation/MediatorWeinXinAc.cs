using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;

public class MediatorWeinXinAc : BaseMediator {
    private GButton btn_ok;
    private int lv;
    private GTextField textContext;
    private GTextField textTitle;

    public override void Init()
    {
        base.Init();
		Create(Config.VIEW_WEINXINAC,false,Tools.GetMessageById("31001"));
        Dictionary<string,object> dd= (Dictionary<string,object>)DataManager.inst.systemSimple["weixin_gift"];
		textContext = this.GetChild("n4").asTextField;
		GTextField num = this.GetChild("n8").asTextField;
		GImage loader = this.GetChild("n1").asImage;
		GButton saveToPhone = this.GetChild("n12").asButton;
        saveToPhone.text = Tools.GetMessageById("31009");
        saveToPhone.onClick.Add(() => {
            Texture2D _tex = (Texture2D)Resources.Load("Embed/n_icon_saomiao_");
            PhoneManager.inst.SaveTexture2D(_tex, "n_icon_saomiao_");
        });

        textContext.text = Tools.GetMessageById("31005");
        num.text = Tools.GetMessageById("31008");
    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView(this);
    }
}
