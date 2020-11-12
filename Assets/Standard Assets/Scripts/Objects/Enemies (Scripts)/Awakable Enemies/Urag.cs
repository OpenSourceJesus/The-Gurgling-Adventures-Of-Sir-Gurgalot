using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	[RequireComponent(typeof(Platformer))]
	public class Urag : AwakableEnemy
	{
		public Platformer platformer;
		float move;
		[MakeConfigurable]
		public float jumpVel;
		[MakeConfigurable]
		public float moveMultiplier;
		[MakeConfigurable]
		public float jumpCooldown;
		float jumpCooldownTimer;
		float toPlayerX;
		
		public override void Update ()
		{
			if (!awakened)
				return;
			platformer.UpdateWhatICanDo ();
			HandleJumping ();
			HandleMovement ();
		}
		
		public virtual void HandleJumping ()
		{
			if (platformer.hittingGround)
			{
				if (jumpCooldownTimer > 0)
					jumpCooldownTimer -= Time.deltaTime;
				if (jumpCooldownTimer <= 0)
					Jump ();
			}
		}
		
		public virtual void Jump ()
		{
			rigid.velocity = rigid.velocity.SetY(jumpVel);
			jumpCooldownTimer += jumpCooldown;
			toPlayerX = Player.instance.trs.position.x - trs.position.x;
		}
		
		public virtual void HandleMovement ()
		{
			if (platformer.hittingGround)
			{
				rigid.velocity = rigid.velocity.SetX(0);
				return;
			}
			move = Mathf.Clamp(toPlayerX * moveMultiplier, -moveSpeed, moveSpeed);
			//move = MathfExtensions.Sign(toPlayerX) * moveSpeed;
			rigid.velocity = rigid.velocity.SetX(move);
		}
	}
}