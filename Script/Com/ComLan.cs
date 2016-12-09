using System;
using FairyGUI;
using DG.Tweening;
using UnityEngine;
public class ComLan:BaseMediator
{
    //private ComProgressBar bar;
    private GProgressBar Probar;
    private GGroup gg;
    private GLoader bg;
	public ComLan ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.COM_LAN, true);
        //GImage bg = this.GetChild ("n2").asImage;
        int num = Tools.GetRandom(0, 4);
        bg = this.GetChild("n2").asLoader;
        bg.url = Tools.GetResourceUrl("Load:qidongye" + num.ToString());
		bg.height = this.height;
		//ViewManager.bgOffsetW = bg.width * (bg.height / 640);
		//ViewManager.bgOffsetX = -(ViewManager.bgOffsetW - this.width) / 2;
		bg.width = ViewManager.bgOffsetW;
		bg.x = ViewManager.bgOffsetX;
        //bar = this.GetChild ("n4").asCom as ComProgressBar;
        //bar.skin = ComProgressBar.BAR4;
        //bar.value = 0;
        //bar.max = 100;
        //bar.height = 46;
        //bar.side.height = 23;
        ////bar.GetChild("n1").asTextField.color= Tools.GetColor("bd2929");
        //bar.txt.color = Tools.GetColor("bd2929");
        //bar.txt.x = bar.x-bar.txt.width;
        //ViewManager.SetWidthHeight (this.GetChild ("n2"));
        //DOTween.To (() => bar.value, x => bar.value = x, 100f, 3f);
        //DOTween.To(() => bar.txt.x, x => bar.txt.x = x, bar.width-bar.txt.width, 3f);

        Probar = this.GetChild("n4").asProgress;
        Probar.GetChild("title").y += 2;
        Probar.value = 50;
        DOTween.To(() => Probar.value, x => Probar.value = x, 100f, 3f);
        gg = Probar.GetChild("n4").asGroup;
        TimerManager.inst.Add(2.8f, 1, (float ff)=> {
            if(gg != null)
                gg.visible = false;
        });
        
    }
	public override void Clear ()
	{
		base.Clear ();
	}
}