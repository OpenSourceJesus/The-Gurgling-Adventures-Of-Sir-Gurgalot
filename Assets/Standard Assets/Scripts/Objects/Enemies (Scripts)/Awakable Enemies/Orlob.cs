using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;

namespace TAoKR
{
	public class Orlob : AwakableEnemy
	{
		public Animator anim;
		public SpriteRenderer spriteRenderer;
		[HideInInspector]
		public float move;
		[MakeConfigurable]
		public float slowDownDist;
		
		public override void Update ()
		{
			base.Update ();
			if (!awakened || Time.timeScale == 0)
				return;
			HandleMovement ();
			spriteRenderer.flipX = rigid.velocity.x < 0;
			HandleAnimation ();
		}
		
		public virtual void HandleMovement ()
		{
			move = Mathf.Clamp(Player.instance.trs.position.x - trs.position.x, -slowDownDist, slowDownDist) * moveSpeed * (1f / slowDownDist);
			rigid.velocity = rigid.velocity.SetX(move);
		}
		
		public virtual void HandleAnimation ()
		{
			anim.SetFloat("speed", Mathf.Abs(rigid.velocity.x) / moveSpeed * Mathf.Sign((float) spriteRenderer.flipX.GetHashCode() - .5f));
		}
		
		public override void Awaken ()
		{
			base.Awaken ();
			anim.enabled = true;
		}
	}
}