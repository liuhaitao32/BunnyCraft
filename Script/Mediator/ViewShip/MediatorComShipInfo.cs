using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;

public class MediatorComShipInfo : BaseMediator {

	private ModelCard cardModel;
	private GTextField title;
	private GList list;

	private object[] tips;
	private CardVo card;
	private Dictionary<string,object> sim;
	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_COMSHIPINFO);

		sim = (Dictionary<string,object>)(DataManager.inst.systemSimple ["card_attr_info"]);
		cardModel = ModelManager.inst.cardModel;
		tips = (object[])cardModel.shipinfo_cardData [cardModel.shipinfo_callinID];
		card = cardModel.shipinfo_cvo;
		card.cardData = cardModel.shipinfo_cardData;
		cardModel.SetData ();
		title = this.GetChild ("n4").asTextField;
		list = this.GetChild ("n1").asList;

		title.text = Tools.GetMessageById (cardModel.shipinfo_title);
		list.itemRenderer = onListRander;
		list.numItems = tips.Length;
		list.foldInvisibleItems = true;
		list.ResizeToFit (tips.Length);
		group.height = list.height + 80;
		this.height = list.height + 80;
	}
	public void setSelectIndex(int idnex)
	{
		this.GetController ("c1").selectedIndex = idnex;
	}
	private void onListRander(int index,GObject obj)
	{
		string n1 = "";
		Dictionary<string,object> monster = new Dictionary<string, object> ();
		Dictionary<string,object> tipi = (Dictionary<string,object>)tips [index];
		string carr = tipi.ContainsKey ("call") ? tipi ["call"].ToString () : null;
		if (carr != null) {
			monster = (Dictionary<string,object>)DataManager.inst.beckon [carr];
		}
		bool bo = false;
		int round = 0;
		if (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]).ContainsKey ("round")) {
			round = (int)(((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["round"]);
		}
		if (tipi.ContainsKey ("replace")) {
			object[] replace = (object[])tipi ["replace"];
			string[] strArr = new string[replace.Length];
			string[] strArr2 = new string[replace.Length];
			for (int j = 0; j < replace.Length; j++) {
				string[] fafa;
				if (carr != null) {
					fafa = MediatorItemShipInfo.suanShu ((object[])replace [j], monster, card,round,true);
				} else {
					fafa = MediatorItemShipInfo.suanShu ((object[])replace [j], cardModel.shipinfo_cardData, card,round,true);
				}
				strArr [j] = fafa[0];
				if (fafa.Length > 1) {
					bo = true;
					if (card.exp < card.maxExp||card.lv == card.maxLv) {
						bo = false;
					}
					strArr2[j] = fafa [1];
				}
			}
			n1 = "" + Tools.GetMessageById (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["info"].ToString (), strArr) + "";
			if (bo) {
//				n1 += "+";
//				n1 += Tools.GetMessageById (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["info"].ToString (), strArr2);
//				n1 += "";
			}
		} else {
			n1 = "" + Tools.GetMessageById (((Dictionary<string,object>)sim [tipi ["type"].ToString ()]) ["info"].ToString ()) + "";
		}
		GLoader n0 = obj.asCom.GetChild ("n0").asLoader;
		GTextField n1t = obj.asCom.GetChild ("n1").asTextField;
		GTextField n2 = obj.asCom.GetChild ("n2").asTextField;

		n1t.text = Tools.GetMessageById (((Dictionary<string,object>)sim[tipi ["type"].ToString ()])["name"].ToString()) + ":";
		n2.text = n1;
		n0.url = Tools.GetResourceUrl ("Icon:"+((Dictionary<string,object>)sim[tipi ["type"].ToString ()])["icon"].ToString());
	}
	public override void Clear ()
	{
		base.Clear ();
	}
}
