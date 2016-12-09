using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using FairyGUI;
using DG.Tweening;
using System;
using System.Globalization;

public class MediatorGiftShow : BaseMediator {//奖励显示界面，通用
//	public static Dictionary<string,object> data;
//	public static Dictionary<string,object> all_Data;
	public object[] dataShip;
	public int _gold = -1;
	public int _coin = -1;
	public int _el_score = -1;
	public int _red_coin = -1;
	List<object> newdata;
	private int nowIndex = 0;
	private GTextField L_Label;
//	private bool canClick = false;

	public static bool isExplore = false;

	private GButton ClickBtn;
	private bool canClick = false;
	private bool allClick = false;

	private GComponent item;
	private GGraph starImg;
	private ComBigIcon comIcon;
	private int mNum = 0; 
	private GComponent mpro;
	private GTextField mprotxt;
	private int cha;
	private GImage upImg;
	private int maxExp = -1;

	private GameObject shipobj;
	private GameObject move;
    public CardVo card;
    public bool isLegend;
    public bool isNew;
    public override void Init ()
	{
		ViewManager.inst.s.displayObject.gameObject.SetActive (false);
		base.Init ();
		this.Create (Config.VIEW_GIFTSHOW,true);
		ViewManager.SetWidthHeight (this.GetChild ("n0"));
		ViewManager.SetWidthHeight (this.GetChild ("n5"));

	}
	private string _move;
	public void SetData(object obj,string moves)
	{
//		newdata = new List<Dictionary<string, object>> ();
//		if (all_Data != null) {
//			newdata = Tools.GetReward (all_Data);
//		} else {
//			newdata = Tools.GetReward (data);
//		}
		_move = moves;

		List<object> dddd = Tools.GetReward (obj);
		newdata = new List<object> ();
		for (int i = 0; i < dddd.Count; i++) {
			Dictionary<string,object> dic = (Dictionary<string,object>)dddd [i];
			if (!dic.ContainsKey (Config.ASSET_RANKSCORE) && !dic.ContainsKey (Config.ASSET_REDBAGCOIN) && !dic.ContainsKey (Config.ASSET_EXP)) {
				newdata.Add (dic);
			}
		}
		nowIndex = 0;
		ClickBtn = this.GetChild ("n0").asButton;
		ClickBtn.onClick.Add (Mask_Click);
		L_Label = this.GetChild ("n2").asTextField;
		L_Label.text = (newdata.Count - 1) == 0 ? "" : (newdata.Count - 1) + "";
		this.GetChild("n4").visible = false;
		L_Label.visible = false;
		if (_move.IndexOf ("egg") != -1) {
			move = EffectManager.inst.AddEffect (_move, "stand", this.GetChild ("n1").asGraph);
//			GameObjectScaler.Scale (move, 1.5f);
			move.transform.localScale *= 1.5f;
		} else if (_move.IndexOf ("bag") != -1) {
			move = EffectManager.inst.AddEffect (_move, "stand", this.GetChild ("n1").asGraph);
//			GameObjectScaler.Scale (move,0.6f);
			move.transform.localScale *= 0.6f;
		} else if (_move.IndexOf ("mail") != -1) {
			move = EffectManager.inst.AddEffect (_move, "start", this.GetChild ("n1").asGraph);
		}else
		{
			move = EffectManager.inst.AddEffect (_move, "stand", this.GetChild ("n1").asGraph);
		}
        ClickBtn.touchable = false;
		TimerManager.inst.Add (0.5f, 1, (float ff) => {
			ClickBtn.touchable = true;
			if(nowIndex>=newdata.Count){
				return;
			}
			if (_move.IndexOf ("mail") != -1) {
				nowID = showItem ((Dictionary<string,object>)newdata[nowIndex],true,0.4f);
			} else {
				nowID = showItem ((Dictionary<string,object>)newdata[nowIndex],true);
			}
			nowIndex++;
		});
	}
	private float openTimes = 0.2f;
	Action<float>onRED_GIFT;
	private string showItem (Dictionary<string,object>_data,bool ismove = false,float times = 0.2f)
	{
		openTimes = times;
		if (this.item != null) {
			if (comIcon != null) {
				comIcon.reMoveTimer ();
				comIcon = null;
			}
			mpro = null;
			mprotxt = null;
			TimerManager.inst.Remove(TimerFunction);
			TimerManager.inst.Remove(TimerFunction2);
			TimerManager.inst.Remove (OnTimerFunction);
			this.RemoveChild (this.item);
			starImg = null;
			this.item = null;
		}
		TimerManager.inst.Remove (onRED_GIFT);
		maxExp = -1;
		string _id = "";
		string _num = "";
		foreach (string id in _data.Keys) {
			_id = id;
			_num = _data [id].ToString ();
		}
        if(_id.StartsWith("C")) {//判断是否是彩卡和新卡
            card = DataManager.inst.GetCardVo(_id);
            if(card.rarity == 3) {
                ClickBtn.touchable = false;
                TimerManager.inst.Add(3f, 1, (float ff) => { ClickBtn.touchable = true; });
            } else if(card.exp.ToString() == _num && card.lv == 1) {
                ClickBtn.touchable = false;
                isNew = true;
                TimerManager.inst.Add(3f, 1, (float ff) => { ClickBtn.touchable = true; });
            }
            if(card.rarity == 3)
                isLegend = true;

        }
		float f = this.GetChild ("n1").asGraph.y;
//		float tt = 0.3f;
//		if (!ismove) {
//			tt = 0;
//		}


//		DOTween.To (() => f, x => f = x, 400f, tt).OnUpdate (() => {
//			this.GetChild ("n1").asGraph.y = f;
//		}).OnComplete (() => {
			
			L_Label.visible = true;
			if (nowIndex == newdata.Count - 1) {
//				if(_move.IndexOf ("bag") != -1)
//				{
////					GameObjectScaler.Scale (EffectManager.inst.AddEffect (_move, "end", this.GetChild ("n1").asGraph),0.6f);
//					EffectManager.inst.AddEffect (_move, "open", this.GetChild ("n1").asGraph).transform.localScale *= 0.6f;
//                } else{
//					EffectManager.inst.PlayEffect(move,"touch");
//                    SoundManager.inst.PlaySound(Config.SOUND_OPENBOX);
//            	}
				this.GetChild ("n4").visible = false;
			} else {
				this.GetChild ("n4").visible = true;
			}
			if (_move.IndexOf ("egg") != -1) {
				EffectManager.inst.PlayEffect(move,"open");
				SoundManager.inst.PlaySound(Config.SOUND_OPENBOX);
			} else if (_move.IndexOf ("bag") != -1) {
            //					GameObjectScaler.Scale (EffectManager.inst.AddEffect (_move, "touch", this.GetChild ("n1").asGraph),0.6f);
            GameObject go = EffectManager.inst.AddEffect(_move, "open", this.GetChild("n1").asGraph);
            go.transform.localScale *= 0.6f;
            if(go.GetComponent<AudioSource>() != null) {
                go.GetComponent<AudioSource>().volume = ModelManager.inst.userModel.isSound ? 1 : 0;
            }
//			} else if(_move.IndexOf ("box") != -1) {
//				EffectManager.inst.PlayEffect(move,"touch");
//				SoundManager.inst.PlaySound(Config.SOUND_OPENBOX);
			} else {
				EffectManager.inst.PlayEffect(move,"touch");
                SoundManager.inst.PlaySound(Config.SOUND_OPENBOX);   
			}
		onRED_GIFT = TimerManager.inst.Add (times, 1, (float asdf) => {
			allClick = true;
			this.item = Tools.GetComponent (Config.RED_GIFT).asCom;
			(item.GetChild ("n8") as ComProgressBar).skin = ComProgressBar.BAR3;
			(item.GetChild ("n8") as ComProgressBar).offsetY = 5;
			(item.GetChild ("n9") as ComProgressBar).skin = ComProgressBar.BAR9;
			(item.GetChild ("n9") as ComProgressBar).offsetY = -3;
			item.GetChild ("n13").asTextField.text = Tools.GetMessageById ("24125");
			item.GetController ("c1").selectedIndex = 1;
			Controller c2 = item.GetController ("c2");
			c2.selectedIndex = 3;
			GTextField name = item.GetChild ("n1").asTextField;
			GTextField info = item.GetChild ("n2").asTextField;
			GTextField num = item.GetChild ("n3").asTextField;
			comIcon = item.GetChild ("n4") as ComBigIcon;
			starImg = item.GetChild ("n5").asGraph;
			this.AddChild (this.item);
			name.visible = false;
			info.visible = false;
			num.visible = false;
			starImg.visible = false;
            starImg.TweenScale(new Vector2(1.2f, 1.2f), 0.5f);
            if (isLegend) {
				EffectManager.inst.AddPrefab ("Legendcard/legendcard", starImg);
			} else {
				EffectManager.inst.AddPrefab ("Normalcard/normalcard", starImg);
			}
//				Log.debug(view.GetChildIndex(item));
//				Log.debug(view.GetChildIndex(view.GetChild("n4")));
			this.item.x = Tools.offectSetX (230f);
			this.item.y = 326f;

			comIcon.rotationY = 180;
			int expNum = 0;
			if (_id.StartsWith ("s") && _id != Config.ASSET_ELSCORE && _id != Config.ASSET_RANKSCORE) {
				name.text = Tools.GetBodyName (_id);
				c2.selectedIndex = 3;
				comIcon.GetChild ("n0").visible = false;
				comIcon.SetSelectIndex (2);
				shipobj = EffectManager.inst.AddShip (_id, comIcon.GetChild ("n11").asGraph, true);

				shipobj.GetComponent<AudioSource> ().volume = ModelManager.inst.userModel.isSound ? 1 : 0;
                    
				//					GameObjectScaler.Scale(shipobj,0.6f);
				shipobj.transform.localScale *= 0.6f;
				shipobj.transform.Rotate (new Vector3 (270, 120, 0));
				shipobj.transform.Rotate (new Vector3 (0, 20, 0));
				info.text = Tools.GetMessageById ("24110");
				num.text = "x" + _num;
				comIcon.SetData (_id, _num);
				TimerManager.inst.Add (0.01f, 0, OnFFFFFFFFF);
				comIcon.SetSelectIndex (1);
			} else {
				float c = 0;
				float y = 90;
				DOTween.To (() => c, x => c = x, 90, 0.5f).OnUpdate (() => {
					comIcon.rotationY = c;
				}).OnComplete (() => {
					TimerManager.inst.Add (0.2f, 1, (float ff) => {
						SoundManager.inst.PlaySound (Config.SOUND_SHOWCARD);
					});
					name.text = _id;
					if (_id.StartsWith ("C") && _id != Config.ASSET_CARD) {
						c2.selectedIndex = 2;
						CardVo ava = DataManager.inst.GetCardVo (_id);
						if (ava.newcard == 1 && isNew) {
							item.GetChild ("n13").asTextField.visible = true;
							isNew = false;
						}
						expNum = ava.exp;
						comIcon.SetData (_id, _num, 2);
						comIcon.SetSelectIndex (1);
						num.text = "x" + _num;
						name.text = Tools.GetMessageById (ava.name);
						info.text = CardVo.GetRarityMss (ava.rarity, 1);
                            
					} else if (_id.StartsWith ("s") && _id != Config.ASSET_ELSCORE && _id != Config.ASSET_RANKSCORE) {

					} else {
						name.text = Tools.GetIconName (_id);
						comIcon.SetSelectIndex (2);
						num.text = "x" + _num;

						//						info.text = Tools.GetIconName (_id);
						info.text = Tools.GetMessageById ("21013");
						//						Dictionary<string,object> ddd = new Dictionary<string,object> ();
						//						ddd.Add (_id, _num);
						if (_id == Config.ASSET_ELSCORE) {
							c2.selectedIndex = 4;
							comIcon.SetData (_id, _num);
						} else {
							if (_id == Config.ASSET_GOLD) {
								c2.selectedIndex = 0;
							} else if (_id == Config.ASSET_COIN) {
								c2.selectedIndex = 1;
							}
							comIcon.SetData (_id, Convert.ToInt32 (_num));
						}
					}
					DOTween.To (() => y, x => y = x, 0, 0.5f).OnUpdate (() => {
						comIcon.rotationY = y;
					});
				});
			}
			comIcon.scale = new Vector2 (0.5f, 0.25f);
			comIcon.TweenScale (new Vector2 (1f, 1f), 0.25f);
			EffectManager.inst.Bezier (this.item, 0.5f, new Vector2 (Tools.offectSetX (230f), 326f), new Vector2 (350f, 0f), new Vector2 (Tools.offectSetX (630f), 200f), () => {
//					TimerManager.inst.Add (0.01f, 0, OnTimerFunction);
				DOTween.Kill(this.item);
				//starImg.TweenScale (new Vector2 (1.2f, 1.2f), 0.5f);
				TimerManager.inst.Remove (effect);
				effect = TimerManager.inst.Add (0.1f, 1, (float eee) => {
					if (this.group == null) {
						return;
					}
					if (canClick) {
						if (_id.StartsWith ("C") && _id != Config.ASSET_CARD) {
							comIcon.SetData (_id, _num, 2);
							CardVo ava = DataManager.inst.GetCardVo (_id);
							if (ava.lv == 1 && ava.exp == Convert.ToInt32 (_num)) {
								item.GetChild ("n13").asTextField.visible = true;
							}
							ComProgressBar pro = item.GetChild ("n8") as ComProgressBar;
							mpro = pro;
							pro.max = ava.maxExp;
							pro.value = ava.exp;
							upImg = item.GetChild ("n11").asImage;
							if (pro.max <= pro.value) {
								EffectManager.inst.TweenJump (upImg, 1f);
								upImg.visible = true;
								pro.skin = ComProgressBar.BAR6;
							} else {
								pro.skin = ComProgressBar.BAR3;
							}
						} else {
//								comIcon.SetData (_id,ModelManager.inst.userModel.GetIconNumNow (_id));
							if (_id == Config.ASSET_GOLD) {
								mpro = item.GetChild ("n6").asCom;
								mprotxt = item.GetChild ("n6").asCom.GetChild ("n0").asTextField;
								mprotxt.text = ModelManager.inst.userModel.GetIconNumNow (_id).ToString ();
							} else if (_id == Config.ASSET_COIN) {
								mpro = item.GetChild ("n7").asCom;
								mprotxt = item.GetChild ("n7").asCom.GetChild ("n0").asTextField;
								mprotxt.text = ModelManager.inst.userModel.GetIconNumNow (_id).ToString ();
							} else if (_id == Config.ASSET_ELSCORE) {
								mpro = item.GetChild ("n9").asCom;
								mprotxt = item.GetChild ("n9").asCom.GetChild ("n1").asTextField;
								maxExp = (int)((object[])DataManager.inst.systemSimple ["el_score"]) [1];
								comIcon.SetData (_id, (ModelManager.inst.userModel.GetIconNumNow (_id) + Convert.ToInt32 (_num) > (int)((object[])DataManager.inst.systemSimple ["el_score"]) [1] ? (int)((object[])DataManager.inst.systemSimple ["el_score"]) [1] : ModelManager.inst.userModel.GetIconNumNow (_id) + Convert.ToInt32 (_num)));
								(mpro as ComProgressBar).value = (ModelManager.inst.userModel.GetIconNumNow (_id) + Convert.ToInt32 (_num) > (int)((object[])DataManager.inst.systemSimple ["el_score"]) [1] ? (int)((object[])DataManager.inst.systemSimple ["el_score"]) [1] : ModelManager.inst.userModel.GetIconNumNow (_id) + Convert.ToInt32 (_num));
								(mpro as ComProgressBar).max = maxExp;
								if ((mpro as ComProgressBar).value == (mpro as ComProgressBar).max) {
									ColorFilter gggg = new ColorFilter ();
									(mpro as ComProgressBar).GetBar ().filter = gggg;
									gggg.Reset ();
									gggg.AdjustBrightness (0.1f);
									gggg.AdjustContrast (0.5f);
									gggg.AdjustSaturation (0.5f);
									gggg.AdjustHue (-0.2f);
								}
//									mprotxt.text = (ModelManager.inst.userModel.GetIconNumNow (_id)+Convert.ToInt32(_num)>(int)((object[])DataManager.inst.systemSimple["el_score"])[1]?(int)((object[])DataManager.inst.systemSimple["el_score"])[1]:ModelManager.inst.userModel.GetIconNumNow (_id)+Convert.ToInt32(_num))+"/"+(int)((object[])DataManager.inst.systemSimple["el_score"])[1];
							} else if (_id == Config.ASSET_RANKSCORE) {
								mpro = item.GetChild ("n14").asCom;
								mprotxt = item.GetChild ("n14").asCom.GetChild ("n2").asTextField;
								mprotxt.text = ModelManager.inst.userModel.GetIconNumNow (_id).ToString ();
							}
						}
					} else {
						if (_id.StartsWith ("C") && _id != Config.ASSET_CARD) {
							try{
							CardVo ava = DataManager.inst.GetCardVo (_id);
							if (ava.lv == 1 && ava.exp == Convert.ToInt32 (_num)) {
								item.GetChild ("n13").asTextField.visible = true;
								ClickBtn.touchable = false;
								TimerManager.inst.Add (1.5f, 1, (float ff) => {
									ClickBtn.touchable = true;
								});
							}

							ComProgressBar pro = item.GetChild ("n8") as ComProgressBar;
							mpro = pro;
							pro.max = ava.maxExp;
							pro.value = ava.exp - Convert.ToInt32 (_num);
							if (pro.max <= pro.value) {
								pro.skin = ComProgressBar.BAR6;
							} else {
								pro.skin = ComProgressBar.BAR3;
							}
							upImg = item.GetChild ("n11").asImage;
							if (ava.lv == ava.maxLv && ava.exp == ava.maxExp) {
								pro.value = ava.exp;
							} else {
								MoveToCard (ava.exp - Convert.ToInt32 (_num), ava.exp);
							}
							}catch(Exception ex){}
						} else {
							try{
							if (_id == Config.ASSET_GOLD) {
								mpro = item.GetChild ("n6").asCom;
								mprotxt = item.GetChild ("n6").asCom.GetChild ("n0").asTextField;
								mprotxt.text = (ModelManager.inst.userModel.GetIconNumNow (_id) - Convert.ToInt32 (_num)).ToString ();
							} else if (_id == Config.ASSET_COIN) {
								mpro = item.GetChild ("n7").asCom;
								mprotxt = item.GetChild ("n7").asCom.GetChild ("n0").asTextField;
								mprotxt.text = (ModelManager.inst.userModel.GetIconNumNow (_id) - Convert.ToInt32 (_num)).ToString ();
							} else if (_id == Config.ASSET_RANKSCORE) {
								mpro = item.GetChild ("n14").asCom;
								mprotxt = item.GetChild ("n14").asCom.GetChild ("n2").asTextField;
								mprotxt.text = (ModelManager.inst.userModel.GetIconNumNow (_id) - Convert.ToInt32 (_num)).ToString ();
							} else if (_id == Config.ASSET_ELSCORE) {
								mpro = item.GetChild ("n9").asCom;
								mprotxt = item.GetChild ("n9").asCom.GetChild ("n1").asTextField;
								maxExp = (int)((object[])DataManager.inst.systemSimple ["el_score"]) [1];
								(mpro as ComProgressBar).value = ModelManager.inst.userModel.GetIconNumNow (_id);
								(mpro as ComProgressBar).max = maxExp;
//									mprotxt.text = ModelManager.inst.userModel.GetIconNumNow (_id) + "/"+ maxExp;
								int tem = (ModelManager.inst.userModel.GetIconNumNow (_id) + Convert.ToInt64 (_num) > (int)((object[])DataManager.inst.systemSimple ["el_score"]) [1] ? (int)((object[])DataManager.inst.systemSimple ["el_score"]) [1] : ModelManager.inst.userModel.GetIconNumNow (_id) + Convert.ToInt32 (_num));
								MoveToExp (ModelManager.inst.userModel.GetIconNumNow (_id), tem);
							}
							if (_id.StartsWith ("s") && _id != Config.ASSET_ELSCORE && _id != Config.ASSET_RANKSCORE || _id == Config.ASSET_ELSCORE) {

							} else {
								MoveToExp (ModelManager.inst.userModel.GetIconNumNow (_id) - Convert.ToInt32 (_num), ModelManager.inst.userModel.GetIconNumNow (_id));
							}
							}catch(Exception exx){}
						}

					}
					try{
					if(name!=null)name.visible = true;
					if(info!=null)info.visible = true;
//						num.visible = true;
					if(starImg!=null)starImg.visible = true;
//						canClick = false;
					}catch(Exception exxx){}
				});

			});	
		});
        //		});	
		return _id;
	}
	private void MoveToCard(int num,int toNum)
	{
		mNum = toNum;
		cha = Convert.ToInt32 (Math.Ceiling ((toNum - num) / 4f));
		TimerManager.inst.Add (0.4f, 1, TimerFunction2);
	}
	private void TimerFunction2(float ff)
	{
		MoveToCardNum (mNum);
	}
	private void MoveToCardNum(int num)
	{
		ComProgressBar com = mpro as ComProgressBar;
		if (com.value >= num) {
			com.value = num;
			if (com.max <= com.value) {
				upImg.visible = true;
				EffectManager.inst.TweenJump (upImg, 1f);
			}
			return;
		}
		upImg.visible = false;
		com.value = (com.value + cha);
		if (com.max <= com.value) {
			com.skin = ComProgressBar.BAR6;
		} else {
			com.skin = ComProgressBar.BAR3;
		}
		com.TweenScaleX (1.1f, 0.05f).OnComplete (() =>
			{
				com.TweenScaleX (1f, 0.05f).OnComplete (() =>
					{
						MoveToCardNum (num);
					});
			});
	}
	public void MoveToExp (int num,int toNum)
	{
		mNum = toNum;
		cha = Convert.ToInt32 (Math.Ceiling ((toNum - num) / 4f));
		TimerManager.inst.Add (0.4f, 1, TimerFunction);
	}
	private void TimerFunction(float ff)
	{
		MoveToNum (mNum);
	}
	private void MoveToNum(int num)
	{
		if (Convert.ToInt32 (mprotxt.text.Split('/')[0]) >= num) {
			if (maxExp != -1) {
				(mpro as ComProgressBar).value = num;
				(mpro as ComProgressBar).max = maxExp;
				mprotxt.text = num.ToString () + "/" + maxExp;
				if((mpro as ComProgressBar).value == (mpro as ComProgressBar).max)
				{
					ColorFilter gggg = new ColorFilter();
					(mpro as ComProgressBar).GetBar().filter = gggg;
					gggg.Reset ();
					gggg.AdjustBrightness (0.1f);
					gggg.AdjustContrast (0.5f);
					gggg.AdjustSaturation (0.5f);
					gggg.AdjustHue (-0.2f);
				}
			} else {
				mprotxt.text = num.ToString();
			}
			return;
		}
		if (maxExp != -1) {
			(mpro as ComProgressBar).value = Convert.ToInt32 (mprotxt.text.Split ('/') [0]) + cha;
			(mpro as ComProgressBar).max = maxExp;
			if((mpro as ComProgressBar).value == (mpro as ComProgressBar).max)
			{
				ColorFilter gggg = new ColorFilter();
				(mpro as ComProgressBar).GetBar().filter = gggg;
				gggg.Reset ();
				gggg.AdjustBrightness (0.1f);
				gggg.AdjustContrast (0.5f);
				gggg.AdjustSaturation (0.5f);
				gggg.AdjustHue (-0.2f);
			}
			mprotxt.text = (Convert.ToInt32 (mprotxt.text.Split('/')[0]) + cha).ToString () + "/" + maxExp;
		} else {
			mprotxt.text = (Convert.ToInt32 (mprotxt.text.Split('/')[0]) + cha).ToString ();
		}

		mpro.TweenScaleX (1.1f, 0.05f).OnComplete (() =>
			{
				mpro.TweenScaleX (1f, 0.05f).OnComplete (() =>
					{
						MoveToNum (num);
					});
			});
	}
	private void OnFFFFFFFFF(float go)
	{
		if (shipobj != null) {
			shipobj.transform.parent.transform.Rotate (new Vector3 (0, 1, 0));
		} else {
			TimerManager.inst.Remove (OnFFFFFFFFF);
		}

	}
	private void OnTimerFunction(float time)
	{
		starImg.rotation += 1f;
	}
	private GameObject pro;
	private string nowID = "";
    private Action<float> effect;

    private void Mask_Click ()
	{
		if (canClick) {
			if (nowIndex >= newdata.Count) {
				ViewManager.inst.CloseView (this);
				_gold = -1;
				_coin = -1;
				_el_score = -1;
				dataShip = null;
				if (isExplore) {
					this.DispatchGlobalEvent (new MainEvent (MainEvent.EXPLORE_GIFT));
				}
				isExplore = false;
				return;
			}
			L_Label.text = (newdata.Count - nowIndex - 1) == 0 ? "" : (newdata.Count - nowIndex - 1) + "";
			canClick = false;
			allClick = false;
			nowID = showItem ((Dictionary<string,object>)newdata [nowIndex], false, openTimes);
			nowIndex++;
		} else {
			if (allClick) {
				if (nowID.StartsWith ("s") && nowID != Config.ASSET_ELSCORE && nowID != Config.ASSET_RANKSCORE)
					return;
				TimerManager.inst.Remove(TimerFunction);
				TimerManager.inst.Remove(TimerFunction2);
				allClick = false;
				canClick = true;
				DOTween.KillAll (true);
				ComBigIcon comIcon = item.GetChild ("n4") as ComBigIcon;
				comIcon.rotationY = 0;
			}
		}
	}
	public override void Clear ()
	{
		ViewManager.inst.s.displayObject.gameObject.SetActive (true);
		if (comIcon != null) {
			comIcon.reMoveTimer ();
		}
		TimerManager.inst.Remove(TimerFunction);
		TimerManager.inst.Remove(TimerFunction2);
		TimerManager.inst.Remove (OnTimerFunction);
		TimerManager.inst.Remove (effect);
        this.DispatchGlobalEvent (new MainEvent (MainEvent.RED_UPDATE));
        if(ModelManager.inst.userModel.isShowText) {//满卡之后 多余的卡牌数转化成金币提示消息
            ViewManager.inst.ShowText(Tools.GetMessageById("33212"));
            ModelManager.inst.userModel.isShowText = false;
        }
        base.Clear ();
	}
}
