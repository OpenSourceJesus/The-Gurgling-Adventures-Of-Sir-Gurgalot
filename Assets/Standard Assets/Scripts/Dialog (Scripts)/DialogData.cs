using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TAoKR.Dialog
{
	[CreateAssetMenu]
	public class DialogData : SingletonScriptableObject<DialogData>
	{
		public string dialogFolderPath;
		public List<DialogEntry> allEntries = new List<DialogEntry>();
	}
}