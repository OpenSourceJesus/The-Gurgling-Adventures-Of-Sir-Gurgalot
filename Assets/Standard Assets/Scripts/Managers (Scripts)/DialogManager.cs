using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace TAoKR.Dialog
{
	[ExecuteAlways]
	public class DialogManager : SingletonMonoBehaviour<DialogManager>
	{
		public DialogData dialogData;
		#if UNITY_EDITOR
		public string dialogScenePath;
		#endif
		
		public override void Start ()
		{
			base.Start ();
			if (dialogData != null)
				DialogData.instance = dialogData;
		}
		
		#if UNITY_EDITOR
		[MenuItem("Dialog/Update Dialog")]
		public static void _UpdateDialog ()
		{
			GetInstance().UpdateDialog ();
		}

		public virtual void UpdateDialog ()
		{
			EditorSceneManager.OpenScene(dialogScenePath, OpenSceneMode.Additive);
			dialogData.allEntries.Clear();
			DialogEntry[] entries = FindObjectsOfType<DialogEntry>();
			List<NodeConnection> connections = new List<NodeConnection>();
			foreach (DialogEntry entry in entries)
			{
				for (int i = 0; i < entry.connections.Length; i ++)
				{
					NodeConnection connection = entry.connections[i];
					if (connections.Count > 0)
					{
						foreach (NodeConnection connection2 in connections)
						{
							if (connection != null && connection.start == connection2.start && connection.end == connection2.end)
							{
								DestroyImmediate(connection.gameObject);
								i --;
								break;
							}
						}
						if (connection != null)
							connections.Add(connection);
					}
					else
						connections.Add(connection);
				}
				entry.connections = entry.GetComponentsInChildren<NodeConnection>();
				entry.Refresh ();
				dialogData.allEntries.Add(entry.prefab);
			}
			foreach (DialogEntry entry in entries)
			{
				entry.prefab.connections = entry.prefab.GetComponentsInChildren<NodeConnection>();
				for (int i = 0; i < entry.prefab.connections.Length; i ++)
					entry.prefab.connections[i].end = (entry.connections[i].end as DialogEntry).prefab;
			}
		}
		#endif
		
		public static bool EntryExists (string entryName)
		{
			return GetEntry(entryName) != null;
		}
		
		public static DialogEntry GetEntry (string entryName)
		{
			foreach (DialogEntry entry in DialogData.instance.allEntries)
			{
				if (entry.name == entryName)
					return entry;
			}
			return null;
		}
	}
}