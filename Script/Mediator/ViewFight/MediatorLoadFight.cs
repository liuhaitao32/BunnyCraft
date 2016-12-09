using System;
using FairyGUI;
using UnityEngine;
public class MediatorLoadFight:BaseMediator
{
	//private ComProgressBar bar;
	private LoadScenes ls;
    private GProgressBar bar;
    private GLoader bg;
	public MediatorLoadFight ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_LOADFIGHT);
        float temp = (view.root.height - view.height)/2;
        float temp2 =  (view.root.width - view.width)/2 ;
        view.width = view.root.width;
        view.height = view.root.height;
        int num = Tools.GetRandom(0, 4);
		bg = this.GetChild ("n0").asLoader;
        bg.url = Tools.GetResourceUrl("Load:qidongye" + num.ToString());
        bg.height = view.height;
		//ViewManager.bgOffsetW = bg.width * (bg.height / 640);
		//ViewManager.bgOffsetX = -(ViewManager.bgOffsetW - view.width) / 2;
        
        bg.width = ViewManager.bgOffsetW;
		bg.x -=temp2;
        bg.y = -temp;
        bar = this.GetChild("n4").asProgress;
        bar.x -= temp2;
        bar.GetChild("title").y += 4;
        bar.value = 20;


		ls = ViewManager.LoadScene ("snake", (float f) =>
		{
			Log.debug ("LoadScene - " + f.ToString ());
			float ff = f * 100;
            if(ff <= 20)
                ff = 20;
			if (ff >= 90)
				ff = 90;
			bar.value = ff;
			if (bar.value >= 90)
			{
				ls.Clear ();
			}
		});
	}

	public override void Clear ()
	{
		base.Clear ();
	}
}