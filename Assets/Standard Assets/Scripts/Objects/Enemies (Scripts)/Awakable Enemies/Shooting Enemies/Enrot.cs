﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	public class Enrot : ShootingEnemy
	{
		[MakeConfigurable]
		public int range;
		float toPlayerX;
		
		public override void DoUpdate ()
		{
			base.DoUpdate ();
			if (!awakened)
				return;
			toPlayerX = Player.instance.trs.position.x - trs.position.x;
			if (Mathf.Abs(toPlayerX) > range)
				rigid.velocity = Vector2.right * Mathf.Sign(toPlayerX) * moveSpeed;
			else
				rigid.velocity = Vector2.zero;
			trs.localScale = trs.localScale.SetX(Mathf.Sign(toPlayerX));
		}
	}
}