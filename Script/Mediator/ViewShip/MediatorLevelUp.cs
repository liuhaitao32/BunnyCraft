using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using FairyGUI;
using DG.Tweening;

public class MediatorLevelUp : BaseMediator
{
	private GList list;
	private GProgressBar pro;
	private GameObject green;
	private GameObject boom;

	Dictionary<string,object> sim;
	private Dictionary<string,object> cardData;
	private object[] tips;
	private CardVo card;
	public static string Cid = "";
	private CardVo cards;
	private List<int> uplist;
	private bool isUp = false;
	private bool isMove = true;
	private bool canMoveItem = false;
	private float n1x;
	private float n2x;
	private bool boll = false;
	private float maxWidth;

	private GLoader n11;

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_LEVELUP, true);
        SoundManager.inst.PlaySound(Config.SOUND_CARDLVUP);
		ViewManager.SetWidthHeight (this.GetChild ("n0"));
		sim = (Dictionary<string,object>)(DataManager.inst.systemSimple ["card_attr_info"]);
		cardData = (Dictionary<string,object>)DataManager.inst.card [Cid];
		if (cardData.ContainsKey ("tips"))
		{
			tips = (object[])cardData ["tips"];
		}
		GButton bgClick = this.GetChild ("n0").asButton;
		list = this.GetChild ("n5").asList;
		n11 = this.GetChild ("n1").asCom.GetChild ("n0").asCom.GetChild ("n11").asLoader;
		bgClick.onClick.Add (Mask_Click);
		pro = this.GetChild ("n1").asCom.GetChild ("n0").asCom.GetChild ("n7").asProgress;
		pro.GetChild ("bar").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shengji1");
		card = DataManager.inst.GetCardVo (Cid);
//		card = DataManager.inst.GetCardVo (Cid, card.lv - 1);
		this.GetChild ("n2").asTextField.text = Tools.GetMessageById (card.name);
		n1x = this.GetChild ("n1").asCom.x;
		n2x = this.GetChild ("n2").asTextField.x;
		this.GetChild ("n2").asTextField.x += 160;
		this.GetChild ("n1").asCom.x += 160;
		list.visible = false;
		this.GetChild ("n3").asTextField.visible = false;
		this.GetChild ("n3").asTextField.text = Tools.GetMessageById ("24133");
		n11.url = Tools.GetResourceUrl ("Image2:n_icon_dengji" + (card.rarity + 1));
		this.GetChild ("n1").asCom.GetChild ("n0").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_bg_kapai" + ((card.rarity < 3) ? "" : "2"));
		switch (card.rarity)
		{
		case 0:
			this.GetChild ("n1").asCom.GetChild ("n0").asCom.GetChild ("n9").asTextField.strokeColor = Tools.GetColor ("8c8c8c");
			break;
		case 1:
			this.GetChild ("n1").asCom.GetChild ("n0").asCom.GetChild ("n9").asTextField.strokeColor = Tools.GetColor ("df890e");
			break;
		case 2:
			this.GetChild ("n1").asCom.GetChild ("n0").asCom.GetChild ("n9").asTextField.strokeColor = Tools.GetColor ("af5bcb");
			break;
		case 3:
			this.GetChild ("n1").asCom.GetChild ("n0").asCom.GetChild ("n9").asTextField.strokeColor = Tools.GetColor ("427ba3");
			break;
		case 4:
			//			cardLv.color = Tools.GetColor ();
			break;
		}
//		cards = DataManager.inst.GetCardVo (Cid, card.lv - 1);
//		pro.value = cards.exp;
//		pro.max = cards.maxExp;
		maxWidth = pro.width;
		uplist = ListData (tips, card);
		list.itemRenderer = OnRander;
		list.numItems = uplist.Count;
//		pro.value = 50f;
//		pro.max = 20f;
//		return;
		//
		setItemData (this.GetChild ("n1").asCom.GetChild ("n0").asCom, Cid, card.lv - 1, false);

//		this.Effect1 ();
	}

	private void Effect1 ()
	{
	}

	private void OnRander (int index, GObject obj)
	{
		GLoader n0 = obj.asCom.GetChild ("n0").asLoader;
		GTextField n1 = obj.asCom.GetChild ("n1").asTextField;
		GTextField n2 = obj.asCom.GetChild ("n2").asTextField;
		GTextField n3 = obj.asCom.GetChild ("n3").asTextField;
		if (canMoveItem)
		{
			obj.alpha = 0;
			obj.TweenFade (1f, (0.7f * (index + 1)));
		}
		else
		{
			obj.alpha = 1;
		}
		CardVo carda = DataManager.inst.GetCardVo (Cid, card.lv - 1);
		Dictionary<string,object> tipi = (Dictionary<string,object>)tips [uplist [index]];
		n1.text = Tools.GetMessageById (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["name"].ToString ()) + ":";
		string[] arr = MediatorItemShipInfo.TextFild (uplist [index], carda, null, null, true, false);
		n2.text = arr [0];
		if (arr.Length != 1)
		{
			n3.text = arr [1];
		}
		else
		{
			n3.text = "";
		}
		n0.url = Tools.GetResourceUrl ("Icon:" + ((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["icon"].ToString ());
	}

	private void Mask_Click ()
	{
		if (isMove)
		{
			boll = true;
			isUp = true;
			this.RemoveChild (this.GetChild ("n6"));
			if (boom != null)
			{
				Tools.Clear (boom);
//				EffectManager.inst.ClearParticles (boom);
			}
			if (green != null)
			{
				Tools.Clear (green);
				EffectManager.inst.ClearParticles (green);
			}
			TimerManager.inst.Remove (OnTimer);
			TimerManager.inst.Remove (OnTimer2);
			OnTimer (1f);
//			OnTimer2 (1f);
			MoveOver ();
		}
		else
		{
			ViewManager.inst.CloseView (this);
		}

	}



	private void MoveOver (bool isMove = false)
	{
		if (!isMove)
		{
			this.GetChild ("n2").asTextField.x = n2x;
			this.GetChild ("n1").asCom.x = n1x;
//			this.GetChild ("n3").x = 525;
			list.visible = true;
			this.GetChild ("n3").asTextField.alpha = 1;
			this.GetChild ("n3").asTextField.visible = true;
			canMoveItem = false;
			list.numItems = uplist.Count;
		}
		else
		{
			this.GetChild ("n2").asTextField.TweenMoveX (n2x, 0.5f);
			this.GetChild ("n1").asCom.TweenMoveX (n1x, 0.5f);
//			view.GetChild ("n3").TweenMoveX (525,0.5f);
			list.visible = true;
			this.GetChild ("n3").asTextField.alpha = 1;
			this.GetChild ("n3").asTextField.visible = true;
			this.GetChild ("n3").TweenFade (1f, 1f);
			canMoveItem = true;
			list.numItems = uplist.Count;

		}
//		//liuhaitao
	}

	private Tweener tweeners;
	private Tweener allTween;
	private Action<float> tween_pro;

	public void setItemData (GComponent go, string id, int lv, bool bo)
	{
		cards = DataManager.inst.GetCardVo (id, lv);

		GLoader bg = go.GetChild ("n4").asLoader;
		bg.url = Tools.GetResourceUrl ("Icon:bg_kapai" + cards.rarity);

		GLoader icon = go.GetChild ("n3").asLoader;
		icon.url = Tools.GetResourceUrl ("Icon:" + id);

		GTextField l_have = go.GetChild ("n6").asTextField;
		l_have.text = Convert.ToString (cards.cost / 1000);

		GTextField l_lv = go.GetChild ("n9").asTextField;
		l_lv.text = cards.lv + "";

		Dictionary<string,object> cfg = DataManager.inst.systemSimple;
		object[] exp_cfg = (object[])(cfg ["exp_config"]);
		GImage img_up = go.GetChild ("n10").asImage;
		pro.name = "pro";


		if (bo == false)
		{
			pro.value = cards.maxExp;
			pro.max = cards.maxExp;
			isMove = true;
			tweeners = EffectManager.inst.SetFilterAdjustBrightness (go, 1.3f, 0f, 1f);
//			int c = cards.maxExp;
			green = EffectManager.inst.AddPrefab (Config.EFFECT_CARDLVUP, this.GetChild ("n6").asGraph);
			boom = EffectManager.inst.AddPrefab (Config.EFFECT_ELV_UP, this.GetChild ("n7").asGraph);
//			GameObjectScaler.Scale (green, 1.2f);
//			GameObjectScaler.Scale (boom, 0.8f);
			green.transform.localScale *= 1.2f;
			boom.transform.localScale *= 0.8f;

			GComponent par = pro.GetChild ("bar").asCom;
			GTextField txt = pro.GetChild ("title").asTextField;

			float times = 1.3f;// / (_max+1);
			if (pro.value >= pro.max) {
				pro.GetChild ("bar").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shengji2");
				pro.GetChild ("title").asTextField.color = Tools.GetColor ("499227");
			} else {
				pro.GetChild ("bar").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shengji1");
				pro.GetChild ("title").asTextField.color = Tools.GetColor ("E8FCD9");
			}
			allTween = DOTween.To (() => pro.value, x => pro.value = x, 0, times).OnComplete (() =>
			{
				if (isMove)
				{
					isMove = false;
					OnTimer (0f);
				}
				else
				{
					Check_pro_width ();
				}
			});
		}
		else
		{
			isMove = false;
//			TimerManager.inst.Remove(tween_pro);
//			if (allTween != null)
//			{
//				DOTween.Kill (allTween, true);
//				allTween = null;
////				Log.debug("aaaaaaaaaaaaaaaaaaaaaaa");
//			}
			if (green != null)
			{
				Tools.Clear (green);
				EffectManager.inst.ClearParticles (green);
			}
            //EffectManager.inst.SetFilterAdjustBrightness(go, 0.3f, 0f, 1f);
            go.TweenScale (new Vector2 (1.3f, 1.3f), 0.15f).OnComplete (() =>
			{
                go.TweenScale (new Vector2 (0.8f, 0.8f), 0.2f).OnComplete(()=> {
                    go.TweenScale(new Vector2(1f, 1f), 0.2f).OnStart(() => {
                        EffectManager.inst.SetFilterAdjustBrightness(go, 0f, 1f, 0f);
                    }).OnComplete(()=> {
                        TimerManager.inst.Add(0.5f, 1, OnTimer2);
                    });
                });
			});
            
            Check_pro_width ();
			if (pro.value >= pro.max)
			{
//				pro.skin = ComProgressBar.BAR6;
				pro.GetChild ("title").asTextField.color = Tools.GetColor ("499227");
				pro.GetChild ("bar").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shengji2");
				img_up.visible = true;
			}
			else
			{
//				pro.skin = ComProgressBar.BAR3;
				pro.GetChild ("title").asTextField.color = Tools.GetColor ("E8FCD9");
				pro.GetChild ("bar").asCom.GetChild ("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shengji1");
				img_up.visible = false;
			}
			isUp = true;
			list.numItems = uplist.Count;
		}
	}
	//	private void Pro_change(float _value,float times,float max){
	//		allTween = DOTween.To (() => pro.value, x => pro.value = x, 0, times).OnComplete(()=>{
	//
	////			Pro_change();
	//		});
	//	}
	private void Check_pro_width ()
	{
//		pro.width = maxWidth;
//		pro.scale = new Vector2 (1f, 1f);
		pro.value = cards.exp;
		pro.max = cards.maxExp;

//		Debug.LogError (pro.width);
	}

	private void OnTimer2 (float ff)
	{
		EffectManager.inst.CardUpScale (this.GetChild ("n1").asCom.GetChild ("n0").asCom);
		if (!boll)
		{
			MoveOver (true);
		}
	}

	private void OnTimer (float ff)
	{
		if (this.group == null)
			return;
//		DOTween.Kill (this.GetChild ("n1").asCom.GetChild ("n0").asCom);
		if (tweeners != null)
			tweeners.Kill ();
		Log.debug ("SetFilterAdjustBrightness - tweeners 0.......");
//		DOTween.Kill (allTween, true);
		if (allTween != null)
			allTween.Kill ();
		setItemData (this.GetChild ("n1").asCom.GetChild ("n0").asCom, Cid, card.lv, true);
//		DOTween.Restart (pro);
//		DOTween.Kill (pro);
//		pro.scale = new Vector2 (1f, 1f);
	}

	private List<int> ListData (object[] obj, CardVo card)
	{
		List<int> re = new List<int> ();
		Dictionary<string,object> up;

		if (cardData.ContainsKey ("up"))
		{
			up = (Dictionary<string,object>)cardData ["up"];
		}
		for (int i = 0; i < obj.Length; i++)
		{
			Dictionary<string,object> tipi = (Dictionary<string,object>)obj [i];
			int round = 0;
			if (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]).ContainsKey ("round"))
			{
				round = Convert.ToInt32 (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["round"]);
			}
			Dictionary<string,object> monster = new Dictionary<string, object> ();
			string carr = tipi.ContainsKey ("call") ? tipi ["call"].ToString () : null;
			string callin = tipi.ContainsKey ("callin") ? tipi ["callin"].ToString () : null;
			if (carr != null)
			{
				monster = (Dictionary<string,object>)DataManager.inst.beckon [carr];
			}
			if (tipi.ContainsKey ("replace"))
			{
				object[] replace = (object[])tipi ["replace"];
				for (int j = 0; j < replace.Length; j++)
				{
					string[] fafa;
					if (carr != null)
					{
						fafa = MediatorItemShipInfo.suanShu ((object[])replace [j], monster, card, round);
					}
					else
					{
						fafa = MediatorItemShipInfo.suanShu ((object[])replace [j], cardData, card, round);
					}
					if (fafa.Length > 1)
					{
						re.Add (i);
						break;
					}
				}
			}
		}
		return re;
	}

	public override void Clear ()
	{
//		DOTween.Clear ();
		DOTween.Kill (pro);
		TimerManager.inst.Remove (OnTimer2);
		TimerManager.inst.Remove (OnTimer);
		this.DispatchGlobalEvent (new MainEvent (MainEvent.CARD_LEVELUP));
		base.Clear ();
	}
}
