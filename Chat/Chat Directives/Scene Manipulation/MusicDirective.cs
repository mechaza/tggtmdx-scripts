using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using Grawly.Dungeon;

namespace Grawly.Chat {

	/// <summary>
	/// Makes contact with the Camera Controller to toggle any given camera on/off for a cutscene.
	/// </summary>
	[Title("Music")]
	[GUIColor(r: 0.8f, g: 0.8f, b: 1f, a: 1f)]
	public class MusicDirective : ChatDirective {

		#region FIELDS
		/// <summary>
		/// The toggle which helps signal if this directive should play music or stop it entirely.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		[InfoBox(message: "Stopping music is currently not implemented!", infoMessageType: InfoMessageType.Error, visibleIfMemberName: "DirectiveWillStopMusic")]
		public MusicDirectivePlaybackType musicPlaybackType = MusicDirectivePlaybackType.Play;
		/// <summary>
		/// The track in which to play this music, if actually using it.
		/// </summary>
		[ShowIf("DirectiveWillPlayMusic")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public int track = 0;
		/// <summary>
		/// The amount of time to fade when... uh... I DONT KNOW 
		/// </summary>
		[ShowIf("DirectiveWillPlayMusic")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public float fadeTime = 0.5f;
		/// <summary>
		/// Use the music type to find the track?
		/// </summary>
		[ShowIf("DirectiveWillPlayMusic")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public bool useMusicType = true;
		/// <summary>
		/// The type of music to use, if an actual type is meant to be set.
		/// </summary>
		[ShowIf("UseMusicTypeForPlayback")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public MusicType musicType;
		/// <summary>
		/// The clip to play.
		/// </summary>
		[ShowIf("UseAudiossetForPlayback")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public IntroloopAudio musicClip;
		#endregion

		#region PROPERTIES
		private IntroloopAudio MusicClip {
			get {
				// If using the music type, acccess it form the data controller. otherwis,e uh
				return this.useMusicType == true
					? DataController.Instance.GetMusic(musicType)
					: this.musicClip;
			}
		}
		#endregion

		#region CONSTRUCTORS
		public MusicDirective() {

		}
		public MusicDirective(ChatDirectiveParams directiveParams) {

			// this.musicType = directiveParams.GetEnum<MusicType>(key: "type");
			this.musicType = directiveParams.GetEnum<MusicType>(key: directiveParams.FirstLabel);
			// When creating a music directive from params, useMusicType is always true.
			this.useMusicType = true;
			
			/*// If the first label is STOP, its a special keyword to set the playbcack type.
			if (directiveParams.FirstLabel.ToLower() == "stop") {
				this.musicPlaybackType = MusicDirectivePlaybackType.Stop;
			} else {
				// this.musicType = directiveParams.GetEnum<MusicType>(key: "type");
				this.musicType = directiveParams.GetEnum<MusicType>(key: directiveParams.FirstLabel);
				// When creating a music directive from params, useMusicType is always true.
				this.useMusicType = true;
			}*/
			
			
			
		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			switch (this.musicPlaybackType) {
				case MusicDirectivePlaybackType.Stop:
					// AudioController.instance?.StopAllMusic(fade: 0f);
					throw new NotImplementedException("THIS SHOULDNT BE REACHED YET.");
					chatController.EvaluateNextDirective();
					break;
				case MusicDirectivePlaybackType.Play:
					AudioController.instance?.PlayMusic(audio: this.MusicClip, track: this.track, fade: this.fadeTime, delay: 0f);
					chatController.EvaluateNextDirective();
					break;
				default:
					throw new System.Exception("THIS IS NOT DEFINED");
			}
			// AudioController.instance?.PlayMusic(audio: this.MusicClip, track: this.track, fade: this.fadeTime, delay: 0f);
			// chatController.EvaluateNextDirective();
		}
		#endregion

		
		#region ODIN HELPERS
		protected override string FoldoutGroupTitle {
			get {

				string foldoutStr = "Music - ";
				if (this.musicPlaybackType == MusicDirectivePlaybackType.Stop) {
					foldoutStr += "[[[STOP]]]";
				} else if (this.UseMusicTypeForPlayback()) {
					foldoutStr += "[" + this.musicType.ToString() + "]";
				} else if (this.UseAudiossetForPlayback()) {
					foldoutStr += "[" + this.musicClip.name + "]";
				}

				return foldoutStr;
				
				return this.GetType().FullName;
			}
		}
		protected bool DirectiveWillPlayMusic() {
			return this.musicPlaybackType == MusicDirectivePlaybackType.Play;
		}
		protected bool DirectiveWillStopMusic() {
			return this.musicPlaybackType != MusicDirectivePlaybackType.Play;
		}
		protected bool UseMusicTypeForPlayback() {
			return this.DirectiveWillPlayMusic() && (this.useMusicType == true);
		}
		protected bool UseAudiossetForPlayback() {
			return this.DirectiveWillPlayMusic() && (this.useMusicType == false);
		}
		#endregion

		#region ENUM DEFINITIONS
		/// <summary>
		/// The type which helps define if the music directive should begin music playback or stop it.
		/// </summary>
		public enum MusicDirectivePlaybackType {
			Stop		= 0,
			Play		= 1,
		}
		#endregion
		
	}


}