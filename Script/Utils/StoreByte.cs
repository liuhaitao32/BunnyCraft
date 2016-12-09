using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class StoreByte {
	public static void Save(object data){
		FileInfo f = new FileInfo (Application.persistentDataPath+"/"+"meng52config.amf3byte");
		BinaryFormatter formatter = new BinaryFormatter(); 
		MemoryStream rems = new MemoryStream(); 
		formatter.Serialize(rems, data);
//		ByteArray bt = new ByteArray ();
//		bt.WriteObject (data);
		byte[] bytes = rems.GetBuffer(); //bt.ToArray ();
		FileStream ft;
		if (!f.Exists) {
			ft = f.Create ();
		} else {
			f.Delete ();
			ft = f.Open (FileMode.OpenOrCreate);//f.OpenWrite ();
		}
		if (ft != null && bytes!=null) {
			ft.Position = 0;
			ft.Write (bytes, 0, bytes.Length);
			ft.Close ();
			ft.Dispose ();
		}
		rems.Close ();
		rems.Dispose ();
	}
	public static object Load(){
		FileStream ft = null;
		try{
			ft = File.Open (Application.persistentDataPath + "/" + "meng52config.amf3byte", FileMode.Open);
		}catch(Exception error){
			return null;
		}
		object data = null;

		if (ft != null) {
			byte[] bytes = new byte[ft.Length];
//			ft.Read (bytes, 0, bytes.Length);

			ft.Position = 0;
			while (ft.Position < ft.Length) {
				bytes [ft.Position] = Convert.ToByte (ft.ReadByte ());
			}
//			ByteArray bt = new ByteArray ();
//			bt.WriteBytes (bytes, 0, bytes.Length);
//			bt.Position = 0;
//			data = bt.ReadObject ();
			BinaryFormatter formatter = new BinaryFormatter(); 
			MemoryStream rems = new MemoryStream(bytes); 
//			data = null; 
			data =  formatter.Deserialize(rems);
			ft.Close ();
			ft.Dispose ();
		}
		return data;
	}
}
