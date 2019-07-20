using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	public class PlayerPrefsSlider : _Slider
	{
		public string playerPrefsKey;
		public bool appliesToAccount = true;
		
		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			base.Awake ();
			if (appliesToAccount)
				playerPrefsKey += SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + GameManager.accountNumber;
			slider.value = PlayerPrefs.GetFloat(playerPrefsKey, slider.value);
		}

		public override void OnValueChanged (float value)
		{
			base.OnValueChanged (value);
			PlayerPrefsExtensions.SetFloat(playerPrefsKey, value);
		}
	}
}