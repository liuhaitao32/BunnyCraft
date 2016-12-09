using System;
using UnityEngine;

public class Bezier2D
{
	private Vector2 a;
	private Vector2 b;
	private Vector2 c;
	private Vector2 d;
	private string type;

	public const string BEZIER2D_2 = "bezier2d_2";
	public const string BEZIER2D_3 = "bezier2d_3";

	public Bezier2D (string type, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
	{
		this.type = type;
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}

	public Vector2 GetPosition (float time)
	{
		double t = Convert.ToDouble (time);
		Vector2 v = new Vector2 ();
		if (this.type == BEZIER2D_2)
		{
			v.x = Convert.ToSingle (Math.Pow ((1 - t), 2) * a.x + 2 * t * (1 - t) * b.x + Math.Pow (t, 2) * c.x);
			v.y = Convert.ToSingle (Math.Pow ((1 - t), 2) * a.y + 2 * t * (1 - t) * b.y + Math.Pow (t, 2) * c.y);
		}
		else
		{
			v.x = Convert.ToSingle (Math.Pow ((1 - t), 3) * a.x + 3 * b.x * t * (1 - t) * (1 - t) + 3 * c.x * t * t * (1 - t) + d.x * Math.Pow (t, 3));
			v.y = Convert.ToSingle (Math.Pow ((1 - t), 3) * a.y + 3 * b.y * t * (1 - t) * (1 - t) + 3 * c.y * t * t * (1 - t) + d.y * Math.Pow (t, 3));
		}
		return v;
	}
}