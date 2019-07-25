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
		
		public virtual void Start ()
		{
			holdTimer = holdTime;
		}
		
		public virtual void Update ()
		{
			if (Time.timeScale == 0)
				return;
			if (InputManager.inputter.GetButtonDown(interactButton) && buttonState == HotkeyState.Down)
				Interact ();
			else if (InputManager.inputter.GetButtonUp(interactButton) && buttonState == HotkeyState.Up)
				Interact ();
			else if (buttonState == AutoClickButton.HotkeyState.Held)
			{
				if (InputManager.inputter.GetButton(interactButton))
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
		}
		
		public virtual void Interact ()
		{
			triggerButton.Trigger ();
			if (!oneUse)
				triggerButton.Restart ();
		}
	}
}