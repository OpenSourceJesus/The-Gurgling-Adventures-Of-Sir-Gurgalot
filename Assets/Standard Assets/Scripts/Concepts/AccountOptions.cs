using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;
using System.IO;
using System;

namespace TGAOSG
{
	[DisallowMultipleComponent]
    public class AccountOptions : MonoBehaviour
    {
    	public Transform trs;
		public Text selectButtonText;
		public Text playButtonText;
		ushort accountNumber;
		public static AccountOptions currentSelectedAccountOptions;
		public static AccountOptions copyingToAccountOptions;
		public static bool isCopying;
		public GameObject selectButtonObj;
		public GameObject cancelCopyButtonObj;
		public GameObject playButtonObj;
		public GameObject copyButtonObj;
		public GameObject eraseButtonObj;
		public GameObject copyConfirmObj;
		public GameObject eraseConfirmObj;

		public virtual void Start ()
		{
			accountNumber = (ushort) (trs.GetSiblingIndex() + 1);
			selectButtonText.text = "Account " + accountNumber;
		}

		public virtual void Select ()
		{
			if (currentSelectedAccountOptions != null)
			{
				currentSelectedAccountOptions.selectButtonObj.SetActive(true);
				currentSelectedAccountOptions.playButtonObj.SetActive(false);
				currentSelectedAccountOptions.copyButtonObj.SetActive(false);
				currentSelectedAccountOptions.eraseButtonObj.SetActive(false);
				currentSelectedAccountOptions.eraseConfirmObj.SetActive(false);
				currentSelectedAccountOptions.copyConfirmObj.SetActive(false);
				currentSelectedAccountOptions.cancelCopyButtonObj.SetActive(false);
			}
			selectButtonObj.SetActive(false);
			if (isCopying)
			{
				if (copyingToAccountOptions != null)
				{
					copyingToAccountOptions.copyConfirmObj.SetActive(false);
					copyingToAccountOptions.selectButtonObj.SetActive(true);
				}
				currentSelectedAccountOptions.selectButtonObj.SetActive(false);
				currentSelectedAccountOptions.cancelCopyButtonObj.SetActive(true);
				copyConfirmObj.SetActive(true);
				copyingToAccountOptions = this;
				return;
			}
			GameManager.accountNumber = accountNumber;
			bool hasStartedAccount = false;
			string[] registeredKeys = PlayerPrefs.GetString(PlayerPrefsExtensions.REGISTRY_KEY, "").Split(new string[] { PlayerPrefsExtensions.REGISTRY_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
			string registeredKey;
			for (int i = 0; i < registeredKeys.Length; i ++)
			{
				registeredKey = registeredKeys[i] + PlayerPrefsExtensions.REGISTRY_SEPERATOR;
				if (registeredKey.Contains(SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + accountNumber) || registeredKey.Contains(accountNumber + SaveAndLoadManager.SavedObjectEntry.ACCOUNT_AND_ID_SEPERATOR))
				{
					hasStartedAccount = true;
					break;
				}
			}
			if (hasStartedAccount)
				playButtonText.text = "Continue";
			else
				playButtonText.text = "Begin";
			playButtonObj.SetActive(true);
			copyButtonObj.SetActive(true);
			eraseButtonObj.SetActive(true);
			currentSelectedAccountOptions = this;
		}

		public virtual void Play ()
		{
			if (PlayerPrefs.GetInt("Scene" + SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + GameManager.accountNumber, 0) > 0)
				SaveAndLoadManager.instance.Load ();
			else
				GameManager.instance.LoadLevelImmediate ("Jorlinion's House");
		}

		public virtual void Copy ()
		{
			isCopying = true;
			MainMenu.instance.copyingAccountTextObj.SetActive(true);
			cancelCopyButtonObj.SetActive(true);
			playButtonObj.SetActive(false);
			copyButtonObj.SetActive(false);
			eraseButtonObj.SetActive(false);
		}

		public virtual void CopyCancel ()
		{
			isCopying = false;
			MainMenu.instance.copyingAccountTextObj.SetActive(false);
			cancelCopyButtonObj.SetActive(false);
			playButtonObj.SetActive(true);
			copyButtonObj.SetActive(true);
			eraseButtonObj.SetActive(true);
			if (copyingToAccountOptions != null)
			{
				copyingToAccountOptions.copyConfirmObj.SetActive(false);
				copyingToAccountOptions.selectButtonObj.SetActive(true);
				copyingToAccountOptions = null;
			}
		}

		public virtual void CopyConfirmYes ()
		{
			isCopying = false;
			string[] registeredKeys = PlayerPrefs.GetString(PlayerPrefsExtensions.REGISTRY_KEY, "").Split(new string[] { PlayerPrefsExtensions.REGISTRY_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
			string registeredKey;
			for (int i = 0; i < registeredKeys.Length; i ++)
			{
				registeredKey = registeredKeys[i] + PlayerPrefsExtensions.REGISTRY_SEPERATOR;
				if (registeredKey.Contains(SaveAndLoadManager.KEY_NAME_AND_ACCOUNT_SEPARATOR + GameManager.accountNumber) || registeredKey.Contains(GameManager.accountNumber + SaveAndLoadManager.SavedObjectEntry.ACCOUNT_AND_ID_SEPERATOR))
				{
					if (registeredKey.Contains("Int" + PlayerPrefsExtensions.REGISTRY_SEPERATOR))
					{
						registeredKey = registeredKey.Substring(0, registeredKey.IndexOf(PlayerPrefsExtensions.VALUE_AND_TYPE_SEPERATOR));
						PlayerPrefsExtensions.SetInt(registeredKey, PlayerPrefs.GetInt(registeredKey));
					}
					else if (registeredKey.Contains("Float" + PlayerPrefsExtensions.REGISTRY_SEPERATOR))
					{
						registeredKey = registeredKey.Substring(0, registeredKey.IndexOf(PlayerPrefsExtensions.VALUE_AND_TYPE_SEPERATOR));
						PlayerPrefsExtensions.SetFloat(registeredKey, PlayerPrefs.GetFloat(registeredKey));
					}
					else if (registeredKey.Contains("String" + PlayerPrefsExtensions.REGISTRY_SEPERATOR))
					{
						registeredKey = registeredKey.Substring(0, registeredKey.IndexOf(PlayerPrefsExtensions.VALUE_AND_TYPE_SEPERATOR));
						PlayerPrefsExtensions.SetString(registeredKey, PlayerPrefs.GetString(registeredKey));
					}
				}
			}
			StartCoroutine(MainMenu.instance.copiedAccountNotificationObj.DisplayRoutine ());
			MainMenu.instance.copyingAccountTextObj.SetActive(false);
			copyConfirmObj.SetActive(false);
			selectButtonObj.SetActive(true);
			currentSelectedAccountOptions.cancelCopyButtonObj.SetActive(false);
			currentSelectedAccountOptions.selectButtonObj.SetActive(true);
		}

		// public virtual void CopyConfirmNo ()
		// {
		// 	copyConfirmObj.SetActive(false);
		// 	selectButtonObj.SetActive(true);
		// 	copyingToAccountOptions = null;
		// }

		public virtual void Erase ()
		{
			playButtonObj.SetActive(false);
			copyButtonObj.SetActive(false);
			eraseButtonObj.SetActive(false);
			eraseConfirmObj.SetActive(true);
		}

		public virtual void EraseConfirm ()
		{
			GameManager.enabledGosString = "";
			GameManager.disabledGosString = "";
			foreach (string key in SaveAndLoadManager.data.Keys)
			{
				if (key.IndexOf(accountNumber + SaveAndLoadManager.VALUE_SEPARATOR) == 0)
					SaveAndLoadManager.data.Remove(key);
			}
			playButtonObj.SetActive(false);
			copyButtonObj.SetActive(false);
			eraseButtonObj.SetActive(false);
			eraseConfirmObj.SetActive(false);
			selectButtonObj.SetActive(true);
			StartCoroutine(MainMenu.instance.erasedAccountNotificationObj.DisplayRoutine ());
		}
    }
}