using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

namespace TGAOSG
{ 
	public class Boss : Enemy
	{
		public int partIndex;
		public string bossText;
		public Boss nextPart;
		public Animator anim;
		public bool invulnerable = true;
		[MakeConfigurable]
		public float invulnerableDuration;
		[HideInInspector]
		public float invulnerableFlashValue = 1;
		public virtual float InvulnerableFlashValue
		{
			get
			{
				return invulnerableFlashValue;
			}
			set
			{
				invulnerableFlashValue = value;
			}
		}
		int invulnerableFlashChangeDir;
		public int numberOfFlashes;
		public AudioClip deathClip;
		bool justStarted = true;
		[HideInInspector]
		public bool isInitialPart = true;
		public BloodExplosion bloodExplosion;
		
		public override void OnEnable ()
		{
			base.OnEnable ();
			justStarted = true;
			if (isInitialPart)
				Init ();
			if (InvulnerableFlashValue == 0)
				invulnerableFlashChangeDir = 1;
			else
				invulnerableFlashChangeDir = -1;
			GameManager.instance.bossText.text = bossText;
			StartCoroutine(InvulnerableFlash ());
		}

		public virtual void Init ()
		{
			GameManager.instance.bossInfoGo.SetActive(true);
			Boss part = this;
			uint totalHp = 0;
			int partCount = 1;
			while (true)
			{
				totalHp += part.maxHp;
				part = part.nextPart;
				if (part != null)
				{
					partCount ++;
					part.isInitialPart = false;
					part.healthbar = Instantiate(healthbar.parent).GetChild(0);
					part.healthbar.parent.SetParent(healthbar.parent.parent);
					part.healthbar.parent.SetAsFirstSibling ();
					part.healthbar.parent.localScale = Vector3.one;
				}
				else
				{
					part = this;
					while (part.nextPart != null)
					{
						part.healthbar.parent.localScale = part.healthbar.parent.localScale.SetX(maxHp / (totalHp / partCount));
						part = part.nextPart;
					}
					return;
				}
			}
		}
		
		public override void TakeDamage (float amount)
		{
			if (!invulnerable)
				base.TakeDamage (amount);
		}
		
		public virtual void OnDisable ()
		{
			if (nextPart != null)
			{
				nextPart.gameObject.SetActive(true);
				nextPart.enabled = true;
			}
			else if (!justStarted)
				Death ();
			justStarted = false;
		}
		
		public override void Death ()
		{
			if (deathClip != null)
			{
				SoundEffect.Settings deathSoundSettings = new SoundEffect.Settings();
				deathSoundSettings.speakerTrs = trs;
				deathSoundSettings.audioClip = deathClip;
				AudioManager.instance.MakeSoundEffect (deathSoundSettings);
			}
			if (nextPart == null)
			{
				bloodExplosion.Explode (this);
				GameManager.instance.bossInfoGo.SetActive(false);
			}
			else
				nextPart.trs.SetParent(null);
			gameObject.SetActive(false);
			if (triggerOnDeath != null)
				triggerOnDeath.Trigger ();
		}
		
		public virtual IEnumerator InvulnerableFlash ()
		{
			float invulnerableTimer = invulnerableDuration;
			float changeRate = 1f / (invulnerableDuration / numberOfFlashes);
			while (invulnerableTimer > 0)
			{
				invulnerableTimer -= Time.deltaTime;
				InvulnerableFlashValue = Mathf.Clamp01(InvulnerableFlashValue + (changeRate * Time.deltaTime * invulnerableFlashChangeDir));
				if (InvulnerableFlashValue == 0 || InvulnerableFlashValue == 1)
					invulnerableFlashChangeDir *= -1;
				yield return new WaitForEndOfFrame();
			}
			invulnerable = false;
			InvulnerableFlashValue = 1;
		}
		
		public override void Bleed (Vector2 pos, Vector2 facing)
		{
			if (!invulnerable)
				base.Bleed(pos, facing);
		}
	}
}