using System;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class SoundManager
{
	private static SoundManager instance;

	private GameObject musicGo;
	private GameObject soundGo;
	private AudioSource music;
	private AudioSource sound;
	private AudioClip ac;
	private Dictionary<string,AudioClip> list;

	private string lastMusic;
	public bool isMusic;
	public bool isSound;
	private AudioListener mainCam;
	public SoundManager ()
	{
		list = new Dictionary<string, AudioClip> ();
	}

	public static SoundManager inst
	{
		get
		{
			if (instance == null)
				instance = new SoundManager ();
			return instance;
		}
	}

	public void Register (GameObject stage)
	{		
		string[] names = Config.SOUND_BTN.Split (':');
		AudioClip ac = (AudioClip)UIPackage.GetItemAsset (names [0], names [1]);
		list.Add (Config.SOUND_BTN, ac);
		UIConfig.buttonSound = ac;

		string m = LocalStore.GetLocal (LocalStore.LOCAL_MUSIC);
		string s = LocalStore.GetLocal (LocalStore.LOCAL_SOUND);
		this.isMusic = m == "" ? true : Convert.ToBoolean (m);
		this.isSound = s == "" ? true : Convert.ToBoolean (s);

		mainCam = GameObject.Find ("Main Camera").GetComponent<AudioListener>();
		musicGo = new GameObject ();
		musicGo.name = "Music";
		musicGo.transform.SetParent (stage.transform);
		music = musicGo.AddComponent<AudioSource> ();

		soundGo = new GameObject ();
		soundGo.name = "Sound";
		soundGo.transform.SetParent (stage.transform);
		sound = soundGo.AddComponent<AudioSource> ();

		music.loop = true;
		sound.loop = false;

//		stage.AddComponent<AudioListener> ();
		SetSound(this.isSound);
	}
	public void SetMusic2(bool value1,bool value2){
		musicGo.SetActive (value1);
		mainCam.enabled = value2;
	}
	public void SetMusic (bool value)
	{
		this.isMusic = value;
		LocalStore.SetLocal (LocalStore.LOCAL_MUSIC, value.ToString ());
		if (music != null)
		{
			if (this.isMusic)
			{
				if (!music.isPlaying)
					music.Play ();
			}
			else
			{
				music.Stop ();
			}
		}					
	}

	public void SetSound (bool value)
	{
		this.isSound = value;
		LocalStore.SetLocal (LocalStore.LOCAL_SOUND, value.ToString ());
		if (sound != null)
		{
			if (this.isSound)
			{
//				if (!sound.isPlaying)
//					sound.Play ();
			}
			else
			{
				sound.Stop ();
			}
		}
		if (this.isSound)
			Stage.inst.EnableSound ();
		else
			Stage.inst.DisableSound ();
	}

	public void PlayMusic (string name)
	{
		if (!this.isMusic)
			return;
		if (lastMusic == name)
			return;
		AudioClip ac;
		if (list.ContainsKey (name))
		{
			ac = list [name];
		}
		else
		{
			string[] names = name.Split (':');
			ac = (AudioClip)UIPackage.GetItemAsset (names [0], names [1]);
			list [name] = ac;
		}
		lastMusic = name;
		this.ac = ac;
		TimerManager.inst.Add (0.05f, 0, Time_Tick1);
	}

	private void Time_Tick1 (float time)
	{		
		if (music.volume <= 0f)
		{			
			TimerManager.inst.Remove (Time_Tick1);
			music.loop = true;
			music.Stop ();
			music.clip = (AudioClip)this.ac;
			music.volume = 0;
			music.Play ();
//			Log.debug ("music len - " + music.clip.length);

			TimerManager.inst.Add (0.05f, 0, Time_Tick2);
			return;
		}
		music.volume = music.volume - 0.02f;
	}

	private void Time_Tick2 (float time)
	{		
		if (music.volume >= 1f)
		{
			TimerManager.inst.Remove (Time_Tick2);
			return;
		}
		music.volume = music.volume + 0.02f;
	}

	public void PlaySound (string name)
	{
		if (!this.isSound)
			return;
		AudioClip ac;
		if (list.ContainsKey (name))
		{
			ac = list [name];
		}
		else
		{
			string[] names = name.Split (':');
			ac = (AudioClip)UIPackage.GetItemAsset (names [0], names [1]);
			list [name] = ac;
		}
		sound.loop = false;
		sound.clip = ac;
		sound.Play ();
	}
}