using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance
	{
		get
		{
			if (instance == null)
				instance = FindObjectOfType<T>();
			return instance;
		}
	}
	public static T instance;
	public MultipleInstancesHandlingType handleMultipleInstances;
	public bool persistant;
	
	public virtual void Start ()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
#endif
		if (handleMultipleInstances != MultipleInstancesHandlingType.KeepAll && instance != null && instance != this)
		{
			if (handleMultipleInstances == MultipleInstancesHandlingType.DestroyNew)
			{
				Destroy(gameObject);
				return;
			}
			else
				Destroy(instance.gameObject);
		}
#if UNITY_EDITOR
		}
#endif
		instance = this as T;
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (persistant)
			DontDestroyOnLoad(gameObject);
	}
	
	public static T GetInstance ()
	{
		if (instance == null)
			instance = FindObjectOfType<T>();
		return instance;
	}

	public enum MultipleInstancesHandlingType
	{
		KeepAll,
		DestroyNew,
		DestroyOld
	}
}
