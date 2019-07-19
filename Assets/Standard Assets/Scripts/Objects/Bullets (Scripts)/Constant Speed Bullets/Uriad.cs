using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	public class Uriad : ConstantSpeedBullet
	{
		public Vector2 initMove;
		
		public override void Start ()
		{
			if (initMove == Vector2.zero)
				initMove = Vector2.right.Rotate(45 * Random.Range(0, 8));
			rigid.velocity = initMove.normalized * MoveSpeed;
		}
		
		public override void OnTriggerEnter2D (Collider2D other)
		{
			
		}
	}
}