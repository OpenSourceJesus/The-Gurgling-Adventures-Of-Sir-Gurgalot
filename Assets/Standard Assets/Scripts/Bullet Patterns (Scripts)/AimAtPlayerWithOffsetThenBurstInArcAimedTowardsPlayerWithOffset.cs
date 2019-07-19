using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	[CreateAssetMenu]
	public class AimAtPlayerWithOffsetThenBurstInArcAimedTowardsPlayerWithOffset : AimAtPlayerWithOffset
	{
		[MakeConfigurable]
		public float burstOffset;
		public Bullet burstBulletPrefab;
		[MakeConfigurable]
		public float burstTime;
		[MakeConfigurable]
		public float burstArc;
		[MakeConfigurable]
		public float burstNumber;
		
		public override Bullet Shoot (Transform spawner, Bullet bulletPrefab)
		{
			Bullet output = base.Shoot(spawner, burstBulletPrefab);
			Vector2 burstDirection = output.trs.up;
			GameManager.instance.StartCoroutine(Burst (output, burstDirection, burstBulletPrefab, burstTime));
			return output;
		}
		
		public virtual IEnumerator Burst (Bullet bullet, Vector2 direction, Bullet splitBulletPrefab, float splitTime)
		{
			yield return new WaitForSeconds(splitTime);
			if (!bullet.isActiveAndEnabled)
				yield break;
			float bulletDirection = (Player.instance.trs.position - bullet.trs.position).GetFacingAngle();
			for (float splitAngle = bulletDirection - burstArc / 2 + burstOffset; splitAngle < bulletDirection + burstArc / 2 + burstOffset; splitAngle += burstArc / burstNumber)
				Shoot (bullet.trs.position, VectorExtensions.GetVectorFromFacingAngle(splitAngle), splitBulletPrefab);
			ObjectPool.instance.Despawn(bullet.PrefabIndex, bullet.gameObject, bullet.trs);
		}
	}
}