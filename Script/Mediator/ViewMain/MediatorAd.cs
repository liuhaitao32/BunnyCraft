using System;
using FairyGUI;
using UnityEngine;

public class MediatorAd:BaseMediator
{
	public MediatorAd ()
	{
	}
	public static string typeAd = "";
	private GTextField killAll;
	private GTextField killPass;
	private GTextField hertAll;
	private GTextField score;
	private GTextField name;
	private GLoader QRcode;
	private GImage fightData;
	private GImage ad;
	private string shareUrl;
	private ModelUser userModel;
	private int left;
	private int top;
	Camera camera;
	ModelShare shareModel;
	ModelFight fightModel;
	private bool isFight;
	public override void Init ()
	{
		GRoot.inst.touchable = false;
		base.Init ();
		this.Create (Config.VIEW_AD);
		//
		fightData = this.GetChild("n0").asImage;
		ad = this.GetChild("n10").asImage;
		isFight = (typeAd == ModelShare.SHARE_FIGHT);
		ad.visible = !isFight;
		//
		killAll = this.GetChild("n3").asTextField;
		killPass = this.GetChild("n4").asTextField;
		hertAll = this.GetChild("n5").asTextField;
		score = this.GetChild("n6").asTextField;
		name = this.GetChild("n7").asTextField;
		QRcode = this.GetChild ("n8").asLoader;
		//
		//

		GameObject cutCamera = new GameObject ();
		cutCamera.name = "cutCamera";
		Vector3 mCa = StageCamera.main.transform.localPosition;
		camera = cutCamera.AddComponent<Camera> ();
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.farClipPlane = StageCamera.main.farClipPlane;
		camera.nearClipPlane = StageCamera.main.nearClipPlane;
		camera.orthographic = true;
		camera.depth = StageCamera.main.depth-1;
		camera.orthographicSize = StageCamera.main.orthographicSize;
		camera.backgroundColor = Color.white;
		camera.transform.localPosition = new Vector3 (-mCa.x, mCa.y,0);
		camera.useOcclusionCulling = false;
		//
		float nw = (this.width / this.height) * GRoot.inst.height;
		this.height = GRoot.inst.height;
		this.width = nw;
		//
		this.group.x = -(GRoot.inst.width*0.5f)-(this.width*0.5f);
		this.group.y = 0;
//		Debug.LogError(":: >> "+this.view.width+" | "+this.view.actualWidth+" | "+this.view.viewWidth + " | "+GRoot.inst.scale.y.ToString());
		//
		userModel = ModelManager.inst.userModel;
		shareModel = ModelManager.inst.shareModel;
		fightModel = ModelManager.inst.fightModel;
		//
		if(isFight){
			object[] arr = (object[])fightModel.fightData["my_data"];
			bool isNotCustom = false;
			if (arr [0] is object[]) {
				isNotCustom = true;
			}
//			Debug.Log (isNotCustom);
			if (isNotCustom) {
				score.text = ((object[])arr [0]) [0] + "";
				killAll.text = ((object[])arr [1]) [0] + "";
				killPass.text = ((object[])arr [2]) [0] + "";
				hertAll.text = ((object[])arr [3]) [0] + "";
			} else {
				score.text = (arr [0]) + "";
				killAll.text = (arr [1]) + "";
				killPass.text = (arr [2]) + "";
				hertAll.text = (arr [3]) + "";
			}
			name.text = userModel.GetUName ();
		}
		//
		shareUrl = DataManager.inst.systemSimple["share_url"] + userModel.uid;
		//
		string qr = LocalStore.GetLocal (LocalStore.LOCAL_QRCODE + userModel.uid);
		if (qr != "" && qr != null && qr.Length > 0) {
			QRcode.texture = new NTexture(PhoneManager.inst.Base64ToTexter2d (qr,150,150));
			CutBitmap ();
		} else {
			LoaderManager.inst.Load(shareUrl + ".jpg", (object w) => {
				
				if(this.group == null || this.parent == null){
					GRoot.inst.touchable = true;
					return;
				}
				Texture2D topImage = (Texture2D)w;
				if (w != null && w != null && QRcode!=null)
				{
					if (this.group == null) return;
					QRcode.texture = new NTexture(topImage);
					//				Log.debug("topImage:"+topImage);
					//				Log.debug("erBg"+erBg);
					LocalStore.SetLocal(LocalStore.LOCAL_QRCODE+userModel.uid, Convert.ToBase64String(topImage.EncodeToJPG()));
					//				share_bitmap = ComposeImage(baseImage, topImage);
					CutBitmap();
				}
			},(object error)=>{
				GRoot.inst.touchable = true;
			});
		}
	}

	private void CutBitmap(){

		//
		TimerManager.inst.Add(0.2f,1,(float t) => {
//		Timers.inst.CallLater ((object t) => {
			GRoot.inst.touchable = true;
			if (this.group == null) return;
			//
//			Debug.LogError(GRoot.inst.width +" &| "+GRoot.inst.actualWidth+" | "+GRoot.inst.viewHeight+" | "+GRoot.inst.initHeight+" | "+GRoot.inst.sourceHeight +" | "+GRoot.inst.scale);
//			Debug.LogError(Screen.width +" | "+Screen.height+" | ");
			Rect rect = new Rect (0,0,640, 960);
//			Debug.LogError(Screen.width +" | "+Screen.height + "|"+GRoot.inst.scale + " | "+(w)+" | "+h);
			//
			Texture2D iconImage = Resources.Load<Texture> ("Embed/share_icon") as Texture2D;
			//		UnityEngine.UI.Image img = GameObject.Find ("ImageTest").GetComponent<UnityEngine.UI.Image>();
			//		img.sprite = UnityEngine.Sprite.Create (iconImage, new Rect (0, 0, iconImage.width, iconImage.height), new Vector2 (0.5f, 0.5f));
			shareModel.shareImageIcon = Convert.ToBase64String (iconImage.EncodeToJPG());

			Texture2D png = PhoneManager.inst.CaptureCamera ("test", camera, rect,rect);
//
			string pngStr = System.Convert.ToBase64String(png.EncodeToJPG());
            //shareModel.SetWeiBoData("204151547", pngStr, "https://api.weibo.com/oauth2/default.html", "微博测试内容 http://news.qq.com", "http://www.meng52.com");
            //shareModel.SetWeiXinData("wx2e794e0b84371812", pngStr);
			shareModel.SetData(pngStr,MediatorAd.typeAd);
			ViewManager.inst.CloseView();
//			ViewManager.inst.ShowView<MediatorShareBtn>(true,false);
//
//			MediatorMain.testTexture = pngStr;
//			if (PlatForm.inst.pf != PlatForm.EX_LOCAL)
//				PlatForm.inst.GetSdk().Call(Ex_Local.CALL_WECHARIMG, new string[] { "wx2e794e0b84371812", pngStr, "0" }, null);

		});
	}
	public override void Clear ()
	{
		GRoot.inst.touchable = true;
		base.Clear ();
        if (QRcode != null)
            QRcode = null;
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.SHARE_DATA_EVENT, null));

    }
}