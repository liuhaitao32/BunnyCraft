using System;
using FairyGUI;

public class ComLoad:BaseMediator
{
	private GGroup gp;
    private GGraph gg;
	public ComLoad ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.COM_LOAD);

		gp = this.GetChild ("n0").asGroup;
        gg = this.GetChild("n3").asGraph;
		EffectManager.inst.AddEffect (Config.EFFECT_LOADING, "icon_loading", gg, null, false, 50);
		gp.visible = false;
		TimerManager.inst.Add (2f, 1, Time_Tick);

	}

	public override void Clear ()
	{
		TimerManager.inst.Remove (Time_Tick);
		base.Clear ();
	}

	private void Time_Tick (float time)
	{			
		if (gp != null)
			gp.visible = true;
	}
}