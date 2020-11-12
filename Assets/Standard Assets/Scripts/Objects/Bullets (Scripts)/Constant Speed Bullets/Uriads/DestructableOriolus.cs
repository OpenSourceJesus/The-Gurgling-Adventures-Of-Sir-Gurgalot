using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class DestructableOriolus : Oriolus, IDestructable
	{
		public Transform healthbar;
		public uint maxHp;
		public _uint MaxHp
		{
			get
			{
				return new _uint(maxHp);
			}
			set
			{
				maxHp = value.value;
			}
		}
		float hp;
		public _float Hp
		{
			get
			{
				return new _float(hp);
			}
			set
			{
				hp = Mathf.Clamp(value.value, 0, maxHp);
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
			Hp = new _float(hp - amount);
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