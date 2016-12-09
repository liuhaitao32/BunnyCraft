using System;
using FairyGUI;

public class ComNumeric:BaseCom
{
	public Action OnChange;

	private GButton btn1;
	private GButton btn2;
	private GTextField t;

	public int value = 1;
	public int min = 1;
	public int max = 99;
	public int step = 1;

	public ComNumeric ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);

		btn1 = this.GetChild ("n1").asButton;
		btn2 = this.GetChild ("n2").asButton;
		t = this.GetChild ("n3").asTextField;
		t.text = min.ToString ();

		btn1.onClick.Add (Btn1_Click);
		btn2.onClick.Add (Btn2_Click);
	}

	public override void Clear ()
	{
		base.Clear ();
	}

	public void SetMinMax (int min, int max, int step = 1)
	{
		this.min = min;
		this.max = max;
		this.value = min;
		this.step = step;

		Check ();
		t.text = value.ToString ();
	}

	private void Btn1_Click ()
	{
		value -= step;
		Check ();
		t.text = value.ToString ();

		if (OnChange != null)
			OnChange ();
	}

	private void Btn2_Click ()
	{
		value += step;
		Check ();
		t.text = value.ToString ();

		if (OnChange != null)
			OnChange ();
	}

	public void SetValue (int value)
	{
		this.value = value;
		Check ();
		t.text = this.value.ToString ();

		if (OnChange != null)
			OnChange ();
	}

	private void Check ()
	{
		if (value <= min)
		{
			value = min;
			btn1.enabled = false;
		}
		else
		{
			btn1.enabled = true;
		}

		if (value >= max)
		{
			value = max;
			btn2.enabled = false;
		}
		else
		{
			btn2.enabled = true;
		}
	}
}