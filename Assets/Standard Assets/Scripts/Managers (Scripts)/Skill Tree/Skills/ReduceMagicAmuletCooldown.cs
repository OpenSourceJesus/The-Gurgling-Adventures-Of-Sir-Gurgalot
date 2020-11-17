using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class ReduceMagicAmuletCooldown : Skill
	{
		public new static ReduceMagicAmuletCooldown instance;
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
				MagicAmulet.instance.cooldown -= seconds;
		}
	}
}