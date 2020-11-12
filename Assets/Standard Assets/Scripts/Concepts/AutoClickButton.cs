using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

namespace TAoKR
{
	public class AutoClickButton : MonoBehaviour
	{
		public Button button;
		public int initTriggersUntilClick = 1;
		int triggersUntilClick;
		public bool onEnable;
		public bool onAwake;
		public bool onStart;
		public bool onUpdate;
		public bool onDisable;
		public bool onDestroy;
		public bool onLevelLoaded;
		public bool onLevelUnloaded;
		public bool onTriggerEnter2D;
		public bool onTriggerStay2D;
		public bool onTriggerExit2D;
		public Hotkey[] hotkeys = new Hotkey[0];
		bool hotkeyIsPressed;
		
		public virtual void OnEnable ()
		{
			if (onEnable)
				Trigger ();
		}
		
		public virtual void Awake ()
		{
			triggersUntilClick = initTriggersUntilClick;
			if (onAwake)
				Trigger ();
			if (onLevelLoaded)
				SceneManager.sceneLoaded += LevelLoaded;
			if (onLevelUnloaded)
				SceneManager.sceneUnloaded += LevelUnloaded;
		}
		
		public virtual void Start ()
		{
			if (onStart)
				Trigger ();
		}
		
		public virtual void Update ()
		{
			if (onUpdate)
				Trigger ();
			foreach (Hotkey hotkey in hotkeys)
			{
				foreach (Hotkey.ButtonEntry buttonEntry in hotkey.requiredButtons)
				{
					switch (buttonEntry.pressState)
					{
					case HotkeyState.Down:
						hotkeyIsPressed = Input.GetKeyDown(buttonEntry.key);
						break;
					case HotkeyState.Held:
						hotkeyIsPressed = Input.GetKey(buttonEntry.key);
						break;
					case HotkeyState.Up:
						hotkeyIsPressed = Input.GetKeyUp(buttonEntry.key);
						break;
					}
					if (!hotkeyIsPressed)
						break;
				}
				if (hotkeyIsPressed)
				{
					Trigger ();
					return;
				}
			}
		}
		
		public virtual void OnDisable ()
		{
			if (onDisable)
				Trigger ();
			if (onLevelLoaded)
				SceneManager.sceneLoaded -= LevelLoaded;
			if (onLevelUnloaded)
				SceneManager.sceneUnloaded -= LevelUnloaded;
		}
		
		public virtual void OnDestroy ()
		{
			if (onDestroy)
				Trigger ();
		}
		
		public virtual void OnTriggerEnter2D (Collider2D other)
		{
			if (onTriggerEnter2D)
				Trigger ();
		}
		
		public virtual void OnTriggerStay2D (Collider2D other)
		{
			if (onTriggerStay2D)
				Trigger ();
		}
		
		public virtual void OnTriggerExit2D (Collider2D other)
		{
			if (onTriggerExit2D)
				Trigger ();
		}
		
		public virtual void LevelLoaded (Scene scene, LoadSceneMode loadMode)
		{
			Trigger ();
		}
		
		public virtual void LevelUnloaded (Scene scene)
		{
			Trigger ();
		}
		
		public virtual void Trigger ()
		{
			if (!button.gameObject.activeSelf)
				return;
			triggersUntilClick --;
			if (triggersUntilClick == 0)
				button.onClick.Invoke ();
		}
		
		public virtual void Restart (int triggersUntilClick)
		{
			this.triggersUntilClick = triggersUntilClick;
		}
		
		public virtual void Restart ()
		{
			Restart (initTriggersUntilClick);
		}
		
		[Serializable]
		public class Hotkey
		{
			public ButtonEntry[] requiredButtons;
			
			public virtual bool IsPressed ()
			{
				bool output = true;
				foreach (ButtonEntry requiredButton in requiredButtons)
				{
					if (!requiredButton.IsPressed ())
					{
						output = false;
						break;
					}
				}
				return output;
			}
			
			[Serializable]
			public class ButtonEntry
			{
				public string keyString;
				public KeyCode key;
				public HotkeyState pressState;
				
				public virtual bool IsPressed ()
				{
					bool output = false;
					switch (pressState)
					{
						case HotkeyState.Down:
							output = (!string.IsNullOrEmpty(keyString) && Input.GetKeyDown(keyString)) || Input.GetKeyDown(key);
							break;
						case HotkeyState.Held:
							output = (!string.IsNullOrEmpty(keyString) && Input.GetKey(keyString)) || Input.GetKey(key);
							break;
						case HotkeyState.Up:
							output = (!string.IsNullOrEmpty(keyString) && Input.GetKeyUp(keyString)) || Input.GetKeyUp(key);
							break;
					}
					return output;
				}
			}
		}
		
		public enum HotkeyState
		{
			Down,
			Held,
			Up
		}
	}
}