using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassExtensions
{
	public static class PlayerPrefsExtensions
	{
		public const bool USE_REGISTRY = true;
		public const string REGISTRY_KEY = "All Keys And Values";
		public const string REGISTRY_SEPERATOR = "↕";
		public const string VALUE_AND_TYPE_SEPERATOR = "↔";

		public static void SetInt (string key, int value, bool registerKey = USE_REGISTRY)
		{
			PlayerPrefs.SetInt(key, value);
			if (registerKey)
				RegisterKey (key, PlayerPrefsValueType.Int);
		}

		public static void SetFloat (string key, float value, bool registerKey = USE_REGISTRY)
		{
			PlayerPrefs.SetFloat(key, value);
			if (registerKey)
				RegisterKey (key, PlayerPrefsValueType.Float);
		}

		public static void SetString (string key, string value, bool registerKey = USE_REGISTRY)
		{
			PlayerPrefs.SetString(key, value);
			if (registerKey)
				RegisterKey (key, PlayerPrefsValueType.String);
		}

		public static bool GetBool (string key, bool defaultValue = false)
		{
			int _defaultValue = 0;
			if (defaultValue)
				_defaultValue = 1;
			return PlayerPrefs.GetInt(key, _defaultValue) == 1;
		}
		
		public static void SetBool (string key, bool value, bool registerKey = USE_REGISTRY)
		{
			SetInt (key, value.GetHashCode(), registerKey);
		}
		
		public static Color GetColor (string key)
		{
			return GetColor(key, Color.black.SetAlpha(0));
		}
		
		public static Color GetColor (string key, Color defaultValue)
		{
			return new Color(PlayerPrefs.GetFloat(key + ".r", defaultValue.r), PlayerPrefs.GetFloat(key + ".g", defaultValue.g), PlayerPrefs.GetFloat(key + ".b", defaultValue.b), PlayerPrefs.GetFloat(key + ".a", defaultValue.a));
		}
		
		public static void SetColor (string key, Color value, bool registerKey = USE_REGISTRY)
		{
			SetFloat (key + ".r", value.r, registerKey);
			SetFloat (key + ".g", value.g, registerKey);
			SetFloat (key + ".b", value.b, registerKey);
			SetFloat (key + ".a", value.a, registerKey);
		}
		
		public static Vector2 GetVector2 (string key, Vector2 defaultValue = new Vector2())
		{
			return new Vector2(PlayerPrefs.GetFloat(key + ".x", defaultValue.x), PlayerPrefs.GetFloat(key + ".y", defaultValue.y));
		}
		
		public static void SetVector2 (string key, Vector2 value, bool registerKey = USE_REGISTRY)
		{
			SetFloat (key + ".x", value.x, registerKey);
			SetFloat (key + ".y", value.y, registerKey);
		}

		public static void RegisterKey (string key, PlayerPrefsValueType valueType)
		{
			PlayerPrefs.SetString(REGISTRY_KEY, PlayerPrefs.GetString(REGISTRY_KEY, "") + key + VALUE_AND_TYPE_SEPERATOR + valueType.ToString() + REGISTRY_SEPERATOR);
		}

		public static void DeregisterKey (string key)
		{
			string registry = PlayerPrefs.GetString(REGISTRY_KEY, "");
			registry = registry.RemoveBetween(key, REGISTRY_SEPERATOR, true);
			PlayerPrefs.SetString(REGISTRY_KEY, registry);
		}

		public static void DeleteKey (string key, bool deregisterKey = USE_REGISTRY)
		{
			PlayerPrefs.DeleteKey(key);
			if (deregisterKey)
				DeregisterKey (key);
		}

		public enum PlayerPrefsValueType
		{
			Int,
			Float,
			String
		}
	}
}