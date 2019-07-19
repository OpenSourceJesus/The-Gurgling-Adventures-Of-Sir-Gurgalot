using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LanguageTranslation
{
	public class LanguagesMenu : SingletonMonoBehaviour<LanguagesMenu>
	{
		public Transform trs;
		public LanguageButton languageButtonPrefab;

		public override void Start ()
		{
			base.Start ();
			LanguageButton languageButton;
			foreach (LanguageManager.Language language in LanguageManager.instance.languages)
			{
				languageButton = Instantiate(languageButtonPrefab, trs);
				languageButton.text.text = language.nativeSpelling;
				languageButton.button.onClick.AddListener(delegate { LanguageManager.instance.SetLanguage(language); });
			}
		}
	}
}