// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(SpriteRenderer))]
// public class MakeSpriteFromCollider2D : EditorScript
// {
// 	public new Collider2D collider;
// 	public SpriteRenderer spriteRenderer;
// 	public float colliderCheckDistInterval;
// 	public bool onlyAllowSpriteInsideCollider;

//     public virtual void Start ()
//     {
// #if UNITY_EDITOR
//     	if (!Application.isPlaying)
//     	{
//     		if (collider == null)
//     			collider = GetComponent<Collider2D>();
//     		if (spriteRenderer == null)
//     			spriteRenderer = GetComponent<SpriteRenderer>();
//     		return;
//     	}
// #endif
//     }

//     Color[] colors;
//     public override void Activate ()
//     {
//     	colors = spriteRenderer.sprite.texture.GetPixels();
//         for (float x = collider.bounds.min.x; x < collider.bounds.max.x; x += colliderCheckDistInterval)
//         {
//         	for (float y = collider.bounds.min.y; y < collider.bounds.max.y; y += colliderCheckDistInterval)
// 	        {
// 	        	if (Physics2D.OverlapPoint(new Vector2(x, y)) != onlyAllowSpriteInsideCollider)
// 	        	{
// 	        		colors[]
// 	        	}
// 	        }
//         }
//     }
// }
