using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Extensions;

namespace TGAOSG
{
	[RequireComponent(typeof(_Rigidbody2D))]
	[RequireComponent(typeof(Animator))]
	public class Platformer : MonoBehaviour, IMoveable
	{
		public float moveSpeed;
		public virtual float MoveSpeed
		{
			get
			{
				return moveSpeed;
			}
			set
			{
				moveSpeed = value;
			}
		}
		public new Collider2D collider;
		[HideInInspector]
		public Rect colliderRect;
		public RaycastHit2D hit;
		[HideInInspector]
		public Vector2 hitRaycastArea;
		public Dictionary<Activity, ActivityStatus> activityStatus = new Dictionary<Activity, ActivityStatus>();
		public RaycastHit2D[] verticalHits = new RaycastHit2D[1];
		public RaycastHit2D[] horizontalHits = new RaycastHit2D[1];
		[HideInInspector]
		public LayerMask whatICollideWith;
		[HideInInspector]
		public bool hittingRight;
		[HideInInspector]
		public bool hittingLeft;
		[HideInInspector]
		public float move;
		public Rigidbody2D rigid;
		public Animator anim;
		public Transform trs;
		public float extendHorizontalChecks;
		public float extendVerticalChecks;
		[HideInInspector]
		public float defaultGravityScale;
		public float jumpSpeed;
		public float fallMultiplier;
		public float lowJumpMultiplier;
		[HideInInspector]
		public float boxColliderEdgeRadius;
		[HideInInspector]
		public bool hittingWall;
		[HideInInspector]
		public bool hittingRoof;
		[HideInInspector]
		public bool hittingGround;
		
		public virtual void Awake ()
		{
			defaultGravityScale = rigid.gravityScale;
			whatICollideWith = Physics2D.GetLayerCollisionMask(gameObject.layer);
			whatICollideWith = LayerMaskExtensions.RemoveFromMask(whatICollideWith, LayerMask.LayerToName(gameObject.layer));
			Array activities = Enum.GetValues(typeof(Activity));
			for (int i = 0; i < activities.Length; i ++)
				activityStatus.Add((Activity) activities.GetValue(i), new ActivityStatus(ActivityState.NotDoing, true));
			//extendWallChecks += Physics2D.defaultContactOffset;
			BoxCollider2D boxCollider = collider as BoxCollider2D;
			if (boxCollider != null)
				boxColliderEdgeRadius = boxCollider.edgeRadius;
		}
		
		public virtual void UpdateWhatICanDo ()
		{
			colliderRect = collider.bounds.ToRect().Expand(Vector2.one * boxColliderEdgeRadius * 2);
			hitRaycastArea = colliderRect.size;
			hitRaycastArea += Vector2.up * extendVerticalChecks * 2;
			hittingGround = false;
			hittingRoof = false;
			if (Physics2D.CapsuleCastNonAlloc(colliderRect.center, hitRaycastArea, CapsuleDirection2D.Vertical, 0, Vector2.zero, verticalHits, 0, whatICollideWith) > 0)
			{
				for (int i = 0; i < verticalHits.Length; i ++)
				{
					hit = verticalHits[i];
					if (hit.collider != collider && Mathf.Abs(hit.normal.y) > Vector2.one.normalized.y)
					{
						if (hit.point.y < colliderRect.center.y)
						{
							activityStatus[Activity.Jumping].canDo = true;
							activityStatus[Activity.Falling].canDo = false;
							activityStatus[Activity.Falling].state = ActivityState.NotDoing;
							hittingGround = true;
						}
						else
						{
							activityStatus[Activity.Jumping].canDo = false;
							activityStatus[Activity.Falling].canDo = true;
							StopJump ();
							hittingRoof = true;
						}
						//break;
					}
				}
			}
			else
			{
				activityStatus[Activity.Falling].canDo = true;
				activityStatus[Activity.Jumping].canDo = false;
			}
			hittingLeft = false;
			hittingRight = false;
			hitRaycastArea = colliderRect.size;
			hitRaycastArea += Vector2.right * extendHorizontalChecks * 2;
			if (Physics2D.CapsuleCastNonAlloc(colliderRect.center, hitRaycastArea, CapsuleDirection2D.Vertical, 0, Vector2.zero, horizontalHits, 0, whatICollideWith) > 0)
			{
				for (int i = 0; i < horizontalHits.Length; i ++)
				{
					hit = horizontalHits[i];
					if (hit.collider != collider && Mathf.Abs(hit.normal.x) > Vector2.one.normalized.x)
					{
						if (hit.point.x < colliderRect.center.x)
							hittingLeft = true;
						else
							hittingRight = true;
						//break;
					}
				}
			}
			hittingWall = (move > 0 && hittingRight) || (move < 0 && hittingLeft);
			activityStatus[Activity.Walking].canDo = !hittingWall;
			//if (!activityStatus[Activity.Walking].canDo)
			//	Idle ();
		}
		
		public virtual void Move (float move)
		{
			if (move == 0)
				return;
			activityStatus[Activity.Walking].state = ActivityState.Doing;
			activityStatus[Activity.Idle].state = ActivityState.NotDoing;
			if (!hittingWall)
				rigid.velocity = rigid.velocity.SetX(move * MoveSpeed);
			anim.SetFloat("speed", Mathf.Abs(rigid.velocity.x) * Mathf.Sign(trs.localScale.x) / MoveSpeed);
			anim.speed = 1;
			anim.Play ("Walk");
		}
		
		public virtual void HandleFacing ()
		{
			if (move != 0)
				FaceDirection (MathfExtensions.Sign(move));
		}
		
		public virtual void FaceDirection (int direction)
		{
			trs.localScale = trs.localScale.SetX(Mathf.Sign(direction) * Mathf.Abs(trs.localScale.x));
		}
		
		public virtual void Idle ()
		{
			rigid.velocity = rigid.velocity.SetX(0);
			activityStatus[Activity.Idle].state = ActivityState.Doing;
			activityStatus[Activity.Walking].state = ActivityState.NotDoing;
			anim.Play ("Idle");
		}
		
		public virtual void StartJump ()
		{
			activityStatus[Activity.Jumping].state = ActivityState.Doing;
			activityStatus[Activity.Falling].state = ActivityState.NotDoing;
		}
		
		public virtual void StopJump ()
		{
			activityStatus[Activity.Jumping].state = ActivityState.NotDoing;
			activityStatus[Activity.Falling].state = ActivityState.Doing;
		}
	}
}