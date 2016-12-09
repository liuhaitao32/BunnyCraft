using UnityEngine;
using System.Collections;

public class ComHurt : BaseMediator
{
	public ComHurt ()
	{

	}

	public override void Init ()
	{
		base.Init ();
		Create (Config.COM_HURT);
	}

	public override void Clear ()
	{
		base.Clear ();
	}
}