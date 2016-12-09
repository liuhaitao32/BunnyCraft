using System;

public class VoPackage
{
	public static int HEAD_LEN = 4;
	public int bodyLen = 0;
	public int readLen = 0;
	public byte[] body;

	public bool isClear = false;

	public VoPackage (int len)
	{
		this.bodyLen = len;
		body = new byte[bodyLen];
	}

	public void Clear ()
	{
		isClear = true;
		Array.Clear (body, 0, body.Length);
	}
}