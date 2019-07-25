using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class PassTriggerEnterToHandlers : MonoBehaviour
	{
		public MonoBehaviour[] handlers;

		public void OnTriggerEnter2D (Collider2D other)
		{
			foreach (MonoBehaviour handler in handlers)
			{
				ICollisionHandler collidable = handler as ICollisionHandler;
				collidable.OnTriggerEnter2DHandler (other);
			}
		}
	}
}