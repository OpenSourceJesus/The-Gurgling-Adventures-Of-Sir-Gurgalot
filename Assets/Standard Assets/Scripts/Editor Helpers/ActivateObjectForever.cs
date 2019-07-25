using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class ActivateObjectForever : MonoBehaviour
	{
		public virtual void OnDisable ()
		{
		}
		
		public virtual void OnEnable ()
		{
			if (!enabled)
				return;
			GameManager.instance.ActivateGoForever(gameObject);
		}
	}
}