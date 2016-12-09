using System;
using System.Collections.Generic;
using UnityEngine;

public class ViewUtils {
    public static Vector3 logicToScene(Vector2D vector2D, float radius, MapData mapData) {
        float radian = Convert.ToSingle(vector2D.x) * mapData.xToRadian;
        if(radius == -1) {
            Debug.Log(11);
        }
        //0.0117286
        return new Vector3(
            Mathf.Cos(radian) * radius,
            Convert.ToSingle(vector2D.y) * mapData.yToLength + mapData.objectOffsetY + (radius / mapData.earthRadius - 1) * 3f,
            Mathf.Sin(radian) * radius
        );

    }
    //public static Vector2D sceneToLogic(Vector3 vector3, float radius) {
    //    float y = ( vector3.y - ViewConstant.MAP_OBJECT_OFFSET_Y ) / ViewConstant.MAP_EARTH_HEIGHT * mapData.height;
    //    if(vector3.z < 0) {
    //        return Vector2D.createVector(
    //            ( Mathf.Acos(-vector3.x / radius) / ( Mathf.PI * 2 ) + 0.5f ) * mapData.width,
    //            y
    //        );
    //    }
    //    return Vector2D.createVector(
    //        Mathf.Acos(vector3.x / radius) / ( Mathf.PI * 2 ) * mapData.width,
    //        y
    //    );
    //}

    //克隆核心材质球ConfigConstant.SHIP_SHADER_NAME
    public static void changeColor(GameObject go, string materialName, Dictionary<string, object> color) {
        changeColor(getMaterial(go, materialName), color);
    }

    public static void changeColor(Material material, Dictionary<string, object> color) {
        if(null == material) return;
        material.SetFloat("_Hue", Convert.ToSingle(color["hue"]));
        material.SetFloat("_Saturation", Convert.ToSingle(color["saturation"]));
        material.SetFloat("_Value", Convert.ToSingle(color["value"]));
        material.SetColor("_Blue", stringToColor(color["passA"].ToString()));
        material.SetColor("_Green", stringToColor(color["passB"].ToString()));
        material.SetColor("_Red", stringToColor(color["passC"].ToString()));
    }

    ///将所有特殊shaderName的 材质球颜色更换为指定颜色
	public static void changeColorMaterial(GameObject go, string shaderName, Color color) {
        MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
        int i, len;
        len = meshRenderers.Length;
        for(i = 0; i < len; i++) {
            MeshRenderer meshRenderer;
            Material material;
            meshRenderer = meshRenderers[i];
            //get得到实例
            material = meshRenderer.material;
            if(material.shader.name == shaderName) {
                material.color = color;
            }
        }
    }

    //缓动材质球
    public static void tweenMaterialValue(GameObject go, string varName, float aimValue, float time = 0.2f) {
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        Material material = meshRenderer.material;
        LeanTween.value(go, (float value) => {
            material.SetFloat(varName, value);
        },
            material.GetFloat(varName),
            aimValue,
            time
        );
    }

    ///缓动缩放到
    public static GameObject tweenScale(GameObject go, Vector3 scaleStart, Vector3 scaleMiddle, Vector3 scaleEnd, float time) {
        if(go == null) return null;
        go.transform.localScale = scaleStart;
        if(scaleStart.Equals(scaleMiddle) && scaleStart.Equals(scaleEnd))
            return go;
        tweenScale(go, scaleMiddle, scaleEnd, time, 0);
        return go;
    }
    ///缓动缩放到
    public static GameObject tweenScale(GameObject go, Vector3 scaleMiddle, Vector3 scaleEnd, float time, float delay) {
        if(go == null) return null;
        LTDescr ltd;
        LTDescr ltd2;
        ltd = LeanTween.scale(go, scaleMiddle, time * 0.7f).setDelay(delay);
        ltd.tweenType = LeanTweenType.easeOutSine;
        ltd.onComplete = () => {
            ltd2 = LeanTween.scale(go, scaleEnd, time * 0.3f);
            ltd2.tweenType = LeanTweenType.easeOutBack;
        };
        return go;
    }

    ///缓动缩放到
    public static GameObject tweenScale(GameObject go, float scaleStart, float scaleMiddle, float scaleEnd, float time) {
        if(go == null) return null;
        go.transform.localScale = new Vector3(scaleStart, scaleStart, scaleStart);
        if(scaleStart == scaleMiddle && scaleStart == scaleEnd)
            return go;
        tweenScale(go, scaleMiddle, scaleEnd, time);
        return go;
    }
    ///缓动缩放到
    public static GameObject tweenScale(GameObject go, float scaleMiddle, float scaleEnd, float time) {
        if(go == null) return null;
        LTDescr ltd;
        LTDescr ltd2;
        ltd = LeanTween.scale(go, new Vector3(scaleMiddle, scaleMiddle, scaleMiddle), time * 0.7f);
        ltd.tweenType = LeanTweenType.easeOutSine;
        ltd.onComplete = () => {
            ltd2 = LeanTween.scale(go, new Vector3(scaleEnd, scaleEnd, scaleEnd), time * 0.3f);
            ltd2.tweenType = LeanTweenType.easeOutBack;
        };
        return go;
    }

    ///缓动移动到
    public static GameObject tweenMove(GameObject go, Vector3 positionStart, Vector3 positionMiddle, Vector3 positionEnd, float time) {
        go.transform.localPosition = positionStart;
        if(positionStart.Equals(positionMiddle) && positionStart.Equals(positionEnd))
            return go;
        tweenMove(go, positionMiddle, positionEnd, time);
        return go;
    }
    ///缓动移动到
    public static GameObject tweenMove(GameObject go, Vector3 positionMiddle, Vector3 positionEnd, float time) {
        LTDescr ltd;
        LTDescr ltd2;
        ltd = LeanTween.moveLocal(go, positionMiddle, time * 0.7f);
        ltd.tweenType = LeanTweenType.easeOutSine;
        ltd.onComplete = () => {
            ltd2 = LeanTween.moveLocal(go, positionEnd, time * 0.3f);
            ltd2.tweenType = LeanTweenType.easeOutBack;
        };
        return go;
    }


    public static void tweenMaterialColor(GameObject go, Color color, float time = 0.2f) {
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        Material material = meshRenderer.material;
        LeanTween.value(go, (Color tempColor) => {
            material.color = tempColor;
        },
            material.color,
            color,
            time
        );
    }


    /**
     * 同一个装备用一个材质球去替换。 所以加了个第三个参数 默认应该都是true
     */
    public static Material getMaterial(GameObject go, string materialName, bool allReplace = true) {
        MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
        Material result = null;
        for(int i = 0, len = meshRenderers.Length; i < len; i++) {
            Material material = meshRenderers[i].material;
            if(material.shader.name == materialName) {
                if(null == result) result = material;
                if(!allReplace) return result;

                meshRenderers[i].material = result;
            }
        }
        return result;
    }

    public static void colorInOutMaterial(GameObject go, string materialName, Color inColor, Color outColor, float inTime = 0.05f, float outTime = 0.2f) {
        LeanTween.cancel(go, true);
        colorInOutMaterial(go, getMaterial(go, materialName), inColor, outColor, inTime, outTime);
    }

    //将所有子物体的材质球做相同的颜色缓动
    public static void colorInOutMaterial(GameObject go, Material material, Color inColor, Color outColor, float inTime = 0.05f, float outTime = 0.2f) {
        //特定材质球才受影响            
        Color color = material.color;
        LTDescr ltd = LeanTween.value(go, (Color tempColor) => {
            material.color = tempColor;
        },
            color,
            inColor,
            inTime
        );
        ltd.onComplete = () => {
            LeanTween.value(go, (Color tempColor) => {
                material.color = tempColor;
            },
                inColor,
                outColor,
                outTime
            );
        };
    }

    public static Color cloneColor(Color c) {return new Color(c.r, c.g, c.b, c.a);}

    //将字符串转为颜色值
    public static Color stringToColor(string str) {
        float r = int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float g = int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float b = int.Parse(str.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        return new Color(r, g, b);
    }

    public static void setEnable(GameObject go, bool enabled) {
        Animator[] animators = go.GetComponentsInChildren<Animator>();
        foreach(Animator animator in animators) {
            animator.enabled = enabled;
        }
        //所有粒子停止发射
        ParticleSystem[] particleSystems = go.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem particleSystem in particleSystems) {
            if(enabled) {
                particleSystem.Play();
            } else {
                particleSystem.Stop();
            }
        }
    }

    ///缓动材质球颜色
    public static void colorOutMaterial(GameObject go, Material material, Color endColor, float time = 0.2f) {
        Color color = material.color;
        LTDescr ltd = LeanTween.value(go, (Color tempColor) =>
        {
            material.color = tempColor;
        },
            color,
            endColor,
            time
        );
        ltd.tweenType = LeanTweenType.easeOutSine;
    }

    public static Dictionary<string, object> equal(object obj1, object obj2, string info = "", Dictionary<string, object> equalInfo = null) {
        //return equalInfo;
        if(null == equalInfo) equalInfo = new Dictionary<string, object>();
        if(obj1 is Dictionary<string, object> && obj2 is Dictionary<string, object>) {
            Dictionary<string, object> dic1 = obj1 as Dictionary<string, object>;
            Dictionary<string, object> dic2 = obj2 as Dictionary<string, object>;
            if(dic1.Count != dic2.Count) {
                equalInfo[info] = "字典数量不一致" + dic1.Count + " " + dic2.Count;
                foreach(string key in dic1.Keys) {
                    if(!dic2.ContainsKey(key)) {
                        Debug.Log("不一致的key是" + key);
                    }
                }

                foreach(string key in dic2.Keys) {
                    if(!dic1.ContainsKey(key)) {
                        Debug.Log("不一致的key是" + key);
                    }
                }

            } else {
                foreach(string key in dic1.Keys) {
                    string info2 = info + "." + key;
                    if(!dic2.ContainsKey(key)) {
                        equalInfo[info2] = "字典key不一致" + key;
                    } else {
                        equal(dic1[key], dic2[key], info2);
                    }
                }
            }
            
        } else if(obj1 is object[] && obj2 is object[]) {
            object[] arr1 = obj1 as object[];
            object[] arr2 = obj2 as object[];
            if(arr1.Length != arr2.Length) {
                int index = -1;
                object[] a1 = arr1.Length > arr2.Length ? arr1 : arr2;
                object[] a2 = arr1.Length < arr2.Length ? arr1 : arr2;
                for(int i = 0, len = a2.Length; i < len; i++) {
                    //Dictionary<string, object> dic1 = (Dictionary<string, object>)a1[i];
                    //Dictionary<string, object> dic2 = (Dictionary<string, object>)a2[i];
                    //if(dic1["netId"].ToString() != dic2["netId"].ToString()) {
                    //    index = i;
                    //    break;
                    //}
                }
                if(index == -1) index = a2.Length;


                equalInfo[info] = "数组数量不一致    " + index;

            } else {
                for(int i = 0, len = arr1.Length; i < len; i++) {
                    equal(arr1[i], arr2[i], info + "[" + i + "]");
                }
            }            
        } else {
            //if(obj2 is double) obj2 = Convert.ToDouble(obj2);
            //if(obj1 is double) obj1 = Utils.precise(Convert.ToDouble(obj1), 4);
            //if(obj2 is double) obj2 = Utils.precise(Convert.ToDouble(obj2), 4);
            //try {
                if(obj1 == obj2) {

                }else if(obj1 is double && obj2 is double) {
                    if(Math.Abs((double)obj1 - (double)obj2) > 0.00011) {
                        equalInfo[info] = "值不一致" + obj1 + "   " + obj2;
                    }
                    //Debug.Log(Math.Abs((double)obj1 - (double)obj2));
                } else if(obj1.ToString() != obj2.ToString()) {
                    equalInfo[info] = "值不一致" + obj1 + "   " + obj2;
                }
            //}catch(Exception e) {
            //    Debug.Log("dfdfd");
            //}
            
        }
        return equalInfo;
    }

    public static string toString(Dictionary<string, object> dic) {
        string result = "";
        foreach(string key in dic.Keys) {
            result += key + "   :" + dic[key] + "\n";
        }
        return result;
    }

    public static void clearChildren(Transform parent) {
        foreach(Transform child in parent) {
            ResFactory.Destroy(child.gameObject);
        }
    }
}
