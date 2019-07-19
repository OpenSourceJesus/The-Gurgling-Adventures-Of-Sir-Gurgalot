using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	[ExecuteInEditMode]
	public class AddUniqueIdToName : MonoBehaviour
	{
	    public virtual void Start ()
		{
			int indexOfUniqueId = name.IndexOf(" (");
			if (indexOfUniqueId != - 1)
				name = name.Remove(indexOfUniqueId);
	    	name += " (" + Random.Range(0, int.MaxValue).ToString() + ")";
	    	DestroyImmediate(this);
	    }
	}
}