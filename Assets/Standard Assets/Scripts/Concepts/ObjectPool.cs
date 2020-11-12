using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace TGAOSG
{
	// [ExecuteAlways]
	public class ObjectPool : SingletonMonoBehaviour<ObjectPool>
	{
		public bool preload;
		public Transform trs;
		public SpawnEntry[] spawnEntries;
		[HideInInspector]
		public List<DelayedDespawn> delayedDespawns = new List<DelayedDespawn>();
		[HideInInspector]
		public List<RangedDespawn> rangedDespawns = new List<RangedDespawn>();
		[HideInInspector]
		public List<SpawnedEntry> spawnedEntries = new List<SpawnedEntry>();
		
		public virtual void Awake ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
			Start ();
			if (!preload)
				return;
			delayedDespawns.Clear();
			rangedDespawns.Clear();
			spawnedEntries.Clear();
			for (int i = 0 ; i < spawnEntries.Length; i ++)
			{
				spawnEntries[i].cache = new List<GameObject>();
				for (int i2 = 0; i2 < spawnEntries[i].preload; i2 ++)
					Preload (i);
			}
		}
		
		public virtual void Update ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
			for (int i = 0; i < delayedDespawns.Count; i ++)
			{
				DelayedDespawn delayedDespawn = delayedDespawns[i];
				delayedDespawn.timeRemaining -= Time.deltaTime;
				if (delayedDespawn.timeRemaining < 0)
				{
					Despawn (delayedDespawn.spawnedEntry.prefabIndex, delayedDespawn.spawnedEntry.go, delayedDespawn.spawnedEntry.trs);
					delayedDespawns.RemoveAt(i);
					i --;
				}
			}
			for (int i = 0; i < rangedDespawns.Count; i ++)
			{
				RangedDespawn rangedDespawn = rangedDespawns[i];
				rangedDespawn.range -= Vector2.Distance(rangedDespawn.spawnedEntry.trs.localPosition, rangedDespawn.previousPos);
				rangedDespawn.previousPos = rangedDespawn.spawnedEntry.trs.localPosition;
				if (rangedDespawn.range < 0)
				{
					Despawn (rangedDespawn.spawnedEntry.prefabIndex, rangedDespawn.spawnedEntry.go, rangedDespawn.spawnedEntry.trs);
					rangedDespawns.Remove(rangedDespawn);
					i --;
				}
			}
		}

		public virtual void OnDestroy ()
		{
			for (int i = 0; i < trs.childCount; i ++)
				Destroy(trs.GetChild(i).gameObject);
		}
		
		public DelayedDespawn DelayDespawn (int prefabIndex, GameObject clone, Transform trs, float delay)
		{
			DelayedDespawn delayedDespawn = new DelayedDespawn(delay);
			delayedDespawn.spawnedEntry = new SpawnedEntry(clone);
			delayedDespawn.spawnedEntry.prefab = spawnEntries[prefabIndex].prefab;
			delayedDespawn.spawnedEntry.trs = trs;
			delayedDespawn.spawnedEntry.prefabIndex = prefabIndex;
			delayedDespawns.Add(delayedDespawn);
			return delayedDespawn;
		}
		
		public RangedDespawn RangeDespawn (int prefabIndex, GameObject clone, Transform trs, float range)
		{
			RangedDespawn rangedDespawn = new RangedDespawn(trs.position, range);
			rangedDespawn.spawnedEntry = new SpawnedEntry(clone);
			rangedDespawn.spawnedEntry.prefab = spawnEntries[prefabIndex].prefab;
			rangedDespawn.spawnedEntry.trs = trs;
			rangedDespawn.spawnedEntry.prefabIndex = prefabIndex;
			rangedDespawns.Add(rangedDespawn);
			return rangedDespawn;
		}
		
		public T SpawnComponent<T> (int prefabIndex, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(), Transform parent = null)
		{
			SpawnEntry spawnEntry = spawnEntries[prefabIndex];
			while (spawnEntry.cache.Count <= spawnEntry.preload)
				Preload (prefabIndex);
			GameObject clone = spawnEntry.cache[0];
			spawnEntry.cache.RemoveAt(0);
			SpawnedEntry entry = new SpawnedEntry(clone);
			entry.prefab = spawnEntry.prefab;
			entry.prefabIndex = prefabIndex;
			entry.trs.position = position;
			entry.trs.rotation = rotation;
			entry.trs.localScale = spawnEntry.trs.localScale;
			clone.SetActive(true);
			entry.trs.SetParent(parent, true);
			spawnedEntries.Add(entry);
			return entry.go.GetComponent<T>();
		}
		
		public T Spawn<T> (T prefab, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(), Transform parent = null)
		{
			return SpawnComponent<T>((prefab as ISpawnable).PrefabIndex, position, rotation, parent);
		}
		
		public GameObject Despawn (DespawnEntry despawnEntry)
		{
			despawnEntry.spawnedEntry.go.SetActive(false);
			despawnEntry.spawnedEntry.trs.SetParent(trs, true);
			spawnEntries[despawnEntry.spawnedEntry.prefabIndex].cache.Add(despawnEntry.spawnedEntry.go);
			return despawnEntry.spawnedEntry.go;
		}
		
		public GameObject Despawn (int prefabIndex, GameObject go, Transform trs)
		{
			go.SetActive(false);
			trs.SetParent(trs, true);
			spawnEntries[prefabIndex].cache.Add(go);
			//spawnedEntries.Remove();
			return go;
		}
		
		public GameObject Preload (int entryIndex)
		{
			GameObject clone = Instantiate(spawnEntries[entryIndex].prefab);
			clone.SetActive(false);
			clone.GetComponent<Transform>().SetParent(trs);
			spawnEntries[entryIndex].cache.Add(clone);
			return clone;
		}

		[Serializable]
		public class ObjectPoolEntry
		{
			public GameObject prefab;
			public Transform trs;
			[HideInInspector]
			public int prefabIndex;
			
			public ObjectPoolEntry ()
			{
			}
			
			public ObjectPoolEntry (GameObject prefab, Transform trs, int prefabIndex)
			{
				this.prefab = prefab;
				this.trs = trs;
				this.prefabIndex = prefabIndex;
			}
		}
		
		[Serializable]
		public class SpawnEntry : ObjectPoolEntry
		{
			public int preload;
			[HideInInspector]
			public List<GameObject> cache = new List<GameObject>();
			
			public SpawnEntry ()
			{
			}
			
			public SpawnEntry (int preload, List<GameObject> cache)
			{
				this.preload = preload;
				this.cache = cache;
			}
		}
		
		public class SpawnedEntry : ObjectPoolEntry
		{
			public GameObject go;
			
			public SpawnedEntry ()
			{
			}
			
			public SpawnedEntry (GameObject go)
			{
				this.go = go;
				trs = go.GetComponent<Transform>();
			}
		}
		
		public class DespawnEntry
		{
			public SpawnedEntry spawnedEntry;
			
			public DespawnEntry ()
			{
			}
			
			public DespawnEntry (SpawnedEntry spawnedEntry)
			{
				this.spawnedEntry = spawnedEntry;
			}
		}
		
		public class DelayedDespawn : DespawnEntry
		{
			public float timeRemaining;
			
			public DelayedDespawn ()
			{
			}
			
			public DelayedDespawn (float timeRemaining)
			{
				this.timeRemaining = timeRemaining;
			}
		}
		
		public class RangedDespawn : DespawnEntry
		{
			public Vector2 previousPos;
			public float range;
			
			public RangedDespawn ()
			{
			}
			
			public RangedDespawn (Vector2 previousPos, float range)
			{
				this.previousPos = previousPos;
				this.range = range;
			}
		}
	}
}