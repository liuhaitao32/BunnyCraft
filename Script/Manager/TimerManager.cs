using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager
{
	private static TimerManager instance;

	private Dictionary<Action<float>, TimerListener> times;
	private Dictionary<Action<float>, TimerListener> updates;
	private List<Action<float>> removeTimes;
	private List<TimerListener> temp;

	public TimerManager ()
	{
		times = new Dictionary<Action<float>, TimerListener> ();
		updates = new Dictionary<Action<float>, TimerListener> ();
		removeTimes = new List<Action<float>> ();
	}

	public static TimerManager inst
	{
		get
		{
			if (instance == null)
				instance = new TimerManager ();
			return instance;
		}
	}

	public Action<float> Add (float interval, int repeat, Action<float> fun)
	{
		TimerListener tl = new TimerListener ();
		tl.interval = interval;
		tl.repeat = repeat;
		tl.onTime = fun;
		tl.isDelete = false;

		if (times.ContainsKey (fun))
		{
			times [fun].isDelete = false;
			times.Remove (fun);
		}
		times [fun] = tl;
		return fun;
	}

	public Action<float> AddUpdate (Action<float> fun)
	{
		TimerListener tl = new TimerListener ();
		tl.onTime = fun;

		updates [fun] = tl;

		return fun;
	}
	public void Remove (Action<float> fun)
	{
		if (fun == null)
			return;
		if (times.ContainsKey (fun))
		{
			times [fun].isDelete = true;
		}
		if (updates.ContainsKey (fun))
		{
			updates [fun].isDelete = true;
		}
	}

	public void Update ()
	{
		//		Log.debug ("delay" + Time.deltaTime.ToString () + "|time" + Time.time.ToString ());
		float time = Time.deltaTime;

		temp = new List<TimerListener> (times.Values);
		foreach (TimerListener tl1 in temp)
		{
			if (tl1.Timer (time))
				times.Remove (tl1.onTime);
		}

		temp = new List<TimerListener> (updates.Values);
		foreach (TimerListener tl2 in temp)
		{			
			if (tl2.isDelete)
				updates.Remove (tl2.onTime);
			else
				tl2.Update (time);
		}
	}
}

public class TimerListener
{
	//	public delegate void TimerEvent (float time);
	//	public TimerEvent onTime;
	public Action<float> onTime;
	public float interval;
	public int repeat;
	public bool isDelete = false;

	public float elapsed = 0;

	public bool Timer (float time)
	{
		if (isDelete)
			return isDelete;
		elapsed += time;
		//Log.debug ("time - " + time.ToString ());
		while (elapsed >= interval)
		{
			this.onTime (time);
			elapsed = elapsed - interval;
			if (repeat > 0)
			{
				repeat--;
				if (repeat == 0)
				{
					isDelete = true;
					break;
				}
			}
		}
		return isDelete;
	}

	public void Update (float time)
	{
		this.onTime (time);
	}
}
