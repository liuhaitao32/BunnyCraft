using System;
using UnityEngine;
using FairyGUI;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
public class ComCard:BaseCom
{
	private GTextField lv;
	private GTextField cardLv;
	private GLoader icon;
	private GLoader bg;
	private GProgressBar bar;
	private GImage up;
	private GTextField txt;
	private GLoader proBar;
	private GLoader n11;
	private GLoader cardBg;

	private Dictionary<string,object> cfg;
	public int cha;
	public CardVo vo;

	public ComCard ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);

		lv = this.GetChild ("n6").asTextField;
		cardLv = this.GetChild ("n9").asTextField;
		icon = this.GetChild ("n3").asLoader;
		bg = this.GetChild ("n4").asLoader;
		cardBg = this.GetChild ("n0").asLoader;
		bar = this.GetChild ("n7").asProgress;
		up = this.GetChild ("n10").asImage;
		txt = this.GetChild ("n12").asTextField;
		n11 = this.GetChild ("n11").asLoader;
		proBar = bar.GetChild ("bar").asCom.GetChild ("n0").asLoader;
//		bar.offsetY = 5;

		EffectManager.inst.AddPrefab (Config.EFFECT_DENGJI, this.GetChild ("n15").asGraph).transform.localScale *= 1.1f;
		EffectManager.inst.AddPrefab (Config.EFFECT_K04, this.GetChild ("n17").asGraph).transform.localScale *= 1.1f;
		this.GetChild ("n15").asGraph.visible = false;
		this.GetChild ("n17").asGraph.visible = false;

		cfg = DataManager.inst.systemSimple;

//		GGraph ggg = this.GetChild ("n18").asCom.GetChild("n3").asGraph;//new GGraph ();
////		ggg.SetSize (200, 50);
////		ggg.x = 300;
////		ggg.y = 5;
//		float mm = 125f;
//		float cc = 5f;
//		float[] rgbA = new float[]{1f,0f,0f,125f,0f,0f};
//		float[] rgbB = new float[]{1f,1f,0f,125f,125f,0f};
//
//		Color ccs = Color.white;
//		Color ccs2 = Color.white;
//		Color[] ccc = new Color[]{ ccs, ccs, ccs2, ccs2 };
//		TimerManager.inst.Add (0.06f, 0, (float x) => {
//			rgbA = Tools.GetRGB(mm,cc,ref rgbA);
//			rgbB = Tools.GetRGB(mm,cc,ref rgbB);
//
//			ccs.r = rgbA[3]/mm;
//			ccs.g = rgbA[4]/mm;
//			ccs.b = rgbA[5]/mm;
//
//			ccs2.r = rgbB[3]/mm;
//			ccs2.g = rgbB[4]/mm;
//			ccs2.b = rgbB[5]/mm;
//			ccc[0] = ccs;
//			ccc[1] = ccs;
//			ccc[2] = ccs2;
//			ccc[3] = ccs2;
//
//			ggg.shape.DrawRect (0, ccc);
//
//		});


//		this.AddChild (ggg);
	}

	public override void Clear ()
	{
//		TimerManager.inst.Remove (timers);
//		Debug.LogError("Clear");
		base.Clear ();
		if (timers != null) {

			TimerManager.inst.Remove (timers);
		}
		timers = null;
//		UnityEngine.Debug.LogError ("card Clear");
	}
	public void ClearTime ()
	{
		if (timers != null) {

			TimerManager.inst.Remove (timers);
		}
		timers = null;
	}
	private Action<float> timers;
	private void SetColor()
	{
		GLoader ia = this.GetChild ("n4").asLoader;
		GLoader ib = this.GetChild ("n11").asLoader;

		float mm = 125f;
		float mmm = 250f;
		float cc = 5f;
		float[] rgbA = new float[]{1f,1f,0f,mm,mm,0f,0f};
//		float[] rgbB = new float[]{1f,1f,0f,mm,mm-40f,0f,0f};
		Color ccs = Color.white;
//		Color ccs2 = Color.white;
//		Color[] ccc = new Color[]{ ccs, ccs, ccs2, ccs2};
		float[] nullColor = new float[]{ 1, 0, 1 };

		timers = TimerManager.inst.Add (0.04f,0,(float x)=>{
//			Debug.LogError("this.isClear:$:  "+(this == null));
//			Debug.LogError("this.isClear::  "+this.isClear);
//			Debug.LogError("this.isClear:%:  "+(this.parent == null));
			if(this.parent == null){
				if(timers!=null){
					TimerManager.inst.Remove (timers);
				}
				timers = null;
				return;
			}

//			Debug.LogError ("timers --- " + timers.ToString());
//			{
//				c+=jian;
//				ColorFilter coingggg = new ColorFilter ();
//				ColorFilter coingggg1 = new ColorFilter ();
//				bg.filter = coingggg;
//				n11.filter = coingggg1;
//				coingggg.AdjustHue (c);
//				coingggg1.AdjustHue (c);
//				if (c > 1f)
//					jian = -0.022f;
//				if (c < 0) {
//					jian = 0.022f;
//				}
//			}
			if(vo.id == "")
			{
			}
			rgbA = Tools.GetRGB(mm,cc,ref rgbA,nullColor);
//			rgbB = Tools.GetRGB(mm,cc,ref rgbB,nullColor);

			ccs.r = rgbA[3]/mmm+0.4f;
			ccs.g = rgbA[4]/mmm+0.4f;
			ccs.b = rgbA[5]/mmm+0.4f;

//			ccs2.r = rgbB[3]/mmm;
//			ccs2.g = rgbB[4]/mmm;
//			ccs2.b = rgbB[5]/mmm;

//			ccc[0] = ccs;
//			ccc[1] = ccs;
//			ccc[2] = ccs2;
//			ccc[3] = ccs2;

			ia.color = ccs;
			ib.color = ccs;
		});
	}
	float c = -1;
	float jian = 0.022f;
	private void TimerFilter(float ff)
	{
		c+=jian;
		ColorFilter coingggg = new ColorFilter ();
		ColorFilter coingggg1 = new ColorFilter ();
		bg.filter = coingggg;
		n11.filter = coingggg1;
		coingggg.AdjustHue (c);
		coingggg1.AdjustHue (c);
		if (c > 1f)
			jian = -0.022f;
		if (c < 0) {
			jian = 0.022f;
		}
	}
	public void SetData (object id, int exp = -1, int type = 0, int _lv = -1)
	{
		this.GetController ("c1").selectedIndex = type;
		vo = DataManager.inst.GetCardVo (id.ToString (), _lv, exp);
		bg.url = Tools.GetResourceUrl ("Icon:bg_kapai" + vo.rarity);
		icon.url = Tools.GetResourceUrl ("Icon:" + id);
//		cardLv.text = Tools.GetMessageById ("14015", new string[]{ vo.lv.ToString () });
		cardLv.text = vo.lv.ToString ();
		this.GetChild ("n15").visible = false;
		this.GetChild ("n17").visible = false;
		switch (vo.rarity)
		{
		case 0:
			cardLv.strokeColor = Tools.GetColor ("8c8c8c");
			break;
		case 1:
			cardLv.strokeColor = Tools.GetColor ("df890e");
			break;
		case 2:
			cardLv.strokeColor = Tools.GetColor ("af5bcb");
			break;
		case 3:
			cardLv.strokeColor = Tools.GetColor ("427ba3");
			if (type == 0 || type == 2||type == 4) {
//				this.GetChild ("n15").visible = true;
//				this.GetChild ("n17").visible = true;
//				SetColor();

			}
			break;
		case 4:
//			cardLv.color = Tools.GetColor ();
			break;
		}
		lv.text = vo.GetCost ();
		n11.url = Tools.GetResourceUrl ("Image2:n_icon_dengji" + (vo.rarity + 1));
		cardBg.url = Tools.GetResourceUrl ("Image2:n_bg_kapai" + ((vo.rarity < 3) ? "" : "2"));
		Dictionary<string,object> exp_Cfg = (Dictionary<string,object>)Tools.Analysis (cfg, "card_lv_exp." + vo.rarity);
		bar.GetChild ("title").asTextField.color = Tools.GetColor ("E8FCD9");
		if (vo.lv >= vo.maxLv)
		{		
//			bar.skin = ComProgressBar.BAR6;
			bar.max = Convert.ToInt32 (((object[])exp_Cfg ["lv_max"]) [0].ToString ());
			bar.value = vo.exp;
			if (bar.value >= bar.max) {
				proBar.url = Tools.GetResourceUrl ("Image2:n_icon_shengji2");
				bar.GetChild ("title").asTextField.color = Tools.GetColor ("499227");
			} else {
				proBar.url = Tools.GetResourceUrl ("Image2:n_icon_shengji1");
			}
			up.visible = false;	
		}
		else
		{							
			if (vo.exp < vo.maxExp)
			{
				up.visible = false;
//				bar.viewWidth = 122;
//				bar.skin = ComProgressBar.BAR3;
				proBar.url = Tools.GetResourceUrl ("Image2:n_icon_shengji1");
			}
			else
			{
				up.visible = true;
//				bar.viewWidth = 102;
//				bar.skin = ComProgressBar.BAR6;
				proBar.url = Tools.GetResourceUrl ("Image2:n_icon_shengji2");
				bar.GetChild ("title").asTextField.color = Tools.GetColor ("499227");
				EffectManager.inst.TweenJump (up, 1f);
			}
			bar.max = vo.maxExp;
			bar.value = vo.exp;		
		}
	}

	public void SetText (string name)
	{
		txt.text = name;
	}

	public void MoveToExp (int exp)
	{
		if (vo.exp >= exp)
		{
			vo.exp = exp;
			bar.max = vo.maxExp;
			bar.value = vo.exp;
			return;
		}
		vo.exp += cha;
		bar.max = vo.maxExp;
		bar.value = vo.exp;
		bar.TweenScaleX (1.2f, 0.05f).OnComplete (() =>
		{
			bar.TweenScaleX (1f, 0.05f).OnComplete (() =>
			{
				MoveToExp (exp);
			});
		});
	}
}