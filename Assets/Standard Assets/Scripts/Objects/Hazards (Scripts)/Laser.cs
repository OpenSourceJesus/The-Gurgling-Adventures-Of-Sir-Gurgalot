using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.Tilemaps;

namespace TGAOSG
{
	[RequireComponent(typeof(LineRenderer))]
	public class Laser : Hazard, ISpawnable, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}
		[MakeConfigurable]
		public float maxLength;
		public LayerMask whatBlocksMe;
		[MakeConfigurable]
		public float activateTime;
		public EdgeCollider2D edgeCollider;
		public LineRenderer line;
		[HideInInspector]
		public RaycastHit2D hit;
		[HideInInspector]
		public RaycastHit2D damaging;
		[HideInInspector]
		public Vector2[] verticies;
		
		public virtual void Start ()
		{
			verticies = new Vector2[2] {Vector2.zero, Vector2.up};
			line.positionCount = 2;
			line.SetPosition(0, verticies[0]);
			line.SetPosition(1, verticies[1]);
			Color c = ColorExtensions.DivideAlpha(line.endColor, 2);
			line.startColor = c;
			line.endColor = c;
			StartCoroutine(Activate ());
		}
		
		public virtual IEnumerator Activate ()
		{
			yield return new WaitForSeconds(activateTime);
			Color c = ColorExtensions.MultiplyAlpha(line.endColor, 2);
			line.startColor = c;
			line.endColor = c;
			edgeCollider.points = verticies;
			edgeCollider.enabled = true;
		}
		
		public virtual void DoUpdate ()
		{
			hit = Physics2D.Raycast(trs.position, trs.up, maxLength, whatBlocksMe);
			if (hit.collider != null)
			{
				line.SetPosition(1, Vector2.up * Vector2.Distance(trs.position, hit.point));
				verticies[1] = Vector2.up * Vector2.Distance(trs.position, hit.point);
			}
			else
			{
				line.SetPosition(1, Vector2.up * maxLength);
				verticies[1] = Vector2.up * maxLength;
			}
			edgeCollider.points = verticies;
		}
	
		public override void OnTriggerEnter2D (Collider2D other)
		{
			if (collider == null || !collider.enabled || !useDamage || Player.instance.Invulnerable)
				return;
			IDestructable destructable = other.GetComponent<IDestructable>();
			if (destructable != null)
			{
				MakeBleed (destructable);
				destructable.TakeDamage(damage);
			}
		}
		
		public override void OnTriggerStay2D (Collider2D other)
		{
			OnTriggerEnter2D (other);
		}
		
		public override void MakeBleed (IDestructable destructable)
		{
			if (destructable.BleedBounds == null)
				return;
			damaging = Physics2D.Raycast(trs.position, trs.up, maxLength, Physics2D.GetLayerCollisionMask(gameObject.layer));
			Vector2 pos = destructable.BleedBounds.bounds.ClosestPoint(damaging.point + (Random.insideUnitCircle * randomizeBleedPosition));
			if (destructable.BleedBoundsIsCircle)
				pos = damaging.point + Vector2.ClampMagnitude(pos - (Vector2) damaging.point, Mathf.Abs(destructable.BleedBounds.bounds.extents.x));
			Vector2 facing = pos - (Vector2) trs.position;
			facing = facing.Rotate(Random.Range(-randomizeBleedDirection, randomizeBleedDirection));
			destructable.Bleed (pos, facing);
		}
	}
}