using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.Story
{
	public class ObjectiveGuider : MonoBehaviour
	{
		public static ObjectiveGuider instance;
		public Transform trs;
		public Transform location;
		
		void Start ()
		{
			instance = this;
			gameObject.SetActive(false);
		}
		
		void Update ()
		{
			trs.up = location.position - trs.position;
		}
	}
}