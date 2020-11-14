using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;
using System;
using UnityEngine.UI;
using TGAOSG.SkillTree;
using System.IO;
using Extensions;
using Random = UnityEngine.Random;

namespace TGAOSG
{
    [ExecuteAlways]
	public class GameManager : SingletonMonoBehaviour<GameManager>, ISavableAndLoadable
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
		public GameObject moneyPanel;
		public Text moneyText;
		public Animator screenEffectAnimator;
		public List<GameObject> registeredGos = new List<GameObject>();
		static _string enabledGosString;
		[SaveAndLoadValue]
		public static _string EnabledGosString
		{
			get
			{
				if (enabledGosString == null)
					enabledGosString = new _string();
				return enabledGosString;
			}
			set
			{
				enabledGosString = value;
			}
		}
		static _string disabledGosString;
		[SaveAndLoadValue]
		public static _string DisabledGosString
		{
			get
			{
				if (disabledGosString == null)
					disabledGosString = new _string();
				return disabledGosString;
			}
			set
			{
				disabledGosString = value;
			}
		}
		public static bool paused;
		public const string STRING_SEPERATOR = "|";
		public MenuDialog menuDialogPrefab;
		public GameObject bossInfoGo;
		public Text bossText;
		[SerializeField]
		public static List<int> identifiableIds = new List<int>();
		public static ushort accountNumber;
		public static IUpdatable[] updatables = new IUpdatable[0];
		public static IUpdatable[] pausedUpdatables = new IUpdatable[0];
		public static bool isFocused = true;

		public override void Start ()
		{
			base.Start ();
#if UNITY_EDITOR
			// PlayerPrefs.DeleteAll();
			if (!Application.isPlaying)
			{
				MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
				foreach (MonoBehaviour monoBehaviour in monoBehaviours)
				{
					IIdentifiable identifiable = monoBehaviour as IIdentifiable;
					if (identifiable != null)
						SetUniqueId (identifiable);
				}
				return;
			}
#endif
			Screen.fullScreen = true;
			DestroyAllSpawnedObjects ();
			screenEffectAnimator.SetFloat("speed", 1);
			screenEffectAnimator.Play("Fade In");
		}
		
		public virtual void Pause ()
		{
			if (!paused)
			{
				paused = true;
				TimeManager.instance.SetTimeScale (0);
			}
		}
		
		public virtual void Unpause ()
		{
			if (paused)
			{
				paused = false;
				TimeManager.instance.SetTimeScale (1);
			}
		}
		
		public virtual void Update ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				EnabledGosString.value = "";
				DisabledGosString.value = "";
				return;
			}
#endif
			try
			{
				foreach (IUpdatable updatable in updatables)
					updatable.DoUpdate ();
				Physics2D.Simulate(Time.deltaTime);
				ObjectPool.instance.DoUpdate ();
				GameplayCamera.instance.DoUpdate ();
				HandlePausing ();
			}
			catch (Exception e)
			{
				print(e.Message + "\n" + e.StackTrace);
			}
		}

		void HandlePausing ()
		{
			if (InputManager.inputter.GetButtonDown("Menu") && (Flowchart.instance == null || !Flowchart.instance.gameObject.activeInHierarchy) && (Obelisk.instance == null || !Obelisk.instance.canvasObj.activeInHierarchy) && !SceneManager.GetSceneByName("Skill Tree").isLoaded && !SceneManager.GetSceneByName("Main Menu").isLoaded)
			{
				if (!paused)
				{
					Pause ();
					LoadLevelAdditive ("Pause Menu");
				}
				else
				{
					Unpause ();
					UnloadLevel ("Pause Menu");
				}
			}
		}

		public virtual void OnApplicationFocus (bool isFocused)
		{
			GameManager.isFocused = isFocused;
			if (isFocused)
			{
				foreach (IUpdatable pausedUpdatable in pausedUpdatables)
					updatables = updatables.Add(pausedUpdatable);
				pausedUpdatables = new IUpdatable[0];
				// foreach (Timer runningTimer in Timer.runningInstances)
				// 	runningTimer.pauseIfCan = false;
				// foreach (TemporaryActiveGameObject tempActiveGo in TemporaryActiveGameObject.activeInstances)
				// 	tempActiveGo.Do ();
			}
			else
			{
				IUpdatable updatable;
				for (int i = 0; i < updatables.Length; i ++)
				{
					updatable = updatables[i];
					if (updatable.PauseWhileUnfocused)
					{
						pausedUpdatables = pausedUpdatables.Add(updatable);
						updatables = updatables.RemoveAt(i);
						i --;
					}
				}
				// foreach (Timer runningTimer in Timer.runningInstances)
				// 	runningTimer.pauseIfCan = true;
				// foreach (TemporaryActiveGameObject tempActiveGo in TemporaryActiveGameObject.activeInstances)
				// 	tempActiveGo.Do ();
			}
		}
		
		public virtual void GameOver ()
		{
			Player.instance.Disable ();
			SaveAndLoadManager.instance.Load ();
		}
		
		public virtual void DestroyAllSpawnedObjects ()
		{
			foreach (ObjectPool.SpawnedEntry spawnedEntry in ObjectPool.GetInstance().spawnedEntries)
				Destroy(spawnedEntry.go);
			ObjectPool.instance.spawnedEntries.Clear();
		}
		
		public virtual void SetGosActive ()
		{
			if (instance != this)
			{
				instance.SetGosActive ();
				return;
			}
			string[] stringSeperators = { STRING_SEPERATOR };
			string[] enabledGos = EnabledGosString.value.Split(stringSeperators, StringSplitOptions.None);
			foreach (string goName in enabledGos)
			{
				for (int i = 0; i < registeredGos.Count; i ++)
				{
					if (goName == registeredGos[i].name)
					{
						registeredGos[i].SetActive(true);
						break;
					}
				}
			}
			string[] disabledGos = DisabledGosString.value.Split(stringSeperators, StringSplitOptions.None);
			foreach (string goName in disabledGos)
			{
				GameObject go = GameObject.Find(goName);
				if (go != null)
					go.SetActive(false);
			}
		}
		
		public virtual void ActivateGoForever (GameObject go)
		{
			go.SetActive(true);
			ActivateGoForever (go.name);
		}
		
		public virtual void DeactivateGoForever (GameObject go)
		{
			go.SetActive(false);
			DeactivateGoForever (go.name);
		}
		
		public virtual void ActivateGoForever (string goName)
		{
			DisabledGosString.value = DisabledGosString.value.Replace(STRING_SEPERATOR + goName, "");
			if (!EnabledGosString.value.Contains(goName))
				EnabledGosString.value += STRING_SEPERATOR + goName;
		}
		
		public virtual void DeactivateGoForever (string goName)
		{
			EnabledGosString.value = EnabledGosString.value.Replace(STRING_SEPERATOR + goName, "");
			if (!DisabledGosString.value.Contains(goName))
				DisabledGosString.value += STRING_SEPERATOR + goName;
		}
		
		public virtual void LoadLevel (string levelName)
		{
			if (instance != this)
			{
				instance.LoadLevel(levelName);
				return;
			}
			Player.instance.enabled = false;
			Player.instance.rigid.simulated = false;
			Player.instance.anim.enabled = false;
			StartCoroutine(LevelTransition (levelName));
		}
		
		public virtual void LoadLevelImmediate (int levelId)
		{
			SceneManager.LoadScene(levelId);
		}
		
		public virtual void LoadLevelImmediate (string levelName)
		{
			SceneManager.LoadScene(levelName);
		}
		
		public virtual IEnumerator LevelTransition (string levelName)
		{
			screenEffectAnimator.SetFloat("speed", -1);
			screenEffectAnimator.Play("Fade In", 0, 1);
			yield return new WaitForSeconds(screenEffectAnimator.GetCurrentAnimatorStateInfo(0).length);
			LoadLevelImmediate (levelName);
		}
		
		public virtual void DestroyObject (GameObject go)
		{
			Destroy(go);
		}

		public virtual void DestroyObject (string goName)
		{
			Destroy(GameObject.Find(goName));
		}
		
		public virtual void Quit ()
		{
			Application.Quit ();
		}
		
		public virtual void UnloadLevel (string levelName)
		{
			SceneManager.UnloadSceneAsync(levelName);
		}
		
		public virtual void LoadLevelAdditive (string levelName)
		{
			SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
		}
		
		public virtual void SetFastTravelMenuActive (bool active)
		{
			FastTravelMenu.instance.gameObject.SetActive(active);
		}

		public virtual void TemporarilyDisableObelisk ()
		{
			if (Obelisk.instance != null)
				instance.StartCoroutine(TemporarilyDisableObeliskRoutine ());
		}

		public virtual IEnumerator TemporarilyDisableObeliskRoutine ()
		{
			Obelisk.instance.interactable.enabled = false;
			yield return new WaitForEndOfFrame();
			Obelisk.instance.interactable.enabled = true;
		}

		public virtual void SetUniqueId (IIdentifiable identifiable)
		{
			if (identifiable.UniqueId == 0)
			{
				do
				{
					identifiable.UniqueId = Random.Range(int.MinValue, int.MaxValue);
				}
				while (identifiableIds.Contains(identifiable.UniqueId));
				identifiableIds.Add(identifiable.UniqueId);
			}
			else if (!identifiableIds.Contains(identifiable.UniqueId))
				identifiableIds.Add(identifiable.UniqueId);
		}

		public virtual void _Debug (string str)
		{
			Debug.LogError(str);
		}
	}
}