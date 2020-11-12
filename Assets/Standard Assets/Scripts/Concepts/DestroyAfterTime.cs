using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class DestroyAfterTime : MonoBehaviour
	{
		public float time;
		
		public virtual void Start ()
		{
			Destroy(gameObject, time);
		}
	}
}