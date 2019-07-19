using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class DestroyIfMovingSlowForFrames : MonoBehaviour
	{
		public Rigidbody2D rigid;
		public float minSpeed;
		public int frameCount = 1;
		int framesRemaining;
		
		void Start ()
		{
			framesRemaining = frameCount;
		}
		
		void Update ()
		{
			if (rigid.velocity.magnitude < minSpeed)
			{
				framesRemaining --;
				if (framesRemaining == 0)
					Destroy(gameObject);
			}
			else
			{
				framesRemaining = frameCount;
			}
		}
	}
}