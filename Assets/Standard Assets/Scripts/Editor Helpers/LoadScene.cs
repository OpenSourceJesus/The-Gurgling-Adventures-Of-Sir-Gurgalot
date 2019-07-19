using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TAoKR
{
	[ExecuteInEditMode]
	public class LoadScene : MonoBehaviour
	{
		public string sceneName;
		public bool update;

		void Update ()
		{
			if (!update)
				return;
			update = false;
			SceneManager.LoadScene(sceneName);
		}
	}
}