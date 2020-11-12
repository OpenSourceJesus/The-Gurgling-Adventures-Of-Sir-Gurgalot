using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System;

namespace TGAOSG
{
	[Serializable]
	public class BloodExplosion
	{
		public uint bloodCount;
		public Spawnable bloodPrefab;

		public virtual void Explode (IDestructable destructable)
		{
			for (int i = 0; i < bloodCount; i ++)
				ObjectPool.instance.Spawn<Spawnable>(bloodPrefab, destructable.BleedBounds.bounds.center, Quaternion.LookRotation(Vector3.forward, VectorExtensions.FromFacingAngle(360f / bloodCount * i)));
		}
	}
}