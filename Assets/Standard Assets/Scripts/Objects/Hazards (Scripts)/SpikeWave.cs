using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace TAoKR
{
	public class SpikeWave : Hazard, ISpawnable
	{
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}
		public float maxLength;
		public float createRate;
		public float destroyDelay;
		public float destroyRate;
		float currentLength;
		public Transform leadEnd;
		public Transform middle;
		public Transform trailingEnd;
		public bool canBeBlocked;
		bool flipped;
		public LayerMask whatBlocksMe;
		public BoxCollider2D boxCollider;
		public Transform graphicsParent;
		public float addToDestroyGraphicsDist;
		float destroyGraphicsDist;
		float distTilDestroyGraphic;
		
		public virtual void Start ()
		{
			currentLength = collider.bounds.size.x;
			StartCoroutine(CreateRoutine ());
			StartCoroutine(DestroyRoutine ());
			destroyGraphicsDist = graphicsParent.GetChild(1).position.x - graphicsParent.GetChild(0).position.x + addToDestroyGraphicsDist;
			distTilDestroyGraphic = destroyGraphicsDist;
		}
		
		public virtual IEnumerator CreateRoutine ()
		{
			while (true)
			{
				Move (createRate * Time.deltaTime, leadEnd);
				yield return new WaitForEndOfFrame();
			}
		}
		
		public virtual IEnumerator DestroyRoutine ()
		{
			yield return new WaitForSeconds(destroyDelay);
			while (true)
			{
				Move (destroyRate * Time.deltaTime, trailingEnd);
				yield return new WaitForEndOfFrame();
			}
		}
		
		public virtual void Move (float move, Transform end)
		{
			if (middle.localScale.x < 0)
			{
				Destroy(gameObject);
				return;
			}
			if (currentLength > maxLength)
			{
				StopCoroutine (CreateRoutine ());
				return;
			}
			if (canBeBlocked && Physics2D.BoxCast(end.position, new Vector2((currentLength - middle.localScale.x) / 2, collider.bounds.size.y), 0, leadEnd.position - trailingEnd.position, move, whatBlocksMe).collider != null)
				return;
			Vector2 toLeadEnd = leadEnd.position - trailingEnd.position;
			end.position += (Vector3) toLeadEnd.normalized * move;
			if (end == leadEnd)
			{
				currentLength += move;
				middle.position += (Vector3) toLeadEnd.normalized * move / 2;
				middle.localScale += Vector3.right * move;
			}
			else if (end == trailingEnd)
			{
				currentLength -= move;
				middle.position += (Vector3) toLeadEnd.normalized * move / 2;
				middle.localScale -= Vector3.right * move;
				distTilDestroyGraphic -= move;
				while (distTilDestroyGraphic < 0)
				{
					distTilDestroyGraphic += destroyGraphicsDist;
					if (!flipped)
					{
						while (boxCollider.bounds.ToRect().min.x > graphicsParent.GetChild(0).position.x + distTilDestroyGraphic)
							DestroyImmediate(graphicsParent.GetChild(0).gameObject);
					}
					else
					{
						while (boxCollider.bounds.ToRect().max.x < graphicsParent.GetChild(graphicsParent.childCount - 1).position.x - distTilDestroyGraphic)
							DestroyImmediate(graphicsParent.GetChild(graphicsParent.childCount - 1).gameObject);
					}
				}
			}
			boxCollider.offset = boxCollider.offset.SetX(((leadEnd.position + trailingEnd.position) / 2).x - trs.position.x);
			boxCollider.size = boxCollider.size.SetX(currentLength);
		}
		
		public virtual void Flip ()
		{
			Transform previousLeadEnd = leadEnd;
			leadEnd = trailingEnd;
			trailingEnd = previousLeadEnd;
			flipped = !flipped;
		}
	}
}