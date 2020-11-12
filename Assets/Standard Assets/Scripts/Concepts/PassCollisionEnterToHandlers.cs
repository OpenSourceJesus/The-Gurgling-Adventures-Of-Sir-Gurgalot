using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class PassCollisionEnterToHandlers : MonoBehaviour
	{
		public MonoBehaviour[] handlers;

		public void OnCollisionEnter2D (Collision2D coll)
		{
			foreach (MonoBehaviour handler in handlers)
			{
				ICollisionHandler collidable = handler as ICollisionHandler;
				collidable.OnCollisionEnter2DHandler (coll);
			}
		}
	}
}