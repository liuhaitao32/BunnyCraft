using System;
using System.Collections.Generic;
using FairyGUI;

public class ModelFight:BaseModel
{
	//排位
	public const string FIGHT_MATCHTEAM = "match_Team";
	//匹配
	public const string FIGHT_MATCH = "match";
	//自定义 乱斗
	public const string FIGHT_FREEMATCH1 = "free_match1";
	//自定义 组队
	public const string FIGHT_FREEMATCH2 = "free_match2";
	//新手
	public const string FIGHT_MATCHGUIDE = "match_guide";

	//0好友 1近期 2公会 3附近
	public object[] listInvite;
	public object[] curInvite;
	public bool isRequest = true;
	public bool isMatch = false;
	public int preTime = 0;
	public int matchTime = 0;
	public bool isPreTimeRequest = false;
	public string fightType;
	public object fightResult;

	//邀请人数据
	public List<object[]> inviteUid;

	//排位
	public string teamId;
	public object[] team1;
	public bool isLeader;

	//匹配
	public string roomId;
	public string groupId;
	public object[] team2;

	//自定义
	public int freeType = 1;
	public object[] team3;

	//结算
	public Dictionary<string, object> fightData;
	public Dictionary<string, object> fightDatatTemp;
	//主界面动画需要
	//战斗详情
	public object[] fightDataDetails;
	public string el_score;
	public string myUid;
	public int recordIndex;
	public string openIndex;
	public bool isOpenFromRecord = true;

	//micro
	public List<object> micros;
	public Dictionary<string,string> micros_Uid;

	//邀请选中页
	public int inviteIndex = -1;

	public ModelFight ()
	{
		inviteUid = new List<object[]> ();
		micros = new List<object> ();
		micros_Uid = new Dictionary<string, string> ();
	}

	public override void Clear ()
	{		
		isRequest = true;
		isMatch = false;
		isPreTimeRequest = false;
		preTime = 0;
		matchTime = 0;
		this.ClearMicro ();
	}

	public void ClearInvite ()
	{
		inviteUid.Clear ();
	}

	public void OpenFreeMatchInvite ()
	{
		bool isOk = ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_FREEMATCH1, null, true);
		if (!isOk)
			return;		
		inviteIndex = 2;
		ViewManager.inst.ShowView<MediatorFreeMatch> ();
		this.InviteRequest ();
	}

	public void ClearMicro ()
	{
		Dictionary<string,object> te;
		for (int i = 0; i < micros.Count; i++)
		{
			te = (Dictionary<string,object>)micros [i];
			(te ["micro"] as Micro).Clear ();
		}
		micros.Clear ();
		micros_Uid.Clear ();
	}

	public void AddMicro (object data)
	{
		Dictionary<string,object> da = (Dictionary<string,object>)data;
		if (micros_Uid.ContainsKey (da ["uid"].ToString ()))
			return;

		object[] o = (object[])da ["content"];
		byte[] bs = new byte[o.Length];
		int index = 0;
		while (index < o.Length)
		{
			bs [index] = Convert.ToByte (o [index].ToString ());
			index++;
		}
		Micro mi = MicroManager.inst.Play (bs, da ["uid"].ToString (), RemoveMicro);
		da.Add ("micro", mi);
		micros.Add (da);
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.MICRO_ADD));
	}

	public void AddMicro_Uid (string uid)
	{
		if (!micros_Uid.ContainsKey (uid))
		{
			micros_Uid.Add (uid, "1");
			object o = micros.Find ((object obj) =>
			{
				Dictionary<string,object> data = (Dictionary<string,object>)obj;
				if (data ["uid"].ToString () == uid)
					return true;
				return false;
			});
			micros.Remove (o);
			Dictionary<string,object> oo = (Dictionary<string,object>)o;
			Micro mi = (Micro)oo ["micro"];
			if (mi != null)
				mi.Clear ();
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.MICRO_ADD));
		}
	}

	public void RemoveMicro_Uid (string uid)
	{
		if (micros_Uid.ContainsKey (uid))
		{
			micros_Uid.Remove (uid);
		}
	}

	private void RemoveMicro (string id)
	{
		object o = micros.Find ((object obj) =>
		{
			Dictionary<string,object> da = (Dictionary<string,object>)obj;
			if (da ["uid"].ToString () == id)
				return true;
			return false;
		});	
		if (o != null)
		{
			micros.Remove (o);
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.MICRO_ADD));
		}
	}

	public void InviteRequest ()
	{
		if (isMatch)
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("25039"));
			return;
		}
		ModelRole roleModel = ModelManager.inst.roleModel;
		NetSocket.inst.AddListener (NetBase.SOCKET_GETINVITEUSERS, (VoSocket vo) =>
		{
			Log.debug ("邀请列表 - " + vo.data.ToString ());
			NetSocket.inst.RemoveListener (NetBase.SOCKET_GETINVITEUSERS);
			//			Log.debug (vo.data.ToString ());
			this.InviteData (vo.data);
			ViewManager.inst.ShowView<MediatorInvite> ();
		});

		Dictionary<string,object> data = new Dictionary<string,object> ();
		data ["lon"] = Convert.ToDouble (roleModel.longitude);
		data ["lat"] = Convert.ToDouble (roleModel.latitude);
		NetSocket.inst.Send (NetBase.SOCKET_GETINVITEUSERS, data);
	}

	public void CreatTeam (object data)
	{		
		isLeader = true;
		Dictionary<string,object> d = (Dictionary<string,object>)data;
		teamId = d ["team_id"].ToString ();
		team1 = (object[])d ["team"];
	}

	public void CreateFreeTeam (object data)
	{
		isLeader = true;
		Dictionary<string,object> d = (Dictionary<string,object>)data;
		roomId = d ["room_id"].ToString ();
		team3 = new object[8]{ d ["user"], null, null, null, null, null, null, null };
	}

	public void SetTeam (object data)
	{		
		isLeader = false;
		Dictionary<string,object> d = (Dictionary<string,object>)data;
		teamId = d ["team_id"].ToString ();
		team1 = (object[])d ["team"];
	}

	public void SetFreeTeam (object data)
	{		
		isLeader = false;
		Dictionary<string,object> d = (Dictionary<string,object>)data;
		roomId = d ["room_id"].ToString ();
		freeType = Convert.ToInt32 (d ["mode"]);
		object[] te = (object[])d ["team"];
		team3 = new object[8]{ null, null, null, null, null, null, null, null };
		for (int i = 0; i < te.Length; i++)
			team3 [i] = te [i];
	}

	public string AddTeam (object data)
	{
		List<object> list = new List<object> (team1);
		Dictionary<string,object> d = (Dictionary<string,object>)data;
		list.Add (d ["join_user"]);

		team1 = list.ToArray ();

		return ((Dictionary<string,object>)d ["join_user"]) ["uid"].ToString ();
	}

	public void ChangeTeam (object data)
	{		
		string uid = data.ToString ();
		List<object> list = new List<object> (team1);
		Dictionary<string,object> da;
		for (int i = 0; i < list.Count; i++)
		{
			da = (Dictionary<string,object>)list [i];
			if (da ["uid"].ToString () == uid)
			{
				list.RemoveAt (i);
				break;
			}
		}
		team1 = list.ToArray ();
	}

	public void ChangeFreeTeam (object data)
	{		
		Dictionary<string,object> da = (Dictionary<string,object>)data;
		int index = Convert.ToInt32 (da ["index"]);
		team3 [index] = da ["user"];
	}

	public void RepalceFreeTeam (object data)
	{		
		Dictionary<string,object> da = (Dictionary<string,object>)data;
		int old_index = Convert.ToInt32 (da ["old_pos"]);
		int new_index = Convert.ToInt32 (da ["new_pos"]);
		object tem = team3 [old_index];
		team3 [old_index] = team3 [new_index];
		team3 [new_index] = tem;
	}

	public bool IsFreeFightStart ()
	{
		bool isOk = false;
		if (this.freeType == 1)
		{
			int count = 0;
			for (int i = 0; i < team3.Length; i++)
				if (team3 [i] != null)
					count++;
			if (count <= 1)
				ViewManager.inst.ShowText (Tools.GetMessageById ("25037"));
			else
				isOk = true;
		}
		else
		{
			bool is1 = false;
			bool is2 = false;
			if (this.team3 [0] != null || this.team3 [1] != null || this.team3 [2] != null || this.team3 [3] != null)
				is1 = true;
			if (this.team3 [4] != null || this.team3 [5] != null || this.team3 [6] != null || this.team3 [7] != null)
				is2 = true;
			if (is1 && is2)
				isOk = true;
			else
				ViewManager.inst.ShowText (Tools.GetMessageById ("25038"));			
		}
		return isOk;
	}

	public int GetFreeTeamCount ()
	{
		int count = 0;
		for (int i = 0; i < team3.Length; i++)
			if (team3 [i] != null)
				count++;		
		return count;
	}

	public void ChangeModel ()
	{
		if (this.freeType == 1)
			team3 = new object[]{ team3 [0], team3 [1], team3 [2], team3 [3], team3 [4], team3 [5], team3 [6], team3 [7] };
		else
			team3 = new object[]{ team3 [0], team3 [1], team3 [4], team3 [5], team3 [2], team3 [3], team3 [6], team3 [7] };
	}

	public bool DelFreeTeam (object data)
	{
		Dictionary<string,object> da = (Dictionary<string,object>)data;
		int index = Convert.ToInt32 (da ["index"]);

		bool b = false;
		Dictionary<string,object> tem = (Dictionary<string,object>)team3 [index];
		if (tem ["uid"].ToString () == ModelManager.inst.userModel.uid)
			b = true;
		else
			b = false;
		team3 [index] = null;
		return b;
	}

	public void InviteData (object data)
	{
		listInvite = (object[])data;
	}

	public object[] GetInviteData (int index)
	{
		object[] o = (object[])listInvite [index];
		for (int i = 0; i < o.Length; i++)
		{
			((Dictionary<string,object>)o [i]) ["check"] = false;
		}
		curInvite = o;
		return o;
	}

	public bool GetInviteSelectAll (int index)
	{
		bool isOK = true;
		object[] o = (object[])listInvite [index];
		for (int i = 0; i < o.Length; i++)
		{			
			if (Convert.ToBoolean (((Dictionary<string,object>)o [i]) ["check"]) == false)
			{
				isOK = false;
				break;
			}
		}
		return isOK;
	}

	public void SetCheck (bool value)
	{
		for (int i = 0; i < curInvite.Length; i++)
			((Dictionary<string,object>)curInvite [i]) ["check"] = value;
	}

	public int GetCheck ()
	{
		int count = 0;
		for (int i = 0; i < curInvite.Length; i++)
			if (Convert.ToBoolean (((Dictionary<string,object>)curInvite [i]) ["check"]) == true)
				count++;
		return count;
	}

	public string[] GetCurrentInviteUid (bool isCheck)
	{
		List<string> uid = new List<string> ();
		Dictionary<string,object> data;
		for (int i = 0; i < curInvite.Length; i++)
		{
			data = (Dictionary<string,object>)curInvite [i];
			if (isCheck)
			{
				if (Convert.ToBoolean (data ["check"]) && data ["state"].ToString () == "0")
				{
					uid.Add (data ["uid"].ToString ());				
				}
			}
			else
			{
				if (Convert.ToBoolean (data ["check"]))
				{
					uid.Add (data ["uid"].ToString ());				
				}
			}
		}
		return uid.ToArray ();
	}

	//userData
	public List<object> GetUserData (List<object> result, object[] myData, string statementType)
	{
		List<object> obj = new List<object> ();
		List<object> data0 = new List<object> ();
		List<object> data1 = new List<object> ();
		List<object> data2 = new List<object> ();
		List<object> data3 = new List<object> ();
		List<object> data4 = new List<object> ();
		List<object> data5 = new List<object> ();
		Dictionary<string, object> list = GetMaxData (result);
		for (int i = 0; i < result.Count; i++)
		{
			if (result [i] != null)
			{
				object[] a = result [i] as object[];
				object[] b = a [1] as object[];
				data0.Insert (i, b [0]);
				data1.Insert (i, b [1]);
				data2.Insert (i, b [2]);
				data3.Insert (i, b [3]);
				data4.Insert (i, b [4]);
				data5.Insert (i, b [5]);
			}
			
		}
		Sort (data0, 1);
		Sort (data1, 1);
		Sort (data2, 1);
		Sort (data3, 1);
		Sort (data4, 1);
		Sort (data5, 0);
		//得到自己每项数据的排名
		obj.Insert (0, GetMyRank (list, data0, "0", statementType, myData));
		obj.Insert (1, GetMyRank (list, data1, "1", statementType, myData));
		obj.Insert (2, GetMyRank (list, data2, "2", statementType, myData));
		obj.Insert (3, GetMyRank (list, data3, "3", statementType, myData));
		obj.Insert (4, GetMyRank (list, data4, "4", statementType, myData));
		obj.Insert (5, GetMyRank (list, data5, "5", statementType, myData));
		return obj;
	}
	//得到最大值和id
	public Dictionary<string, object> GetMaxData (List<object> result)
	{
		List<List<object>> arr_index = new List<List<object>> ();//存储最大值的位置
		List<object> arr_value = new List<object> ();//存储最大的数值

		for (int i = 0; i < result.Count; i++)
		{
			if (result [i] != null)
			{
				object[] userData = result [i] as object[];//
				object[] fightData = userData [1] as object[];

				for (int j = 0; j < 6; j++)
				{

					if (i == 0)
					{
						arr_value.Insert (j, fightData [j]);
						List<object> temp = new List<object> ();
						temp.Insert (i, i);
						arr_index.Insert (j, temp);
					}
					else
					{
						if (j == 5)
						{
							if ((int)arr_value [j] > (int)fightData [j])
							{
								arr_value [j] = fightData [j];
								((List<object>)arr_index [j]) [i - 1] = -1;
								((List<object>)arr_index [j]).Insert (i, i);
							}
							else if ((int)arr_value [j] == (int)fightData [j])
							{
								((List<object>)arr_index [j]).Insert (i, i);
							}
							else
							{
								((List<object>)arr_index [j]).Insert (i, -1);
							}
						}
						else
						{
							if ((int)arr_value [j] < (int)fightData [j])
							{
								arr_value [j] = fightData [j];
								for (int k = 0; k < ((List<object>)arr_index [j]).Count; k++)
								{
									((List<object>)arr_index [j]) [k] = -1;
								}
								((List<object>)arr_index [j]) [i - 1] = -1;
								((List<object>)arr_index [j]).Insert (i, i);
							}
							else if ((int)arr_value [j] == (int)fightData [j])
							{
								((List<object>)arr_index [j]).Insert (i, i);
							}
							else
							{
								((List<object>)arr_index [j]).Insert (i, -1);
							}
						}
					}
				}
			}
                
		}
		Dictionary<string, object> list = new Dictionary<string, object> ();
		list.Add ("0", new object[] { arr_index [0], arr_value [0] });
		list.Add ("1", new object[] { arr_index [1], arr_value [1] });
		list.Add ("2", new object[] { arr_index [2], arr_value [2] });
		list.Add ("3", new object[] { arr_index [3], arr_value [3] });
		list.Add ("4", new object[] { arr_index [4], arr_value [4] });
		list.Add ("5", new object[] { arr_index [5], arr_value [5] });
		return list;

	}


	//fightData
	public object[] GetUserMaxData (int index, List<object> result, String uid, List<int> fightDataTypeChange)
	{
        
		Dictionary<string, object> list = GetMaxData (result);
		object[] myTypeData = null;
       
		List<int> statement_data_type = new List<int> ();
		switch (index)
		{
		case 0:
			myTypeData = new object[] { 0, 0 };
			break;
		default:
			myTypeData = GetMyData (list, index, uid, fightDataTypeChange);
			break;
		}

		return new object[] { myTypeData [0], myTypeData [1] };
	}

	private int GetMyRank (Dictionary<string, object> list, List<object> data, string v, string statementType, object[] myData)
	{
		int temp0_rank = 0;
		for (int i = 0; i < data.Count; i++)
		{
			object[] dd = (object[])list [v];
			if (statementType.Equals (ModelFight.FIGHT_FREEMATCH2) || statementType.Equals (ModelFight.FIGHT_FREEMATCH1))
			{
				if ((int)data [i] == (int)myData [Convert.ToInt32 (v)])
				{
					temp0_rank = i;
					break;
				}
			}
			else
			{
				if ((int)data [i] == (int)((object[])myData [Convert.ToInt32 (v)]) [0])
				{
					temp0_rank = i;
					break;
				}
			}
		}
		return temp0_rank;
	}

	private object[] GetMyData (Dictionary<string, object> list, int index, string uid, List<int> fightDataTypeChange)
	{
		object[] data;
		object[] list_ids = null;
		List<int> fightDataType_temp = null;
		List<KeyValuePair<string, object>> arr_statement_data_type = new List<KeyValuePair<string, object>> ();
		string myUid = ModelManager.inst.userModel.uid;
		List<int> fightDataType_fixed = new List<int> ();
		for (int i = 0; i < 5; i++)
		{
			fightDataType_fixed.Add (i);
		}
		//if (!myUid.Equals (uid))
		//{
		fightDataType_temp = fightDataTypeChange;
		//}
		//else
		//{
		//	fightDataType_temp = fightDataType_fixed;
		//}
		foreach (KeyValuePair<string, object> v in list)
		{
			list_ids = (object[])v.Value;
			int type = Convert.ToInt32 (v.Key);
			foreach (int fightDataType_Index in fightDataType_temp)
			{
				if (type == fightDataType_Index)
				{
					List<object> ids = (List<object>)list_ids [0];
					if (ids.Contains (index))
					{
						arr_statement_data_type.Add (v);
						break;
					}
				}
			}
            
		}
		if (arr_statement_data_type.Count != 0)
		{
			data = GetMinValue (arr_statement_data_type);
		}
		else
		{
			data = new object[] { 0, 0 };
		}
		return data;
	}

	private object[] GetMinValue (List<KeyValuePair<string, object>> arr_statement_data_type)
	{
		int temp = 0;
		int value = 0;
		KeyValuePair<string, object> data = (KeyValuePair<string, object>)arr_statement_data_type [0];
		value = (int)((object[])data.Value) [1];
		temp = Convert.ToInt32 (data.Key);
		object[] obj = new object[] { temp, value };
		return obj;
	}

	//参数一：数据 参数二： 升降  参数三： 数据的种类
	public static void Sort (List<object> list, int type, int type1 = 0)
	{
		switch (type1)
		{
		case 0:
			list.Sort ((object a, object b) =>
			{
				if (a == null && b == null)
					return 0;
				if (a != null && b == null)
					return -1;
				if (a == null && b != null)
					return 1;
				int result = 0;
				int xi = 0;
				int yi = 0;
				xi = (int)a;
				yi = (int)b;
				result = xi.CompareTo (yi);
				if (type == 0)
					return result;
				else
					return result == 1 ? -1 : 1;
			});
			break;
		case 1://战斗结算的排序
			list.Sort ((object a, object b) =>
			{
				if (a == null && b == null)
					return 0;
				if (a != null && b == null)
					return -1;
				if (a == null && b != null)
					return 1;
				int result = 0;
				int xi = 0;
				int yi = 0;
				object[] temp1 = (object[])a;
				object[] temp2 = (object[])b;
				object[] temp1_1 = (object[])temp1 [1];
				object[] temp1_2 = (object[])temp2 [1];
				xi = (int)temp1_1 [0];
				yi = (int)temp1_2 [0];
				result = xi.CompareTo (yi);
				if (type == 0)
					return result;
				else
					return result == 1 ? -1 : 1;
			});
			break;
		case 2://战斗记录的排序
			list.Sort ((object a, object b) =>
			{
				if (a == null && b == null)
					return 0;
				if (a != null && b == null)
					return -1;
				if (a == null && b != null)
					return 1;
				int result = 0;
				int xi = 0;
				int yi = 0;
				object[] temp1 = (object[])a;
				object[] temp2 = (object[])b;
				object[] temp1_1 = (object[])temp1 [5];
				object[] temp1_2 = (object[])temp2 [5];
				xi = (int)temp1_1 [0];
				yi = (int)temp1_2 [0];
				result = xi.CompareTo (yi);
				if (type == 0)
					return result;
				else
					return result == 1 ? -1 : 1;
			});
			break;
		}
		
	}

	public Dictionary<string, object> GetFightUser (List<object> data, string uid_)
	{
		Dictionary<string, object> value = new Dictionary<string, object> ();
		for (int i = 0; i < data.Count; i++)
		{
			if (data [i] != null)
			{
				object[] uid = (object[])data [i];
				if (uid [0].ToString ().Equals (uid_))
				{
					value ["0"] = uid;
					value ["1"] = i;
				}
			}
		}
		return value;
	}

	public Dictionary<string, object> GetFightTag ()
	{
		Dictionary<string, object> fight_tag = new Dictionary<string, object> ();
		fight_tag.Add ("0", Tools.GetMessageById ("24210"));
		fight_tag.Add ("1", Tools.GetMessageById ("24211"));
		fight_tag.Add ("2", Tools.GetMessageById ("24212"));
		fight_tag.Add ("3", Tools.GetMessageById ("24213"));
		fight_tag.Add ("4", Tools.GetMessageById ("24214"));
		fight_tag.Add ("5", Tools.GetMessageById ("24215"));
		return fight_tag;
	}

	public bool isTeam (string uid)
	{
		bool isOk = false;
		Dictionary<string,object> tem;
		if (fightType == ModelFight.FIGHT_MATCHTEAM)
		{
			for (int i = 0; i < team1.Length; i++)
			{
				tem = (Dictionary<string,object>)team1 [i];
				if (tem != null && tem ["uid"].ToString () == uid)
				{
					isOk = true;
					break;
				}
			}
		}
		else if (fightType == ModelFight.FIGHT_FREEMATCH1 || fightType == ModelFight.FIGHT_FREEMATCH2)
		{
			for (int i = 0; i < team3.Length; i++)
			{
				tem = (Dictionary<string,object>)team3 [i];
				if (tem != null && tem ["uid"].ToString () == uid)
				{
					isOk = true;
					break;
				}
			}
		}
		return isOk;
	}
}