using System;
using System.Collections.Generic;

public class DataManager
{
	private static DataManager instance;

	public Dictionary<string,object> systemSimple;
	public Dictionary<string,object> unames;
	public Dictionary<string,object> returnMsg;
	public Dictionary<string,object> helpMsg;
	public Dictionary<string,object> card;
	public Dictionary<string,object> beckon;
	public Dictionary<string,object> union;
	public Dictionary<string,object> robot;
	public Dictionary<string,object> combat;
	//Biggo添加
	public Dictionary<string,object> ai;
	public Dictionary<string,object> mark;

	public Dictionary<string,object> effort;
	public Dictionary<string,object> redbag;
	public Dictionary<string,object> guide;
	public Dictionary<string,object> shop;
	public Dictionary<string,object> random_award;
	public Dictionary<string,object> explore;
	public Dictionary<string,object> award;
	public Dictionary<string,object> effect;
	public Dictionary<string,object> pay_config;
	public Dictionary<string,object> body;
	public Dictionary<string,object> guild;
	public Dictionary<string,object> match;
	//-----------------------
	public const string SHARE_WEIBO_ID = "share_weibo_id";
	public const string SHARE_WEIXIN_ID = "share_weixin_id";
	public const string SHARE_WEIBO_URL = "share_weibo_url";
	public const string SHARE_WEIBO_CONTENT = "share_weibo_content";
	public const string SHARE_WEIBO_CALLBACK = "share_weibo_callback";

	public const string SHARE_QQ_ID = "share_qq_id";
	public const string SHARE_QQ_URL = "share_qq_url";
	public const string SHARE_QQ_TITLE = "share_qq_title";
	public const string SHARE_QQ_DES = "share_qq_des";
	//-----------------------
	//
	public bool ConfigWasGet = false;
	public DataManager ()
	{
	}

	public static DataManager inst
	{
		get
		{
			if (instance == null)
				instance = new DataManager ();
			return instance;
		}
	}

	public void Decode (object value)
	{		
//		ModelGame model = ModelManager.inst.gameModel;
//		StoreByte.Save (value);
		//
		Dictionary<string, object> re = (Dictionary<string, object>)value;
		Dictionary<string, object> cfg = (Dictionary<string, object>)re ["cfg"];
		//
		PlatForm.inst.Cfg_Version = (string)re ["cfg_version"];
		//
		if (cfg.ContainsKey ("system_simple"))
			this.systemSimple = (Dictionary<string,object>)cfg ["system_simple"];
		if (cfg.ContainsKey ("return_msg"))
		{
			this.returnMsg = (Dictionary<string, object>)cfg ["return_msg"];
			this.unames = (Dictionary<string, object>)this.returnMsg ["unames"];
		}
		if (cfg.ContainsKey ("help_msg"))
			this.helpMsg = (Dictionary<string, object>)cfg ["help_msg"];
		if (cfg.ContainsKey ("card"))
			this.card = (Dictionary<string, object>)cfg ["card"];
		if (cfg.ContainsKey ("beckon"))
			this.beckon = (Dictionary<string, object>)cfg ["beckon"];
		//		if (cfg ["union"] != null)
		//			this.union = (Dictionary<string, object>)cfg ["union"];
		//		if (cfg ["robot"] != null)
		//			this.robot = (Dictionary<string, object>)cfg ["robot"];
		if (cfg.ContainsKey("combat"))
			this.combat = (Dictionary<string, object>)cfg ["combat"];
		//Biggo添加
		if (cfg.ContainsKey("ai"))
			this.ai = (Dictionary<string, object>)cfg ["ai"];
		if (cfg.ContainsKey("mark"))
			this.mark = (Dictionary<string, object>)cfg ["mark"];
		

		if (cfg.ContainsKey ("effort"))
			this.effort = (Dictionary<string, object>)cfg ["effort"];
		if (cfg.ContainsKey ("redbag"))
			this.redbag = (Dictionary<string, object>)cfg ["redbag"];
		//		if (cfg ["guide"] != null)
		//			this.guide = (Dictionary<string, object>)cfg ["guide"];
		//		if (cfg ["shop"] != null)
		//			this.shop = (Dictionary<string, object>)cfg ["shop"];
		if (cfg.ContainsKey ("random_award"))
			this.random_award = (Dictionary<string, object>)cfg ["random_award"];
		if (cfg.ContainsKey ("explore"))
			this.explore = (Dictionary<string, object>)cfg ["explore"];
		if (cfg.ContainsKey ("award"))
			this.award = (Dictionary<string, object>)cfg ["award"];
//		if (cfg ["effect"] != null)
//			this.effect = (Dictionary<string, object>)cfg ["effect"];
		if (cfg.ContainsKey ("pay_config"))
			this.pay_config = (Dictionary<string,object>)cfg ["pay_config"];
		if (cfg.ContainsKey ("body"))
			this.body = (Dictionary<string,object>)cfg ["body"];
		if (cfg.ContainsKey ("guild"))
			this.guild = (Dictionary<string,object>)cfg ["guild"];
		if (cfg.ContainsKey ("guide"))
			this.guide = (Dictionary<string,object>)cfg ["guide"];
		if (cfg.ContainsKey ("match"))
			this.match = (Dictionary<string,object>)cfg ["match"];
	}

	public CardVo GetCardVo (string id, int lv = -1, int exp = -1)
	{
		Dictionary<string, object> o = (Dictionary<string, object>)this.card [id];
		Dictionary<string, object> userCard = ModelManager.inst.userModel.card;
		Dictionary<string,object> ss = (Dictionary<string,object>)(this.systemSimple ["card_lv_exp"]);


		if (o == null)
			return null;
		CardVo vo = new CardVo ();
		vo.id = id;
		vo.name = (string)o ["name"];
		vo.info = (string)o ["info"];
		vo.cd = Convert.ToSingle (o ["cd"])/1000f;
		//		vo.position = (int)o ["position"];
		vo.rarity = (int)o ["rarity"];
//		vo.baseLevel = (int)o ["base_lv"];
		vo.cost = (int)o ["cost"];
		vo.effort_lv = (int)o ["effort_lv"];
		vo.hp_score = (int)o ["hp_score"];
		vo.atk_score = (int)o ["atk_score"];
		vo.move_score = (int)o ["move_score"];
		vo.special_score = (int)o ["special_score"];

		if (o.ContainsKey ("part"))
		{
			vo.armor = (int)((Dictionary<string,object>)o ["part"]) ["position"];
		}

		ss = (Dictionary<string,object>)(ss [vo.rarity + ""]);
		object[] exp_cfg = (object[])(ss ["exp"]);

		vo.maxLv = exp_cfg.GetLength (0);
		if (userCard.ContainsKey (id))
		{
			vo.have = true;
			vo.lv = (int)(((Dictionary<string, object>)(userCard [id])) ["lv"]);
			vo.newcard = (int)((Dictionary<string,object>)userCard [id]) ["new"];
			if (lv != -1)
			{
				vo.lv = lv;
			}
			if (exp != -1)
			{
				vo.exp = ((int)(((Dictionary<string, object>)(userCard [id])) ["exp"]) - (int)(exp_cfg [(vo.lv - 1)])) - exp;
			}
			else
			{
				vo.exp = (int)(((Dictionary<string, object>)(userCard [id])) ["exp"]) - (int)(exp_cfg [(vo.lv - 1)]);
			}
			if (o.ContainsKey ("part"))
			{
				vo.armor = (int)((Dictionary<string,object>)(o ["part"])) ["position"];
			}
			if (vo.lv >= vo.maxLv)
			{
				vo.maxExp = Convert.ToInt32 (((object[])ss ["lv_max"]) [0]);
			}
			else
			{
				vo.maxExp = (int)(exp_cfg [(vo.lv)]) - (int)(exp_cfg [(vo.lv - 1)]);
			}
		}
		if (lv != -1)
		{
			vo.lv = lv;
			if (vo.lv >= exp_cfg.Length) {
				vo.maxExp = Convert.ToInt32 (((object[])ss ["lv_max"]) [0]);
			} else {
				vo.maxExp = (int)(exp_cfg [(vo.lv)]) - (int)(exp_cfg [(vo.lv - 1)]);
			}
		}
		return vo;
	}

	public int GetCardNumByLv (int lv)
	{
		int num = 0;
		Dictionary<string,object> cfg = (Dictionary<string,object>)DataManager.inst.systemSimple ["award_card_pool"];
		Dictionary<string,object> currLvCfg = (Dictionary<string,object>)cfg ["Lv" + lv];
		foreach (string i in currLvCfg.Keys)
		{
			Dictionary<string,object> cf = (Dictionary<string,object>)currLvCfg [i];
			num += cf.Keys.Count;
		}
		return num;
	}

	public int GetCardRarity (string cid)
	{
		if (card.ContainsKey (cid))
		{
			return Convert.ToInt32 (Tools.Analysis (card [cid], "rarity"));
		}
		return -1;
	}
	public bool EffortIsMax (int myLv){
		bool b = false;
		if (this.effort != null) {
			if (this.effort.ContainsKey ("effort_cond")) {
				int total = ((Dictionary<string ,object>)this.effort ["effort_cond"]).Count;
				if (myLv > total) {
					b = true;
				}
			}
		}
		return b;
	}
	public static bool Clip_guide_over_tag = false;
}