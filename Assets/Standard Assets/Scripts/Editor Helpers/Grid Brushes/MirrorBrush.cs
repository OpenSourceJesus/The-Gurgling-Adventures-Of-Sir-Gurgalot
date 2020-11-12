#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ClassExtensions;

namespace UnityEditor
{
	[CustomGridBrush(true, false, false, "Mirror Brush")]
	public class MirrorBrush : UnityEditor.Tilemaps.GridBrush
	{
		public Vector3 origin;
		public bool mirrorX;
		public bool mirrorY;
		public bool mirrorZ;
		public bool radialSymmetry;
		
		public override void Paint (GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			base.Paint(grid, brushTarget, position);
			Vector3 toOrigin = origin - position;
			if (radialSymmetry)
			{
				base.Paint(grid, brushTarget, position + (toOrigin * 2).ToVec3Int(MathfExtensions.RoundingMethod.HalfOrLessRoundsDown));
			}
			if (mirrorX)
			{
				base.Paint(grid, brushTarget, position + Vector3Int.right * (toOrigin * 2).ToVec3Int(MathfExtensions.RoundingMethod.HalfOrLessRoundsDown).x);
			}
			if (mirrorY)
			{
				base.Paint(grid, brushTarget, position + Vector3Int.up * (toOrigin * 2).ToVec3Int(MathfExtensions.RoundingMethod.HalfOrLessRoundsDown).y);
			}
			if (mirrorZ)
			{
				base.Paint(grid, brushTarget, position + new Vector3Int(0, 0, 1) * (toOrigin * 2).ToVec3Int(MathfExtensions.RoundingMethod.HalfOrLessRoundsDown).z);
			}
		}
		
		[MenuItem("Assets/Create/Line Brush")]
		public static void CreateBrush ()
		{
			string path = EditorUtility.SaveFilePanelInProject("Save Mirror Brush", "New Mirror Brush", "Asset", "Save Mirror Brush", "Assets");
			if (path == "")
				return;
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MirrorBrush>(), path);
		}
	}
	[CustomEditor(typeof(MirrorBrush))]
	public class MirrorBrushEditor : UnityEditor.Tilemaps.GridBrushEditor
	{
		private MirrorBrush mirrorBrush { get { return target as MirrorBrush; } }
		public override void OnPaintSceneGUI (GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
		{
			base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
		}
	}
}
#endif