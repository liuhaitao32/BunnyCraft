using System;
using System.Collections.Generic;

public class VoHttp
{
	public string actionid;
	public string method;
	public Dictionary<string,object> param;

	public object data;
	public string return_code;
	public DateTime server_now;

	public VoHttp ()
	{
	}

	public byte[] ToBytes (string method, string value, char chars = '|')
	{
		this.actionid = DateTime.Now.Ticks.ToString ();
		this.method = method;

		Dictionary<string,object> data = new Dictionary<string, object> ();
		data ["method"] = this.method;
		data ["actionid"] = this.actionid;
		param = new Dictionary<string, object> ();
		if (value != "" && value != null)
		{
			string[] msg = value.Split (chars);
			string[] t;
			for (int i = 0; i < msg.Length; i++)
			{
				t = msg [i].Split ('=');
				param [t [0]] = t [1]; 
			}
		}
		param ["app_version"] = PlatForm.inst.App_Version;
		param ["cfg_version"] = PlatForm.inst.Cfg_Version;
		data ["params"] = param;
		data ["sessionid"] = ModelManager.inst.userModel.sessionid;
		data ["uid"] = ModelManager.inst.userModel.uid;
		ByteArray ba = new ByteArray ();
		ba.WriteObject (data);
		return ba.ToArray ();
	}

	public byte[] ToBytes (string method, Dictionary<string,object> value)
	{
		this.actionid = DateTime.Now.Ticks.ToString ();
		this.method = method;

		Dictionary<string, object> data = new Dictionary<string, object> ();
		data ["method"] = this.method;
		data ["actionid"] = this.actionid;

		param = value;
		param ["app_version"] = PlatForm.inst.App_Version;
		param ["cfg_version"] = PlatForm.inst.Cfg_Version;
		data ["params"] = param;
		data ["sessionid"] = ModelManager.inst.userModel.sessionid;
		data ["uid"] = ModelManager.inst.userModel.uid;
		ByteArray ba = new ByteArray ();
		ba.WriteObject (data);
		return ba.ToArray ();
	}

	public void toDatas (byte[] data)
	{		
		ByteArray ba = new ByteArray ();
		ba.WriteBytes (data, 0, data.Length);
		ba.Position = 0;
		Dictionary<string,object> d = ba.ReadObject () as Dictionary<string,object>;
		this.actionid = d ["actionid"].ToString ();
		this.method = d ["method"].ToString ();
		this.param = d ["params"] as Dictionary<string,object>;
		this.data = d ["data"];
		this.server_now = (DateTime)d ["server_now"];
		this.return_code = d ["return_code"].ToString ();
	}
}