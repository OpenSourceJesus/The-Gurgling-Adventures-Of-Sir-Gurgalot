#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MakeSpriteFromCollider2D))]
public class MakeSpriteFromCollider2DEditor : EditorScriptEditor
{
}
#endif