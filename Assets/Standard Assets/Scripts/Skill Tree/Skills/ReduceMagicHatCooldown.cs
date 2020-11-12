using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class ReduceMagicHatCooldown : Skill
	{
		public new static ReduceMagicHatCooldown instance;
		public float seconds;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}
		
		public override void ApplyKnowledgeIfShould ()
		{
			base.ApplyKnowledgeIfShould ();
			if (learned)
				MagicHat.instance.cooldown -= seconds;
		}
	}
}