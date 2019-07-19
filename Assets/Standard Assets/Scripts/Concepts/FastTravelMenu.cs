using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class FastTravelMenu : SingletonMonoBehaviour<FastTravelMenu>
	{
		public GameObject[] continents;
		
		public virtual void OnEnable ()
		{
			GameManager.instance.SetGosActive ();
			foreach (GameObject continent in continents)
				continent.SetActive(continent.GetComponentInChildren<FastTravelButton>() != null);
		}
	}
}