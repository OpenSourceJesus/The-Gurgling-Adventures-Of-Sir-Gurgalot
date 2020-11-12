using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TGAOSG.SkillTree
{
	[ExecuteAlways]
	public class LearnSkillButton : BorderedSelectable
	{
		public Button button;
		public bool autoSetNameText;
		public Text nameText;
		public Text costText;
		public bool autoSetSkill;
		public Skill skill;
		public Color learnedColor;
		
		public override void Start ()
		{
			base.Start ();
			if (autoSetSkill)
			{
				foreach (Skill _skill in SkillTreeManager.instance.skills)
				{
					if (_skill.name == name)
					{
						skill = _skill;
						break;
					}
				}
			}
			if (autoSetNameText)
				nameText.text = skill.skillName;
			costText.text += "" + skill.cost;
			if (!skill.unlocked || skill.learned)
				Disable ();
		}
		
		public virtual void OnPressed ()
		{
			bool learned = skill.TryToLearn();
			if (learned)
			{
				Disable ();
				LearnSkillButton[] skillButtons = rectTrs.parent.GetComponentsInChildren<LearnSkillButton>();
				if (skillButtons.Length > rectTrs.GetSiblingIndex() + 1)
					skillButtons[rectTrs.GetSiblingIndex() + 1].skill.Unlock ();
			}
		}
		
		public virtual void Disable ()
		{
			Interactable = false;
			if (skill.learned)
				button.image.color = learnedColor;
		}
	}
}