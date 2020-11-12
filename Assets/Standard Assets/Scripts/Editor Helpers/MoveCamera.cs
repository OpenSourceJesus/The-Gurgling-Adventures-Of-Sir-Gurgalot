#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ClassExtensions;

[ExecuteInEditMode]
public class MoveCamera : MonoBehaviour
{
	public Vector2 position;
	
	void LateUpdate ()
	{
		SceneView.currentDrawingSceneView.camera.transform.position = position.SetZ(SceneView.currentDrawingSceneView.camera.transform.position.z);
		SceneView.currentDrawingSceneView.MoveToView();
	}
}
#endif