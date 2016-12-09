using System;
using System.Collections;
using System.Collections.Generic;

public class CardVo
{
	//源配置
	public string id;
	public string name;
	public string info;
	//	public int position;
	public int rarity;
	public int baseLevel;

	//附加字段
	public int lv;
	public bool have = false;
	public int exp = 0;
	public int maxExp = 999;
	public int count = 0;
	public int maxCount = 999;
	public int cost = 0;
	public int maxLv = 0;
	public int armor = -1;
	public int effort_lv;
	public float cd = 0;

	public int hp_score = 0;
	public int atk_score = 0;
	public int move_score = 0;
	public int special_score = 0;
	public int newcard = -1;

	public Dictionary<string,object> cardData = null;

	public int GetChildLv(int mylv,float initV){
		float powerV = 0f;
		float addV = 0f;
		int re = 1;
		if (cardData != null) {
			if(cardData.ContainsKey("up")){
				Dictionary<string,object> upData = (Dictionary<string,object>)cardData["up"];
				Dictionary<string,object> va = null;

				foreach(string key in upData.Keys){
//					type = objs.GetType ().ToString();
//					if (true) {
					va = (Dictionary<string,object>)upData[key];
//					} else {
						if (va.ContainsKey ("power") && va.ContainsKey ("add")) {
							powerV = Convert.ToSingle ((string)va ["power"]);
							addV = Convert.ToSingle ((string)va ["add"]);//最终值 = 初值*power^(lv-1)+add*(lv-1)

							re = Convert.ToInt32 (initV * Math.Pow (powerV, (mylv - 1) + addV * (mylv - 1)));
						break;
						} else if (va.ContainsKey ("fixed")) {
							object[] arr = (object[])va ["fixed"];
							object[] arrE = null;
							for (int i = 0; i < arr.Length; i++) {
								arrE = (object[])arr [i];
								if ((int)arrE [0] == mylv) {
									re = (int)arrE [1];
									break;
								}
							}
						} else {
							re = mylv;

						}
						
//					}
				}
			}
		}
		return re;
		
	}
	public CardVo ()
	{
		
	}

	public int getUpLevelNeed ()
	{
		int needGold = 0;
		Dictionary<string,object> cfg = (Dictionary<string,object>)DataManager.inst.systemSimple ["card_lv_exp"];
		cfg = (Dictionary<string,object>)cfg [this.rarity + ""];
		object[] ss = (object[])(cfg ["lv_gold"]);
		if (lv == (ss.GetLength (0)))
		{
			needGold = 0;
		}
		else
		{
			needGold = (int)(ss [lv]);
		}
		Dictionary<string,object> d = (Dictionary<string,object>)ModelManager.inst.userModel.records ["rarity_lv"];
		if ((int)d [this.rarity + ""] > lv)
		{
			needGold = 0;
		}			
		return needGold;
	}

	public int getUpLevelGiveRed ()
	{
		int needGold = 0;
		Dictionary<string,object> cfg = (Dictionary<string,object>)DataManager.inst.systemSimple ["card_lv_exp"];
		cfg = (Dictionary<string,object>)cfg [this.rarity + ""];
		object[] ss = (object[])(cfg ["redbag_coin"]);
		if (lv >= (ss.GetLength (0)))
		{
			needGold = 0;
		}
		else
		{
			needGold = (int)(ss [lv]);
		}
		return needGold;
	}

	public string GetCost ()
	{
		string str = "";
		str = Convert.ToString (cost / 1000);
		if (cost < 0)
		{
			str = "?";
		}
		return str;
	}

	public static string GetRarityMss (int ra, int info = 0)
	{
		if (info == 0)
		{
			switch (ra)
			{
			case 0:
				return Tools.GetMessageById ("30100");
			case 1:
				return Tools.GetMessageById ("30101");
			case 2:
				return Tools.GetMessageById ("30102");
			case 3:
				return Tools.GetMessageById ("30103");
			}
		}
		else
		{
			switch (ra)
			{
			case 0:
				return Tools.GetMessageById ("24105");
			case 1:
				return Tools.GetMessageById ("24106");
			case 2:
				return Tools.GetMessageById ("24107");
			case 3:
				return Tools.GetMessageById ("24120");
			}
		}

		return "";
	}

	public static string GetCardName (string cid)
	{
		Dictionary<string,object> cardCfg = DataManager.inst.card;
		return Tools.GetMessageById (((Dictionary<string,object>)cardCfg [cid]) ["name"].ToString ());
	}

	public static string GetArmorMss (int armor)
	{
		switch (armor)
		{
		case -1:
			return Tools.GetMessageById ("30110");
		case 0:
			return Tools.GetMessageById ("30111");
		case 1:
			return Tools.GetMessageById ("30112");
		case 2:
			return Tools.GetMessageById ("30113");
		}
		return "";
	}
}