using System;

public class BaseModel:IBaseClass
{
	public BaseModel ()
	{
		this.Init ();
	}

	public virtual void Init ()
	{
	}

	public virtual void Clear ()
	{
	}

}