using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Extensions;

[RequireComponent(typeof(SpriteRenderer))]
public class MakeSpriteFromCollider2D : EditorScript
{
	public new Collider2D collider;
	public SpriteRenderer spriteRenderer;
	public Vector2Int colliderChecks;
	public bool onlyAllowSpriteInsideCollider;
    public TextureFormat textureFormat;

    public virtual void Start ()
    {
#if UNITY_EDITOR
    	if (!Application.isPlaying)
    	{
    		if (collider == null)
    			collider = GetComponent<Collider2D>();
    		if (spriteRenderer == null)
    			spriteRenderer = GetComponent<SpriteRenderer>();
    		return;
    	}
#endif
    }

    public override void Activate ()
    {
    	Color[] colors = new Color[colliderChecks.x * colliderChecks.y];
        for (int x = 0; x < colliderChecks.x; x ++)
        {
            for (int y = 0; y < colliderChecks.y; y ++)
	        {
	        	if (collider.OverlapPoint(collider.bounds.min + (collider.bounds.size.Multiply(new Vector2(x / colliderChecks.x, y / colliderChecks.y)))) != onlyAllowSpriteInsideCollider)
	        		colors[x + y * colliderChecks.x] = Color.white;
	        }
        }
        Texture2D texture = null;
        int i = 0;
        while (texture == null)
        {
            texture = Texture2D.CreateExternalTexture(colliderChecks.x, colliderChecks.y, textureFormat, false, false, new IntPtr(i));
            i ++;
        }
        texture.SetPixels(colors);
        texture.Apply();
        spriteRenderer.sprite = Sprite.Create(texture, Rect.MinMaxRect(0, 0, 1, 1), Vector2.one / 2);
    }
}
