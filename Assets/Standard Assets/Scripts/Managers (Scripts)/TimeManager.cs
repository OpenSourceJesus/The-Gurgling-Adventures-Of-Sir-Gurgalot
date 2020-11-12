using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Extensions;

namespace TGAOSG
{
	public class TimeManager : SingletonMonoBehaviour<TimeManager>
	{
		[HideInInspector]
		public static float currentTime = 0;
		public static float TotalGameplayDuration
		{
			get
			{
				return PlayerPrefs.GetFloat("Total Gameplay Duration", 0);
			}
			set
			{
				PlayerPrefsExtensions.SetFloat("Total Gameplay Duration", value);
			}
		}
		public float defaultTimeScaleMultiplier;
		public float TimeScaleMultiplier
		{
			get
			{
				return PlayerPrefs.GetFloat("Time Scale", defaultTimeScaleMultiplier);
			}
			set
			{
				PlayerPrefsExtensions.SetFloat("Time Scale", value);
			}
		}
		public static float UnscaledDeltaTime
		{
			get
			{
				if (Time.timeScale > 0)
					return Time.unscaledDeltaTime;
				return 0;
			}
		}
		public Text timerText;
		
		public override void Start ()
		{
			base.Start ();
			SetTimeScale (1);
		}
		
		void Update ()
		{
			currentTime += Time.deltaTime;
		}
		
		public void SetTimeScale (float timeScale)
		{
			Time.timeScale = timeScale * TimeScaleMultiplier;
			foreach (Rigidbody2D rigid in _Rigidbody2D.allInstances)
				rigid.simulated = Time.timeScale > 0;
		}
		
		void OnApplicationQuit ()
		{
			TotalGameplayDuration += Time.realtimeSinceStartup;
		}
		
		public void SetTimerTextActive (bool active)
		{
			instance.timerText.gameObject.SetActive(active);
		}
	}
}