using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

///边缘提示
///author Biggo
public class EdgeHint : MonoBehaviour {
	///目标
	public FightEntity fightEntity;
	///是否使用最远距离
	public bool isRadarMax;
	///是否永生（目标死亡仍然标示）
	public bool isEternal;

	Vector2 localPositionV2;
	Vector3 localScaleV3;

	///材质球列表
	List<Material> materialList;
	///贴图列表
	Image[] imageArr;

	///上一次缓存的不透明度
	float lastAlpha;

    public ClientRunTime clientRunTime;

//	void Awake () {
//		Init ();
//	}

	public void init (FightEntity entity) {
		this.fightEntity = entity;
        this.clientRunTime = (ClientRunTime)entity.map;
		this.localPositionV2 = new Vector2();
		this.localScaleV3 = new Vector3 (1, 1, 1);
        this.materialList = new List<Material> ();
		Material material;

		//一次性取出所有 MeshRenderer ParticleSystem
		Renderer[] renderers = this.GetComponentsInChildren<Renderer>(); 
		foreach (Renderer renderer in renderers) {
			material = renderer.material;
			renderer.material = material;
			materialList.Add (material);
		}
		imageArr = this.GetComponentsInChildren<Image>(); 
        
		if (this.fightEntity.type == ConfigConstant.ENTITY_PLAYER) {
			Image image = Tools.FindChild2("Canvas/faceImage", this.gameObject).transform.GetChild (0).GetComponent<Image> ();
            ClientPlayerEntity player = (ClientPlayerEntity)entity;
            ResFactory.setHeadSprite(player.headUrl, image, player.uid);
			//Sprite spr = ((Player)player.view).faceSprite;
			//image.sprite = spr;
		}

		lastAlpha = 1f;
		ChangeAlpha (0);
	}



	/// 萝卜提示切换状态
	//public void changeRadishEdgeHint (int team) {
	//	//清理子容器
	//	ViewUtils.clearChildren(this.transform);
	//	GameObject gameObject;
	//	if (team != 0) {
	//		gameObject = this.clientRunTime.scene.edgeHintController.getEdgeHintGameObject(team);
 //           gameObject.transform.SetParent(this.transform, false);
	//	}
	//}
	
	public void updatePosition () {
		ClientPlayerEntity player = this.clientRunTime.localPlayer;
		float radarDistance = isRadarMax ? this.clientRunTime.mapData.widthHalf : Convert.ToSingle(player.getProperty(ConfigConstant.PROPERTY_RADAR));
        Vector2D aimLogicDelta = Collision.realPosition(player.position, this.fightEntity.position, this.clientRunTime.mapData).deltaPos;
		double logicDistance = aimLogicDelta.length;
        double logicAbsX = Math.Abs (aimLogicDelta.x);
        double logicAbsY = Math.Abs (aimLogicDelta.y);

		if (!isEternal && !fightEntity.alived) {
			this.gameObject.SetActive (false);
			return;
		}

		if (logicDistance > radarDistance|| (logicAbsX < EdgeHintController.LOGIC_EDGE_X && logicAbsY < EdgeHintController.LOGIC_EDGE_Y)) {
            //超出雷达范围，或直接可以看到，不显示提示
            this.gameObject.SetActive (false);
		} else {

			double angle = aimLogicDelta.angle;
			Vector2D v2d = aimLogicDelta.clone ();
            //			Debug.Log (angle);
            double angleDeltaAbs = Math.Abs (angle);
            //根据相距距离较长的轴，缩放，越远越小，最大为1
            double tempScale;
			//逻辑
			if (angleDeltaAbs <= EdgeHintController.VERTEX_ANGLE || angleDeltaAbs >= Math.PI - EdgeHintController.VERTEX_ANGLE) {
                //左右边超出
                tempScale = EdgeHintController.LOGIC_EDGE_X / logicAbsX;
				v2d.y *= tempScale;
                v2d.x = v2d.x > 0 ? EdgeHintController.LOGIC_EDGE_X : -EdgeHintController.LOGIC_EDGE_X;
			} else
			{
                //上下边超出, logicAbsY != 0
                tempScale = EdgeHintController.LOGIC_EDGE_Y / logicAbsY;
				v2d.x *= tempScale;
                v2d.y = v2d.y > 0 ? EdgeHintController.LOGIC_EDGE_Y : -EdgeHintController.LOGIC_EDGE_Y;
			}
			tempScale = Math.Pow (tempScale,0.8);


            //v2d转为视图含义
            v2d.x = v2d.x / EdgeHintController.LOGIC_EDGE_X * EdgeHintController.VIEW_EDGE_X;
            v2d.y = v2d.y / EdgeHintController.LOGIC_EDGE_Y * EdgeHintController.VIEW_EDGE_Y;

			localPositionV2.x = Convert.ToSingle(v2d.x);
			localPositionV2.y = Convert.ToSingle(v2d.y);
            
            //根据相距距离较长的轴，缩放，越远越小，临近时最大为1
            localScaleV3.x = localScaleV3.y = localScaleV3.z = Convert.ToSingle(tempScale);
            
			//改变透明度，很近时很小，很远时很小
			double temp = logicDistance / radarDistance;
            double alpha;
			if (temp > 0.8f) {
				//太远
//				alpha = (1f - temp) * 4f+0.2f;
				alpha = (1f - temp) * 5f;
			} else if (tempScale > 0.6f) {
				//太近
//				alpha = (1f - tempScale) * 2f+0.2f;
				alpha = (1f - tempScale) * 2f;
//				Debug.Log ("太近tempScale:"+tempScale+"   alpha:"+alpha);
			} else {
				alpha = 1f;
			}

			ChangeAlpha (Convert.ToSingle(alpha));
			this.transform.localScale = localScaleV3;
            //首次进来 让他直接跳到目标点。
            if(!this.gameObject.activeSelf) {
                this.update(1);
            }
            //将提示坐标映射到屏幕边缘
            this.gameObject.SetActive(true);
        }
	}
    

	///调整其内材质球和所有贴图的颜色透明度
	public void ChangeAlpha (float alpha) {
		if(Mathf.Abs(lastAlpha - alpha) > 0.05f)
		{
			if (alpha <= 0.05f)
				alpha = 0f;
			else if(alpha >= 0.95f)
				alpha = 1f;
			lastAlpha = alpha;

			int i, len;
			len = materialList.Count;
			for (i = 0; i < len; i++) {
				Material material = materialList [i];
				Color color = material.GetColor ("_TintColor");
				color.a = alpha;
				material.SetColor ("_TintColor", color);
			}
			//头像
			len = imageArr.Length;
			for (i = 0; i < len; i++) {
				Image image = imageArr [i];
				Color color = image.color;
				color.a = alpha * 2f;
				image.color = color;
			}
		}
	}

    public void update(float rate = 0.3f) {
        this.transform.localPosition = Vector2.Lerp(this.transform.localPosition, localPositionV2, rate);
        
    }

	public void Destroy(){
		Destroy (this.gameObject);
	}
}
