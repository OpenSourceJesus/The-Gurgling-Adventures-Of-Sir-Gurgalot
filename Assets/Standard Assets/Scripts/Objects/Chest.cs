using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using TGAOSG.Analytics;

namespace TGAOSG
{
	[ExecuteAlways]
	public class Chest : MonoBehaviour, IConfigurable, IMoneyCarrier
	{
		public Transform trs;
		[MakeConfigurable]
		public float SizeX
		{
			get
			{
				return Mathf.Abs(trs.localScale.x);
			}
			set
			{
				trs.localScale = trs.localScale.SetX(value * Mathf.Sign(trs.localScale.x));
			}
		}
		[MakeConfigurable]
		public float SizeY
		{
			get
			{
				return Mathf.Abs(trs.localScale.y);
			}
			set
			{
				trs.localScale = trs.localScale.SetY(value * Mathf.Sign(trs.localScale.y));
			}
		}
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
		public virtual string Category
		{
			get
			{
				return "Other";
			}
		}
		public ushort goldReward;
		public _ushort Money
		{
			get
			{
				return new _ushort(goldReward);
			}
			set
			{
				goldReward = value.value;
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
				name = "Chest " + value;
			}
		}
		public GameObject fullChestObj;
		
		public virtual void Start ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				return;
			}
			#endif	
		}
		
		public virtual void OnTriggerEnter2D (Collider2D other)
		{
			GameManager.instance.DeactivateGoForever (fullChestObj);
			Player.instance.AddMoney (goldReward);
			AnalyticsManager.GotGoldEvent gotGoldEvent = new AnalyticsManager.GotGoldEvent();
			gotGoldEvent.eventName.value = "Got gold";
			gotGoldEvent.amount.value = goldReward;
			AnalyticsManager.instance.LogEvent (gotGoldEvent);
		}
	}
}