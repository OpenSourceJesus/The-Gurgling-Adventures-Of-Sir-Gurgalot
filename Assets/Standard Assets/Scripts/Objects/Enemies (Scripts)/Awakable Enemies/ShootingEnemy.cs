using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	public class ShootingEnemy : AwakableEnemy
	{
		public ShootEntry[] shootEntries;
		public Rect RectICanShootWithin
		{
			get
			{
				return GameplayCamera.instance.worldViewRect.Expand(Vector2.one * GameplayCamera.instance.lookRange);
			}
		}
		
		public virtual void Start ()
		{
			foreach (ShootEntry shootEntry in shootEntries)
				shootEntry.bulletPattern.Start ();
		}
		
		public override void Update ()
		{
			if (!awakened || !collider.bounds.ToRect().Overlaps(RectICanShootWithin))
				return;
			base.Update ();
			foreach (ShootEntry shootEntry in shootEntries)
				HandleShooting (shootEntry);
		}
		
		public virtual void HandleShooting (ShootEntry shootEntry)
		{
			if (shootEntry.shootTimer > 0)
				shootEntry.shootTimer -= Time.deltaTime;
			if (shootEntry.shootTimer <= 0)
			{
				shootEntry.shootTimer += shootEntry.shootRate;
				shootEntry.bulletPattern.Shoot(shootEntry.spawner, shootEntry.bulletPrefab);
			}
		}
	}
	
	[System.Serializable]
	public class ShootEntry
	{
		public BulletPattern bulletPattern;
		public Bullet bulletPrefab;
		public float shootRate;
		public float shootTimer;
		public Transform spawner;
	}
}