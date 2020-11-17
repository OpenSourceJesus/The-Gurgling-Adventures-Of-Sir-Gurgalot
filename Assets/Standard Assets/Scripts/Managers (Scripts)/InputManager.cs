using UnityEngine;
using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace TGAOSG
{
	public class InputManager : SingletonMonoBehaviour<InputManager>
	{
		public InputSettings settings;
		public static InputSettings Settings
		{
			get
			{
				return InputManager.Instance.settings;
			}
		}
		public static bool UsingGamepad
		{
			get
			{
				return Gamepad.current != null;
			}
		}
		public static bool UsingMouse
		{
			get
			{
				return Mouse.current != null;
			}
		}
		public static bool UsingKeyboard
		{
			get
			{
				return Keyboard.current != null;
			}
		}
		public float MoveInput
		{
			get
			{
				return GetMoveInput(MathfExtensions.NULL_INT);
			}
		}
		public bool JumpInput
		{
			get
			{
				return GetJumpInput(MathfExtensions.NULL_INT);
			}
		}
		public bool InteractInput
		{
			get
			{
				return GetInteractInput(MathfExtensions.NULL_INT);
			}
		}
		public bool SubmitInput
		{
			get
			{
				return GetSubmitInput(MathfExtensions.NULL_INT);
			}
		}
		public bool SwordInput
		{
			get
			{
				return GetSwordInput(MathfExtensions.NULL_INT);
			}
		}
		public bool MagicHatInput
		{
			get
			{
				return GetMagicHatInput(MathfExtensions.NULL_INT);
			}
		}
		public bool DashInput
		{
			get
			{
				return GetDashInput(MathfExtensions.NULL_INT);
			}
		}
		public bool MagicAmuletInput
		{
			get
			{
				return GetMagicAmuletInput(MathfExtensions.NULL_INT);
			}
		}
		public bool MagicScepterInput
		{
			get
			{
				return GetMagicScepterInput(MathfExtensions.NULL_INT);
			}
		}
		public bool CancelMagicScepterInput
		{
			get
			{
				return GetCancelMagicScepterInput(MathfExtensions.NULL_INT);
			}
		}
		public Vector2 AimInput
		{
			get
			{
				return GetAimInput(MathfExtensions.NULL_INT);
			}
		}
		public Vector2 UIMovementInput
		{
			get
			{
				return GetUIMovementInput(MathfExtensions.NULL_INT);
			}
		}
		public bool MenuInput
		{
			get
			{
				return GetMenuInput(MathfExtensions.NULL_INT);
			}
		}
		public bool SkillTreeInput
		{
			get
			{
				return GetSkillTreeInput(MathfExtensions.NULL_INT);
			}
		}
		public Vector2 MousePosition
		{
			get
			{
				return GetMousePosition(MathfExtensions.NULL_INT);
			}
		}
		public bool LeftClickInput
		{
			get
			{
				return GetLeftClickInput(MathfExtensions.NULL_INT);
			}
		}
		
		public virtual void Start ()
		{
			InputSystem.onDeviceChange += OnDeviceChanged;
		}
		
		public virtual void OnDeviceChanged (InputDevice device, InputDeviceChange change)
		{
			if (device is Gamepad)
			{
				if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
				{
					// GameManager.activeCursorEntry.rectTrs.gameObject.SetActive(false);
					// if (VirtualKeyboard.Instance != null)
					// 	VirtualKeyboard.instance.outputToInputField.readOnly = true;
					if (Gamepad.all.Count == 1)
					{
						for (int i = 0; i < GamepadSwitch.instances.Count; i ++)
						{
							GamepadSwitch gamepadSwitch = GamepadSwitch.instances[i];
							gamepadSwitch.ToggleGos ();
						}
					}
				}
				else if (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected)
				{
					// GameManager.activeCursorEntry.rectTrs.gameObject.SetActive(true);
					// if (VirtualKeyboard.Instance != null)
					// 	VirtualKeyboard.instance.outputToInputField.readOnly = false;
					if (Gamepad.all.Count == 0)
					{
						for (int i = 0; i < GamepadSwitch.instances.Count; i ++)
						{
							GamepadSwitch gamepadSwitch = GamepadSwitch.instances[i];
							gamepadSwitch.ToggleGos ();
						}
					}
				}
				// foreach (_Text text in _Text.instances)
				// 	text.UpdateText ();
			}
		}
		
		public virtual void OnDestroy ()
		{
			InputSystem.onDeviceChange -= OnDeviceChanged;
		}

		public static float GetMoveInput (int playerIndex)
		{
			if (UsingGamepad)
				return GetGamepad(playerIndex).leftStick.x.ReadValue();
			else
			{
				int output = 0;
				Keyboard keyboard = GetKeyboard(playerIndex);
				if (keyboard.dKey.isPressed)
					output ++;
				if (keyboard.aKey.isPressed)
					output --;
				return output;
			}
		}

		public static bool GetJumpInput (int playerIndex)
		{
			if (UsingGamepad)
				return GetGamepad(playerIndex).leftShoulder.isPressed;
			else
				return GetKeyboard(playerIndex).wKey.isPressed;
		}

		public static bool GetInteractInput (int playerIndex)
		{
			if (UsingGamepad)
				return GetGamepad(playerIndex).aButton.isPressed;
			else
				return GetKeyboard(playerIndex).eKey.isPressed;
		}

		public static bool GetSubmitInput (int playerIndex)
		{
			if (UsingGamepad)
				return GetGamepad(playerIndex).aButton.isPressed;
			else
				return GetKeyboard(playerIndex).enterKey.isPressed;
		}
		
		public static bool GetSwordInput (int playerIndex)
		{
			if (UsingGamepad)
			{
				Gamepad gamepad = GetGamepad(playerIndex);
				return gamepad.rightTrigger.isPressed || gamepad.rightStickButton.isPressed;
			}
			else
				return GetMouse(playerIndex).leftButton.isPressed;
		}

		public static bool GetMagicHatInput (int playerIndex)
		{
			if (UsingGamepad)
				return GetGamepad(playerIndex).rightStickButton.isPressed;
			else
				return GetMouse(playerIndex).rightButton.isPressed;
		}

		public static bool GetDashInput (int playerIndex)
		{
			if (UsingGamepad)
				return GetGamepad(playerIndex).leftTrigger.isPressed;
			else
				return GetKeyboard(playerIndex).leftShiftKey.isPressed;
		}
		
		public static bool GetMagicAmuletInput (int playerIndex)
		{
			if (UsingGamepad)
			{
				Gamepad gamepad = GetGamepad(playerIndex);
				return gamepad.rightTrigger.isPressed || gamepad.rightStickButton.isPressed;
			}
			else
				return GetMouse(playerIndex).leftButton.isPressed;
		}

		public static bool GetMagicScepterInput (int playerIndex)
		{
			return false;
		}

		public static bool GetCancelMagicScepterInput (int playerIndex)
		{
			return false;
		}

		public static Vector2 GetAimInput (int playerIndex)
		{
			if (UsingGamepad)
				return Vector2.ClampMagnitude(GetGamepad(playerIndex).rightStick.ReadValue(), 1);
			else
				return Vector2.ClampMagnitude(GameplayCamera.Instance.camera.ScreenToWorldPoint(GetMousePosition(playerIndex)) - Player.instance.trs.position, 1);
		}

		public static Vector2 GetUIMovementInput (int playerIndex)
		{
			if (UsingGamepad)
				return Vector2.ClampMagnitude(GetGamepad(playerIndex).leftStick.ReadValue(), 1);
			else
			{
				Keyboard keyboard = GetKeyboard(playerIndex);
				int x = 0;
				if (keyboard.dKey.isPressed)
					x ++;
				if (keyboard.aKey.isPressed)
					x --;
				int y = 0;
				if (keyboard.wKey.isPressed)
					y ++;
				if (keyboard.sKey.isPressed)
					y --;
				return Vector2.ClampMagnitude(new Vector2(x, y), 1);
			}
		}

		public static bool GetMenuInput (int playerIndex)
		{
			if (UsingGamepad)
			{
				Gamepad gamepad = GetGamepad(playerIndex);
				return gamepad.startButton.isPressed || gamepad.selectButton.isPressed;
			}
			else
				return GetKeyboard(playerIndex).escapeKey.isPressed;
		}

		public static bool GetSkillTreeInput (int playerIndex)
		{
			if (UsingGamepad)
			{
				Gamepad gamepad = GetGamepad(playerIndex);
				return gamepad.startButton.isPressed || gamepad.selectButton.isPressed;
			}
			else
				return GetKeyboard(playerIndex).escapeKey.isPressed;
		}

		public static bool GetLeftClickInput (int playerIndex)
		{
			if (UsingMouse)
				return GetMouse(playerIndex).leftButton.isPressed;
			else
				return false;
		}

		public static Vector2 GetMousePosition (int playerIndex)
		{
			// if (UsingMouse)
				return GetMouse(playerIndex).position.ReadValue();
			// else
				// return GameManager.activeCursorEntry.rectTrs.position;
		}

		public static Vector2 GetWorldMousePosition (int playerIndex)
		{
			return CameraScript.Instance.camera.ViewportToWorldPoint(GetMousePosition(playerIndex));
		}

		public static Gamepad GetGamepad (int playerIndex)
		{
			Gamepad gamepad = Gamepad.current;
			if (Gamepad.all.Count > playerIndex)
				gamepad = Gamepad.all[playerIndex];
			return gamepad;
		}

		public static Mouse GetMouse (int playerIndex)
		{
			Mouse mouse = Mouse.current;
			// if (Mouse.all.Count > playerIndex)
			// 	mouse = (Mouse) Mouse.all[playerIndex];
			return mouse;
		}

		public static Keyboard GetKeyboard (int playerIndex)
		{
			Keyboard keyboard = Keyboard.current;
			// if (Keyboard.all.Count > playerIndex)
			// 	keyboard = (Keyboard) Keyboard.all[playerIndex];
			return keyboard;
		}
	}
}