using System;

public class ModelGame:BaseModel
{
	public float width;
	public float height;

	public long gameStartTimer;

	public DateTime timeObj;
	public bool isTimerGetServer = false;
	public bool isLogin = false;

	//config.json
	public string remote;
	public bool fightTest;
	public bool fightLocal;
	public bool loginDoubel;


	public ModelGame ()
	{
	}

	public override void Init ()
	{
		base.Init ();
	}

	public override void Clear ()
	{
		base.Clear ();
	}		

	public DateTime time {
		get{ 
			return new DateTime (timeObj.Ticks + (System.DateTime.Now.Ticks - gameStartTimer));
		}
		set{
			timeObj = value;
		}
	}
}