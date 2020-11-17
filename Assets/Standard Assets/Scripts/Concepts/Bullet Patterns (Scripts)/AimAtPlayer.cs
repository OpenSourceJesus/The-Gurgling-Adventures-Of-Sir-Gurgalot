using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
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