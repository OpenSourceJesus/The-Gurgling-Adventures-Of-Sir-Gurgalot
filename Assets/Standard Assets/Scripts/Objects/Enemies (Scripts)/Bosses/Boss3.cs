using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	public class Boss3 : Boss
	{
		Vector2 heading;
		Vector2 toPlayer;
		[MakeConfigurable]
		public float turnRate;
		[MakeConfigurable]
		public float maxFacingOffsetToStartCharge;
		[MakeConfigurable]
		public float chargeSpeedModifier;
		[MakeConfigurable]
		public float chargeTurnRateModifier;
		bool charging;
		float facingOffset;
		public Transform waypointsParent;
		Vector2 toWaypoint;
		[MakeConfigurable]
		public float stopDistFromWaypoint;
		[MakeConfigurable]
		public float shootFlameDelay;
		public override bool BleedBoundsIsCircle
		{
			get
			{
				return true;
			}
		}
		public new SpriteRenderer renderer;
		public override float InvulnerableFlashValue
		{
			get
			{
				return invulnerableFlashValue;
			}
			set
			{
				invulnerableFlashValue = value;
				renderer.color = ColorExtensions.SetAlpha(renderer.color, value);
			}
		}
		
		public override void OnEnable ()
		{
			base.OnEnable ();
			healthbar.parent.gameObject.SetActive(true);
			if (partIndex == 1)
				StartCoroutine(RotateToPlayer ());
			else if (partIndex == 2)
				StartCoroutine(TravelToWaypoint ());
			if (nextPart != null)
			{
				Transform nextWaypointsParent = ((Boss3) nextPart).waypointsParent;
				if (nextWaypointsParent != null)
					nextWaypointsParent.SetParent(null);
			}
		}
		
		public override void Update ()
		{
			//base.Update ();
			if (partIndex == 1)
			{
				if (!charging && facingOffset <= maxFacingOffsetToStartCharge)
					StartCharge ();
				else if (charging && facingOffset > maxFacingOffsetToStartCharge)
					StopCharge ();
			}
			if (charging)
				Charge ();
		}
		
		public virtual void StartCharge ()
		{
			charging = true;
			moveSpeed += chargeSpeedModifier;
			turnRate -= chargeTurnRateModifier;
		}
		
		public virtual void Charge ()
		{
			rigid.velocity = heading.normalized * moveSpeed;
		}
		
		public virtual void StopCharge ()
		{
			charging = false;
			moveSpeed -= chargeSpeedModifier;
			turnRate += chargeTurnRateModifier;
		}
		
		public virtual IEnumerator TravelToWaypoint ()
		{
			// anim.Play();
			while (true)
			{
				toWaypoint = waypointsParent.position - trs.position;
				heading = trs.right * trs.localScale.x;
				facingOffset = Vector2.Angle(toWaypoint, heading);
				if (facingOffset > maxFacingOffsetToStartCharge)
				{
					RotateTo (toWaypoint);
					if (charging)
						StopCharge ();
				}
				else
				{
					if (!charging && toWaypoint.magnitude > stopDistFromWaypoint)
						StartCharge ();
					else if (toWaypoint.magnitude <= stopDistFromWaypoint)
					{
						if (charging)
							StopCharge ();
						StartCoroutine(HandleShootingFlame ());
						StartCoroutine(RotateToPlayer ());
						yield break;
					}
				}
				yield return new WaitForEndOfFrame();
			}
		}
		
		public virtual IEnumerator HandleShootingFlame ()
		{
			while (true)
			{
				ShootFlame ();
				yield return new WaitUntil(() => (!anim.GetCurrentAnimatorStateInfo(0).IsName("Blow Fire")));
				yield return new WaitForSeconds(shootFlameDelay);
			}
		}
		
		public virtual IEnumerator RotateToPlayer ()
		{
			while (true)
			{
				heading = trs.right * trs.localScale.x;
				toPlayer = Player.instance.trs.position - trs.position;
				facingOffset = Vector2.Angle(toPlayer, heading);
				RotateTo (toPlayer);
				yield return new WaitForEndOfFrame();
			}
		}
		
		public virtual void ShootFlame ()
		{
			anim.Play("Blow Fire");
		}
		
		public virtual void RotateTo (Vector2 dir)
		{
			rigid.angularVelocity = Mathf.Clamp(Mathf.DeltaAngle(heading.GetFacingAngle(), dir.GetFacingAngle()), -1, 1) * turnRate * Time.timeScale;
		}
	}
}