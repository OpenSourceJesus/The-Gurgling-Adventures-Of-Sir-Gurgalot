using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System;
using UnityEngine.UI;

namespace TAoKR
{
	public class MagicScepter : Item
	{
		public new static MagicScepter instance;
		[MakeConfigurable]
		public float totalUsableDuration;
		float remainingUsableTime;
		public float RemainingUsableTime
		{
			get
			{
				return remainingUsableTime;
			}
			set
			{
				remainingUsableTime = Mathf.Clamp(value, 0, totalUsableDuration);
				manaBar.localScale = manaBar.localScale.SetX(remainingUsableTime / totalUsableDuration);
			}
		}
		[MakeConfigurable]
		public float rechargeRate;
		public Transform manaBar;
		public Image manaBarImage;
		[HideInInspector]
		public List<TimelinePoint> timelinePoints = new List<TimelinePoint>();
		bool inRecordMode = true;
		Vector2 previousPlayerVel = VectorExtensions.NULL;
		Vector2 previousPlayerPos = VectorExtensions.NULL;
		bool canUse = true;
		public Transform futureShadowTrs;
		public float futureShadowDurationDifference;
		Animator anim;
		float recordStartTime;
		public float manaBarAlphaWhileEnabled;
		public float manaBarAlphaWhileDisabled;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
			anim = GameManager.instance.screenEffectAnimator;
			enabled = false;
			gamma = 1;
		}
		
		public virtual void Update ()
		{
			if (canUse && gamma == 1)
			{
				if (InputManager.inputter.GetButtonDown("Magic Scepter"))
				{
					if (inRecordMode)
						StartRecording ();
					else
						StartPlayback ();
				}
				else if (InputManager.inputter.GetButton("Magic Scepter"))
				{
					if (inRecordMode)
						ContinueRecording ();
					RemainingUsableTime -= Time.deltaTime;
					if (RemainingUsableTime == 0 || InputManager.inputter.GetButtonDown("Cancel Magic Scepter"))
					{
						if (inRecordMode)
						{
							if (InputManager.inputter.GetButtonDown("Cancel Magic Scepter"))
								CancelRecording ();
							else
							{
								canUse = false;
								FinishRecording ();
							}
						}
						else
							StopPlayback ();
					}
				}
				else if (InputManager.inputter.GetButtonUp("Magic Scepter"))
				{
					if (inRecordMode)
						FinishRecording ();
					else
						StopPlayback ();
				}
				else
					RechargeMana ();
			}
			else
			{
				canUse = !InputManager.inputter.GetButton("Magic Scepter") && gamma == 1;
				if (gamma == 1)
					RechargeMana ();
			}
		}

		public virtual void RechargeMana ()
		{
			RemainingUsableTime += Time.deltaTime * rechargeRate;
			// if (previousPlayerVel != (Vector2) VectorExtensions.NULL)
			// {
			// 	Player.instance.addToVel.Remove(previousPlayerVel);
			// 	previousPlayerVel = VectorExtensions.NULL;
			// }
		}
		
		public virtual void StartRecording ()
		{
			timelinePoints.Clear();
			previousPlayerVel = Player.instance.rigid.velocity;
			// previousPlayerVel = Vector2.zero;
			previousPlayerPos = Player.instance.trs.position;
			recordStartTime = Time.time;
			anim.Play("Start Recording");
		}
		
		public virtual void ContinueRecording ()
		{
			TimelinePoint timelinePoint = new TimelinePoint();
			timelinePoint.velocity = (Vector2) Player.instance.rigid.velocity;
			timelinePoint.deltaTime = Time.deltaTime;
			timelinePoint.movement = timelinePoint.velocity * timelinePoint.deltaTime;
			timelinePoint.deltaVelocity = timelinePoint.velocity - previousPlayerVel;
			previousPlayerVel = Player.instance.rigid.velocity;
			previousPlayerPos = Player.instance.trs.position;
			timelinePoint.time = Time.time - recordStartTime;
			timelinePoints.Add(timelinePoint);
		}
		
		public virtual void FinishRecording ()
		{
			inRecordMode = false;
			anim.Play("Finish Recording");
		}

		public virtual void CancelRecording ()
		{
			inRecordMode = true;
			StopAllCoroutines();
			anim.Play("Cancel Recording");
		}

		public virtual void StartPlayback ()
		{
			StartCoroutine(Playback ());
			anim.Play("Start Playback");
		}

		public virtual void StopPlayback ()
		{
			inRecordMode = true;
			StopAllCoroutines();
			anim.Play("Stop Playback");
			futureShadowTrs.gameObject.SetActive(false);
			StartCoroutine(Cooldown ());
		}

		public virtual IEnumerator Playback ()
		{
			// previousPlayerVel = VectorExtensions.NULL;
			StartCoroutine(MoveFutureShadow ());
			for (int i = 0; i < timelinePoints.Count; i ++)
			{
				TimelinePoint timelinePoint = timelinePoints[i];
				if (i > 0)
				{
					yield return new WaitForSeconds(timelinePoint.deltaTime);
					// if (previousPlayerVel != (Vector2) VectorExtensions.NULL)
					// 	Player.instance.addToVel.Remove(previousPlayerVel);
					// Player.instance.addToVel.Add(timelinePoint.velocity);
					// previousPlayerVel = timelinePoint.velocity;
					Player.instance.rigid.velocity -= timelinePoint.deltaVelocity;
					Player.instance.trs.position += (Vector3) timelinePoint.movement;
				}
				else
				{
					Player.instance.rigid.velocity += timelinePoint.velocity;
					Player.instance.trs.position += (Vector3) timelinePoint.movement;
					yield return new WaitForSeconds(timelinePoint.deltaTime);
				}
			}
			StopPlayback ();
		}

		public virtual IEnumerator MoveFutureShadow ()
		{
			futureShadowTrs.position = Player.instance.trs.position;
			futureShadowTrs.gameObject.SetActive(true);
			foreach (TimelinePoint timelinePoint in timelinePoints)
			{
				futureShadowTrs.position += (Vector3) timelinePoint.movement;
				if (timelinePoint.time >= futureShadowDurationDifference)
					yield return new WaitForSeconds(timelinePoint.deltaTime);
			}
		}

		public virtual void OnEnable ()
		{
			manaBarImage.color = manaBarImage.color.SetAlpha(manaBarAlphaWhileEnabled);
			manaBar.gameObject.SetActive(true);
		}

		public virtual void OnDisable ()
		{
			manaBarImage.color = manaBarImage.color.SetAlpha(manaBarAlphaWhileDisabled);
		}

		[Serializable]
		public class TimelinePoint
		{
			public Vector2 position;
			public Vector2 movement;
			public Vector2 velocity;
			public Vector2 deltaVelocity;
			public float deltaTime;
			public float time;
		}
	}
}