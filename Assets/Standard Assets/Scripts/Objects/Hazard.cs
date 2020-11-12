using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;
using UnityEngine.Tilemaps;
using TAoKR.Analytics;

namespace TAoKR
{
	public class Hazard : MonoBehaviour, ICollisionHandler, IConfigurable
	{
		[MakeConfigurable]
		public virtual float DamageOnHit
		{
			get
			{
				hazards = GetComponentsInChildren<Hazard>();
				return hazards[hazards.Length - 1].damage;
			}
			set
			{
				hazards = GetComponentsInChildren<Hazard>();
				hazards[hazards.Length - 1].damage = value;
			}
		}
		public bool useDamage;
		//[MakeConfigurable]
		public float damage;
		public bool useDamageOverTime;
		//[MakeConfigurable]
		public float damageOverTime;
		public float randomizeBleedPosition;
		public float randomizeBleedDirection;
		public Transform trs;
		public new Collider2D collider;
		public string Name
		{
			get
			{
				return name;
			}
		}
		public virtual string Category
		{
			get
			{
				return "Hazards";
			}
		}
		IDestructable destructable;
		Hazard[] hazards;
		
		public virtual void OnCollisionEnter2D (Collision2D coll)
		{
			OnTriggerEnter2D (coll.collider);
		}
		
		public virtual void OnCollisionStay2D (Collision2D coll)
		{
			OnTriggerStay2D (coll.collider);
		}
		
		public virtual void OnTriggerEnter2D (Collider2D other)
		{
			if (collider == null || !collider.enabled || !useDamage || Player.instance.Invulnerable)
				return;
			destructable = other.GetComponent<IDestructable>();
			if (destructable != null)
			{
				if ((destructable as Player) != null)
				{
					AnalyticsManager.PlayerHurtEvent playerHurtEvent = new AnalyticsManager.PlayerHurtEvent();
					hazards = GetComponentsInParent<Hazard>();
					playerHurtEvent.eventName.value = "Player hurt";
					playerHurtEvent.hurtBy.value = hazards[hazards.Length - 1].name;
					AnalyticsManager.instance.LogEvent (playerHurtEvent);
					if (destructable.Hp.value - damage <= 0)
					{
						AnalyticsManager.PlayerDiedEvent playerDiedEvent = new AnalyticsManager.PlayerDiedEvent();
						playerDiedEvent.eventName.value = "Player died";
						playerDiedEvent.killedBy.value = hazards[hazards.Length - 1].name;
						AnalyticsManager.instance.LogEvent (playerDiedEvent);
					}
				}
				MakeBleed (destructable);
				destructable.TakeDamage (damage);
			}
		}
		
		public virtual void OnTriggerStay2D (Collider2D other)
		{
			OnTriggerEnter2D (other);
		}
		
		public virtual void MakeBleed (IDestructable destructable)
		{
			if (destructable.BleedBounds == null)
				return;
			Vector2 pos = collider.bounds.ClosestPoint(destructable.BleedBounds.bounds.center + (Vector3) (Random.insideUnitCircle * randomizeBleedPosition));
			Vector2 pos2 = destructable.BleedBounds.bounds.ClosestPoint(pos);
			if (destructable.BleedBoundsIsCircle)
				pos2 = pos + Vector2.ClampMagnitude(pos2 - pos, Mathf.Abs(destructable.BleedBounds.bounds.extents.x));
			Vector2 facing = pos2 - pos;
			facing = facing.Rotate(Random.Range(-randomizeBleedDirection, randomizeBleedDirection));
			destructable.Bleed (pos, facing);
		}
		
		public virtual void OnTriggerEnter2DHandler (Collider2D other)
		{
			OnTriggerEnter2D (other);
		} 
		
		public virtual void OnTriggerStay2DHandler (Collider2D other)
		{
			OnTriggerStay2D (other);
		}
		
		public virtual void OnTriggerExit2DHandler (Collider2D other)
		{
		}
		
		public virtual void OnCollisionEnter2DHandler (Collision2D coll)
		{
			OnCollisionEnter2D (coll);
		}
		
		public virtual void OnCollisionStay2DHandler (Collision2D coll)
		{
			OnCollisionStay2D (coll);
		}
		
		public virtual void OnCollisionExit2DHandler (Collision2D coll)
		{
		}
	}
}