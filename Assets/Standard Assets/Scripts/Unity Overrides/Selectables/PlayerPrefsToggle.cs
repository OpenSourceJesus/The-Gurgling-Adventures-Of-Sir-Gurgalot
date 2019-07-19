using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.UI;

namespace TAoKR
{
	[RequireComponent(typeof(Toggle))]
	public class PlayerPrefsToggle : _Selectable
	{
		public Toggle toggle;
		public string PlayerPrefsKey;
		public bool appliesToAccount = true;
		
		public virtual void Awake ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
			if (appliesToAccount)
				PlayerPrefsKey += SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + GameManager.accountNumber;
			toggle.isOn = PlayerPrefsExtensions.GetBool(PlayerPrefsKey, toggle.isOn);
		}
		
		#if UNITY_EDITOR
		public override void Update ()
		{
			base.Update ();
		#endif
		#if !UNITY_EDITOR
		public virtual void Update ()
		{
		#endif
			PlayerPrefsExtensions.SetBool(PlayerPrefsKey, toggle.isOn);
		}
	}
}