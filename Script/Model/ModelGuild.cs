using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelGuild : BaseModel
{

	public List<object> guildList;
	public Dictionary<string,object> my_guild_info;
	public int my_guild_job = -1;
	public Dictionary<string,object> my_guild_member;
	public object[] word_guild_list;
	public bool guildHave = false;
	public int location;

	public VoHttp viewData;

	public ModelGuild ()
	{
	}

	public Dictionary<string,object> getGuildCfg (string id)
	{
		return (Dictionary<string,object>)(DataManager.inst.guild [id]);
	}

	public string getJob (int job)
	{
		string str = "";
		switch (job)
		{
		case 0:
			str = Tools.GetMessageById ("20120");
			break;
		case 1:
			str = Tools.GetMessageById ("20121");
			break;
		case 2:
			str = Tools.GetMessageById ("20122");
			break;
		case 3:
			str = Tools.GetMessageById ("20123");
			break;
		}
		return str;
	}

	public void SetGuildList (object[] da)
	{
		if (guildList == null)
			guildList = new List<object> ();
		if (da.Length == 0)
			return;
		for (int i = 0; i < da.GetLength (0); i++)
		{
			guildList.Add (da [i]);
		}
	}

	public bool IsKick (int job)
	{
		if (job >= 0 && job <= 2)
			return true;
		else
			return false;				
	}
}
