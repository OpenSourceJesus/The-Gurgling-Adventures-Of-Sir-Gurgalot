#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(EditorScript))]
public class EditorScriptEditor : Editor
{
	public virtual void OnSceneGUI ()
	{
		EditorScript editorScript = (EditorScript) target;
		if (editorScript.activateHotkey.previousKeys != (KeyCode[]) editorScript.activateHotkey.keys.Clone())
			editorScript.activateHotkey.isPressingKeys = new bool[editorScript.activateHotkey.keys.Length];
		for (int i = 0; i < editorScript.activateHotkey.keys.Length; i ++)
		{
			if (Event.current != null && Event.current.keyCode == editorScript.activateHotkey.keys[i])
			{
				if (Event.current.type == EventType.KeyDown)
				{
					editorScript.activateHotkey.isPressingKeys[i] = true;
					foreach (bool isPressingKey in editorScript.activateHotkey.isPressingKeys)
					{
						if (!isPressingKey)
							break;
					}
					editorScript.Activate ();
				}
				else if (Event.current.type == EventType.KeyUp)
					editorScript.activateHotkey.isPressingKeys[i] = false;
			}
		}
		editorScript.activateHotkey.previousKeys = (KeyCode[]) editorScript.activateHotkey.keys.Clone();
	}

	[Serializable]
	public class Hotkey
	{
		public KeyCode[] keys;
		[HideInInspector]
		public KeyCode[] previousKeys = new KeyCode[0];
		[HideInInspector]
		public bool[] isPressingKeys;
	}
}
#endif