using UnityEngine;
using System.Collections;

/// 进入场景时缩放显示
public class StartTween : MonoBehaviour {

	///缩放起始值
	public float scaleStart = 0.1f;
	///缩放终点值
	public float scaleEnd = 1f;
	///缩放时间
	public float scalescaleime = 0.3f;

	void Start () {
		this.transform.localScale = new Vector3(scaleStart,scaleStart,scaleStart);

		LTDescr ltd = LeanTween.scale (this.gameObject, new Vector3 (scaleEnd, scaleEnd, scaleEnd), scalescaleime);
		ltd.tweenType = LeanTweenType.easeOutBack;
	}
}
