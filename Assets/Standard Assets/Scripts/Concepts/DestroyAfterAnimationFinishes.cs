using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class DestroyAfterAnimationFinishes : MonoBehaviour
	{
		public Animation anim;
		bool hasStarted;
		
		void Update ()
		{
			if (hasStarted && !anim.isPlaying)
				Destroy(gameObject);
			else
				hasStarted = anim.isPlaying;
		}
	}
}