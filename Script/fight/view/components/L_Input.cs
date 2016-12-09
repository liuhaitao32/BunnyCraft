using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class L_Input :InputField
{
	public delegate void InputEvent ();

	public InputEvent onSelect;
	public InputEvent onDeselect;
	public InputEvent onPointerClick;
	public InputEvent onSubmit;

	public override void OnSelect (UnityEngine.EventSystems.BaseEventData eventData)
	{
		base.OnSelect (eventData);
		if (onSelect != null)
			onSelect ();
	}

	public override void OnDeselect (UnityEngine.EventSystems.BaseEventData eventData)
	{
		base.OnDeselect (eventData);
		if (onDeselect != null)
			onDeselect ();
	}

	public override void OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnPointerClick (eventData);
		if (onPointerClick != null)
			onPointerClick ();
	}

	public override void OnSubmit (UnityEngine.EventSystems.BaseEventData eventData)
	{
		base.OnSubmit (eventData);
		if (onSubmit != null)
			onSubmit ();
	}		
		
}