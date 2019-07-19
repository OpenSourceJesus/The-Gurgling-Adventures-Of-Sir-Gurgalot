using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	[RequireComponent(typeof(_Rigidbody2D))]
	public class Bullet : Hazard, IMoveable, ISpawnable
	{
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}
		[MakeConfigurable]
		public float MoveSpeed
		{
			get
			{
				return moveSpeed;
			}
			set
			{
				moveSpeed = value;
			}
		}
		public float moveSpeed;
		public Rigidbody2D rigid;
		[MakeConfigurable]
		public float SizeX
		{
			get
			{
				return Mathf.Abs(trs.localScale.x);
			}
			set
			{
				trs.localScale = trs.localScale.SetX(value * Mathf.Sign(trs.localScale.x));
			}
		}
		[MakeConfigurable]
		public float SizeY
		{
			get
			{
				return Mathf.Abs(trs.localScale.y);
			}
			set
			{
				trs.localScale = trs.localScale.SetY(value * Mathf.Sign(trs.localScale.y));
			}
		}
		public override string Category
		{
			get
			{
				return "Bullets";
			}
		}
		
		public virtual void Start ()
		{
			if (rigid.velocity == Vector2.zero)
				rigid.velocity = trs.up * MoveSpeed;
		}
		
		public override void OnTriggerEnter2D (Collider2D other)
		{
			base.OnTriggerEnter2D (other);
			if (!useDamageOverTime)
				ObjectPool.instance.Despawn(PrefabIndex, gameObject, trs);
		}
	}
}