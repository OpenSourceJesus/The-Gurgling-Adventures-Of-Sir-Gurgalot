using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TGAOSG.Story
{
	[CreateAssetMenu]
	public class StoryData : SingletonScriptableObject<StoryData>
	{
		public string questsFolderPath;
		public List<Quest> allQuests = new List<Quest>();
	}
}