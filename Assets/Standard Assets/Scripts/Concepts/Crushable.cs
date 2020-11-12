using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Crushable : MonoBehaviour
{
	List<Vector2> contactNormals = new List<Vector2>();
	public IDestructable destructable;
	
	public virtual void Start ()
	{
		destructable = GetComponent<IDestructable>();
	}
	
	public virtual void OnCollisionStay2D (Collision2D coll)
	{
		foreach (ContactPoint2D contact in coll.contacts)
		{
			if (contactNormals.Contains(-contact.normal))
			{
				destructable.Hp.value = 0;
				destructable.Death ();
			}
			else
				contactNormals.Add(contact.normal);
		}
	}

	public virtual void Update ()
	{
		contactNormals.Clear ();
	}
}