using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 材质球和颜色相关静态方法
 * author Biggo
 **/
public class MaterialUtils
{
	//所有飞船shader的统一名称
	public static readonly string SHIP_SHADER_NAME = "meng52/ShipShader";
	public static readonly string EARTH_SHADER_NAME = "meng52/EarthShader";

	//克隆核心材质球
	public static Material cloneMaterial (GameObject go,string materialName) {
		MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer> ();
		int i,len;
		len = meshRenderers.Length;
		for (i = 0; i < len; i++) {
			MeshRenderer meshRenderer;
			Material material;
			meshRenderer = meshRenderers [i];
			//get得到实例
			material = meshRenderer.material;
			if (material.shader.name == materialName) {
				return material;
			}
		}
		return null;
	}
	//将所有子物体赋予相同材质球
	public static void setMaterial (GameObject go,Material material) {
		MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer> ();
		int i,len;
		string materialName = material.shader.name;
		len = meshRenderers.Length;
		for (i = 0; i < len; i++) {
			MeshRenderer meshRenderer;
			Material tempMaterial;
			meshRenderer = meshRenderers [i];
			tempMaterial = meshRenderer.material;
			if (tempMaterial.shader.name == materialName) {
				//都指向这个材质球
				meshRenderer.material = material;
			}
		}
	}

	//缓动材质球
	public static void tweenMaterialValue (GameObject go,string varName,float aimValue,float time = 0.2f) {
		MeshRenderer meshRenderer = go.GetComponent<MeshRenderer> ();
		Material material = meshRenderer.material;
		LeanTween.value (go, (float value) => 
			{
				material.SetFloat(varName,value);
			},
			material.GetFloat (varName),
			aimValue,
			time
		);
	}
	public static void tweenMaterialColor (GameObject go,Color color,float time = 0.2f) {
		MeshRenderer meshRenderer = go.GetComponent<MeshRenderer> ();
		Material material = meshRenderer.material;
		LeanTween.value (go, (Color tempColor)=>
			{
				material.color = tempColor;
			},
			material.color,
			color,
			time
		);
	}
	//将所有子物体的材质球做相同的颜色缓动
	public static void colorInOutMaterial (GameObject go,string materialName,Color inColor,Color outColor,float inTime = 0.05f,float outTime = 0.2f) {
		MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer> ();
		int i,len;
		len = meshRenderers.Length;
		for (i = 0; i < len; i++) {
			MeshRenderer meshRenderer;
			Material material;
			meshRenderer = meshRenderers [i];
			material = meshRenderer.material;

			if (material.shader.name == materialName) {
				//特定材质球才受影响            
				Color color = material.color;
				LTDescr ltd = LeanTween.value(go,(Color tempColor)=>
					{
						material.color = tempColor;
					},
					color,
					inColor,
					inTime
				);
				ltd.onComplete = () => {
					LeanTween.value(go,(Color tempColor)=>
						{
							material.color = tempColor;
						},
						inColor,
						outColor,
						outTime
					);
				};

			}
		}
	}



	//将字符串转为颜色值
	public static Color stringToColor (string str) {
		float r = int.Parse (str.Substring (0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
		float g = int.Parse (str.Substring (2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
		float b = int.Parse (str.Substring (4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
		return new Color (r, g, b);
	}
	public static Color cloneColor (Color c) {
		return new Color (c.r, c.g, c.b, c.a);
	}
}


