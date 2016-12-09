using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using FairyGUI;

public class Load :MonoBehaviour
{
	private GComponent view;
	private GProgressBar bar;
    private GLoader BG;

	private bool isLoad = false;
	private bool isLoadNum = false;
	private bool isGameing = false;
	private long time;
	private float cur = 10;
	public int LoadFiles = 0;
	public List<string> fileNames = new List<string> ();
	void Awake ()
	{
		GRoot.inst.SetContentScaleFactor (960, 640);
	}

	void Start ()
	{
		Application.targetFrameRate = 30;
		Screen.orientation = ScreenOrientation.Landscape;
//		Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		//
		StageCamera.main.nearClipPlane = -20;
		StageCamera.main.farClipPlane = 50;
		StageCamera.main.transform.position = new Vector3 (StageCamera.main.transform.position.x, StageCamera.main.transform.position.y, -30);
		//
//		Screen.fullScreen = true;
		time = DateTime.Now.Ticks;
		view = this.gameObject.GetComponent<UIPanel> ().ui;
//		Log.debug (Stage.inst.scale.ToString () + "|" + GRoot.inst.scale.ToString ());
//		view.SetPivot (0.5f, 0.5f, true);
		GRoot.inst.fairyBatching = true;
		view.width = view.root.width;
		view.height = view.root.height;
        /*
		GImage bg = view.GetChild ("n0").asImage;
		bg.height = view.height;
		ViewManager.bgOffsetW = bg.width * (bg.height / 640);
		ViewManager.bgOffsetX = -(ViewManager.bgOffsetW - view.width) / 2;
		bg.width = ViewManager.bgOffsetW;
		bg.x = ViewManager.bgOffsetX;
        */
        int num = Tools.GetRandom(0, 4);
        BG = view.GetChild("n4").asLoader;
        BG.url = Tools.GetResourceUrl("Load:qidongye"+num.ToString());
        ViewManager.bgOffsetW = BG.width * ( BG.height / 640 );
        ViewManager.bgOffsetX = -( ViewManager.bgOffsetW - view.width ) / 2;
        BG.height = view.height;
        BG.width = ViewManager.bgOffsetW;
        BG.x = ViewManager.bgOffsetX;
		//
		bar = view.GetChild ("n1").asProgress;
       	//
        this.StartCoroutine (LoadResource ());
		//
//		UIPackage.AddPackage("Asset/Flash", (string name, string extension, System.Type type) => { 
//			Debug.LogError(name + extension);
//			return Resources.Load(name, type); });
	}

	public void Clear ()
	{
		view.Dispose ();
		Tools.Clear (this.gameObject);
	}
	void Update ()
	{
		if (isGameing) {
			return;
		}
		if (isLoad)
		{
//			isLoad = false;
			StopCoroutine (LoadResource ());
//			bar.value = 98;
//			Log.debug ("Load Texture " + (DateTime.Now.Ticks - time) / 10000 + "ms");

			//进入
			if (isLoadNum) {
				isGameing = true;
				Main.loadView = this;
//				Debug.LogError("--------");
				GameObject.Find ("Stage").AddComponent<Main> ();
			}
		}
//		else
//		{
//			if (bar.value == 98)
//				return;
			if(isLoadNum){
				return;
			}
			
			cur += 2f;
			bar.value = cur;
            if(cur > 97) {
                bar.GetChild("n4").visible = false;
            }
            if (cur >= 100){
				cur = 100;
//				bar.value = cur;
				//bar.GetChild ("n4").visible = false;
				
				isLoadNum = true;	
			}
//		}
	}

	private IEnumerator LoadResource ()
	{	
		UIPackage.AddPackage ("Asset/Flash");	
		UIPackage.AddPackage ("Asset/Base");
		UIPackage.AddPackage ("Asset/Image");
		UIPackage.AddPackage ("Asset/Image2");
		UIPackage.AddPackage ("Asset/Font");
		UIPackage.AddPackage ("Asset/Icon");
		UIPackage.AddPackage ("Asset/ViewMain");
		UIPackage.AddPackage ("Asset/ViewChat");
		UIPackage.AddPackage ("Asset/ViewRedPackage"); 
		UIPackage.AddPackage ("Asset/ViewRoleInfo");
		UIPackage.AddPackage ("Asset/ViewFriend");
		UIPackage.AddPackage ("Asset/ViewExplore");
		UIPackage.AddPackage ("Asset/ViewShop");
		UIPackage.AddPackage ("Asset/ViewGuild");
		UIPackage.AddPackage ("Asset/ViewShip");
		UIPackage.AddPackage ("Asset/ViewMail");
		UIPackage.AddPackage ("Asset/ViewActivation");
		UIPackage.AddPackage ("Asset/ViewRanking");
		UIPackage.AddPackage ("Asset/ViewEffort");
		UIPackage.AddPackage ("Asset/ViewFight");
		UIPackage.AddPackage ("Asset/ViewStatement");
		TextAsset text = Resources.Load ("Embed/filterword") as TextAsset;
		FilterManager.inst.Decode (text.text);
//		LoaderManager.inst.tm = Resources.Load<GameObject> ("Image/texture").GetComponent<TextureMap> ();
		isLoad = true;
		yield return null;
	}
}