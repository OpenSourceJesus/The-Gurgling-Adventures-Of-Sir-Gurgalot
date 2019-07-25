#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Extensions;
using UnityEditor;
using TGAOSG;

[RequireComponent(typeof(SpriteRenderer))]
public class MakeSpriteFromCollider2D : EditorScript
{
	public new Collider2D collider;
	public Vector2Int colliderChecks;
	public Behaviour behaviour;
	public string assetPath;
	public bool checkDiagonals;
	public float outlineRadius;
	public int textureBorder;
	BoxCollider2D boxCollider;
	float boxColliderEdgeRadius;
	Bounds colliderBounds;

	public virtual void Start ()
	{
		if (!Application.isPlaying)
		{
			if (collider == null)
				collider = GetComponent<Collider2D>();
			return;
		}
	}

	public override void Activate ()
	{
		Color[] colors = new Color[(colliderChecks.x + textureBorder * 2) * (colliderChecks.y + textureBorder * 2)];
		Vector2Int[] outlinePositions = new Vector2Int[0];
		bool foundCollider;
		bool foundNoCollider;
		colliderBounds = collider.bounds;
		boxCollider = collider as BoxCollider2D;
		if (boxCollider != null)
		{
			boxColliderEdgeRadius = boxCollider.edgeRadius;
			colliderBounds.Expand(Vector2.one * boxColliderEdgeRadius * 2);
			boxCollider.edgeRadius = 0;
		}
		for (int x = 0; x < colliderChecks.x + textureBorder * 2; x ++)
		{
			for (int y = 0; y < colliderChecks.y + textureBorder * 2; y ++)
			{
				if (behaviour == Behaviour.FillInside)
				{
					if (CheckForCollider(new Vector2Int(x, y)))
						colors[x + y * (colliderChecks.x + textureBorder * 2)] = Color.white;
				}
				else if (behaviour == Behaviour.FillOutside)
				{
					if (!CheckForCollider(new Vector2Int(x, y)))
						colors[x + y * (colliderChecks.x + textureBorder * 2)] = Color.white;
				}
				else
				{
					foundCollider = CheckForCollider(new Vector2Int(x, y)) || CheckForCollider(new Vector2Int(x + 1, y)) || CheckForCollider(new Vector2Int(x, y + 1)) || CheckForCollider(new Vector2Int(x - 1, y)) || CheckForCollider(new Vector2Int(x, y - 1));
					foundNoCollider = !CheckForCollider(new Vector2Int(x, y)) || !CheckForCollider(new Vector2Int(x + 1, y)) || !CheckForCollider(new Vector2Int(x, y + 1)) || !CheckForCollider(new Vector2Int(x - 1, y)) || !CheckForCollider(new Vector2Int(x, y - 1));
					if (checkDiagonals)
					{
						foundCollider |= CheckForCollider(new Vector2Int(x + 1, y + 1)) || CheckForCollider(new Vector2Int(x - 1, y + 1)) || CheckForCollider(new Vector2Int(x - 1, y + 1)) || CheckForCollider(new Vector2Int(x - 1, y - 1)); 
						foundNoCollider |= !CheckForCollider(new Vector2Int(x + 1, y + 1)) || !CheckForCollider(new Vector2Int(x - 1, y + 1)) || !CheckForCollider(new Vector2Int(x - 1, y + 1)) || !CheckForCollider(new Vector2Int(x - 1, y - 1));
					}
					if (foundCollider && foundNoCollider)
					{
						colors[x + y * (colliderChecks.x + textureBorder * 2)] = Color.white;
						outlinePositions = outlinePositions.Add(new Vector2Int(x, y));
					}
				}
			}
		}
		if (behaviour == Behaviour.Outline)
		{
			for (int x = 0; x < colliderChecks.x + textureBorder * 2; x ++)
			{
				for (int y = 0; y < colliderChecks.y + textureBorder * 2; y ++)
				{
					foreach (Vector2Int outlinePosition in outlinePositions)
					{
						if (Vector2Int.Distance(new Vector2Int(x, y), outlinePosition) <= outlineRadius)
						{
							colors[x + y * (colliderChecks.x + textureBorder * 2)] = Color.white;
							break;
						}
					}
				}
			}
		}
		Texture2D texture = Instantiate(Texture2D.blackTexture);
		texture.Resize(colliderChecks.x + textureBorder * 2, colliderChecks.y + textureBorder * 2);
		texture.SetPixels(colors);
		texture.Apply();
		Sprite sprite = Sprite.Create(texture, Rect.MinMaxRect(0, 0, colliderChecks.x + textureBorder * 2, colliderChecks.y + textureBorder * 2), Vector2.one / 2);
		AssetDatabase.CreateAsset(sprite, assetPath);
		if (boxCollider != null)
			boxCollider.edgeRadius = boxColliderEdgeRadius;
	}

	public virtual bool CheckForCollider (Vector2Int checkPosition)
	{
		checkPosition -= new Vector2Int(textureBorder, textureBorder);
		Vector2 checkPoint = colliderBounds.min + (colliderBounds.size.Multiply(new Vector2((float) checkPosition.x / colliderChecks.x, (float) checkPosition.y / colliderChecks.y)));
		if (boxCollider != null)
			return Vector2.Distance(collider.ClosestPoint(checkPoint), checkPoint) <= boxColliderEdgeRadius;
		else
			return collider.OverlapPoint(checkPoint);
	}

	public enum Behaviour
	{
		FillInside,
		FillOutside,
		Outline
	}
}
#endif