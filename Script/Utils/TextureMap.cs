using UnityEngine;
using System.Collections.Generic;

public class TextureMap : MonoBehaviour
{
	private List<Texture2D> list;

	public TextureMap ()
	{
		list = new List<Texture2D> ();
	}

	public Texture2D GetTexture (string name)
	{		
		return list.Find ((Texture2D t) =>
		{
			return t.name == name;
		});
	}

	public void AddTexture (Texture2D t)
	{		
		list.Add (t);
	}
}
