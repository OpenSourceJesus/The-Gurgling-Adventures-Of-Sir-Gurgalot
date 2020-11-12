using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class PassTriggerExitToHandlers : MonoBehaviour
	{
		public MonoBehaviour[] handlers;

		public void OnTriggerExit2D (Collider2D other)
		{
			foreach (MonoBehaviour handler in handlers)
			{
				ICollisionHandler collidable = handler as ICollisionHandler;
				collidable.OnTriggerExit2DHandler (other);
			}
		}
	}
}