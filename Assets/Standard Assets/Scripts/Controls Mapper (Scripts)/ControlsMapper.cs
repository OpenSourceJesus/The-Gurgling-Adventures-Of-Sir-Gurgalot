// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TGAOSG;
// using UnityEngine.UI;
// using InputManager = TGAOSG.InputManager;

// public class ControlsMapper : SingletonMonoBehaviour<ControlsMapper>
// {
// 	public ButtonMapper buttonMapperPrefab;
// 	public Transform buttonMappersParent;

// 	public override void Start ()
// 	{
// 		base.Start ();
// 		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
// 		{
// 			foreach (ActionElementMap actionElementMap in controllerMap.AllMaps)
// 			{
// 				ButtonMapper buttonMapper = Instantiate(buttonMapperPrefab, buttonMappersParent);
// 				buttonMapper.actionNameText.text = ReInput.mapping.GetAction(actionElementMap.actionId).name;
// 				buttonMapper.buttonNameText.text = actionElementMap.elementIdentifierName;
// 			}
// 		}
// 	}

// 	public virtual void ResetToDefaults ()
// 	{
// 		InputManager.inputter.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
// 		InputManager.inputter.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
// 		InputManager.inputter.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);
// 	}
// }