using UnityEngine;

public interface IDestructable
{
	uint MaxHp {get; set;}
	float Hp {get; set;}
	Collider2D BleedBounds {get;}
	bool BleedBoundsIsCircle {get;}
	
	void TakeDamage (float amount);
	void Bleed (Vector2 pos, Vector2 facing);
	void Death ();
}