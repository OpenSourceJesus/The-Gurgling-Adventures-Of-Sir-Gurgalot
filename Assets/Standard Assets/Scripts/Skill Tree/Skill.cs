using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using TAoKR.Analytics;

namespace TAoKR.SkillTree
{
	[ExecuteAlways]
	public class Skill : SingletonMonoBehaviour<Skill>, ISavableAndLoadable
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
		public ushort cost;
		public Transform trs;
		public string skillName;
		public bool unlocked;
		[SaveAndLoadValue]
		public _bool Unlocked
		{
			get
			{
				return new _bool(unlocked);
			}
			set
			{
				unlocked = value.value;
			}
		}
		public bool learned;
		[SaveAndLoadValue]
		public _bool Learned
		{
			get
			{
				return new _bool(learned);
			}
			set
			{
				learned = value.value;
			}
		}
		
		public virtual bool TryToLearn ()
		{
			bool output = unlocked && !learned && Player.instance.SubtractMoney(cost);
			if (output)
				Learn ();
			return output;
		}
		
		public virtual void Learn ()
		{
			learned = true;
			AnalyticsManager.SpentGoldEvent spentGoldEvent = new AnalyticsManager.SpentGoldEvent();
			spentGoldEvent.eventName.value = "Spent gold";
			spentGoldEvent.amount.value = -cost;
			AnalyticsManager.instance.LogEvent (spentGoldEvent);
			ApplyKnowledgeIfShould ();
		}
		
		public virtual void ApplyKnowledgeIfShould ()
		{
		}
		
		public virtual void Unlock ()
		{
			unlocked = true;
		}
		
#if UNITY_EDITOR
		public virtual void Update ()
		{
			if (Application.isPlaying)
				return;
			skillName = name;
		}
#endif
	}
}