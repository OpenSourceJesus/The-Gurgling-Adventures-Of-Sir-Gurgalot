using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TAoKR
{
	[CreateAssetMenu]
	public class InstantiateObjectTile : Tile
	{
		public override bool StartUp (Vector3Int location, ITilemap tilemap, GameObject go)
		{
			if (go != null)
				go.transform.position = location + new Vector3Int(1, 1, 0);
			return true;
		}
	}
}