using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class GamepadSwitch : MonoBehaviour
	{
		public static List<GamepadSwitch> instances = new List<GamepadSwitch>();
		public GameObject[] toggleGos = new GameObject[0];
		
		void Awake ()
		{
			if (InputManager.UsingGamepad)
				ToggleGos ();
			instances.Add(this);
		}
		
		public void ToggleGos ()
		{
			for (int i = 0; i < toggleGos.Length; i ++)
			{
				GameObject go = toggleGos[i];
				go.SetActive(!go.activeSelf);
			}
		}

		void OnDestroy ()
		{
			instances.Remove(this);
		}
	}
}