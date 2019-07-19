using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class DeactivateObjectForever : MonoBehaviour
	{
		void OnDisable ()
		{
		}
		
		void Awake ()
		{
			if (!enabled)
				return;
			GameManager.instance.DeactivateGoForever(gameObject);
		}
	}
}