using System;
using FairyGUI;

public class BaseCom:GComponent
{
	public BaseCom ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
//		UnityEngine.Debug.LogError ("base ConstructFromXML");
		base.ConstructFromXML (xml);

//		this.onRemovedFromStage.Add (Clear);
	}

	public virtual void Clear ()
	{
	}
	override public void Dispose(){
		base.Dispose ();
		Clear ();
	}
}