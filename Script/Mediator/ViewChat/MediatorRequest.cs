using System;
using FairyGUI;
using System.Collections.Generic;

public class MediatorRequest:BaseMediator
{
	private ModelChat chatModel;
	private ModelUser userModel;

	private GList list;
	private GButton btn;
	private GImage sel;

	private List<string> ld;

	public MediatorRequest ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_REQUEST, false, Tools.GetMessageById ("22025"));

		chatModel = ModelManager.inst.chatModel;
		userModel = ModelManager.inst.userModel;

		list = this.GetChild ("n5").asList;
		btn = this.GetChild ("n4").asButton;
		btn.text = Tools.GetMessageById ("22048");
		ld = chatModel.GetRequestCards ();

		list.itemRenderer = Item_Render;
		list.SetVirtual ();
		list.numItems = ld.Count;

		btn.onClick.Add (Btn_Click);
	}

	public override void Clear ()
	{
		base.Clear ();
	}

	private void Btn_Click ()
	{
		if (chatModel.support_Id == "")
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("22021"));
			return;
		}
//		if (!chatModel.IsSendRequestCard ())
//		{
//			ViewManager.inst.ShowText (Tools.GetMessageById ("22020"));
//			return;
//		}
		Dictionary<string,object> data = new Dictionary<string, object> ();
		data ["cid"] = chatModel.support_Id;
		NetHttp.inst.Send (NetBase.HTTP_REQUIREGUILDSUPPORT, data, (VoHttp vo) =>
		{
//			Log.debug ("RequestCard - " + vo.data.ToString ());
			ModelManager.inst.userModel.UpdateData (vo.data);
			ViewManager.inst.CloseView (this);
			this.DispatchGlobalEventWith (MainEvent.CHAT_SENDREQUESTCARD);
		});
	}

	private void Item_Render (int index, GObject go)
	{
		ComCard cc = go.asCom.GetChild ("n0") as ComCard;
		cc.SetData (ld [index]);

		cc.RemoveEventListeners ();
		cc.onClick.Add (() =>
		{
			chatModel.support_Id = ld [index].ToString ();

			GObject[] os = list.GetChildren ();
			foreach (GObject oo in os)
			{				
				if (go == oo)
					oo.asCom.GetController ("c1").selectedIndex = 1;
				else
					oo.asCom.GetController ("c1").selectedIndex = 0;
			}
		});
	}
}