using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR.SkillTree
{
	public class MagicHatIsInvulnerable : Skill
	{
		public new static MagicHatIsInvulnerable instance;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}
	}
}