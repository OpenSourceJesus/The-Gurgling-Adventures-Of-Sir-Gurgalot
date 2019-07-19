using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TAoKR;
using Rewired;
using UnityEngine.UI;
using InputManager = TAoKR.InputManager;

public class ButtonMapper : MonoBehaviour
{
	public Transform trs;
	public Text actionNameText;
	public Text buttonNameText;

	public virtual void PollInput ()
	{
		StopAllCoroutines();
		StartCoroutine(PollInputRoutine ());
	}

	public virtual IEnumerator PollInputRoutine ()
	{
		IEnumerable<ControllerPollingInfo> pollInfos;
		while (true)
		{
			if (InputManager.usingJoystick)
			{
				pollInfos = ReInput.controllers.polling.PollControllerForAllButtons(ControllerType.Joystick, ReInput.controllers.Joysticks[0].id);
				foreach (ControllerPollingInfo pollInfo in pollInfos)
				{
					if (pollInfo.success)
					{
						if (OnPollInputSuccess (pollInfo))
						{
							UpdateRemappedAction ();
							yield break;
						}
					}
				}
			}
			pollInfos = ReInput.controllers.polling.PollControllerForAllButtons(ControllerType.Keyboard, ReInput.controllers.Keyboard.id);
			foreach (ControllerPollingInfo pollInfo in pollInfos)
			{
				if (pollInfo.success)
				{
					if (OnPollInputSuccess (pollInfo))
					{
						UpdateRemappedAction ();
						yield break;
					}
				}
			}
			pollInfos = ReInput.controllers.polling.PollControllerForAllButtons(ControllerType.Mouse, ReInput.controllers.Mouse.id);
			foreach (ControllerPollingInfo pollInfo in pollInfos)
			{
				if (pollInfo.success)
				{
					if (OnPollInputSuccess (pollInfo))
					{
						UpdateRemappedAction ();
						yield break;
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public virtual void UpdateRemappedAction ()
	{
		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
		{
			foreach (ActionElementMap actionElementMap in controllerMap.ElementMapsWithAction(actionNameText.text))
			{
				Debug.Log(actionElementMap.elementIdentifierName);
				buttonNameText.text = actionElementMap.elementIdentifierName;
				Save ();
			}
		}
	}

	public virtual bool OnPollInputSuccess (ControllerPollingInfo pollInfo)
	{
		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
		{
			foreach (ActionElementMap actionElementMap in controllerMap.ElementMapsWithAction(actionNameText.text))
			{
				if (actionElementMap.elementIdentifierName == "Mouse Horizontal" || actionElementMap.elementIdentifierName == "Mouse Vertical")
					return false;
				ElementAssignment elementAssignment = new ElementAssignment();
				elementAssignment.elementIdentifierId = pollInfo.elementIndex;
				elementAssignment.actionId = actionElementMap.actionId;
				elementAssignment.elementMapId = actionElementMap.id;
				elementAssignment.axisContribution = actionElementMap.axisContribution;
				elementAssignment.axisRange = actionElementMap.axisRange;
				if (pollInfo.controllerType == ControllerType.Keyboard)
					elementAssignment.keyboardKey = pollInfo.keyboardKey;
				return controllerMap.ReplaceElementMap (elementAssignment);
			}
		}
		return false;
	}

	public virtual void Save ()
	{
		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
		{
			foreach (ActionElementMap actionElementMap in controllerMap.ButtonMapsWithAction(actionNameText.text))
			{
				string elementAssignmentData = actionElementMap.elementIndex + ControlsMapper.VALUE_SEPARATOR;
				if (actionElementMap.keyCode != KeyCode.None)
					elementAssignmentData += actionElementMap.keyCode.GetHashCode() + ControlsMapper.VALUE_SEPARATOR;
				PlayerPrefs.SetString(actionNameText.text + SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + GameManager.accountNumber, elementAssignmentData);
			}
		}
	}
}