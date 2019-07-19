using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	[ExecuteAlways]
    public class AcidFog : MonoBehaviour
    {
    	public Timer killTimer;
		public Animator anim;

    	public virtual void Start ()
    	{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (anim == null)
					anim = GetComponent<Animator>();
				return;
			}
#endif
    		killTimer.onFinished += delegate { Player.instance.Death(); };
    	}

    	public virtual void OnDestroy ()
    	{
    		killTimer.onFinished -= delegate { Player.instance.Death(); };
    	}

		public virtual void OnTriggerEnter2D (Collider2D collider)
		{
			anim.Play("Acid Fog");
			killTimer.timeRemaining = killTimer.duration;
			killTimer.Start ();
		}

		public virtual void OnTriggerExit2D (Collider2D collider)
		{
			anim.Play("Idle");
			killTimer.Stop ();
		}
    }
}