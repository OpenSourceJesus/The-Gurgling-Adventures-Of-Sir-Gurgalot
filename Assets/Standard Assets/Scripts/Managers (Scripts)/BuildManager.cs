using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using TAoKR.Story;
using TAoKR.Dialog;
using TAoKR.Analytics;
using System.IO;
using UnityEngine.UI;
using LanguageTranslation;
#endif

namespace TAoKR
{
	[ExecuteAlways]
	public class BuildManager : SingletonMonoBehaviour<BuildManager>
	{
#if UNITY_EDITOR
		public BuildAction[] buildActions;
		static BuildPlayerOptions buildOptions;
		#endif
		public int versionIndex;
		public string versionNumberPrefix;
#if UNITY_EDITOR
		public Text versionNumberText;
		public BuildManager buildManagerPrefab;
		public ConfigurationManager configurationManagerPrefab;
		public AnalyticsManager analyticsManagerPrefab;
		public SaveAndLoadManager saveAndLoadManagerPrefab;
		public LanguageManager languageManagerPrefab;
		#endif
		
#if UNITY_EDITOR
		public static string[] GetScenePathsInBuild ()
		{
			List<string> scenePathsInBuild = new List<string>();
			string scenePath = null;
			for (int i = 0; i < EditorBuildSettings.scenes.Length; i ++)
			{
				scenePath = EditorBuildSettings.scenes[i].path;
				if (EditorBuildSettings.scenes[i].enabled)
					scenePathsInBuild.Add(scenePath);
			}
			return scenePathsInBuild.ToArray();
		}

		public static string[] GetAllScenePaths ()
		{
			List<string> scenePathsInBuild = new List<string>();
			for (int i = 0; i < EditorBuildSettings.scenes.Length; i ++)
				scenePathsInBuild.Add(EditorBuildSettings.scenes[i].path);
			return scenePathsInBuild.ToArray();
		}
		
		[MenuItem("Build/Make Builds")]
		public static void Build ()
		{
			GetInstance()._Build ();
		}

		public virtual void _Build ()
		{
			buildManagerPrefab.versionIndex ++;
			foreach (BuildAction buildAction in buildActions)
			{
				if (buildAction.enabled)
					buildAction.Do ();
			}
		}
		
		[Serializable]
		public class BuildAction
		{
			public string name;
			public bool enabled;
			public BuildTarget target;
			public string locationPath;
			public BuildOptions[] options;
			public bool updateQuests;
			public bool updateDialog;
			public bool updateTranslations;
			public bool moveCrashHandler;
			public bool makeZip;
			public string directoryToZip;
			public string zipLocationPath;
			
			public virtual void Do ()
			{
				bool previousSaveAndLoadManagerDebugMode = GetInstance().saveAndLoadManagerPrefab.debugMode;
				if (instance.versionNumberText != null)
					instance.versionNumberText.text = instance.versionNumberPrefix + DateTime.Now.Date.ToString("MMdd");
				instance.configurationManagerPrefab.canvas.gameObject.SetActive(false);
				instance.languageManagerPrefab.canvas.gameObject.SetActive(false);
				instance.saveAndLoadManagerPrefab.debugMode = false;
				if (updateQuests)
					QuestManager._UpdateQuests ();
				if (updateDialog)
					DialogManager._UpdateDialog ();
				if (updateTranslations)
					LanguageManager._SerializeTranslations ();
				buildOptions = new BuildPlayerOptions();
				buildOptions.scenes = GetScenePathsInBuild();
				buildOptions.target = target;
				buildOptions.locationPathName = locationPath;
				foreach (BuildOptions option in options)
					buildOptions.options |= option;
				BuildPipeline.BuildPlayer(buildOptions);
				instance.configurationManagerPrefab.canvas.gameObject.SetActive(true);
				instance.languageManagerPrefab.canvas.gameObject.SetActive(true);
				instance.saveAndLoadManagerPrefab.debugMode = previousSaveAndLoadManagerDebugMode;
				AssetDatabase.Refresh();
				if (moveCrashHandler)
				{
					string extrasPath = locationPath + Path.DirectorySeparatorChar + "Extras";
					string crashHandlerFileName = "UnityCrashHandler64.exe";
					if (!Directory.Exists(extrasPath))
						Directory.CreateDirectory(extrasPath);
					else if (File.Exists(extrasPath + Path.DirectorySeparatorChar + crashHandlerFileName))
						File.Delete(extrasPath + Path.DirectorySeparatorChar + crashHandlerFileName);
					File.Move(locationPath + Path.DirectorySeparatorChar + crashHandlerFileName, extrasPath + Path.DirectorySeparatorChar + crashHandlerFileName);
				}
				if (makeZip)
				{
					File.Delete(zipLocationPath);
					DirectoryCompressionOperations.CompressDirectory (directoryToZip, zipLocationPath);
				}
			}
		}
#endif
	}
}