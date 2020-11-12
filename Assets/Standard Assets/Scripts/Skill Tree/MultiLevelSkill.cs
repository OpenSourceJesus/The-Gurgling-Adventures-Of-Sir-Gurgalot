using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG.SkillTree
{
	[ExecuteAlways]
	public class MultiLevelSkill : Skill
	{
		public ushort level;
		[SaveAndLoadValue]
		public _ushort Level
		{
			get
			{
				return new _ushort(level);
			}
			set
			{
				level = value.value;
			}
		}
		public ushort maxLevel;
		
		public override void Learn ()
		{
			level ++;
			if (level == maxLevel)
				learned = true;
		}
	}
}