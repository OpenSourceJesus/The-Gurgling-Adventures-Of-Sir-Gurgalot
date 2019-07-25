using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class AddToMagicHatRange : Skill
	{
		public new static AddToMagicHatRange instance;
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
				MagicHat.instance.rangeTrs.localScale += Vector3.one * range;
		}
	}
}