using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

///边缘提示控制器
///author Biggo
public class EdgeHintController : MonoBehaviour, IClientView{
	public static int INDEX_RADISH = 3;
	//视图内可以看到的逻辑距离，根据屏幕宽高比可变X
	public static int LOGIC_EDGE_X = 2100;
	public static int LOGIC_EDGE_Y = 1250;

	//视图内可以看到的屏幕距离，根据屏幕宽高比可变X
	public static float VIEW_EDGE_X = 6.5f;
	public static float VIEW_EDGE_Y = 3.8f;

	//屏幕长宽比导致的顶点角度（右下角向量）
	public static float VERTEX_ANGLE;

	public GameObject[] prefabs;

	List<EdgeHint> edgeHintList = new List<EdgeHint>();
	EdgeHint radishEdgeHint;
    

    void Start() {
        resetScale();
        FightMain.instance.addListener(EventConstant.PLAYER_IN, (MainEvent e) => {
            FightMain.instance.selection.registerClientView(this);
            FightMain.instance.selection.addListener(EventConstant.LOGIC_COMPLETE, this.updateEdge);
        });
    }

    private void updateEdge(MainEvent obj) {
        for(int i = 0, len = this.edgeHintList.Count; i < len; i++) {
            this.edgeHintList[i].updatePosition();
        }
    }

    //	void Update () {
    //		reScale();
    //	}
    void resetScale () {
//		Debug.Log (Screen.width+","+ Screen.height);
		float temp = (float)Screen.width / Screen.height;
		LOGIC_EDGE_X = (int)(LOGIC_EDGE_Y * temp);
		VIEW_EDGE_X = VIEW_EDGE_Y * temp;
		VERTEX_ANGLE = Mathf.Atan2 (EdgeHintController.VIEW_EDGE_Y, EdgeHintController.VIEW_EDGE_X);
	}


	///找到某个FightObject的提示
	public EdgeHint findEdgeHint (FightEntity fightObject) {
		int i, len;
		len = edgeHintList.Count;
		for (i = 0; i < len; i++) {
			EdgeHint edgeHint = edgeHintList [i];
			if (fightObject == edgeHint.fightEntity) {
				return edgeHint;
			}
		}
		return null;
	}

    /// 获得一个物体的边缘提示GO
	public GameObject getEdgeHintGameObject(int index) {
        return ResFactory.createObject<GameObject>(prefabs[index]);
    }

    /// 增加对一个物体的边缘提示
    public void addFightObject (int index,FightEntity fightEntity) {
		GameObject go = this.getEdgeHintGameObject(index);
		go.SetActive (false);
		go.transform.SetParent (this.gameObject.transform, false);

		EdgeHint edgeHint = go.GetComponent<EdgeHint> ();
		//edgeHint.name = prefabs[index].name +" "+fightEntity.view.name;
		edgeHint.init(fightEntity);
		if (index == INDEX_RADISH) {
			radishEdgeHint = edgeHint;
		}
		edgeHintList.Add (edgeHint);
//		ViewUtils.setGameObjectScale (go, 12f);
	}
	///移除对一个物体的边缘提示
	public void removeFightObject (FightEntity fightEntity) {
		int i, len;
		EdgeHint edgeHint;
		len = edgeHintList.Count;
		for (i = 0; i < len; i++) {
			edgeHint = edgeHintList [i];
			if (edgeHint.fightEntity == fightEntity) {
				edgeHintList.RemoveAt (i);
				edgeHint.Destroy ();
				return;
			}
		}
	}

    ///开启或关闭某个玩家的远程提示
    public void ChangeEdgeHint(PlayerEntity player, bool isRadarMax) {
        this.findEdgeHint(player).isRadarMax = isRadarMax;
    }

    public void onUpdate(float rate) {
        for(int i = 0, len = this.edgeHintList.Count; i < len; i++) {
            if(this.edgeHintList[i].gameObject.activeSelf) this.edgeHintList[i].update();
        }
    }


    /// 萝卜提示切换状态
    //public void changeRadishEdgeHint(int team) {
    //    if(null != radishEdgeHint) {
    //        radishEdgeHint.changeRadishEdgeHint(team);
    //    }
    //}

    public void clear () {
		radishEdgeHint = null;
		edgeHintList.Clear ();
		ViewUtils.clearChildren (this.transform);
	}
}
