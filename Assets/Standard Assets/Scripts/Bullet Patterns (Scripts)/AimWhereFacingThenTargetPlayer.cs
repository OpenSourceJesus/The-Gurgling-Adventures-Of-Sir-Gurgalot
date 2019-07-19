using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	[CreateAssetMenu]
	public class AimWhereFacingThenTargetPlayer : AimWhereFacing
	{
		[MakeConfigurable]
		public float retargetTime;
		
		public override Bullet Shoot (Transform spawner, Bullet bulletPrefab)
		{
			Bullet output = base.Shoot(spawner, bulletPrefab);
			output.StartCoroutine(Retarget (output, retargetTime));
			return output;
		}
		
		public override Vector2 GetRetargetDirection (Bullet bullet)
		{
			return Player.instance.trs.position - bullet.trs.position;
		}
	}
}