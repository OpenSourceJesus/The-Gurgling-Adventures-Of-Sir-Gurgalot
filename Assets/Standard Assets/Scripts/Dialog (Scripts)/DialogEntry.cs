using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus2;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using Menu = Fungus2.Menu;

namespace TGAOSG.Dialog
{
	public class DialogEntry : Graphs.Node
	{
		[HideInInspector]
		public DialogEntry prefab;
		string assetPath;
		public bool deleteEntry;
		public string sceneName;
		[Multiline]
		public string content;
		public AudioClip voiceOverClip;
		
		#if UNITY_EDITOR
		public virtual void Refresh ()
		{
			if (prefab != null)
			{
				assetPath = DialogManager.GetInstance().dialogData.dialogFolderPath + "/" + prefab.name + ".prefab";
				AssetDatabase.DeleteAsset(assetPath);
				if (deleteEntry)
				{
					DestroyImmediate(gameObject);
					return;
				}
			}
			assetPath = DialogManager.GetInstance().dialogData.dialogFolderPath + "/" + name + ".prefab";
			if (prefab == null || prefab.name != name)
				prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, assetPath).GetComponent<DialogEntry>();
			ApplyData ();
		}
		
		public virtual void ApplyData ()
		{
			List<string> scenePaths = new List<string>();
			scenePaths.AddRange(BuildManager.GetScenePathsInBuild());
			foreach (string scenePath in scenePaths)
			{
				if (!scenePath.Contains("Init"))
				{
					Scene openedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
					Say[] says = FindObjectsOfType<Say>();
					foreach (Say say in says)
					{
						do
						{
							say.uniqueId = Random.Range(int.MinValue, int.MaxValue);
						}
						while (GameManager.identifiableIds.Contains(say.uniqueId));
						if (say.entryName == prefab.name)
						{
							say.storyText = prefab.content;
							say.voiceOverClip = prefab.voiceOverClip;
						}
					}
					Menu[] menus = FindObjectsOfType<Menu>();
					foreach (Menu menu in menus)
					{
						do
						{
							menu.uniqueId = Random.Range(int.MinValue, int.MaxValue);
						}
						while (GameManager.identifiableIds.Contains(menu.uniqueId));
						if (menu.entryName == prefab.name)
							menu.SetStandardText(prefab.content);
					}
					EditorSceneManager.SaveScene(openedScene);
					EditorSceneManager.CloseScene(openedScene, true);
				}
			}
		}
		#endif
	}
}