using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using TGAOSG.Story;
using TGAOSG.Dialog;
using TGAOSG.Analytics;
using System.IO;
using UnityEngine.UI;
using LanguageTranslation;
using Extensions;
#endif

namespace TGAOSG
{
	[ExecuteAlways]
	public class BuildManager : SingletonMonoBehaviour<BuildManager>
	{
		public int versionIndex;
		public string versionNumberPrefix;
#if UNITY_EDITOR
		public BuildAction[] buildActions;
		static BuildPlayerOptions buildOptions;
		public Text versionNumberText;
		public BuildManager buildManagerPrefab;
		public ConfigurationManager configurationManagerPrefab;
		public AnalyticsManager analyticsManagerPrefab;
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
			public bool excludeScenes;
			public string[] scenes;
			
			public virtual void Do ()
			{
				Directory.CreateDirectory(locationPath);
				if (instance.versionNumberText != null)
					instance.versionNumberText.text = instance.versionNumberPrefix + DateTime.Now.Date.ToString("MMdd");
				instance.configurationManagerPrefab.canvas.gameObject.SetActive(false);
				instance.languageManagerPrefab.canvas.gameObject.SetActive(false);
				if (updateQuests)
					QuestManager._UpdateQuests ();
				if (updateDialog)
					DialogManager._UpdateDialog ();
				if (updateTranslations)
					LanguageManager._SerializeTranslations ();
				buildOptions = new BuildPlayerOptions();
				if (excludeScenes)
					buildOptions.scenes = GetScenePathsInBuild().RemoveEach(scenes);
				else
					buildOptions.scenes = scenes;
				buildOptions.target = target;
				buildOptions.locationPathName = locationPath;
				foreach (BuildOptions option in options)
					buildOptions.options |= option;
				BuildPipeline.BuildPlayer(buildOptions);
				instance.configurationManagerPrefab.canvas.gameObject.SetActive(true);
				instance.languageManagerPrefab.canvas.gameObject.SetActive(true);
				AssetDatabase.Refresh();
				if (moveCrashHandler)
				{
					string extrasPath = locationPath + Path.DirectorySeparatorChar + "Extras";
					string crashHandlerFileName = "UnityCrashHandler32.exe";
					if (target == BuildTarget.StandaloneWindows64)
						crashHandlerFileName = "UnityCrashHandler64.exe";
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