using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;

[RequireComponent(typeof(EdgeCollider2D))]
[ExecuteInEditMode]
public class SnapEdgeCollider2D : MonoBehaviour
{
	public EdgeCollider2D edgeCollider;
	public Vector2 snapTo;
	public Vector2 offset;
	
	public virtual void Start ()
	{
		if (Application.isPlaying)
		{
			DestroyImmediate(this);
			return;
		}
		if (edgeCollider == null)
			edgeCollider = GetComponent<EdgeCollider2D>();
	}
	
	public virtual void Update ()
	{
		Vector2[] points = new Vector2[edgeCollider.pointCount];
		for (int i = 0; i < points.Length; i ++)
			points[i] = (Vector2) VectorExtensions.Snap(edgeCollider.points[i], snapTo) + offset;
		edgeCollider.points = points;
	}
}
