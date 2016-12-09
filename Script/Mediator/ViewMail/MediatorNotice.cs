using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using FairyGUI;

public class MediatorNotice : BaseMediator
{
	private List<object> noticeData;
	private GList list_notice;

    public override void Init ()
	{
        noticeData = new List<object>();
        Create (Config.VIEW_NOTICE,false,Tools.GetMessageById("14061"));
        //保存
        LocalStore.SetLocal(LocalStore.NOTICE_VERSION, ModelManager.inst.userModel.notice_version);
        FindObject ();
		initData ();
        
        
	}

    private void InitItem ()
	{

        list_notice.emptyStr = Tools.GetMessageById("21011");
        list_notice.itemRenderer = OnRender;
		list_notice.numItems = noticeData.Count;
	}

	private void OnRender (int index, GObject item)
	{
		GTextField title = item.asCom.GetChild ("n1").asTextField;
		GTextField content = item.asCom.GetChild ("n2").asTextField;
		GButton more = item.asCom.GetChild ("n3").asButton;
		GImage bg = item.asCom.GetChild ("n0").asImage;
		Dictionary<string, object> itemData = (Dictionary<string,object>)noticeData [index];

		if (itemData ["button"].Equals (""))
		{
			more.visible = false;
		}
		else
		{
			more.visible = true;
			more.text = Tools.GetMessageById((string)itemData["button"]);
			more.onClick.Add (() =>
			{
				Application.OpenURL (Tools.GetMessageById((string)itemData["url"]));
			});
		}
        
        title.text = BaseUbbParser.inst.Parse(Tools.GetMessageById((string)itemData["name"]));
		content.text = BaseUbbParser.inst.Parse(Tools.GetMessageById((string)itemData["content"]));
        item.asCom.height = content.textHeight + content.y + more.height +30;
        bg.height = item.asCom.height;
        more.y = item.asCom.height - more.height - 20;
    }

	private void FindObject ()
	{
		list_notice = this.GetChild ("n0").asList;
    }

	private void initData ()
	{
		Dictionary<string, object> dd = new Dictionary<string, object> ();
		dd ["version"] = PlatForm.inst.App_Version;
		NetHttp.inst.Send (NetBase.HTTP_GETNOTICE, dd, (VoHttp vo) =>
		{
			object[] obj = (object[])vo.data;
            if (!Tools.IsNullEmpty(obj[0]))
            {
                foreach (object v in ((object[])obj[1]))
                {
                    noticeData.Add(v);
                }
            }
            InitItem();
			
		});
	}

	public override void Clear ()
	{
		base.Clear ();
        noticeData.Clear();

    }
    public override void Close()
    {
        base.Close();
        ViewManager.inst.CloseView(this);
    }
}
