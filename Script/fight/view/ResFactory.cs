using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResFactory:MonoBehaviour {

	public static ResFactory instance;

	public GameObject player;

	public GameObject bean1;

    public GameObject bean2;

    public GameObject bean3;

    public GameObject bean4;    

    public GameObject call;

    public AvatarView avatar;

    public static bool enable = true;

    public static string voiceFolder = "voice000";

    public ResFactory () {
        instance = this;
	}
//————————————————————————————创建预设体————————————————————————————————————

    public static T createObject<T>(UnityEngine.Object obj) where T:UnityEngine.Object {
        try {
            return Instantiate<T>((T)obj);
        } catch {
            return null;
        }
        
	}

    public GameObject getBean(int type) {
        GameObject result = null;
        switch(type){
            case 1:
                result = bean1;
                break;
            case 2:
                result = bean2;
                break;
            case 3:
                result = bean3;
                break;
            case 4:
                result = bean4;
                break;
        }
        return createObject<GameObject>(result);
    }

//————————————————————————————加载类型————————————————————————————————————

    private static Dictionary<string, UnityEngine.Object> map = new Dictionary<string, UnityEngine.Object>();

    public static GameObject getShip(string name) { return loadPrefab("Ship/" + name); }

    public static GameObject getOther(string name) { return loadPrefab("Others/" + name); }

    public static GameObject loadPrefab(string path) { return loadRes<GameObject>("Fight/Prefabs/" + path); }

    public static T loadRes<T>(string name) where T : UnityEngine.Object{
        return (T)( map.ContainsKey(name) ? map[name] : map[name] = Resources.Load<T>(name) );
    }

    public static Sprite getSprite(string name) { return loadRes<Sprite>("Fight/Image/" + name); }

    public static AudioClip loadVoiceClip(string voiceName) { return loadRes<AudioClip>("Fight/Voice/" + voiceFolder + "/" + voiceName); }

    public static void setHeadSprite(string name, Image image, string uid) {
        if(FightMain.fightTest) return;
        name = ModelUser.GetHeadUrl(name, false, uid.IndexOf("-") == 0, true);
        if(map.ContainsKey(name)) {
            image.sprite = (Sprite)map[name];
        } else {
            Tools.SetLoaderButtonUrl(null, name, null, image, (sp) => {
                map[name] = image.sprite;
            });
        }

    }

    //public static void getHeadSprite(string path, Image image) {
    //    if(map.ContainsKey(path)) {
    //        image.sprite = (Sprite)map[path];
    //    }

    //    return loadRes<Sprite>("Fight/Image/" + name);

    //}


    //————————————————————————————以下是特效缓存————————————————————————————————————

    public static Dictionary<string, List<GameObject>> effectMap = new Dictionary<string, List<GameObject>>();

    public static GameObject getEffect(string name) {
        GameObject result = loadPrefab("Effect/" + name);
        if(result == null) {
            Debug.Log("特效资源缺失" + name);
            result = new GameObject();
        }

        return result;
    }

    public static GameObject getCacheEffect(string name, ClientRunTime clientRunTime, bool visible = true) {
        GameObject result = null;
        if(!effectMap.ContainsKey(name)) effectMap[name] = new List<GameObject>();

        List<GameObject> gameObjects = effectMap[name];
        if(gameObjects.Count > 0) {
            int len = gameObjects.Count - 1;
            result = gameObjects[len];
            gameObjects.RemoveAt(len);
        } else {
            result = ResFactory.getEffect(name);
            if(null == result) Debug.Log("没有资源" + name);
            result = ResFactory.createObject<GameObject>(result);
            result.AddComponent<CacheEffect>().clientRunTime = clientRunTime;
            result.name = name;
            AutoDestroy auto = result.GetComponent<AutoDestroy>();
            if(auto != null) {
                auto.onlyDeactivate = true;
            }
        }
        if(visible) result.gameObject.SetActive(true);//有些要延迟生效 就不给他visible等于true。
        return result;
    }



    public static void addPool(GameObject gameObject) {
        if(!enable) return;
        //Destroy(gameObject);
        //return;
        if(effectMap[gameObject.name].Contains(gameObject)) {
            Debug.Log("有问题 重复添加！" + gameObject.name);
            return;
        }
        //太多还是要清理的！
        //if(effectMap[gameObject.name].Count >= 20) {
            //Destroy(gameObject);
        //} else {
            effectMap[gameObject.name].Add(gameObject);
        //}
        
    }

    public static void removePool(GameObject gameObject) {
        if(!enable) return;
        if(!effectMap.ContainsKey(gameObject.name)) return;
        effectMap[gameObject.name].Remove(gameObject);
        
    }

 //————————————————————————————特效子弹——————————————————————————————————   

    public static Bullet getBullet(string name, ClientRunTime clientRunTime) {
        GameObject go = getCacheEffect(name, clientRunTime, false);
        Bullet result = go.GetComponent<Bullet>();
        if(null == result) {
            result = go.AddComponent<Bullet>();
        } else {
            TimerManager.inst.Add(0, 1, (float t) => {
                if(!result.fightEntity.cleared) {
                    result.gameObject.SetActive(true);
                    result.init();
                }
            });
        }        
        result.bulletName = name;
        return result;
    }

    //————————————————————————————清理——————————————————————————————————


    public static void clear() {
        foreach(string key in effectMap.Keys) {
            while(0 != effectMap[key].Count) {
                Destroy(effectMap[key][0]);
                effectMap[key].RemoveAt(0);
            }
        }
        effectMap.Clear();
        enable = false;
    }
    
}



















class CacheEffect : MonoBehaviour {
    private bool isDestroyed = false;

    public ClientRunTime clientRunTime;

    /// <summary>
    /// 在隐藏的时候 添加到Pool里面。
    /// </summary>
    void OnDisable() {
        if(!this.gameObject.activeSelf && !this.isDestroyed) ResFactory.addPool(this.gameObject);
    }

    void OnDestroy() {
        this.isDestroyed = true;
        if(!this.clientRunTime.cleared) ResFactory.removePool(this.gameObject);
    }
}
