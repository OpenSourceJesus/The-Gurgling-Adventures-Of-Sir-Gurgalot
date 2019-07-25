using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class FriendlyArea : Area
	{
		public override void Start ()
		{
			base.Start ();
			Player.GetInstance().sword.gameObject.SetActive(false);
			if (MagicAmulet.instance != null)
				MagicAmulet.instance.enabled = false;
		}
	}
}