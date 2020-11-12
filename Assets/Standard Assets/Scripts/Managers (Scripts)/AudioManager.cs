using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;

namespace TAoKR
{
	public class AudioManager : SingletonMonoBehaviour<AudioManager>
	{
		public float Volume
		{
			get
			{
				return PlayerPrefs.GetFloat("Volume", 1);
			}
			set
			{
				AudioListener.volume = value;
				PlayerPrefsExtensions.SetFloat("Volume", value);
			}
		}
		public bool Mute
		{
			get
			{
				return PlayerPrefsExtensions.GetBool("Mute", false);
			}
			set
			{
				AudioListener.pause = value;
				PlayerPrefsExtensions.SetBool("Mute", value);
			}
		}
		public SoundEffect soundEffectPrefab;
		public AudioClip[] deathSounds;
		public AudioClip[] deathResponses;

		public override void Start ()
		{
			base.Start ();
			AudioListener.pause = Mute;
			AudioListener.volume = Volume;
		}
		
		public virtual SoundEffect MakeSoundEffect (SoundEffect.Settings soundEffectSettings)
		{
			SoundEffect output = Instantiate(soundEffectPrefab, soundEffectSettings.Position, soundEffectSettings.Rotation);
			output.settings = soundEffectSettings;
			output.Play();
			return output;
		}
		
		public SoundEffect MakeSoundEffectIfInRange (SoundEffect.Settings soundEffectSettings)
		{
			SoundEffect output = null;
			if (Vector2.Distance(Player.instance.trs.position, soundEffectSettings.Position) <= soundEffectSettings.MaxDistance)
				output = MakeSoundEffect (soundEffectSettings);
			return output;
		}
	}
}