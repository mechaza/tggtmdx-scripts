using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly {
	
	/// <summary>
	/// Automatically plays music when the scene loads
	/// and provides controls to tweak when and how it does, if necessary.
	/// </summary>
	public class AutoPlayMusic : MonoBehaviour {

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// Should a custom music file be used?
		/// If false, the controls for MusicType will appear.
		/// </summary>
		[SerializeField]
		private bool useCustomMusic = true;
		#endregion
		
		#region FIELDS - TOGGLES : FILES
		/// <summary>
		/// The music to play 
		/// </summary>
		[SerializeField, ShowIf("useCustomMusic")]
		private IntroloopAudio musicFile;
		/// <summary>
		/// The music type to play if customMusic is set to off.
		/// </summary>
		[SerializeField, HideIf("useCustomMusic")]
		private MusicType musicType = MusicType.Dungeon;
		#endregion
		
		#region UNITY CALLS
		private void Start() {
			
			// If music should not be auto played as signaled by a global flag, return.
			if (GlobalFlagController.Instance.GetFlag(GlobalFlagType.SuppressAutoPlayMusic) == true) {
				Debug.Log("Suppressing auto play music.");
				return;
			}
			
			// Figure out which music to play and then do so.
			if (this.useCustomMusic == true) {
				Debug.Log("Auto playing music from file: " + this.musicFile.name);
				AudioController.instance.PlayMusic(audio: this.musicFile);
			} else {
				Debug.Log("Auto playing music from type: " + musicType.ToString());
				AudioController.instance.PlayMusic(type: this.musicType);
			}
			
		}
		#endregion
		
	}

	
}