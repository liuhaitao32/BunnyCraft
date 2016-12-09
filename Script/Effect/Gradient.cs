using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

///渐变上色
[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
	[SerializeField]
	private Color32 topColor = Color.white;
	[SerializeField]
	private Color32 bottomColor = Color.black;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!this.IsActive())
			return;

		List<UIVertex> vertexList = new List<UIVertex>();
		vh.GetUIVertexStream(vertexList);

		ModifyVertices(vertexList);

		vh.Clear();
		vh.AddUIVertexTriangleStream(vertexList);
	}

	public void ModifyVertices(List<UIVertex> vertexList)
	{
		if (!IsActive())
		{
			return;
		}

		int count = vertexList.Count;
		if (count > 0)
		{
			UIVertex uiVertex;
			uiVertex = vertexList [0];

			float bottomY = uiVertex.position.y;
			float topY = bottomY;

			for (int i = 1; i < count; i++)
			{
				float y = vertexList[i].position.y;
				if (y > topY)
				{
					topY = y;
				}
				else if (y < bottomY)
				{
					bottomY = y;
				}
			}

			float uiElementHeight = topY - bottomY;
		

			for (int i = 0; i < count; i++)
			{
				uiVertex = vertexList[i];
				uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - bottomY) / uiElementHeight);
				vertexList[i] = uiVertex;
			}
		}
	}
}