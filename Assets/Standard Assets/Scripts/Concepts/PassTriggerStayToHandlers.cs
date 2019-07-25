using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class PassTriggerStayToHandlers : MonoBehaviour
	{
		public MonoBehaviour[] handlers;

		public void OnTriggerStay2D (Collider2D other)
		{
			foreach (MonoBehaviour handler in handlers)
			{
				ICollisionHandler collidable = handler as ICollisionHandler;
				collidable.OnTriggerStay2DHandler (other);
			}
		}
	}
}