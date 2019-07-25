using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class AddToSwordDamage : Skill
	{
		public new static AddToSwordDamage instance;
		public float damage;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}
		
		public override void ApplyKnowledgeIfShould ()
		{
			base.ApplyKnowledgeIfShould ();
			if (learned)
				Player.instance.sword.damageOverTime += damage;
		}
	}
}