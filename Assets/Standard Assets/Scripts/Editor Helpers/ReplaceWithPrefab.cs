// #if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReplaceWithPrefab : MonoBehaviour
{
	public Transform trs;
	public Transform prefabTrs;

	void Start ()
	{
		if (Application.isPlaying)
			DestroyImmediate(this);
		if (trs == null)
			trs = GetComponent<Transform>();
	}

	void Update ()
	{
		if (prefabTrs != null)
		{
			Transform spawnedTrs = Instantiate(prefabTrs, trs.position, trs.rotation, trs.parent);
			spawnedTrs.localScale = trs.localScale;
			spawnedTrs.name = name;
			DestroyImmediate(gameObject);
		}
	}
}
// #endif