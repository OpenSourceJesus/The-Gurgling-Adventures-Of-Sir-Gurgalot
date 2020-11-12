using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace TGAOSG
{
	public class GamepadSwitch : MonoBehaviour
	{
		public GameObject[] toggleGos;
		
		public virtual void Awake ()
		{
			ReInput.ControllerConnectedEvent += OnControllerConnected;
			ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
			if (ReInput.controllers.joystickCount > 0)
				ToggleGos ();
		}
		
		public virtual void OnControllerConnected (ControllerStatusChangedEventArgs args)
		{
			if (ReInput.controllers.joystickCount > 0)
				ToggleGos ();
		}
		
		public virtual void OnControllerDisconnected (ControllerStatusChangedEventArgs args)
		{
			if (ReInput.controllers.joystickCount == 0)
				ToggleGos ();
		}
		
		public virtual void ToggleGos ()
		{
			foreach (GameObject go in toggleGos)
				go.SetActive(!go.activeSelf);
		}
		
		public virtual void OnDestroy ()
		{
			ReInput.ControllerConnectedEvent -= OnControllerConnected;
			ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
		}
	}
}