using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using ClassExtensions;

namespace TAoKR
{
	public class InputManager : SingletonMonoBehaviour<InputManager>
	{
		public static Rewired.Player inputter;
		public static bool usingJoystick;
		public static int currentJoystickId;
		public float defaultJoystickDeadzone;
		public float JoystickDeadzone
		{
			get
			{
				return PlayerPrefs.GetFloat("Joystick Deadzone" + SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPEARATOR + GameManager.accountNumber, defaultJoystickDeadzone);
			}
			set
			{
				PlayerPrefsExtensions.SetFloat("Joystick Deadzone" + SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPEARATOR + GameManager.accountNumber, value);
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
			if (usingJoystick)
                currentJoystickId = ReInput.controllers.Joysticks[0].id;
			ReInput.ControllerConnectedEvent += OnControllerConnected;
			ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
			ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnect;
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
			usingJoystick = true;
			Cursor.visible = false;
            currentJoystickId = args.controllerId;
		}

		public virtual void OnControllerDisconnected (ControllerStatusChangedEventArgs args)
		{
			usingJoystick = ReInput.controllers.joystickCount > 0;
            if (usingJoystick)
                currentJoystickId = ReInput.controllers.Joysticks[ReInput.controllers.joystickCount - 1].id;
            Debug.Log(usingJoystick);
			Cursor.visible = true;
        }
		
		public virtual void OnControllerPreDisconnect (ControllerStatusChangedEventArgs args)
		{
			OnControllerDisconnected (args);
		}
		
		public virtual void OnDestroy ()
		{
			ReInput.ControllerConnectedEvent -= OnControllerConnected;
			ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
			ReInput.ControllerPreDisconnectEvent -= OnControllerPreDisconnect;
			hideCursorTimer.onFinished -= HideCursor;
		}
	}
}