using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class EyeTurret : ShootingEnemy
	{
		[MakeConfigurable]
		public float rotateSpeed;
		public Animator anim;
		public AnimationClip prepareShotAnimClip;
		bool preparingShot;
		public override bool BleedBoundsIsCircle
		{
			get
			{
				return true;
			}
		}
		
		public override void Start ()
		{
			base.Start ();
			healthbar.parent.SetParent(null);
		}
		
		public override void DoUpdate ()
		{
			base.DoUpdate ();
			if (!awakened)
			{
				trs.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
				return;
			}	
			trs.up = Player.instance.trs.position - trs.position;
			healthbar.parent.eulerAngles = Vector3.zero;
		}
		
		public override void HandleShooting (ShootEntry shootEntry)
		{
			if (shootEntry.shootTimer > 0)
				shootEntry.shootTimer -= Time.deltaTime;
			if (!preparingShot && shootEntry.shootTimer <= prepareShotAnimClip.length)
			{
				anim.Play ("Prepare Shoot");
				preparingShot = true;
			}
			else if (preparingShot && !anim.GetCurrentAnimatorStateInfo(0).IsName("Prepare Shoot"))
			{
				shootEntry.bulletPattern.Shoot(shootEntry.spawner, shootEntry.bulletPrefab);
				shootEntry.shootTimer += shootEntry.shootRate;
				preparingShot = false;
			}
		}
		
		public override void Death ()
		{
			base.Death ();
			Destroy (healthbar.parent.gameObject);
		}
	}
}