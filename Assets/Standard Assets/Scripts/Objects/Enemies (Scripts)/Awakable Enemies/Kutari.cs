using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class Kutari : AwakableEnemy
	{
		[MakeConfigurable]
		public float shootRate;
		float shootTimer;
		public Animator anim;
		public BulletFollow bulletFollowPrefab;
		public Transform bulletSpawn;
		[MakeConfigurable]
		public int maxBulletCount;
		[HideInInspector]
		public int bulletCount;
		bool isPlayingAnim;
		bool wasPlayingAnimLastFrame;
		
		public override void Update ()
		{
			base.Update ();
			if (!awakened)
				return;
			trs.localScale = new Vector3(-Mathf.Sign(Player.instance.trs.position.x - trs.position.x), 1);
			isPlayingAnim = anim.GetCurrentAnimatorStateInfo(0).IsName("Prepare Spawn");
			if (!isPlayingAnim)
			{
				if (wasPlayingAnimLastFrame)
				{
					if (bulletCount < maxBulletCount)
					{
						bulletCount ++;
						BulletFollow bulletFollow = ObjectPool.instance.Spawn(bulletFollowPrefab, bulletSpawn.position);
						bulletFollow.shooter = this;
					}
				}
				else
				{
					shootTimer -= Time.deltaTime;
					if (shootTimer <= 0 && bulletCount < maxBulletCount)
					{
						anim.Play("Prepare Spawn");
						shootTimer = shootRate;
					}
				}
			}
			wasPlayingAnimLastFrame = isPlayingAnim;
		}
	}
}