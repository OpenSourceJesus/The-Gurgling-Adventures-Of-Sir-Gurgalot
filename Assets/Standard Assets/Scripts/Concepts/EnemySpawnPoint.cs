using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	[ExecuteInEditMode]
	public class EnemySpawnPoint : MonoBehaviour
	{
		public Enemy[] enemyTypesThatUseMe;
		public Vector2 anchorPosition;
		public Transform trs;
		
		#if UNITY_EDITOR
		void Start ()
		{
			if (Application.isPlaying)
				return;
			if (trs == null)
				trs = GetComponent<Transform>();
		}
		#endif
	}
}