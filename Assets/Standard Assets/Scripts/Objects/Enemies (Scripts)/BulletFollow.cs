using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class BulletFollow : Enemy, ISpawnable
	{
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}
		Vector2 destination;
		float distTillDest;
		float prevDistTillDest;
		[HideInInspector]
		public Kutari shooter;
		public LineRenderer line;
		[MakeConfigurable]
		public float range;
		public override bool BleedBoundsIsCircle
		{
			get
			{
				return true;
			}
		}
		
		void Start ()
		{
			destination = trs.position;
			prevDistTillDest = -Mathf.Infinity;
		}
		
		public override void Update ()
		{
			base.Update ();
			distTillDest = Vector2.Distance(trs.position, destination);
			if (distTillDest > prevDistTillDest)
			{
				destination = Player.instance.trs.position;
				Vector2 toDestination = destination - (Vector2) trs.position;
				if (toDestination.magnitude > range)
					destination = (Vector2) trs.position + (toDestination.normalized * range);
				rigid.velocity = (destination - (Vector2) trs.position).normalized * moveSpeed;
			}
			prevDistTillDest = distTillDest;
			line.SetPosition(1, trs.InverseTransformPoint(destination));
		}
		
		public override void Death ()
		{
			shooter.bulletCount --;
			Destroy(gameObject);
		}
	}
}
