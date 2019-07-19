using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	public class Boss2 : Boss
	{
		public Transform[] laserParents;
		[MakeConfigurable]
		public float[] laserRotaRates;
		public new SpriteRenderer renderer;
		public override float InvulnerableFlashValue
		{
			get
			{
				return invulnerableFlashValue;
			}
			set
			{
				invulnerableFlashValue = value;
				renderer.color = ColorExtensions.SetAlpha(renderer.color, value);
			}
		}
		
		public override void Update ()
		{
			//base.Update ();
			for (int i = 0; i < laserParents.Length; i ++)
				laserParents[i].eulerAngles += Vector3.forward * laserRotaRates[i] * Time.deltaTime;
		}
	}
}