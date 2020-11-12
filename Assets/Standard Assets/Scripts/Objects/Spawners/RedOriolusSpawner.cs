using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	public class RedOriolusSpawner : Spawner
	{
		public DestructableOriolus redOriolusPrefab;
		
		public override void Spawn ()
		{
			Oriolus redOriolus = null;
			if (useMyRotation)
				redOriolus = ObjectPool.instance.Spawn(redOriolusPrefab, trs.position, trs.rotation);
			else
				redOriolus = ObjectPool.instance.Spawn(redOriolusPrefab, trs.position, prefabTrs.rotation);
			redOriolus.initMove = trs.up.Snap(Vector3.one);
			redOriolus.Start ();
		}
	}
}