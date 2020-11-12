using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	public class PlayerPrefsSlider : _Slider
	{
		public string PlayerPrefsKey;
		public bool appliesToAccount = true;
		
		public override void Awake ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
			if (appliesToAccount)
				PlayerPrefsKey += SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPEARATOR + GameManager.accountNumber;
			slider.value = PlayerPrefs.GetFloat(PlayerPrefsKey, slider.value);
			base.Awake ();
		}
		
		public override void Update ()
		{
			base.Update ();
			PlayerPrefsExtensions.SetFloat(PlayerPrefsKey, slider.value);
		}
	}
}