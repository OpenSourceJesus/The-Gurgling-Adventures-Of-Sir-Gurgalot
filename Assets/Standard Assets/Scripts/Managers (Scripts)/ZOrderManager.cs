using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TAoKR
{
	[ExecuteAlways]
	public class ZOrderManager : SingletonMonoBehaviour<ZOrderManager>
	{
		public Renderer[] addToZOrderedObjects;
		public List<ZOrderedObject> zOrderedObjects;
		public const int MIN_Z_ORDER = -32768;
		public const int MAX_Z_ORDER = 32767;

		[Serializable]
		public class ZOrderedObject
		{
			public string objectName;
			public float orderController;
			public Renderer renderer;
			[Range(-32768, 32767)]
			public int order;
			public string layerName;
			public int layerIndex;
		}

		public virtual void OnValidate ()
		{
			ZOrderedObject zOrderedObject;
			foreach (Renderer renderer in addToZOrderedObjects)
			{
				zOrderedObject = new ZOrderedObject();
				zOrderedObject.objectName = renderer.name;
				zOrderedObject.order = renderer.sortingOrder;
				zOrderedObject.layerName = renderer.sortingLayerName;
				zOrderedObject.layerIndex = renderer.sortingLayerID;
				zOrderedObject.renderer = renderer;
				zOrderedObject.orderController = int.MaxValue;
				zOrderedObjects.Add(zOrderedObject);
			}
			addToZOrderedObjects = new Renderer[0];
			addToZOrderedObjects = null;
			zOrderedObjects.Sort((a, b) => ZOrderedObjectSorter(a, b));
			int previousLayerIndex = 0;
			int previousOrder = MIN_Z_ORDER;
			int previousOrderDiff = (MAX_Z_ORDER - MIN_Z_ORDER) / 2;
			for (int i = 0; i < zOrderedObjects.Count; i ++)
			{
				zOrderedObject = zOrderedObjects[i];
				zOrderedObject.orderController = i;
				zOrderedObject.order = previousOrder;
				if (zOrderedObject.layerIndex > previousLayerIndex)
				{
					previousLayerIndex = zOrderedObject.layerIndex;
					previousOrder = MIN_Z_ORDER;
					previousOrderDiff = (MAX_Z_ORDER - MIN_Z_ORDER) / 2;
				}
				else
				{
					previousOrder += previousOrderDiff;
					previousOrderDiff /= 2;
				}
				zOrderedObjects[i] = zOrderedObject;
			}
		}

		public virtual int ZOrderedObjectSorter (ZOrderedObject o1, ZOrderedObject o2)
		{
			int output = 0;
			if (o1.layerIndex > o2.layerIndex || o1.orderController > o2.orderController)
				output = 1;
			else if (o2.layerIndex > o1.layerIndex || o2.orderController > o1.orderController)
				output = -1;
			return output;
		}

		public virtual void ApplyChanges ()
		{
			foreach (ZOrderedObject zOrderedObject in zOrderedObjects)
			{
				zOrderedObject.renderer.sortingOrder = zOrderedObject.order;
				zOrderedObject.renderer.sortingLayerID = zOrderedObject.layerIndex;
			}
		}

#if UNITY_EDITOR
		[MenuItem("Z Order/Apply Changes")]
		public static void _ApplyChanges ()
		{
			GetInstance().ApplyChanges ();
		}
#endif
	}
}