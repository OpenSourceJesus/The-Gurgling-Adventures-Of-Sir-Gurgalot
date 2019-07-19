using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraScript : SingletonMonoBehaviour<CameraScript>
	{
		public Transform trs;
		public new Camera camera;
		public Vector2 viewSize;
		protected Rect normalizedScreenViewRect;
		protected float screenAspect;
		
		public override void Start ()
		{
			base.Start ();
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				if (camera == null)
					camera = GetComponent<Camera>();
				return;
			}
			#endif
			Player.GetInstance();
			HandlePosition ();
			HandleViewSize ();
		}
		
		public virtual void LateUpdate ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
			HandlePosition ();
		}
		
		public virtual void HandlePosition ()
		{
			
		}
		
		public virtual void HandleViewSize ()
		{
			screenAspect = (float) Screen.width / Screen.height;
			camera.aspect = viewSize.x / viewSize.y;
			camera.orthographicSize = Mathf.Max(viewSize.x / 2 / camera.aspect, viewSize.y / 2);
			normalizedScreenViewRect = new Rect();
			normalizedScreenViewRect.size = new Vector2(camera.aspect / screenAspect, Mathf.Min(1, screenAspect / camera.aspect));
			normalizedScreenViewRect.center = Vector2.one / 2;
			camera.rect = normalizedScreenViewRect;
		}
	}
}