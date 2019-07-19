using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class Oriolus : Uriad
	{
		public override void Start ()
		{
			base.Start ();
			if (initMove.y != 0)
				rigid.constraints |= RigidbodyConstraints2D.FreezePositionX;
			else
				rigid.constraints |= RigidbodyConstraints2D.FreezePositionY;
		}
	}
}