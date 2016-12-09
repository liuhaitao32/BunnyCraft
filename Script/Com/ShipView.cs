using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * 飞船视图组件，外部调用change方法改变外观
 * author Biggo
 **/
using System;


public class ShipView : MonoBehaviour
{

	static readonly int MODEL_COUNT = 2;
	static readonly int PART_COUNT = 3;
	static readonly int ALL_COUNT = MODEL_COUNT + PART_COUNT;

	static readonly string[] MODEL_PREFIX = { "main", "avatar" };
	static readonly string[] PART_PREFIX = { "head", "wing", "tail" };


	private string _shipPureId;
	private float _shipHue;
	private float _shipSaturation;
	private float _shipValue;
	private Color _passAColor;
	private Color _passBColor;
	private Color _passCColor;

	private string[] _modelIds;
	private GameObject[] _allGameObjects = new GameObject[ALL_COUNT];

	//	private HighlightableObject _highlightableObject;

	string _id = "ship000";
	bool _showAvatar = false;


	void Start ()
	{
//		_highlightableObject = GetComponent<HighlightableObject> ();
//		reset ();
	}

	public void change (string id, bool showAvatar = false)
	{
		_id = id;
		_showAvatar = showAvatar;
		reset ();
	}

	private void reset ()
	{
		_shipPureId = _id.Substring (4);
		_modelIds = new string[MODEL_COUNT];
		_modelIds [0] = MODEL_PREFIX [0] + _shipPureId;
		_modelIds [1] = MODEL_PREFIX [1] + "000";
		initShipCongfig ();
		showModels ();
	}


	//通过数据，显示所有模型
	private void showModels ()
	{
		string id;
		for (int i = 0; i < MODEL_COUNT; i++)
		{
			destroyModel (i);
			if (i == 1 && !_showAvatar)
			{
				continue;
			}
			id = _modelIds [i];
			initMaterial (initModelById (id, i));
		}
		//把其他碎部件和主体统一
		for (int i = 0; i < PART_COUNT; i++)
		{
			id = PART_PREFIX [i] + _shipPureId;
			initMaterial (initModelById (id, i + MODEL_COUNT));
		}
		updatePartHighlight ();
	}

	private void destroyModel (int index)
	{
		Destroy (_allGameObjects [index]);
		_allGameObjects [index] = null;
	}
	//初始化一个go，并将之前的移除
	private GameObject initModelById (string id, int index)
	{
		GameObject go;
		string res = id; 
		destroyModel (index);

		//加载
//		go = Resources.Load ("Fight/Prefabs/Ship/"+res) as GameObject;
		go = Tools.GetPrefab ("Fight/Prefabs/Ship/" + res) as GameObject;
		if (go)
		{
//			go = Instantiate (go)as GameObject;
			_allGameObjects [index] = go;

			go.SetActive (false);
			go.transform.SetParent (this.transform, false);

			go.transform.localRotation = new Quaternion ();
			go.transform.localScale = new Vector3 (1, 1, 1);
			if (index == MODEL_COUNT - 1)
			{
				//兔子的位置
				go.transform.localPosition = new Vector3 (0, 0, 0.95f);
				go.transform.Rotate (new Vector3 (0, 15f, 0));
			}
			else
			{
				go.transform.localPosition = new Vector3 ();
			}
			go.transform.localPosition += new Vector3 (0, 0, -0.5f);
			go.SetActive (true);
		}
		return go;
	}
	//初始化克隆材质球，并赋予所有子对象
	private Material initMaterial (GameObject go)
	{
		if (null != go)
		{
			Material tempMaterial = MaterialUtils.cloneMaterial (go, MaterialUtils.SHIP_SHADER_NAME);
			if (tempMaterial != null)
			{
				//按玩家配置修改色相
				tempMaterial.SetFloat ("_Hue", _shipHue);
				tempMaterial.SetFloat ("_Saturation", _shipSaturation);
				tempMaterial.SetFloat ("_Value", _shipValue);
				tempMaterial.SetColor ("_Blue", _passAColor);
				tempMaterial.SetColor ("_Green", _passBColor);
				tempMaterial.SetColor ("_Red", _passCColor);

				MaterialUtils.setMaterial (go, tempMaterial);
			}
			return tempMaterial;
		}
		else
			return null;
	}

	private void initShipCongfig ()
	{
		//数据源
		Dictionary<string,object> shipData = (Dictionary<string,object>)DataManager.inst.body [_id];

		_shipHue = shipData.ContainsKey ("hue") ? Convert.ToSingle (shipData ["hue"]) : 0;
		_shipSaturation = shipData.ContainsKey ("saturation") ? Convert.ToSingle (shipData ["saturation"]) : 0;
		_shipValue = shipData.ContainsKey ("value") ? Convert.ToSingle (shipData ["value"]) : 0;
		_passAColor = MaterialUtils.stringToColor (shipData.ContainsKey ("passA") ? (string)shipData ["passA"] : "FFFFFF");
		_passBColor = MaterialUtils.stringToColor (shipData.ContainsKey ("passB") ? (string)shipData ["passB"] : "FFFFFF");
		_passCColor = MaterialUtils.stringToColor (shipData.ContainsKey ("passC") ? (string)shipData ["passC"] : "FFFFFF");
	}

	//更新装备和外部描边
	private void updatePartHighlight ()
	{
//		_highlightableObject.enabled = false;
//		_highlightableObject.enabled = true;
//		_highlightableObject.constantly = true;
	}
}
