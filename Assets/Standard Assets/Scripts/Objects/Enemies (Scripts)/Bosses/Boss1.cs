using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	public class Boss1 : Boss
	{
		public Transform waypointsParent;
		Vector2 toWaypoint;
		Transform waypoint;
		[MakeConfigurable]
		public float shootRate;
		[MakeConfigurable]
		public float firstShotDelay;
		float shootTimer;
		bool preparingShot;
		public Transform shootPoint;
		public Bullet bulletPrefab;
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
				renderer.color = renderer.color.SetAlpha(value);
			}
		}
		public override bool BleedBoundsIsCircle
		{
			get
			{
				return true;
			}
		}
		public int currentWaypoint;
		
		public override void OnEnable ()
		{
			base.OnEnable ();
			shootTimer = shootRate + firstShotDelay;
			waypointsParent.SetParent(null);
		}
		
		public override void Update ()
		{
			//base.Update ();
			if (partIndex >= 2)
				trs.up = Player.instance.trs.position - trs.position;
			HandleShooting ();
			if (currentWaypoint >= waypointsParent.childCount)
				return;
			waypoint = waypointsParent.GetChild(currentWaypoint);
			//if (!waypoint.gameObject.activeSelf)
			//	return;
			toWaypoint = waypoint.position - trs.position;
			if (toWaypoint.magnitude == 0)
			{
				if (partIndex == 3)
					waypoint.SetSiblingIndex(waypoint.GetSiblingIndex() + 1);
				else
					currentWaypoint ++;
			}
			else
				trs.position = Vector2.Lerp(trs.position, waypoint.position, moveSpeed * (1f / toWaypoint.magnitude) * Time.deltaTime);
		}
		
		public virtual void HandleShooting ()
		{
			shootTimer -= Time.deltaTime;
			if (!preparingShot && shootTimer <= anim.GetCurrentAnimatorStateInfo(0).length)
			{
				anim.Play ("Prepare Shoot");
				preparingShot = true;
			}
			else if (preparingShot && !anim.GetCurrentAnimatorStateInfo(0).IsName("Prepare Shoot"))
			{
				Shoot ();
				shootTimer = shootRate;
				preparingShot = false;
			}
		}
		
		public virtual void Shoot ()
		{
			shootPoint.up = trs.up;
			ObjectPool.instance.Spawn(bulletPrefab, shootPoint.position, shootPoint.rotation);
		}

		public override void Death ()
		{
			if (nextPart != null)
			{
				nextPart.trs.SetParent(null); // Need this line even if base has equivalent line
				nextPart.trs.localScale = nextPart.size;
			}
			base.Death ();
		}
	}
}