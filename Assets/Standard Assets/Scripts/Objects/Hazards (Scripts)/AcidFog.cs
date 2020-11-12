using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
    public class AcidFog : MonoBehaviour
    {
    	public Timer killTimer;

    	public virtual void Start ()
    	{
    		killTimer.onFinished += delegate { Player.instance.Death(); };
    	}

    	public virtual void OnDestroy ()
    	{
    		killTimer.onFinished -= delegate { Player.instance.Death(); };
    	}

		public virtual void OnTriggerEnter2D (Collider2D collider)
		{
			killTimer.timeRemaining = killTimer.duration;
			killTimer.Start ();
		}

		public virtual void OnTriggerExit2D (Collider2D collider)
		{
			killTimer.Stop ();
		}
    }
}