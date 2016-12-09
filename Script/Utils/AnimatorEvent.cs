using System;
using UnityEngine;

public class AnimatorEvent:MonoBehaviour
{
	public Action onEvent1;
	public Action onEvent2;
	public Action onEvent3;
	public Action onEvent4;
	public Action onEvent5;

	public AnimatorEvent ()
	{
	}

	public void OnEvent1 ()
	{
		if (onEvent1 != null)
			onEvent1 ();
	}

	public void OnEvent2 ()
	{
		if (onEvent2 != null)
			onEvent2 ();
	}

	public void OnEvent3 ()
	{
		if (onEvent3 != null)
			onEvent3 ();
	}

	public void OnEvent4 ()
	{
		if (onEvent4 != null)
			onEvent4 ();
	}

	public void OnEvent5 ()
	{
		if (onEvent5 != null)
			onEvent5 ();			
	}
			
}