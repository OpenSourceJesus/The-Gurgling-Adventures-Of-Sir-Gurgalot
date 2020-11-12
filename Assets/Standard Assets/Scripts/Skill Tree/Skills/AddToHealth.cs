using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class AddToHealth : Skill
	{
		public new static AddToHealth instance;
		public int health;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}
		
		public override void ApplyKnowledgeIfShould ()
		{
			base.ApplyKnowledgeIfShould ();
			if (learned)
				Player.instance.maxHp = (uint) (Player.instance.maxHp + health);
		}
	}
}