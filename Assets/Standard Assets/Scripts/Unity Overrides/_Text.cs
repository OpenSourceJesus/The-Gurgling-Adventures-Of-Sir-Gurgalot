using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LanguageTranslation;

namespace TGAOSG
{
	[ExecuteAlways]
	[RequireComponent(typeof(Text))]
	[DisallowMultipleComponent]
	public class _Text : MonoBehaviour, IIdentifiable
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
		string keyboardText;
		public bool useSeperateTextForGamepad;
		[Multiline]
		public string gamepadText;
		public Text text;
		bool usingGamepad;
		bool previousUsingGamepad;
		Coroutine translateRoutine;
		public static List<_Text> instances = new List<_Text>();

		public virtual IEnumerator Start ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				text = GetComponent<Text>();
				do
				{
					uniqueId = Random.Range(int.MinValue, int.MaxValue);
				}
				while (GameManager.identifiableIds.Contains(uniqueId));
				yield break;
			}
			#endif
			keyboardText = text.text;
			Translate ();
			yield break;
		}

		public virtual void OnEnable ()
		{
			instances.Add(this);
		}

		public virtual void OnDisable ()
		{
			instances.Remove(this);
		}
		
		public virtual void Update ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
			if (useSeperateTextForGamepad)
			{
				usingGamepad = InputManager.inputter.controllers.joystickCount > 0;
				if (usingGamepad != previousUsingGamepad)
					Translate ();
				previousUsingGamepad = usingGamepad;
			}
		}

		public virtual void Translate ()
		{
			if (useSeperateTextForGamepad)
			{
				if (usingGamepad)
					text.text = gamepadText;
				else
					text.text = keyboardText;
			}
			else
			{
				text.text = keyboardText;
			}
			if (LanguageManager.currentLanguage == null || LanguageManager.currentLanguage.name == "English")
				return;
			// if (translateRoutine != null)
			// 	StopCoroutine(translateRoutine);
			// translateRoutine = StartCoroutine(TranslateRoutine ());
			text.text = LanguageManager.instance.GetSerializedTranslation(uniqueId);
		}

		public virtual IEnumerator TranslateRoutine ()
		{
			LanguageManager.instance.Translate (text.text);
			yield return new WaitUntil(() => (LanguageManager.instance.translations.ContainsKey(text.text)));
			text.text = LanguageManager.instance.translations[text.text];
			LanguageManager.instance.translations.Remove(text.text);
		}
	}
}