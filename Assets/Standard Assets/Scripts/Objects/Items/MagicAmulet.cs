using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	public class MagicAmulet : Item
	{
		public new static MagicAmulet instance;
		public Laser laserPrefab;
		public AimingVisualizer aimer;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
			aimer.gameObject.SetActive(true);
		}
		
		public override void DoUpdate ()
		{
			if (GameManager.paused)
				return;
			cooldownTimer -= Time.deltaTime;
			if (InputManager.Instance.MagicAmuletInput && cooldownTimer <= 0)
			{
				cooldownTimer = cooldown;
				StartCoroutine(Cooldown ());
				ObjectPool.instance.Spawn(laserPrefab, aimer.aimerLaser.trs.position, Quaternion.LookRotation(Vector3.forward, aimer.aimerLaser.trs.up));
			}
		}
		
		public override IEnumerator Cooldown ()
		{
			cooldownTimer = cooldown;
			gamma = 0;
			renderer.color = new Color(gamma, gamma, gamma);
			yield return new WaitUntil(() => (cooldownTimer <= 0));
			gamma = 1;
			renderer.color = new Color(gamma, gamma, gamma);
		}
		
	}
}