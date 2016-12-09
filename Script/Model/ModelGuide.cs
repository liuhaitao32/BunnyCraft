using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelGuide : BaseModel
{
	private ModelUser userModel;

	public ModelGuide ()
	{
		userModel = ModelManager.inst.userModel;
	}

	public int CheckEffort ()
	{
		if (userModel.effort_lv != 1)
			return 0;
		int num = 0;
		Dictionary<string,object> effortCfg = (Dictionary<string,object>)(DataManager.inst.effort ["mission"]);
		List<object> lisData = new List<object> ();
		for (int i = 0; i < userModel.effort.Length; i++)
		{
			Dictionary<string,object> da = (Dictionary<string,object>)userModel.effort [i];
			Dictionary<string,object> dacfg = (Dictionary<string,object>)(effortCfg [da ["eid"].ToString ()]);
			Dictionary<string,object> ddd = new Dictionary<string, object> ();
			ddd.Add ("rate", (int)(da ["rate"]));
			ddd.Add ("need", (object[])dacfg ["need"]);
			int need = (int)((object[])ddd ["need"]) [(((object[])ddd ["need"]).Length) - 1];
			if (need <= (int)(da ["rate"]))
			{
				num++;
			}
		}
//		if (num > 1)
//			return true;
		return num;
	}
}
