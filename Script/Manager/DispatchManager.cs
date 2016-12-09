using System;
using System.Collections.Generic;

public class DispatchManager
{
	private static DispatchManager instance;
	private Dictionary<string,List<Action<MainEvent>>> list;

	public DispatchManager ()
	{
		list = new Dictionary<string, List<Action<MainEvent>>>();
	}

	public static DispatchManager inst
	{
		get
		{
			if (instance == null)
				instance = new DispatchManager ();
			return instance;
		}
	}

	public void Register (string name, Action<MainEvent> fun)
	{
		if (!list.ContainsKey (name)) {
            list.Add(name, new List<Action<MainEvent>>());
		}
        List<Action<MainEvent>> funs = list[name];
        ///从后面往前查，在战斗里比较有用。
        if(!this.HasEventListener(name, fun)) {
            funs.Add(fun);
        }
	}

	public bool HasEventListener (string name)
	{
		return list.ContainsKey (name);
	}

    public bool HasEventListener(string name, Action<MainEvent> fun) {
        if(!list.ContainsKey(name)) return false;
        List<Action<MainEvent>> funs = list[name];
        return ( funs.Count > 0 && -1 != funs.LastIndexOf(fun) );
    }

    public void Unregister (string name, Action<MainEvent> fun)
	{
		if (list.ContainsKey (name))
		{
            List<Action<MainEvent>> funs = list[name];
            if(funs.Count > 0) {
                int index = funs.LastIndexOf(fun);
                if(-1 != index) funs.RemoveAt(index);
            }
		}
	}

	public void Unregisters (string name)
	{
		if (list.ContainsKey (name))
		{
			list.Remove (name);
		}
	}

	public void Clear ()
	{		
		list.Clear ();
	}

	public void Dispatch (MainEvent e)
	{
		if (!list.ContainsKey (e.name))
			return;
        //派发是从后往前，理论上不会影响程序吧。
        List<Action<MainEvent>> funs = list[e.name];
        for(int i = funs.Count - 1; i > -1; i--) {
            funs[i].Invoke(e);
        }
	}
}