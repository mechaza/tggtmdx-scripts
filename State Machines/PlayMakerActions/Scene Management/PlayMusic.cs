using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace Grawly.PlayMakerActions {

	[ActionCategory("Grawly - Scene"), Tooltip("Tells the AudioController to play the given music.")]
	public class PlayMusic : FsmStateAction {

		#region FIELDS
		/// <summary>
		/// The audio to play on the AudioController.
		/// </summary>
		[ObjectType(typeof(IntroloopAudio)), Tooltip("The introloop audio to play. Whoa.")]
		public FsmObject audio;
		/// <summary>
		/// The 'track' to play the audio on.
		/// </summary>
		[Tooltip("The 'track' to play the audio on.")]
		public FsmInt trackNumber;
		[Tooltip("The amount of time to fade in the track by.")]
		public FsmFloat fadeTime;
		[Tooltip("The volume the track should be played at")]
		public FsmFloat delayAmount;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			
			// If music should not be auto played as signaled by a global flag, return.
			if (GlobalFlagController.Instance.GetFlag(GlobalFlagType.SuppressAutoPlayMusic) == true) {
				UnityEngine.Debug.Log("Suppressing auto play music.");
				base.Finish();
				return;
			}
			
			AudioController.instance.PlayMusic(
				audio: ((IntroloopAudio)this.audio.Value),
				track: this.trackNumber.Value,
				fade: this.fadeTime.Value,
				delay: this.delayAmount.Value);
			base.Finish();
		}
		#endregion

	}


}