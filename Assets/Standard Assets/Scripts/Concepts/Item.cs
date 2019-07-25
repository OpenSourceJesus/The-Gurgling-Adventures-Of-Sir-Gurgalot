using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class Item : SingletonMonoBehaviour<Item>, IConfigurable
	{
		public string Name
		{
			get
			{
				return name;
			}
		}
		public virtual string Category
		{
			get
			{
				return "Items";
			}
		}
		public new SpriteRenderer renderer;
		[MakeConfigurable]
		public float cooldown;
		[HideInInspector]
		public float cooldownTimer;
		[HideInInspector]
		public float gamma;
		
		public virtual IEnumerator Cooldown ()
		{
			cooldownTimer += cooldown;
			gamma = 0;
			renderer.color = new Color(gamma, gamma, gamma);
			while (cooldownTimer > 0)
			{
				cooldownTimer -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			gamma = 1;
			renderer.color = new Color(gamma, gamma, gamma);
		}
	}
}