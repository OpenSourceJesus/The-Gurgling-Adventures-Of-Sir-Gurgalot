using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class AddToMagicHatThrowSpeed : Skill
	{
		public new static AddToMagicHatThrowSpeed instance;
		public int speed;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}
		
		public override void ApplyKnowledgeIfShould ()
		{
			base.ApplyKnowledgeIfShould ();
			if (learned)
				MagicHat.instance.throwForce += speed;
		}
	}
}