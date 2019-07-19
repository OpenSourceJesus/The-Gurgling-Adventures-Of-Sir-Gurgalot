using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TAoKR;
using Rewired;
using UnityEngine.UI;
using InputManager = TAoKR.InputManager;
using Extensions;
using System;

public class ControlsMapper : SingletonMonoBehaviour<ControlsMapper>
{
	public ButtonMapper buttonMapperPrefab;
	public Transform buttonMappersParent;
	ButtonMapper[] buttonMappers = new ButtonMapper[0];
	public const string VALUE_SEPARATOR = "⧫";

	public override void Start ()
	{
		base.Start ();
		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
		{
			foreach (ActionElementMap actionElementMap in controllerMap.AllMaps)
			{
				ButtonMapper buttonMapper = Instantiate(buttonMapperPrefab, buttonMappersParent);
				buttonMapper.trs.SetSiblingIndex(0);
				InputAction inputAction = ReInput.mapping.GetAction(actionElementMap.actionId);
				if (inputAction.type == InputActionType.Button)
					buttonMapper.actionNameText.text = inputAction.name;
				else if (actionElementMap.axisContribution == Pole.Positive)
					buttonMapper.actionNameText.text = inputAction.positiveDescriptiveName;
				else
					buttonMapper.actionNameText.text = inputAction.negativeDescriptiveName;
				buttonMapper.buttonNameText.text = actionElementMap.elementIdentifierName;
				buttonMappers = buttonMappers.Add_class(buttonMapper);
			}
		}
	}

	public static void Load ()
	{
		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
		{
			for (int i = 0; i < controllerMap.AllMaps.Count; i ++)
			{
				ActionElementMap actionElementMap = controllerMap.AllMaps[i];
				InputAction inputAction = ReInput.mapping.GetAction(actionElementMap.actionId);
				string actionName = inputAction.name;
				if (inputAction.type != InputActionType.Button)
				{
					if (actionElementMap.axisContribution == Pole.Positive)
						actionName = inputAction.positiveDescriptiveName;
					else
						actionName = inputAction.negativeDescriptiveName;
				}
				string elementAssignmentData = PlayerPrefs.GetString(actionName + SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + GameManager.accountNumber, "");
				if (string.IsNullOrEmpty(elementAssignmentData))
					return;
				string[] elementAssignmentDataPieces = elementAssignmentData.Split(new string[] { VALUE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
				ElementAssignment elementAssignment = new ElementAssignment();
				elementAssignment.elementIdentifierId = int.Parse(elementAssignmentDataPieces[0]);
				elementAssignment.actionId = actionElementMap.actionId;
				elementAssignment.elementMapId = actionElementMap.id;
				if (elementAssignmentDataPieces.Length > 1)
					elementAssignment.keyboardKey = (KeyCode) Enum.ToObject(typeof(KeyCode), int.Parse(elementAssignmentDataPieces[1]));
				controllerMap.ReplaceElementMap (elementAssignment);
			}
		}
	}

	public virtual void ResetToDefaults ()
	{
		InputManager.inputter.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
		InputManager.inputter.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
		InputManager.inputter.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);
		foreach (ButtonMapper buttonMapper in buttonMappers)
		{
			foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
			{
				foreach (ActionElementMap actionElementMap in controllerMap.ElementMapsWithAction(buttonMapper.actionNameText.text))
				{
					buttonMapper.buttonNameText.text = actionElementMap.elementIdentifierName;
					buttonMapper.Save ();
				}
			}
		}
	}
}