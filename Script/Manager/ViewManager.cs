using System;
using UnityEngine;
using FairyGUI;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;

public class ViewManager
{
	private static ViewManager instance;

	private BaseMediator scene;
	private List<Type> scenes;
	private List<BaseMediator> views;
	private List<object[]> touchs;
	//scene
	public GComponent s;
	//view
	private GComponent v;
	//p
	private GComponent p;
	//flash
	private GComponent f;
	//guide
	private GComponent g;
	// top
	private GComponent t;
	//mask wait
	private GComponent mask;
	private GComponent tmask;
	private GComponent pmask;
	private GComponent sceneMask;
	private ComAlert alert;
	private ComAlertLike alertLike;
	private ComLoad wait;
	private int waitCount = 0;
	//showicon
	private ComGoldCoinExp showIcon;
	private Action showIconFun;
	private object tween_ShowIcon;
	//tip
	private ComTip touch;
	//message
	private GComponent showMessage;
	private List<string> messages;

	public static float bgOffsetX = -1;
	public static float bgOffsetW = -1;
    public ViewManager ()
	{
		this.Register ();
	}

	public static ViewManager inst
	{
		get
		{
			if (instance == null)
				instance = new ViewManager ();
			return instance;
		}
	}

	public void Register ()
	{
		scenes = new List<Type> ();
		views = new List<BaseMediator> ();
		touchs = new List<object[]> ();
		messages = new List<string> ();

		s = new GComponent ();
		v = new GComponent ();
		p = new GComponent ();
		f = new GComponent ();
		g = new GComponent ();
		t = new GComponent ();
		GRoot.inst.AddChild (s);

		s.z = 0;
		GRoot.inst.AddChild (v);
		v.z = -300;
		GRoot.inst.AddChild (p);
		p.z = -301;
		GRoot.inst.AddChild (f);
		f.z = -302;
		GRoot.inst.AddChild (g);
		g.z = -303;
		GRoot.inst.AddChild (t);
		t.z = -304;

		SetTMask ();
		SetMask ();
//		SetPMask ();
		GuideManager.inst.Stage (g);
//		this.SetWidthHeight (GuideManager.inst.GetMask ());
//		this.SetWidthHeight (g);
		AddEventTouch ();
	}

	public static void SetWidthHeight (GObject g)
	{
		if (g is GComponent)
		{
			GComponent ggg = g.asCom;
			GObject gg;
			for (int i = 0; i < ggg.numChildren; i++)
			{
				gg = ggg.GetChildAt (i);
				gg.height = GRoot.inst.height;
				gg.width = bgOffsetW;
				gg.x = bgOffsetX;
			}
		}
		else
		{
			g.height = GRoot.inst.height;
			g.width = bgOffsetW;
			g.x = bgOffsetX;
		}
	}
	void abccc(){
	}
	public void AddEventTouch ()
	{
		Stage.inst.onTouchBegin.Add (Stage_Begin);
		Stage.inst.onTouchEnd.Add (Stage_End);
	}

	public void RemoveEventTouch ()
	{
		Stage.inst.onTouchBegin.Remove (Stage_Begin);
		Stage.inst.onTouchEnd.Remove (Stage_End);
	}
	public void AddPView(BaseMediator view)
	{
		SetPMask ();
		p.AddChild (view.group);
	}
	public void RemovePView (BaseMediator view)
	{
		p.RemoveChild (view.group, true);
	}
	//全屏特殊
	public void AddTopView (BaseMediator view)
	{
		f.AddChild (view.group);
	}

	public void RemoveTopView (BaseMediator view)
	{
		f.RemoveChild (view.group, true);
	}

	public void ShowInviteAlert (Dictionary<string,object> data)
	{
		List<object[]> uids = ModelManager.inst.fightModel.inviteUid;
		object[] o = uids.Find ((object[] obj) =>
		{
			if (obj [0].ToString () == data ["uid"].ToString ())
				return true;
			return false;
		});
		if (o != null)
			return;
		MediatorInvoteAlert ma = new MediatorInvoteAlert ();
		ma.x = 0;
        //ma.y = GRoot.inst.height - ma.height - 200;
        ma.y = GRoot.inst.height - ma.height - 20;
		f.AddChild (ma.group);
		ma.SetData (data);
		EffectManager.inst.ShowViewLeft (ma.group);
//		uids.Add (data ["uid"].ToString ());
		uids.Add (new object[]{ data ["uid"].ToString (), ma });
		ModelManager.inst.fightModel.inviteUid = uids;
	}

	public void CloseInviteAlert (string uid)
	{
		List<object[]> uids = ModelManager.inst.fightModel.inviteUid;
		object[] o = uids.Find ((object[] obj) =>
		{
			if (obj [0].ToString () == uid)
				return true;
			return false;
		});
		if (o != null)
		{
			uids.Remove (o);
		}
		f.RemoveChild ((o [1] as BaseMediator).group, true);
	}

	public void CloseInviteAlertAll ()
	{
		List<object[]> uids = ModelManager.inst.fightModel.inviteUid;
		uids.Find ((object[] obj) =>
		{
			f.RemoveChild ((obj [1] as BaseMediator).group, true);
			return false;
		});
		uids.Clear ();
	}

	public void AddTouchTip (string type, GObject name, object data)
	{
		bool ok = false;
		object[] o;
		for (int i = 0; i < touchs.Count; i++)
		{
			o = (object[])touchs [i];
			if ((o [1] as GObject).parent == null)
			{
				touchs.RemoveAt (i);
				i--;
				continue;
			}
			if (o [0].ToString () == type && o [1] == name)
			{
				o [1] = name;
				o [2] = data;
				ok = true;
				break;
			}
		}
		if (!ok)
			touchs.Add (new object[]{ type, name, data });
	}

	private void Stage_Begin ()
	{		
		bool isOk = ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_TIP);
		if (!isOk)
			return;
		if (Stage.inst.touchCount > 1) {
			return;
		}
		DisplayObject ds = Stage.inst.touchTarget;
		if (ds == null || ds.gOwner == null)
			return;
		object[] n = touchs.Find ((object[] obj) =>
		{	
			if (obj [0].ToString () == ds.gOwner.ToString ().Replace ("FairyGUI.", ""))
			{
				if (obj [1] == ds.gOwner)
				{
					return true;
				}
				return false;
			}
			else if (obj [0].ToString () == ds.gOwner.parent.ToString () && obj [1] == ds.gOwner.parent)
				return true;
			return false;
		});
		if (n != null)
		{
			Vector2 v2 = Stage.inst.touchPosition;
			v2 = f.GlobalToLocal (v2);
			ModelManager.inst.alertModel.data = n [2];
			ModelManager.inst.alertModel.type = n [0].ToString ();
			ModelManager.inst.alertModel.isTip = true;
			if (touch != null && touch.parent != null) {
				f.RemoveChild (touch.group);
			}
			touch = null;
			touch = new ComTip ();
			f.AddChild (touch.group);
			touch.group.x = v2.x;
			touch.group.y = v2.y;
			touch.UpdatePos ();
			float xx = touch.group.x + touch.group.width;
			float yy = touch.group.y + touch.group.height;
			if (touch.group.x < 0)
				touch.group.x = 0;
			if (touch.group.y < 30)
				touch.group.y = 30;
			float gw = GRoot.inst.width;
			float gh = GRoot.inst.height;
			if (xx > gw)
				touch.group.x -= xx - gw;
			if (yy > gh)
				touch.group.y -= yy - gh;
		}
	}

	private void Stage_End ()
	{
		if (touch != null)
		{
			f.RemoveChild (touch.group);
			touch = null;
			ModelManager.inst.alertModel.isTip = false;
		}
	}
	private void PMask_Click()
	{
		p.RemoveChildren (0,-1,true);
		pmask = null;
	}
	private void Mask_Click ()
	{
        if (!ModelRole.isCloseMask)
        {
            if (views.Count != 0)
            {
                BaseMediator bm = views[views.Count - 1];
                if (!bm.isAutoClose)
                    return;
            }
            //		mask.alpha = 1;
            this.CloseView();
        }
        else
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("13164"));
        }
		
	}

	public void Clear ()
	{
		if (scene != null)
		{
			s.RemoveChild (scene.group, true);
			scene = null;
		}
		v.RemoveChildren (0,-1,true);
		s.RemoveChildren (0,-1,true);
		f.RemoveChildren (0,-1,true);
		GuideManager.inst.Clear ();
		views.Clear ();
		scenes.Clear ();
		mask = null;
		sceneMask = null;
		alert = null;
	}

	public void ShowScene <T> (bool isRemove = false)
	{		
		if (sceneMask == null)
		{
			sceneMask = Tools.GetComponent (Config.COM_SCENEMASK).asCom;
			s.AddChild (sceneMask);
			ViewManager.SetWidthHeight (sceneMask);
		}
		scenes.Add (typeof(T));
		if (isRemove)
		{
			if (scenes.Count != 0)
				scenes.RemoveAt (scenes.Count - 1);
		}
		if (scene != null)
		{
			s.RemoveChild (scene.group, true);
			scene = null;
		}
		this.ClearViews ();

		BaseMediator view = Activator.CreateInstance<T> () as BaseMediator;
		scene = view;
		s.AddChild (view.group);
		EffectManager.inst.TweenAlpha (view.group, 0.3f, 0.5f, 1f);
	}

	public void CloseScene (int removeCount = 1,bool isBack = false)
	{
		if (scene != null)
		{
			s.RemoveChild (scene.group, true);
			scene = null;
            int index = removeCount;
            if (isBack)
            {
                index = scenes.Count - removeCount;
            }            
            while (index > 0)
            {
                if (scenes.Count == 0)
                    break;
                scenes.RemoveAt(scenes.Count - 1);
                --index;
            }            
		}
		if (scenes.Count != 0)
		{
			Type type = scenes [scenes.Count - 1];
			BaseMediator view = Activator.CreateInstance (type) as BaseMediator;
			scene = view;
			s.AddChild (view.group);
		}
	}

	private void ClearViews ()
	{
		if (views.Count == 0)
			return;
		views.ForEach ((BaseMediator o) =>
		{
			v.RemoveChild (o.group, true);
			o = null;
		});
		views.Clear ();
		SetMask ();
	}

	public BaseMediator ShowView <T> (bool isEffect = true,bool ifMask = true)
	{				
		BaseMediator view = Activator.CreateInstance<T> () as BaseMediator;
        views.Add (view);
		v.AddChild (view.group);
		SetMask();
		if(!ifMask)
		{
			mask.alpha = 0;
        }
        else
        {
            mask.alpha = 1;
        }

		if (isEffect)
		{
			GRoot.inst.touchable = false;
			EffectManager.inst.ShowView (view.group, () =>
			{
				GRoot.inst.touchable = true;
				view.group.InvalidateBatchingState();
			});
		}
		return view;
	}

	public void CloseView (BaseMediator view = null)
	{
//		s.visible = true;
		if (views.Count == 0)
			return;
		BaseMediator b;
		if (view == null)
		{
			b = views [views.Count - 1];
			views.RemoveAt (views.Count - 1);
			v.RemoveChild (b.group, true);
		}
		else
		{
			for (int i = 0; i < views.Count; i++)
			{
				if (views [i] == view)
				{
					b = view;
					views.RemoveAt (i);
					v.RemoveChild (b.group, true);
					break;
				}
			}		
		}
		SetMask ();
	}

	public void ShowWait ()
	{
		waitCount++;
//		Log.debug ("waitcount++ - " + waitCount.ToString ());
		if (wait != null)
			return;
		wait = new ComLoad ();
		GRoot.inst.AddChild (wait.group);
	}

	public void CloseWait ()
	{		
		waitCount--;
//		Log.debug ("waitcount-- - " + waitCount.ToString ());
		if (waitCount <= 0 && wait != null)
		{
			GRoot.inst.RemoveChild (wait.group, true);
			wait = null;
			waitCount = 0;
		}
	}
	private void SetPMask ()
	{
		if (pmask != null)
		{			
			p.AddChildAt (pmask, 0);
		}
		else
		{
			pmask = Tools.GetComponent (Config.COM_MASK).asCom;
			pmask.onClick.Add (PMask_Click);
			p.AddChildAt (pmask, 0);
			pmask.GetChild ("n0").alpha = 0f;
			ViewManager.SetWidthHeight (pmask);
//			SetTMask ();
		}
	}
	private void SetTMask ()
	{
		if (tmask != null)
		{			
			tmask.visible = t.numChildren < 2 ? false : true;
			t.AddChildAt (tmask, 0);
		}
		else
		{
			tmask = Tools.GetComponent (Config.COM_MASK).asCom;
			t.AddChildAt (tmask, 0);
			tmask.GetChild ("n0").alpha = 0.6f;
			ViewManager.SetWidthHeight (tmask);
			SetTMask ();
		}
	}

	private void SetMask ()
	{
//		Debug.LogError ("SetMask "+views.Count);
		if (mask != null)
		{
			if (views.Count == 0)
			{
				mask.visible = false;
			}
			else
			{
				mask.visible = true;
				v.AddChildAt (mask, views.Count - 1);
			}
		}
		else
		{
			mask = Tools.GetComponent (Config.COM_MASK).asCom;
			v.AddChildAt (mask, 0);
			mask.onClick.Add (Mask_Click);
			mask.GetChild ("n0").alpha = 0.6f;
			ViewManager.SetWidthHeight (mask);
			SetMask ();
		}
	}

	public void ShowAlert (string text, ModelAlert.AlertEvent fun = null, bool isYesAndNo = false,bool close = false)
	{		
		ModelAlert model = ModelManager.inst.alertModel;
		model.text = text;
		model.onAlert = fun;
		model.isYesAndNo = isYesAndNo;
		model.close = close;
		model.showType = 0;

		CloseAlert ();
		alert = new ComAlert ();
		t.AddChild (alert.group);
		SetTMask ();

		EffectManager.inst.ShowView (alert.group);
	}

	//需要消耗多少
	public void ShowAlert (string id, int count, ModelAlert.AlertEvent fun = null)
	{
		ModelAlert model = ModelManager.inst.alertModel;
		model.text = "";
		model.onAlert = fun;
		model.isYesAndNo = true;
		model.id = id;
		model.count = count;
		model.showType = 1;

		CloseAlert ();
		alert = new ComAlert ();
		t.AddChild (alert.group);
		SetTMask ();

		EffectManager.inst.ShowView (alert.group);
	}

	public void ShowAlertLike (string id, int count, ModelAlert.AlertEvent fun = null)
	{
		ModelAlert model = ModelManager.inst.alertModel;
		model.onAlert = fun;
		model.id = id;
		model.count = count;
		CloseAlert ();
		if (model.isOpen)
		{
			alertLike = new ComAlertLike ();
			t.AddChild (alertLike.group);
			SetTMask ();
			EffectManager.inst.ShowView (alertLike.group);
		}
		else
		{
			model.execute (1);
		}
        

	}


	public void CloseAlert ()
	{
		if (alert != null)
		{
			t.RemoveChild (alert.group, true);
			SetTMask ();
		}
		if (alertLike != null)
		{
			t.RemoveChild (alertLike.group, true);
			SetTMask ();
		}
	}

    public void ShowText(string text) {
        GComponent t = Tools.GetComponent(Config.COM_TEXT).asCom;
        t.Center();
        t.GetChild("n1").text = text;
        t.touchable = false;
        float sy = GRoot.inst.height;
        //		t.alpha = 1f;
        //t.y = sy * 0.5f;//- t.height-40;//GRoot.inst.height;// - 100;
        t.y = sy * 0.5f - t.height;
		GRoot.inst.AddChild (t);
		float pos = t.y - 100;
//		float pos = t.y - t.height-20;

        /*
		t.TweenMoveY (pos, 0.5f).OnComplete (() =>
		{
//			GRoot.inst.RemoveChild (t, true);
				t.TweenMoveY (t.y-50, 0.3f).SetDelay (1f).OnComplete(()=>{GRoot.inst.RemoveChild (t, true);});
		});
        */
        t.TweenMoveY(t.y-100, 0.5f).OnStart(()=> {
            t.TweenFade(0, 0.5f).OnComplete(()=> { GRoot.inst.RemoveChild(t, true); });
        }).SetDelay(1f);

//		DOTween.To(()=>t.alpha,x=>t.alpha=x,1f,0.5f).OnComplete(()=>{
//			DOTween.To(()=>t.alpha,x=>t.alpha=x,1f,0.2f).SetDelay(1.5f).OnComplete(()=>{
//				GRoot.inst.RemoveChild (t, true);
//			});
//		});
	}

	//type = 0卡组里面的一般用不到，1你所拥有的卡，2通用查看,3别人的卡,4看自己的卡
	public void ShowCardInfo (string cid, int type = 2, int lv = -1)
	{
		MediatorItemShipInfo.CID = cid;
		MediatorItemShipInfo.isKu = type;
		MediatorItemShipInfo.lv = lv;
		ViewManager.inst.ShowView<MediatorItemShipInfo> ();
	}

	public void ShowReward (object data)
	{
		if (data == null)
			return;
		ComReward cr = new ComReward ();
		cr.SetData (data);
		views.Add (cr);
		v.AddChild (cr.group);
		SetMask ();
		EffectManager.inst.ShowView (cr.group);
	}

	public void ShowGift (object data, string effect = Config.EFFECT_EGG103)
	{
		if (data == null)
			return;
		MediatorGiftShow cr = new MediatorGiftShow ();
		cr.SetData (data, effect);
		views.Add (cr);
		v.AddChild (cr.group);
		SetMask ();
//		EffectManager.inst.ShowView (cr.group);
	}

	public void RemoveIcon ()
	{
		for (int i = 0; i < f.numChildren; i++)
		{
			f.RemoveChild (f.GetChildAt (i));
		}
	}

	public void ShowIcon (object obj, Action fun = null,GObject obj1=null)
	{
		bool isOk = ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_ASSET);
		if (!isOk)
			return;
		Dictionary<string,object> dic = obj as Dictionary<string,object>;
		if (dic.Keys.Count == 0)
		{
			if (fun != null)
				fun ();
			return;
		}
        Vector2 v2;
        if (obj1 == null)
        {
//            Debug.Log(Stage.inst.touchPosition.x + "ccc" + Stage.inst.touchPosition.y);
             v2 = Stage.inst.touchPosition;
            v2 = f.GlobalToLocal(v2);
        }
        else
        {
            //190ccc592
            //v2 = obj1.LocalToGlobal(new Vector2(obj1.x, obj1.y));
            v2 = new Vector2(obj1.x+obj1.width,obj1.y+obj1.height);

        }

        
		if (tween_ShowIcon != null)
		{
			DOTween.Kill (tween_ShowIcon);
			if (showIconFun != null)
				showIconFun ();
			tween_ShowIcon = null;
		}
		if (showIcon == null)
		{
			showIconFun = fun;
			showIcon = new ComGoldCoinExp ();
			f.AddChild (showIcon.group);
			showIcon.group.y = 0;// showIcon.group.height / 2;
			showIcon.group.alpha = 0;
		}
		else
		{
			showIconFun = fun;
		}
		showIcon.group.TweenFade (1f, 0.3f).OnComplete(()=>{showIcon.group.InvalidateBatchingState();});
		showIcon.RemoveGlobalListener (MainEvent.JUMP_OVER, ShowIconOver);
		showIcon.AddGlobalListener (MainEvent.JUMP_OVER, ShowIconOver);
		int index = 0;
		foreach (string i in dic.Keys)
		{
			string id = i;
			int _index = index;
			if (i == Config.ASSET_GOLD || i == Config.ASSET_COIN || i == Config.ASSET_EXP || i == Config.ASSET_CARD) {
				if (i == Config.ASSET_CARD) {
					int num = 0;
					Dictionary<string,object> card = (Dictionary<string,object>)dic [i];
					foreach (string str in card.Keys) {
						num += (int)card [str];
					}
					TimerManager.inst.Add (0.2f * _index, 1, (float time) => {
						EffectManager.inst.ShowIcon (id, num, f, new Vector2 (v2.x, v2.y), new Vector2 (569f, 320f));
					});

				} else {
					TimerManager.inst.Add (0.2f * _index, 1, (float time) => {
						EffectManager.inst.ShowIcon (id, (int)dic [id], f, new Vector2 (v2.x, v2.y), new Vector2 (569f, 320f));
					});
				}
				index++;
			}
		}
	}

	public void ShowIconOver (MainEvent e = null)
	{
		showIcon.RemoveGlobalListener (MainEvent.JUMP_OVER, ShowIconOver);
		tween_ShowIcon = 1;
		showIcon.group.TweenFade (0f, 0.3f).OnComplete (() =>
		{
			if (showIconFun != null)
				showIconFun ();
			f.RemoveChild (showIcon.group, true);
			showIcon = null;
			tween_ShowIcon = null;
//			f.InvalidateBatchingState(true);
//				s.InvalidateBatchingState(true);
				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.USER_UPDATE_UI));
		}).SetId (tween_ShowIcon);
	}

	public void ShowMessage (string name)
	{
        if(showMessage == null) {
            showMessage = Tools.GetComponent(Config.COM_MESSAGE).asCom;
        } else {
            messages.Add(name);
            return;
        }
        //showMessage.sortingOrder = 1;
        showMessage.x = ViewManager.bgOffsetX;
        showMessage.width = ViewManager.bgOffsetW;
		showMessage.GetChild ("n1").text = name;
		showMessage.y = -showMessage.height;
		float c = showMessage.y;
		f.AddChild (showMessage);

		showMessage.TweenMove (new Vector2 (showMessage.x, 0f), 0.5f).OnComplete (() =>
		{
            showMessage.InvalidateBatchingState();

            showMessage.TweenFade (0f, 1f).SetDelay (3f).OnComplete (() =>
			{	
				f.RemoveChild (showMessage, true);
				showMessage = null;
				if (messages.Count != 0)
				{
					string msg = messages [0];
					messages.RemoveAt (0);
					this.ShowMessage (msg);
				}
			});
		});	
	
	}

	public void ShowGetElScore (int addNum, Vector2 v2)
	{
//		Vector2 v2 = new Vector2 (100f, 100f);
		EffectManager.inst.ShowIcon (Config.ASSET_ELSCORE, addNum, f, new Vector2 (v2.x, v2.y), new Vector2 (569f, 100f));
	}

    public void ShowGetRedBagCoin(int addNum,Vector2 v2) 
    {
        //Debug.LogError("aaaaaaaaaaaaaa"+v2);
        v2 = f.GlobalToLocal(v2);
        TimerManager.inst.Add(0.3f, 1, (float time) => { 
            EffectManager.inst.ShowIcon(Config.ASSET_REDBAGCOIN, addNum, f, v2, new Vector2(569f, 100f));
        });
    }
	//有界面上来需要请求的需要调用一下
	public void ShowViewCallLaterRequest (Action fun)
	{
		int c = 0;
		DOTween.To (() => c, x => c = x, 1, 0.2f).OnComplete (() =>
		{
			if (fun != null)
				fun ();
		});
	}

	public static LoadScenes LoadScene (string name, Action<float> fun,UnityEngine.SceneManagement.LoadSceneMode _mode = UnityEngine.SceneManagement.LoadSceneMode.Additive)
	{
		LoadScenes ls = LoadScenes.Get ();
		ls.Load (name, fun,_mode);
		return ls;
	}

	public static void RemoveScene (string scene)
	{
		UnityEngine.SceneManagement.SceneManager.UnloadScene (scene);
	}

}

public class LoadScenes:MonoBehaviour
{
	private AsyncOperation ao;
	private string scene;
	private Action<float> fun;
	private UnityEngine.SceneManagement.LoadSceneMode mode;
	public LoadScenes ()
	{
	}

	public static LoadScenes Get ()
	{
		GameObject go = new GameObject ("LoadScene");
		return go.AddComponent<LoadScenes> ();
	}

	public void Load (string scene, Action<float> fun,UnityEngine.SceneManagement.LoadSceneMode _mode = UnityEngine.SceneManagement.LoadSceneMode.Additive)
	{
		this.mode = _mode;
		this.scene = scene;
		this.fun = fun;
		this.StartCoroutine (Load_Scene ());
	}

	private IEnumerator Load_Scene ()
	{
		ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (this.scene, this.mode);
		yield return ao;
	}

	void Update ()
	{
		if (ao != null && this.fun != null)
			this.fun (ao.progress);
	}

	public void Clear ()
	{
		this.StopCoroutine (Load_Scene ());
		Tools.Clear (this.gameObject);
	}
}