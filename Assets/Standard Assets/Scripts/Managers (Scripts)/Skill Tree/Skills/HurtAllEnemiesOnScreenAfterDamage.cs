using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class HurtAllEnemiesOnScreenAfterDamage : Skill
	{
		public new static HurtAllEnemiesOnScreenAfterDamage instance;
		public int damage;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}
	}
}