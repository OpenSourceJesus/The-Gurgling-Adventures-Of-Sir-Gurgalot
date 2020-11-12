using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class RegisterObject : MonoBehaviour
	{
		public virtual void OnDisable ()
		{
		}
		
		public virtual void Awake ()
		{
			if (!enabled)
				return;
			GameManager.GetInstance().registeredGos.Add(gameObject);
		}
	}
}