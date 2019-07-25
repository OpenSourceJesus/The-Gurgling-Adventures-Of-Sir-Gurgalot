using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGAOSG
{
	public class DestroyAfterAudioFinishes : MonoBehaviour
	{
		public new AudioSource audio;
		bool hasStarted;
		public Transform trs;
		
		void Update ()
		{
			if (hasStarted && !audio.isPlaying)
				Destroy(gameObject);
			else
				hasStarted = audio.isPlaying;
		}
	}
}