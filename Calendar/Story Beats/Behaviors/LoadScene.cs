using System.Collections;
using System.Collections.Generic;
using Grawly.Chat;
using Grawly.Dungeon;
using Grawly.DungeonCrawler;
using UnityEngine;
using Sirenix.OdinInspector;
using System;


namespace Grawly.Calendar.Behavior.General {
	
	/// <summary>
	/// Loads up a scene.
	/// </summary>
	// [InfoBox("Loads up a scene.")]
	[Title("Load Scene")]
	[System.Serializable]
	[GUIColor(r: 1f, g: 0.9f, b: 0.9f, a: 1f)]
	public class LoadScene : StoryBehavior {

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The kind of transition to use.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private GameTransitionType gameTransitionType = GameTransitionType.CustomTransition;
		/// <summary>
		/// The name of the scene to load for this story beat.
		/// </summary>
		[ShowIf("UseCustomScene")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public string sceneName = "";
		/// <summary>
		/// The overworld type to transition to, if not using a custom scene.
		/// </summary>
		[ShowIf("UseLocationScene")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public LocationType locationType;
		/// <summary>
		/// Don't stop playing the music when performing the load.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		public bool dontStopMusic = false;
		/// <summary>
		/// The camera tag to use as the override, if set to anything other than None.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		public CameraTagType cameraTagOverride = CameraTagType.None;
		#endregion
		
		#region STORYBEAT REACTION
		/// <summary>
		/// Loads up a scene.
		/// </summary>
		/// <returns></returns>
		public override StoryBeatReaction OnStoryBeatLoad() {
			return delegate(StoryBeatReactionSequence sequence) {

				if (this.dontStopMusic == false) {
					// Fade out the music if it was told to do so.
					AudioController.instance.StopMusic(track: 0, fade: 0.5f);
				}
				
				// Define a callback that should basically always be run for any case.
				// This continues the sequence.
				Action sceneLoadedCallback = () => {
					sequence.ExecuteNextReaction();
				};
				
				// Switch based on the transition type, then load accordingly.
				// Remember: The callback above gets run when the scene is loaded.
				switch (this.gameTransitionType) {
					
					case GameTransitionType.CustomTransition:
						// SceneController.instance.BasicLoadScene(sceneName: this.sceneName, onComplete: sceneLoadedCallback);
						SceneController.instance.BasicLoadSceneWithFade(
							sceneName: this.sceneName,
							onComplete: sceneLoadedCallback);
						break;
					
					case GameTransitionType.LocationTransition:
						// SceneController.instance.BasicLoadScene(locationType: this.locationType, onComplete: sceneLoadedCallback);
						SceneController.instance.BasicLoadSceneWithFade(
							locationType: this.locationType, 
							onComplete: sceneLoadedCallback);
						break;
					
					
					default:
						throw new System.Exception("This isn't allowed!");
				}
				
			};
		}
		#endregion
		
		#region ODIN HELPERS
		private bool UseLocationScene() {
			return this.gameTransitionType == GameTransitionType.LocationTransition ;
		}
		private bool UseCustomScene() {
			return this.gameTransitionType == GameTransitionType.CustomTransition;
		}
		private string TargetSceneNameForInspector {
			get {
				if (this.UseCustomScene()) {
					return this.sceneName;
				} else if (this.UseLocationScene()) {
					return this.locationType.ToString();
				} else {
					return "???";
				}
			}
		}
		/// <summary>
		/// The string to use for the foldout groups in the inspector.
		/// </summary>
		protected override string FoldoutGroupTitle {
			get {
				return "Load Scene (" + this.TargetSceneNameForInspector + ")";
			}
		}
		#endregion
		
	}
}