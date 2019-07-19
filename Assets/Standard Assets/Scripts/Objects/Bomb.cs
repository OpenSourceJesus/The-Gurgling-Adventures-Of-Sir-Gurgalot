using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	public class Bomb : Bullet
	{
		public Vector2 destination;
		public Animator anim;
		[MakeConfigurable]
		public float range;
		bool atDestination;
		public DestroyAfterTime destroyerAfterTime;
		public AnimationClip explodeAnimClip;
		[MakeConfigurable]
		public float explodeDelay;

		public override void Start ()
		{
			anim.speed = explodeAnimClip.length / destroyerAfterTime.time;
		}

		public virtual void Update ()
		{
			if (atDestination)
				return;
			trs.position += Vector3.ClampMagnitude((Vector3) destination - trs.position, moveSpeed * Time.deltaTime);
			atDestination = (Vector2) trs.position == destination;
			if (atDestination)
				StartCoroutine(Explode ());
		}

		public virtual IEnumerator Explode ()
		{
			yield return new WaitForSeconds(explodeDelay);
			destroyerAfterTime.enabled = true;
			anim.Play("Explode");
		}

		// public virtual void OnTransformParentChanged ()
		// {
		// 	destroyerAfterTime.enabled = false;
		// 	atDestination = false;
		// }
	}
}