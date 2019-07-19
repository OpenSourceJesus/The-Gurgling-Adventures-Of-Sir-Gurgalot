using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetFacingOfObjects : MonoBehaviour
{
	public bool update;
	public float arcDegrees;
	public Transform parent;
	
	void Update ()
	{
		if (!update)
			return;
		update = false;
		int i = 0;
		float rota = arcDegrees / parent.childCount;
		for (float a = -arcDegrees / 2; a < arcDegrees / 2; a += rota)
		{
			parent.GetChild(i).localEulerAngles = Vector3.forward * a;
			i ++;
		}
	}
}
