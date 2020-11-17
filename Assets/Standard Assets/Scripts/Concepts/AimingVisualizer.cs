using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

namespace TGAOSG
{
	public class AimingVisualizer : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public float aimingSensitivity;
		public Transform aimingReticleTrs;
		public Image aimingReticleImage;
		public float recticleDistMultiplier;
		[HideInInspector]
		public Vector2 aimDirection;
		Vector2 idealAimDireciton;
		Vector2 previousAimDirection;
		public Laser aimerLaser;
		public Transform aimOrigin;
		public AimMode aimMode;
		Vector2 aimInput;
		public float minAngleBetweenMouseDeltas;
		Vector2 mousePosition;
		Vector2 previousMousePosition;

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public void DoUpdate ()
		{
			mousePosition = InputManager.Instance.MousePosition;
			aimingReticleImage.enabled = InputManager.UsingGamepad;
			if (InputManager.UsingGamepad)
			{
				aimInput = InputManager.Instance.AimInput;
				if (aimInput.magnitude > InputManager.Settings.defaultDeadzoneMin)
				{
					idealAimDireciton = aimInput;
					aimDirection += Vector2.ClampMagnitude(idealAimDireciton - aimDirection, aimingSensitivity * Time.deltaTime);
					aimDirection = Vector2.ClampMagnitude(aimDirection, 1);
					aimingReticleTrs.localPosition = aimDirection * recticleDistMultiplier;
				}
			}
			else
			{
				switch (aimMode)
				{
					default:
					case AimMode.Relative:
						previousAimDirection = aimInput;
						aimInput = mousePosition - previousMousePosition;
						if (aimInput.magnitude > InputManager.Settings.defaultDeadzoneMin || Vector2.Dot(aimDirection, previousAimDirection) >= minAngleBetweenMouseDeltas)
						{
							idealAimDireciton = aimInput;
							aimDirection = aimInput;
						}
						break;
					case AimMode.Absolute:
						idealAimDireciton = Camera.main.ScreenToWorldPoint(Input.mousePosition) - aimOrigin.position;
						aimDirection = Vector2.ClampMagnitude(idealAimDireciton / recticleDistMultiplier, 1);
						break;
				}
			}
			if (aimerLaser != null)
			{
				if (idealAimDireciton.magnitude > 0)
				{
					aimerLaser.trs.up = aimDirection;
					previousAimDirection = aimDirection;
				}
				else
					aimerLaser.trs.up = previousAimDirection;
			}
			previousMousePosition = mousePosition;
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public enum AimMode
		{
			Absolute,
			Relative
		}
	}
}