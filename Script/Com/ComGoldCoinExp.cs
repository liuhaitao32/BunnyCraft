using UnityEngine;
using System.Collections;
using System;
using FairyGUI;
using DG.Tweening;

public class ComGoldCoinExp : BaseMediator
{
	private ModelUser userModel;

	public static Vector2 expXY = new Vector2 (112, 75f);
	public static Vector2 goldXY = new Vector2 (Tools.offectSetX (421f), 35f);
	public static Vector2 coinXY = new Vector2 (Tools.offectSetX (625f), 32f);
	public static Vector2 cardXY = new Vector2 (Tools.offectSetX (350f), 548f);
	public static Vector2 el_scoreXY = new Vector2 (Tools.offectSetX (950), 25f);
	public static Vector2 redbag_coinXY = new Vector2 (Tools.offectSetX (1055,1), 80);

	private GProgressBar exp;
	private GComponent gold;
	private GComponent coin;

	private GTextField name;
	private GTextField lv;
	private GButton head;
	//	private ComProgressBar bar;

	public static int count = 0;
	public static int Elcount = 0;
	public static int Redcount = 0;

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.COM_GOLDCOINEXP, true);
		this.width = GRoot.inst.width;

		userModel = ModelManager.inst.userModel;
		exp = this.GetChild ("n54").asProgress;
		gold = this.GetChild ("n2").asCom;
		coin = this.GetChild ("n1").asCom;

		name = this.GetChild ("n3").asTextField;
		lv = this.GetChild ("n12").asCom.GetChild ("n2").asTextField;
		head = this.GetChild ("n12").asCom.GetChild("n0").asButton;
		head.touchable = false;
//		bar = view.GetChild ("n54") as ComProgressBar;

//		exp.skin = ComProgressBar.BAR8;
		exp.GetChild ("bar").asCom.GetChild("n0").asLoader.url = Tools.GetResourceUrl ("Image2:n_icon_shengji2");
		exp.value = userModel.exp;
		exp.max = userModel.GetExpMax (userModel.lv);
		name.text = userModel.uname;
		lv.text = userModel.lv.ToString ();
		Tools.SetLoaderButtonUrl (head, ModelUser.GetHeadUrl (userModel.head ["use"].ToString ()));

		TimerManager.inst.Add (1f, 0, Over);
		this.AddGlobalListener (MainEvent.JUMP_COINGOLDEXPGET, Onfunction);
	}
	private int tempCount = 0;
	private int times = 0;
	private void Over(float t){
//		Debug.LogError ("ComGoldCoinExp.count -->> " + ComGoldCoinExp.count);
		if (tempCount == ComGoldCoinExp.count) {
			times += 1;
		}
		if (times >= 3) {
//			Debug.LogError ("ComGoldCoinExp.count -->> 强制关闭");
			ClearSelf (1);
		}
		tempCount = ComGoldCoinExp.count;
	}
	private void Onfunction (MainEvent e)
	{
		string[] str = e.data as string[];
		switch (str [0])
		{
		case Config.ASSET_GOLD:
			DOTween.Kill (gold, true);
			DOTween.Kill (gold.GetChild ("n0"));
			ColorFilter goldgggg = new ColorFilter ();
			gold.GetChild ("n1").filter = goldgggg;
			ModelManager.inst.userModel.gold += Convert.ToInt32 (str [1]);
			gold.GetChild ("n0").TweenMoveY (-2f, 0.05f);
			goldgggg.AdjustBrightness (0.4f);
			gold.TweenMoveY (18, 0.05f).OnComplete (() =>
			{
				gold.y = 27;
				gold.GetChild ("n0").y = 2;
				goldgggg.Reset ();
				gold.GetChild ("n1").filter=null;
			});
			break;
		case Config.ASSET_COIN:
			DOTween.Kill (coin, true);
			DOTween.Kill (coin.GetChild ("n0"));
			ColorFilter coingggg = new ColorFilter();
			coin.GetChild ("n2").filter = coingggg;
			ModelManager.inst.userModel.coin += Convert.ToInt32 (str [1]);
			coin.GetChild ("n0").TweenMoveY (-2f, 0.05f);
			coingggg.AdjustBrightness (0.4f);
			coin.TweenMoveY (18, 0.05f).OnComplete (() => {
				coin.y = 27;
				coin.GetChild ("n0").y = 2;
				coingggg.Reset ();
				coin.GetChild ("n2").filter = null;
			});
			break;
		case Config.ASSET_EXP:
			DOTween.Kill (exp, true);
			DOTween.Kill (exp.GetChild ("n1"));
			ColorFilter expgggg = new ColorFilter();
			ModelManager.inst.userModel.exp += Convert.ToInt32 (str [1]);
			exp.GetChild ("n1").TweenScale (new Vector2 (1.1f, 1.1f), 0.05f);
			expgggg.AdjustBrightness (0.4f);
			exp.TweenMoveY (60, 0.05f).OnComplete (() =>
			{
				exp.y = 55;
				exp.GetChild ("n1").scale = new Vector2 (1f, 1f);
				expgggg.Reset ();
				exp.InvalidateBatchingState();
			});
			exp.value = userModel.exp;
            int i = 0;
			exp.max = userModel.GetExpMax (userModel.lv);
            
            if(exp.value>=exp.max) {
                    exp.value -= exp.max;
                    i++;
            }
            exp.max = userModel.GetExpMax(userModel.lv + i);
            exp.text = userModel.exp + "/" + exp.max;
			lv.text = userModel.lv.ToString ();
			break;
		}

		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.USER_UPDATE));
		ClearSelf (0);
	}
	private void ClearSelf(int type){
		if (type > 0) {
			ComGoldCoinExp.count = 0;
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.JUMP_OVER));
			return;
		}
		ComGoldCoinExp.count--;
		if (ComGoldCoinExp.count <= 0)
		{
			ComGoldCoinExp.count = 0;
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.JUMP_OVER));
		}
	}
	public override void Clear ()
	{
		TimerManager.inst.Remove (Over);
		this.RemoveGlobalListener (MainEvent.JUMP_COINGOLDEXPGET, Onfunction);
		base.Clear ();
	}
}
