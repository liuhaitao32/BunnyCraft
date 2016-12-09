using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class ComBigIcon : BaseCom
{
	private int thisType = 0;
	private Controller c1;
	private ComCard card;

	//	private GLoader iconbg_2;
	private GLoader icon_2;
	private GLoader bar_2;
	private GTextField num_2;

	private int mExp;
	private int mNum;
	private object _num;
	public int cha = 0;
	private Tweener teener;

	public ComBigIcon ()
	{
		
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);

		c1 = this.GetController ("c1");
		card = this.GetChild ("n0").asCom.GetChild ("n0") as ComCard;

//		iconbg_2 = this.GetChild ("n4").asLoader;
		icon_2 = this.GetChild ("n4").asLoader;
		num_2 = this.GetChild ("n15").asTextField;
	}

	public void SetData (string id, object num = null, int type = 0, string type2ID = null)
	{
		_num = num;
		if (id.StartsWith ("C"))
		{
			thisType = 1;
			if (num != null)
			{
				card.SetData (id, Convert.ToInt32 (num), type);
				card.SetText ("x" + num);
			}
			else
			{
				card.SetData (id, -1, type);
			}
		}
		else
		{
			thisType = 2;
			if (id == Config.ASSET_BODY) {
				icon_2.url = Tools.GetResourceUrl ("Icon:" + num);
				num_2.text = "x1";
			} else {
				if (type2ID != null)
				{
					icon_2.url = Tools.GetResourceUrl (type2ID);
				}
				else
				{
					if (id == Config.ASSET_GOLD)
					{
						icon_2.url = Tools.GetResourceUrl ("Image2:n_icon_xm1");
					}
					else if (id == Config.ASSET_COIN)
					{
						icon_2.url = Tools.GetResourceUrl ("Image2:n_icon_zs1");
					}
					else if (id == Config.ASSET_ELSCORE)
					{
						icon_2.url = Tools.GetResourceUrl ("Image:icon_jf1");
					}
					else
					{
						icon_2.url = Tools.GetIconUrl (id);
					}
				}
				num_2.text = "x" + num.ToString ();
			}
		}
//		c1.selectedIndex = 2;
//		setSelectIndex (thisType);
	}

	public void SetText (string name)
	{
		card.SetText (name);
	}

	public void MoveToExp (int exp, int num)
	{
		mExp = exp;
		mNum = num;
		TimerManager.inst.Add (0.4f, 1, TimerFunction);
	}

	private void TimerFunction (float fa)
	{
		switch (thisType)
		{
		case 1:
			card.cha = Convert.ToInt32 (Math.Ceiling ((mExp - card.vo.exp) / 4f));
			card.MoveToExp (mExp);
			break;
		case 2:
			cha = Convert.ToInt32 (Math.Ceiling ((mNum - Convert.ToInt32 (_num)) / 4f));
			MoveToNum (mNum);
			break;
		}
	}

	public void reMoveTimer ()
	{
		TimerManager.inst.Remove (TimerFunction);
	}

	public void MoveToNum (int num)
	{
		if ((Convert.ToInt32 (_num) + cha) >= num)
		{
			num_2.text = "x" + num.ToString ();
			return;
		}
		num_2.text = "x" + (Convert.ToInt32 (_num) + cha).ToString ();
//		downBar.TweenScaleX(1.2f,0.05f).OnComplete(()=>{
//				downBar.TweenScaleX(1f,0.05f).OnComplete(()=>{
//				MoveToNum(num);
//			});
//		});
	}

	public void SetSelectIndex (int index)
	{
		if (index != c1.selectedIndex)
			c1.selectedIndex = index;
	}

	public void RemRotateCard ()
	{
		if (teener != null)
		{
			DOTween.Kill (teener);
		}
	}

	public void RotateCard (float time,Action fun = null)
	{
		if (time == 0)
		{
			c1.selectedIndex = thisType;
			return;
		}
		this.rotationY = 0;
		float c = 0;
		float y = 90;
		teener = DOTween.To (() => c, x => c = x, 90, time / 2f).OnUpdate (() =>
		{
			this.rotationY = c;
		}).OnComplete (() =>
		{			
			c1.selectedIndex = thisType;
			//刷新排序状态
			teener = DOTween.To (() => y, x => y = x, 0, time / 2f).OnUpdate (() =>
			{
				this.rotationY = y;
				this.InvalidateBatchingState ();
			}).OnComplete (() =>
			{
				teener = null;
				if(fun!=null){
					fun();
				}
				//刷新排序状态
				this.InvalidateBatchingState ();
			});
		});
	}
}
