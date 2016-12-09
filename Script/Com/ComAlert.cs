using System;
using FairyGUI;

public class ComAlert:BaseMediator
{
	private ModelAlert alertModel;

	private GTextField t;
	private GButton btn1;
	private GButton btn2;
	private Controller c1;
	private GLoader loader;
	private GTextField count;
	private int closeTime = 5;

	public ComAlert ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.isAutoClose = false;
		this.Create (Config.COM_ALERT);

		alertModel = ModelManager.inst.alertModel;

		btn1 = this.GetChild ("n1").asButton;
        btn1.text = Tools.GetMessageById("14057");
		btn2 = this.GetChild ("n2").asButton;
		btn2.text = Tools.GetMessageById ("14056");

        t = this.GetChild ("n3").asTextField;
		c1 = this.GetController ("c1");
		loader = this.GetChild ("n15").asLoader;
		count = this.GetChild ("n16").asTextField;


		if (!alertModel.isYesAndNo)
		{
			btn1.visible = false;
			btn2.x = (this.width - btn2.width) / 2;
		}
		if (alertModel.close)
		{
			TimerManager.inst.Add (1f, 0, CloseSelf);
		}
		btn1.onClick.Add (Btn1_Click);
//        btn1.text = Tools.GetMessageById("14025");
		btn2.onClick.Add (Btn2_Click);
//        btn1.text = Tools.GetMessageById("14056");

        if (c1.selectedIndex != alertModel.showType)
		{
			c1.selectedIndex = alertModel.showType;
			loader.url = Tools.GetIconUrl (alertModel.id);
			count.text = Tools.GetMessageById ("14016", new string[]{ alertModel.count.ToString () });
		}
		else
		{
			t.text = alertModel.text;
		}

//		EffectManager.inst.SetFilterAdjustBrightnessChild (this.group, 5f, 0f, 1f);

		//		TimerManager.inst.Add (5.5f, 1, (float t) =>
		//		{
		//			EffectManager.inst.SetFilterAdjustBrightnessChild (g, 5f, 1f, 0f);
		//		});
	}

	public override void Clear ()
	{
		base.Clear ();
	}

	private void Btn1_Click ()
	{
		ViewManager.inst.CloseAlert ();
		alertModel.execute (0);//取消
	}

	private void Btn2_Click ()
	{
		ViewManager.inst.CloseAlert ();
		alertModel.execute (1);
	}

	private void CloseSelf (float time)
	{
		if (closeTime <= 0)
		{
			TimerManager.inst.Remove (CloseSelf);
			ViewManager.inst.CloseAlert ();
		}
		else
		{
			closeTime--;
		}
	}

	public override void Close ()
	{
		base.Close ();
		this.Btn1_Click ();
	}
}