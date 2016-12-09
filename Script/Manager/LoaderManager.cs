using System;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Collections;

public class LoaderManager
{
	//	public static string PATH_WIN = "file://" + Application.dataPath + "/StreamingAssets/";
	//	public static string PATH_AND = "jar:file://" + Application.dataPath + "!/assets/";
	//	public static string PATH_IOS = Application.dataPath + "/Raw/";

//	public delegate void LoadEvent (object w);

	private static LoaderManager instance;
	public Dictionary<string,Texture2D> source;
	public Dictionary<string,object> sourceDic;

	//图集资源
	public TextureMap tm;

	public LoaderManager ()
	{
		source = new Dictionary<string, Texture2D> ();
		sourceDic = new Dictionary<string, object> ();
	}

	public static LoaderManager inst
	{
		get
		{
			if (instance == null)
				instance = new LoaderManager ();
			return instance;
		}
	}

	//texturemap 获取texture2d
	public Texture2D Load (string path)
	{
		return tm.GetTexture (path);
	}

	public T Load<T> (string name) where T :UnityEngine.Object
	{
		return Resources.Load<T> (name);
		//GameObject
		//Texture2D
		//AudioClip
		//Material
	}

	public Loads Load (string url, Action<object> fun, Action<object> error = null,GLoader loader = null)
	{
		Loads lw = null;
		if (url == "" || url == null)
			return lw;		
		if (loader != null && loader.image.isDisposed) {
			return lw;
		}
		if (source.ContainsKey (url) && loader==null)
		{
			if (fun != null) {
				if (url.LastIndexOf (".png") > -1 || url.LastIndexOf (".jpg") > -1 || url.LastIndexOf (".jpeg") > -1 || loader != null) {
					fun ((Dictionary<string,object>)sourceDic [url]);
				} else {
					fun (source [url]);
				}
			}
			return lw;
		}
		lw = Loads.Get ();
		lw.LoadUrl (url, (object w) =>
		{
			Loads loadW;
			//Log.debug ("Load Complete - " + url);
			loadW = w as Loads;
			bool isDis = false;
			if (loadW.url.LastIndexOf (".png") > -1 || loadW.url.LastIndexOf (".jpg") > -1 || loadW.url.LastIndexOf (".jpeg") > -1 || loadW.loader!=null)
			{

				Texture2D t = loadW.www.texture;
				source [url] = t;
				isDis = loadW.www == null ? true : false;
				
				if (fun != null && !isDis){
					if(loadW.loader!=null){
						Dictionary<string,object> wwdic = new Dictionary<string, object>();
						wwdic.Add("url",loadW.url);
						wwdic.Add("loader",loadW.loader);
						wwdic.Add("texture",t.EncodeToJPG());
//							if(loadW.loader!=null){
//								loadW.loader.texture = new NTexture(PhoneManager.inst.Base64ToTexter2d((string)wwdic["texture"], 100, 100));
//							}
						sourceDic[url] = wwdic;
						fun (wwdic);
					}
					else{
						fun (source [url]);
					}
				}
				loadW.Clear ();
			}
			else
			{
				loadW = w as Loads;		
				isDis = loadW.www == null ? true : false;
				if (fun != null && !isDis)
					fun (loadW.www);
				loadW.Clear ();
			}
		}, (object xx) =>
		{
			if (error != null)
				error (null);
		},loader);
		return lw;
	}
}

public class Loads:MonoBehaviour
{
	public WWW www;
	public Action<object> fun;
	public Action<object> error;
	public string url;
	public GLoader loader; 

	public Loads ()
	{
	}

	public static Loads Get ()
	{
		GameObject go = new GameObject ("LoadWww"+Guid.NewGuid().ToString("N"));
		return go.AddComponent<Loads> ();
	}

	public void LoadUrl (string url, Action<object> fun, Action<object> error,GLoader loader=null)
	{
		this.fun = fun;
		this.error = error;
		this.url = url;
		this.loader = loader;
		this.StartCoroutine (LoadStart ());
	}

	private IEnumerator LoadStart ()
	{
		www = new WWW (this.url);
		yield return www;
		if (www.error == null)
		{
			if (www.isDone)
			{
				if (this.fun != null)
					this.fun (this);
				else
					Clear ();
			}
			else
			{
				Clear ();
			}
		}
		else
		{			
//			Log.debug ("Load Url - " + url + " : " + www.error);
			if (error != null)
				this.error (this);
			Clear ();
		}
	}

	public void Clear ()
	{
		this.StopCoroutine (LoadStart ());
		Tools.Clear (this.gameObject);
		if (www != null)
			www.Dispose ();
		www = null;
 
	}
}