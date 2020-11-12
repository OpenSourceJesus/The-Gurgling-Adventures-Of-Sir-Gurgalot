using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using ClassExtensions;

namespace TAoKR
{
	public class Boss5 : Boss
	{
		public GameObject portal;
		public GameObject portal2;
		public Transform portalTrs;
		public AttackPattern[] attackPatterns;
		Dictionary<string, AttackPattern> attackPatternDict = new Dictionary<string, AttackPattern>();
		public float teleportDelay;
		public float teleportHeight;
		public float teleportXDist;
		public Transform leftTeleportBound;
		public Transform rightTeleportBound;
		public Transform topTeleportBound;
		bool hasTeleportedSinceHitGround;
		public float teleportReload;
		Vector2 toPlayer;
		public float upVelToGetUnstuck;
		static Vector2 velocity;
		
		public override void OnEnable ()
		{
			rigid.velocity = velocity;
			base.OnEnable ();
			if (partIndex == 0)
			{
				StartCoroutine(HandleTeleportion ());
				portalTrs.SetParent(null);
			}
			attackPatternDict.Clear ();
			foreach (AttackPattern attackPattern in attackPatterns)
			{
				attackPatternDict.Add(attackPattern.name, attackPattern);
				attackPattern.reloadTimer.timeRemaining = attackPattern.reloadTimer.duration;
				switch (attackPattern.name)
				{
					case "Circle Burst":
						attackPattern.bulletPattern.Start ();
						attackPattern.reloadTimer.onFinished += DoCircleBurst;
						attackPattern.reloadTimer.Start ();
						break;
					case "Homing Explosives":
						attackPattern.bulletPattern.Start ();
						attackPattern.reloadTimer.onFinished += DoHomingExplosives;
						attackPattern.reloadTimer.Start ();
						break;
				}
			}
		}

		public override void OnDisable ()
		{
			velocity = rigid.velocity;
			base.OnDisable ();
			foreach (AttackPattern attackPattern in attackPatterns)
			{
				switch (attackPattern.name)
				{
					case "Circle Burst":
						attackPattern.reloadTimer.onFinished -= DoCircleBurst;
						break;
					case "Homing Explosives":
						attackPattern.reloadTimer.onFinished -= DoHomingExplosives;
						break;
				}
			}
			Destroy(portal);
		}
		
		public override void Update ()
		{
			base.Update ();
			#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.N))
				Death();
			#endif
			toPlayer = Player.instance.trs.position - trs.position;
			HandleMovement ();
		}

		public virtual void HandleMovement ()
		{
			if (!portal.activeSelf || hasTeleportedSinceHitGround)
			{
				if (rigid.velocity.x != 0 && Mathf.Abs(rigid.velocity.x) < moveSpeed / 2)
					rigid.velocity = rigid.velocity.SetY(upVelToGetUnstuck);
				rigid.velocity = rigid.velocity.SetX(moveSpeed * MathfExtensions.Sign(toPlayer.x));
			}
			else
				rigid.velocity = rigid.velocity.SetX(0);
		}
		
		public virtual IEnumerator HandleTeleportion ()
		{
			while (true)
			{
				yield return new WaitForSeconds(teleportReload);
				Vector2 teleportPosition = Player.instance.trs.position;
				teleportPosition.y += teleportHeight;
				if (teleportPosition.y > topTeleportBound.position.y)
					teleportPosition.y = topTeleportBound.position.y;
				teleportPosition.x = Mathf.Clamp(teleportPosition.x, leftTeleportBound.position.x + teleportXDist, rightTeleportBound.position.x - teleportXDist);
				teleportPosition.x += Random.Range(-teleportXDist, teleportXDist);
				StartCoroutine(Teleport (teleportPosition));
			}
		}
		
		public virtual IEnumerator Teleport (Vector2 position)
		{
			portal.SetActive(true);
			portal2.SetActive(true);
			portalTrs.position = position;
			yield return new WaitForSeconds(teleportDelay);
			portal2.SetActive(false);
			portal.SetActive(false);
			trs.position = (Vector3) position;
			hasTeleportedSinceHitGround = true;
		}

		public virtual void DoCircleBurst ()
		{
			DoBulletPattern ("Circle Burst");
		}

		public virtual void DoHomingExplosives ()
		{
			DoBulletPattern ("Homing Explosives");
		}

		public virtual void DoBulletPattern (string attackName)
		{
			AttackPattern attackPattern = attackPatternDict[attackName];
			attackPattern.bulletPattern.Shoot(attackPattern.spawner, attackPattern.bulletPrefab);
		}
		
		public virtual void DoSpikeWaves ()
		{
			string attackName = "Spike Waves";
			AttackPattern attackPattern = attackPatternDict[attackName];
			SpikeWave spikeWave = attackPattern.spawnPrefab.GetComponent<SpikeWave>();
			ObjectPool.instance.Spawn(spikeWave, attackPattern.spawner.position, Quaternion.identity);
			spikeWave = ObjectPool.instance.Spawn(spikeWave, attackPattern.spawner.position, Quaternion.identity);
			spikeWave.Flip ();
		}
		
		public override void OnCollisionEnter2D (Collision2D coll)
		{
			if (hasTeleportedSinceHitGround)
			{
				hasTeleportedSinceHitGround = false;
				DoSpikeWaves ();
			}
		}
		
		[Serializable]
		public class AttackPattern
		{
			public string name;
			public Timer reloadTimer;
			public BulletPattern bulletPattern;
			public Transform spawner;
			public Bullet bulletPrefab;
			public GameObject spawnPrefab;
		}
	}
}