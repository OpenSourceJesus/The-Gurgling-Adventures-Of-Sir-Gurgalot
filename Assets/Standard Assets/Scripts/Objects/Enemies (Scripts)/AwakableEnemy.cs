using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class AwakableEnemy : Enemy
	{
		[HideInInspector]
		public bool awakened;
		public SpriteRenderer visionIndicator;
		[MakeConfigurable]
		public float VisionRadius
		{
			get
			{
				return visionIndicator.GetComponent<Transform>().localScale.x / 2;
			}
			set
			{
				visionIndicator.GetComponent<Transform>().localScale = Vector3.one * value * 2;
			}
		}
		
		public override void OnTriggerEnter2D (Collider2D other)
		{
			//base.OnTriggerEnter2D (other);
			if (Player.instance.rigid.simulated && other.gameObject == Player.instance.gameObject)
				Awaken ();
		}
		
		public virtual void Awaken ()
		{
			awakened = true;
			visionIndicator.enabled = false;
		}
		
		public override void TakeDamage (float amount)
		{
			base.TakeDamage (amount);
			Awaken ();
		}
	}
}