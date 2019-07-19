using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graphs
{
	[ExecuteInEditMode]
	public class Node : MonoBehaviour
	{
		[Multiline]
		public string title;
		public Color color;
		public Color textColor;
		public Node connectTo;
		Node previousConnectedTo;
		public Transform trs;
		public Text text;
		public Image image;
		public NodeConnection connectionPrefab;
		public NodeConnection[] connections;
		bool traversed;
		public bool Traversed
		{
			get
			{
				return traversed;
			}
			private set
			{
				traversed = value;
			}
		}
		
		public virtual void Start ()
		{
			connectTo = null;
		}
		
		public virtual void Update ()
		{
			if (connectTo != null && previousConnectedTo == null)
			{
				NodeConnection connection = Instantiate(connectionPrefab);
				connection.start = this;
				connection.end = connectTo;
				connection.Update ();
				connectTo = null;
			}
			connections = GetComponentsInChildren<NodeConnection>();
			image.color = color;
			text.text = title;
			text.color = textColor;
			name = title;
			previousConnectedTo = connectTo;
		}
		
		public virtual void Traverse ()
		{
			Traversed = true;
		}
	}
}
