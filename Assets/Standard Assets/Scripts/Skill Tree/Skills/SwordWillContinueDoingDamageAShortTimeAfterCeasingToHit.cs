using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR.SkillTree
{
	public class SwordWillContinueDoingDamageAShortTimeAfterCeasingToHit : Skill
	{
		public new static SwordWillContinueDoingDamageAShortTimeAfterCeasingToHit instance;
		public float seconds;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}
	}
}