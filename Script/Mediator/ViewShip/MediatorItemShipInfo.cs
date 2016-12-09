using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using FairyGUI;
using DG.Tweening;

public class MediatorItemShipInfo : BaseMediator
{

	private Dictionary<string,object> sim;

	public static string CID = "";
	public static int isKu = 0;
//0牌库1卡组2为拥有3别人的
	public static string changeCID = "";
	public static int lv = -1;

	private GButton btn_up;
	private GButton btn_change;
	private GTextField l_giveRed;

	private CardVo card;
	private GLoader proBar;
	private bool infobar = false;

	public override void Init ()
	{
		base.Init ();
		if (isKu == 2)
		{
			card = DataManager.inst.GetCardVo (CID, 1);
		}
		else
		{
			card = DataManager.inst.GetCardVo (CID, lv);
		}
		this.Create (Config.VIEW_ITEMSHIPINFO, false);
//		, Tools.GetMessageById (card.name)
		sim = (Dictionary<string,object>)(DataManager.inst.systemSimple ["card_attr_info"]);

		btn_up = this.GetChild ("n15").asButton;
		btn_up.onClick.Add (OnUpCardHandler);
		btn_change = this.GetChild ("n14").asButton;
		btn_change.text = Tools.GetMessageById ("24132");
		btn_change.onClick.Add (OnChangeHandler);

		GImage img_upbuy = this.GetChild ("n15").asCom.GetChild ("n3").asImage;
		GTextField l_getredcoin = this.GetChild ("n25").asTextField;
		GButton img_getredcoin = this.GetChild ("n16").asButton;
//		GImage img_getred = view.GetChild ("n27").asImage;
		GTextField title = this.GetChild ("n64").asTextField;
		GTextField card_lv = this.GetChild ("n54").asTextField;
		GTextField info = this.GetChild ("n12").asTextField;
		GLoader img_rarity = this.GetChild ("n3").asLoader;
		GTextField di = this.GetChild ("n55").asTextField;

		ViewManager.inst.AddTouchTip ("GButton", img_getredcoin, Config.ASSET_REDBAGCOIN);

		btn_up.text = "";
		this.GetChild ("n73").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_d" + card.armor.ToString ());
		this.GetChild ("n52").asTextField.text = card.GetCost ();
		title.text = Tools.GetMessageById(card.name);
		card_lv.text = card.lv.ToString ();
		info.text = Tools.GetMessageById (card.info);
		l_giveRed = this.GetChild ("n26").asTextField;
		l_giveRed.text = "x" + card.getUpLevelGiveRed ();
		GTextField gold = this.GetChild ("n15").asCom.GetChild ("n5").asTextField;
		gold.text = Tools.GetMessageById ("24123", new string[]{ card.getUpLevelNeed () + "" });
		GTextField ssss = this.GetChild ("n15").asCom.GetChild ("n4").asTextField;
		ssss.text = Tools.GetMessageById ("24131");
		img_rarity.url = Tools.GetResourceUrl ("Image2:n_icon_dengji" + (card.rarity + 1) + "_");
		di.text = "";
		this.GetChild ("n72").asTextField.text = Tools.GetMessageById ("24134", new string[]{ card.cd.ToString () });
		GTextField l_rarity = this.GetChild ("n6").asTextField;
		l_rarity.text = Tools.GetMessageById ("13051", new string[]{ CardVo.GetRarityMss (card.rarity) });

		GTextField l_type = this.GetChild ("n5").asTextField;
		l_type.text = CardVo.GetArmorMss (card.armor);
		l_getredcoin.text = Tools.GetMessageById ("24130");
		if (card.getUpLevelNeed () == 0)
		{
			img_upbuy.visible = false;
			gold.text = "";
			ssss.text = "";
			btn_up.text = Tools.GetMessageById ("14012");
		}
		if (card.lv >= card.maxLv)
		{
			btn_up.visible = false;
			img_upbuy.visible = false;
			img_getredcoin.visible = false;
			l_giveRed.text = "";
			Dictionary<string,object> cfg = DataManager.inst.systemSimple;
			Dictionary<string,object> card_exp = (Dictionary<string,object>)cfg ["card_lv_exp"];
			card_exp = (Dictionary<string,object>)card_exp [card.rarity + ""];
			l_getredcoin.text = "";
			di.text = Tools.GetMessageById ("13050", new string[] {
				((object[])card_exp ["lv_max"]) [1].ToString (),
				((object[])card_exp ["lv_max"]) [0].ToString ()
			});

		}
		if (card.exp < card.maxExp)
		{
			btn_up.enabled = false;
			img_upbuy.grayed = true;
			gold.grayed = true;
			ssss.grayed = true;

		}
		if (card.rarity == 3) {
//			EffectManager.inst.SetRainbowBar (this.GetChild ("n69").asCom);
			//		n70
			GGraph ggg = this.GetChild("n70").asGraph;
			float mm = 125f;
			float mmm = 125f;
			float cc = 5f;
			float[] rgbA = new float[]{1f,1f,0f,mm,mm,0f,0f};
			float[] rgbB = new float[]{1f,1f,0f,mm,mm-40f,0f,0f};

			Color ccs = Color.white;
			Color ccs2 = Color.white;
			Color[] ccc = new Color[]{ ccs, ccs, ccs2, ccs2};
			float[] nullColor = new float[]{ 1, 0, 1 };
			TimerManager.inst.Add (0.04f, 0, (float x) => {
				rgbA = Tools.GetRGB(mm,cc,ref rgbA,nullColor);
				rgbB = Tools.GetRGB(mm,cc,ref rgbB,nullColor);

				ccs.r = rgbA[3]/mmm;
				ccs.g = rgbA[4]/mmm;
				ccs.b = rgbA[5]/mmm;

				ccs2.r = rgbB[3]/mmm;
				ccs2.g = rgbB[4]/mmm;
				ccs2.b = rgbB[5]/mmm;

				ccc[0] = ccs;
				ccc[1] = ccs;
				ccc[2] = ccs2;
				ccc[3] = ccs2;

				ggg.shape.DrawRect (0, ccc);

			});
//			this.GetChild ("n69").asCom.visible = true;
		} else {
//			this.GetChild ("n69").asCom.visible = false;
			this.GetChild ("n70").visible = false;
		}
//		else if (!ModelUser.GetCanBuy (Config.ASSET_GOLD, card.getUpLevelNeed (), false))
//		{
//			btn_up.text = Tools.GetMessageColor (Tools.GetMessageById ("19009", new string[]{ card.getUpLevelNeed () + "" }), new string[]{ "ff0000" });
//		}
		if (isKu == 1)
		{
			btn_change.visible = false;
		}
		(this.GetChild ("n56") as ComCard).SetData (CID);
		if (isKu == 2 || isKu == 3)
		{
			btn_change.visible = false;
			img_upbuy.visible = false;
			btn_up.visible = false;
			l_giveRed.text = "";
			l_getredcoin.text = "";
			gold.text = "";
			ssss.text = "";
			img_getredcoin.visible = false;
			if (isKu == 2)
			{
				(this.GetChild ("n56") as ComCard).SetData (CID, -1, 4, 1);
				(this.GetChild ("n56") as ComCard).SetText (Tools.GetMessageById (card.name));
			}
			else
			{
				(this.GetChild ("n56") as ComCard).SetData (CID, -1, 4, lv);
				(this.GetChild ("n56") as ComCard).SetText (Tools.GetMessageById ("14015", new string[]{ card.lv.ToString () }));
			}
//			this.height = 490;
		}
		if (isKu == 4)
		{
			btn_change.visible = false;
			img_upbuy.visible = false;
			btn_up.visible = false;
			l_giveRed.text = "";
			l_getredcoin.text = "";
			gold.text = "";
			ssss.text = "";
			img_getredcoin.visible = false;
			if (card.have)
			{
				(this.GetChild ("n56") as ComCard).SetData (CID);
			}
			else
			{
				(this.GetChild ("n56") as ComCard).SetData (CID, -1, 4, 1);
				(this.GetChild ("n56") as ComCard).SetText (Tools.GetMessageById ("13053"));
			}
//			this.height = 490;
		}
		this.group.Center ();
		if (img_upbuy.visible && btn_up.enabled)
		{
			di.text = Tools.GetMessageById ("24118", new string[] {
				CardVo.GetRarityMss (card.rarity),
				(card.lv + 1).ToString ()
			});
		}
		FushDataToShow (CID);
		if (GuideManager.inst.Check ("1:1"))
		{
			if (btn_up.enabled) {
				GuideManager.inst.Next ();
				GuideManager.inst.Show (this);
			} else {
				GuideManager.inst.Clear ();
			}
		}
		this.GetChild ("n58").asButton.onClick.Add (() => {
			ViewManager.inst.CloseView(this);
		});
		this.GetChild("n61").asButton.onClick.Add(()=>{
//			this.GetController("c1").selectedIndex = this.GetController("c1").selectedIndex == 1?0:1;
			infobar = !infobar;
			if(infobar == false)
			{
				float c = this.width;
				DOTween.To (() => c, x => c = x, 543, 0.2f).OnUpdate (() =>
				{
					this.width = c;
				}).OnComplete(()=>{
					this.GetController("c1").selectedIndex = 0;	
				});
			}else{
				float c = this.width;
				this.GetController("c1").selectedIndex = 1;	
				DOTween.To (() => c, x => c = x, 878, 0.2f).OnUpdate (() =>
				{
					this.width = c;
				});
			}
		});



		//


	}

	private void OnUpCardHandler ()
	{
		CardVo card = DataManager.inst.GetCardVo (CID);
		if (GuideManager.inst.Check ("1:2"))
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic["card_id"] = CID;
			dic["guide_num"] = 2;
//			GuideManager.inst.Next ();
//			GuideManager.inst.Show (this);
			ModelManager.inst.userModel.SetGuide (2, OnCardLvupHandler, dic);
			return;
		}
		if (ModelUser.GetCanBuy (Config.ASSET_GOLD, card.getUpLevelNeed (),"24129"))
			NetHttp.inst.Send (NetBase.HTTP_CARD_LVUP, "cid=" + CID, OnCardLvupHandler);
	}

	private void OnCardLvupHandler (VoHttp vo)
	{
		if (GuideManager.inst.Check ("2:0"))
		{
			GuideManager.inst.Clear ();
		}
		ModelManager.inst.userModel.UpdateData (vo.data);
		MediatorLevelUp.Cid = CID;
		ViewManager.inst.CloseView (this);
		ViewManager.inst.ShowView<MediatorLevelUp> (false);
	}

	private void OnChangeHandler ()
	{
		changeCID = CID;
		ViewManager.inst.CloseView (this);
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.CARD_CHANGE));
	}

	private object[] tips;
	private Dictionary<string,object> cardData;
	private string _cid = "";

	private void FushDataToShow (string cid)
	{
		_cid = cid;
		GList list = this.GetChild ("n49").asList;
		list.itemRenderer = onListRander;
		cardData = (Dictionary<string,object>)DataManager.inst.card [cid];
		tips = (object[])cardData ["tips"];
		list.numItems = tips.Length;
//		int hang = list.numItems / 2 + (list.numItems % 2 == 0 ? 0 : 1);
//		if (hang < 4) {
//			list.y += (40 / 2) * (4-hang);
//		}
	}

	public static string[] TextFild (int index, CardVo card, GButton n3 = null, GComponent _view = null, bool isUp = true,bool careExp = true)
	{
		string n1 = "";
		Dictionary<string,object> cardData = (Dictionary<string,object>)DataManager.inst.card [card.id];
		Dictionary<string,object> sim = (Dictionary<string,object>)(DataManager.inst.systemSimple ["card_attr_info"]);
		object[] tips = (object[])cardData ["tips"];
		Dictionary<string,object> tipi = (Dictionary<string,object>)tips [index];
		Dictionary<string,object> up;
		if (cardData.ContainsKey ("up"))
		{
			up = (Dictionary<string,object>)cardData ["up"];
		}
		if (n3 != null)
		{
			n3.visible = false;
		}
		bool bo = false;
		Dictionary<string,object> monster = new Dictionary<string, object> ();
		string carr = tipi.ContainsKey ("call") ? tipi ["call"].ToString () : null;
		string callin = tipi.ContainsKey ("callin") ? tipi ["callin"].ToString () : null;
		if (carr != null)
		{
			monster = (Dictionary<string,object>)DataManager.inst.beckon [carr];
		}
		if (callin != null)
		{
			if (n3 != null)
			{
				n3.visible = true;
				n3.onClick.Add (() =>
				{
					ModelManager.inst.cardModel.SetComShipInfoData (callin, tipi ["title"].ToString (), cardData, card);
					Vector2 v2 = Stage.inst.touchPosition;
//					MediatorComShipInfo bbb = ViewManager.inst.ShowView<MediatorComShipInfo> (false) as MediatorComShipInfo;
					MediatorComShipInfo bbb = new MediatorComShipInfo();
					ViewManager.inst.AddPView(bbb);
					v2 = _view.parent.GlobalToLocal (v2);
					bbb.group.x = v2.x - bbb.group.width + 190 + 190;
					bbb.group.y = v2.y - bbb.GetChild("n9").y - 5;
				});
			}
		}
		string fa1 = null;
		string fa2 = null;
		int round = 0;
		if (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]).ContainsKey ("round"))
		{
			round = Convert.ToInt32 (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["round"]);
		}
		if (tipi.ContainsKey ("replace"))
		{
			object[] replace = (object[])tipi ["replace"];
			string[] strArr = new string[replace.Length];
			string[] strArr2 = new string[replace.Length];
			for (int j = 0; j < replace.Length; j++)
			{
				string[] fafa;
				if (carr != null)
				{
					fafa = suanShu ((object[])replace [j], monster, card, round);
				}
				else
				{
					fafa = suanShu ((object[])replace [j], cardData, card, round);
				}
				strArr [j] = fafa [0];
				if (fafa.Length > 1)
				{
					bo = true;
					if (careExp) {
						if (card.exp < card.maxExp || card.lv == card.maxLv)
						{
							bo = false;
						}
					}
					strArr2 [j] = fafa [1];
				}
			}
			n1 = Tools.GetMessageById (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["info"].ToString (), strArr);
			fa1 = n1;
			if (bo && (isKu == 0 || isKu == 1) && isUp)
			{
				n1 = "";
//				n1 += "[color=#0cff00]";
				n1 += "+"+Tools.GetMessageById (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["info"].ToString (), strArr2);
//				n1 += "[/color]";
				fa2 = n1;
			}
			if (callin != null) {
				
			}
		}
		else
		{
			n1 = Tools.GetMessageById (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["info"].ToString ());
			fa1 = n1;
		}
		if (fa2 == null)
		{
			return new string[]{ fa1 };
		}
		else
		{
			return new string[]{ fa1, fa2 };
		}


	}

	private void onListRander (int index, GObject obj)
	{
		GLoader n0 = obj.asCom.GetChild ("n0").asLoader;
		GTextField n1 = obj.asCom.GetChild ("n1").asTextField;
		GTextField n2 = obj.asCom.GetChild ("n2").asTextField;
		GButton n3 = obj.asCom.GetChild ("n3").asButton;
		GTextField n4 = obj.asCom.GetChild ("n4").asTextField;

		Dictionary<string,object> tipi = (Dictionary<string,object>)tips [index];
		n3.RemoveEventListeners ();
		n1.text = Tools.GetMessageById (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["name"].ToString ()) + ":";
		string[] arr = TextFild (index, card, n3, group);
		n2.text = arr [0];
		if (arr.Length != 1)
		{
			n4.text = arr [1];
		}
		else
		{
			n4.text = "";
		}
		n0.url = Tools.GetResourceUrl ("Icon:" + ((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["icon"].ToString ());
	}

	private void setItemData (string id)
	{
		Dictionary<string,object> cfg = DataManager.inst.systemSimple;
		GLoader ra = this.GetChild ("n8").asLoader;
		GLoader icon = this.GetChild ("n9").asLoader;
		ComProgressBar pro = this.GetChild ("n10") as ComProgressBar;

		proBar = pro.GetBar ();

		ra.url = Tools.GetResourceUrl ("Image:bg_kapai" + card.rarity);
		icon.url = Tools.GetResourceUrl ("Icon:" + id);

		GTextField proText = pro.GetChild ("n1").asTextField;

		Dictionary<string,object> exp_cfg = (Dictionary<string,object>)(cfg ["card_lv_exp"]);
		exp_cfg = (Dictionary<string,object>)exp_cfg [card.rarity + ""];

		pro.max = card.maxExp;
		pro.value = card.exp;
		if (card.lv >= card.maxLv)
		{
			proText.color = new Color (1, 1, 0);
		}
		else
		{
			if (card.exp < card.maxExp)
			{
				pro.skin = ComProgressBar.BAR3;
			}
			else
			{
				pro.skin = ComProgressBar.BAR4;
			}
		}
	}
/*
	public static string[] suanShu (object[] arr, Dictionary<string,object> dic, CardVo card, int round,bool isChild = false)
	{
		double num = 0f;
		string fuhao = "";
		double shengji = 0f;

		Dictionary<string,object> up = null;
		if (dic.ContainsKey ("up"))
		{
			up = (Dictionary<string,object>)dic ["up"];
		}
		for (int i = 0; i < arr.Length; i++)
		{
			if (arr [i] is string)
			{
				if ((arr [i].ToString ()).IndexOf ("/") != -1 || (arr [i].ToString ()).IndexOf ("*") != -1 || (arr [i].ToString ()).IndexOf ("-") != -1 || (arr [i].ToString ()).IndexOf ("+") != -1)
				{
					fuhao = arr [i].ToString ();
				}
				else
				{
					object en;
//					if ((arr [i].ToString ()).IndexOf ("!lv") != -1) {
//						en = card.lv;
//					}else{
					en = Tools.Analysis (dic, (arr [i]).ToString ());
//					}
					double zzz = 0f;
					if (up != null && up.ContainsKey ((arr [i].ToString ())))
					{
						Dictionary<string,object> fafa = (Dictionary<string,object>)up [arr [i].ToString ()];
						if (fafa.ContainsKey ("add"))
						{
							int lvA = card.lv - 1;
							int lvB = card.lv;
							if (isChild) {
								lvA = card.GetChildLv (card.lv, 1);
								lvB = card.GetChildLv (card.lv+1, 1);
								lvA -= 1;
								lvB -= 1;
							}
							zzz = Convert.ToSingle (Convert.ToSingle (en) * Math.Pow (Convert.ToSingle (fafa ["power"]), Convert.ToSingle (lvA)) + Convert.ToSingle (fafa ["add"]) * Convert.ToSingle (lvA));
							shengji = Convert.ToSingle (Convert.ToSingle (en) * Math.Pow (Convert.ToSingle (fafa ["power"]), Convert.ToSingle (lvB)) + Convert.ToSingle (fafa ["add"]) * Convert.ToSingle (lvB) - zzz);
						}
						else if (fafa.ContainsKey ("fixed"))
						{
							zzz = Convert.ToSingle (en);
							for (int ee = 0; ee < ((object[])fafa ["fixed"]).Length; ee++)
							{
								object[] abcde = (object[])((object[])fafa ["fixed"]) [ee];
								if ((card.lv) >= (int)abcde [0])
								{
									zzz = Math.Floor(Convert.ToDouble(abcde [1]));
								}
								if ((card.lv) < (int)abcde [0] && (card.lv + 1) >= (int)abcde [0])
								{
									shengji = Math.Floor(Convert.ToDouble(abcde [1])) - Math.Floor(zzz);
								}
							}
						}
					}
					if (fuhao != "")
					{
						switch (fuhao)
						{
						case "+":
							num = Math.Floor(num) + Math.Floor(Convert.ToSingle (en));
							shengji = Math.Floor(shengji) + Math.Floor(Convert.ToDouble (en));
							break;
						case "-":
							num = Math.Floor(num) - Math.Floor(Convert.ToSingle (en));
							shengji = Math.Floor(shengji) - Math.Floor(Convert.ToDouble (en));
							break;
						case "*":
							num = Math.Floor(num) * Math.Floor(Convert.ToSingle (en));
							shengji = Math.Floor(shengji) * Math.Floor(Convert.ToDouble (en));
							break;
						case "/":
							num = Math.Floor(num) / Math.Floor(Convert.ToSingle (en));
							shengji = Math.Floor(shengji) / Math.Floor(Convert.ToDouble (en));
							break;
						}
						fuhao = "";
					}
					else
					{
						if (zzz != 0)
						{
							num = Math.Floor(zzz);
						}
						else
						{
							num = Math.Floor(Convert.ToDouble (en));
						}

					}
				}
			}
			else
			{
				switch (fuhao)
				{
				case "+":
					num = num + Convert.ToSingle (arr [i]);
					shengji = Math.Floor(shengji) + Math.Floor(Convert.ToDouble (arr [i]));
					break;
				case "-":
					num = num - Convert.ToSingle (arr [i]);
					shengji = Math.Floor(shengji) - Math.Floor(Convert.ToDouble (arr [i]));
					break;
				case "*":
					num = num * Convert.ToSingle (arr [i]);
					shengji = Math.Floor(shengji) * Math.Floor(Convert.ToDouble (arr [i]));
					break;
				case "/":
					num = num / Convert.ToSingle (arr [i]);
					shengji = Math.Floor(shengji) / Math.Floor(Convert.ToDouble (arr [i]));
					break;
				}
				fuhao = "";
			}
		}
		string rr = "0.";
		if (round > 0)
		{
			for (int jj = 0; jj < round; jj++)
			{
				rr += "0";
			}
		}
		if (shengji != 0)
		{
			return new string[]{ num.ToString (rr), shengji.ToString (rr) };
		}
		return new string[]{ num.ToString (rr) };
	}
*/


	public static string[] suanShu (object[] arr, Dictionary<string,object> dic, CardVo card, int round,bool isChild = false)
	{
		int shengji = 0;
		int num = suanShu_get (arr, dic, card, card.lv, round, isChild);
		int b = suanShu_get (arr, dic, card, card.lv + 1, round, isChild);

//		string rr = "0.";
//		if (round > 0)
//		{
//			for (int jj = 0; jj < round; jj++)
//			{
//				rr += "0";
//			}
//		}
//		num = (float)Math.Floor (num);
//		Debug.LogError (shengji);
		shengji = b-num;//(float)Math.Round (shengji);
		if (shengji > 0)
		{
			return new string[]{ num + "", shengji+"" };
		}
		return new string[]{ num + "" };
	}
	public static int suanShu_get(object[] arr, Dictionary<string,object> dic, CardVo card, int lv,int round,bool isChild = false){
		float num = 0f;
		string fuhao = "";
//		float shengji = 0f;
		int _lv = lv;
		Dictionary<string,object> up = null;
		if (dic.ContainsKey ("up"))
		{
			up = (Dictionary<string,object>)dic ["up"];
		}
		for (int i = 0; i < arr.Length; i++)
		{
			if (arr [i] is string)
			{
				if ((arr [i].ToString ()).IndexOf ("/") != -1 || (arr [i].ToString ()).IndexOf ("*") != -1 || (arr [i].ToString ()).IndexOf ("-") != -1 || (arr [i].ToString ()).IndexOf ("+") != -1)
				{
					fuhao = arr [i].ToString ();
				}
				else
				{
					object en;
					//					if ((arr [i].ToString ()).IndexOf ("!lv") != -1) {
					//						en = card.lv;
					//					}else{
					en = Tools.Analysis (dic, (arr [i]).ToString ());
					//					}
					float zzz = 0f;
					if (up != null && up.ContainsKey ((arr [i].ToString ())))
					{
						Dictionary<string,object> fafa = (Dictionary<string,object>)up [arr [i].ToString ()];
						if (fafa.ContainsKey ("add"))
						{
							int lvA = _lv - 1;
							int lvB = _lv;
							if (isChild) {
								lvA = card.GetChildLv (_lv, 1);
								lvB = card.GetChildLv (_lv+1, 1);
								lvA -= 1;
								lvB -= 1;
							}
							zzz = Convert.ToSingle (Convert.ToSingle (en) * Math.Pow (Convert.ToSingle (fafa ["power"]), Convert.ToSingle (lvA)) + Convert.ToSingle (fafa ["add"]) * Convert.ToSingle (lvA));
//							shengji = Convert.ToSingle (Convert.ToSingle (en) * Math.Pow (Convert.ToSingle (fafa ["power"]), Convert.ToSingle (lvB)) + Convert.ToSingle (fafa ["add"]) * Convert.ToSingle (lvB) - zzz);

						}
						else if (fafa.ContainsKey ("fixed"))
						{
							zzz = Convert.ToSingle (en);
							for (int ee = 0; ee < ((object[])fafa ["fixed"]).Length; ee++)
							{
								object[] abcde = (object[])((object[])fafa ["fixed"]) [ee];
								if ((_lv) >= (int)abcde [0])
								{
									zzz = (int)abcde [1];
								}
//								if ((_lv) < (int)abcde [0] && (_lv + 1) >= (int)abcde [0])
//								{
//									//									shengji = Convert.ToSingle (Math.Floor(Convert.ToDouble(abcde [1]))) - Convert.ToSingle (Math.Floor(Convert.ToDouble(zzz)));//(int)abcde [1] - zzz;
////									shengji = (int)abcde [1] - zzz;
//									//									shengji = Convert.ToSingle(Math.Floor((float)abcde [1]) - Math.Floor(zzz));
//								}
							}
						}
					}
					if (fuhao != "")
					{
						switch (fuhao)
						{
						case "+":
							num = num + Convert.ToSingle (en);
//							shengji = shengji + Convert.ToSingle (en);
							break;
						case "-":
							num = num - Convert.ToSingle (en);
//							shengji = shengji - Convert.ToSingle (en);
							break;
						case "*":
							num = num * Convert.ToSingle (en);
//							shengji = shengji * Convert.ToSingle (en);
							break;
						case "/":
							num = num / Convert.ToSingle (en);
//							shengji = shengji / Convert.ToSingle (en);
							break;
						}
						fuhao = "";
					}
					else
					{
						if (zzz != 0)
						{
							num = zzz;
						}
						else
						{
							num = Convert.ToSingle (en);
						}

					}
				}
			}
			else
			{
				switch (fuhao)
				{
				case "+":
					num = num + Convert.ToSingle (arr [i]);
//					shengji = shengji + Convert.ToSingle (arr [i]);
					break;
				case "-":
					num = num - Convert.ToSingle (arr [i]);
//					shengji = shengji - Convert.ToSingle (arr [i]);
					break;
				case "*":
					num = num * Convert.ToSingle (arr [i]);
//					shengji = shengji * Convert.ToSingle (arr [i]);
					break;
				case "/":
					num = num / Convert.ToSingle (arr [i]);
//					shengji = shengji / Convert.ToSingle (arr [i]);
					break;
				}
				fuhao = "";
			}
		}
		return (int)Math.Floor (num);
	}
	public override void Clear ()
	{
		CID = "";
		isKu = 0;
		lv = -1;
		base.Clear ();
	}
}
