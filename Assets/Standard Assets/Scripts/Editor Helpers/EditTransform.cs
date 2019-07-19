using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditTransform : MonoBehaviour
{
	public Transform trs;
	public bool updateTrs;
	public bool saveData;
	public float scaleMultiplier = 1;
	public List<TransformInfo> previousTrsInfos = new List<TransformInfo>();
	public bool undo;
	
	void Start ()
	{
		if (trs == null)
			trs = GetComponent<Transform>();
	}
	
	void Update ()
	{
		if (undo)
		{
			undo = false;
			trs.localScale = previousTrsInfos[previousTrsInfos.Count - 1].localScale;
			previousTrsInfos.RemoveAt(previousTrsInfos.Count - 1);
			return;
		}
		if (saveData)
		{
			saveData = false;
			previousTrsInfos.Add(new TransformInfo(trs.localScale));
		}
		if (updateTrs)
		{
			if (previousTrsInfos.Count > 0)
				trs.localScale = previousTrsInfos[previousTrsInfos.Count - 1].localScale * scaleMultiplier;
		}
	}
	
	public class TransformInfo
	{
		public Vector3 localScale;
		
		public TransformInfo (Vector3 localScale)
		{
			this.localScale = localScale;
		}
	}
}
