using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	public class Item : SingletonMonoBehaviour<Item>, IConfigurable, IUpdatable
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
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public new SpriteRenderer renderer;
		[MakeConfigurable]
		public float cooldown;
		[HideInInspector]
		public float cooldownTimer;
		[HideInInspector]
		public float gamma;

		public virtual void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public virtual void DoUpdate ()
		{
		}
		
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

		public virtual void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}