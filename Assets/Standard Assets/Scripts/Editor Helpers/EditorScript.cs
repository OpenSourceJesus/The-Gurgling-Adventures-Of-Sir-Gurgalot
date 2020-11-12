#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class EditorScript : MonoBehaviour
{
	public EditorScriptEditor.Hotkey activateHotkey;

	public virtual void OnEnable ()
	{
		EditorApplication.update += DoEditorUpdate;
	}

	public virtual void OnDisable ()
	{
		EditorApplication.update -= DoEditorUpdate;
	}

	public virtual void DoEditorUpdate ()
	{
	}

	public virtual void Activate ()
	{
	}
}
#endif