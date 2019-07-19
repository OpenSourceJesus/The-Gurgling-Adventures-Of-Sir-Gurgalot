using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DisableGraphicsIfInEditMode : MonoBehaviour
{
	public bool startEnabled;
	public bool useRenderer;
	public new Renderer renderer;
	public MaskableGraphic graphic;
	
	public virtual void Start ()
	{
		if (Application.isPlaying)
		{
			if (useRenderer)
				renderer.enabled = startEnabled;
			else
				graphic.enabled = startEnabled;
			DestroyImmediate(this);
		}
		else
		{
			renderer = GetComponent<Renderer>();
			graphic = GetComponent<MaskableGraphic>();
		}
	}
	
	public virtual void Update ()
	{
		if (!Application.isPlaying)
		{
			if (useRenderer)
				renderer.enabled = false;
			else
				graphic.enabled = false;
		}
	}
}