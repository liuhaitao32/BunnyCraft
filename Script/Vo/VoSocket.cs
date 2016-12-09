using System;
using System.Collections.Generic;

public class VoSocket
{
	public string method;
	public object data;

	public VoSocket ()
	{
	}

	public byte[] ToBytes (string method, string value, char chars = '|')
	{
		this.method = method;

		List<object> list = new List<object> ();
		list.Add (this.method);

		Dictionary<object,object> param = new Dictionary<object, object> ();
		if (value != "" && value != null)
		{
			string[] msg = value.Split (chars);
			for (int i = 0; i < msg.Length; i++)
			{
				param [msg [0]] = msg [1];
			}
		}	
		list.Add (param);

		ByteArray ba = new ByteArray ();
		ba.WriteObject (list);
		return ba.ToArray ();
	}

	public byte[] ToBytes (string method, Dictionary<string,object> value)
	{
		this.method = method;

		List<object> list = new List<object> ();
		list.Add (this.method);
		list.Add (value);

		ByteArray ba = new ByteArray ();
		ba.WriteObject (list);
		return ba.ToArray ();
	}

	public void toDatas (byte[] data)
	{
		ByteArray ba = new ByteArray ();
		ba.WriteBytes (data, 0, data.Length);
		ba.Position = 0;
		object o = ba.ReadObject ();
		if (o == null)
		{
			Log.debug ("x");
			return;
		}
		if (o is object[])
		{
			object[] oo = (object[])o;
			this.method = oo [0].ToString ();
			this.data = (object)oo [1];
		}
		else
		{
			Dictionary<string,object> oo = (Dictionary<string,object>)o;
			this.method = (string)oo ["method"];
			this.data = (object)oo ["data"];
		}
	}
}