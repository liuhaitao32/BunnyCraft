using UnityEngine;
using System.Collections;

public class AvatarAnimator : MonoBehaviour {
	Animator animator;
	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();

		animator.SetFloat ("stand", 2f);
		// Set all animations to loop 设置所有动画为循环

//		animatior.wrapMode = WrapMode.Loop;

		// except shooting 除了射击（不循环）

//		animatior["shoot"].wrapMode = WrapMode.Once;



		//放置idle 和 walk 进低一级别的 layers  (默认 layer 总是 0)

		// This will do two things这将作两件事情

		// - 当 calling CrossFade时，由于shoot 和 idle/walk 在不同的layers 中

		//   它们将不会影响互相之间的重放.

		// - 由于 shoot 在高一级的 layer, 当faded in 时shoot动画将替换

		//   idle/walk 动画 .

//		animatior["shoot"].layer = 1;



		// Stop animations that are already playing停止已经播放的动画

		//(万一 user 忘记的话，自动disable播放)

//		animatior.Stop();

	}



	void Update () {

		// Based on the key that is pressed,基于按下的键

		// play the walk animation or the idle animation播放走，站动画

//		if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1)
//
//			animation.CrossFade("walk");
//
//		else
//
//			animation.CrossFade("idle");
//
//
//
//		// Shoot射击
//
//		if (Input.GetButtonDown ("Fire1"))
//
//			animation.CrossFade("shoot");

	}
}
