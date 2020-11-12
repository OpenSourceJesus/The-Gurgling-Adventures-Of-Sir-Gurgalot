using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;
using System;
using TAoKR;
using ClassExtensions;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Fungus;
using Menu = Fungus.Menu;
#endif

namespace LanguageTranslation
{
	[ExecuteAlways]
	public class LanguageManager : SingletonMonoBehaviour<LanguageManager>
	{
		[HideInInspector]
		public string translationsFolderPath;
		public LanguageTranslator translator;
		public string translationModel;
		public string versionDate;
		public string iamApiKey;
		public Dictionary<string, string> translations = new Dictionary<string, string>();
		public Language[] languages;
		public static Language currentLanguage;
		#if UNITY_EDITOR
		static StreamWriter[] fileWriters;
		#endif
		public static bool shouldInitialize = true;
		public Canvas canvas;

		public override void Start ()
		{
			base.Start ();
			shouldInitialize = true;
		}

		public virtual IEnumerator Init ()
		{
			translationsFolderPath = Application.dataPath + "/Translations";
			LogSystem.InstallDefaultReactors();
			TokenOptions iamTokenOptions = new TokenOptions()
		    {
		        IamApiKey = iamApiKey
		    };
		    Credentials credentials = new Credentials(iamTokenOptions, "https://gateway.watsonplatform.net/language-translator/api");
		    yield return new WaitUntil(() => (credentials.HasIamTokenData()));
			translator = new LanguageTranslator(versionDate, credentials);
			currentLanguage = languages[0];
			foreach (Language language in languages)
			{
				if (string.IsNullOrEmpty(language.nativeSpelling))
				{
					translationModel = "en-" + language.abbreviation;
					Translate (language.name);
					yield return new WaitUntil(() => (translations.ContainsKey(language.name)));
					language.nativeSpelling = translations[language.name];
					translations.Remove(language.name);
				}
			}
		}

		public virtual void SetLanguage (Language language)
		{
			translationModel = "en-" + language.abbreviation;
			currentLanguage = language;
			foreach (_Text text in _Text.instances)
				text.Translate ();
		}

		public virtual void Translate (string englishText)
		{
			if (currentLanguage.name == "English")
				return;
			Dictionary<string, object> data = new Dictionary<string, object>();
			data.Add(englishText, englishText);
			translator.GetTranslation(OnTranslateSuccess, OnTranslateFail, englishText, translationModel, data);
		}

		public virtual void OnTranslateSuccess (Translations response, Dictionary<string, object> data)
		{
			IEnumerator enumerator = data.Keys.GetEnumerator();
			enumerator.MoveNext();
			translations.Add((string) enumerator.Current, response.translations[0].translation);
		}

		public virtual void OnTranslateFail (RESTConnector.Error error, Dictionary<string, object> data)
		{
			Debug.Log(error.ToString());
		}

		public virtual string GetSerializedTranslation (int uniqueId)
		{
			string[] translationsString = File.ReadAllLines(translationsFolderPath + "/" + currentLanguage.nativeSpelling + ".txt");
			string uniqueIdString;
			foreach (string translationString in translationsString)
			{
				uniqueIdString = translationString.SubstringStartEnd(translationString.IndexOf("("), translationString.IndexOf(")"));
				if (uniqueId == int.Parse(uniqueIdString))
					return translationString.StartAfter(": ");
			}
			return null;
		}

		#if UNITY_EDITOR
		[MenuItem("Language/Serialize Translations")]
		public static void _SerializeTranslations ()
		{
			GetInstance().SerializeTranslations ();
		}

		public virtual void SerializeTranslations ()
		{
			StopAllCoroutines();
			StartCoroutine(SerializeTranslationsRoutine ());
		}

		public virtual IEnumerator SerializeTranslationsRoutine ()
		{
			if (shouldInitialize)
			{
				shouldInitialize = false;
				yield return StartCoroutine(Init ());
			}
			if (!Directory.Exists(translationsFolderPath))
				Directory.CreateDirectory(translationsFolderPath);
			string translationFilePath;
			Language[] translationLanguages = new Language[languages.Length - 1];
			fileWriters = new StreamWriter[translationLanguages.Length];
			Language language;
			StreamWriter fileWriter = null;
			for (int i = 1; i < languages.Length; i ++)
			{
				language = languages[i];
				translationLanguages[i - 1] = language;
				translationFilePath = translationsFolderPath + "/" + language.nativeSpelling + ".txt";
				if (File.Exists(translationFilePath))
					File.Delete(translationFilePath);
				fileWriters[i - 1] = File.CreateText(translationFilePath);
			}
			_Text[] texts;
			string[] scenePaths = BuildManager.GetScenePathsInBuild();
			Scene openedScene = new Scene();
			Say[] says;
			Menu[] menus;
			foreach (string scenePath in scenePaths)
			{
				if (!Application.isPlaying)
					openedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
				else
					SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);
				texts = FindObjectsOfType<_Text>();
				foreach (_Text text in texts)
				{
					for (int i = 0; i < translationLanguages.Length; i ++)
					{
						language = translationLanguages[i];
						fileWriter = fileWriters[i];
						translationModel = "en-" + language.abbreviation;
						currentLanguage = language;
						Translate (text.text.text);
						yield return new WaitUntil(() => (translations.ContainsKey(text.text.text)));
						fileWriter.WriteLine("Text (" + text.uniqueId + "): " + translations[text.text.text]);
						translations.Remove(text.text.text);
					}
				}

				says = FindObjectsOfType<Say>();
				foreach (Say say in says)
				{
					for (int i = 0; i < translationLanguages.Length; i ++)
					{
						language = translationLanguages[i];
						fileWriter = fileWriters[i];
						translationModel = "en-" + language.abbreviation;
						currentLanguage = language;
						Translate (say.storyText);
						yield return new WaitUntil(() => (translations.ContainsKey(say.storyText)));
						fileWriter.WriteLine("Say (" + say.uniqueId + "): " + translations[say.storyText]);
						translations.Remove(say.storyText);
					}
				}

				menus = FindObjectsOfType<Menu>();
				foreach (Menu menu in menus)
				{
					for (int i = 0; i < translationLanguages.Length; i ++)
					{
						language = translationLanguages[i];
						fileWriter = fileWriters[i];
						translationModel = "en-" + language.abbreviation;
						currentLanguage = language;
						Translate (menu.GetStandardText());
						yield return new WaitUntil(() => (translations.ContainsKey(menu.GetStandardText())));
						fileWriter.WriteLine("Menu (" + menu.uniqueId + "): " + translations[menu.GetStandardText()]);
						translations.Remove(menu.GetStandardText());
					}
				}
				if (!Application.isPlaying)
					EditorSceneManager.CloseScene(openedScene, true);
				else
					yield return SceneManager.UnloadSceneAsync(scenePath);
			}
			translationModel = "en-" + languages[0].abbreviation;
			currentLanguage = languages[0];
			foreach (StreamWriter _fileWriter in fileWriters)
				fileWriter.Close();
		}
		#endif
		
		[Serializable]
		public class Language
		{
			public string name;
			public string abbreviation;
			public string nativeSpelling;
		}
	}
}