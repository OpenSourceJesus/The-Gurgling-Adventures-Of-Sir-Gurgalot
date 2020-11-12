using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;
using TAoKR.SkillTree;

namespace TAoKR
{
	public class MagicCape : Item
	{
		public new static MagicCape instance;
		[MakeConfigurable]
		public float moveSpeed;
		[MakeConfigurable]
		public float distance;
		[MakeConfigurable]
		public float glidingFallRate;
		public static bool isDashing;

		void Update ()
		{
			if (InputManager.inputter.GetButtonDown("Dash") && cooldownTimer <= 0)
			{
				StartCoroutine(Cooldown ());
				StartCoroutine(Dash ());
			}
			if (MagicCapeCanGlide.instance.learned && Player.instance.activityStatus[Activity.Falling].state == ActivityState.Doing)
			{
				if (InputManager.inputter.GetButton("Jump"))
				{
					Vector2 velocity = Player.instance.rigid.velocity;
					if (velocity.y <= -glidingFallRate)
					{
						velocity.y = -glidingFallRate;
						Player.instance.rigid.gravityScale = 0;
						Player.instance.rigid.velocity = velocity;
					}
					else
						Player.instance.rigid.gravityScale = Player.instance.defaultGravityScale;
				}
				else if (InputManager.inputter.GetButtonUp("Jump"))
					Player.instance.rigid.gravityScale = Player.instance.defaultGravityScale;
			}
			else
				Player.instance.rigid.gravityScale = Player.instance.defaultGravityScale;
		}
		
		IEnumerator Dash ()
		{
			isDashing = true;
			float remainingDistance = distance;
			Vector2 addToVel;
			if (Player.instance.move != 0)
				addToVel = Vector2.right * Mathf.Sign(Player.instance.move) * moveSpeed;
			else
				addToVel = Vector2.right * Mathf.Sign(Player.instance.trs.localScale.x) * moveSpeed;
			Player.instance.addToVel.Add(addToVel);
			while (true)
			{
				remainingDistance -= moveSpeed * Time.deltaTime;
				if (remainingDistance <= 0 || Player.instance.hittingWall)
					break;
				yield return new WaitForEndOfFrame();
			}
			Player.instance.addToVel.Remove(addToVel);
			isDashing = false;
		}

		void OnDestroy ()
		{
			isDashing = false;
		}
	}
}