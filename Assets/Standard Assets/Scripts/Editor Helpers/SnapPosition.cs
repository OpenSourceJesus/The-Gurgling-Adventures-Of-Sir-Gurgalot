using UnityEngine;
using System.Collections;
using Extensions;

[ExecuteInEditMode]
public class SnapPosition : MonoBehaviour
{
	public Vector3 snapTo;
	public Vector3 offset;
	public Transform trs;
	Vector3 newPos;
	public bool useLocalPosition = true;
	
	public virtual void Start ()
	{
		if (Application.isPlaying)
			DestroyImmediate(this);
		if (trs == null)
			trs = GetComponent<Transform>();
	}
	
	public virtual void Update ()
	{
		if (useLocalPosition)
		{
			newPos = VectorExtensions.Snap(trs.localPosition, snapTo) + offset;
			if (snapTo.x == 0)
				newPos.x = trs.localPosition.x;
			if (snapTo.y == 0)
				newPos.y = trs.localPosition.y;
			if (snapTo.z == 0)
				newPos.z = trs.localPosition.z;
				trs.localPosition = newPos;
		}
		else
		{
			newPos = VectorExtensions.Snap(trs.position, snapTo) + offset;
			if (snapTo.x == 0)
				newPos.x = trs.position.x;
			if (snapTo.y == 0)
				newPos.y = trs.position.y;
			if (snapTo.z == 0)
				newPos.z = trs.position.z;
			trs.position = newPos;
		}
	}
}
