using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class AddToMagicAmuletRange : Skill
	{
		public new static AddToMagicAmuletRange instance;
		public float range;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}
		
		public override void ApplyKnowledgeIfShould ()
		{
			base.ApplyKnowledgeIfShould ();
			if (learned)
				MagicAmulet.instance.aimer.aimerLaser.maxLength += range;
		}
	}
}