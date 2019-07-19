using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;
using SavedObjectEntry = TAoKR.SaveAndLoadManager.SavedObjectEntry;
using Random = UnityEngine.Random;

namespace TAoKR
{
	[ExecuteAlways]
	public class SaveAndLoadObject : MonoBehaviour, IIdentifiable
	{
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		public int uniqueId;
		public int UniqueId
		{
			get
			{
				return uniqueId;
			}
			set
			{
				uniqueId = value;
			}
		}
		public Transform[] saveableChildren;
		public ISavableAndLoadable[] saveables;
		public string typeId;
		public SavedObjectEntry[] saveEntries;
		
#if UNITY_EDITOR
		public virtual void Start ()
		{
			if (Application.isPlaying)
				return;
			List<Transform> saveableChildrenList = new List<Transform>();
			saveableChildrenList.AddRange(saveableChildren);
			Transform trs = GetComponent<Transform>();
			if (!saveableChildrenList.Contains(trs) && GetComponentsInChildren<ISavableAndLoadable>().Length > 0)
			{
				saveableChildrenList.Add(trs);
				saveableChildren = saveableChildrenList.ToArray();
			}
		}
#endif

		public virtual void Init ()
		{
			List<ISavableAndLoadable> saveablesList = new List<ISavableAndLoadable>();
			foreach (Transform saveableChild in saveableChildren)
				saveablesList.AddRange(saveableChild.GetComponentsInChildren<ISavableAndLoadable>());
			saveables = saveablesList.ToArray();
			SaveAndLoadObject sameTypeObj;
			if (!SaveAndLoadManager.saveAndLoadObjectTypeDict.TryGetValue(typeId, out sameTypeObj))
			{
				saveEntries = new SavedObjectEntry[saveables.Length];
				for (int i = 0; i < saveables.Length; i ++)
				{
					SavedObjectEntry saveEntry = new SavedObjectEntry();
					saveEntry.saveAndLoadObject = this;
					saveEntry.saveableAndLoadable = saveables[i];
					List<PropertyInfo> saveProperties = new List<PropertyInfo>();
					saveProperties.AddRange(saveEntry.saveableAndLoadable.GetType().GetProperties());
					for (int i2 = 0; i2 < saveProperties.Count; i2 ++)
					{
						SaveAndLoadValue saveAndLoadValue = Attribute.GetCustomAttribute(saveProperties[i2], typeof(SaveAndLoadValue)) as SaveAndLoadValue;
						if (saveAndLoadValue == null)
						{
							saveProperties.RemoveAt(i2);
							i2 --;
						}
					}
					saveEntry.properties = saveProperties.ToArray();
					
					List<FieldInfo> saveFields = new List<FieldInfo>();
					saveFields.AddRange(saveEntry.GetType().GetFields());
					for (int i2 = 0; i2 < saveFields.Count; i2 ++)
					{
						SaveAndLoadValue saveAndLoadValue = Attribute.GetCustomAttribute(saveFields[i2], typeof(SaveAndLoadValue)) as SaveAndLoadValue;
						if (saveAndLoadValue == null)
						{
							saveFields.RemoveAt(i2);
							i2 --;
						}
					}
					saveEntry.fields = saveFields.ToArray();
					saveEntries[i] = saveEntry;
				}
				SaveAndLoadManager.saveAndLoadObjectTypeDict.Add(typeId, this);
			}
			else
			{
				saveEntries = sameTypeObj.saveEntries;
				SavedObjectEntry saveEntry;
				for (int i = 0; i < saveEntries.Length; i ++)
				{
					saveEntry = saveEntries[i];
					saveEntry.saveableAndLoadable = saveables[i];
					saveEntry.saveAndLoadObject = this;
				}
			}
		}
	}
}