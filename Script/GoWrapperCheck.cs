using UnityEngine;
using System.Collections;
using FairyGUI;

public class GoWrapperCheck : MonoBehaviour {

	// Use this for initialization
	public GameObject go;
	public GObject item;
	public Vector2 rV2;
	public float left;
	public float right;
	public float p;
	public float xx;
	public float scale;
	public void setItem(GObject obj,float l,float r,GameObject g,float ss){
		scale = ss;
		left = l*scale;
		right = r*scale;
		item = obj;
		go = g;


	}
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (item != null && go!=null) {
//			rV2 = item.LocalToRoot (new Vector2 (item.x, item.y), item.root);
			rV2 = item.LocalToGlobal (new Vector2 (item.x, item.y));
//			Debug.LogError (item.x);
//			Debug.LogError (rV2.x);
			xx = rV2.x-item.x*scale;
			if (xx < left || xx > right) {
				if (go.activeSelf) {
					go.SetActive (false);
				}
			} else {
				if (!go.activeSelf) {
					go.SetActive (true);
				}
			}
		}
	}
}
