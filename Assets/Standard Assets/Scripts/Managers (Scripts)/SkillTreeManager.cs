using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;
using Extensions;

namespace TGAOSG.SkillTree
{
	[ExecuteAlways]
	public class SkillTreeManager : SingletonMonoBehaviour<SkillTreeManager>, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Transform trs;
		public Skill[] skills;
		bool skillTreeInput;
		bool previousSkillTreeInput;
		
		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public virtual void DoUpdate ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				skills = GetComponentsInChildren<Skill>();
				return;
			}
#endif
			skillTreeInput = InputManager.Instance.SkillTreeInput;
			if (skillTreeInput && !previousSkillTreeInput)
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
					GameManager.instance.UnloadLevel ("Skill Tree");
					if (!((Flowchart.instance == null || !Flowchart.instance.gameObject.activeInHierarchy) && !GameManager.paused && (Obelisk.instance == null || !Obelisk.instance.canvasObj.activeInHierarchy)))
						GameManager.instance.Unpause ();
				}
			}
			previousSkillTreeInput = skillTreeInput;
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}