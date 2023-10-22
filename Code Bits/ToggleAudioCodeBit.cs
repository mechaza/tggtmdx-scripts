using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.CodeBits {

	/// <summary>
	/// Provides a few sets of parameters that allow one to toggle the audio being played currently.
	/// </summary>
	[System.Serializable]
	public class ToggleAudioCodeBit : CodeBit {

		#region FIELDS
		/// <summary>
		/// Should this code bit start playing the audio, or stop it?
		/// </summary>
		[SerializeField]
		private bool playMusic = true;

		/// <summary>
		/// The type of music to play, if playing.
		/// </summary>
		[ShowIf("PlayMusic"), SerializeField]
		private MusicType musicType = MusicType.None;
		/// <summary>
		/// The music to play, if playing.
		/// </summary>
		[ShowIf("PlayMusic"), SerializeField]
		private IntroloopAudio musicToPlay;
		#endregion



		public override void Run() {
			Debug.Log("AUDIO CODE BIT.");
			// Tell the audio controller to play the music if its specified, or not to play anything if its told not to.
			if (this.playMusic == true) {
				Debug.Log("CODE BIT IS PLAYING SONG ON TRACK 0");
				AudioController.instance.PlayMusic(audio: this.musicToPlay, track: 0, fade: 0f, delay: 0f);
			} else {
				Debug.Log("CODE BIT IS STOPPING SONG ON TRACK 0");
				AudioController.instance.StopMusic(track: 0, fade: 0f);
			}
		}

		#region ODIN FUNCTIONS
		private bool PlayMusic() {
			return this.playMusic;
		}
		#endregion

	}
}