using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.Tilemaps;
using TGAOSG.Analytics;

namespace TGAOSG
{
	public class AmuletLaser : Laser
	{
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
		[MakeConfigurable]
		public float duration;
		public override string Category
		{
			get
			{
				return "Amulet Laser";
			}
		}
		IDestructable destructable;
		Enemy enemy;
		
		public override void Start ()
		{
			base.Start ();
			Destroy(gameObject, duration);
		}
		
		public override void Update ()
		{
			base.Update ();
			line.startColor = line.startColor.AddAlpha(-1f / duration * Time.deltaTime);
			line.endColor = line.startColor.AddAlpha(-1f / duration * Time.deltaTime);
		}
		
		public override void OnTriggerEnter2D (Collider2D other)
		{
			base.OnTriggerEnter2D (other);
			DestructableBlock destructableBlock = other.GetComponent<DestructableBlock>();
			if (destructableBlock != null)
				Destroy(destructableBlock.gameObject);	
			else
			{
				DissolveBlock dissolveBlock = other.GetComponent<DissolveBlock>();
				if (dissolveBlock != null)
					dissolveBlock.StartCoroutine(dissolveBlock.DissolveRoutine ());
			}
		}
		
		public override void OnTriggerStay2D (Collider2D other)
		{
			if (collider == null || !collider.enabled || !useDamageOverTime || Player.instance.Invulnerable)
				return;
			destructable = other.GetComponent<IDestructable>();
			if (destructable != null)
			{
				if (destructable.Hp.value <= damageOverTime * Time.deltaTime)
				{
					enemy = destructable as Enemy;
					if (enemy != null)
					{
						AnalyticsManager.EnemyDiedEvent enemyDiedEvent= new AnalyticsManager.EnemyDiedEvent();
						enemyDiedEvent.eventName.value = "Enemy died";
						enemyDiedEvent.killedEnemyName.value = enemy.name;
						AnalyticsManager.instance.LogEvent (enemyDiedEvent);
					}
				}
				destructable.TakeDamage(damageOverTime * Time.deltaTime);
				MakeBleed (destructable);
			}
		}
	}
}