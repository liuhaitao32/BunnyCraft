using System;
using System.Collections.Generic;

public class FilterManager
{
	private static FilterManager instance;

	private Dictionary<string,object> _result;
	//	private int _checkLen = 7;
	private string _replace = "*";

	public FilterManager ()
	{
		_result = new Dictionary<string, object> ();
	}

	public static FilterManager inst
	{
		get
		{
			if (instance == null)
				instance = new FilterManager ();
			return instance;
		}
	}

	public string Exec (string value)
	{
//		long time = Tools.GetSystemTicks ();
		int index = GetCheckIndex (0, value);
		while (index != -1)
		{
			value = GetReplaceMsg (index, value);
			index++;
			index = GetCheckIndex (index, value);
		}
//		Log.debug (Tools.GetSystemTicks () - time);
		return value;
	}

	public void Decode (string value, string replace = "*")
	{		
		_replace = replace;
		string[] strs = value.Split (new string[]{ "\n" }, 0);
		string f;
		Dictionary<string,object> temp;
		for (int i = 0; i < strs.Length; i++)
		{
//			Log.debug (strs [i].ToString ());
			if (strs [i] == "")
				continue;
			f = strs [i].Substring (0, 1);
			if (_result.ContainsKey (f))
			{
				temp = (Dictionary<string,object>)_result [f];
				if (!temp.ContainsKey (strs [i]))
				{
					temp [strs [i]] = "";
				}				   
			}
			else
			{
				_result [f] = new Dictionary<string,object> ();
				(_result [f] as Dictionary<string,object>) [strs [i]] = "";
			}
		}
	}

	private int GetCheckIndex (int index, string msg)
	{
		int o = -1;
		if (index > msg.Length)
			return o;
		msg = msg.Substring (index);
		for (int i = 0; i < msg.Length; i++)
		{
			if (_result.ContainsKey (msg [i].ToString ()))
			{
				o = index + i;
				break;
			}
		}
		return o;
	}

	private string GetReplaceMsg (int index, string value, int len = 0)
	{		
		string msg = value.Substring (index);
		string f = msg [0].ToString ();
		string str = f;
		for (int i = 1; i <= len; i++)
			str += msg [i].ToString ();
//		Log.debug (str.ToString ());
		Dictionary<string,object> temp;
		if (_result.ContainsKey (f))
		{
			temp = (Dictionary<string,object>)_result [f];
			if (temp.ContainsKey (str))
			{
				string start = value.Substring (0, index);
				string zhone = "";
				string end = value.Substring (index + str.Length);
				for (int e = 0; e < str.Length; e++)
				{
					zhone += _replace;
				}
				return  start + zhone + end;
			}
		}
			
		if (len >= value.Length - index - 1)
			return value;
		len++;
		return GetReplaceMsg (index, value, len);
	}
}