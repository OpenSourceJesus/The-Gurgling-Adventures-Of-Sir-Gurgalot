using UnityEngine;
using System.Collections;
using Extensions;

namespace TGAOSG
{
	[ExecuteInEditMode]
	public class SetPosition : MonoBehaviour
	{
		public Vector3 position;
		public bool xIsLocal;
		public bool yIsLocal;
		public bool zIsLocal;
		public Transform trs;
		
		void Start ()
		{
			//if (Application.isPlaying)
			//	DestroyImmediate(this);
			if (trs == null)
				trs = GetComponent<Transform>();
		}
		
		void Update ()
		{
			if (xIsLocal)
				trs.localPosition = trs.localPosition.SetX(position.x);
			else
				trs.position = trs.position.SetX(position.x);
			if (yIsLocal)
				trs.localPosition = trs.localPosition.SetY(position.y);
			else
				trs.position = trs.position.SetY(position.y);
			if (zIsLocal)
				trs.localPosition = trs.localPosition.SetZ(position.z);
			else
				trs.position = trs.position.SetZ(position.z);
		}
	}
}
