using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TGAOSG
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class Slime : Enemy
	{
		//public LayerMask whatChangesMyDirection;
		public BoxCollider2D boxCollider;
		[HideInInspector]
		public float colliderEdgeRadius;
		public bool onWall;
		public Vector2 Facing
		{
			get
			{
				return (trs.right * trs.localScale.x).Snap(Vector2.one);
			}
		}
		bool changedDirectionLastFrame;
		
		public virtual void Start ()
		{
			colliderEdgeRadius = boxCollider.edgeRadius;
		}
		
		public override void OnCollisionEnter2D (Collision2D coll)
		{
			OnCollisionStay2D (coll);
		}
		
		public override void OnCollisionStay2D (Collision2D coll)
		{
			//if (LayerMaskExtensions.MaskContainsLayer(whatChangesMyDirection, coll.gameObject.layer))
			//{
				foreach (ContactPoint2D contact in coll.contacts)
				{
					if (onWall)
					{
						if (Mathf.Abs(contact.normal.y) > .5f)
						{
							if (!changedDirectionLastFrame)
								ChangeDirection ();
							else
								changedDirectionLastFrame = false;
							return;
						}
					}
					else
					{
						if (Mathf.Abs(contact.normal.x) > .5f)
						{
							if (!changedDirectionLastFrame)
								ChangeDirection ();
							else
								changedDirectionLastFrame = false;
							return;
						}
					}
				}
			//}
		}
		
		public override void DoUpdate ()
		{
			healthbar.parent.eulerAngles = Vector3.zero;
			healthbar.parent.position = trs.position + (Vector3) healthbarCanvasOffset;
		}
		
		public virtual void FixedUpdate ()
		{
			rigid.velocity = Facing * moveSpeed;
		}
		
		public virtual void ChangeDirection ()
		{
			trs.localScale = trs.localScale.SetX(-trs.localScale.x);
			changedDirectionLastFrame = true;
		}
	}
}