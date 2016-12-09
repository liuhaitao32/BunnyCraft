using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using FairyGUI;
using DG.Tweening;

public class MediatorItemShipInfo2 : BaseMediator
{

	private Dictionary<string,object> sim;

	public static string CID = "";
//0牌库1卡组2为拥有3别人的
	public static int lv = -1;

	private CardVo card;
	private GLoader proBar;
	private bool infobar = false;

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_ITEMSHIPINFO2, false);
		if (lv == -1) {
			card = DataManager.inst.GetCardVo (CID, 1);
		} else {
			card = DataManager.inst.GetCardVo (CID, lv);
		}

		sim = (Dictionary<string,object>)(DataManager.inst.systemSimple ["card_attr_info"]);

		GTextField title = this.GetChild ("n64").asTextField;
		GTextField info = this.GetChild ("n12").asTextField;
		GLoader img_rarity = this.GetChild ("n3").asLoader;

		title.text = Tools.GetMessageById(card.name);
		info.text = Tools.GetMessageById (card.info);

		img_rarity.url = Tools.GetResourceUrl ("Image2:n_icon_dengji" + (card.rarity + 1) + "_");
		this.GetChild ("n69").asTextField.text = Tools.GetMessageById ("24134", new string[]{ card.cd.ToString () });
		this.GetChild ("n71").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_d" + card.armor.ToString ());
		GTextField l_type = this.GetChild ("n5").asTextField;
		l_type.text = CardVo.GetArmorMss (card.armor);
		if (card.lv >= card.maxLv)
		{
			Dictionary<string,object> cfg = DataManager.inst.systemSimple;
			Dictionary<string,object> card_exp = (Dictionary<string,object>)cfg ["card_lv_exp"];
			card_exp = (Dictionary<string,object>)card_exp [card.rarity + ""];
		}
		if (lv == -1) {
			(this.GetChild ("n56") as ComCard).SetData (CID, -1, 4, card.lv);
		} else {
			(this.GetChild ("n56") as ComCard).SetData (CID, -1, 2, card.lv);
		}
		(this.GetChild ("n56") as ComCard).SetText (Tools.GetMessageById (card.name));
		FushDataToShow (CID);
		this.GetChild ("n58").asButton.onClick.Add (() => {
			ViewManager.inst.CloseView(this);
		});

		if (card.rarity == 3) {
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
		} else {
			this.GetChild ("n70").visible = false;
		}
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

					MediatorComShipInfo bbb = new MediatorComShipInfo();
					ViewManager.inst.AddPView(bbb);
					bbb.setSelectIndex(1);
					v2 = _view.parent.GlobalToLocal (v2);
					bbb.group.x = v2.x - bbb.group.width + 175;
					bbb.group.y = v2.y - bbb.group.height - 20;
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
					fafa = MediatorItemShipInfo.suanShu ((object[])replace [j], monster, card, round);
				}
				else
				{
					fafa = MediatorItemShipInfo.suanShu ((object[])replace [j], cardData, card, round);
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
			if (false)
			{
				n1 = "";
//				n1 += "[color=#0cff00]";
				n1 += "+"+Tools.GetMessageById (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["info"].ToString (), strArr2);
//				n1 += "[/color]";
				fa2 = n1;
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
	/**
	public static string[] suanShu (object[] arr, Dictionary<string,object> dic, CardVo card, int round,bool isChild = false)
	{
		float num = 0f;
		string fuhao = "";
		float shengji = 0f;
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
									zzz = (int)abcde [1];
								}
								if ((card.lv) < (int)abcde [0] && (card.lv + 1) >= (int)abcde [0])
								{
									shengji = (int)abcde [1] - zzz;
								}
							}
						}
					}
					if (fuhao != "")
					{
						switch (fuhao)
						{
						case "+":
							num = num + Convert.ToSingle (en);
							shengji = shengji + Convert.ToSingle (en);
							break;
						case "-":
							num = num - Convert.ToSingle (en);
							shengji = shengji - Convert.ToSingle (en);
							break;
						case "*":
							num = num * Convert.ToSingle (en);
							shengji = shengji * Convert.ToSingle (en);
							break;
						case "/":
							num = num / Convert.ToSingle (en);
							shengji = shengji / Convert.ToSingle (en);
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
					shengji = shengji + Convert.ToSingle (arr [i]);
					break;
				case "-":
					num = num - Convert.ToSingle (arr [i]);
					shengji = shengji - Convert.ToSingle (arr [i]);
					break;
				case "*":
					num = num * Convert.ToSingle (arr [i]);
					shengji = shengji * Convert.ToSingle (arr [i]);
					break;
				case "/":
					num = num / Convert.ToSingle (arr [i]);
					shengji = shengji / Convert.ToSingle (arr [i]);
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
	public override void Clear ()
	{
		CID = "";
		lv = -1;
		base.Clear ();
	}
}
