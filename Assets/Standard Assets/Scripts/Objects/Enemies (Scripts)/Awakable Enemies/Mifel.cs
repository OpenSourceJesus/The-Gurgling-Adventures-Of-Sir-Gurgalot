using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;

namespace TAoKR
{
	public class Mifel : AwakableEnemy
	{
		[MakeConfigurable]
		public float fireRate;
		float fireTimer;
		public Laser laserPrefab;
		[MakeConfigurable]
		public float turnRate;
		float turnAmount;
		float idealTurnAmount;
		
		public override void Update ()
		{
			base.Update ();
			if (!awakened)
				return;
			fireTimer -= Time.deltaTime;
			if (fireTimer <= 0)
			{
				Quaternion rota = Quaternion.LookRotation(Vector3.forward, Player.instance.trs.position - trs.position);
				Laser shotLaser = ObjectPool.instance.Spawn<Laser>(laserPrefab, trs.position, rota, trs);
				fireTimer = fireRate;
			}
			idealTurnAmount = rigid.velocity.GetFacingAngleBetween(Player.instance.trs.position - trs.position);
			turnAmount = Mathf.Clamp(idealTurnAmount, -turnRate * Time.deltaTime, turnRate * Time.deltaTime);
			rigid.velocity = rigid.velocity.Rotate(turnAmount).normalized * MoveSpeed;
		}
		
		public override void Awaken ()
		{
			base.Awaken ();
			if (rigid.velocity.magnitude == 0)
			{
				rigid.velocity = Player.instance.trs.position - trs.position;
				rigid.velocity = rigid.velocity.normalized * MoveSpeed;
			}
		}
	}
}