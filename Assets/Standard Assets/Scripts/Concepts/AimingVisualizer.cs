using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

namespace TGAOSG
{
	public class AimingVisualizer : MonoBehaviour
	{
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
		
		public virtual void Update ()
		{
			aimingReticleImage.enabled = InputManager.usingJoystick;
			if (InputManager.usingJoystick)
			{
				aimInput = InputManager.GetAxis2D("Aim Horizontal", "Aim Vertical");
				if (aimInput.magnitude > InputManager.instance.JoystickDeadzone)
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
						aimInput = InputManager.inputter.GetAxis2D("Mouse Horizontal", "Mouse Vertical");
						if (aimInput.magnitude > InputManager.instance.JoystickDeadzone || Vector2.Dot(aimDirection, previousAimDirection) >= minAngleBetweenMouseDeltas)
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
		}

		public enum AimMode
		{
			Absolute,
			Relative
		}
	}
}