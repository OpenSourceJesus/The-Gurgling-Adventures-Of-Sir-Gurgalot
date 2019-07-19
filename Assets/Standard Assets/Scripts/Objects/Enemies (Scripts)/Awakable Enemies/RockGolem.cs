using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class RockGolem : AwakableEnemy
	{
		float shootTimer;
		public Bullet bulletPrefab;
		public Transform shootPoint;
		public Animator anim;
		public CircleCollider2D escapeRange;
		[MakeConfigurable]
		public float EscapeRange
		{
			get
			{
				return escapeRange.bounds.extents.x;
			}
			set
			{
				escapeRange.radius = value * escapeRange.GetComponent<Transform>().lossyScale.x;
			}
		}
		Vector2 toPlayer;
		bool shooting;
		bool walking;
		
		public override void Update ()
		{
			base.Update ();
			if (!awakened)
				return;
			toPlayer = Player.instance.trs.position - trs.position;
			shootPoint.up = Vector3.right * Mathf.Sign(toPlayer.x);
			if (toPlayer.magnitude < escapeRange.bounds.extents.x)
			{
				rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
				rigid.velocity = new Vector2(-Mathf.Sign(toPlayer.x) * MoveSpeed, rigid.velocity.y);
				walking = true;
			}
			else
			{
				rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
				rigid.velocity = new Vector2(0, rigid.velocity.y);
				walking = false;
			}
			HandleAnimation ();
		}
		
		public virtual void HandleAnimation ()
		{
			if (shooting && walking)
			{
				if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Shoot+Walk"))
					anim.Play("Shoot+Walk", 0, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
			}
			else if (shooting)
			{
				if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
					anim.Play("Shoot", 0, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
			}
			else if (walking)
			{
				if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
					anim.Play("Walk", 0, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
			}
			else
			{
				if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
					anim.Play("Idle", 0, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
			}
		}
		
		public virtual void Shoot ()
		{
			ObjectPool.instance.Spawn(bulletPrefab, shootPoint.position, shootPoint.rotation);
		}
		
		public override void Awaken ()
		{
			base.Awaken ();
			shooting = true;
		}
	}
}