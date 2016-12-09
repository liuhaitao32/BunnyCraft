using System;
using FairyGUI;
using System.Collections.Generic;
using UnityEngine;

public class ComTabEffect:BaseCom
{
	private GImage bg;

	private int count;
	private int gap = 0;

	public ComTabEffect ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);
		bg = this.GetChild ("n0").asImage;
	}

	public void SetCount (int count, int gap = 5)
	{
		this.count = count;
		this.gap = gap;
	}

	public void SetIndex (int index, bool isEffect = true)
	{
		Vector2 v2 = new Vector2 ();
		v2.x = bg.width * index + (index * gap);
//		v2.y = 0;
		if (isEffect)
			bg.TweenMove (v2, 0.2f);
		else
			bg.TweenMove (v2, 0f);
	}

	public void SetIndex (GObject obj, bool isEffect = true)
	{
		Vector2 v2 = new Vector2 (obj.x - this.x, bg.y);
//		v2.x = bg.width * index + (index * gap);
		//		v2.y = 0;
		bg.x = obj.x-this.x;
		bg.y = bg.y;
//		if (isEffect)
//			bg.TweenMoveX (v2.x, 0.2f);
//		else
//			bg.TweenMoveX (v2.x, 0f);
	}
}