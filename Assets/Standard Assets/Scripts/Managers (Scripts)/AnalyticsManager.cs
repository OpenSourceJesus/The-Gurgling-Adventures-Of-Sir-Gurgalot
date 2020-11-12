using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Reflection;
using UnityEngine.SceneManagement;
using ClassExtensions;
using System.IO;

namespace TAoKR.Analytics
{
	public class AnalyticsManager : SingletonMonoBehaviour<AnalyticsManager>
	{
		public Transform trs;
		public string formUrl;
		UnityWebRequest webRequest;
		WWWForm form;
		public AnalyticsEvent _AnalyticsEvent;
		public PlayerHurtEvent _PlayerHurtEvent;
		public PlayerDiedEvent _PlayerDiedEvent;
		public EnemyDiedEvent _EnemyDiedEvent;
		public ArenaWaveClearedEvent _ArenaWaveClearedEvent;
		public SpentGoldEvent _SpentGoldEvent;
		public GotGoldEvent _GotGoldEvent;
		public SavedEvent _SavedEvent;
		public Queue<AnalyticsEvent> eventQueue = new Queue<AnalyticsEvent>();
		public bool collectAnalyticsDefault;
		public bool logAnalyticsLocallyDefault;
		public bool CollectAnalytics
		{
			get
			{
				return PlayerPrefsExtensions.GetBool("Collect Analytics", collectAnalyticsDefault);
			}
			set
			{
				PlayerPrefsExtensions.SetBool("Collect Analytics", value);
			}
		}
		public bool LogAnalyticsLocally
		{
			get
			{
				return PlayerPrefsExtensions.GetBool("Log Analytics Locally", logAnalyticsLocallyDefault);
			}
			set
			{
				PlayerPrefsExtensions.SetBool("Log Analytics Locally", value);
			}
		}
		public int SessionNumber
		{
			get
			{
				return PlayerPrefs.GetInt("Session Number", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Session Number", value);
			}
		}
		public int PreviousTotalGameplayDuration
		{
			get
			{
				return PlayerPrefs.GetInt("Total Gameplay Duration", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Total Gameplay Duration", value);
			}
		}
		public static Dictionary<string, AnalyticsManager> analyticsManagers = new Dictionary<string, AnalyticsManager>();
		public Timer sessionTimer;
		public Timer timerSinceLastLog;
		public string localLogFolderPath;
		string currentLogFilePath;
		public const int MAX_STRING_LENGTH = 30;
		public const char FILLER_CHARACTER = ' ';
		public const string VALUE_SEPERATOR = "-----";
		public DataColumn[] dataColumns;
		public Dictionary<string, DataColumn> dataColumnDict = new Dictionary<string, DataColumn>();
		public Dictionary<string, string> columnData = new Dictionary<string, string>();
		
		public override void Start ()
		{
			if (analyticsManagers.ContainsKey(name))
			{
				Destroy(gameObject);
				return;
			}
			foreach (DataColumn dataColumn in dataColumns)
				dataColumnDict.Add(dataColumn.name, dataColumn);
			localLogFolderPath = Application.persistentDataPath + Path.DirectorySeparatorChar + localLogFolderPath;
			analyticsManagers.Add(name, this);
			trs.SetParent(null);
			base.Start ();
		}

		public virtual void OnApplicationQuit ()
		{
			PreviousTotalGameplayDuration += Mathf.RoundToInt(sessionTimer.TimeElapsed);
			SessionNumber ++;
		}
		
		public virtual void LogEvent (AnalyticsEvent _event)
		{
			if (CollectAnalytics)
			{
				eventQueue.Enqueue(_event);
				StopAllCoroutines();
				StartCoroutine(LogEventRoutine (_event));
			}
		}
		
		public virtual IEnumerator LogAllEventsRoutine ()
		{
			if (!CollectAnalytics)
				yield break;
			while (eventQueue.Count > 0)
				yield return StartCoroutine(LogEventRoutine (eventQueue.Dequeue()));
			yield break;
		}
		
		public virtual IEnumerator LogEventRoutine (AnalyticsEvent _event)
		{
			if (!CollectAnalytics)
				yield break;
			_event.LogData (this);
			if (LogAnalyticsLocally)
				LogEventLocally (_event);
			yield return StartCoroutine(LogEventOnline(_event));
			yield break;
		}

		public virtual IEnumerator LogEventOnline (AnalyticsEvent _event)
		{
			if (!CollectAnalytics)
				yield break;
			using (webRequest = UnityWebRequest.Post(formUrl, form))
			{
				yield return webRequest.SendWebRequest();
				if (webRequest.isNetworkError || webRequest.isHttpError)
					Debug.Log(webRequest.error);
				else
					Debug.Log("Form upload complete!");
			}
			webRequest.Dispose();
			yield break;
		}

		public virtual void LogEventLocally (AnalyticsEvent _event)
		{
			if (!CollectAnalytics || !LogAnalyticsLocally)
				return;
			if (!Directory.Exists(localLogFolderPath))
				Directory.CreateDirectory(localLogFolderPath);
			currentLogFilePath = localLogFolderPath + Path.DirectorySeparatorChar + "Analytics " + SessionNumber + ".txt";
			string[] currentLogFileLines = new string[1];
			string dataColumn;
			StreamWriter writer = null;
			if (File.Exists(currentLogFilePath))
			{
				currentLogFileLines = File.ReadAllLines(currentLogFilePath);
				File.Delete(currentLogFilePath);
				writer = File.CreateText(currentLogFilePath);
				string currentLogFileLine = "";
				string dataValue;
				for (int i = 0; i < dataColumns.Length; i ++)
				{
					dataColumn = dataColumns[i].name;
					columnData.TryGetValue(dataColumn, out dataValue);
					if (dataValue == null)
						dataValue = "";
					else
						currentLogFileLine += dataValue;
					for (int i2 = dataValue.Length; i2 < MAX_STRING_LENGTH; i2 ++)
						currentLogFileLine += FILLER_CHARACTER;
					currentLogFileLine += VALUE_SEPERATOR;
				}
				currentLogFileLines = currentLogFileLines.Add_class(currentLogFileLine);
				foreach (string line in currentLogFileLines)
					writer.Write("\n" + line);
			}
			else
			{
				writer = File.CreateText(currentLogFilePath);
				for (int i = 0; i < dataColumns.Length; i ++)
				{
					dataColumn = dataColumns[i].name;
					currentLogFileLines[0] += dataColumn;
					for (int i2 = dataColumn.Length; i2 < MAX_STRING_LENGTH; i2 ++)
						currentLogFileLines[0] += FILLER_CHARACTER;
					currentLogFileLines[0] += VALUE_SEPERATOR;
				}
				writer.Write(currentLogFileLines[0]);
			}
			writer.Close();
			writer.Dispose();
		}
		
		#region Analytics Events
		[Serializable]
		public class AnalyticsEvent
		{
			public GameVersionDataEntry gameVersion = new GameVersionDataEntry();
			public PlayerDataEntry player = new PlayerDataEntry();
			public SceneDataEntry scene = new SceneDataEntry();
			public TimeSinceLastLogDataEntry timeSinceLastLog = new TimeSinceLastLogDataEntry();
			public AnalyticsDataEntry_string eventName = new AnalyticsDataEntry_string();
			public SessionDurationDataEntry sessionDuration = new SessionDurationDataEntry();
			public TotalGameplayDurationDataEntry totalGameplayDuration = new TotalGameplayDurationDataEntry();
			public SessionNumberDataEntry sessionNumber = new SessionNumberDataEntry();
			public TotalGoldDataEntry totalGold = new TotalGoldDataEntry();
			
			public AnalyticsEvent ()
			{
				Init ();
			}
			
			public virtual void Init ()
			{
				if (instance == null)
					return;
				AnalyticsEvent _event = (AnalyticsEvent) typeof(AnalyticsManager).GetField("_" + GetName()).GetValue(instance);
				gameVersion.dataColumnName = _event.gameVersion.dataColumnName;
				player.dataColumnName = _event.player.dataColumnName;
				scene.dataColumnName = _event.scene.dataColumnName;
				timeSinceLastLog.dataColumnName = _event.timeSinceLastLog.dataColumnName;
				eventName.dataColumnName = _event.eventName.dataColumnName;
				sessionDuration.dataColumnName = _event.sessionDuration.dataColumnName;
				totalGameplayDuration.dataColumnName = _event.totalGameplayDuration.dataColumnName;
				sessionNumber.dataColumnName = _event.sessionNumber.dataColumnName;
				totalGold.dataColumnName = _event.totalGold.dataColumnName;
			}
			
			public virtual string GetName ()
			{
				string output = GetType().ToString();
				output = output.Substring(output.LastIndexOf("+") + 1);
				return output;
			}
			
			public virtual void LogData (AnalyticsManager analyticsManager)
			{
				analyticsManager.form = new WWWForm();
				analyticsManager.columnData.Clear();
				gameVersion.LogData (analyticsManager);
				player.LogData (analyticsManager);
				scene.LogData (analyticsManager);
				timeSinceLastLog.LogData (analyticsManager);
				eventName.LogData (analyticsManager);
				sessionDuration.LogData (analyticsManager);
				totalGameplayDuration.LogData (analyticsManager);
				sessionNumber.LogData (analyticsManager);
				totalGold.LogData (analyticsManager);
			}
		}

		[Serializable]
		public class PlayerHurtEvent : AnalyticsEvent
		{
			public AnalyticsDataEntry_string hurtBy = new AnalyticsDataEntry_string();

			public PlayerHurtEvent ()
			{
				Init ();
			}

			public override void Init ()
			{
				if (instance == null)
					return;
				base.Init ();
				PlayerHurtEvent _event = (PlayerHurtEvent) typeof(AnalyticsManager).GetField("_" + GetName()).GetValue(instance);
				hurtBy.dataColumnName = _event.hurtBy.dataColumnName;
			}

			public override void LogData (AnalyticsManager analyticsManager)
			{
				base.LogData (analyticsManager);
				hurtBy.LogData (analyticsManager);
			}
		}

		[Serializable]
		public class PlayerDiedEvent : AnalyticsEvent
		{
			public AnalyticsDataEntry_string killedBy = new AnalyticsDataEntry_string();
			
			public PlayerDiedEvent ()
			{
				Init ();
			}
			
			public override void Init ()
			{
				if (instance == null)
					return;
				base.Init ();
				PlayerDiedEvent _event = (PlayerDiedEvent) typeof(AnalyticsManager).GetField("_" + GetName()).GetValue(instance);
				killedBy.dataColumnName = _event.killedBy.dataColumnName;
			}
			
			public override void LogData (AnalyticsManager analyticsManager)
			{
				base.LogData (analyticsManager);
				killedBy.LogData (analyticsManager);
			}
		}

		[Serializable]
		public class EnemyDiedEvent : AnalyticsEvent
		{
			public AnalyticsDataEntry_string killedEnemyName = new AnalyticsDataEntry_string();
			
			public EnemyDiedEvent ()
			{
				Init ();
			}
			
			public override void Init ()
			{
				if (instance == null)
					return;
				base.Init ();
				EnemyDiedEvent _event = (EnemyDiedEvent) typeof(AnalyticsManager).GetField("_" + GetName()).GetValue(instance);
				killedEnemyName.dataColumnName = _event.killedEnemyName.dataColumnName;
			}
			
			public override void LogData (AnalyticsManager analyticsManager)
			{
				base.LogData (analyticsManager);
				killedEnemyName.LogData (analyticsManager);
			}
		}

		[Serializable]
		public class ArenaWaveClearedEvent : AnalyticsEvent
		{
			public AnalyticsDataEntry_int scoreAfterCleared = new AnalyticsDataEntry_int();
			
			public ArenaWaveClearedEvent ()
			{
				Init ();
			}
			
			public override void Init ()
			{
				if (instance == null)
					return;
				base.Init ();
				ArenaWaveClearedEvent _event = (ArenaWaveClearedEvent) typeof(AnalyticsManager).GetField("_" + GetName()).GetValue(instance);
				scoreAfterCleared.dataColumnName = _event.scoreAfterCleared.dataColumnName;
			}
			
			public override void LogData (AnalyticsManager analyticsManager)
			{
				base.LogData (analyticsManager);
				scoreAfterCleared.LogData (analyticsManager);
			}
		}

		[Serializable]
		public class SpentGoldEvent : AnalyticsEvent
		{
			public AnalyticsDataEntry_int amount = new AnalyticsDataEntry_int();
			public AnalyticsDataEntry_string upgradeName = new AnalyticsDataEntry_string();
			
			public SpentGoldEvent ()
			{
				Init ();
			}
			
			public override void Init ()
			{
				if (instance == null)
					return;
				base.Init ();
				SpentGoldEvent _event = (SpentGoldEvent) typeof(AnalyticsManager).GetField("_" + GetName()).GetValue(instance);
				amount.dataColumnName = _event.amount.dataColumnName;
				upgradeName.dataColumnName = _event.upgradeName.dataColumnName;
			}
			
			public override void LogData (AnalyticsManager analyticsManager)
			{
				base.LogData (analyticsManager);
				amount.LogData (analyticsManager);
				upgradeName.LogData (analyticsManager);
			}
		}

		[Serializable]
		public class GotGoldEvent : AnalyticsEvent
		{
			public AnalyticsDataEntry_int amount = new AnalyticsDataEntry_int();
			public AnalyticsDataEntry_string chestName = new AnalyticsDataEntry_string();
			
			public GotGoldEvent ()
			{
				Init ();
			}
			
			public override void Init ()
			{
				if (instance == null)
					return;
				base.Init ();
				GotGoldEvent _event = (GotGoldEvent) typeof(AnalyticsManager).GetField("_" + GetName()).GetValue(instance);
				amount.dataColumnName = _event.amount.dataColumnName;
				chestName.dataColumnName = _event.chestName.dataColumnName;
			}
			
			public override void LogData (AnalyticsManager analyticsManager)
			{
				base.LogData (analyticsManager);
				amount.LogData (analyticsManager);
				chestName.LogData (analyticsManager);
			}
		}

		[Serializable]
		public class SavedEvent : AnalyticsEvent
		{
			public AnalyticsDataEntry_string saveSource = new AnalyticsDataEntry_string();

			public SavedEvent ()
			{
				Init ();
			}
			
			public override void Init ()
			{
				if (instance == null)
					return;
				base.Init ();
				SavedEvent _event = (SavedEvent) typeof(AnalyticsManager).GetField("_" + GetName()).GetValue(instance);
				saveSource.dataColumnName = _event.saveSource.dataColumnName;
			}
			
			public override void LogData (AnalyticsManager analyticsManager)
			{
				base.LogData (analyticsManager);
				saveSource.LogData (analyticsManager);
			}
		}
		#endregion
		
		#region Analytics Data Entries
		[Serializable]
		public class AnalyticsDataEntry
		{
			public string dataColumnName;

			public virtual string GetFieldNameInForm (AnalyticsManager analyticsManager)
			{
				return analyticsManager.dataColumnDict[dataColumnName].fieldNameInForm;
			}

			public virtual string GetValue (AnalyticsManager analyticsManager)
			{
				return "";
			}

			public virtual void LogData (AnalyticsManager analyticsManager)
			{
				analyticsManager.form.AddField(GetFieldNameInForm(analyticsManager), GetValue(analyticsManager));
				analyticsManager.columnData.Add(dataColumnName, GetValue(analyticsManager));
			}
		}

		[Serializable]
		public class AnalyticsDataEntry<T> : AnalyticsDataEntry
		{
			public T value;

			public override string GetValue (AnalyticsManager analyticsManager)
			{
				string output = "";
				if (value != null)
					output = value.ToString();
				return output;
			}
		}

		[Serializable]
		public class AnalyticsDataEntry_string : AnalyticsDataEntry<string>
		{
		}

		[Serializable]
		public class AnalyticsDataEntry_float : AnalyticsDataEntry<float>
		{
		}

		[Serializable]
		public class AnalyticsDataEntry_int : AnalyticsDataEntry<int>
		{
		}

		[Serializable]
		public class AnalyticsDataEntry_Vector2 : AnalyticsDataEntry<Vector2>
		{
		}
		#endregion
		
		[Serializable]
		public class GameVersionDataEntry : AnalyticsDataEntry
		{
			public override string GetValue (AnalyticsManager analyticsManager)
			{
				return "" + BuildManager.instance.versionIndex;
			}
		}

		[Serializable]
		public class PlayerDataEntry : AnalyticsDataEntry
		{
			public override string GetValue (AnalyticsManager analyticsManager)
			{
				return "";
			}
		}

		[Serializable]
		public class SceneDataEntry : AnalyticsDataEntry
		{
			public override string GetValue (AnalyticsManager analyticsManager)
			{
				return SceneManager.GetActiveScene().name;
			}
		}

		[Serializable]
		public class TotalGameplayDurationDataEntry : AnalyticsDataEntry
		{
			public override string GetValue (AnalyticsManager analyticsManager)
			{
				return "" + (analyticsManager.PreviousTotalGameplayDuration + analyticsManager.sessionTimer.TimeElapsed);
			}
		}

		[Serializable]
		public class TimeSinceLastLogDataEntry : AnalyticsDataEntry
		{
			public override string GetValue (AnalyticsManager analyticsManager)
			{
				float timeSinceLastLogged = analyticsManager.timerSinceLastLog.TimeElapsed;
				analyticsManager.timerSinceLastLog.Reset ();
				analyticsManager.timerSinceLastLog.Start ();
				return "" + timeSinceLastLogged;
			}
		}

		[Serializable]
		public class SessionNumberDataEntry : AnalyticsDataEntry
		{
			public override string GetValue (AnalyticsManager analyticsManager)
			{
				return "" + analyticsManager.SessionNumber;
			}
		}

		[Serializable]
		public class SessionDurationDataEntry : AnalyticsDataEntry
		{
			public override string GetValue (AnalyticsManager analyticsManager)
			{
				return "" + analyticsManager.sessionTimer.TimeElapsed;
			}
		}

		[Serializable]
		public class TotalGoldDataEntry : AnalyticsDataEntry
		{
			public override string GetValue (AnalyticsManager analyticsManager)
			{
				return "" + Player.instance.Money.value;
			}
		}

		[Serializable]
		public class DataColumn
		{
			public string name;
			public string fieldNameInForm;
		}
	}
}