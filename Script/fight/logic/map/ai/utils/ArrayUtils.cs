using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

///数组和列表转换用工具
public class ArrayUtils
{
	///得到最大值所在的index
	public static int getMaxValueIndex(double[] marks){
		int i;
		double max = double.MinValue;
		int maxIndex = 0;
		int len = marks.Length;
		for (i = 0; i < len; i++) {
			if (marks [i] > max) {
				max = marks [i];
				maxIndex = i;
			}
		}
		return maxIndex;
	}
	///将数组单位化，最大值为1
	public static double[] normalize(double[] marks){
		int i;
		double max = double.MinValue;
		int len = marks.Length;
		for (i = 0; i < len; i++) {
			if (marks [i] > max) {
				max = marks [i];
			}
		}
		for (i = 0; i < len; i++) {
			marks [i] /= max;
		}
		return marks;
	}
		
	///把arr中的值加个系数返回
	public static double[] addArrayValue(double[] arr, double value)  
	{  
		if (value == 0)
			return arr;
		int len = arr.Length;
		for(int i=0; i<len; i++)  
		{  
			arr [i] += value;
		}  
		return arr;
	} 
	///把arr2中的值加到arr1中
	public static double[,] addArray2D(double[,] arr1, double[,] arr2)  
	{  
		int i, j, iLen, jLen;
		iLen = arr1.GetLength (0);
		jLen = arr1.GetLength (1);
		for(i = 0; i<iLen; i++)  
		{  
			for (j = 0; j < jLen; j++) {  
				arr1 [i,j] += arr2 [i,j];
			}
		}  
		return arr1;
	} 	
	///把arr2中的值加到arr1中
	public static int[] addArrayInt(int[] arr1, int[] arr2)  
	{  
		int len = arr1.Length;
		for(int i=0; i<len; i++)  
		{  
			arr1[i] += arr2[i];
		}  
		return arr1;
	} 	
	///arr1减去arr2
	public static int[] subtractArrayInt(int[] arr1, int[] arr2)  
	{  
		int len = arr1.Length;
		for(int i=0; i<len; i++)  
		{  
			arr1[i] -= arr2[i];
		}  
		return arr1;
	} 
	///把arr2中的值加到arr1中
	public static double[] addArray(double[] arr1, double[] arr2)  
	{  
		int len = arr1.Length;
		for(int i=0; i<len; i++)  
		{  
			arr1[i] += arr2[i];
		}  
		return arr1;
	} 	
	///arr1减去arr2
	public static double[] subtractArray(double[] arr1, double[] arr2)  
	{  
		int len = arr1.Length;
		for(int i=0; i<len; i++)  
		{  
			arr1[i] -= arr2[i];
		}  
		return arr1;
	} 
	///把arr中的值乘个系数返回
	public static double[] multiplyArrayValue(double[] arr, double value)  
	{  
		if (value == 1)
			return arr;
		int len = arr.Length;
		for(int i=0; i<len; i++)  
		{  
			arr [i] *= value;
		}  
		return arr;
	} 	
	///两个array中对应系数相乘，最终相加返回（暂时必须保证数组长度相同）
	public static double multiplyArray(double[] arr1, double[] arr2)  
	{  
		int len = Math.Min(arr1.Length,arr2.Length);
		double temp = 0;
		for(int i=0; i<len; i++)  
		{  
			temp += arr1 [i] * arr2 [i];
		}  
		return temp;
	} 	


	///Array所有单位求和
	public static int sumArrayInt(int[] arr, int start = -1, int end = -1)  
	{  
		end = end < 0?arr.Length:Math.Min (arr.Length, end);
		start = start<0?0:Math.Min (start, end);
		int temp = 0;
		for(int i=start; i<end; i++)  
		{  
			temp += arr[i];
		}  
		return temp;
	} 
	///Array所有单位求和
	public static double sumArray(double[] arr, int start = -1, int end = -1)  
	{  
		end = end < 0?arr.Length:Math.Min (arr.Length, end);
		start = start<0?0:Math.Min (start, end);
		double temp = 0;
		for(int i=start; i<end; i++)  
		{  
			temp += arr[i];
		}  
		return temp;
	}

	///查找元素位置
	public static int indexOfArray<T>(T[] arr, T value){
		int i;
		int len = arr.Length;
		for (i = 0; i < len; i++) {
			if (arr [i].Equals(value))
				return i;
		}
		return -1;
	}
	///克隆Array,只从start到end
	public static T[] cloneArray<T>(T[] arr, int start = -1, int end = -1)  
	{  
		end = end < 0?arr.Length:Math.Min (arr.Length, end);
		start = start<0?0:Math.Min (start, end);

		T[] returnArr= new T[end-start]; 
		for(int i=start; i<end; i++)  
		{  
			returnArr[i] = arr[i];
		}  
		return returnArr;
	} 
	///List转Array,只从start到end
	public static T[] listToArray<T>(List<T> list, int start = -1, int end = -1)  
	{  
		end = end < 0?list.Count:Math.Min (list.Count, end);
		start = start<0?0:Math.Min (start, end);

		T[] arr= new T[end-start]; 
		for(int i=start; i<end; i++)  
		{  
			arr[i] = list[i];
		}  
		return arr;
	} 
	///Array转List,只从start到end
	public static List<T> arrayToList<T>(T[] arr, int start = -1, int end = -1)  
	{  
		end = end < 0?arr.Length:Math.Min (arr.Length, end);
		start = start<0?0:Math.Min (start, end);

		List<T> list=new List<T>(); 
		for(int i=start; i<end; i++)  
		{  
			list.Add (arr [i]);
		}  
		return list;
	} 

	///连接两个列表,到list1
	public static List<T> concatList<T>(List<T> list1,List<T> list2)  
	{  
		int i, len;  
		len = list2.Count;
		for (i = 0; i < len; i++) {
			list1.Add (list2 [i]);
		}
		return list1;
	} 


	///从list1里排除list2中所有元素，返回新的list
	public static List<T> getExcludeList<T>(List<T> list1,List<T> list2)
	{  
		int i, j, len1,len2;  
		T temp1,temp2;
		bool isSame;
		//		index = 0;
		len1 = list1.Count;
		len2 = list2.Count;
		List<T> returnList = new List<T>();
		//		Debug.Log ("排除前数组长度："+reList.Length);
		for(i=0; i<len1; i++)  
		{  
			isSame = false;
			temp1 = list1[i];
			for(j=0; j<len2; j++)  
			{  
				temp2 = list2[j];
				if (temp1.Equals(temp2)) {
					//排除
					isSame = true;
					break;
				}
			}
			if (!isSame) {
				returnList.Add (temp1);
				//				index++;
			}
		}
		//		Debug.Log ("排除后数组长度："+reList.Length);
		return returnList;
	} 

	///合并去重，直接修改list1
	public static List<T> mergeList<T>(List<T> list1,List<T> list2)
	{  
		int i, j, len1,len2;  
		T temp1,temp2;
		bool isSame;
		//		index = 0;
		len1 = list1.Count;
		len2 = list2.Count;
		for(i=0; i<len1; i++)  
		{  
			isSame = false;
			temp1 = list1[i];
			for(j=0; j<len2; j++)  
			{  
				temp2 = list2[j];
				if (temp1.Equals(temp2)) {
					//排除
					isSame = true;
					break;
				}
			}
			if (!isSame) {
				list1.Add (temp1);
			}
		}
		return list1;
	}

	///打印所有数组内容
	public static string arrayToString<T>(T[] arr)  
	{  
		int len = arr.Length;
		string str = len + ",[";
		for(int i=0; i<len; i++)  
		{  
			str += arr [i].ToString ();
			if(i!=len-1)
				str += ",";
		}
		str += "]";
		return str;
	} 	
	///打印所有数组内容
	public static string listToString<T>(List<T> list)  
	{  
		int len = list.Count;
		string str = "len:" + len + ",[";
		for(int i=0; i<len; i++)  
		{  
			str += list [i].ToString ();
			if(i!=len-1)
				str += ",";
		}
		str += "]";
		return str;
	} 	
}


