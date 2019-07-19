using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class MainMenu : SingletonMonoBehaviour<MainMenu>
	{
		public ushort accountCount;
		public RectTransform accountOptionsParent;
		public AccountOptions accountOptionsPrefab;
		public GameObject copyingAccountTextObj;
		public TemporaryDisplayObject copiedAccountNotificationObj;
		public TemporaryDisplayObject erasedAccountNotificationObj;

		public override void Start ()
		{
			base.Start ();
			for (int i = 1; i <= accountCount; i ++)
				Instantiate(accountOptionsPrefab, accountOptionsParent);
		}
	}
}