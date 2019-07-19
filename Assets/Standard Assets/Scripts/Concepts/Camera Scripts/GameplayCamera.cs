using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	public class GameplayCamera : CameraScript
	{
		public new static GameplayCamera instance;
		public float followDist;
		Vector2 toPlayer;
		[HideInInspector]
		public Rect worldViewRect;
		Vector2 newPos;
		public float lookRange;
		Vector2 toLookPos;
		public Rect UnblockedWorldViewRect
		{
			get
			{
				toPlayer = Player.instance.trs.position - trs.position;
				worldViewRect.center = Player.instance.trs.position - Vector2.ClampMagnitude(toPlayer, followDist).SetZ(trs.position.z);
				worldViewRect.size = camera.ViewportToWorldPoint(Vector2.one) - camera.ViewportToWorldPoint(Vector2.zero);
				return worldViewRect;
			}
		}
		public BoxCollider2D boxCollider;
		public LayerMask whatICollideWith;
		Vector2 minDiff;
		Vector2 maxDiff;
		Vector2 toNewPos;
		Vector2 movement;
		Vector2 previousMovement;
		Rect playerRect;
		public float minPlayerDistFromEdge;
		public float AimingSensitivity
		{
			get
			{
				return PlayerPrefs.GetFloat("Aiming Sensitivity", 1);
			}
			set
			{
				PlayerPrefsExtensions.SetFloat("Aiming Sensitivity", value);
			}
		}
		
		public override void Start ()
		{
			base.Start ();
			instance = this;
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
			boxCollider.size = worldViewRect.size;
			trs.SetParent(null);
		}
		
		public virtual bool IncludeFullRectInViewIfCan (Rect rect)
		{
			bool output = rect.size.x <= worldViewRect.size.x && rect.size.y <= worldViewRect.size.y;
			if (output)
			{
				minDiff = worldViewRect.min - rect.min;
				maxDiff = rect.max - worldViewRect.max;
				minDiff = minDiff.ClampVectorComponents(Vector2.zero, VectorExtensions.INFINITE);
				maxDiff = maxDiff.ClampVectorComponents(Vector2.zero, VectorExtensions.INFINITE);
				worldViewRect.center += maxDiff - minDiff;
			}
			return output;
		}
		
		public override void HandlePosition ()
		{
			base.HandlePosition ();
			worldViewRect = UnblockedWorldViewRect;
			ConstrainPositionToVisionRect (Area.instance.visibleRect);
			HandleLooking ();
			worldViewRect.center += toLookPos;
			playerRect = Player.instance.colliderRect.Expand(Vector2.one * minPlayerDistFromEdge * 2);
			IncludeFullRectInViewIfCan (playerRect);
			ConstrainPositionToVisionRect (Area.instance.visibleRect);
			toNewPos = worldViewRect.center - (Vector2) trs.position;
			if (Physics2D.OverlapArea(camera.ViewportToWorldPoint(Vector2.zero) + (Vector3) toNewPos, camera.ViewportToWorldPoint(Vector2.one) + (Vector3) toNewPos, whatICollideWith) == null)
				trs.position = worldViewRect.center.SetZ(trs.position.z);
		}
		
		public virtual void HandleLooking ()
		{
			if (InputManager.usingJoystick)
				toLookPos = InputManager.GetAxis2D("Aim Horizontal", "Aim Vertical") * lookRange;
			else
			{
				movement = (camera.ScreenToWorldPoint(Input.mousePosition) - trs.position) * AimingSensitivity;
				if (movement != previousMovement)
				{
					toLookPos = movement;
					previousMovement = movement;
				}
			}
			toLookPos = Vector2.ClampMagnitude(toLookPos, lookRange);
		}
		
		public virtual void ConstrainPositionToVisionRect (Rect bounds)
		{
			minDiff = bounds.min - (Vector2) worldViewRect.min;
			maxDiff = (Vector2) worldViewRect.max - bounds.max;
			if (worldViewRect.size.x < Area.instance.visibleRect.size.x)
			{
				if (minDiff.x > 0)
				{
					worldViewRect.min = new Vector2(bounds.min.x, worldViewRect.min.y);
					worldViewRect.max += new Vector2(minDiff.x, 0);
				}
				if (maxDiff.x > 0)
				{
					worldViewRect.max = new Vector2(bounds.max.x, worldViewRect.max.y);
					worldViewRect.min -= new Vector2(maxDiff.x, 0);
				}
			}
			else
				worldViewRect.center = new Vector2(bounds.center.x, worldViewRect.center.y);
			if (worldViewRect.size.y < Area.instance.visibleRect.size.y)
			{
				if (minDiff.y > 0)
				{
					worldViewRect.min = new Vector2(worldViewRect.min.x, bounds.min.y);
					worldViewRect.max += new Vector2(0, minDiff.y);
				}
				if (maxDiff.y > 0)
				{
					worldViewRect.max = new Vector2(worldViewRect.max.x, bounds.max.y);
					worldViewRect.min -= new Vector2(0, maxDiff.y);
				}
			}
			else
				worldViewRect.center = new Vector2(worldViewRect.center.x, bounds.center.y);
		}
		
		public override void HandleViewSize ()
		{
			base.HandleViewSize ();
			while (worldViewRect.IsExtendingOutside(Area.instance.visibleRect, false))
			{
				camera.orthographicSize -= .1f;
				HandlePosition ();
			}
		}

		public new static GameplayCamera GetInstance ()
		{
			if (instance == null)
				instance = FindObjectOfType<GameplayCamera>();
			return instance;
		}
	}
}