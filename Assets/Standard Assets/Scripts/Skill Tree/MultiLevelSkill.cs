using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	[ExecuteAlways]
	public class MultiLevelSkill : Skill
	{
		[SaveAndLoadValue]
		public ushort level;
		public ushort maxLevel;
		
		public override void Learn ()
		{
			level ++;
			if (level == maxLevel)
				learned = true;
		}
	}
}