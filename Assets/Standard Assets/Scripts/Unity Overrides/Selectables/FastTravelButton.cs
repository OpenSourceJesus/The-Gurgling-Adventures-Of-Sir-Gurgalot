using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TGAOSG
{
	[ExecuteAlways]
	[RequireComponent(typeof(Button))]
	public class FastTravelButton : BorderedSelectable
	{
		public Button button;
		public Text nameText;
		
		public override void Start ()
		{
			base.Start ();
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			nameText.text = name;
			if (name == gameObject.scene.name)
				Interactable = false;
			button.onClick.AddListener(delegate { FastTravel (); });
		}
		
		public virtual void FastTravel ()
		{
			if (!Interactable)
				return;
			Player.instance.SetEntrance ("Obelisk");
			GameManager.instance.Unpause ();
			GameManager.instance.LoadLevel (name);
		}
	}
}