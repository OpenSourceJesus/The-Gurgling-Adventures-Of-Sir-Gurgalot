using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class AddToMagicAmuletDamage : Skill
	{
		public new static AddToMagicAmuletDamage instance;
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
				MagicAmulet.instance.laserPrefab.damageOverTime += damage;
		}
	}
}