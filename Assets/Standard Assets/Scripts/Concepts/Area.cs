using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.Tilemaps;

namespace TAoKR
{
	[ExecuteAlways]
	public class Area : SingletonMonoBehaviour<Area>
	{
		public bool update;
		public Rect visibleRect;
		public RectOffset invisibleBorder;
		public Rect safeRect;
		public Color backgroundColor;
		
		public override void Start ()
		{
			base.Start ();
			#if UNITY_EDITOR
			if (GameplayCamera.GetInstance() != null)
			#endif
			GameplayCamera.GetInstance().camera.backgroundColor = backgroundColor;
		}
		
		public virtual void CalculateBounds ()
		{
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			Rect[] rendererRects = new Rect[renderers.Length];
			for (int i = 0; i < renderers.Length; i ++)
				rendererRects[i] = renderers[i].bounds.ToRect();
			visibleRect = RectExtensions.Combine(rendererRects);
			safeRect = visibleRect;
			visibleRect.min += new Vector2(invisibleBorder.left, invisibleBorder.bottom);
			visibleRect.max -= new Vector2(invisibleBorder.right, invisibleBorder.top);
		}
		
		#if UNITY_EDITOR
		public virtual void Update ()
		{
			if (Application.isPlaying || !update)
				return;
			foreach (Tilemap tilemap in GetComponentsInChildren<Tilemap>())
				tilemap.CompressBounds ();
			CalculateBounds ();
		}
		#endif
	}
}