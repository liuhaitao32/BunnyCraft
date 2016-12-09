using System;
using FairyGUI;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class EffectManager
{
	public const string PATH_ANIMATE = "Effect/";
	public const string PATH_SHIP = "Prefabs/ship";

	private static EffectManager instance;

	public EffectManager ()
	{
	}

	public static EffectManager inst
	{
		get
		{
			if (instance == null)
				instance = new EffectManager ();
			return instance;
		}
	}

	public void ShowView (GComponent view, Action fun = null)
	{
		view.SetScale (0.98f, 0.98f);
		view.TweenScale (new Vector2 (1, 1), 0.2f).SetEase (Ease.OutQuad).OnComplete (() =>
		{
			if (fun != null)
				fun ();
//			view.InvalidateBatchingState();
		});
	}

	public void ShowViewLeft (GComponent view, Action fun = null)
	{
		float xx = view.width;
		view.x = -xx;
		view.TweenMoveX (0, 0.3f).SetEase (Ease.OutQuad).OnComplete (() =>
		{
			if (fun != null)
				fun ();
            view.InvalidateBatchingState();  //
		});
	}

	public void CloseView (GComponent view)
	{
	}

	public GameObject AddPrefab (string name, GGraph parent)
	{
		GameObject go = Tools.GetPrefab (PATH_ANIMATE + name);
		go.SetActive (false);
//		GameObjectScaler.Scale (go, 50);//new Vector3 (50, 50, 50);
		go.transform.localScale *= 50f;
//		GameObjectScaler.Scale (go, 0.8f);
		parent.SetNativeObject (new GoWrapper (go));
		go.SetActive (true);
		return go;
	}

	public GameObject AddShip (string id, GGraph parent, bool isAvatar = false)
	{
		GameObject go = Tools.GetPrefab (PATH_SHIP);
		go.GetComponent<ShipView> ().change (id, isAvatar);
		go.transform.localScale *= 140;
		parent.SetNativeObject (new GoWrapper (go));
		return go;
	}

	public GameObject AddEffect (string name, string act, object parent, Action<Animator> fun = null, bool isStop = false, float scale = 50f, string objPath = null, bool isActiveDelay = false)
	{
		long time = Tools.GetSystemTicks ();
		GameObject go = Tools.GetPrefab (PATH_ANIMATE + name);
//		Log.debug ("Create " + PATH_ANIMATE + name + " - " + (Tools.GetSystemTicks () - time) + "ms");
		Animator a = go.GetComponent<Animator> ();
		if (a == null)
		{
			Log.debug ("Effect Animator Is null - " + name);
//			return null;
		}
		go.transform.localScale *= scale;
//		GameObjectScaler.Scale (go, scale, false);
//		GameObjectScaler.ScaleParticles (go, 0.8f);

		((GGraph)parent).SetNativeObject (new GoWrapper (go));

		if (fun != null)
		{
			AnimatorState[] abArr = a.GetBehaviours<AnimatorState> ();
			for (int i = 0; i < abArr.Length; i++)
			{
				abArr [i].onEnd = fun;
			}
		}
		if (act != null)
		{
			if (objPath != null)
			{
				//Debug.LogError(name+"|"+ act + "|" + objPath+ " |go| "+ go.name);
				a = go.transform.Find (objPath).gameObject.GetComponent<Animator> ();
			}
			Timers.inst.CallLater ((object obj) =>
			{
				if (a != null)
				{
					if (isStop)
						a.Stop ();
					else
						a.Play (act);
				}
			});
		}
		if (isActiveDelay)
		{
			go.SetActive (false);
			float goActive = 10f;

			DOTween.To (() => goActive, x => goActive = x, 0, 0.06f).OnComplete (() =>
			{
				go.SetActive (true);
			});
		}
		return go;
	}

	public void StopAnimation (GameObject go)
	{
		if (go == null)
			return;
		Animator a = go.GetComponent<Animator> ();
		if (a != null)
			a.enabled = false;
		Animator[] aas = go.GetComponentsInChildren<Animator> (true);
		foreach (Animator aa in aas)
		{ 
			aa.enabled = false;
		}

		ParticleSystem[] pp = go.GetComponentsInChildren<ParticleSystem> (true);
		foreach (ParticleSystem p in pp)
		{ 
			//				p.Stop ();
			p.GetComponent<Renderer> ().enabled = false;
		}

		LineRenderer[] lr = go.GetComponentsInChildren<LineRenderer> (true);
		foreach (LineRenderer l in lr)
		{ 
			l.enabled = false;
		}
//		TimerManager.inst.Add (0.01f, 1, (float ooo) =>
//		{
//
//		});
	}

	public void PlayAnimation (GameObject go)
	{
		Animator a = go.GetComponent<Animator> ();
		if (a != null)
			a.enabled = true;
		Animator[] aas = go.GetComponentsInChildren<Animator> (true);
		foreach (Animator aa in aas)
		{ 
			aa.enabled = true;
		}

		ParticleSystem[] pp = go.GetComponentsInChildren<ParticleSystem> (true);
		foreach (ParticleSystem p in pp)
		{ 
//			p.Play ();
			p.GetComponent<Renderer> ().enabled = true;
		}

		LineRenderer[] lr = go.GetComponentsInChildren<LineRenderer> (true);
		foreach (LineRenderer l in lr)
		{ 
			l.enabled = true;
		}
	}

	public void ClearParticles (GameObject go)
	{
		ParticleSystem[] pp = go.GetComponentsInChildren<ParticleSystem> (true);
		foreach (ParticleSystem p in pp)
		{
			p.GetComponent<Renderer> ().enabled = false;
		} 
	}

	public void DeleteEffectEndFuntion (GameObject go)
	{
		Animator a = go.GetComponent<Animator> ();

		AnimatorState[] abArr = a.GetBehaviours<AnimatorState> ();
		for (int i = 0; i < abArr.Length; i++)
		{
			abArr [i].onEnd = null;
		}
	}

	public void PlayEffect (GameObject go, string act, Action<Animator> fun = null, string objPath = null)
	{
		Animator a = go.GetComponent<Animator> ();
		if (a == null)
		{
			Log.debug ("Effect Animator Is null - " + go.ToString ());
			return;
		}
		if (fun != null)
		{						
			AnimatorState[] abArr = a.GetBehaviours<AnimatorState> ();
			for (int i = 0; i < abArr.Length; i++)
			{				
				abArr [i].onEnd = fun;
			}
		}
		if (objPath != null)
		{
			a = go.transform.Find (objPath).gameObject.GetComponent<Animator> ();
		}
		Timers.inst.CallLater ((object obj) =>
		{
			if (act != null)
			{
//					a.playbackTime = 0;
				a.Play (act, 0, 0f);
			}
		});
    }

	public void TweenJump (GObject go, float time)
	{
		go.TweenScale (new Vector2 (1f, 0.8f), time / 2).OnComplete (() =>
		{
			go.TweenScale (new Vector2 (1f, 1f), time / 2).OnComplete (() =>
			{
				TweenJump (go, time);
			});
		});
	}

	public void ReTweenJump (GObject go)
	{
		go.scale = new Vector2 (1f, 1f);
		DOTween.Kill (go);
	}

	public void TweenJumpPro (GObject go, float time)
	{
		go.TweenScaleX (1.2f, time / 2).OnComplete (() =>
		{
			go.TweenScaleX (1f, time / 2).OnComplete (() =>
			{
				TweenJumpPro (go, time);
			});
		});
	}

	public void TweenAlpha (GObject go, float time, float start = 0f, float end = 1f)
	{
		go.alpha = start;
		go.TweenFade (end, time);
	}

	public void TweenHuXi (GObject go, float time)
	{
		go.TweenScale (new Vector2 (0.8f, 0.8f), time / 2).OnComplete (() =>
		{
			go.TweenScale (new Vector2 (1f, 1f), time / 2).OnComplete (() =>
			{
				TweenHuXi (go, time);
			}).SetEase (Ease.OutQuad);
		}).SetEase (Ease.OutQuad);
	}

	public void RedBagShow (GObject go, int num = 40, Action fun = null)
	{
		go.TweenMoveY (go.y + num, 1f).OnComplete (() =>
		{
			go.TweenMoveY (go.y - num, 1f).OnComplete (() =>
			{
				RedBagShow (go, num, fun);
			});
		});
	}

	public void ShowIcon (string type, int count, GComponent par, Vector2 start, Vector2 point)
	{
		Dictionary<string,object> ccc = (Dictionary<string,object>)DataManager.inst.systemSimple ["show_ranimation"];
		if (ccc == null || !ccc.ContainsKey (type))
			return;
		object[] ddd = (object[])ccc [type];
		int yuanshi = count;
		Vector2 end = new Vector2 ();
		string imgUrl = Tools.GetIconMcName (type);
		int ran = Tools.GetRandom (10, 40);
		int f = 10;
		double b = Convert.ToDouble (ddd [1]);
		count = Convert.ToInt32 (Math.Ceiling (Math.Pow (count, 1f / Convert.ToSingle (ddd [0]))));
		count = Convert.ToInt32 (Math.Ceiling (count * b));
		if (count > (int)ddd [2]||count<yuanshi)
		{
			count = (int)ddd [2]>yuanshi?yuanshi:(int)ddd[2];
		}
		b = Convert.ToDouble (Convert.ToInt32 (Math.Floor (Convert.ToSingle (yuanshi / count))));
		int lestOne = (yuanshi + Convert.ToInt32 (b)) - (Convert.ToInt32 (b) * count);
//		Log.debug ("count=" + count + "|b=" + b + "|lestOne=" + lestOne + "|");
		TextFormat tf;
		GTextField txt = new GTextField ();
		tf = txt.textFormat;
		tf.size = 30;
		txt.stroke = 2;
		txt.strokeColor = Color.white;
		Vector2 off = new Vector2 (0, 0);

        float ff=0f;
        if(type == Config.ASSET_REDBAGCOIN) {
            ff = 0.4f;
        }else {
            ff = 0;
        }

		switch (type)
		{
		case Config.ASSET_COIN:
			txt.color = Tools.GetColor ("FF2E94");
			end = ComGoldCoinExp.coinXY;
			off = new Vector2 (30, 30);
			break;
		case Config.ASSET_GOLD:
			txt.color = Tools.GetColor ("E18F00");
			end = ComGoldCoinExp.goldXY;
			off = new Vector2 (30, -30);
			break;
		case Config.ASSET_EXP:
			txt.color = Tools.GetColor ("00B23B");
			end = ComGoldCoinExp.expXY;
			off = new Vector2 (-30, -30);
			break;
		case Config.ASSET_CARD:
			txt.color = Tools.GetColor ("ABABAB");
			end = ComGoldCoinExp.cardXY;
			break;
		case Config.ASSET_ELSCORE:
			txt.color = Tools.GetColor ("00A7E5");
			end = ComGoldCoinExp.el_scoreXY;
			break;
		case Config.ASSET_REDBAGCOIN:
			txt.color = Tools.GetColor ("00A7E5"); //FB2900
             end = ComGoldCoinExp.redbag_coinXY;
			break;
		}
		txt.textFormat = tf;
		for (int i = 0; i < count; i++)
		{
			if (type == Config.ASSET_ELSCORE)
			{
				ComGoldCoinExp.Elcount++;
			}
			else if (type == Config.ASSET_REDBAGCOIN)
			{
				ComGoldCoinExp.Redcount++;
			}
			else
			{
				ComGoldCoinExp.count++;
			}
			int indexxxx = i;
//			GComponent g = Tools.GetComponent (Config.COM_EFFECTICON).asCom;
			GMovieClip g = Tools.GetComponent (imgUrl).asMovieClip;
			par.AddChild (g);
//			g.GetChild ("n0").asLoader.url = imgUrl;
			g.x = start.x + off.x;
			g.y = start.y + off.y;
			g.TweenMove (new Vector2 (start.x + Convert.ToSingle (Math.Cos (f * i) * ran) + off.x, start.y + Convert.ToSingle (Math.Sin (f * i) * ran) + off.y), 0.4f).SetDelay (i * 0.1f);
			g.alpha = 0;
			g.TweenFade (1f, (0.1f * i));
			g.scale = new Vector2 (0.7f, 0.7f);
			TimerManager.inst.Add ((0.1f * i) + 0.5f, 1, (float time) =>
			{
				g.TweenScale (new Vector2 (1f, 1f), 0.5f).OnComplete (() =>
				{
					g.TweenScale (new Vector2 (0.7f, 0.7f), 0.4f);
				});
				this.Bezier (g, 0.6f, new Vector2 (g.x, g.y), point, end, () =>
				{
					par.RemoveChild (g, true);
					if (type == Config.ASSET_ELSCORE)
					{
						if (indexxxx == (count - 1))
						{
							DispatchManager.inst.Dispatch (new MainEvent (MainEvent.JUMP_ELSCORE, new String[] {
								type,
								lestOne.ToString ()
							}));
						}
						else
						{
							DispatchManager.inst.Dispatch (new MainEvent (MainEvent.JUMP_ELSCORE, new String[] { type, b.ToString () }));
						}
					}
					else
					{
						if (indexxxx == (count - 1))
						{
							DispatchManager.inst.Dispatch (new MainEvent (MainEvent.JUMP_COINGOLDEXPGET, new String[] {
								type,
								lestOne.ToString ()
							}));
						}
						else
						{
							DispatchManager.inst.Dispatch (new MainEvent (MainEvent.JUMP_COINGOLDEXPGET, new String[] {
								type,
								b.ToString ()
							}));
						}
					}
				}).SetDelay(ff);
			});
		}
		par.AddChild (txt);
		txt.text = "+" + yuanshi.ToString ();
		txt.x = start.x + off.x;
		txt.y = start.y - 20 + off.y;
		txt.TweenMoveY (txt.y - 70f, 2f).OnUpdate (() =>
		{
			txt.alpha -= 0.01f;
		}).OnComplete (() =>
		{
			par.RemoveChild (txt);
			txt.Dispose();
		}).SetEase (Ease.OutBack);
	}
	public void SetRainbowBar(GComponent par)
	{
		GComponent n0 = par.GetChild ("n0").asCom;
		GComponent n1 = par.GetChild ("n1").asCom;
		n0.width = par.width * 3;
		n0.height = par.height;
		n0.y = 0;
		n0.x = -n0.width + par.width;

		n1.width = n0.width;
		n1.height = par.height;
		n1.y = 0;
		n1.x = n0.x - n1.width;

		float n0x = -n0.width + par.width;
		float n1x = n0.x - n1.width;

		float n0xx = par.width;
		float n1xx = -n0.width + par.width;


		n1.TweenMoveX (n1xx, 5).OnComplete (()=>{
			tweenMoveRainbowN1(n1,n0x,n0xx,n1x,n1xx);
		}).SetEase(Ease.Linear);
		n0.TweenMoveX (n0xx, 5).OnComplete (()=>{
			tweenMoveRainbowN0(n0,n0x,n0xx,n1x,n1xx);
		}).SetEase(Ease.Linear);

	}
	private void tweenMoveRainbowN0(GComponent box,float n0x,float n0xx,float n1x,float n1xx)
	{
		box.x = n1x;
		box.TweenMoveX (n1xx,5).OnComplete(()=>{
			tweenMoveRainbowN1(box,n0x,n0xx,n1x,n1xx);
		}).SetEase(Ease.Linear);
	}
	private void tweenMoveRainbowN1(GComponent box,float n0x,float n0xx,float n1x,float n1xx)
	{
		box.x = n0x;
		box.TweenMoveX (n0xx,5).OnComplete(()=>{
			tweenMoveRainbowN0(box,n0x,n0xx,n1x,n1xx);
		}).SetEase(Ease.Linear);
	}

	public Tweener Bezier (GObject view, float time, Vector2 a, Vector2 b, Vector2 c, Action fun = null)
	{	
		Bezier2D b2 = new Bezier2D (Bezier2D.BEZIER2D_2, a, b, c, Vector2.zero);
		float f = 0;
		Tweener t = DOTween.To (() => f, x => f = x, time, time).OnUpdate (() =>
		{						
			view.xy = b2.GetPosition (f / time);
			}).OnComplete (() =>
//		}).SetEase (Ease.OutQuad).OnComplete (() =>
		{
			if (fun != null)
				fun ();
		});
		return t;
	}

	public void EffortMainFuHaoJump (GObject go)
	{
//		go.TweenScaleY (0.8f, 0.2f).OnComplete (() =>
//		{
//			go.TweenScaleY (1f, 0.2f).OnComplete (() =>
//			{
		go.TweenMoveY (go.y - 20, 1f).OnComplete (() =>
		{
			go.TweenMoveY (go.y + 20, 1f).OnComplete (() =>
			{
				EffortMainFuHaoJump (go);
			}).SetEase (Ease.OutQuad);
		}).SetEase (Ease.OutQuad);
//			}).SetEase (Ease.OutQuad);
//		}).SetEase (Ease.OutQuad);
	}

	public void CardUpScale (GObject go)
	{
		go.TweenScale (new Vector2 (0.95f, 0.95f), 1f).SetEase (Ease.OutQuad).OnComplete (() =>
		{
			go.TweenScale (new Vector2 (1.0f, 1.0f), 1f).SetEase (Ease.OutQuad).OnComplete (() =>
			{
				CardUpScale (go);
			});
		});
	}

	public Tweener SetFilterAdjustBrightness (GObject go, float time, float start = 0f, float end = 1f)
	{
		Tweener t;
		ColorFilter cf = Tools.AddColorFilter (go);
		cf.Reset ();
		float c = start;
		if (time == 0)
		{			
			cf.AdjustBrightness (0);
			t = null;
		}
		else
		{
			t = DOTween.To (() => c, x => c = x, end, time).OnUpdate (() =>
			{
//				Log.debug ("xxxxxxxxxxxxxxxxxxxxxxxxxxxxx - " + Math.Round (c, 2).ToString ());
				cf.Reset ();
				cf.AdjustBrightness (Convert.ToSingle (Math.Round (c, 2)));
			});
		}
		return t;
	}

	//亮度
	public void SetShaderValue (GameObject go, float time, float start = 0f, float end = 0f)
	{
		Shader shader = Tools.GetAsset<Shader> (Config.SHADER_WHITE);
		MeshRenderer sh = go.GetComponent<MeshRenderer> ();
		if (sh != null)
		{
			Material ma = sh.material;
			if (ma != null)
			{
				ma.shader = shader;

				ma.SetFloat ("_Value", start);
				float c = start;
				DOTween.To (() => c, x => c = x, end, time).OnUpdate (() =>
				{
					ma.SetFloat ("_Value", c);
				});
			}
		}
		if (go.transform.childCount == 0)
			return;
		foreach (Transform t in go.transform)
		{
			SetShaderValue (t.gameObject, time, start, end);
		}
	}

	//对比度 置灰
	public void SetShaderSaturation (GameObject go, float gary = 0)
	{		
		MeshRenderer sh = go.GetComponent<MeshRenderer> ();
		if (sh != null)
		{			
			Material ma = sh.material;
			if (ma != null)
			{
				Shader shader = Tools.GetAsset<Shader> (Config.SHADER_WHITE);
				ma.shader = shader;
				ma.SetFloat ("_Saturation", gary);
				if (gary <= -1) {
					ma.SetFloat ("_Value", -0.2f);
				}
			}
		}
		if (go.transform.childCount == 0)
			return;
		foreach (Transform t in go.transform)
		{
			SetShaderSaturation (t.gameObject, gary);
		}
	}
	public void EffectAlpha(GImage img,float alpha)
	{
		float c;
		float next;
		if (alpha == 0) {
			c = 1;
			next = 1;
		} else {
			c = 0;
			next = 0;
		}

		DOTween.To (() => c, x => c = x, alpha, 1).OnUpdate (() =>
		{
				img.alpha = c;
			}).OnComplete(()=>{
				EffectAlpha(img,next);
			});
	}

	public Action<float> RotationLight(GImage img){

		float ro = 0;
		return TimerManager.inst.Add (0.03f, 0, (float t) => {
			if(img!=null){
				ro+=1;
				if(ro>=360){
					ro = 0;
				}
				img.rotation = ro;
			}
		});
	}
}