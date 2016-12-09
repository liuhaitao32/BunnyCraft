using UnityEngine;
using System;

public class AnimatorState : StateMachineBehaviour
{
	public Action onEnter;
	public Action onExit;
	public Action onUpdate;
	public Action<Animator> onEnd;

	private bool isEnd = false;

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter (animator, stateInfo, layerIndex);
		isEnd = false;
		if (onEnter != null)
		{
			onEnter ();
		}
	}

	override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (onExit != null)
		{
			onExit ();
		}
	}

	override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (onUpdate != null)
		{
			onUpdate ();
		}

		if (!isEnd)
		{
			if (stateInfo.normalizedTime >= 1)
			{
//                Debug.LogError("On Attack normalizedTime >= " + stateInfo.normalizedTime);
				if (onEnd != null)
				{
					onEnd (animator);
				}
				isEnd = true;
			}
		}
	}
		
}
