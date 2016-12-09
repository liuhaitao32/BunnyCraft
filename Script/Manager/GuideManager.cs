using System;
using FairyGUI;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GuideManager
{
	//正常
	public static int GUIDE_NORMAL = 0;
	//条件
	public static int GUIDE_EXIST = 1;

	private static GuideManager instance;
	public int guide11;
	public int guide12;

	private GComponent stage;
	private GComponent view;
	private GGraph hand;
	private GTextField text;
	private GGroup content;
	private GButton btn1;
	private GComponent add;
	private GGraph mask;

	private GComponent v;
	private BaseMediator p;
	private GComponent ps;
	private ModelUser userModel;

	private object[] dataLen;
	//顺序
	public Dictionary<string,object> data1;
	//条件
	public Dictionary<string,object> data2;
	private int guide21 = -1;
	private int guide22 = -1;
	private int type = 0;
	private bool isEffect = false;
	private string curCheck;
	private Dictionary<string,object> curData;
	public Action onClick;
	//
	public bool isDraw;
	public GImage mImage;
	//
	public GuideManager ()
	{
	}

	public static GuideManager inst
	{
		get
		{
			if (instance == null)
				instance = new GuideManager ();
			return instance;
		}
	}

	public void Stage (GComponent stage,bool draw = false)
	{				
		isDraw = draw;
		userModel = ModelManager.inst.userModel;
		this.stage = stage;
		this.stage.visible = false;	
//		if (Convert.ToInt32 (userModel.records ["guide"]) == 2)
//			return;				
		view = Tools.GetComponent (Config.COM_GUIDE).asCom;
		this.stage.AddChild (view);

		mask = view.GetChild ("n0").asGraph;
		ViewManager.SetWidthHeight (mask);
		text = view.GetChild ("n3").asTextField;
		content = view.GetChild ("n1").asGroup;
		btn1 = view.GetChild ("n5").asButton;
		add = new GComponent ();
		view.GetChild ("n2").asGraph.ReplaceMe (add);
		btn1.onClick.Add (() =>
		{
			Clear ();
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.GUIDE_UPDATE_OK, null));
//			SetGuild(4);
//			this.guide11++;
//			this.guide12 = 0;
//			this.stage.visible = false;
		});
		mask.onClick.Add (Mask_Click);
		this.guide11 = 0;
		this.guide12 = 0;
		if (isDraw) {
			mask.visible = false;
		}
	}

	//	public GGraph GetMask ()
	//	{
	//		return mask;
	//	}

	public void SetGuide (int guide)
	{
		this.guide11 = guide;
		this.guide12 = 0;
		this.guide21 = -1;
		this.guide22 = -1;
	}

	public void SetGuideNormal (int guide)
	{
		this.guide11 = guide;
	}

	private void Mask_Click ()
	{
		if (isEffect)
			return;		
		if (type == GUIDE_NORMAL)
		{
			if (data1 ["button"].ToString () == "1")
				return;
		}
		else if (type == GUIDE_EXIST)
		{
			if (data2 ["button"].ToString () == "1")
				return;
		}
		if (onClick != null)
			onClick ();
		this.Next ();
	}

	// 0:0
	public bool Check (string guide)
	{		
		bool isOk = ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_GUIDE);
		if (!isOk)
			return false;
		curCheck = guide;
		return this.CheckData (guide) != null;
	}

	public void Show (BaseMediator view)
	{
		p = view;
		string[] names = curCheck.Split (':');
		int g1 = Convert.ToInt32 (names [0]);
		int g2 = Convert.ToInt32 (names [1]);
		if (this.guide11 == g1 && this.guide12 == g2)
		{			
			CheckView (GUIDE_NORMAL, data1);
		}
		else if (this.guide21 == g1 && this.guide22 == g2)
		{
			CheckView (GUIDE_EXIST, data2);
		}
	}

	private Dictionary<string,object> CheckData (string guide)
	{
		string[] names = guide.Split (':');
		int g1 = Convert.ToInt32 (names [0]);
		int g2 = Convert.ToInt32 (names [1]);
		btn1.visible = false;//临时屏蔽
		if (g1 >= 100)
		{
			btn1.visible = false;
			type = GUIDE_EXIST;
			dataLen = (object[])DataManager.inst.guide [g1.ToString ()];
			data2 = (Dictionary<string,object>)dataLen [g2];
			if (this.guide21 == -1)
			{
				this.guide21 = g1;
				this.guide22 = g2;
				return data2;
			}
			else if (this.guide21 == g1 && this.guide22 == g2)
			{
				return data2;
			}
		}
		else if (this.guide11 == g1 && this.guide12 == g2)
		{
			type = GUIDE_NORMAL;
			if (DataManager.inst.guide.ContainsKey (g1.ToString ())) {
				dataLen = (object[])DataManager.inst.guide [g1.ToString ()];
				if (dataLen.Length > 0) {
					data1 = (Dictionary<string,object>)dataLen [g2];
				} else {
					data1 = new Dictionary<string,object> ();
				}	
			} else {
				data1 = new Dictionary<string,object> ();
			}
			return data1;
		}
		return null;
	}
	private Vector2 v_pre;

	private void CheckView (int type, Dictionary<string,object> data)
	{
		v_pre = new Vector2(-1,-1);
		this.type = type;
		this.curData = data;
		stage.visible = true;
		if (data.ContainsKey ("name") && !data.ContainsKey ("info"))
		{
			v = p.GetChild (data ["name"].ToString ()).asCom;
			if (data.ContainsKey ("addp") && data ["addp"].ToString () == "1")
			{
//				Log.debug (data ["name"].ToString () + " -- " + v.x + " -- " + p.x);
				if (curCheck == "101:1")//love
				{
					v.x = v.x + p.x + p.parent.x + p.parent.parent.x;
					v.y = v.y + p.y + p.parent.y + p.parent.parent.y;
				}
				else
				{
					v.x = v.x + p.x;
					v.y = v.y + p.y + p.titleHeight;
				}
			}
			add.AddChild (v);

			if (hand == null)
			{
				hand = new GGraph ();
				view.AddChild (hand);
			}
			EffectManager.inst.AddEffect (Config.EFFECT_HAND, "hand", hand);
			hand.TweenMove (new Vector2 (v.x + v.width / 2, v.y + v.height / 2), 0.5f);
			if (data.ContainsKey ("hand") && data ["hand"].ToString() == "1") {
				hand.scaleX = -1;
				hand.rotation = -90;
			}
		}
		else if (data.ContainsKey ("info"))
		{
			object[] info = (object[])data ["info"];
			ShowInfo (info, 0);		

			if (curCheck == "100:2" || curCheck == "100:3")
			{				
				ps = p.GetChild ("n14").asList.GetChildAt (0).asCom;
				v = ps.GetChild (data ["name"].ToString ()).asCom;
				Vector2 root =  v.LocalToRoot (new Vector2 (v.x, v.y), v.root);
//				if (data.ContainsKey ("addp") && data ["addp"].ToString () == "1")
//				{					
//				Debug.LogError(v.root.width+" || "+p.width+" || "+(1138-v.root.width));
				v_pre = new Vector2 (v.x,v.y);
//				Tools.offectSetX (150);
				v.x = Tools.offectSetX (170);//root.x - v.width*0.5f;//
				if (curCheck == "100:3") {
					v.x = Tools.offectSetX (574f);//root.x - v.width*0.5f;//
				}
				v.y = v.y + (v.root.height - 640)*0.5f;
//				}
			}
			else if (curCheck == "101:2")//love
			{
				v = p.GetChild (data ["name"].ToString ()).asCom;
				if (data.ContainsKey ("addp") && data ["addp"].ToString () == "1")
				{					
					v.x = v.x + p.x + p.parent.x + p.parent.parent.x;
					v.y = v.y + p.y + p.parent.y + p.parent.parent.y;
				}
			}
			else
			{
				v = p.GetChild (data ["name"].ToString ()).asCom;
				if (data.ContainsKey ("addp") && data ["addp"].ToString () == "1")
				{
					v.x = v.x + p.x;
					v.y = v.y + p.y;
				}
			}			
			v.touchable = false;
			add.AddChild (v);
		}
		if (data.ContainsKey ("text"))
		{
			text.text = Tools.GetMessageById (data ["text"].ToString ());
			content.x = Convert.ToInt32 (data ["cx"]) + this.GetXX ();
			content.y = Convert.ToInt32 (data ["cy"]) + this.GetYY ();
			content.visible = true;
		}
		else
		{
			content.visible = false;
		}
	}

	private float GetXX ()
	{
//		Log.debug (Screen.width + " - " + ViewManager.bgOffsetW + " - " + GRoot.inst.width);
		return (GRoot.inst.width - 1138) / 2;
	}

	private float GetYY ()
	{
//		Log.debug (Screen.width + " - " + ViewManager.bgOffsetW + " - " + GRoot.inst.width);
		return (GRoot.inst.height - 640) / 2;
	}

	private void ShowInfo (object[] info, int index)
	{
		this.isEffect = true;
		if (info.Length > index)
		{
			object[] o = (object[])info [index];
			GComponent g = Tools.GetComponent (Config.COM_POPTEXT).asCom;
			g.touchable = false;
			g.GetChild ("n0").text = Tools.GetMessageById (o [0].ToString ());
			g.x = Convert.ToInt32 (o [1]) + this.GetXX ();
			g.y = Convert.ToInt32 (o [2]) + this.GetYY ();
			g.GetController ("c1").selectedIndex = o [3].ToString () == "0" ? 0 : 1;
			g.scaleY = 0f;
//			g.scale = new Vector2 (0f, 0f);
//			g.TweenScale (new Vector2 (1f, 1f), 1f);
			g.TweenScaleY(1f,0.5f);

			DOTween.To (() => g.scaleY, x => g.scaleY = x, 1, 0.2f).OnComplete(()=>{
				g.InvalidateBatchingState ();
				DOTween.Kill(g,true);
			});
			add.AddChild (g);

			TimerManager.inst.Add (1.1f, 1, (float obj) =>
			{
				ShowInfo (info, ++index);
			});
		}
		else
		{
			isEffect = false;
		}
	}

	public void Next (bool isShow = false)
	{		
		Clear ();
		Dictionary<string,object> data = this.NextData ();
		if (data == null)
			return;
		if (curData != null && curData ["button"].ToString () == "0")
		{            
			this.Show (p);
		}
		else if (isShow)
		{
			this.Show (p);
		}
	}

	//type  0正常 1条件
	//	public int[] GetData (int type)
	//	{
	//		if (type == GUIDE_NORMAL)
	//			return new int[]{ this.guide11, this.guide12 };
	//		else
	//			return new int[]{ this.guide21, this.guide22 };
	//	}

	public void Clear ()
	{		
		if (v != null && ps != null && ps.parent != null)
		{
			if (curData.ContainsKey ("addp") && curData ["addp"].ToString () == "1")
			{
				v.x -= ps.x;
				v.y -= ps.y;
			}
			if (v_pre != null && v_pre.x>0) {
				v.x = v_pre.x;
				v.y = v_pre.y;
			}
			v.touchable = true;
			ps.AddChild (v);
			v = null;
			ps = null;
		}
		else if (v != null && p != null && p.parent != null)
		{
			if (curData.ContainsKey ("addp") && curData ["addp"].ToString () == "1")
			{
				if (curCheck == "101:1" || curCheck == "101:2")
				{
					v.x = v.x - p.x - p.parent.x - p.parent.parent.x;
					v.y = v.y - p.y - p.parent.y - p.parent.parent.y;
				}
				else
				{
					v.x -= p.x;
					v.y -= p.y;
				}
			}
			v.touchable = true;
			if (v_pre != null && v_pre.x>0) {
				v.x = v_pre.x;
				v.y = v_pre.y;
			}
			p.AddChild (v);
			v = null;
		}
		add.RemoveChildren (0,-1,true);
		if (hand != null)
		{
			
			view.RemoveChild (hand);
			hand.Dispose ();
			hand = null;
		}
		this.stage.visible = false;
	}

	public Dictionary<string,object> NextData ()
	{
		Dictionary<string,object> data;
		if (type == GUIDE_NORMAL)
		{
			this.guide12++;
			if (this.guide12 >= dataLen.Length)
			{
				this.guide11++;
//				userModel.records ["guide"] = this.guide11;
//				ModelManager.inst.userModel.SetGuide (this.guide11, (VoHttp vo) =>
//				{
//					ModelManager.inst.userModel.UpdateData (vo.data);
//					DispatchManager.inst.Dispatch (new MainEvent (MainEvent.GUIDE_UPDATE_OK, null));
//				});
				SetGuild(this.guide11);
				this.guide12 = 0;
				return null;
			}
			curCheck = this.guide11 + ":" + this.guide12;
			data = this.CheckData (this.guide11 + ":" + this.guide12);
		}
		else
		{
			this.guide22++;
			if (this.guide22 >= dataLen.Length)
			{
				this.guide21 = -1;
				this.guide22 = -1;
				return null;
			}
			curCheck = this.guide21 + ":" + this.guide22;
			data = this.CheckData (this.guide21 + ":" + this.guide22);
		}
		return data;
	}
	public void SetGuild(int index){
//		ModelManager.inst.userModel.SetGuide (index, (VoHttp vo) => {
//			ModelManager.inst.userModel.UpdateData (vo.data);
//			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.GUIDE_UPDATE_OK, null));
//		});
	}

}