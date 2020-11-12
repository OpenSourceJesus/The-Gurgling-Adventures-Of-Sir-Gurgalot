using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System.Reflection;
using UnityEngine.UI;
using System.IO;
using System;

namespace TGAOSG
{
	[ExecuteAlways]
	public class SaveAndLoadManager : SingletonMonoBehaviour<SaveAndLoadManager>
	{
		[HideInInspector]
		public SaveAndLoadObject[] saveAndLoadObjects;
		public static SavedObjectEntry[] savedObjectEntries;
		public static Dictionary<string, SaveAndLoadObject> saveAndLoadObjectTypeDict = new Dictionary<string, SaveAndLoadObject>();
		public TemporaryDisplayObject displayOnSave;
		public Text saveText;
		public bool debugMode;
		public string debugFilePath;
		public List<string> debugFileContents = new List<string>();
		public const string KEY_NAME_AND_ACCOUNT_SEPEARATOR = "|";
#if UNITY_EDITOR
		public bool saveFromDebugFile;
		
		public virtual void OnEnable ()
		{
			saveAndLoadObjects = FindObjectsOfType<SaveAndLoadObject>();
		}
#endif

		public virtual void Awake ()
		{
			if (debugMode)
			{
				if (!debugFilePath.Contains(Application.dataPath))
					debugFilePath = Application.dataPath + debugFilePath;
				if (MainMenu.GetInstance() != null)
				{
					for (int i = 1; i <= MainMenu.instance.accountCount; i ++)
					{
						if (!File.Exists(debugFilePath + i + ".txt"))
							File.CreateText(debugFilePath + i + ".txt");
					}
				}
			}
		}
		
		public override void Start ()
		{
			base.Start ();
			saveAndLoadObjectTypeDict.Clear();
			SaveAndLoadObject saveAndLoadObject;
			List<SavedObjectEntry> _savedObjectEntries = new List<SavedObjectEntry>();
			for (int i = 0; i < saveAndLoadObjects.Length; i ++)
			{
				saveAndLoadObject = saveAndLoadObjects[i];
				saveAndLoadObject.Init ();
				_savedObjectEntries.AddRange(saveAndLoadObject.saveEntries);
			}
			savedObjectEntries = _savedObjectEntries.ToArray();
			if (GameManager.GetInstance().gameObject.scene.buildIndex == PlayerPrefs.GetInt("Scene" + SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPEARATOR + GameManager.accountNumber, 0))
			{
				if (debugMode)
				{
					debugFileContents.Clear();
					debugFileContents.AddRange(File.ReadAllLines(debugFilePath + GameManager.accountNumber + ".txt"));
				}
				for (int i = 0; i < savedObjectEntries.Length; i ++)
					savedObjectEntries[i].Load ();
			}
			GameManager.instance.SetGosActive ();
		}

#if UNITY_EDITOR
		public virtual void Update ()
		{
			if (!saveFromDebugFile)
				return;
			saveFromDebugFile = false;
			debugFileContents.Clear();
			debugFileContents.AddRange(File.ReadAllLines(debugFilePath + GameManager.accountNumber + ".txt"));
			SavedObjectEntry savedObjectEntry;
			string jsonString;
			string debugFileLine;
			int indexOfJsonString;
			for (int i = 0; i < savedObjectEntries.Length; i ++)
			{
				savedObjectEntry = savedObjectEntries[i];
				debugFileContents.RemoveAt(0);
				foreach (PropertyInfo property in savedObjectEntry.properties)
				{
					debugFileLine = debugFileContents[0];
					indexOfJsonString = debugFileLine.IndexOf(": ");
					if (indexOfJsonString != -1)
					{
						jsonString = debugFileLine.Substring(indexOfJsonString + 1);
						PlayerPrefsExtensions.SetString(savedObjectEntry.PlayerPrefsExtensionsKeysPrefix + property.Name, jsonString);
					}
				}
				foreach (FieldInfo field in savedObjectEntry.fields)
				{
					debugFileLine = debugFileContents[0];
					indexOfJsonString = debugFileLine.IndexOf(": ");
					if (indexOfJsonString != -1)
					{
						jsonString = debugFileLine.Substring(indexOfJsonString + 1);
						PlayerPrefsExtensions.SetString(savedObjectEntry.PlayerPrefsExtensionsKeysPrefix + field.Name, jsonString);
					}
				}
			}
		}
#endif
		
		public virtual void Save ()
		{
			if (instance != this)
			{
				instance.Save ();
				return;
			}
			debugFileContents.Clear();
			for (int i = 0; i < savedObjectEntries.Length; i ++)
				savedObjectEntries[i].Save ();
			PlayerPrefsExtensions.SetInt("Scene" + KEY_NAME_AND_ACCOUNT_SEPEARATOR + GameManager.accountNumber, GameManager.instance.gameObject.scene.buildIndex);
			StartCoroutine(displayOnSave.DisplayRoutine ());
			if (debugMode)
				File.WriteAllLines(debugFilePath + GameManager.accountNumber + ".txt", debugFileContents.ToArray());
		}
		
		public virtual void Load ()
		{
			if (instance != this)
			{
				instance.Load ();
				return;
			}
			GameManager.instance.LoadLevelImmediate (PlayerPrefs.GetInt("Scene" + KEY_NAME_AND_ACCOUNT_SEPEARATOR + GameManager.accountNumber, 0));
		}
		
		public class SavedObjectEntry
		{
			public SaveAndLoadObject saveAndLoadObject;
			public ISavableAndLoadable saveableAndLoadable;
			public PropertyInfo[] properties;
			public FieldInfo[] fields;
			public const string VALUE_SEPERATOR = ", ";
			public const string ACCOUNT_AND_ID_SEPERATOR = ": ";
			public string PlayerPrefsExtensionsKeysPrefix
			{
				get
				{
					return GameManager.accountNumber + ACCOUNT_AND_ID_SEPERATOR + saveableAndLoadable.UniqueId + VALUE_SEPERATOR;
				}
			}
			
			public SavedObjectEntry ()
			{
			}
			
			public virtual void Save ()
			{
				if (SaveAndLoadManager.instance.debugMode)
					SaveAndLoadManager.instance.debugFileContents.Add(saveableAndLoadable.Name);
				string jsonString;
				foreach (PropertyInfo property in properties)
				{
					jsonString = JsonUtility.ToJson(property.GetValue(saveableAndLoadable, null));
					PlayerPrefsExtensions.SetString(PlayerPrefsExtensionsKeysPrefix + property.Name, jsonString);
					if (SaveAndLoadManager.instance.debugMode)
						SaveAndLoadManager.instance.debugFileContents.Add(property.Name + ACCOUNT_AND_ID_SEPERATOR + jsonString);
				}
				foreach (FieldInfo field in fields)
				{
					jsonString = JsonUtility.ToJson(field.GetValue(saveableAndLoadable));
					PlayerPrefsExtensions.SetString(PlayerPrefsExtensionsKeysPrefix + field.Name, jsonString);
					if (SaveAndLoadManager.instance.debugMode)
						SaveAndLoadManager.instance.debugFileContents.Add(field.Name + ACCOUNT_AND_ID_SEPERATOR + jsonString);
				}
			}
			
			public virtual void Load ()
			{
				object value;
				if (!SaveAndLoadManager.instance.debugMode)
				{
					foreach (PropertyInfo property in properties)
					{
						value = JsonUtility.FromJson(PlayerPrefs.GetString(PlayerPrefsExtensionsKeysPrefix + property.Name, JsonUtility.ToJson(property.GetValue(saveableAndLoadable, null))), property.PropertyType);
						property.SetValue(saveableAndLoadable, value, null);
					}
					foreach (FieldInfo field in fields)
					{
						value = JsonUtility.FromJson(PlayerPrefs.GetString(PlayerPrefsExtensionsKeysPrefix + field.Name, JsonUtility.ToJson(field.GetValue(saveableAndLoadable))), field.FieldType);
						field.SetValue(saveableAndLoadable, value);
					}
				}
				else
				{
					SaveAndLoadManager.instance.debugFileContents.RemoveAt(0);
					string debugFileLine;
					int indexOfJsonString;
					foreach (PropertyInfo property in properties)
					{
						debugFileLine = SaveAndLoadManager.instance.debugFileContents[0];
						indexOfJsonString = debugFileLine.IndexOf(ACCOUNT_AND_ID_SEPERATOR);
						if (indexOfJsonString != -1)
						{
							value = JsonUtility.FromJson(debugFileLine.Substring(indexOfJsonString + 1), property.PropertyType);
							property.SetValue(saveableAndLoadable, value, null);
							SaveAndLoadManager.instance.debugFileContents.RemoveAt(0);
						}
					}
					foreach (FieldInfo field in fields)
					{
						debugFileLine = SaveAndLoadManager.instance.debugFileContents[0];
						indexOfJsonString = debugFileLine.IndexOf(ACCOUNT_AND_ID_SEPERATOR);
						if (indexOfJsonString != -1)
						{
							value = JsonUtility.FromJson(debugFileLine.Substring(indexOfJsonString + 1), field.FieldType);
							field.SetValue(saveableAndLoadable, value);
							SaveAndLoadManager.instance.debugFileContents.RemoveAt(0);
						}
					}
				}
			}
		}
	}
}

[Serializable]
public class _ushort
{
	public ushort value;

	public _ushort ()
	{
	}

	public _ushort (ushort value)
	{
		this.value = value;
	}
}

[Serializable]
public class _string
{
	public string value = "";

	public _string ()
	{
	}

	public _string (string value)
	{
		this.value = value;
	}
}

[Serializable]
public class _bool
{
	public bool value;

	public _bool ()
	{
	}

	public _bool (bool value)
	{
		this.value = value;
	}
}

[Serializable]
public class _float
{
	public float value;

	public _float ()
	{
	}

	public _float (float value)
	{
		this.value = value;
	}
}

[Serializable]
public class _uint
{
	public uint value;

	public _uint ()
	{
	}

	public _uint (uint value)
	{
		this.value = value;
	}
}