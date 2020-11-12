using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace TGAOSG.Story
{
	[ExecuteInEditMode]
	public class QuestManager : SingletonMonoBehaviour<QuestManager>
	{
		public static List<Quest> currentQuests = new List<Quest>();
		public StoryData questData;
		#if UNITY_EDITOR
		public string questScenePath;
		#endif
		
		public override void Start ()
		{
			base.Start ();
			if (questData != null)
				StoryData.instance = questData;
		}
		
		#if UNITY_EDITOR
		[MenuItem("Quests/Update Quests")]
		public static void _UpdateQuests ()
		{
			GetInstance().UpdateQuests ();
		}

		public virtual void UpdateQuests ()
		{
			EditorSceneManager.OpenScene(questScenePath, OpenSceneMode.Additive);
			questData.allQuests.Clear();
			Quest[] quests = FindObjectsOfType<Quest>();
			foreach (Quest quest in quests)
			{
				quest.Refresh ();
				questData.allQuests.Add(quest.questPrefab);
				quest.connections = quest.GetComponentsInChildren<NodeConnection>();
			}
			foreach (Quest quest in quests)
			{
				quest.questPrefab.connections = quest.questPrefab.GetComponentsInChildren<NodeConnection>();
				for (int i = 0; i < quest.questPrefab.connections.Length; i ++)
					quest.questPrefab.connections[i].end = (quest.connections[i].end as Quest).questPrefab;
			}
		}
		#endif
		
		public static bool QuestExists (string questName)
		{
			return GetQuest(questName) != null;
		}
		
		public static Quest GetQuest (string questName)
		{
			foreach (Quest quest in StoryData.instance.allQuests)
			{
				if (quest.name == questName)
					return quest;
			}
			return null;
		}
		
		public virtual void StartQuest (string questName)
		{
			StartQuest (GetQuest(questName));
		}
		
		public virtual void StartQuest (Quest quest)
		{
			currentQuests.Add(quest);
			foreach (Quest.Event _event in quest.events)
			{
				if (_event.type == Quest.EventType.OnStart)
					_event.Trigger ();
			}
			GameObject location = GameObject.Find(quest.locations[0]);
			if (location != null)
			{
				ObjectiveGuider.instance.location = location.transform;
				ObjectiveGuider.instance.gameObject.SetActive(true);
			}
		}
		
		public virtual void CompleteQuest (string questName)
		{
			CompleteQuest (GetQuest(questName));
		}
		
		public virtual void CompleteQuest (Quest quest)
		{
			quest.completionCount ++;
			if (quest.completionCount >= quest.minCompletionCount && quest.completionCount <= quest.maxCompletionCount)
			{
				ObjectiveGuider.instance.gameObject.SetActive(false);
				quest.Traverse ();
				currentQuests.Remove(quest);
			}
		}
	}
}