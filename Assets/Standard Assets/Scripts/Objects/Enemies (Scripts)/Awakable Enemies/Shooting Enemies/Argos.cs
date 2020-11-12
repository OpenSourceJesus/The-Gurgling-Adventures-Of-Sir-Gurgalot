using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	public class Argos : ShootingEnemy
	{
		public Animator anim;
		public override bool BleedBoundsIsCircle
		{
			get
			{
				return true;
			}
		}
		Vector2 toPlayer;
		[MakeConfigurable]
		public float turnRate;
		[MakeConfigurable]
		public float facingPlayerAngleThreshold;
		
		public override void Update ()
		{
			if (!awakened)
				return;
			base.Update ();
			HandleMovement ();
			healthbar.parent.eulerAngles = Vector3.zero;
			foreach (ShootEntry shootEntry in shootEntries)
				HandleShooting (shootEntry);
		}
		
		public virtual void HandleMovement ()
		{
			toPlayer = Player.instance.trs.position - trs.position;
			trs.up = trs.up.RotateTo(toPlayer, turnRate * Time.deltaTime);
			if (Vector2.Angle(trs.up, toPlayer) <= facingPlayerAngleThreshold)
			{
				rigid.velocity = trs.up * moveSpeed;
				anim.Play("Fly");
			}
			else
				anim.Play("Idle");
		}
	}
}