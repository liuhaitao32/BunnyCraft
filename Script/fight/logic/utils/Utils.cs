using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class Utils {

    public static void clearList(IList list) {
        while(list.Count != 0) {
            object item = list[0];
            clearObject(item);
            if(list.Contains(item)) list.Remove(item);
        }
    }

    public static void clearObject(object obj) {
        IClear item = obj as IClear;
        if(null != item && !item.cleared) item.clear();
    }

    public static double transitionNum(double start, double end, double rate) {
		rate = Math2.range (rate, 1f, 0);
        return start + ( end - start ) * rate;
    }

    public static bool equal(Dictionary<string, object> datas, string key, int value) {
        return datas.ContainsKey(key) && (int)(datas[key]) == value;
    }

    public static Dictionary<string, object> cloneDictionary(Dictionary<string, object> source) {
        Dictionary<string, object> result = new Dictionary<string, object>();
        foreach(string key in source.Keys) {
            result[key] = source[key];
        }
        return result;
    }

    public static void union1(Dictionary<string, object> dic1, Dictionary<string, object> dic2) {
        foreach(string key in dic2.Keys) {
            dic1[key] = dic2[key];
        }
    }

    public static void union2(Dictionary<string, double> dic1, Dictionary<string, double> dic2) {
        foreach(string key in dic2.Keys) {
            dic1[key] = dic2[key];
        }
    }
    /**
     * 求physon的深复制。
     */
    public static object clone(object obj) {
        object result = null;
        if(obj is Dictionary<string, object>) {
            Dictionary<string, object> sourceDic = (Dictionary<string, object>)obj;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach(string key in sourceDic.Keys) {
                dic[key] = clone(sourceDic[key]);
            }
            result = dic;
        } else if(obj is object[]) {
            object[] sourceArr = (object[])obj;
            object[] arr = new object[sourceArr.Length];
            for(int i = 0, len = sourceArr.Length; i < len; i++) {
                arr[i] = clone(sourceArr[i]);
            }
            result = arr;
        } else {
            result = obj;
        }
        return result;
    }

    public static void setDictionay(Dictionary<string, object> dic, string path, Dictionary<string, object> formula, int level) {
        Regex r = new Regex("^(\\w+)\\[(\\d+)\\]$");
        Match m;
        string[] msg = path.Split('.');
        object[] o;
        Dictionary<string, object> d;
        object temp = dic;
        int index;
        for(int i = 0, len = msg.Length; i < len; i++) {
            if(r.IsMatch(msg[i])) {
                m = r.Match(msg[i]);
                d = (Dictionary<string, object>)temp;
                temp = d[m.Groups[1].ToString()];

                o = (object[])temp;
                index = Convert.ToInt32(m.Groups[2]);

                if(i == len - 1) {
                    o[index] = LogicOperation.grow(Convert.ToSingle(temp), formula, level);
                } else {
                    temp = o[index];
                }
            } else {
                d = (Dictionary<string, object>)temp;
                if(i == len - 1) {
                    d[msg[i].ToString()] = LogicOperation.grow(Convert.ToSingle(temp), formula, level);
                } else {
                    temp = d[msg[i].ToString()];
                }
            }
        }
    }

    public static List<string> changeTo1(object[] objs) {
        return new List<object>(objs).ConvertAll<string>((e) => { return e.ToString(); });
    }

    public static object[] changeTo2(List<string> objs) {
        return objs.ConvertAll<object>((e) => { return e; }).ToArray();
    }

    public static int toInt(double value) {
        return (int)( value * 100000 );
    }

    public static double toFloat(int value) {
        return  value * 1f / 100000 ;
    }

    public static object[] changeTo3(List<Dictionary<string, object>> objs) {
        return objs.ConvertAll<object>((e) => { return e; }).ToArray();
    }

    public static double precise(double value, int digits = 0) {
        value = Math.Round(value, digits + 5);
        double multiple = Math.Pow(10, digits);
        value = (int)(value * multiple);
        value = value / multiple;
        return value;
    }
}
