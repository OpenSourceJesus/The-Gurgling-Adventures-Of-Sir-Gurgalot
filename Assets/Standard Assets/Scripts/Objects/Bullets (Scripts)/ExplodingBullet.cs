using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;

namespace TAoKR
{
	public class ExplodingBullet : Bullet
	{
		public Animator anim;
		public DestroyAfterTime destroyerAfterTime;
		public AnimationClip explodeAnimClip;
		public LayerMask whatExplodesMe;

		public override void Start ()
		{
			base.Start ();
			anim.speed = explodeAnimClip.length / destroyerAfterTime.time;
		}

		public override void OnTriggerEnter2D (Collider2D other)
		{
			if (!LayerMaskExtensions.MaskContainsLayer(whatExplodesMe, other.gameObject.layer))
				return;
			base.OnTriggerEnter2D (other);
			collider.enabled = false;
			enabled = false;
			rigid.isKinematic = true;
			rigid.velocity = Vector2.zero;
			anim.Play("Explode");
			destroyerAfterTime.enabled = true;
		}

		// public virtual void OnTransformParentChanged ()
		// {
		// 	destroyerAfterTime.enabled = false;
		// 	rigid.isKinematic = false;
		// 	collider.enabled = true;
		// 	enabled = true;
		// }
	}
}