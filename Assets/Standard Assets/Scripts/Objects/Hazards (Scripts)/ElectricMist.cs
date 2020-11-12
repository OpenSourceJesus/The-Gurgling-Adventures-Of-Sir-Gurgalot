using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
    public class ElectricMist : Hazard
    {
    	public SpriteRenderer eletricitySpriteRenderer;
    	public Animator anim;

    	public virtual void TurnOn ()
    	{
    		useDamage = true;
			anim.SetFloat("speed", 1);
    		anim.Play ("Turn Off");
    	}

    	public virtual void TurnOff ()
    	{
    		useDamage = false;
    	}

		public override void OnTriggerEnter2D (Collider2D collider)
		{
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("Turn Off"))
			{
				anim.SetFloat("speed", 0);
				TurnOn ();
			}
			else
			{
				anim.SetFloat("speed", 1);
				anim.Play("Turn On");
			}
		}
    }
}