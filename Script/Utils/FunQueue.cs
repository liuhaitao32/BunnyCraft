using System;
using System.Collections.Generic;

public class FunQueue
{
	public Action complete;
	private List<Action> list;

	public FunQueue ()
	{
        
	}

	public void Init (List<Action> list)
	{
		this.list = list;
		this.Next ();
	}

	public void Next ()
	{
		if (0 != this.list.Count)
		{
			Action fun = this.list [0];
			this.list.RemoveAt (0);
			fun ();
		}
		else
		{
			if (null != this.complete)
				this.complete ();
		}        
	}
}
