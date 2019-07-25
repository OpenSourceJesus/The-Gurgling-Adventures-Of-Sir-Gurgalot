using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.UI;

namespace TGAOSG
{
	[RequireComponent(typeof(Toggle))]
	public class PlayerPrefsToggle : _Selectable
	{
		public Toggle toggle;
		public string playerPrefsKey;
		public bool appliesToAccount = true;
		
		public virtual void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (appliesToAccount)
				playerPrefsKey += SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + GameManager.accountNumber;
			toggle.isOn = PlayerPrefsExtensions.GetBool(playerPrefsKey, toggle.isOn);
		}

		public virtual void OnValueChanged (bool value)
		{
			PlayerPrefsExtensions.SetBool(playerPrefsKey, value);
		}
	}
}