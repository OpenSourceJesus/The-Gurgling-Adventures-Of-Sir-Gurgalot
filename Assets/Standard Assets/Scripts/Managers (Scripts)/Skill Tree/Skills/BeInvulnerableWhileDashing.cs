using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	public class BeInvulnerableWhileDashing : Skill
	{
		public new static BeInvulnerableWhileDashing instance;
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
		}
	}
}