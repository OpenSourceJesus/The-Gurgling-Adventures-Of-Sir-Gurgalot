using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

namespace TAoKR.SkillTree
{
	[ExecuteAlways]
	public class SkillTreeManager : SingletonMonoBehaviour<SkillTreeManager>
	{
		public Transform trs;
		public Skill[] skills;
		
		public override void Start ()
		{
			base.Start ();
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
		}
		
		public virtual void Update ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				skills = GetComponentsInChildren<Skill>();
				return;
			}
			#endif
			if (InputManager.inputter.GetButtonDown("Skill Tree"))
			{
				if (!SceneManager.GetSceneByName("Skill Tree").isLoaded)
				{
					if ((Flowchart.instance == null || !Flowchart.instance.gameObject.activeInHierarchy) && !GameManager.paused && (Obelisk.instance == null || !Obelisk.instance.canvasObj.activeInHierarchy))
					{
						GameManager.instance.Pause ();
						GameManager.instance.LoadLevelAdditive ("Skill Tree");
					}
				}
				else
				{
					GameManager.instance.UnloadLevel("Skill Tree");
					if (!((Flowchart.instance == null || !Flowchart.instance.gameObject.activeInHierarchy) && !GameManager.paused && (Obelisk.instance == null || !Obelisk.instance.canvasObj.activeInHierarchy)))
						GameManager.instance.Unpause ();
				}
			}
		}
	}
}