using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
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