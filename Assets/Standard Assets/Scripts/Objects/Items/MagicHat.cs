using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TAoKR.SkillTree;

namespace TAoKR
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class MagicHat : Item, IDestructable
	{
		public new static MagicHat instance;
		[MakeConfigurable]
		public int throwForce;
		public Rigidbody2D rigid;
		public Transform trs;
		public Transform shootPos;
		Vector2 initPos;
		public new Collider2D collider;
		public Collider2D playerBounds;
		public float reducePlayerBoundsAmount;
		[HideInInspector]
		public bool isAiming;
		public GameObject rangeObj;
		public Transform rangeTrs;
		[MakeConfigurable]
		public float ThrowRange
		{
			get
			{
				return rangeTrs.localScale.x / 2;
			}
			set
			{
				rangeTrs.localScale = Vector3.one * value * 2;
			}
		}
		SetZOrder currentZOrderer;
		public SetZOrder[] zOrderers;
		public LineRenderer aimHelper;
		public int aimHelperPosCount;
		public _float Hp
		{
			get
			{
				return new _float();
			}
			set
			{
				TakeDamage (0);
			}
		}
		public _uint MaxHp
		{
			get
			{
				return new _uint();
			}
			set
			{
			}
		}
		public Collider2D BleedBounds
		{
			get
			{
				return null;
			}
		}
		public bool BleedBoundsIsCircle
		{
			get
			{
				return false;
			}
		}
		public AimingVisualizer aimer;
		[HideInInspector]
		public bool justTeleported;
		[HideInInspector]
		public bool previousJustTeleported;
		
		public override void Start ()
		{
			base.Start ();
			foreach (SetZOrder zOrderer in zOrderers)
			{
				if (zOrderer.enabled)
				{
					currentZOrderer = zOrderer;
					break;
				}
			}
			initPos = trs.localPosition;
			aimer.gameObject.SetActive(true);
			aimHelper.positionCount = aimHelperPosCount;
		}
		
		void FixedUpdate ()
		{
			if (GameManager.paused || !Player.instance.enabled)
				return;
			if (!isAiming && !rigid.simulated && InputManager.inputter.GetButtonDown("Magic Hat"))// && Player.instance.activityStatus[Activity.Attacking].state != ActivityState.Doing)
				TakeOffHead ();
			if (isAiming && InputManager.inputter.GetButtonUp("Magic Hat"))
				Throw ();
			if (rigid.simulated && cooldownTimer <= 0 && InputManager.inputter.GetButtonDown("Magic Hat") && Physics2D.OverlapCapsule(playerBounds.bounds.center, playerBounds.bounds.size - (Vector3.one * reducePlayerBoundsAmount), CapsuleDirection2D.Vertical, 0, Player.instance.whatICollideWith) == null)
			{
				StartCoroutine(Cooldown ());
				PlayerTeleportToMe ();
				PutBackOn ();
			}
			if (isAiming)
			{
				if (MagicScepter.instance != null)
					MagicScepter.instance.RemainingUsableTime += Time.deltaTime * MagicScepter.instance.rechargeRate;
				DrawAimHelper ();
				aimHelper.enabled = true;
				// if (Player.instance.activityStatus[Activity.Attacking].state != ActivityState.NotDoing)
				// 	PutBackOn ();
			}
			else
				aimHelper.enabled = false;
			if (InputManager.inputter.GetButtonDown("Interact"))
			{
				PutBackOn ();
				//if (rigid.simulated && Physics2D.OverlapCapsule(Player.instance.colliderBounds.center, Player.instance.colliderBounds.size, CapsuleDirection2D.Vertical, 0, gameObject.layer))
				//	PutBackOn ();
			}
		}
		
		void PlayerTeleportToMe ()
		{
			if (MagicScepter.instance != null)
			{
				MagicScepter.TimelinePoint timelinePoint = new MagicScepter.TimelinePoint();
				timelinePoint.movement = playerBounds.bounds.center - Player.instance.trs.position;
				timelinePoint.deltaTime = Time.deltaTime;
				MagicScepter.instance.timelinePoints.Add(timelinePoint);
			}
			Player.instance.trs.position = playerBounds.bounds.center;
		}
		
		void TakeOffHead ()
		{
			isAiming = true;
			Player.instance.sword.gameObject.SetActive(false);
			if (MagicScepter.instance != null)
				MagicScepter.instance.gameObject.SetActive(false);
			SetCurrentZOrderer (1);
			trs.position = shootPos.position;
			rangeObj.SetActive(true);
		}
		
		void PutBackOn ()
		{
			isAiming = false;
			rigid.simulated = false;
			collider.enabled = false;
			trs.SetParent(Player.instance.itemsParent);
			trs.localPosition = initPos;
			Player.instance.sword.gameObject.SetActive(true);
			if (MagicScepter.instance != null)
				MagicScepter.instance.gameObject.SetActive(true);
			SetCurrentZOrderer (0);
			rangeObj.SetActive(false);
		}
		
		void Throw ()
		{
			rigid.velocity = Player.instance.rigid.velocity;
			isAiming = false;
			trs.SetParent(null);
			collider.enabled = true;
			rigid.simulated = true;
			rigid.velocity += aimer.aimDirection * throwForce;
			SetCurrentZOrderer (1);
			Player.instance.sword.gameObject.SetActive(true);
			if (MagicScepter.instance != null)
				MagicScepter.instance.gameObject.SetActive(true);
			aimHelper.enabled = false;
		}
		
		void OnTriggerExit2D (Collider2D other)
		{
			if (other == Player.instance.collider)
				PutBackOn ();
		}
		
		void DrawAimHelper ()
		{
			Vector2 pos = trs.position;
			Vector2 velocity = Player.instance.rigid.velocity;
			velocity += aimer.aimDirection * throwForce;
			for (int i = 0; i < aimHelperPosCount; i ++)
			{
				aimHelper.SetPosition(i, pos);
				pos += (velocity / (rigid.drag + 1)) * Time.fixedDeltaTime;
				velocity += Physics2D.gravity * rigid.gravityScale * Time.fixedDeltaTime;
			}
		}
		
		public void TakeDamage (float amount)
		{
			if (!MagicHatIsInvulnerable.instance.learned)
				PutBackOn ();
		}
		
		public void Bleed (Vector2 pos, Vector2 facing)
		{
		}
		
		public void Death ()
		{
		}
		
		void SetCurrentZOrderer (int stateIndex)
		{
			currentZOrderer.enabled = false;
			currentZOrderer = zOrderers[stateIndex];
			currentZOrderer.enabled = true;
		}
	}
}