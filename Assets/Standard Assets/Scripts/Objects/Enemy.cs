using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;

namespace TAoKR
{
	public class Enemy : Hazard, IDestructable, IMoveable
	{
		public override string Category
		{
			get
			{
				return "Enemies";
			}
		}
		public float moveSpeed;
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
		float hp;
		public _float Hp
		{
			get
			{
				return new _float(hp);
			}
			set
			{
				if (maxHp == 0)
					return;
				hp = Mathf.Clamp(value.value, 0, maxHp);
				if (healthbar != null)
					healthbar.localScale = new Vector3(hp / maxHp, 1);
				if (hp == 0)
					Death ();
			}
		}
		public uint maxHp;
		[MakeConfigurable]
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
		public Transform healthbar;
		public Spawnable bloodPrefab;
		public Collider2D bleedBounds;
		public Collider2D BleedBounds
		{
			get
			{
				return bleedBounds;
			}
		}
		public Rigidbody2D rigid;
		public virtual bool BleedBoundsIsCircle
		{
			get
			{
				return false;
			}
		}
		public AutoClickButton triggerOnDeath;
		[MakeConfigurable]
		public int difficulty;
		public Vector2 size;
		[MakeConfigurable]
		public float SizeX
		{
			get
			{
				return Mathf.Abs(trs.localScale.x) * size.x;
			}
			set
			{
				trs.localScale = trs.localScale.SetX(value / size.x * Mathf.Sign(trs.localScale.x));
			}
		}
		[MakeConfigurable]
		public float SizeY
		{
			get
			{
				return Mathf.Abs(trs.localScale.y) * size.y;
			}
			set
			{
				trs.localScale = trs.localScale.SetY(value / size.y * Mathf.Sign(trs.localScale.y));
			}
		}
		[HideInInspector]
		public Vector2 healthbarCanvasOffset;
		// public static List<Enemy> deadEnemies = new List<Enemy>();

		public virtual void OnEnable ()
		{
			hp = maxHp;
			if (healthbar != null)
				healthbarCanvasOffset = healthbar.parent.position - trs.position;
		}
		
		public virtual void Update ()
		{
			healthbar.parent.localScale = new Vector3(Mathf.Sign(trs.localScale.x), 1);
		}
		
		public virtual void Death ()
		{
			SoundEffect.Settings deathSoundSettings = new SoundEffect.Settings();
			deathSoundSettings.speakerTrs = trs;
			deathSoundSettings.audioClip = AudioManager.instance.deathSounds[Random.Range(0, AudioManager.instance.deathSounds.Length)];
			SoundEffect deathSoundEffect = AudioManager.instance.MakeSoundEffect (deathSoundSettings);
			Player.instance.StartCoroutine(Player.instance.DeathResponse (deathSoundEffect));
			gameObject.SetActive(false);
			if (triggerOnDeath != null)
				triggerOnDeath.Trigger ();
			// deadEnemies.Add(this);
		}
		
		public virtual void TakeDamage (float amount)
		{
			if (Player.instance.Invulnerable)
				return;
			Hp = new _float(hp - amount);
		}
		
		public virtual void Bleed (Vector2 pos, Vector2 facing)
		{
			if (bloodPrefab != null)
				ObjectPool.instance.Spawn<Spawnable>(bloodPrefab, pos, Quaternion.LookRotation(Vector3.forward, facing));
		}
	}
}