using UnityEngine;
using System.Collections;

/// 音乐控制，并跟随玩家位置
/// author Biggo
public class FightAudioController : MonoBehaviour {
    public AudioSource audioSourceBGM;
	///停止播放
	public void StopBGM () {
		this.audioSourceBGM.enabled = false;
	}
	///开始 继续播放
	public void PlayBGM (bool isLastTime = false) {
        this.audioSourceBGM.pitch = isLastTime ? 1.4f : 1f;
        this.audioSourceBGM.enabled = true;
	}

	///静音
	public void Mute () {
		AudioListener.volume = 0;
	}
	///取消静音
	public void UnMute () {
		AudioListener.volume = 1;
	}
	private void Update() {
		if (null != FightMain.instance.selection && null != FightMain.instance.selection.localPlayer) {
            this.transform.position = FightMain.instance.selection.localPlayer.view.transform.position;
		}
	}
}
