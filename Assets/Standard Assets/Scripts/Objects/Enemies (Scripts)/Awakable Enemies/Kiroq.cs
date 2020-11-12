using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	[RequireComponent(typeof(Animator))]
	public class Kiroq : AwakableEnemy
	{
		public LayerMask whatIAwaken;
		public Animator anim;
		public CircleCollider2D alertRange;
		
		public override void OnEnable ()
		{
		}

		public override void Awaken ()
		{
			base.Awaken ();
			anim.enabled = true;
			foreach (Collider2D collider in Physics2D.OverlapCircleAll(alertRange.bounds.center, alertRange.bounds.extents.x, whatIAwaken))
			{
				AwakableEnemy enemy = collider.GetComponentInParent<AwakableEnemy>();
				if (!enemy.awakened && collider != this.collider)
					enemy.Awaken ();
			}
		}
		
		public override void Update ()
		{
		}
		
		public override void Death ()
		{
			gameObject.SetActive(false);
			if (triggerOnDeath != null)
				triggerOnDeath.Trigger ();
		}
	}
}
