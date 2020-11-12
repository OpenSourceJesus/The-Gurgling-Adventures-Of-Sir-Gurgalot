using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;

namespace TAoKR
{
	[RequireComponent(typeof(Platformer))]
	public class Monjo : AwakableEnemy
	{
		[MakeConfigurable]
		public float speedUpRate;
		[MakeConfigurable]
		public float slowDownRate;
		Vector2 toPlayer;
		public SpriteRenderer spriteRenderer;
		float velX;
		public Platformer platformer;
		public float yDiffToPlayerTillJump;
		
		public override void Update ()
		{
			base.Update ();
			if (!awakened || Time.timeScale == 0)
				return;
			toPlayer = Player.instance.trs.position - trs.position;
			platformer.UpdateWhatICanDo ();
			HandleMovement ();
			HandleJumping ();
			spriteRenderer.flipX = rigid.velocity.x < 0;
		}
		
		public virtual void HandleMovement ()
		{
			velX = rigid.velocity.x;
			if (!MathfExtensions.AreOppositeSigns(toPlayer.x, rigid.velocity.x))
			{
				velX = Mathf.Clamp(rigid.velocity.x + Mathf.Sign(toPlayer.x) * speedUpRate * Time.deltaTime, -MoveSpeed, MoveSpeed);
				rigid.velocity = rigid.velocity.SetX(velX);
			}
			else
			{
				velX = Mathf.Clamp(rigid.velocity.x - Mathf.Sign(velX) * slowDownRate * Time.deltaTime, -MoveSpeed, MoveSpeed);
				rigid.velocity = rigid.velocity.SetX(velX);
			}
		}
		
		public virtual void HandleJumping ()
		{
			bool playerIsAbove = Player.instance.colliderRect.min.y - platformer.colliderRect.min.y > yDiffToPlayerTillJump;
			bool jumping = platformer.activityStatus[Activity.Jumping].state == ActivityState.Doing;
			bool canJump = platformer.activityStatus[Activity.Jumping].canDo;
			bool canMove = (toPlayer.x > 0 && platformer.hittingRight) || (toPlayer.x < 0 && platformer.hittingLeft);
			bool shouldStartJump = (playerIsAbove || !canMove) && !jumping && canJump;
			Debug.Log("playerIsAbove: " + playerIsAbove);
			Debug.Log("jumping: " + jumping);
			Debug.Log("canJump: " + canJump);
			Debug.Log("canMove: " + canMove);
			Debug.Log("shouldStartJump: " + shouldStartJump);
			if (rigid.velocity.y < 0)
			{
				rigid.velocity += Vector2.up * Physics2D.gravity.y * (platformer.fallMultiplier - 1) * Time.deltaTime;
				if (platformer.activityStatus[Activity.Jumping].state == ActivityState.Doing)
					platformer.StopJump ();
			}
			else
			{
				if (jumping)
				{
					if (!playerIsAbove && platformer.activityStatus[Activity.Falling].canDo && platformer.activityStatus[Activity.Falling].state != ActivityState.Doing)
						platformer.StopJump ();
				}
				else
					rigid.velocity += Vector2.up * Physics2D.gravity.y * (platformer.lowJumpMultiplier - 1) * Time.deltaTime;
			}
			if (shouldStartJump)
			{
				rigid.velocity += Vector2.up * platformer.jumpSpeed;
				platformer.StartJump ();
			}
		}
	}
}