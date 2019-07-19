using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	[ExecuteInEditMode]
	public class SetZOrder : MonoBehaviour
	{
		public bool useLayerId;
		public ushort layerId;
		public string layerName;
		[Range(-32768, 32767)]
		public int order;
		public new Renderer renderer;

		void OnEnable ()
		{
			if (!useLayerId)
				renderer.sortingLayerName = layerName;
			else
				renderer.sortingLayerID = layerId;
			renderer.sortingOrder = order;
		}
	}
}