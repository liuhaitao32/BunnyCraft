using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;

public class MediatorGetWin : BaseMediator {
    private GTextInput uid;
    private GButton btn_ok;
    private ModelUser userModel;
    private Dictionary<string, object> share;

    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_GETWIN,false,Tools.GetMessageById("19956"));
        InitData();
        FindObject();
    }

    private void InitData()
    {
        userModel = ModelManager.inst.userModel;
        share = (Dictionary<string, object>)DataManager.inst.systemSimple["share"];
    }

    private void FindObject()
    {
		uid = this.GetChild("n5").asCom.GetChild("n1").asTextInput;
        uid.promptText = Tools.GetMessageById("13137");
		btn_ok = this.GetChild("n4").asButton;
        btn_ok.text = Tools.GetMessageById("19008");
        btn_ok.onClick.Add(() => {
        
            if (!uid.text.Equals(""))
            {
                if (!uid.text.Equals(userModel.uid))
                {
                    try
                    {
                        Convert.ToInt32(uid.text);
                        if (!Tools.IsNullEmpty(userModel.records["invite_uid"] + ""))
                        {
                            ViewManager.inst.ShowText(Tools.GetMessageById("19962"));
                        }
                        else
                        {
                            Dictionary<string, object> data = new Dictionary<string, object>();
                            data["uid"] = uid.text;
                            NetHttp.inst.Send(NetBase.HTTP_GETWIN, data, (VoHttp vo) =>
                            {
                                DispatchManager.inst.Dispatch(new MainEvent(MainEvent.EVENTSHARE, "1"));
                                ViewManager.inst.ShowText(Tools.GetMessageById("19961"));
                                Dictionary<string, object> dIcon = new Dictionary<string, object>();
                                dIcon[Config.ASSET_COIN] = share["invited"];
                                ViewManager.inst.ShowIcon(dIcon, () => {
                                    userModel.UpdateData(vo.data);
                                });
                                ViewManager.inst.CloseView(this);

                            });
                        }
                    }
                    catch(Exception e)
                    {
                        ViewManager.inst.ShowText(Tools.GetMessageById("13160"));
                    }
                    
                }
                else {
                    ViewManager.inst.ShowText(Tools.GetMessageById("19436"));
                }
            }
            else {
                ViewManager.inst.ShowText(Tools.GetMessageById("13138"));
            }
        });
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
