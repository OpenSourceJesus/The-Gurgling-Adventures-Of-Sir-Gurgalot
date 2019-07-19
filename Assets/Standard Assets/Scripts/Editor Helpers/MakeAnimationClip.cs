using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MakeAnimationClip : MonoBehaviour
{
	#if UNITY_EDITOR
	public bool update;
	public AnimationClip clip;
	public Sprite[] sprites;
	public float frameRate;
	List<AnimationEvent> events = new List<AnimationEvent>();
	AnimationEvent newEvent;
	
	void Update ()
	{
		if (!update)
			return;
		update = false;
		events.Clear();
		for (int i = 0; i < sprites.Length; i ++)
		{
			newEvent = new AnimationEvent();
			newEvent.time = (float) i * (1f / frameRate);
			newEvent.functionName = "SetSprite";
			newEvent.objectReferenceParameter = sprites[i];
			events.Add(newEvent);
		}
		AnimationUtility.SetAnimationEvents(clip, events.ToArray());
	}
	#endif
}
