	using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class MediatorRobRed : BaseMediator {
	object[] reData;
	public static VoHttp redVo;
	public static int _coin;
	public static int _gold;

	private GComponent[] aa = new GComponent[10];
	private GComponent[] bb = new GComponent[10];
	private int len = 0;
	private string timer;
//	private bool canClick = false;
//	private bool canMChui = true;
	//
	private bool step1 = false;
	private bool step2 = false;
	//
	private List<object> reData1;
	private int[] reNum;
	private int reNumIndex = 0;

	private GImage img_num;
	private GTextField l_num;

	private GComponent item;
	private GComponent item_box;
	private GameObject item_go;
	private GButton ClickBtn;
//	private Dictionary<string,object> userCard;
	private int mNum = 0; 
	private GComponent mpro;
	private GTextField mprotxt;
	private int cha;
    public int gold;
    public int totalNum;
    public float posY = GRoot.inst.height * 0.3f;
    public bool isLegend;
    public bool isNew;
	public MediatorRobRed ()
	{
		
	}
	public override void Init ()
	{
//		Debug.LogError (ViewManager.inst.s.displayObject.gameObject.name);
		ViewManager.inst.s.displayObject.gameObject.SetActive (false);
		base.Init ();
		this.Create (Config.VIEW_ROBRED, true);
		ViewManager.SetWidthHeight (this.GetChild ("n7"));
		ViewManager.SetWidthHeight (this.GetChild ("n8"));
		ClickBtn = this.GetChild ("n7").asButton;
//		userCard = ModelManager.inst.userModel.card;
		len = 0;
		l_num = this.GetChild ("n3").asTextField;
		img_num = this.GetChild ("n2").asImage;
        gold = _gold;
        Get_Red (redVo);
        //		view.touchable = false;
        
	}
	GameObject[] mc;
	private void Get_Red(VoHttp vo)
	{		
		Dictionary<string,object> data = (Dictionary<string, object>)vo.data;
		object[] da = data["res"] as object[];
		reData = da;
		l_num.text = da.Length.ToString ();
		mc = new GameObject[da.Length];
		for(int i = (da.GetLength (0) - 1);i>=0;i--)
		{
			item_box = Tools.GetComponent (Config.RED_PEOPLE).asCom;
			item_box.GetChild ("tag_scale").visible = false;
            this.item_box.GetChild("n1").visible = false;
            this.item_box.GetChild("n4").visible = false;
            item_go = EffectManager.inst.AddEffect (Config.EFFECT_GIFT, "stand", item_box.GetChild ("n2").asGraph);
            GameObjectScaler.ScaleParticles(item_go, 0.3f);
			Dictionary<string,object> js = (Dictionary<string,object>)da [i];
            
            //EffectManager.inst.StopAnimation (mc [i]);
            GTextField l = item_box.GetChild ("n1").asTextField;
            GButton head = item_box.GetChild("n4").asCom.GetChild("n0").asButton;
            String dc_ = (string)js["head_use"];
            if(js["uid"].ToString().StartsWith("bot")) {
                Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(dc_, false, true));
            } else {
                Tools.SetLoaderButtonUrl(head, ModelUser.GetHeadUrl(dc_));
            }
                //l.visible = false;
            if(js ["uname"] != null) {
                //l.text = Tools.GetMessageById("12010",new string[] {js["uname"].ToString() });
                l.text = js["uname"].ToString();
			} else {
                // l.text = Tools.GetMessageById("12010", new string[] { js["uid"].ToString() });
                l.text = js["uid"].ToString();
            }
            //Debug.LogError("uname  " + js["uname"].ToString());
//            Debug.LogError("head_use  " + js["head_use"].ToString()+" || "+"uid  "+js["uid"].ToString());
            //item.GetChild ("n2").asGraph.rotation = UnityEngine.Random.Range (-10f, 10f);
            mc[i] = item_go;
			aa[i] = item_box;
			item_box.SetPivot(0.5f,0.5f);
			this.AddChild(item_box);
			item_box.scale = new Vector2 (0.3f, 0.3f);//
            
			float half = Convert.ToSingle(da.GetLength (0)) / 2f + 0.5f;
			half = half - (i+1);
//			item.x = Tools.offectSetX (((float)-half * 165f) + 430);
			//item.GetChild ("n1").y = 100;
//			item.y = i%2f * 30f + 100f;
			item_box.x = Tools.offectSetX (((float)i * 80) - 160f + 250f);
			item_box.y = i % 2f * 30f + 100f;

//			if (i == 0) {
//				item.x = -100;
//				item.y = -100;
//			}
//			item.TweenMove (new Vector2 (Tools.offectSetX (((float)i * 90) - 150f + 250f), i%2f * 30f + 100f), 0f);
		}
//		for (int j = (da.GetLength (0) - 1);j>=0;j--) {
//			float half = Convert.ToSingle (da.GetLength (0)) / 2f + 0.5f;
//			half = half - (j + 1);
//			aa [j].TweenScale (new Vector2 (0.5f, 0.5f), 0.5f);
//			GameObjectScaler.ScaleParticles ((aa [j]).displayObject.gameObject, 0.3f);
//			aa [j].TweenMove (new Vector2 (Tools.offectSetX (((float)j * 90) - 150f + 250f), j%2f * 30f + 100f), 0.5f);
//		}
		//
		step1 =false;
		step2 =false;
		//
		this.SetChildIndex (img_num, this.numChildren - 1);
		this.SetChildIndex (l_num, this.numChildren - 1);
		reData1 = DataChuLione ((Dictionary<string,object>)((Dictionary<string,object>)reData [reNumIndex]) ["gifts_dict"]);
		len = reData1.Count - 1;

		//
		this.item_go = mc [reNumIndex];
		this.item_box = aa [reNumIndex];
		//
		ClickBtn.onClick.Add(Mask_Click);
		//
		OverFun (1);
	}
	private long tick = 0;
	private void Clear_card(){
		if (this.item != null) {

			DOTween.Kill (bb [len]);
			DOTween.Kill (mpro);
			TimerManager.inst.Remove(OnStarImgRatation);
			this.RemoveChild (this.item,true);
			this.RemoveChild(bb[len]);
			this.item = null;
			starImg = null;
			mpro = null;
			mprotxt = null;
		}
	}
	private bool step2Clip;
	private void Mask_Click ()
	{
        
        if ((ModelManager.inst.gameModel.time.Ticks - tick) > 10000) {
			tick = ModelManager.inst.gameModel.time.Ticks;
		} else
			return;
		if (!step1) {
			return;
		}
		if (step2) {
			return;
		}
		if (len > 0) {
			Clear_card ();
		}
		step2Clip = (len <= 0);
		len -= 1;
		if (len >= 0) {
			
			GiftMove (0.5f, len, step2Clip);
		} else {
			len = 0;
			reNumIndex += 1;
			if (reNumIndex < reData.Length){
				Clear_card ();
				DOTween.Kill (this.item_box,true);
				if (t != null) {
					t.Kill ();
					t = null;
				}
				this.RemoveChild (this.item_box);
				this.item_go = mc [reNumIndex];
				this.item_box = aa [reNumIndex];
				//
				for (int i = 0; i < bb.Length; i++) {
					if (bb [i] != null) {
						this.RemoveChild (bb [i]);
					}
				}
				//
				reData1 = DataChuLione ((Dictionary<string,object>)((Dictionary<string,object>)reData [reNumIndex]) ["gifts_dict"]);
				len = reData1.Count - 1;
				//
				step1 = false;
				//
				OverFun (1);
			}
			else{
//				if (!step2) {
					ClickBtn.onClick.Remove (Mask_Click);
					ViewManager.inst.CloseView (this);
					return;
//				}
			}
		}
	}
//	private void TiQianWanChengFunction()
//	{
//        //GameObjectScaler.ScaleParticles(( aa[reNumIndex] ).displayObject.gameObject, 3f);
////        TimerManager.inst.Remove (OnTimeFunction);
//		this.RemoveChild(this.item_box);
//        this.RemoveChild(bb[len]);
//		DOTween.Kill (this.item_box,true);
//		if (t != null) {
//			t.Kill ();
//			t = null;
//        }
//		OverFun(0);
//    }
	private void GiftMove(float time,int index,bool isStep2 = false)
	{
//		if (bb [len].x == Tools.offectSetX (600f) && bb[len].y == 200f)
//			return;
//		canMChui = false;
//		Debug.LogError (len+"   isStep2 " + step2Clip);
		if(isStep2){
			
			bb [len].scale = new Vector2 (1f,1f);
			bb [len].x = Tools.offectSetX (600f);
			bb [len].y = 200f;
//			(bb [len].GetChild ("n0") as ComBigIcon).RotateCard (time);
			//
			step2 = true;
			(bb [len].GetChild ("n0") as ComBigIcon).RotateCard (time, () => {
				showItem ((Dictionary<string,object>)reData1 [len]);
			});
            TimerManager.inst.Add(0.5f, 1, (float ff) => {
                SoundManager.inst.PlaySound(Config.SOUND_SHOWCARD);
            });
            return;
		}
		bb [len].TweenScale (new Vector2 (1f, 1f), time);
		bb [len].TweenMove (new Vector2 (Tools.offectSetX (600f), 200f),time).OnComplete(()=>{
     
		});
		step2 = true;
		(bb [len].GetChild ("n0") as ComBigIcon).RotateCard (time,()=>{
			showItem ((Dictionary<string,object>)reData1 [len]);
		});
        TimerManager.inst.Add(0.5f, 1, (float ff) => { SoundManager.inst.PlaySound(Config.SOUND_SHOWCARD);
    });
    }
	private Tweener t;
	private void OverFun(float f)
	{

		DOTween.Kill (this.item_box);
		//
		this.item_box.GetChild ("n2").rotation = 0;
        this.item_box.GetChild ("n1").visible = false;
        this.item_box.GetChild("n4").visible = false;
        //
        this.SetChildIndex (this.GetChild ("n9").asGraph, this.numChildren - 1);
	    //
		t = EffectManager.inst.Bezier(this.item_box, 0.4f, this.item_box.xy, new Vector2(Tools.offectSetX(260), posY), new Vector2(Tools.offectSetX(260), posY), () => {

		});
		this.item_box.TweenScale(new Vector2(0.8f, 0.8f), 0.5f).OnComplete(()=> {
			EffectManager.inst.PlayEffect(this.item_go, "open");

            item_go.transform.FindChild("gift03").GetComponent<AudioSource>().volume = ModelManager.inst.userModel.isSound?1:0;

            OnTimeFunction(f);
        });
	}
	private void OnTimeFunction(float f)
	{
		if (!this.item_box.GetChild("tag_scale").visible) {
			this.item_box.GetChild ("tag_scale").visible = true;
			GameObjectScaler.ScaleParticles (this.item_go, 3f);
		}
		this.item_box.GetChild("n1").visible = true;
        this.item_box.GetChild("n4").visible = true;
       // this.item_box.GetChild("n1").y = posY + 110;
        //this.item_box.GetChild("n1").x = Tools.offectSetX(80);

        OnFunOverHandler(f);
	}
	private void OnFunOverHandler(float f)
	{
//		AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo (0);

//		if (info.IsName ("open")) {
		l_num.text = (reData.Length - reNumIndex).ToString();
        reData1 = DataChuLione ((Dictionary<string,object>)((Dictionary<string,object>)reData [reNumIndex]) ["gifts_dict"]);
          
		int i = 0;
        GComponent go = null;
		ComBigIcon asd;
		for(i=0;i<reData1.Count;i++)
		{
			go = Tools.GetComponent (Config.COM_BINDBIGICON).asCom;
			asd = go.GetChild ("n0") as ComBigIcon;
			go.touchable = false;
			foreach (string str in ((Dictionary<string,object>)reData1[i]).Keys) {

                int num = (int)( (Dictionary<string, object>)reData1[i] )[str];

                if (str == Config.ASSET_GOLD) {
					asd.SetData (str, num);
					_gold = _gold + num;
                    totalNum += num;
				} else if (str == Config.ASSET_COIN) {
					asd.SetData (str, num);
					_coin = _coin + num;
				} else {
					asd.SetData (str, num, 2);
                    CardVo card= DataManager.inst.GetCardVo(str);
                    if(card.rarity == 3) {//如果是彩卡
                        //Debug.Log("caika");
                        ClickBtn.touchable = false;
                        TimerManager.inst.Add(2.0f, 1, (float ff) => { ClickBtn.touchable = true; });
                    } else if(card.exp - num == 0&&card.lv==1) {//如果获得的第一张卡
                        //Debug.Log("xinka");
                        isNew = true;
                        ClickBtn.touchable = false;
                        TimerManager.inst.Add(2.0f, 1, (float ff) => { ClickBtn.touchable = true; });
                    } else { }
                    isLegend = card.rarity == 3 ? true : false;
				}
			}

			go.x = Tools.offectSetX (350);
			go.y = 370;
			go.scale = new Vector2 (0f, 0f);
			//
			bb [i] = go;
			this.AddChild (go);
			go.TweenScale (new Vector2 (0.8f, 0.8f), 0.5f);
//			go.scale = new Vector2 (0.8f, 0.8f);
//			go.x = Tools.offectSetX ((760 - reData1.Count * 80f) + (i * 160f));
//			go.y = 430f;

			if (i<(reData1.Count-1)){
				EffectManager.inst.Bezier (go, 0.5f, go.xy,  new Vector2 (300f, 0f),new Vector2 (Tools.offectSetX ((760 - reData1.Count * 80f) + (i * 160f)), 430f));
			}
		}
		EffectManager.inst.Bezier (go, 0.5f, go.xy, new Vector2 (300f, 0f), new Vector2 (Tools.offectSetX ((760 - reData1.Count * 80f) + (i * 160f)), 430f)).OnComplete (()=>{
			step1 = true;
			step2Clip = (len <= 0);
			GiftMove (0.5f,i-1);
		});
	}
	private void MoveToCard(int num,int toNum)
	{
		mNum = toNum;
		cha = Convert.ToInt32 (Math.Ceiling ((toNum - num) / 4f));
		TimerManager.inst.Add (0.4f, 1, TimerFunction2);
	}
	private void TimerFunction2(float ff)
	{
        try {
            MoveToCardNum(mNum);
        } catch(Exception e) { }
    }
	private void MoveToCardNum(int num)
	{
		ComProgressBar com = mpro as ComProgressBar;
		if (com == null)
			return;
		if (com.value >= num) {
			com.value = num;
			if (com.max <= com.value) {
				upImg.visible = true;
				EffectManager.inst.TweenJump (upImg, 1f);
			}
			return;
		}
		if (upImg != null && upImg.displayObject.gameObject!=null) {
			upImg.visible = false;
		}
		com.value = (com.value + cha);
		if (com.max <= com.value) {
			com.skin = ComProgressBar.BAR6;
		} else {
			com.skin = ComProgressBar.BAR3;
		}
		com.TweenScaleX (1.1f, 0.05f).OnComplete (() =>//card
		{
			com.TweenScaleX (1f, 0.05f).OnComplete (() =>
			{
				MoveToCardNum (num);
			});
		});
	}
	public void MoveToExp (int num,int toNum)
	{
		mNum = toNum;
		cha = Convert.ToInt32 (Math.Ceiling ((toNum - num) / 4f));
		TimerManager.inst.Add (0.4f, 1, TimerFunction);
	}
    private void TimerFunction(float ff) {      
        MoveToNum(mNum);  
    }
	private void MoveToNum(int num)
	{
		if (mpro == null || mprotxt == null)
			return;
		if (Convert.ToInt32 (mprotxt.text) >= num) {
			mprotxt.text = num.ToString();
			return;
		}
//		Log.debug (mprotxt.text + "====" + cha);
		mprotxt.text = (Convert.ToInt32 (mprotxt.text) + cha).ToString ();
		mpro.TweenScaleX (1.1f, 0.05f).OnComplete (() =>//gold
		{
			mpro.TweenScaleX (1f, 0.05f).OnComplete (() =>
			{
				MoveToNum (num);
			});
		});
	}
	private GGraph starImg;
	private GImage upImg;
	private void showItem (Dictionary<string,object> data,bool isKuai = false)
	{
		step2 = false;
		//
		TimerManager.inst.Remove (TimerFunction);
		//
		this.item = Tools.GetComponent (Config.RED_GIFT).asCom;

		(item.GetChild ("n8") as ComProgressBar).skin = ComProgressBar.BAR3;
		(item.GetChild ("n8") as ComProgressBar).offsetY = 5;
		(item.GetChild ("n9") as ComProgressBar).skin = ComProgressBar.BAR9;
		(item.GetChild ("n9") as ComProgressBar).offsetY = -3;
		item.GetChild ("n13").asTextField.text = Tools.GetMessageById ("24125");
		Controller c2 = item.GetController ("c2");

		this.AddChildAt (this.item, 3);
		item.x = Tools.offectSetX (580f);
		item.y = 200f;

		string _id = "";
		string _num = "";
		foreach (string id in data.Keys) {
			_id = id;
			_num = data [id].ToString ();
		}
		GTextField name = item.GetChild ("n1").asTextField;
		GTextField info = item.GetChild ("n2").asTextField;
		GTextField num = item.GetChild ("n3").asTextField;
		starImg = item.GetChild ("n5").asGraph;

        //		TimerManager.inst.Add (0.01f, 0,OnStarImgRatation);
        starImg.TweenScale (new Vector2 (1.2f, 1.2f), 0.2f);
        if(isLegend) {
            EffectManager.inst.AddPrefab("Legendcard/legendcard", starImg);
            //TimerManager.inst.Remove(eff);
        } else {
            GameObject go = EffectManager.inst.AddPrefab("Normalcard/normalcard", starImg);
            //go.transform.localScale*=1.2f;
            go.transform.localPosition=( new Vector3( 15,0,0));
        }
        name.text = _id;

		if (_id.StartsWith ("C")&&_id != Config.ASSET_CARD) {
			num.text = "x" + _num;
			CardVo ava = DataManager.inst.GetCardVo (_id);
			name.text = Tools.GetMessageById (ava.name);
			info.text = CardVo.GetRarityMss (ava.rarity,1);
			c2.selectedIndex = 2;
			ComProgressBar pro = item.GetChild ("n8") as ComProgressBar;
			mpro = pro;
			pro.max = ava.maxExp;

			if (ava.lv == 1 && ava.exp == Convert.ToInt32 (_num)&&isNew) {
				item.GetChild ("n13").asTextField.visible = true;
                isNew = false;
			}
			upImg = item.GetChild ("n11").asImage;
			if (isKuai) {
				pro.value = ava.exp;
				if (pro.max <= pro.value) {
					upImg.visible = true;
					pro.skin = ComProgressBar.BAR6;
				} else {
					upImg.visible = false;
					pro.skin = ComProgressBar.BAR3;
				}
			} else {
				pro.value = ava.exp - Convert.ToInt32 (_num);
				if (pro.max <= pro.value) {
					upImg.visible = true;
					pro.skin = ComProgressBar.BAR6;
				} else {
					upImg.visible = false;
					pro.skin = ComProgressBar.BAR3;
				}
				if(ava.lv == ava.maxLv&&ava.exp == ava.maxExp)
				{
					pro.value = ava.exp;
				}else{
					MoveToCard (Convert.ToInt32 (pro.value), ava.exp);
				}

			}

		} else if(_id.StartsWith("s")&&_id != Config.ASSET_ELSCORE&&_id != Config.ASSET_RANKSCORE)
		{
			info.text = Tools.GetMessageById ("24110");
			num.text = "x" + _num;
		}
		else
		{
			if (_id == Config.ASSET_GOLD) {
				c2.selectedIndex = 0;
				item.GetChild ("n6").asCom.GetChild ("n0").asTextField.text = (_gold - Convert.ToInt32 (_num)).ToString ();
				mpro = item.GetChild ("n6").asCom;
				mprotxt = item.GetChild ("n6").asCom.GetChild ("n0").asTextField;
				if (!isKuai) {
					MoveToExp (_gold - Convert.ToInt32 (_num), _gold);
				} else {
					mprotxt.text = _gold.ToString ();
				}
			} else if(_id == Config.ASSET_COIN) {
				c2.selectedIndex = 1;
				item.GetChild ("n7").asCom.GetChild ("n0").asTextField.text = (_coin - Convert.ToInt32 (_num)).ToString ();
				mpro = item.GetChild ("n7").asCom;
				mprotxt = item.GetChild ("n7").asCom.GetChild ("n0").asTextField;
				if (!isKuai) {
					MoveToExp (_coin - Convert.ToInt32 (_num), _coin);
				} else {
					mprotxt.text = _coin.ToString ();
				}
			}else{
				c2.selectedIndex = 2;
			}
			name.text = Tools.GetIconName (_id);
			num.text = "x" + _num;
			info.text = Tools.GetMessageById ("21013");
			Dictionary<string,object> ddd = new Dictionary<string,object> ();
			ddd.Add (_id, _num);
		}
	}
	private void OnStarImgRatation(float fa)
	{
		starImg.rotation += 1f;
	}
	public List<object> DataChuLione(Dictionary<string,object> data)
	{
		List<object> list = new List<object> ();
		foreach (string s in data.Keys) {
			if (s == Config.ASSET_CARD) {
				foreach (string ss in ((Dictionary<string,object>)data[Config.ASSET_CARD]).Keys) {
					Dictionary<string,object> da = new Dictionary<string, object> ();
					da.Add (ss, ((Dictionary<string,object>)data [Config.ASSET_CARD]) [ss]);
					list.Add (da);
				}
			} else {
				Dictionary<string,object> da = new Dictionary<string, object> ();
				da.Add (s, data [s]);
				list.Add (da);
			}
		}
		return list;
	}
	public List<object> DataChuLi(object[] data)
	{
		reNum = new int[data.Length];
		List<object> list = new List<object> ();
		for(int i = (data.GetLength (0) - 1);i>=0;i--)
		{
			int index = 0;
			Dictionary<string,object> js = (Dictionary<string,object>)data [i];
			js = (Dictionary<string,object>)js ["gifts_dict"];
			foreach (string s in js.Keys) {
				if (s == Config.ASSET_CARD) {
					foreach (string ss in ((Dictionary<string,object>)js[Config.ASSET_CARD]).Keys) {
						Dictionary<string,object> da = new Dictionary<string, object> ();
						da.Add (ss, ((Dictionary<string,object>)js [Config.ASSET_CARD]) [ss]);
						list.Add (da);
						index++;
					}
				} else {
					Dictionary<string,object> da = new Dictionary<string, object> ();
					da.Add (s, js [s]);
					list.Add (da);
					index++;
				}
			}
			reNum[i] = index;
		}
		return list;
	}

	public override void Clear ()
	{
//		ViewManager.inst.s.visible = true;
		ViewManager.inst.s.displayObject.gameObject.SetActive (true);
		if (this.item != null) {
			this.RemoveChild (this.item,true);
			this.item = null;
		}
		TimerManager.inst.Remove (TimerFunction);
		if (mpro != null) {
			DOTween.Kill (mpro);
		}
		mpro = null;
		mprotxt = null;
		base.Clear ();
		this.DispatchGlobalEvent(new MainEvent (MainEvent.RED_GIFT));
        if(ModelManager.inst.userModel.gold-gold!=totalNum)
            ViewManager.inst.ShowText(Tools.GetMessageById("33212"));
    }
}
