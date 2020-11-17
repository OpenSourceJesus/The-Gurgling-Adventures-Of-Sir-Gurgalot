using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class BulletPattern : ScriptableObject, IConfigurable
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
				return "Bullet Patterns";
			}
		}
		
		public virtual void Start ()
		{
		}
		
		public virtual Vector2 GetShootDirection (Transform spawner)
		{
			return spawner.up;
		}
		
		public virtual Bullet Shoot (Transform spawner, Bullet bulletPrefab)
		{
			//spawner.up = GetShootDirection(spawner);
			return ObjectPool.instance.Spawn(bulletPrefab, spawner.position, Quaternion.LookRotation(Vector3.forward, GetShootDirection(spawner)));
		}
		
		public virtual Bullet Shoot (Vector2 spawnPos, Vector2 direction, Bullet bulletPrefab)
		{
			return ObjectPool.instance.Spawn(bulletPrefab, spawnPos, Quaternion.LookRotation(Vector3.forward, direction));
		}
		
		public virtual IEnumerator Retarget (Bullet bullet, float retargetTime)
		{
			yield return new WaitForSeconds(retargetTime);
			bullet.trs.up = GetRetargetDirection(bullet);
		}
		
		public virtual Vector2 GetRetargetDirection (Bullet bullet)
		{
			return bullet.trs.up;
		}
		
		public virtual IEnumerator Split (Bullet bullet, Bullet splitBulletPrefab, float splitTime)
		{
			yield return new WaitForSeconds(splitTime);
			Shoot (bullet.trs.position, GetSplitDirection(bullet), splitBulletPrefab);
		}
		
		public virtual IEnumerator Split (Bullet bullet, Vector2 direction, Bullet splitBulletPrefabIndex, float splitTime)
		{
			yield return new WaitForSeconds(splitTime);
			Shoot (bullet.trs.position, direction, splitBulletPrefabIndex);
		}
		
		public virtual Vector2 GetSplitDirection (Bullet bullet)
		{
			return bullet.trs.up;
		}
	}
}