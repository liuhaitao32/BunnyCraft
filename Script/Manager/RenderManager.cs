using System;
using UnityEngine;
using System.Collections.Generic;

public class RenderManager
{
	private static RenderManager instance;

	private int instanceId = 0;
	private Dictionary<string,RenderListener> times;
	private Dictionary<string,RenderListener> overs;
	private Dictionary<string,RenderListener> updates;
	private Dictionary<string,RenderListener> fixedUpdates;

	public RenderManager ()
	{
		times = new Dictionary<string, RenderListener> ();
		overs = new Dictionary<string, RenderListener> ();
		updates = new Dictionary<string, RenderListener> ();
		fixedUpdates = new Dictionary<string, RenderListener> ();
	}

	public static RenderManager Instance ()
	{
		if (instance != null)
			return instance;
		instance = new RenderManager ();
		return instance;
	}

	private string CreateId ()
	{
		instanceId++;
		return instanceId.ToString ();
	}

	public string AddTimeUpdate (RenderListener.RenderEvent fun, float time = 1f)
	{
		string id = this.CreateId ();
		RenderListener r = new RenderListener ();
		r.onTime += fun;
		r.curtime = 0;
		r.maxTime = time;
		times [id] = r;
		return id;
	}

	public void RemoveTimeUpdate (string name)
	{
		if (name == null)
			return;
		if (times.ContainsKey (name))
			times.Remove (name);
	}

	public string AddTimeOver (RenderListener.RenderEvent fun, float over = 1f)
	{		
		string id = this.CreateId ();
		RenderListener r = new RenderListener ();
		r.onTime += fun;
		r.curtime = 0;
		r.maxTime = over;
		overs [id] = r;
		return id;
	}

	public void RemoveTimeOver (string name)
	{
		if (overs.ContainsKey (name))
			overs.Remove (name);
	}

	public string AddUpdate (RenderListener.RenderEvent fun)
	{
		string id = this.CreateId ();
		RenderListener r = new RenderListener ();
		r.onTime += fun;
		updates [id] = r;
		return id;
	}

	public void RemoveUpdate (string name)
	{
		if (updates.ContainsKey (name))
			updates.Remove (name);
	}

	public string AddFixedUpdate (RenderListener.RenderEvent fun)
	{
		string id = this.CreateId ();
		RenderListener r = new RenderListener ();
		r.onTime += fun;
		fixedUpdates [id] = r;
		return id;
	}

	public void RemoveFiexdUpdate (string name)
	{
		if (fixedUpdates.ContainsKey (name))
			fixedUpdates.Remove (name);
	}

	public void Update ()
	{
//		Log.debug (Time.deltaTime.ToString ());
		List<RenderListener> list = new List<RenderListener> (updates.Values);
		if (list.Count > 0)
		{
			foreach (RenderListener r in list)
			{
				r.Excute (Time.deltaTime);
			}
		}

		List<RenderListener> time = new List<RenderListener> (times.Values);
		if (time.Count > 0)
		{
			foreach (RenderListener t in time)
			{
				t.Check (Time.deltaTime);
			}
		}

		List<string> over = new List<string> (overs.Keys);
		if (over.Count > 0)
		{		
			RenderListener t;	
			foreach (string n in over)
			{
				t = overs [n];
				if (t.Check (Time.deltaTime))
				{											
					overs.Remove (n);
				}			
			}
		}
	}

	public void FixedUpdate ()
	{
		if (fixedUpdates.Keys.Count == 0)
			return;
		List<RenderListener> list = new List<RenderListener> (fixedUpdates.Values);
		foreach (RenderListener r in list)
		{
			r.Excute (Time.fixedTime);
		}
	}
		
}

public class RenderListener
{
	public delegate void RenderEvent (float time);

	public RenderEvent onTime;

	public float curtime;
	public float maxTime;

	public void Excute (float time)
	{
		if (onTime != null)
			onTime (time);
	}

	public bool Check (float time)
	{

        curtime += time;
        while (curtime > maxTime) {
            curtime -= maxTime;
            if (onTime != null)
			{
				onTime (curtime);
                return true;
			}
		}
        return false;
	}
}
