using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	[CreateAssetMenu]
	public class AimAtPlayerWithOffsetThenConinouslySplitWhereFacingWithOffset : AimAtPlayerWithOffset
	{
		[MakeConfigurable]
		public float splitOffset;
		public Bullet splitBulletPrefab;
		[MakeConfigurable]
		public float splitTime;
		
		public override Bullet Shoot (Transform spawner, Bullet bulletPrefab)
		{
			Bullet output = base.Shoot(spawner, bulletPrefab);
			GameManager.instance.StartCoroutine(Split (output, splitBulletPrefab, splitTime));
			return output;
		}
		
		public override IEnumerator Split (Bullet bullet, Bullet splitBulletPrefab, float splitTime)
		{
			yield return new WaitForSeconds(splitTime);
			Shoot (bullet.trs.position, GetSplitDirection(bullet), splitBulletPrefab);
			if (bullet.isActiveAndEnabled)
				bullet.StartCoroutine(Split (bullet, splitBulletPrefab, splitTime));
		}
		
		public override Vector2 GetSplitDirection (Bullet bullet)
		{
			return bullet.trs.up.Rotate(splitOffset);
		}
	}
}