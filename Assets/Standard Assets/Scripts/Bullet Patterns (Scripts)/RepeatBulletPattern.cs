using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	[CreateAssetMenu]
	public class RepeatBulletPattern : BulletPattern
	{
		[MakeConfigurable]
		public int repeatCount;
		public BulletPattern bulletPattern;
		
		public override void Start ()
		{
			base.Start ();
			bulletPattern.Start ();
		}

		public override Bullet Shoot (Transform spawner, Bullet bulletPrefab)
		{
			Bullet output = null;
			bulletPattern.Start ();
			for (int i = 0; i < repeatCount; i ++)
				output = bulletPattern.Shoot(spawner, bulletPrefab);
			return output;
		}
		
		public override Bullet Shoot (Vector2 spawnPos, Vector2 direction, Bullet bulletPrefab)
		{
			Bullet output = null;
			bulletPattern.Start ();
			for (int i = 0; i < repeatCount; i ++)
				output = bulletPattern.Shoot(spawnPos, direction, bulletPrefab);
			return output;
		}
	}
}