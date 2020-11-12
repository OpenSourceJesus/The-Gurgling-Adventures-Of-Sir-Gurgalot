using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;

namespace TAoKR
{
	public class HomingExplodingBullet : ExplodingBullet
	{
		public float turnRate;

		public virtual void Update ()
		{
			trs.up = trs.up.RotateTo(Player.instance.trs.position - trs.position, turnRate * Time.deltaTime);
			rigid.velocity = trs.up * moveSpeed;
		}
	}
}