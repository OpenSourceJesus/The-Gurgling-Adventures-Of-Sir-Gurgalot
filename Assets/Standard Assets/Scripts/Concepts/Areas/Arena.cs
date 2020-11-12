using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

namespace TAoKR
{
	[ExecuteAlways]
	public class Arena : Area
	{
		public new static Arena instance;
		public EnemySpawnEntry[] enemySpawnEntries;
		public int difficulty;
		public int difficultyIncrease;
		[HideInInspector]
		public int currentDifficulty;
		[HideInInspector]
		public EnemySpawnPoint[] enemySpawnPoints;
		[HideInInspector]
		public List<Enemy> aliveEnemies = new List<Enemy>();
		public float shortenRaycasts;
		public Text scoreText;
		public Text bestScoreText;
		int score;
		public int Score
		{
			get
			{
				return score;
			}
		}
		public int Highscore
		{
			get
			{
				return PlayerPrefs.GetInt(name + " Score", 0);
			}
			set
			{
				PlayerPrefsExtensions.SetInt(name + " Score", value);
			}
		}
		// bool hasStarted;
		public float minSpawnRangeFromPlayer;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				enemySpawnPoints = FindObjectsOfType<EnemySpawnPoint>();
				foreach (EnemySpawnEntry enemySpawnEntry in enemySpawnEntries)
					enemySpawnEntry.Init ();
				return;
			}
			#endif
			scoreText.enabled = false;
			bestScoreText.enabled = false;
			bestScoreText.text = "Highscore: " + Highscore;
		}
		
		public virtual void Begin ()
		{
			scoreText.enabled = true;
			bestScoreText.enabled = true;
			StartCoroutine(HandleDifficulty ());
		}
		
		IEnumerator HandleEnemySpawning ()
		{
			while (currentDifficulty < difficulty)
			{
				do
				{
					EnemySpawnEntry.nextSpawn = enemySpawnEntries[Random.Range(0, enemySpawnEntries.Length)];
				}
				while (currentDifficulty + EnemySpawnEntry.nextSpawn.enemyPrefab.difficulty > difficulty || Random.value > EnemySpawnEntry.nextSpawn.chance);
				yield return StartCoroutine(EnemySpawnEntry.nextSpawn.Spawn ());
			}
		}
		
		IEnumerator HandleDifficulty ()
		{
			while (true)
			{
				if (aliveEnemies.Count > 0)
				{
					if (!aliveEnemies[0].gameObject.activeSelf)
						aliveEnemies.RemoveAt(0);
				}
				else
				{
					Player.instance.Hp = new _float(Player.instance.maxHp);
					score = difficulty / difficultyIncrease;
					scoreText.text = "Score: " + score;
					if (score > Highscore)
						Highscore = score;
					bestScoreText.text = "Highscore: " + Highscore;
					difficulty += difficultyIncrease;
					currentDifficulty = 0;
					yield return StartCoroutine(HandleEnemySpawning ());
				}
				yield return new WaitForEndOfFrame();
			}
		}
		
		public new static Arena GetInstance ()
		{
			if (instance == null)
				instance = FindObjectOfType<Arena>();
			return instance;
		}
		
		[Serializable]
		public class EnemySpawnEntry
		{
			public static EnemySpawnEntry nextSpawn;
			public Enemy enemyPrefab;
			public bool useSubEntries;
			public EnemySpawnEntry[] subEntries;
			[Range(0, 1)]
			public float chance;
			// [HideInInspector]
			public List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();
			Vector3 spawnPos;
			
			public virtual void Init ()
			{
				if (useSubEntries)
				{
					foreach (EnemySpawnEntry subEntry in subEntries)
						subEntry.Init ();
					return;
				}
				spawnPoints.Clear();
				foreach (EnemySpawnPoint enemySpawnPoint in Arena.instance.enemySpawnPoints)
				{
					foreach (Enemy enemyType in enemySpawnPoint.enemyTypesThatUseMe)
					{
						if (enemyType == enemyPrefab)
						{
							spawnPoints.Add(enemySpawnPoint);
							break;
						}
					}
				}
			}
			
			public virtual IEnumerator Spawn ()
			{
				if (useSubEntries)
				{
					do
					{
						nextSpawn = subEntries[Random.Range(0, subEntries.Length)];
					}
					while (Random.value > nextSpawn.chance);
					yield return Arena.instance.StartCoroutine(nextSpawn.Spawn ());
				}
				else
				{
					EnemySpawnPoint spawnPoint = null;
					do
					{
						spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
						spawnPos = (Vector2) spawnPoint.trs.position - (spawnPoint.anchorPosition * enemyPrefab.size);
						yield return new WaitForEndOfFrame();
					}
					while (Physics2D.OverlapBox(spawnPos, enemyPrefab.size - (Vector2.one * Arena.instance.shortenRaycasts * 2), 0) != null || Physics2D.OverlapCircle(spawnPos, Arena.instance.minSpawnRangeFromPlayer, LayerMask.GetMask("Player")) != null);
					Enemy spawnedEnemy = Instantiate(enemyPrefab, spawnPos, enemyPrefab.trs.rotation);
					AwakableEnemy awakableEnemy = spawnedEnemy as AwakableEnemy;
					if (awakableEnemy != null)
						awakableEnemy.Awaken ();
					Arena.instance.currentDifficulty += spawnedEnemy.difficulty;
					Arena.instance.aliveEnemies.Add(spawnedEnemy);
				}
			}
		}
	}
}