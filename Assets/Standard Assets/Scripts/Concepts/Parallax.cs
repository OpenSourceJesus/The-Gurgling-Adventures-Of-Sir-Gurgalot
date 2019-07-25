using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	[ExecuteAlways]
	public class Parallax : MonoBehaviour
	{
		#if UNITY_EDITOR
		public bool updateInEditMode;
		#endif
		public Transform trs;
		public float moveRate;
		Vector3 previousCameraPos;
		
		public virtual void Start ()
		{
			previousCameraPos = GameplayCamera.instance.trs.position;
		}
		
		public virtual void Update ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying && !updateInEditMode)
				return;
			#endif
			trs.position += (GameplayCamera.instance.trs.position - previousCameraPos) * moveRate;
			previousCameraPos = GameplayCamera.instance.trs.position;
		}
	}
}