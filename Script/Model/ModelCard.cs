using System;
using System.Collections.Generic;
using UnityEngine;

public class ModelCard : BaseModel
{
	public List<object> myTeamList;
	public List<object> myCard;
	public List<object> noMyCard;
	public float have;
	public int teamIndex = 0;
	private ModelUser userModel = ModelManager.inst.userModel;
	private Dictionary<string,object> asdf;
	private Dictionary<string,object> cardCfg;

	public string shipinfo_callinID = "";
	public string shipinfo_title = "";
	public Dictionary<string,object> shipinfo_cardData;
	public CardVo shipinfo_cvo;

	//item pos
	public Vector2 v2;

	public ModelCard ()
	{
		
	}

	public void SetData ()
	{
		UpdataMyTeamData ();
		UpdataMyCardData ();
		UpdataNoMyCardData ();
	}
	public void SetComShipInfoData(string callinId,string title,Dictionary<string,object> cardData,CardVo card)
	{
		shipinfo_callinID = callinId;
		shipinfo_title = title;
		shipinfo_cardData = cardData;
		shipinfo_cvo = card;
	}
	public void UpdataMyTeamData ()
	{
		object[] team = userModel.card_group;
		myTeamList = new List<object> ();
		for (int j = 0; j < 5; j++)
		{
			List<object> currTeam = new List<object> ();
//			currTeam.Add (new object ());
			for (int i = 0; i < (team [j] as object[]).Length; i++)
			{
				currTeam.Add ((team [j] as object[]) [i]);
			}
			myTeamList.Add (currTeam);
		}
	}
	public float GetCostTeam(int index)
	{
		List<object> teamCard = getDataMyTeamData (index);
		float num = 0;
		int jjj = 8;
		for (int i = 0; i < teamCard.Count; i++) {
			CardVo card = DataManager.inst.GetCardVo (teamCard [i].ToString ());
			if (card.cost >= 0) {
				num += card.cost;
			} else {
				jjj--;
			}
		}
		return num / jjj;
	}
	public float GetAtkTeam(int index)
	{
		List<object> teamCard = getDataMyTeamData (index);
		float num = 0;
		int jjj = 8;
		for (int i = 0; i < teamCard.Count; i++) {
			CardVo card = DataManager.inst.GetCardVo (teamCard [i].ToString ());
			if (card.atk_score >= 0) {
				num += card.atk_score;
			} else {
				jjj--;
			}
		}
		return num / jjj;
	}
	public float GetMoveTeam(int index)
	{
		List<object> teamCard = getDataMyTeamData (index);
		float num = 0;
		int jjj = 8;
		for (int i = 0; i < teamCard.Count; i++) {
			CardVo card = DataManager.inst.GetCardVo (teamCard [i].ToString ());
			if (card.move_score >= 0) {
				num += card.move_score;
			} else {
				jjj--;
			}
		}
		return num / jjj;
	}
	public float GetSpecTeam(int index)
	{
		List<object> teamCard = getDataMyTeamData (index);
		float num = 0;
		int jjj = 8;
		for (int i = 0; i < teamCard.Count; i++) {
			CardVo card = DataManager.inst.GetCardVo (teamCard [i].ToString ());
			if (card.special_score >= 0) {
				num += card.special_score;
			} else {
				jjj--;
			}
		}
		return num / jjj;
	}
	public float GetHPTeam(int index)
	{
		List<object> teamCard = getDataMyTeamData (index);
		float num = 0;
		int jjj = 8;
		for (int i = 0; i < teamCard.Count; i++) {
			CardVo card = DataManager.inst.GetCardVo (teamCard [i].ToString ());
			if (card.hp_score >= 0) {
				num += card.hp_score;
			} else {
				jjj--;
			}
		}
		return num / jjj;
	}
	public List<object> getDataMyTeamData (int index)
	{
		return (List<object>)(myTeamList [index]);
	}

	public void UpdataMyCardData ()
	{
		myCard = new List<object> ();
		Dictionary<string,object> userCard = userModel.card;
		Dictionary<string,object> cfgCard = DataManager.inst.card;
		List<object> teamCard = getDataMyTeamData (teamIndex);
		Dictionary<string,object> newData;
		foreach (string h in userCard.Keys)
		{
			bool boo = false;
			for (int i = 0; i < teamCard.Count; i++) {
				if (teamCard [i].ToString () == h) {
					boo = true;
				}
			}
			if (!boo)
			{
				newData = new Dictionary<string, object> ();
				newData.Add ("id", h);
				newData.Add ("new", ((Dictionary<string,object>)userCard [h]) ["new"]);
				newData.Add ("index", (int)(((Dictionary<string,object>)cfgCard [h]) ["index"]));
				myCard.Add (newData);
			}
		}
		Tools.Sort (myCard, new string[]{ "index:int:0" });
	}

	public void UpdataNoMyCardData ()
	{
		noMyCard = new List<object> ();
		Dictionary<string,object> userCard = userModel.card;
		Dictionary<string,object> cfgCard = DataManager.inst.card;
		List<object> teamCard = getDataMyTeamData (teamIndex);
		Dictionary<string,object> newData;
		foreach (string h in cfgCard.Keys)
		{
			if (!userCard.ContainsKey (h)&&(int)(((Dictionary<string,object>)cfgCard [h]) ["effort_lv"]) <= ModelManager.inst.userModel.effort_lv+1)
			{
				newData = new Dictionary<string, object> ();
				newData.Add ("id", h);
				newData.Add ("index", (int)(((Dictionary<string,object>)cfgCard [h]) ["index"]));
				noMyCard.Add (newData);
			}
		}
		Tools.Sort (noMyCard, new string[]{ "index:int:0" });
	}

	public bool CanFrush()
	{
		Dictionary<string,object> user = userModel.records ["daily_box"] as Dictionary<string,object>;
		if (user ["refresh_time"] != null) {
			DateTime date = (DateTime)user ["refresh_time"];
			if ((date.Ticks) < Tools.GetSystemTicks ()) {
				return true;
			} else {
				return false;
			}
		} else {
			return true;
		}
	}
	public static int returnNum(string id,object data)
	{
		int num = 0;
		switch (id) {
		case Config.ASSET_CARD:
			Dictionary<string,object> card = data as Dictionary<string,object>;
			foreach(string s in card.Keys)
			{
				Dictionary<string,object> a00 = card [Config.ASSET_CARD] as Dictionary<string,object>;
				foreach(string l in a00.Keys)
				{
					num = (int)a00[l];
				}
			}
			break;
		case Config.ASSET_COIN:
			num = (int)(data as Dictionary<string,object>) [id];
			break;
		case "exp":
			num = (int)(data as Dictionary<string,object>) [id];
			break;
		case Config.ASSET_GOLD:
			num = (int)(data as Dictionary<string,object>) [id];
			break;
		}
		if (num == 0) {
			return 0;
		}
		return num;
	}
	public int GetNewCardNum()
	{
		int num = 0;
		Dictionary<string,object> card = userModel.card;
		foreach (string i in card.Keys) {
			Dictionary<string,object> cardData = (Dictionary<string,object>)card [i];
			if ((int)cardData ["new"] == 1)
				num++;
		}
		return num;
	}
	public int GetLevelUpCardNum()
	{
		int num = 0;
		asdf = DataManager.inst.systemSimple;
		Dictionary<string,object> card = userModel.card;
		cardCfg = DataManager.inst.card;
		Dictionary<string,object> ss = (Dictionary<string,object>)(asdf ["card_lv_exp"]);
		foreach (string i in card.Keys) {
			Dictionary<string,object> aa = (Dictionary<string,object>)(ss [((Dictionary<string,object>)cardCfg[i])["rarity"].ToString() + ""]);
			object[] exp_cfg = (object[])(aa ["exp"]);
			Dictionary<string,object> cardData = (Dictionary<string,object>)card [i];
			int cardLV = (int)cardData ["lv"];
			int maxLv = exp_cfg.GetLength (0);
			int exp = (int)cardData ["exp"] - (int)(exp_cfg [(cardLV - 1)]);
			int maxExp = 0;
			if (cardLV >= maxLv)
			{
				maxExp = 0;
			}
			else
			{
				maxExp = (int)(exp_cfg [(cardLV)]) - (int)(exp_cfg [(cardLV - 1)]);
			}
			if (exp >= maxExp)
				num++;
		}
		return num;
	}
	public int GetShipNum()
	{
		int num = 0;
		Dictionary<string,object> dat = DataManager.inst.body;
		foreach(string i in dat.Keys)
		{
			Dictionary<string,object> data = (Dictionary<string,object>)dat [i];
			if (data.ContainsKey ("show")&&(int)data["show"] == 1&&!GetCanRed(i)) {
				num++;
			}
		}
		return num;
	}
	private bool GetCanRed(string sid)
	{
		string str = LocalStore.GetLocal (LocalStore.LOCAL_SHIPRED);
		if (str == null) {
			return false;
		} else {
			if (str.IndexOf (sid) != -1)
				return true;
			else
				return false;
		}
	}
}
