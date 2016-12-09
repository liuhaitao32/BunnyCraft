using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class MediatorEffort : BaseMediator
{
	private GTextField effort_name;
	private GButton btn_esc;
	private GButton btn_xx;
	private Controller c1;
	private GList list;
	private GList listxx;
	private ComProgress pro;

	private ModelUser userModel;
	private Dictionary<string,object> effortCfg;
	private List<object> lisData;
	private float weizhi;

	private bool isUP = false;
	private int beforeEff_Lv;
	private GLoader ball;

	private GameObject gameobj;
	private bool initXX = true;

	private int nowIndex;
	private Dictionary<string,object> nowGift;
	private GObject moveGo;
	private bool donghuaXX = false;
	private object[] lisXXda;
	private int[] addList;

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_EFFORT, true);

		userModel = ModelManager.inst.userModel;
		effortCfg = (Dictionary<string,object>)(DataManager.inst.effort ["mission"]);

		this.GetChild ("n3").asTextField.text = Tools.GetMessageById ("23007");
		lisData = new List<object> ();
		int proValue = 0;
		for (int i = 0; i < userModel.effort.Length; i++)
		{
			Dictionary<string,object> da = (Dictionary<string,object>)userModel.effort [i];
			Dictionary<string,object> dacfg = (Dictionary<string,object>)(effortCfg [da ["eid"].ToString ()]);
			Dictionary<string,object> ddd = new Dictionary<string, object> ();
			ddd.Add ("id", da ["eid"].ToString ());
			ddd.Add ("rate", (int)(da ["rate"]));
			ddd.Add ("index", i);
			ddd.Add ("name", dacfg ["name"].ToString ());
			ddd.Add ("info", dacfg ["info"].ToString ());
			ddd.Add ("type", dacfg ["type"].ToString ());
			ddd.Add ("need", (object[])dacfg ["need"]);
			ddd.Add ("status", (int)(da ["status"]));
			ddd.Add ("reward", (Dictionary<string,object>)dacfg ["reward"]);
			int need = (int)((object[])ddd ["need"]) [(((object[])ddd ["need"]).Length) - 1];
			if (((int)ddd ["status"] == 0) && lisData.Count < 4)
			{
				lisData.Add (ddd);
			}
			if ((int)ddd ["status"] == 1)
			{
				proValue++;
			}
		}
		beforeEff_Lv = userModel.effort_lv;
		ball = this.GetChild ("n19").asLoader;
		effort_name = this.GetChild ("n18").asTextField;
		btn_esc = this.GetChild ("n7").asButton;
		c1 = this.GetController ("c1");
		c1.onChanged.Add (OnC1Change);
		btn_xx = this.GetChild ("n17").asButton;
		list = this.GetChild ("n4").asList;
		pro = this.GetChild ("n12") as ComProgress;
		listxx = this.GetChild ("n14").asList;
//		listxx.onTouchBegin.Add (OnBeginDrag);
//		listxx.onTouchEnd.Add (OnEndDrag);

		listxx.itemRenderer = OnRenderListXX;
		listxx.SetVirtual ();

		EffectManager.inst.EffectAlpha (this.GetChild ("n30").asImage, 0);
		EffectManager.inst.EffectAlpha (this.GetChild ("n31").asImage, 0);

		string sid = Tools.GetEffortBuildID ("0" + userModel.effort_lv);
		gameobj = EffectManager.inst.AddEffect (sid, "bulid01", btn_xx.GetChild ("n2").asGraph,null,false,50,null,true);
		gameobj.transform.localPosition += new Vector3 (0f, 70, 0f);
//		GameObjectScaler.Scale (gameobj, 0.8f);
		gameobj.transform.localScale *= 0.9f;
//        Debug.Log(gameobj.transform.localScale);
//		list.itemRenderer = OnRenderList;
//		list.numItems = lisData.Count;
		FrushList ();
		if (userModel.effort_lv>((Dictionary<string,object>)DataManager.inst.effort["effort_cond"]).Count) {
			c1.selectedIndex = 2;
		}
//		this.GetChild ("n26").asTextField.text = userModel.effort_lv + "";

		effort_name.text = Tools.GetEffortName (userModel.effort_lv);
		btn_esc.onClick.Add (OnEscBtnClick);
		btn_xx.onClick.Add (OnChangeXX);
		btn_esc.text = Tools.GetMessageById ("24111");
		weizhi = pro.viewWidth / 2 + pro.x;
		ball.visible = false;

		pro.SetMax (userModel.effort.Length);
		pro.SetValue (proValue);
		pro.x = weizhi - pro.viewWidth / 2;
		if (ModelManager.inst.guideModel.CheckEffort () == 2)
		{
			if (GuideManager.inst.Check ("100:0"))
			{
				GuideManager.inst.Next ();
				GuideManager.inst.Show(this);
			}
		}

//		if (guideModel.CheckEffort () == 2 && GuideManager.inst.Check ("100:0"))
//		{
//			GuideManager.inst.Next(true);
//		}
	}

	private void OnC1Change ()
	{
		if(c1.selectedIndex == 0)
		{
			this.GetChild("n21").asImage.x = Tools.offectSetX(106);
		}else if(c1.selectedIndex == 2){
			this.GetChild("n21").asImage.x = Tools.offectSetX(370);
		}	
		if (c1.selectedIndex == 1)
		{
			if (initXX)
			{
				initXX = false;
				listXXInit ();
			}
		}
		if (items == null)
			return;
		for (int i = 0; i < items.Count; i++)
		{
			if (c1.selectedIndex == 0)
			{
				items [i].visible = true;
			}
			else
			{
				items [i].visible = false;
			}
		}
	}

	private List<GComponent> items;

	private void FrushList ()
	{
		if (items != null)
		{
			foreach (GComponent s in items)
			{
				this.RemoveChild (s, true);
			}
		}
		items = new List<GComponent> ();
		for (int i = 0; i < lisData.Count; i++)
		{
			GComponent go = Tools.GetComponent (Config.ITEMEFFORT).asCom;
			go.name = "s" + i;
			this.AddChild (go);
			OnRenderList (i, go);
			items.Add (go);
			go.x = list.x;
			go.y = list.y + (167 + 20) * i;
			if (i == 3)
			{
				go.alpha = 0;
			}
		}
		this.SetChildIndex (ball, this.numChildren - 1);
	}

	private void MoveList ()
	{
		if (items == null)
			return;
		for (int i = 0; i < items.Count; i++)
		{
			if (i < 3)
			{
				items [i].alpha = 1;
			}
			items [i].TweenMoveY (list.y + (170 + 20) * i, 0.5f);
		}
	}

	private void OnRenderList (int index, GObject go)
	{
		go.alpha = 1;
		go.scale = new Vector2 (1f, 1f);
		Dictionary<string,object> _da = (Dictionary<string,object>)lisData [index];
		ComBigIcon icon = go.asCom.GetChild ("n8") as ComBigIcon;
		ComProgressBar progress = go.asCom.GetChild ("n7") as ComProgressBar;
		GTextField name = go.asCom.GetChild ("n1").asTextField;
		GTextField info = go.asCom.GetChild ("n3").asTextField;
		GButton btn = go.asCom.GetChild ("n0").asButton;
		Controller c1 = go.asCom.GetController ("c1");
		GImage bg = go.asCom.GetChild ("n10").asImage;
		name.text = Tools.GetMessageById (_da ["name"].ToString ());
		info.text = Tools.GetMessageById (_da ["info"].ToString (), GetTypeByObj (_da ["type"].ToString (), (object[])_da ["need"]));
		progress.skin = ComProgressBar.BAR5;
		progress.offsetY = 5;
//		ColorFilter gggg = btn.GetChild ("n0").asImage.filter as ColorFilter;
//		gggg.Reset ();


		bool isCard = false;
		Dictionary<string,object> gfit = new Dictionary<string, object> ();
		string giftType = "";
		foreach (string str in ((Dictionary<string,object>)_da["reward"]).Keys)
		{
			if (str == Config.ASSET_CARD)
			{
				isCard = true;
			}
			giftType = str;
			gfit.Add (str, ((Dictionary<string,object>)_da ["reward"]) [str]);
			break;
		}
		if (isCard)
		{
			icon.SetSelectIndex (1);
			icon.SetData (((object[])gfit [Config.ASSET_CARD]) [0].ToString (), ((object[])gfit [Config.ASSET_CARD]) [1].ToString (), 4);
		}
		else
		{
			icon.SetSelectIndex (2);
			icon.SetData (giftType, gfit [giftType]);
		}
		progress.max = (int)((object[])_da ["need"]) [(((object[])_da ["need"]).Length) - 1];
		progress.value = (int)(_da ["rate"]);

		btn.RemoveEventListeners ();
		bool isclick = false;
		btn.onTouchBegin.Add (() =>
		{
			isclick = true;
			go.scale = new Vector2 (0.95f, 0.95f);
		});
		btn.onTouchEnd.Add (() =>
		{
			isclick = false;
			go.scale = new Vector2 (1f, 1f);
		});
		btn.onRollOut.Add (() =>
		{
			if (isclick)
			{
				go.scale = new Vector2 (1f, 1f);
			}
		});
		go.asCom.GetChild ("n9").asTextField.text = Tools.GetMessageById ("23012");
		progress.GetChild ("n1").asTextField.color = Tools.GetColor ("E8FCD9");
		if (progress.value >= progress.max)
		{
			btn.touchable = true;
			c1.selectedIndex = 1;
			EffectManager.inst.TweenHuXi (go.asCom.GetChild ("n9").asTextField, 3f);
			bg.color = Tools.GetColor ("#FFFF99");
//			EffectManager.inst.SetColorShader (btn.displayObject.gameObject, "FFFF00");
//			gggg.AdjustBrightness (0.1f);
//			gggg.AdjustContrast (0.5f);
//			gggg.AdjustSaturation (0.5f);
//			gggg.AdjustHue (-0.2f);
		}
		else
		{
			btn.touchable = false;
//			btn.GetController ("c1").selectedIndex = 0;
			c1.selectedIndex = 0;
		}
		btn.onClick.Add (() =>
			{
                SoundManager.inst.PlaySound(Config.SOUND_MISSIONREWARD);
                btn.touchable = false;
				this.touchable = false;
				moveGo = go;
				nowGift = gfit;
				nowIndex = index;

				if (ModelManager.inst.guideModel.CheckEffort () == 2 && items.Count == 1)
				{
					if (GuideManager.inst.Check ("100:1"))
					{
						GuideManager.inst.Next ();
					}
				}
				NetHttp.inst.Send (NetBase.HTTP_GETEFFORTREWARD, "index=" + (int)(_da ["index"]), GetReward);
			});
	}

	private object[] GetTypeByObj (string type, object[] da)
	{
		object[] data = (object[])Tools.Clone (da);
		switch (type)
		{
		case "use_card":	
			data [0] = CardVo.GetCardName ((string)da [0]);
			break;
		case "have_card_group":
			data [0] = CardVo.GetRarityMss ((int)da [0]);
			break;
		case "level_card_group":
			data [0] = CardVo.GetRarityMss ((int)da [0]);
			break;
		}
		return data;
	}
	private bool back = true;
	private void GetReward (VoHttp vo)
	{
		float timessssss = 0.3f;
		if (!nowGift.ContainsKey (Config.ASSET_BODY)) {
			back = false;
			ViewManager.inst.ShowIcon (nowGift, () =>
			{
				userModel.UpdateData (vo.data);
				back = true;
			});
		}

		EffectManager.inst.SetFilterAdjustBrightness (moveGo, timessssss, 0f,1f);
		moveGo.TweenScale (new Vector2 (1.15f, 1.15f), timessssss).OnComplete (() =>
		{
            EffectManager.inst.SetFilterAdjustBrightness(moveGo, timessssss, 1f, 0f);
            moveGo.TweenScale (new Vector2 (0.7f, 0.7f), 0.4f);
			ball.width = moveGo.width;
			ball.height = moveGo.height;
			ball.x = moveGo.x;
			ball.y = moveGo.y;
			float c = 1;
			DOTween.To (() => c, x => c = x, 0, 0.4f).OnUpdate (() =>
			{
				moveGo.alpha = c;
			}).OnComplete (() =>
			{
				items.Remove (moveGo.asCom);
				this.RemoveChild (moveGo);
				MoveList ();
			});
			ball.TweenScale (new Vector2 (0.7f, 0.7f), 0.4f);
			ball.visible = true;
			ball.alpha = 0;
			float b = ball.width;
			DOTween.To (() => b, x => b = x, 100, 0.4f).OnUpdate (() =>
			{
				ball.width = b;
			});
			float d = ball.height;
			DOTween.To (() => d, x => d = x, 100, 0.4f).OnUpdate (() =>
			{
				ball.height = d;
			}).OnComplete (() =>
			{
				ball.TweenScale (new Vector2 (0.3f, 0.3f), timessssss);
			});
			float f = 0;
			DOTween.To (() => f, x => f = x, 1, 0.7f).OnUpdate (() =>
			{
				ball.alpha = f;
			}).OnComplete (() =>
			{
				EffectManager.inst.Bezier (ball, 0.4f, ball.xy, new Vector2 (500f, -100f), new Vector2 (pro.x - 30f + (pro.value * 47f), pro.y - 37f), () =>
				{
					ball.visible = false;
					NetHttp.inst.Send (NetBase.HTTP_GETEFFORT, "", OnChangeEff);
				});
			});
		});			
	}

	private void OnChangeEff (VoHttp vo)
	{
		userModel.UpdateData (vo.data);
		if (beforeEff_Lv != userModel.effort_lv)
		{
			pro.SetValue (pro.max);
			TimerManager.inst.Add (0.5f, 1, (float time) =>
			{
				float c = 1;
				DOTween.To (() => c, x => c = x, 0, 0.3f).OnUpdate (() =>
				{
					effort_name.alpha = c;
					pro.alpha = c;
					this.GetChild ("n21").asImage.alpha = c;
					this.GetChild ("n3").asTextField.alpha = c;
//					this.GetChild("n26").asTextField.alpha = c;
					btn_esc.alpha = c;
				});
				btn_xx.TweenMoveY (170f, 0.5f);
				btn_xx.TweenMoveX (Tools.offectSetX (400f), 0.5f).OnComplete (() =>
				{
                    SoundManager.inst.PlaySound(Config.SOUND_SCIENCELVUP);
					EffectManager.inst.AddPrefab (Config.EFFECT_ELV_UP, this.GetChild ("n20").asGraph);
					EffectManager.inst.SetShaderValue (gameobj, 1.7f, 0f, 1f);
//					EffectManager.inst.SetFilterAdjustBrightness1(btn_xx, 1.8f, 1f);
//					TimerManager.inst.Add(1.8f,1,(float obj)=>{
//						
//					});
					TimerManager.inst.Add (2f, 1, (float obj) =>
					{
						UpEffortMain ();
						EffectManager.inst.SetShaderValue (gameobj, 0f, 1f, 0f);
//						EffectManager.inst.SetFilterAdjustBrightness1(btn_xx, 1f, 0f);
						btn_xx.x = Tools.offectSetX (150f);
						btn_xx.y = 92f;
						effort_name.alpha = 1;
						pro.alpha = 1;
						this.GetChild ("n3").asTextField.alpha = 1;
						this.GetChild ("n21").asImage.alpha = 1;
//						this.GetChild("n26").asTextField.alpha = 1;
					});
				});	
			});
			return;
		}
		UpEffortMain ();
	}

	private void UpEffortMain ()
	{
		lisData = new List<object> ();
		int proValue = 0;
		for (int i = 0; i < userModel.effort.Length; i++)
		{
			Dictionary<string,object> da = (Dictionary<string,object>)userModel.effort [i];
			Dictionary<string,object> dacfg = (Dictionary<string,object>)(effortCfg [da ["eid"].ToString ()]);
			Dictionary<string,object> ddd = new Dictionary<string, object> ();
			ddd.Add ("id", da ["eid"].ToString ());
			ddd.Add ("rate", (int)(da ["rate"]));
			ddd.Add ("index", i);
			ddd.Add ("name", dacfg ["name"].ToString ());
			ddd.Add ("info", dacfg ["info"].ToString ());
			ddd.Add ("need", (object[])dacfg ["need"]);
			ddd.Add ("status", (int)(da ["status"]));
			ddd.Add ("type", dacfg ["type"].ToString ());
			ddd.Add ("reward", (Dictionary<string,object>)dacfg ["reward"]);
			int need = (int)((object[])ddd ["need"]) [(((object[])ddd ["need"]).Length) - 1];
			if (((int)ddd ["status"] == 0) && lisData.Count < 3)
			{
				lisData.Add (ddd);
			}
			if ((int)ddd ["status"] == 1)
			{
				proValue++;
			}
		}
//		list.numItems = lisData.Count;
		FrushList ();
		effort_name.text = Tools.GetEffortName (userModel.effort_lv);

		pro.SetMax (userModel.effort.Length);
		pro.SetValue (proValue);
		pro.x = weizhi - pro.viewWidth / 2;

//		this.GetChild ("n26").asTextField.text = userModel.effort_lv + "";
		if (ModelManager.inst.guideModel.CheckEffort () == 2) {
			if (GuideManager.inst.Check ("100:1")) {
				GuideManager.inst.Show (this);
			}
		}
		if (beforeEff_Lv != userModel.effort_lv)
		{
			isUP = true;
			beforeEff_Lv = userModel.effort_lv;
			string sid = Tools.GetEffortBuildID ("0" + beforeEff_Lv);
			gameobj = EffectManager.inst.AddEffect (sid, "bulid0" + beforeEff_Lv, btn_xx.GetChild ("n2").asGraph,null,false,50,null,true);
			gameobj.transform.localPosition += new Vector3 (0f, 70, 0f);
//			GameObjectScaler.Scale (gameobj, 0.8f);
			gameobj.transform.localScale *= 0.9f;
            Debug.Log(gameobj.transform.localScale);
            c1.selectedIndex = 1;
			donghuaXX = true;
			curIndex = beforeEff_Lv - 1;
			listxx.numItems = lisXXda.Length;
			listxx.ScrollToView (beforeEff_Lv - 1);
//			listxx.numItems = lisXXda.Length;
		}
		else
		{
			this.touchable = true;
		}

//		if (ModelManager.inst.guideModel.CheckEffort () == 2)
//		if (GuideManager.inst.Check ("100:1"))
//			GuideManager.inst.Show (this);
//		else
//		{
////			if (GuideManager.inst.GetData (GuideManager.GUIDE_EXIST) [0] == 100)
////			{				
//			GuideManager.inst.ClearGuide();
////			}
//		}			
	}

	private void listXXInit ()
	{
		Dictionary<string,object> effCfg = (Dictionary<string,object>)(DataManager.inst.effort ["effort_cond"]);
		Dictionary<string,object> cardCfg = DataManager.inst.card;

		int fffff = effCfg.Count + 1;
		if (userModel.effort_lv > effCfg.Count + 1) {
			fffff = userModel.effort_lv;
		}

		addList = new int[fffff];
		lisXXda = new object[fffff];

		for(int j = 0;j<addList.Length;j++)
		{
			addList [j] = 0;
		}
		foreach (string i in cardCfg.Keys)
		{
			int cardeff = (int)(((Dictionary<string,object>)cardCfg [i]) ["effort_lv"]);
			Dictionary<string,object> dic = new Dictionary<string, object> ();
			dic.Add ("id", i);
			dic.Add ("index", (int)((Dictionary<string,object>)cardCfg [i]) ["index"]);
			dic.Add ("name", ((Dictionary<string,object>)cardCfg [i]) ["name"].ToString ());
			if (cardeff <= fffff)
			{
				if (lisXXda [(cardeff - 1)] == null)
				{
					lisXXda [(cardeff - 1)] = new List<object> ();
					(lisXXda [(cardeff - 1)] as List<object>).Add (dic);
				}
				else
				{
					(lisXXda [(cardeff - 1)] as List<object>).Add (dic);
				}
			}
		}
	}

	private float curX;
	private float curX2;
	private int curIndex = 0;

	private void OnBeginDrag ()
	{
//		Log.debug ("start");
		donghuaXX = false;
		curIndex = Convert.ToInt16 (Math.Round (Math.Abs (listxx.container.x) / 1136));
		curX = listxx.container.x;
	}

	private void OnEndDrag ()
	{
//		Log.debug ("end");
		curX2 = listxx.container.x;
		if (curX > listxx.container.x)
		{
			if (curX - listxx.container.x > (250))
			{
				if (curIndex + 1 <= (lisXXda.Length - 1))
				{
					listxx.ScrollToView (curIndex + 1, true);
					curIndex = curIndex + 1;
					listxx.numItems = lisXXda.Length;
//					TweenMove (curIndex + 1);
				}
				else
				{
					listxx.ScrollToView (lisXXda.Length - 1, true);
					curIndex = lisXXda.Length - 1;
					listxx.numItems = lisXXda.Length;
//					TweenMove (lisXXda.Length - 1);
				}
			}
			else
			{
				listxx.ScrollToView (curIndex, true);
				listxx.numItems = lisXXda.Length;
//				TweenMove (curIndex);
			}
		}
		else
		{
			if (listxx.container.x - curX > (250))
			{
				if ((curIndex - 1) < 0)
				{
					listxx.ScrollToView (0, true);
					curIndex = 0;
					listxx.numItems = lisXXda.Length;
//					TweenMove (0);
				}
				else
				{
					listxx.ScrollToView (curIndex - 1, true);
					curIndex = curIndex - 1;
					listxx.numItems = lisXXda.Length;
//					TweenMove (curIndex - 1);
				}

			}
			else
			{
				listxx.ScrollToView (curIndex, true);
				listxx.numItems = lisXXda.Length;
//				TweenMove (curIndex);
			}
		}
//		listxx.ScrollToView (5,true);
	}
	//	private void TweenMove(int index)
	//	{
	//		float c = curX2;
	//		DOTween.To (() => c, x => c = x, index * -1100, 1f).OnUpdate (() => {
	//			listxx.container.x = c;
	//		});
	//	}

	private void OnRenderListXX (int index, GObject go)
	{
		//		listxx.ch
//		Debug.LogError (index + " || "+listxx.GetChildIndex(go) + " || " +listxx.numItems + " ||  "+curIndex);
//		if (btn_xx.touchable)
//			return;
//		if ((index == (curIndex - 1) || index == (curIndex+1) || index == curIndex)&&addList[index] == 0) {
//			addList [index] = 1;
//		} else
//			return;
		GList list = go.asCom.GetChild ("n14").asList;
		GTextField tex = go.asCom.GetChild ("n11").asTextField;
		GButton btn = go.asCom.GetChild ("n12").asButton;
		GTextField effort_lv = go.asCom.GetChild ("n9").asTextField;
		GTextField benji = go.asCom.GetChild ("n13").asTextField;
		GTextField dangqian = go.asCom.GetChild ("n16").asTextField;
		GButton jianzhu = go.asCom.GetChild ("n8").asButton;

//		GImage dangqianImg = go.asCom.GetChild ("n17").asImage;
		GImage n18 = go.asCom.GetChild ("n18").asImage;
//		GComponent n19 = go.asCom.GetChild ("n19").asCom;
		GImage n20 = go.asCom.GetChild ("n20").asImage;
		GImage n21 = go.asCom.GetChild ("n21").asImage;

//		GTextField n23 = go.asCom.GetChild ("n23").asTextField;
		GImage n24 = go.asCom.GetChild ("n24").asImage;
		GImage n25 = go.asCom.GetChild ("n25").asImage;
		GComponent bg = go.asCom.GetChild ("bg").asCom;
//		n23.text = (index + 1) + "";
		btn.touchable = true;
		if (donghuaXX && index == beforeEff_Lv - 1)
		{
			dangqian.alpha = 0;
			effort_lv.alpha = 0;
			benji.alpha = 0;
			btn.alpha = 0;
			tex.alpha = 0;
			btn_esc.alpha = 0;
//			dangqianImg.alpha = 0;
			n18.alpha = 0;
//			n19.alpha = 0;
			n20.alpha = 0;
			n21.alpha = 0;
//			n23.alpha = 0;
			n24.alpha = 0;
			n25.alpha = 0;
			bg.alpha = 0;
			DOTween.Kill (this.GetChild ("n30"), true);
			DOTween.Kill (this.GetChild ("n31"), true);
			this.GetChild ("n30").alpha = 0;
			this.GetChild ("n31").alpha = 0;
			jianzhu.x = 400f;
			TimerManager.inst.Add (2f, 1, (float time) =>
			{
				jianzhu.TweenMoveX (170f, 0.6f).OnComplete (() =>
				{
					float c = 0;
					DOTween.To (() => c, x => c = x, 1, 0.3f).OnUpdate (() =>
					{
						dangqian.alpha = c;
						effort_lv.alpha = c;
						benji.alpha = c;
						btn.alpha = c;
						tex.alpha = c;
						btn_esc.alpha = c;
//						dangqianImg.alpha = c;
						n18.alpha = c;
//						n19.alpha = c;
						n20.alpha = c;
						n21.alpha = c;

//						n23.alpha = c;
						n24.alpha = c;
						n25.alpha = c;
						bg.alpha = c;
						this.GetChild ("n30").alpha = c;
						this.GetChild ("n31").alpha = c;
					}).OnComplete(()=>{
						EffectManager.inst.EffectAlpha (this.GetChild ("n30").asImage, 0);
						EffectManager.inst.EffectAlpha (this.GetChild ("n31").asImage, 0);
					});
				});	
			});
		}
		effort_lv.text = Tools.GetEffortName ((index + 1));//GuideManager
		tex.text = Tools.GetMessageById ("23006");
		if ((index + 1) == userModel.effort_lv)
		{
			benji.text = Tools.GetMessageById ("23002");
			if (isUP)
			{
				dangqian.text = Tools.GetMessageById ("23004");
			}
			else
			{
				dangqian.text = Tools.GetMessageById ("23005");
			}
//			dangqianImg.visible = true;
		}
		else
		{
			benji.text = Tools.GetMessageById ("23003", new object[]{ (index + 1) });
			dangqian.text = "";
//			dangqianImg.visible = false;
		}



		string iii = Tools.GetEffortBuildID ("0" + (index + 1));
		GameObject asd = EffectManager.inst.AddEffect (iii, iii, jianzhu.GetChild ("n2").asGraph,null,false,50,null,true);
        asd.transform.localScale *= 0.9f;
//        Debug.Log(asd.transform.localScale);
		if (userModel.effort_lv < (index + 1))
		{
			EffectManager.inst.SetShaderSaturation (asd, -1);
			EffectManager.inst.StopAnimation (asd);
		}
		list.itemRenderer = (int _index, GObject _ggo) =>
		{
			GObject _go = _ggo.asCom.GetChild ("n0");
			_go.data = _go.x;
			if (donghuaXX && index == beforeEff_Lv - 1)
			{
				_go.alpha = 0;
				TimerManager.inst.Add (2f, 1, (float time) =>
				{
					float hh = 0;
					DOTween.To (() => hh, x => hh = x, 1f, 0.6f + (4f - (_index) % 4f) / 10f).OnUpdate (() =>
					{
						_go.alpha = hh;
					}).OnComplete (() =>
					{
						if ((list.numItems - 1) == _index && beforeEff_Lv == 2)
						{
							if (GuideManager.inst.Check ("100:2"))
							{
								GuideManager.inst.Show (this);
								GuideManager.inst.onClick = OnClick;
											donghuaXX = false;
							}
							else{
								this.touchable = true;
							}
						}else{
							if(beforeEff_Lv !=2)
							{
								this.touchable = true;
											donghuaXX = false;
							}
						}
					});
					_go.x -= (4 - (_index) % 4) * 30 + 100;
					_go.TweenMoveX (Convert.ToSingle (go.data), 0.8f).OnComplete (() =>
					{
						_go.x = Convert.ToSingle (go.data);
					});
				});
			}
			else
			{
				_go.x = Convert.ToSingle (go.data);
				if(!donghuaXX)
				{
					this.touchable = true;
				}
			}
			GComponent iconBtn = _go.asCom;
			string dic = ((Dictionary<string,object>)(((List<object>)lisXXda [index]) [_index])) ["id"].ToString ();
			(_go as ComCard).SetData (dic, -1, 4);
			(_go as ComCard).SetText (Tools.GetMessageById (((Dictionary<string,object>)(((List<object>)lisXXda [index]) [_index])) ["name"].ToString ()));
			iconBtn.RemoveEventListeners ();
			iconBtn.onClick.Add (() =>
			{
				MediatorItemShipInfo2.CID = dic;
				ViewManager.inst.ShowView<MediatorItemShipInfo2> (true);
			});
		};

		if (((List<object>)lisXXda [index]) == null)
		{
			list.numItems = 0;
		}
		else
		{
			Tools.Sort (((List<object>)lisXXda [index]), new string[]{ "index:int:0" });
			list.numItems = ((List<object>)lisXXda [index]).Count;
		}
//		list.onClear.Add (()=>{
//			if(list == null)
//			{
//			}
//		});
//		int hang = list.numItems / 4 + (list.numItems % 4 == 0 ? 0 : 1);
//		if (hang < 3)
//		{
//			list.y = 151 + (196 / 2) * (3 - hang);
//		}
//		else
//		{
//			list.y = 177f;
//		}
		btn.RemoveEventListeners ();
		btn.onClick.Add (() =>
		{
			MediatorEffortXX.curEffort = (index + 1);
			ViewManager.inst.ShowView<MediatorEffortXX> ();
		});
	}
	private void OnClick()
	{
		this.touchable = true;
		GuideManager.inst.onClick = null;

	}
	private void OnShowAtherXX ()
	{
		ViewManager.inst.ShowView<MediatorEffortXX> ();
	}

	private void OnChangeXX ()
	{
		btn_xx.touchable = false;
		c1.selectedIndex = 1;
		curIndex = userModel.effort_lv - 1;
		listxx.numItems = lisXXda.Length;
		listxx.ScrollToView ((userModel.effort_lv - 1));
	}

	private void OnEscBtnClick ()
	{
		if (!back)
			return;
		if (c1.selectedIndex == 1)
		{
//			gameobj = EffectManager.inst.AddEffect (Tools.GetEffortBuildID("0" + userModel.effort_lv), "bulid0" + userModel.effort_lv, btn_xx.GetChild ("n2").asGraph, null, false, 50, null, true);
//			gameobj.transform.localPosition += new Vector3 (0f, 70, 0f);
//			gameobj.transform.localScale *= 0.7f;
			btn_xx.touchable = true;
			if (userModel.effort_lv > ((Dictionary<string,object>)DataManager.inst.effort ["effort_cond"]).Count) {
				c1.selectedIndex = 2;
			} else {
				c1.selectedIndex = 0;
			}

			isUP = false;
			donghuaXX = false;
			listxx.numItems = lisXXda.Length;
			return;
		}
//		listxx.onClear.Add
		ViewManager.inst.CloseScene ();
	}

	public override void Clear ()
	{
//		listxx.onClear.Call ();
		base.Clear ();
	}
}
