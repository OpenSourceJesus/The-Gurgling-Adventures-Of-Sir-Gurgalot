using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class Boss4 : Boss
	{
		[MakeConfigurable]
		public float shootRate;
		float shootTimer;
		[MakeConfigurable]
		public int maxBullets;
		int bulletCount;
		public Bullet bulletPrefab;
		Vector2 toPlayer;
		[MakeConfigurable]
		public float chargeRate;
		float chargeTimer;
		
		public override void Update ()
		{
			//base.Update ();
			if (Time.timeScale == 0)
				return;
			shootTimer -= Time.deltaTime;
			chargeTimer -= Time.deltaTime;
			if (shootTimer < 0 && bulletCount < maxBullets)
			{
				shootTimer = shootRate;
				Bullet bullet = ObjectPool.instance.Spawn(bulletPrefab, trs.position);
				bullet.rigid.velocity = Random.insideUnitCircle.normalized * bullet.moveSpeed;
				bulletCount ++;
			}
			if (chargeTimer < 0)
			{
				chargeTimer += chargeRate;
				ChargePlayer ();
			}
		}
		
		public virtual void ChargePlayer ()
		{
			toPlayer = Player.instance.trs.position - trs.position;
			rigid.velocity = toPlayer.normalized * moveSpeed;
		}
	}
}