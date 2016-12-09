using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using FairyGUI;

public class MediatorItemGuildSearch : BaseMediator {
	private GButton btn_search;
	private GButton btn_tuijian;
	private GButton btn_sort;
	private GTextInput L_Input;
	private GList list;
	private GComboBox bo_suozaidi;

	private ComNumeric L_NumericStepper0;
	private ComNumeric L_NumericStepper1;
	private ComNumeric L_NumericStepper2;

	private List<object> listData;
	private List<object> newlistData;
	private int fushType = 0;//0推荐  1筛选
	private Dictionary<string,object> simCfg;
	private ModelUser userModel;
	public override void Init()
	{
		base.Init ();
		this.Create (Config.VIEW_GUILDSEARCH);

		userModel = ModelManager.inst.userModel;

		simCfg = DataManager.inst.systemSimple;
		btn_search = this.GetChild ("n2").asButton;
		btn_tuijian = this.GetChild ("n4").asButton;
		btn_sort = this.GetChild ("n3").asButton;
		L_Input = this.GetChild ("n0").asTextInput;
		list = this.GetChild ("n13").asList;
		bo_suozaidi = this.GetChild ("n16").asComboBox;
		L_NumericStepper0 = this.GetChild ("n7") as ComNumeric;
		L_NumericStepper1 = this.GetChild ("n8") as ComNumeric;
		L_NumericStepper2 = this.GetChild ("n9") as ComNumeric;

		this.GetChild ("n12").asGroup.visible = false;
		GButton b = this.GetChild ("n11").asButton;
		b.text = Tools.GetMessageById("20140");
		b.onClick.Add (OnBtnSure);
		L_Input.text = "";
		L_Input.promptText = Tools.GetMessageById("20116");
		list.itemRenderer = ListRander;

        bg = this.GetChild("n14");
        BaseMediator.emptyBg = bg;
        list.emptyStr = Tools.GetMessageById("19936");
        list.onChangeNum.Add(this.CheckListNum);
        list.SetVirtual ();


		btn_search.text = Tools.GetMessageById("20137");
		btn_sort.text = Tools.GetMessageById("20138");
		btn_tuijian.text = Tools.GetMessageById("20139");
		this.GetChild ("n5").asTextField.text = Tools.GetMessageById ("20141");
		this.GetChild ("n10").asTextField.text = Tools.GetMessageById ("20142");
		this.GetChild ("n6").asTextField.text = Tools.GetMessageById ("20143");
		list.numItems = 0;

		btn_search.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image2:n_btn_7");
		btn_search.GetChild ("title").asTextField.strokeColor = Tools.GetColor ("486fcc");
//		btn_tuijian.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_9");
//		btn_sort.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_9");

		Dictionary<string,object> cc = (Dictionary<string,object>) (simCfg ["society_location"]);

		string[] str = new string[cc.Count];
		string[] va = new string[cc.Count];
		int ii = 0;
		foreach (string i in cc.Keys) {
			str [ii] = Tools.GetMessageById (((object[])cc [i]) [1].ToString ());
			va[ii] = i;
			ii++;
		}
		bo_suozaidi.items = str;
		bo_suozaidi.values = va;

		L_NumericStepper0.SetMinMax (0, 50, 10);
		L_NumericStepper1.SetMinMax (0, 50, 10);
		L_NumericStepper2.SetMinMax (0, 1000, 100);

		btn_search.onClick.Add (OnBtnSearchHandler);
		btn_tuijian.onClick.Add (OnBtnTuJianHandler);
		btn_sort.onClick.Add (OnBtnSortHandler);
		OnBtnTuJianHandler ();
		this.AddGlobalListener (MainEvent.CHAT_GUILDJOIN, CHAT_GUILDJOIN);
	}
	private void CHAT_GUILDJOIN (MainEvent e)
	{
		this.DispatchGlobalEvent (new MainEvent (MainEvent.GUILD_ESC));
	}
	private void OnBtnSearchHandler()
	{
		
//		btn_search.GetController ("c1").selectedIndex = 1;
//		btn_tuijian.GetController ("c1").selectedIndex = 0;
//		btn_sort.GetController ("c1").selectedIndex = 0;
		this.GetChild ("n12").asGroup.visible = false;
//		list.height = 400;
//		list.y = 102;
		if (L_Input.text != "") {
			NetHttp.inst.Send (NetBase.HTTP_GUILD_FIND, "gid=" + L_Input.text, OnSearchHandler);
//			btn_search.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image2:n_btn_7");
//			btn_tuijian.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_9");
//			btn_sort.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_9");
		} else {
			ViewManager.inst.ShowText (Tools.GetMessageById ("20116"));
//			btn_search.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image2:n_btn_7");
//			btn_tuijian.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_9");
//			btn_sort.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_9");
		}
	}
	private void OnBtnTuJianHandler()
	{
//		btn_search.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image2:n_btn_14");
//		btn_tuijian.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_17");
//		btn_sort.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_9");
//		btn_search.GetController ("c1").selectedIndex = 0;
//		btn_tuijian.GetController ("c1").selectedIndex = 1;
//		btn_sort.GetController ("c1").selectedIndex = 0;
		this.GetChild ("n12").asGroup.visible = false;
//		list.height = 400;
//		list.y = 102;
		fushType = 0;
		pagindex = 1;
		NetHttp.inst.Send (NetBase.HTTP_GUILD_RECOMMEND, "", OnTuiJianHandler);
	}
	private void OnBtnSure()
	{
		this.GetChild ("n12").asGroup.visible = false;
//		list.height = 400;
//		list.y = 102;
		float min = L_NumericStepper1.value > L_NumericStepper0.value ? L_NumericStepper0.value : L_NumericStepper1.value;
		float max = L_NumericStepper1.value > L_NumericStepper0.value ? L_NumericStepper1.value : L_NumericStepper0.value;
		float score = L_NumericStepper2.value;
		NetHttp.inst.Send (NetBase.HTTP_GUILD_FILTER, "score="+score+"|member_count_min="+min+"|member_count_max="+max+"|location="+bo_suozaidi.values [bo_suozaidi.selectedIndex], OnTuiJianHandler);
	}
	private void OnBtnSortHandler()
	{
//		btn_search.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image2:n_btn_14");
//		btn_tuijian.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_9");
//		btn_sort.GetChild ("n6").asLoader.url = Tools.GetResourceUrl("Image:btn_17");
//		btn_search.GetController ("c1").selectedIndex = 0;
//		btn_tuijian.GetController ("c1").selectedIndex = 0;
//		btn_sort.GetController ("c1").selectedIndex = 1;
		fushType = 1;
		pagindex = 1;
		list.numItems = 0;

		GGroup g = this.GetChild ("n12").asGroup;
		g.visible = !g.visible;

//		if (g.visible) {
//			list.y = 231;
//			list.height = 290;
//		} else {
//			list.height = 415;
//			list.y = 106;
//		}
	}
	private void OnSureHandler(VoHttp vo)
	{
		if (vo.data is object[]) {
			listData = new List<object> ();
			newlistData = new List<object> ();
			for (int i = 0; i < ((object[])vo.data).GetLength (0); i++) {
				listData.Add (((object[])vo.data)[i]);
			}
			newlistData = ((List<object>)Tools.Clone (listData));
//			list.numItems = newlistData.Count;
			if (newlistData.Count > 6) {
				list.numItems = newlistData.Count;
			} else {
				list.numItems = 6;
			}
			if (newlistData.Count <= 5) {
				list.scrollPane.touchEffect = false;
			}
		}
	}
	private void OnTuiJianHandler(VoHttp vo)
	{
		if (vo.data is object[]) {
			listData = new List<object> ();
			newlistData = new List<object> ();
			for (int i = 0; i < ((object[])vo.data).GetLength (0); i++) {
				listData.Add (((object[])vo.data)[i]);
			}
			Tools.Sort (listData, new string[]{ "member_count:int:1", "score:int:1", "id:int:0" });
			newlistData = ((List<object>)Tools.Clone (listData));
			if (((object[])vo.data).GetLength (0) == 10) {
				newlistData.Add (true);
			}
			if (newlistData.Count > 6) {
				list.numItems = newlistData.Count;
			} else {
				list.numItems = 6;
			}
			if (newlistData.Count <= 5) {
				list.scrollPane.touchEffect = false;
			}
		}
	}
	private void OnSearchHandler(VoHttp vo)
	{//搜索事件回调
		if (vo.data is object[]) {
			if (((object[])vo.data).GetLength (0) == 0) {
				ViewManager.inst.ShowText (Tools.GetMessageById("20107"));
			} else {
				listData = new List<object> ();
				for (int i = 0; i < ((object[])vo.data).GetLength (0); i++) {
					listData.Add (((object[])vo.data)[i]);
				}
				newlistData = ((List<object>)Tools.Clone (listData));
				if (newlistData.Count > 6) {
					list.numItems = newlistData.Count;
				} else {
					list.numItems = 6;
				}
				if (newlistData.Count <= 5) {
					list.scrollPane.touchEffect = false;
				}
			}
		}
	}
	//分割线//////////////////////////////////////////////////////////////////////////////////////
	private int pagindex = 1;
	private bool have10 = false;
	private void ListRander(int index,GObject go)
	{
		GButton btn = go.asCom.GetChild ("n0").asButton;
		if (index == 0 || index % 2 == 0) {
			btn.alpha = 0;
		} else {
			btn.alpha = 1;
		}
		GButton btn_btn = go.asCom.GetChild ("n13").asButton;
		GLoader img_icon = go.asCom.GetChild ("n3").asLoader;
		GTextField l_guildName = go.asCom.GetChild ("n4").asTextField;
//		GTextField l_guildID = go.asCom.GetChild ("n5").asTextField;
		GTextField l_guildNum = go.asCom.GetChild ("n6").asTextField;
		GButton btn_more = go.asCom.GetChild ("n14").asButton;
		GComponent rankscore = go.asCom.GetChild ("n18").asCom;
		GTextField l_guildCoin =rankscore.GetChild ("n2").asTextField;
		GTextField n19 = go.asCom.GetChild ("n19").asTextField;

		if (index > newlistData.Count - 1) {
			btn_btn.visible = false;
			img_icon.visible = false;
			l_guildName.visible = false;
			l_guildNum.visible = false;
			btn_more.visible = false;
			rankscore.visible = false;
			n19.visible = false;
			btn.touchable = false;
			return;
		} else {
			btn_btn.visible = true;
			img_icon.visible = true;
			l_guildName.visible = true;
			l_guildNum.visible = true;
			btn_more.visible = true;
			rankscore.visible = true;
			n19.visible = true;
			btn.touchable = true;
		}


		btn.RemoveEventListeners ();
		btn_btn.RemoveEventListeners ();
		btn_more.RemoveEventListeners ();
		btn.visible = true;
		if (newlistData[index] is bool) {
			btn.visible = false;
			btn_btn.visible = false;
			img_icon.visible = false;
			l_guildName.visible = false;
//			l_guildID.visible = false;
			l_guildNum.visible = false;
			rankscore.visible = false;
			n19.visible = false;
			btn_more.visible = true;
			btn_more.onClick.Add (()=>{
				if(fushType == 0)
				{
					pagindex++;
					NetHttp.inst.Send (NetBase.HTTP_GUILD_RECOMMEND, "page="+pagindex, GetNewPageRank);
				}else{
					pagindex++;
					float min = L_NumericStepper1.value > L_NumericStepper0.value ? L_NumericStepper0.value : L_NumericStepper1.value;
					float max = L_NumericStepper1.value > L_NumericStepper0.value ? L_NumericStepper1.value : L_NumericStepper0.value;
					float score = L_NumericStepper2.value;
					NetHttp.inst.Send (NetBase.HTTP_GUILD_FILTER, "score="+score+"|member_count_min="+min+"|member_count_max="+max+"|page="+pagindex, GetNewPageRank);
				}

			});
			return;
		}

		btn_btn.visible = true;
		img_icon.visible = true;
		l_guildName.visible = true;
//		l_guildID.visible = true;
		l_guildNum.visible = true;
		rankscore.visible = true;
		n19.visible = true;
		btn_more.visible = false;
		Dictionary<string,object> _da = (Dictionary<string,object>)(newlistData[index]);

		if ((bool)_da ["is_apply"]) {
			btn_btn.text = Tools.GetMessageById ("20106");
			btn_btn.GetChild("title").asTextField.strokeColor = Tools.GetColor("dd8680");
			btn_btn.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_15");
//			btn_btn.GetController ("c1").selectedIndex = 1;
		} else {
			btn_btn.text = Tools.GetMessageById ("20105");
			btn_btn.GetChild("title").asTextField.strokeColor = Tools.GetColor("52b04f");
			btn_btn.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_14");
//			btn_btn.GetController ("c1").selectedIndex = 0;
		}
		btn_more.text = Tools.GetMessageById ("20183");
		img_icon.url = Tools.GetResourceUrl ("Icon:"+(string)(_da ["icon"]));
		l_guildName.text = (string)(_da ["name"]);
//		l_guildID.text = Tools.GetMessageById("20102",new string[]{(_da ["id"]).ToString()});
		l_guildNum.text = Tools.GetMessageById("20103",new string[]{(_da ["member_count"]).ToString () + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString ()});
		l_guildCoin.text = _da ["score"].ToString();
//		n19.text = (_da ["member_count"]).ToString () + "/" + ((int)(ModelManager.inst.guildModel.getGuildCfg ("society") ["society_num"])).ToString ();

		btn.onClick.Add (() => {
			NetHttp.inst.Send (NetBase.HTTP_GUILD_INFO, "gid="+(_da ["id"]).ToString(), GetGuildInfo);
		});

		btn_btn.onClick.Add(() => {
			if ((bool)_da ["is_apply"]) {
				NetHttp.inst.Send (NetBase.HTTP_GUILD_CANCEL_MEMBER, "gid="+(_da ["id"]).ToString(), OnGuildQuxiao,'|',errorHttp);
			} else {
				if((_da ["member_count"]).ToString() == ((int)(ModelManager.inst.guildModel.getGuildCfg("society")["society_num"])).ToString())
				{
					ViewManager.inst.ShowText(Tools.GetMessageById("20169"));
					return;
				}
				Dictionary<string,object> daAtt = (Dictionary<string,object>)_da["attrs"];
				if(daAtt.ContainsKey("join_condition"))
				{
					Dictionary<string,object> daAtt2 = (Dictionary<string,object>)daAtt["join_condition"];
					if(daAtt2.ContainsKey("type")&&((int)daAtt2["type"] == 2||(int)daAtt2["type"] == 1))
					{
						if(userModel.lv < (int)daAtt2["lv"])
						{
							ViewManager.inst.ShowText(Tools.GetMessageById("20185"));
							return;
						}else if(userModel.effort_lv<(int)daAtt2["effort_lv"])
						{
							ViewManager.inst.ShowText(Tools.GetMessageById("20184"));
							return;
						}
						else if(userModel.rank_score<(int)daAtt2["rank_score"])
						{
							ViewManager.inst.ShowText(Tools.GetMessageById("20186"));
							return;
						}
					}
				}
				NetHttp.inst.Send (NetBase.HTTP_GUILD_APPLY_MEMBER, "gid="+(_da ["id"]).ToString(), OnGuildShenqing,'|',errorHttp);
			}
			curobj = newlistData[index];
			curIndex = index;
			curbtn = btn_btn;
		});
	}
	private void errorHttp(VoHttp vo)
	{
		switch (vo.return_code) {
		case "10108":
			this.DispatchGlobalEvent(new MainEvent (MainEvent.GUILD_ESC));
			break;
		}
	}
	private object curobj;
	private int curIndex;
	private GButton curbtn;
	private void OnGuildShenqing(VoHttp vo)
	{
		if (vo.data is bool) {
//			listData [curIndex] = true;
			curbtn.GetChild("title").asTextField.strokeColor = Tools.GetColor("dd8680");
			curbtn.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_15");
//			curbtn.GetController ("c1").selectedIndex = 1;
			curbtn.text = Tools.GetMessageById ("20106");
			((Dictionary<string,object>)curobj) ["is_apply"] = true;
		}else {
//			this.DispatchGlobalEvent(new MainEvent (MainEvent.GUILD_ESC));
		}
	}
	private void OnGuildQuxiao(VoHttp vo)
	{
		if (vo.data is bool) {
//			listData [curIndex] = false;
			curbtn.GetChild("title").asTextField.strokeColor = Tools.GetColor("52b04f");
			curbtn.GetChild ("n6").asLoader.url = Tools.GetResourceUrl ("Image2:n_btn_14");
//			curbtn.GetController ("c1").selectedIndex = 0;
			curbtn.text = Tools.GetMessageById ("20105");
			((Dictionary<string,object>)curobj) ["is_apply"] = false;
		}
	}
	private void GetNewPageRank(VoHttp vo)
	{
		if (((object[])vo.data).GetLength (0) == 10) {
			have10 = true;
		} else {
			have10 = false;
		}
		int num = listData.Count;
		newlistData = new List<object> ();
		for (int i = 0; i < ((object[])vo.data).GetLength (0); i++) {
			listData.Add(((object[])vo.data)[i]);
		}
		newlistData = ((List<object>)Tools.Clone (listData));
		if (have10) {
			newlistData.Add (true);
		}
		list.numItems = newlistData.Count;
		list.ScrollToView ((newlistData.Count - 12)<0?0:(newlistData.Count - 12));
	}
	private void GetGuildInfo(VoHttp vo)
	{
		MediatorGuildInfo.data = (Dictionary<string,object>)(vo.data);
		ViewManager.inst.ShowView<MediatorGuildInfo> ();
	}
	//分割线//////////////////////////////////////////////////////////////////////////////////////修改请修改两处
	public override void Clear()
	{
		base.Clear();
	}
}
