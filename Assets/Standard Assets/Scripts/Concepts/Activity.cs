using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public enum Activity
	{
		Idle,
		Walking,
		Jumping,
		Falling,
		Attacking
	}
	
	public class ActivityStatus
	{
		public ActivityState state;
		public bool canDo;
		
		public ActivityStatus (ActivityState state, bool canDo)
		{
			this.state = state;
			this.canDo = canDo;
		}
	}
	
	public enum ActivityState
	{
		Doing,
		NotDoing,
		IntendToDo
	}
}