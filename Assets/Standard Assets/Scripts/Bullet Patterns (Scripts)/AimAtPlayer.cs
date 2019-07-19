using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	[CreateAssetMenu]
	public class AimAtPlayer : BulletPattern
	{
		public override Vector2 GetShootDirection (Transform spawner)
		{
			return Player.instance.trs.position - spawner.position;
		}
	}
}