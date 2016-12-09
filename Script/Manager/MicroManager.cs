using System;
using UnityEngine;
using System.Collections;
using System.IO;
using FairyGUI;
using MoPhoGames.USpeak.Core;
public class MicroManager
{
	private static MicroManager instance;

	private GameObject microGo;
	private Micro mi;

	//	private AudioClip clipSpeek;
	//	private static int Num_lis = 0;

	public MicroManager ()
	{
	}

	public static MicroManager inst
	{
		get
		{
			if (instance == null)
				instance = new MicroManager ();
			return instance;
		}
	}

	public void Register (GameObject stage)
	{
		microGo = new GameObject ();
		microGo.name = "Micro";
//		microGo.AddComponent<AudioSource> ();
//		microGo.AddComponent<Micro> ();
		microGo.transform.SetParent (stage.transform);
	}

	//	public GameObject CreatLis ()
	//	{
	//		GameObject go = new GameObject ();
	//		go.name = "Micro" + Num_lis;
	//		go.AddComponent<AudioSource> ();
	//		go.AddComponent<Micro> ();
	//		return go;
	//	}

	public Micro Start (Action<byte[]> fun, Action<int> voice = null,GButton btn = null)
	{
//		mi = microGo.GetComponent<Micro> ();//Micro.Get (true);
		mi = Micro.Get ();
		mi.Load (fun, voice,btn);
		return mi;
	}

	public void Close ()
	{
		if (mi != null)
		{
			mi.Clear ();		
			mi = null;
		}
	}

	public Micro Play (byte[] data, string id, Action<string> end)
	{
		if (data == null || data.Length == 0)
			return null;
		Micro m = Micro.Get ();
		m.SetData (data, id, end);
		return m;
	}
}

public class Micro:MonoBehaviour
{
	private AudioSource ac;
	private AudioClip clip;
	//	private GameObject self;
	private int recordTime = 5;
	private const int frequency = 4000;
	private Action<byte[]> onMicro;
	private Action<int> onVoice;
	private Action<string> onEnd;
	private string id;
	private bool isClear = false;
	private GButton btnTxt = null;
	private Action<float> clearTimer;
	public Micro ()
	{
	}

	public static Micro Get ()
	{
		GameObject go = new GameObject ("Micro" + Tools.GetRandom ());
		return go.AddComponent<Micro> ();
	}

	public void Load (Action<byte[]> fun, Action<int> voice,GButton btn = null)
	{
		onMicro = fun;
		onVoice = voice;
		btnTxt = btn;
		if (Microphone.devices.Length == 0)
		{
			ViewManager.inst.ShowText (Tools.GetMessageById ("14508"));
			this.onMicro (null);
			this.Clear ();
			return;
		}

		ac = this.gameObject.AddComponent<AudioSource> ();
		ac.Stop ();
		ac.mute = true;
		ac.clip = Microphone.Start (null, false, recordTime, frequency);
		ac.Play ();
		this.StartCoroutine (TimeDown ());
		TimerManager.inst.AddUpdate (Check_Voice);
	}
	int time = 0;
	private IEnumerator TimeDown ()
	{
		while (time < recordTime)
		{  			
			if (!Microphone.IsRecording (null))
			{
				yield break;  
			}
			yield return new WaitForSeconds (1);  
			time++; 
			SetTimeTxt (recordTime - time);
		}
		if (time >= recordTime)
		{ 
			time = recordTime;
			SetTimeTxt (0);
			Over();
		}
		yield return null;
	}
	private void SetTimeTxt(int txt){
		
		if (btnTxt != null) {
			btnTxt.text = txt + "";
		}
	}
	public void Over(){
		Microphone.End (null);
		this.onMicro (GetData ());
		this.Clear ();
		TimerManager.inst.Remove (Check_Voice);
	}
	private void Check_Voice (float time)
	{
		if (!Microphone.IsRecording (null))
			return;
		if (onVoice != null)
			onVoice (GetVoice ());
	}

	private int GetVoice ()
	{
		if (ac == null || ac.clip == null)
			return 0;				
		float voice = 0f;
		float[] samples = new float[128];
		int start = Microphone.GetPosition (null) - 128;
		if (start < 128)
			return 0;
		ac.clip.GetData (samples, start);
		for (int i = 0; i < samples.Length; i++)
		{
			if (voice < samples [i])
				voice = samples [i];
		}
		return (int)(voice * 100);
	}

	private byte[] GetData ()
	{
		if (ac.clip == null)
			return null;
		ac.Stop ();
//		float[] samples = new float[ac.clip.samples * ac.clip.channels];
//		ac.clip.GetData (samples, 0);
//		byte[] bs = new byte[samples.Length * 4];
//
//		byte[] every;
//		for (var i = 0; i < samples.Length; i++)
//		{
//			every = BitConverter.GetBytes (samples [i]);
//			for (var j = 0; j < every.Length; j++)
//			{
//				bs [i * 4 + j] = every [j];
//			}
//		}
////		Log.debug ("Zip1  - " + bs.Length);
//		byte[] outs = Tools.Compress (bs);
////		Log.debug ("Zip2 - " + outs.Length);
//		return outs;
		int s;

		byte[] b = USpeakAudioClipCompressor.CompressAudioClip (ac.clip, out s, 1.0f);
		return b;
	}

	public void SetData (byte[] data, string id, Action<string> end)
	{
		this.id = id;
		onEnd = end;
		ac = this.gameObject.AddComponent<AudioSource> ();
		if (data == null)
		{
			this.Clear ();
			return;
		}
//		byte[] bs = Tools.Decompress (data);
//		float[] da = new float[bs.Length / 4];
//		for (var i = 0; i < da.Length; i++)
//		{
//			da [i] = BitConverter.ToSingle (bs, i * 4);
//		}
//		ac.clip = AudioClip.Create ("clip", da.Length, 1, frequency, false);
//		ac.clip.SetData (da, 0);
		ac.clip = USpeakAudioClipCompressor.DecompressAudioClip (data, 0, 1, false, 1.0f);
		ac.mute = false;
		ac.Play ();
		//倒计时 清理语音
		clearTimer = TimerManager.inst.Add (ac.clip.length + 1, 1, (float time) => {
			if (isClear)
				return;
			this.onEnd (this.id);
			this.Clear ();
		});
	}

	public void Clear ()
	{
		isClear = true;
		if (ac != null) {
			ac.Stop ();
		}
		this.StopCoroutine (TimeDown ());
		TimerManager.inst.Remove (Check_Voice);
		TimerManager.inst.Remove (clearTimer);
		Tools.Clear (this.gameObject);
	}
}