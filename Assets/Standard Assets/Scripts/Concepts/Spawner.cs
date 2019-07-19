using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class Spawner : MonoBehaviour, IConfigurable
	{
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
				return "Spawners";
			}
		}
		[MakeConfigurable]
		public float spawnRate;
		public Transform trs;
		public Transform prefabTrs;
		public bool useMyRotation;
		
		public virtual void Start ()
		{
			StartCoroutine(SpawnRoutine ());
		}

		public virtual IEnumerator SpawnRoutine ()
		{
			while (true)
			{
				Spawn ();
				yield return new WaitForSeconds(spawnRate);
			}
		}
		
		public virtual void Spawn ()
		{
			if (useMyRotation)
				Instantiate(prefabTrs, trs.position, trs.rotation);
			else
				Instantiate(prefabTrs, trs.position, prefabTrs.rotation);
		}
	}
}