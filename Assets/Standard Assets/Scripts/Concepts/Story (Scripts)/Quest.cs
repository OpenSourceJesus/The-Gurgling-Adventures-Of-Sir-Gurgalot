using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Graphs;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TGAOSG.Story
{
	public class Quest : UnlockableNode
	{
		public Event[] events;
		[HideInInspector]
		public int completionCount;
		public int minCompletionCount = 1;
		public int maxCompletionCount = 1;
		//[HideInInspector]
		public Quest questPrefab;
		string assetPath;
		public string[] locations;
		public bool deleteQuest;
		
		#if UNITY_EDITOR
		public void Refresh ()
		{
			if (questPrefab != null)
			{
				assetPath = QuestManager.GetInstance().questData.questsFolderPath + "/" + questPrefab.name + ".prefab";
				AssetDatabase.DeleteAsset(assetPath);
				if (deleteQuest)
				{
					DestroyImmediate(gameObject);
					return;
				}
			}
			assetPath = QuestManager.GetInstance().questData.questsFolderPath + "/" + name + ".prefab";
			if (questPrefab == null || questPrefab.name != name)
				questPrefab = PrefabUtility.SaveAsPrefabAsset(gameObject, assetPath).GetComponent<Quest>();
		}
		#endif
		
		public override void Unlock ()
		{
			base.Unlock ();
			foreach (Event _event in events)
			{
				if (_event.type == EventType.OnUnlock)
					_event.Trigger ();
			}
		}
		
		public override void Traverse ()
		{
			base.Traverse ();
			foreach (Event _event in events)
			{
				if (_event.type == EventType.OnComplete)
					_event.Trigger ();
			}
		}
		
		[Serializable]
		public class Event
		{
			public EventType type;
			public EventActionEntry[] actions;
			
			public void Trigger ()
			{
				foreach (EventActionEntry action in actions)
				{
					switch (action.activate)
					{
						case ActivateState.Activate:
							GameManager.instance.ActivateGoForever (action.goName);
							break;
						case ActivateState.Deactivate:
							GameManager.instance.DeactivateGoForever (action.goName);
							break;
						//case ActivateState.Toggle:
						//	if (GameManager.enabledGosString.Contains(action.goName))
						//		GameManager.instance.DeactivateGoForever (action.goName);
						//	else if (GameManager.disabledGosString.Contains(action.goName))
						//		GameManager.instance.ActivateGoForever (action.goName);
						//	break;
					}
					GameManager.instance.SetGosActive ();
				}
			}
		}
		
		[Serializable]
		public class EventActionEntry
		{
			public string goName;
			public ActivateState activate;
		}
		
		[Serializable]
		public enum ActivateState
		{
			Activate,
			Deactivate,
			//Toggle
		}
		
		public enum EventType
		{
			OnUnlock,
			OnStart,
			OnComplete
		}
	}
}