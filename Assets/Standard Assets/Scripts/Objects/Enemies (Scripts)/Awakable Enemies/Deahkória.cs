using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
	public class Deahkória : AwakableEnemy
	{
		[MakeConfigurable]
		public float shootRate;
		public Bomb bombPrefab;
		float shootTimer;
		public SpriteRenderer spriteRenderer;
		[MakeConfigurable]
		public float[] maxShootDestinationOffsets;
		
		public override void Update ()
		{
			base.Update ();
			if (!awakened || Time.timeScale == 0)
				return;
			spriteRenderer.flipX = Player.instance.trs.position.x > trs.position.x;
			if (shootTimer > 0)
				shootTimer -= Time.deltaTime;
			if (shootTimer <= 0)
			{
				shootTimer += shootRate;
				Bomb bomb;
				Vector2 toDestination;
				foreach (float maxShootDestinationOffset in maxShootDestinationOffsets)
				{
					bomb = ObjectPool.instance.Spawn(bombPrefab, trs.position, Quaternion.identity);
					toDestination = (Vector2) Player.instance.trs.position - (Vector2) trs.position;
					toDestination = Vector2.ClampMagnitude(toDestination + (Random.insideUnitCircle * maxShootDestinationOffset), bomb.range);
					bomb.destination = (Vector2) trs.position + toDestination;
				}
			}
		}
	}
}