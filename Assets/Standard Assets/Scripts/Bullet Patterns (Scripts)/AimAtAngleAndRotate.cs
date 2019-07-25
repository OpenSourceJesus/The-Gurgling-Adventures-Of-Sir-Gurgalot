using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	[CreateAssetMenu]
	public class AimAtAngleAndRotate : AimWhereFacing
	{
		[MakeConfigurable]
		public float initAngle;
		[MakeConfigurable]
		public float rotate;
		Vector2 currentDir;
		
		public override void Start ()
		{
			base.Start ();
			currentDir = VectorExtensions.GetVectorFromFacingAngle(initAngle);
		}
		
		public override Vector2 GetShootDirection (Transform spawner)
		{
			currentDir = currentDir.Rotate(rotate);
			return currentDir;
		}
	}
}