using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;
using TGAOSG;

public class UIControlManager : SingletonMonoBehaviour<UIControlManager>, IUpdatable
{
	public bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	public _Selectable currentSelected;
	public ComplexTimer colorMultiplier;
	public static List<_Selectable> selectables = new List<_Selectable>();
	Vector2 inputDirection;
	Vector2 previousInputDirection;
	public Timer repeatTimer;
	[Range(0, 1)]
	public float angleEffectiveness;
	[Range(0, 1)]
	public float distanceEffectiveness;
	_Selectable selectable;
	bool inControlMode;
	bool controllingWithJoystick;
	Vector2 mousePosition;
	Vector2 previousMousePosition;
	bool leftClickInput;
	bool previousLeftClickInput;

	public override void Start ()
	{
		base.Start ();
		repeatTimer.onFinished += delegate { _HandleChangeSelected (); ControlSelected (); };
		GameManager.updatables = GameManager.updatables.Add(this);
	}

	void OnDestroy ()
	{
		repeatTimer.onFinished -= delegate { _HandleChangeSelected (); ControlSelected (); };
		GameManager.updatables = GameManager.updatables.Remove(this);
	}

	public void DoUpdate ()
	{
		mousePosition = InputManager.Instance.MousePosition;
		leftClickInput = InputManager.Instance.LeftClickInput;
		if (currentSelected != null)
		{
			if (!selectables.Contains(currentSelected) || !currentSelected.selectable.IsInteractable())
			{
				ColorSelected (currentSelected, 1);
				HandleChangeSelected (false);
			}
			ColorSelected (currentSelected, colorMultiplier.GetValue());
			HandleMouseInput ();
			HandleMovementInput ();
			HandleSubmitSelected ();
		}
		else
			HandleChangeSelected (false);
		previousMousePosition = mousePosition;
		previousLeftClickInput = leftClickInput;
	}

	public void AddSelectable (_Selectable selectable)
	{
		selectables.Add(selectable);
	}

	public void RemoveSelectable (_Selectable selectable)
	{
		selectables.Remove(selectable);
	}

	public bool IsMousedOverSelectable (_Selectable selectable)
	{
		return IsMousedOverRectTransform(selectable.rectTrs, selectable.canvas, selectable.canvasRectTrs);
	}

	public bool IsMousedOverRectTransform (RectTransform rectTrs, Canvas canvas, RectTransform canvasRectTrs)
	{
		if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			return rectTrs.GetRectInCanvasNormalized(canvasRectTrs).Contains(canvasRectTrs.GetRectInWorld().ToNormalizedPosition(mousePosition));
		else
			return rectTrs.GetRectInCanvasNormalized(canvasRectTrs).Contains(canvasRectTrs.GetRectInWorld().ToNormalizedPosition(Camera.main.ScreenToWorldPoint(mousePosition)));
	}

	public void HandleMouseInput ()
	{
		bool justCanceledControlMode = false;
		if (((leftClickInput && !previousLeftClickInput) || mousePosition != previousMousePosition) && controllingWithJoystick)
		{
			controllingWithJoystick = false;
		}
		if (!leftClickInput && previousLeftClickInput && !controllingWithJoystick)
		{
			inControlMode = false;
			justCanceledControlMode = true;
		}
		if (!controllingWithJoystick)
		{
			foreach (_Selectable selectable in selectables)
			{
				if (IsMousedOverSelectable(selectable))
				{
					if (justCanceledControlMode || currentSelected != selectable)
					{
						ChangeSelected (selectable);
						return;
					}
				}
			}
		}
		if (leftClickInput)
		{
			_Slider slider = currentSelected.GetComponent<_Slider>();
			if (slider != null)
			{
				Vector2 closestPointToMouseCanvasNormalized = new Vector2();
				if (currentSelected.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					if (selectable != null)
						closestPointToMouseCanvasNormalized = slider.slidingArea.GetRectInCanvasNormalized(selectable.canvasRectTrs).ClosestPoint(slider.canvasRectTrs.GetRectInWorld().ToNormalizedPosition(mousePosition));
				}
				else
				{
					if (selectable != null)
						closestPointToMouseCanvasNormalized = slider.slidingArea.GetRectInCanvasNormalized(selectable.canvasRectTrs).ClosestPoint(slider.canvasRectTrs.GetRectInWorld().ToNormalizedPosition(Camera.main.ScreenToWorldPoint(mousePosition)));
				}
				float normalizedValue = slider.slidingArea.GetRectInCanvasNormalized(slider.canvasRectTrs).ToNormalizedPosition(closestPointToMouseCanvasNormalized).x;
				slider.slider.value = Mathf.Lerp(slider.slider.minValue, slider.slider.maxValue, normalizedValue);
				if (slider.snapValues.Length > 0)
					slider.slider.value = MathfExtensions.GetClosestNumber(slider.slider.value, slider.snapValues);
			}
		}
	}

	public void HandleMovementInput ()
	{
		inputDirection = InputManager.Instance.UIMovementInput;
		if (inputDirection.magnitude > InputManager.Settings.defaultDeadzoneMin)
		{
			controllingWithJoystick = true;
			if (previousInputDirection.magnitude <= InputManager.Settings.defaultDeadzoneMin)
			{
				HandleChangeSelected (true);
				ControlSelected ();
				repeatTimer.timeRemaining = repeatTimer.duration;
				repeatTimer.Start ();
			}
		}
		else
			repeatTimer.Stop ();
		previousInputDirection = inputDirection;
	}

	public void _HandleChangeSelected ()
	{
		HandleChangeSelected (true);
	}

	public void HandleChangeSelected (bool useInputDirection = true)
	{
		if (selectables.Count == 0 || inControlMode)
			return;
		List<_Selectable> otherSelectables = new List<_Selectable>();
		otherSelectables.AddRange(selectables);
		otherSelectables.Remove(currentSelected);
		float selectableAttractiveness;
		_Selectable nextSelected = otherSelectables[0];
		float highestSelectableAttractiveness = GetAttractivenessOfSelectable(nextSelected, useInputDirection);
		_Selectable selectable;
		for (int i = 1; i < otherSelectables.Count; i ++)
		{
			selectable = otherSelectables[i];
			selectableAttractiveness = GetAttractivenessOfSelectable(selectable, useInputDirection);
			if (selectableAttractiveness > highestSelectableAttractiveness)
			{
				highestSelectableAttractiveness = selectableAttractiveness;
				nextSelected = selectable;
			}
		}
		ChangeSelected (nextSelected);
	}

	public void ChangeSelected (_Selectable selectable)
	{
		if (inControlMode)
			return;
		if (currentSelected != null)
			ColorSelected (currentSelected, 1);
		currentSelected = selectable;
		currentSelected.selectable.Select();
		colorMultiplier.JumpToStart ();
	}

	public void HandleSubmitSelected ()
	{
		bool submitButtonPressed = InputManager.Instance.SubmitInput;
		if (submitButtonPressed)
			controllingWithJoystick = true;
		if (submitButtonPressed || (IsMousedOverSelectable(currentSelected) && leftClickInput && !previousLeftClickInput))
		{
			Button button = currentSelected.GetComponent<Button>();
			if (button != null)
				button.onClick.Invoke();
			else
			{
				Toggle toggle = currentSelected.GetComponent<Toggle>();
				if (toggle != null)
					toggle.isOn = !toggle.isOn;
				else
				{
					_Slider slider = currentSelected.GetComponent<_Slider>();
					if (slider != null)
						inControlMode = !inControlMode;
				}
			}
		}
	}

	public void ControlSelected ()
	{
		if (!inControlMode)
			return;
		_Slider slider = currentSelected.GetComponent<_Slider>();
		if (slider != null)
		{
			slider.indexOfCurrentSnapValue = Mathf.Clamp(slider.indexOfCurrentSnapValue + MathfExtensions.Sign(inputDirection.x), 0, slider.snapValues.Length - 1);
			slider.slider.value = slider.snapValues[slider.indexOfCurrentSnapValue];
		}
	}

	public float GetAttractivenessOfSelectable (_Selectable selectable, bool useInputDirection = true)
	{
		float attractiveness = selectable.priority;
		if (useInputDirection)
		{
			Vector2 directionToSelectable = GetDirectionToSelectable(selectable);
			attractiveness = (Vector2.Dot(inputDirection.normalized, directionToSelectable.normalized) * angleEffectiveness) - (directionToSelectable.magnitude * distanceEffectiveness);
		}
		return attractiveness;
	}

	public Vector2 GetDirectionToSelectable (_Selectable selectable)
	{
		return selectable.rectTrs.GetCenterInCanvasNormalized(selectable.canvasRectTrs) - currentSelected.rectTrs.GetCenterInCanvasNormalized(currentSelected.canvasRectTrs);
	}

	public void ColorSelected (_Selectable selectable, float colorMultiplier)
	{
		ColorBlock colors = selectable.selectable.colors;
		colors.colorMultiplier = colorMultiplier;
		selectable.selectable.colors = colors;
	}
}