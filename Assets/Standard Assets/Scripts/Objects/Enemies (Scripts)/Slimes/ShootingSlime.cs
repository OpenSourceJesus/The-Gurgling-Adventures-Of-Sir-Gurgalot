using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	public class ShootingSlime : Slime
	{
		public Bullet bulletPrefab;
		[MakeConfigurable]
		public float shootRate;
		float shootTimer;
		[MakeConfigurable]
		public float[] shootAngles;
		public Rect RectICanShootWithin
		{
			get
			{
				return GameplayCamera.instance.worldViewRect.Expand(Vector2.one * GameplayCamera.instance.lookRange);
			}
		}
		
		public override void Update ()
		{
			base.Update ();
			shootTimer -= Time.deltaTime;
			if (!collider.bounds.ToRect().Overlaps(RectICanShootWithin))
				return;
			if (shootTimer < 0)
			{
				shootTimer = shootRate;
                foreach (float shootAngle in shootAngles)
	                ObjectPool.instance.Spawn(bulletPrefab, trs.position, Quaternion.LookRotation(Vector3.forward, trs.right.Rotate(shootAngle)));
			}
		}
	}
}