using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using DG.Tweening;
using System;

public class ComProgress : BaseCom
{

	public int max = 1;
	public int value = 0;

	private List<GLoader> list;
	private GLoader bar0;

	private int oneWidth = 41;

	public ComProgress ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);

		bar0 = this.GetChild ("n1").asLoader;
	}

	public override void Clear ()
	{
		base.Clear ();
		GLoader lo = null;
		if (list != null && list.Count > 0) {
			for (int j = 1; j < list.Count; j++)
			{
				lo = list [j];
				lo.Dispose ();
				lo.RemoveFromParent ();
			}
			list.Clear();
			list = null;
		}
	}

	public void SetMax (int max)
	{
		GLoader lo = null;
		if (list != null)
		{
			for (int j = 1; j < list.Count; j++)
			{
				lo = list [j];
				lo.Dispose ();
				lo.RemoveFromParent ();
			}
			list.Clear();
			list = null;
		}
		list = new List<GLoader> ();
		list.Add (bar0);
		if (max > 7) {
			oneWidth = Convert.ToInt32 (41 * 7 / max);
		} else {
			oneWidth = 41;
		}
		bar0.width = oneWidth;
		bar0.height = 24;
		bar0.pivot = new Vector2 (0.5f,0.5f);
		bar0.y = 2;
		bar0.x = 6;
		this.max = max;
		this.viewWidth = max * (oneWidth+1) + 12;
		
		for (int i = 1; i < max; i++)
		{
			lo = new GLoader ();
			lo.url = bar0.resourceURL;
			lo.fill = FillType.ScaleFree;
			lo.width = oneWidth;
			lo.height = 24;
			lo.pivot = new Vector2 (0.5f,0.5f);
			lo.y = 2;
			lo.x = i * (oneWidth+1) + 6;
			list.Add (lo);
			this.AddChildAt (lo, 1);
		}
	}

	public void SetValue (int value)
	{
		this.value = value;
		if (value > max)
		{
			this.value = max;
		}
		GLoader img = null;
		for (int j = 0; j < list.Count; j++)
		{
			img = list [j];
			if (j > this.value - 1)
			{
				img.url = Tools.GetResourceUrl ("Image2:n_icon_chengjiu1_");
			}
			else
			{
				if (j == value-1) {
					img.scale = new Vector2 (1.2f, 1f);
					img.TweenScale(new Vector2(0.8f,1f),0.1f).OnComplete(()=>{
						img.TweenScale(new Vector2(1.1f,1f),0.1f).OnComplete(()=>{
							img.TweenScale(new Vector2(1f,1f),0.1f);
						});
					});	
				}
				img.url = Tools.GetResourceUrl ("Image2:n_icon_chengjiu2_");
			}
		}
	}
}
