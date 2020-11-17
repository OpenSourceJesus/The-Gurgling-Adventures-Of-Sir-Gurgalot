using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HotkeyState = TGAOSG.AutoClickButton.HotkeyState;

namespace TGAOSG
{
	public class Interactable : MonoBehaviour
	{
		public string interactButton;
		public HotkeyState buttonState;
		public float holdTime;
		float holdTimer;
		public AutoClickButton triggerButton;
		public bool oneUse;
		bool interactInput;
		bool previousInteractInput;
		
		public virtual void Start ()
		{
			holdTimer = holdTime;
		}
		
		public virtual void Update ()
		{
			interactInput = InputManager.Instance.InteractInput;
			if (Time.timeScale == 0)
				return;
			if (interactInput && !previousInteractInput && buttonState == HotkeyState.Down)
				Interact ();
			else if (!interactInput && previousInteractInput && buttonState == HotkeyState.Up)
				Interact ();
			else if (buttonState == AutoClickButton.HotkeyState.Held)
			{
				if (interactInput)
				{
					holdTimer -= Time.deltaTime;
					if (holdTimer < 0)
					{
						holdTimer = holdTime;
						Interact ();
					}
				}
				else
					holdTimer = holdTime;
			}
			previousInteractInput = interactInput;
		}
		
		public virtual void Interact ()
		{
			triggerButton.Trigger ();
			if (!oneUse)
				triggerButton.Restart ();
		}
	}
}