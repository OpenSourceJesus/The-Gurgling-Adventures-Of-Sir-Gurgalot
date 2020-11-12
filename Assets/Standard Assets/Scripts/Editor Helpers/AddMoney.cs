using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TGAOSG
{
	[ExecuteInEditMode]
	public class AddMoney : MonoBehaviour
	{
		public ushort amount;
		public bool update;

		void Update ()
		{
			if (!update)
				return;
			update = false;
			Player.instance.AddMoney (amount);
		}
	}
}