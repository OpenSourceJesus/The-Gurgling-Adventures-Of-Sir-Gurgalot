using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using TGAOSG.Analytics;
using TGAOSG.SkillTree;

namespace TGAOSG
{
	public class Sword : Hazard
	{
		public Animator anim;
		[MakeConfigurable]
		public float DamageOverTime
		{
			get
			{
				return damageOverTime;
			}
			set
			{
				damageOverTime = value;
			}
		}
		IDestructable destructable;
		Enemy enemy;
		public Dictionary<IDestructable, Coroutine> continueDamageRoutines = new Dictionary<IDestructable, Coroutine>();
		
		public override void OnTriggerEnter2D (Collider2D other)
		{
			if (!SwordWillContinueDoingDamageAShortTimeAfterCeasingToHit.instance.learned || !collider.enabled || Player.instance.Invulnerable)
				return;
			destructable = other.GetComponent<IDestructable>();
			if (destructable != null)
			{
				foreach (KeyValuePair<IDestructable, Coroutine> continueDamageRoutine in continueDamageRoutines)
				{
					if (destructable == continueDamageRoutine.Key)
					{
						StopCoroutine(continueDamageRoutine.Value);
						return;
					}
				}
			}
		}

		public override void OnTriggerStay2D (Collider2D other)
		{
			if (!collider.enabled || Player.instance.Invulnerable)
				return;
			destructable = other.GetComponent<IDestructable>();
			if (destructable != null)
				ApplyDamage (destructable, damageOverTime * Time.deltaTime);
		}

		public void OnTriggerExit2D (Collider2D other)
		{
			if (!SwordWillContinueDoingDamageAShortTimeAfterCeasingToHit.instance.learned || !collider.enabled || Player.instance.Invulnerable)
				return;
			destructable = other.GetComponent<IDestructable>();
			if (destructable != null)
				continueDamageRoutines.Add(destructable, StartCoroutine(ContinueDamageOnDestructable (destructable)));
		}

		public IEnumerator ContinueDamageOnDestructable (IDestructable destructable)
		{
			float timer = 0;
			float damage = 0;
			while (timer < SwordWillContinueDoingDamageAShortTimeAfterCeasingToHit.instance.seconds)
			{
				ApplyDamage (destructable, damageOverTime * Time.deltaTime);
				damage += damageOverTime * Time.deltaTime;
				timer += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			ApplyDamage (destructable, damageOverTime * SwordWillContinueDoingDamageAShortTimeAfterCeasingToHit.instance.seconds - damage);
		}

		public void ApplyDamage (IDestructable destructable, float damage)
		{
			if (Player.instance.Invulnerable)
				return;
			if (destructable.Hp <= damage)
			{
				enemy = destructable as Enemy;
				if (enemy != null)
				{
					AnalyticsManager.EnemyDiedEvent enemyKilledEvent = new AnalyticsManager.EnemyDiedEvent();
					enemyKilledEvent.eventName.value = "Enemy died";
					enemyKilledEvent.killedEnemyName.value = enemy.name;
					AnalyticsManager.instance.LogEvent (enemyKilledEvent);
				}
			}
			MakeBleed (destructable);
			destructable.TakeDamage (damage);
		}
	}
}