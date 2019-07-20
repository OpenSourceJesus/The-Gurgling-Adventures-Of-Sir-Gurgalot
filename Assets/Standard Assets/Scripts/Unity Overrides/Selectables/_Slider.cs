using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired.Integration.UnityUI;
using UnityEngine.EventSystems;
using Extensions;

[RequireComponent(typeof(Slider))]
public class _Slider : _Selectable
{
	public Slider slider;
	public Text displayValue;
	string initDisplayValue;
	public float[] snapValues;
	[HideInInspector]
	public int indexOfCurrentSnapValue;
	public RectTransform slidingArea;
	
	public virtual void Awake ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (displayValue != null)
			initDisplayValue = displayValue.text;
		if (snapValues.Length > 0)
		{
			indexOfCurrentSnapValue = MathfExtensions.GetIndexOfClosestNumber(slider.value, snapValues);
			slider.value = snapValues[indexOfCurrentSnapValue];
		}
		SetDisplayValue ();
		slider.onValueChanged.AddListener(OnValueChanged);
	}

#if UNITY_EDITOR
	public override void OnEnable ()
	{
		if (!Application.isPlaying)
		{
			if (rectTrs == null)
				rectTrs = GetComponent<RectTransform>().Find("Handle") as RectTransform;
			if (slidingArea == null)
				slidingArea = GetComponent<RectTransform>().Find("Handle Slide Area") as RectTransform;
			return;
		}
		base.OnEnable ();
	}
#endif
	
#if UNITY_EDITOR
	public override void Update ()
	{
		base.Update ();
		if (!Application.isPlaying)
			return;
#else
	public virtual void Update ()
	{
#endif
		if (snapValues.Length > 0)
			slider.value = MathfExtensions.GetClosestNumber(slider.value, snapValues);
	}
	
	public virtual void SetDisplayValue ()
	{
		displayValue.text = initDisplayValue + slider.value;
	}

	public virtual void ChangeValue (float amount)
	{
		slider.value += amount * Time.unscaledDeltaTime;
	}

	public virtual void ChangeValue (int direction)
	{
		indexOfCurrentSnapValue = Mathf.Clamp(indexOfCurrentSnapValue + direction, 0, snapValues.Length - 1);
		slider.value = snapValues[indexOfCurrentSnapValue];
	}

	public virtual void OnValueChanged (float value)
	{
		SetDisplayValue ();
	}
}