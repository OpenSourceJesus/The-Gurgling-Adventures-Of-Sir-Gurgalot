// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TGAOSG;
// using Rewired;
// using UnityEngine.UI;
// using InputManager = TGAOSG.InputManager;

// public class ButtonMapper : MonoBehaviour
// {
// 	public Text actionNameText;
// 	public Text buttonNameText;

// 	public virtual void PollInput ()
// 	{
// 		StopAllCoroutines();
// 		StartCoroutine(PollInputRoutine ());
// 	}

// 	public virtual IEnumerator PollInputRoutine ()
// 	{
// 		IEnumerable<ControllerPollingInfo> pollInfos;
// 		while (true)
// 		{
// 			if (InputManager.UsingGamepad)
// 			{
// 				pollInfos = InputManager.inputter.controllers.polling.PollControllerForAllButtons(ControllerType.Joystick, InputManager.currentJoystickId);
// 				foreach (ControllerPollingInfo pollInfo in pollInfos)
// 				{
// 					if (pollInfo.success)
// 					{
// 						OnPollInputSuccess (pollInfo);
// 						yield break;
// 					}
// 				}
// 			}
// 			pollInfos = InputManager.inputter.controllers.polling.PollControllerForAllButtons(ControllerType.Keyboard, 0);
// 			foreach (ControllerPollingInfo pollInfo in pollInfos)
// 			{
// 				if (pollInfo.success)
// 				{
// 					OnPollInputSuccess (pollInfo);
// 					yield break;
// 				}
// 			}
// 			yield return new WaitForEndOfFrame();
// 		}
// 		yield break;
// 	}

// 	public virtual void OnPollInputSuccess (ControllerPollingInfo pollInfo)
// 	{
// 		IEnumerable<ControllerMap> controllerMaps = InputManager.inputter.controllers.maps.GetAllMaps();
// 		foreach (ControllerMap controllerMap in controllerMaps)
// 		{
// 			if (controllerMap.ContainsAction(actionNameText.text))
// 			{
// 				ElementAssignment elementAssignment = new ElementAssignment();
// 				controllerMap.ReplaceElementMap (elementAssignment);
// 			}
// 		}
// 	}
// }