using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class L_Button :Button
{
	public delegate void ButtonEvent ();

	public ButtonEvent onPointerClick;
	public ButtonEvent onPointerDown;
	public ButtonEvent onPointerUp;
	public ButtonEvent onPointerEnter;
	public ButtonEvent onPointerExit;

	private ButtonEvent onUpDownUpdate;
	private bool isDown = false;
	private string timer = null;
	private float time;
	private float delay;

	private LTDescr lt;

	public void SetEnable (bool value)
	{				
		Button btn = this.gameObject.GetComponent<Button> ();
		btn.interactable = value;
		if (value)
			btn.image.color = new Color (1f, 1f, 1f);
		else
			btn.image.color = new Color (0.5f, 0.5f, 0.5f);
	}

	public void SetText (string value)
	{
		Tools.FindChild<Text> (this.gameObject, "L_Label").text = value;
	}

	public override void OnPointerClick (PointerEventData eventData)
	{		
		base.OnPointerClick (eventData);
		if (onPointerClick != null)
			onPointerClick ();
	}

	public override void OnPointerDown (PointerEventData eventData)
	{
		base.OnPointerDown (eventData);
		if (onPointerDown != null)
			onPointerDown ();
		isDown = true;

		if (timer != null)
			return;
		if (onUpDownUpdate != null)
		{			
			lt = LeanTween.value (this.gameObject, 0f, 1f, this.delay).setOnComplete (() =>
			{
				if (!isDown)
					return;
				timer = RenderManager.Instance ().AddTimeUpdate ((float t) =>
				{					
					if (isDown)
						this.onUpDownUpdate ();
					else
					{
						RenderManager.Instance ().RemoveTimeUpdate (timer);					
						timer = null;
					}
				}, this.time);	
			});
		}
	}

	public override void OnPointerUp (PointerEventData eventData)
	{		
		base.OnPointerUp (eventData);
		if (onPointerUp != null)
			onPointerUp ();
		isDown = false;

		if (lt != null)
			lt.cleanup ();
	}

	public void SetUpDownUpdate (ButtonEvent fun, float time = 1, float delay = 1)
	{
		this.onUpDownUpdate = fun;
		this.time = time;
		this.delay = delay;
	}

	public override void OnPointerEnter (PointerEventData eventData)
	{
		base.OnPointerEnter (eventData);
		if (onPointerEnter != null)
			onPointerEnter ();
	}

	public override void OnPointerExit (PointerEventData eventData)
	{
		base.OnPointerExit (eventData);
		if (onPointerExit != null)
			onPointerExit ();
	}

}