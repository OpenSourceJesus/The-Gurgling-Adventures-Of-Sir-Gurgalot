using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TGAOSG;

[ExecuteAlways]
[RequireComponent(typeof(Canvas))]
[DisallowMultipleComponent]
public class _Canvas : MonoBehaviour
{
	public Canvas canvas;
	
	public virtual void OnEnable ()
	{
		#if UNITY_EDITOR
		if (canvas == null)
			canvas = GetComponent<Canvas>();
		#endif
		if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
			canvas.worldCamera = CameraScript.instance.camera;
	}
}
