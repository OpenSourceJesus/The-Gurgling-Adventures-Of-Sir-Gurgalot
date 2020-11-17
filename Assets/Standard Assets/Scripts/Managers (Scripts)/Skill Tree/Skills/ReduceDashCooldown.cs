using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class ReduceDashCooldown : Skill
	{
		public new static ReduceDashCooldown instance;
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
				MagicCape.instance.cooldown -= seconds;
		}
	}
}