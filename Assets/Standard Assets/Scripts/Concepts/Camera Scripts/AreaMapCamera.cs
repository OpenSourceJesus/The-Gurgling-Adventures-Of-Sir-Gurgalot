using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.Tilemaps;

namespace TAoKR
{
	public class AreaMapCamera : CameraScript
	{
		public new static AreaMapCamera instance;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}

		public new static AreaMapCamera GetInstance ()
		{
			if (instance == null)
				instance = FindObjectOfType<AreaMapCamera>();
			return instance;
		}
	}
}