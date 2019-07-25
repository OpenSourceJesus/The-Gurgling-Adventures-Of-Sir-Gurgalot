using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class DestructableOriolus : Oriolus, IDestructable
	{
		public Transform healthbar;
		public uint maxHp;
		public uint MaxHp
		{
			get
			{
				return maxHp;
			}
			set
			{
				maxHp = value;
			}
		}
		float hp;
		public float Hp
		{
			get
			{
				return hp;
			}
			set
			{
				hp = Mathf.Clamp(value, 0, maxHp);
				if (healthbar != null)
					healthbar.localScale = new Vector3(hp / maxHp, 1);
				if (hp == 0)
					Death ();
			}
		}
		public Collider2D BleedBounds
		{
			get
			{
				return collider;
			}
		}
		public bool BleedBoundsIsCircle
		{
			get
			{
				return false;
			}
		}

		public virtual void TakeDamage (float amount)
		{
			Hp = hp - amount;
		}

		public virtual void Bleed (Vector2 pos, Vector2 facing)
		{
		}

		public virtual void Death ()
		{
			Destroy(gameObject);
		}
	}
}