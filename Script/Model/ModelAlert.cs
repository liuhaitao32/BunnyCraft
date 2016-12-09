using System;

public class ModelAlert:BaseModel
{
	public delegate void AlertEvent (int index);

	public AlertEvent onAlert;

	public string text;
	public bool isYesAndNo = false;
	public bool close = false;
	public int showType = 0;
	public string id;
	public int count;
    public bool isOpen=true;//是否有弹板

	//tip
	public object data;
	public string type;
	public bool isTip = false;

	public ModelAlert ()
	{
	}

	public void execute (int index)
	{
		if (onAlert != null)
		{
			onAlert (index);
		}
	}

}