using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;

public class DissolveBlock : MonoBehaviour
{
	public new Collider2D collider;
	public SpriteRenderer spriteRenderer;
	public float dissolveDuration;
	float dissolveTimer;
	public float revivingDuration;
	LayerMask whatICollideWith;
	Rect colliderBoundsRect;
	
	public virtual void Start ()
	{
		whatICollideWith = Physics2D.GetLayerCollisionMask(gameObject.layer);
		colliderBoundsRect = collider.bounds.ToRect();
		dissolveTimer = dissolveDuration;
	}
	
	public virtual void OnCollisionEnter2D (Collision2D coll)
	{
		StartCoroutine(DissolveRoutine ());
	}
	
	public virtual IEnumerator DissolveRoutine ()
	{
		while (dissolveTimer > 0)
		{
			dissolveTimer -= Time.deltaTime;
			spriteRenderer.color = spriteRenderer.color.SetAlpha(dissolveTimer / dissolveDuration);
			yield return new WaitForEndOfFrame();
		}
		collider.enabled = false;
		spriteRenderer.enabled = false;
		StartCoroutine(ReviveRoutine ());
	}
	
	public virtual IEnumerator ReviveRoutine ()
	{
		yield return new WaitForSeconds(revivingDuration);
		yield return new WaitUntil(() => (Physics2D.OverlapArea(colliderBoundsRect.min, colliderBoundsRect.max, whatICollideWith) == null));
		dissolveTimer = dissolveDuration;
		collider.enabled = true;
		spriteRenderer.color = spriteRenderer.color.SetAlpha(1);
		spriteRenderer.enabled = true;
	}
}
