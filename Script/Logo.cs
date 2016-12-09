using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo:MonoBehaviour
{
	private Animator a;
	private GameObject fl;
	public GameObject ani;

	void Start ()
	{
//		this.Invoke ("Show", 1f);
		Show();
	}

	private void Show ()
	{
		fl = GameObject.Instantiate<GameObject> (ani);
		fl.transform.SetParent (this.gameObject.transform);
//		a = fl.GetComponent<Animator> ();
//		int dan = Tools.GetRandom (0, 10);
//		if (dan < 5)
//			a.Play ("00");
//		else
//			a.Play ("01");

		this.Invoke ("Load", 4f); 
	}

	private void Load ()
	{
		ViewManager.LoadScene ("Main", (float t) =>
		{
//			ViewManager.RemoveScene ("Logo");
			},UnityEngine.SceneManagement.LoadSceneMode.Single);
	}
}