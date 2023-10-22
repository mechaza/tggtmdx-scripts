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
using Grawly.Toggles;
using CameraTransitions;
using Grawly.Battle;
using Cinemachine;
using Grawly.Calendar;
using UnityEngine.Playables;

namespace Grawly.Cutscenes {
    
	/// <summary>
	/// A script to help me manage controls regarding more in depth cutscenes that can't be done with the ChatController.
	/// Note that there's an assumption that there will be only one PlayableDirector/Timeline per scene that this component is inside of.
	/// I may make an expanded version of this script that can manage multiple PlayableDirectors but for the time being, its just one.
	/// </summary>
	[RequireComponent(typeof(PlayableDirector))]
	public class CutsceneController : MonoBehaviour {
		
		public static CutsceneController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The callback to invoke when the cutscene has been completed.
		/// </summary>
		private Action OnCutsceneComplete { get; set; } = null;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The PlayableDirector that will be used to initiate the cutscene this script will also manage alongside.
		/// </summary>
		private PlayableDirector PlayableDirector { get; set; }
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
			this.PlayableDirector = this.GetComponent<PlayableDirector>();
		}
		#endregion

		#region CUTSCENE PLAYBACK
		/// <summary>
		/// Plays the cutscene.
		/// </summary>
		public void PlayCutscene(Action onCutsceneComplete = null) {
			// Save the callback that should be invoked when the cutscene is done.
			this.OnCutsceneComplete = onCutsceneComplete;
			// Tell the director to play the cutscene.
			this.PlayableDirector.Play();
		}
		#endregion

		#region CUTSCENE EVENTS
		/// <summary>
		/// Gets called to signal that the cutscene is complete and figures out where to go from here.
		/// </summary>
		public void CutsceneComplete() {
			// Invoke the OnCutsceneComplete callback, if one was provided.
			this.OnCutsceneComplete?.Invoke();
		}
		#endregion
		
	}

    
}
