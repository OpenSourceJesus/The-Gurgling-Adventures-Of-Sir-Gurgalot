using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class ConstantSpeedBullet : Bullet
	{
		public virtual void Update ()
		{
			rigid.velocity = rigid.velocity.normalized * MoveSpeed;
		}
	}
}