using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
	public class UnlockableNode : Node
	{
		public int unlockValue;
		int currentValue;
		public int CurrentValue
		{
			get
			{
				return currentValue;
			}
			set
			{
				currentValue = value;
				if (currentValue >= unlockValue)
					Unlock ();
			}
		}
		
		public virtual void Unlock ()
		{
		}
		
		public override void Traverse ()
		{
			base.Traverse();
			foreach (NodeConnection connection in connections)
			{
				UnlockableNode endNode = connection.end as UnlockableNode;
				if (endNode != null)
					endNode.CurrentValue += connection.weight;
			}
		}
	}
}