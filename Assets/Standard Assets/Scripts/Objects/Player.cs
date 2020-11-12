using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Extensions;
using TGAOSG.SkillTree;
using Fungus;

namespace TGAOSG
{
	public class Player : PlatformerController, IDestructable, IMoneyCarrier, IConfigurable, ISavableAndLoadable
	{
		public int uniqueId;
		public int UniqueId
		{
			get
			{
				return uniqueId;
			}
			set
			{
				uniqueId = value;
			}
		}
		public static Player instance;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		public virtual string Category
		{
			get
			{
				return "Player";
			}
		}
		public static float hp;
		public _float Hp
		{
			get
			{
				return new _float(hp);
			}
			set
			{
				hp = Mathf.Clamp(value.value, 0, maxHp);
				for (int i = 0; i < hpImages.Length; i ++)
					hpImages[i].color = hpImages[i].color.SetAlpha((hp > i).GetHashCode());
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
		static ushort money;
		[SaveAndLoadValue]
		public _ushort Money
		{
			get
			{
				return new _ushort(money);
			}
			set
			{
				money = value.value;
				//GameManager.instance.moneyPanel.SetActive(value.value > 0);
			}
		}
		public Transform healthbar;
		public Image hpImage;
		public float attackReload;
		float attackReloadTimer;
		static string entranceName;
		public Transform itemsParent;
		public Sword sword;
		public Spawnable bloodPrefab;
		public Collider2D bleedBounds;
		public Collider2D BleedBounds
		{
			get
			{
				return bleedBounds;
			}
		}
		public bool BleedBoundsIsCircle
		{
			get
			{
				return false;
			}
		}
		public AudioClip[] deathSounds;
		bool isDead;
		[HideInInspector]
		public List<Vector2> addToVel = new List<Vector2>();
		public bool Invulnerable
		{
			get
			{
				return Time.time - timeLastDamaged < invulnerableDuration || (MagicCape.isDashing && BeInvulnerableWhileDashing.instance.Learned.value);
			}
			set
			{
				if (value)
				{
					if (LongerInvulnerabilityAfterDamage.instance.Learned.value)
						GameManager.instance.screenEffectAnimator.SetFloat("speed", invulnerableDuration / (invulnerableDuration + LongerInvulnerabilityAfterDamage.instance.addSeconds));
					else
						GameManager.instance.screenEffectAnimator.SetFloat("speed", 1);
					GameManager.instance.screenEffectAnimator.Play("Damaged");
				}
			}
		}
		public float invulnerableDuration;
		float timeLastDamaged;
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
		SoundEffect enemyDeathResponse;
		bool hasMoved;
		Transform entranceTrs;
		[SaveAndLoadValue]
		public Vector2 Position
		{
			get
			{
				return (Vector2) trs.position;
			}
			set
			{
				trs.position = value;
			}
		}
		Image[] hpImages;
		[MakeConfigurable]
		public override float MoveSpeed
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
#if UNITY_EDITOR
		public bool activateAllItems;
#endif
		
		public virtual void Start ()
		{
#if UNITY_EDITOR
			if (activateAllItems)
			{
				itemsParent.Find("Magic Hat").gameObject.SetActive(true);
				itemsParent.Find("Magic Cape").gameObject.SetActive(true);
				itemsParent.Find("Magic Amulet").gameObject.SetActive(true);
				// itemsParent.Find("Magic Scepter").gameObject.SetActive(true);
			}
#endif
			addToVel.Clear();
			instance = this;
			timeLastDamaged = -Mathf.Infinity;
			hpImages = new Image[maxHp];
			hpImages[0] = hpImage;
			for (int i = 1; i < maxHp; i ++)
			{
				hpImage = Instantiate(hpImage, healthbar).GetComponent<Image>();
				hpImage.enabled = true;
				hpImages[i] = hpImage;
			}
			hp = maxHp;
			GameObject entrance = GameObject.Find(entranceName);
			if (entrance != null)
			{
				entranceTrs = entrance.GetComponent<Transform>();
				trs.position = entranceTrs.position;
				trs.localScale = trs.localScale.SetX(trs.localScale.x * Mathf.Sign(entranceTrs.localScale.x));
				SetEntrance ("");
			}
			SaveAndLoadManager.instance.Save ();
			GameManager.instance.moneyText.text = "" + money;
			defaultGravityScale = rigid.gravityScale;
			foreach (Skill skill in SkillTreeManager.GetInstance().skills)
				skill.ApplyKnowledgeIfShould ();
		}
		
		public override void FixedUpdate ()
		{
			//if (GameManager.paused)
			//	return;
			base.FixedUpdate ();
			if (!colliderRect.Overlaps(Area.instance.safeRect))
				Death ();
			healthbar.parent.localScale = trs.localScale;
			if (sword.gameObject.activeSelf)
				HandleAttack ();
			if (GameManager.instance.screenEffectAnimator.GetCurrentAnimatorStateInfo(0).IsName("Damaged") && !Invulnerable)
			{
				GameManager.instance.screenEffectAnimator.SetFloat("speed", 1);
				GameManager.instance.screenEffectAnimator.Play("None");
			}
			else if (!GameManager.instance.screenEffectAnimator.GetCurrentAnimatorStateInfo(0).IsName("Damaged") && Invulnerable)
				Invulnerable = true;
		}
		
		public override void Move (float move)
		{
			if (move == 0)
				return;
			if (MagicCape.isDashing)
				rigid.velocity = rigid.velocity.SetX(0);
			base.Move (move);
			hasMoved = true;
		}
		
		public override void HandleIdle ()
		{
			if (hasMoved)
				base.HandleIdle ();
		}
		
		public override void HandleMoving ()
		{
			base.HandleMoving ();
			foreach (Vector2 v in addToVel)
				rigid.velocity += v;
		}
		
		void HandleAttack ()
		{
			if (activityStatus[Activity.Attacking].state == ActivityState.NotDoing)
			{
				if (attackReloadTimer > 0)
					attackReloadTimer -= Time.deltaTime;
				if (attackReloadTimer <= 0)
					activityStatus[Activity.Attacking].canDo = true;
				if (InputManager.inputter.GetButton("Sword") && activityStatus[Activity.Attacking].canDo)
					Attack ();
			}
			else
			{
				if (!sword.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
					activityStatus[Activity.Attacking].state = ActivityState.NotDoing;
			}
		}
		
		void Attack ()
		{
			attackReloadTimer = attackReload;
			sword.anim.Play("Attack");
			activityStatus[Activity.Attacking].state = ActivityState.Doing;
			activityStatus[Activity.Attacking].canDo = false;
		}
		
		public void AddMoney (int amount)
		{
			AddMoney ((ushort) amount);
		}
		
		public void AddMoney (ushort amount)
		{
			money += amount;
			GameManager.instance.moneyText.text = "" + money;
		}
		
		public bool SubtractMoney (ushort amount)
		{
			bool output = Money.value >= amount;
			if (output)
			{
				money -= amount;
				GameManager.instance.moneyText.text = "" + money;
			}
			return output;
		}
		
		public void SetEntrance (string newEntranceName)
		{
			entranceName = "Entrance: " + newEntranceName;
		}
		
		public void TakeDamage (float amount)
		{
			if (Invulnerable)
				return;
			timeLastDamaged = Time.time;
			Invulnerable = true;
			Hp = new _float(hp - amount);
		}
		
		public static Player GetInstance ()
		{
			if (instance == null)
				instance = FindObjectOfType<Player>();
			return instance;
		}
		
		public void Bleed (Vector2 pos, Vector2 facing)
		{
			ObjectPool.instance.Spawn(bloodPrefab, pos, Quaternion.LookRotation(Vector3.forward, facing));
		}
		
		public void Death ()
		{
			if (isDead)
				return;
			isDead = true;
			SoundEffect.Settings deathSoundSettings = new SoundEffect.Settings();
			deathSoundSettings.speakerTrs = trs;
			deathSoundSettings.persistant = true;
			deathSoundSettings.audioClip = deathSounds[Random.Range(0, deathSounds.Length)];
			AudioManager.instance.MakeSoundEffect(deathSoundSettings);
			GameManager.instance.GameOver ();
		}
		
		public void Enable ()
		{
			instance.enabled = true;
			instance.anim.enabled = true;
		}
		
		public void Disable ()
		{
			if (instance != this)
			{
				instance.Disable ();
				return;
			}
			enabled = false;
			StopJump ();
			anim.enabled = false;
			rigid.velocity = rigid.velocity.SetX(0);
		}
		
		public SoundEffect Speak (AudioClip clip)
		{
			SoundEffect.Settings speakSoundSettings = new SoundEffect.Settings();
			speakSoundSettings.audioClip = clip;
			speakSoundSettings.speakerTrs = instance.trs;
			return AudioManager.instance.MakeSoundEffect(speakSoundSettings);
		}
		
		public IEnumerator DeathResponse (SoundEffect deathSoundEffect)
		{
			yield return new WaitUntil(() => (deathSoundEffect == null));
			if (enemyDeathResponse == null)
				enemyDeathResponse = Speak (AudioManager.instance.deathResponses[Random.Range(0, AudioManager.instance.deathResponses.Length)]);
			yield break;
		}
	}
}