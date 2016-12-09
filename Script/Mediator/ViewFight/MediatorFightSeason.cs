using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using DG.Tweening;
using UnityEngine;
public class MediatorFightSeason:BaseMediator
{
//	private ComRankScore rank1;
//	private ComRankScore rank2;
	public static object[] data;
	private ModelUser userModel;
	private GLoader rank;
	private Action<float> eff;
	private int mTotal = 0;
	private int mLast = 0;
	private int mChange = 0;
	//
	private int mAddVal = 1;
	private int mAddValEnd = 0;
	//
	private bool clip1 = false;
	private List<SeasonClipObj> clipList;
	//
	private GTextField mLastT;
	private GTextField mChangeT;
	private int mLastT_clip_times = 0;
	private int mChangeT_clip_times = 0;
	//
	private GObject cup;
	private float cupY;
	private float rankY;
	public MediatorFightSeason ()
	{
	}

	public override void Init ()
	{
		base.Init ();
		this.Create (Config.VIEW_FIGHTSEASON, false, Tools.GetMessageById ("25053"));
//		this.GetChild ("n5").text = Tools.GetMessageById ("25054");
		this.GetChild ("n8").text = Tools.GetMessageById ("25055");
		this.GetChild ("n10").text = Tools.GetMessageById ("25056");
		this.GetChild("n13").asTextField.text = Tools.GetMessageById ("25054");
		//
//		this.GetChild("n14").asTextField.text = data[1]+"";
		mLastT = this.GetChild("n19").asTextField;

		mChangeT = this.GetChild ("n20").asTextField;//.text = data [2]+"";

		cup = this.GetChild ("n18");
		cupY = cup.y;
		//
		userModel = ModelManager.inst.userModel;
		rank = this.GetChild ("rank").asLoader;
		rankY = rank.y;
		rank.url = userModel.GetRankImg ((int)data [0]);
//		rank1 = this.GetChild ("n12").asCom as ComRankScore;
//		rank2 = this.GetChild ("n0").asCom as ComRankScore;

//		Tools.StartSetValue(this.GetChild ("n7").asCom, data[1]+"", "");
//		this.GetChild ("n7").asCom.GetChild ("n4").visible = false;
		GLoader rankBg = this.GetChild("n22").asLoader;
		int rankNum = Convert.ToInt32(data[1]);
		this.GetChild("n14").asTextField.text = Tools.StartValueTxt(rankNum);
		switch (rankNum) {
			case 1:
				rankBg.url = Tools.GetResourceUrl ("Image2:n_icon_paiming6");//n_icon_paiming6
				break;
			case 2:
				rankBg.url = Tools.GetResourceUrl ("Image2:n_icon_paiming7");
				break;
			case 3:
				rankBg.url = Tools.GetResourceUrl ("Image2:n_icon_paiming8");
				break;
			default:
				rankBg.url = Tools.GetResourceUrl ("Image2:n_icon_paiming5");
				break;
		}
		//
		eff = EffectManager.inst.RotationLight (this.GetChild("light").asImage);
		//
		mLast = Convert.ToInt32 (data [0]);
		mChange = Convert.ToInt32 (data [2]);
		mTotal = mLast + mChange;
		//
		if(mChange>10){
			mAddValEnd = mChange % 10;
			mAddVal = (int)Math.Floor ((float)(mChange / 10));

			mLastT_clip_times = mAddValEnd;
			mChangeT_clip_times = mAddValEnd;
		}
		else{
			mAddVal = 1;
			mAddValEnd = 0;
		}
//		Debug.LogError(mLast + " || " +mChange);
		//
		mLastT.text = mTotal+"";
		//
		clipList = new List<SeasonClipObj>();
		TimerManager.inst.Add (0.03f, 0, UpdateClip);
//
		if (mChange > 0) {
			clip1 = true;
			//
			Play_mLastT ();
		} else {
			mLastT.text = mLast+"";
			mChangeT.text = mChange+"";
		}
	}

	public void Play_mLastT(){
		
		mLastT.scaleX = 1;
		if (mLastT_clip_times < mChange) {
			DOTween.To (() => rank.y, x => rank.y = x, rank.y-5, 0.1f)
				.SetUpdate (true).SetTarget (mLastT).OnComplete (()=>{
					rank.y = rankY;
				});
			DOTween.To (() => mLastT.scaleX, x => mLastT.scaleX = x, 0.9f, 0.2f)
			.SetUpdate (true)
			.SetTarget (mLastT).OnComplete (() => {
				mLastT.text = (mTotal - mLastT_clip_times) + "    [color=#348DFF]-"+mLastT_clip_times+"[/color]";
					mLastT.InvalidateBatchingState();
				NewPoint();
				Play_mLastT ();
			});
		} else {
			mLastT.text = mLast + "    [color=#348DFF]-" + mChange + "[/color]";
		}
		mLastT_clip_times += mAddVal;
	}
	public void Play_mChangeT(){
		mChangeT_clip_times += mAddVal;
		mChangeT.scaleX = 1;
		float endY = cup.y + 5;
		if (mChangeT_clip_times <= mChange) {
			DOTween.To (() => cup.y, x => cup.y = x, endY, 0.2f)
				.SetUpdate (true)
				.SetTarget (mChangeT).OnComplete(()=>{
					cup.y = cupY;
				});
			DOTween.To (() => mChangeT.scaleX, x => mChangeT.scaleX = x, 0.9f, 0.2f)
				.SetUpdate (true)
				.SetTarget (mChangeT).OnComplete (() => {
					mChangeT.text = (mChangeT_clip_times)+"";// + "    [color=#317D39]+"+mChangeT_clip_times+"[/color]";//#317D39
					mChangeT.scaleX = 1;
					mChangeT.InvalidateBatchingState();
//					NewPoint();
//					Play_mChangeT ();
				});
		} else {
//			mLastT.text = mChange +"";
		}

	}
	public void NewPoint(){
		GObject end = this.GetChild ("n18");
		SeasonClipObj clip = new SeasonClipObj ();
		string ui =  Tools.GetResourceUrl ("Image2:smallpoint");
		float sx = rank.x - 5;
		clip.speed = -Tools.GetRandom (5, 10);// * 0.1f;
		clip.dis = -60;
		clip.setData (this.view, ui, sx + Tools.GetRandom (1, 65), rank.y + Tools.GetRandom (1, 20),new Vector2(end.x,end.y),0.05f);
		//
		clipList.Add(clip);
	}

	public void UpdateClip(float t){
		if (clip1 && clipList!=null) {
			for (int i = 0; i < clipList.Count; i++) {
				clipList [i].moveY ();
				if (clipList [i].stop && !clipList [i].clipOver) {
					clipList [i].clipOver = true;
					Play_mChangeT ();
				}
			}
		}
	}
	public override void Clear ()
	{
//		Debug.LogError ("Clear this");
		base.Clear ();
		TimerManager.inst.Remove (eff);
		TimerManager.inst.Remove (UpdateClip);
		DOTween.Kill (mLastT);
		DOTween.Kill (mChangeT);
		DOTween.Kill (rank);
		DOTween.Kill (cup);
		clipList.Clear ();
		clipList = null;
	}
}
class SeasonClipObj{
	public float x;
	public float y;
	public float sx;
	public float sy;
	public float ex;
	public float ey;
	public float speed;
	public float dis;
	public GComponent parent;
	public GLoader self;
	public Vector2 endP;
	private Vector2 bezier;
	Bezier2D b2;
	public bool stopY;
	public bool playB2;
	public bool stop;
	public bool clipOver;

	public float b2_speed;
	public float b2_pre;

	private float delay = 2;
	//
	public void setData(GComponent _parent,string _ui,float _sx,float _sy,Vector2 _endP,float _b2_speed){
		parent = _parent;
		self = new GLoader();
		self.url = _ui;
		self.width = 30;
		self.height = 30;
		self.fill = FillType.ScaleFree;
//		self.autoSize = false;
		x = sx = _sx;
		y = sy = _sy;
		ex = sx + dis;
		ey = sy + dis;
		endP = _endP;
		bezier = new Vector2 (ex + 200, ey - 50);
		stopY = false;
		playB2 = false;
		b2_pre = 0;
		b2_speed = _b2_speed;
		parent.AddChild (self);
		//
		updateY();
	}
	public void moveY(){
		if (self != null && !stopY) {
			y += speed;
			if (speed < 0) {
				if (y <= ey) {
					y = ey;
					stopY = true;
					b2 = new Bezier2D (Bezier2D.BEZIER2D_2, new Vector2(x,y), bezier, endP, Vector2.zero);
					playB2 = true;
				}
			}
			updateY ();
		}
		moveB2 ();
	}
	public void moveB2(){
		if (self != null && playB2 && !stop) {
			delay -= 1;
			if (delay > 0) {
				return;
			}
			b2_pre += b2_speed;
			if(b2_pre>=1){
				b2_pre = 1;
				stop = true;
				self.visible = false;
				Clear ();
			}
			if (b2 == null) {
				return;
			}
			updateB2 (b2.GetPosition (b2_pre / 1f));
		}
	}
	public void updateY(){
		if (self != null && !stopY) {
			updateUI ();
		}
	}
	public void updateB2(Vector2 v){
		if (self != null && playB2) {
			x = v.x;
			y = v.y;
			updateUI ();
		}
	}
	public void updateUI(){
		if (self != null) {
			self.x = x;
			self.y = y;
		}
	}
	private void Clear(){
		if (self != null) {
			self.Dispose ();
		}
		self = null;
		parent = null;
		b2 = null;
	}
}