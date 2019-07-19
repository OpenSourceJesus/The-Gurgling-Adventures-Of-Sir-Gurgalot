using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Extensions;

namespace TAoKR
{
	public class InputManager : SingletonMonoBehaviour<InputManager>
	{
		public static Rewired.Player inputter;
		public static bool usingJoystick;
		public float defaultJoystickDeadzone;
		public float JoystickDeadzone
		{
			get
			{
				return PlayerPrefs.GetFloat("Joystick Deadzone" + SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + GameManager.accountNumber, defaultJoystickDeadzone);
			}
			set
			{
				PlayerPrefsExtensions.SetFloat("Joystick Deadzone" + SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + GameManager.accountNumber, value);
			}
		}
		public float unhideCursorThreshold;
		public Timer hideCursorTimer;
		public Rewired.InputManager rewiredInputManager;
		
		public virtual void OnEnable ()
		{
			instance = this;
			inputter = ReInput.players.GetPlayer("Player");
			usingJoystick = ReInput.controllers.joystickCount > 0;
			foreach (Joystick joystick in ReInput.controllers.Joysticks)
				inputter.controllers.AddController(joystick, true);
			ReInput.ControllerConnectedEvent += OnControllerConnected;
			ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnect;
			ControlsMapper.Load ();
		}

		public override void Start ()
		{
			base.Start ();
			hideCursorTimer.onFinished += HideCursor;
		}
		
		public virtual void Update ()
		{
			if (GetAxis2D("Mouse Horizontal", "Mouse Vertical").magnitude > unhideCursorThreshold)
			{
				Cursor.visible = true;
				hideCursorTimer.Start ();
			}
		}

		public virtual void HideCursor ()
		{
			if (usingJoystick)
				Cursor.visible = false;
			hideCursorTimer.timeRemaining += hideCursorTimer.duration;
		}
		
		public static Vector2 GetAxis2D (string xAxis, string yAxis)
		{
			Vector2 output = inputter.GetAxis2D(xAxis, yAxis);
			output = Vector2.ClampMagnitude(output, 1);
			return output;
		}
		
		public virtual void OnControllerConnected (ControllerStatusChangedEventArgs args)
		{
			// if (inputter.controllers.joystickCount > 0)
			// 	return;
			inputter.controllers.AddController(args.controller, true);
			usingJoystick = true;
			Cursor.visible = false;
		}
		
		public virtual void OnControllerPreDisconnect (ControllerStatusChangedEventArgs args)
		{
			// if (inputter.controllers.joystickCount > 1)
			// 	return;
			inputter.controllers.RemoveController(args.controller);
			Debug.Log(inputter.controllers.joystickCount);
			if (inputter.controllers.joystickCount == 0)
				usingJoystick = false;
			Cursor.visible = true;
		}
		
		public virtual void OnDestroy ()
		{
			ReInput.ControllerConnectedEvent -= OnControllerConnected;
			ReInput.ControllerPreDisconnectEvent -= OnControllerPreDisconnect;
			hideCursorTimer.onFinished -= HideCursor;
		}
	}
}