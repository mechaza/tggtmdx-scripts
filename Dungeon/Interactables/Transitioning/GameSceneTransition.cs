using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Calendar;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Grawly.Dungeon.Interactable {

	/// <summary>
	/// Serves as a hard script to attach to a collider to allow the player to transition scenes.
	/// </summary>
	public class GameSceneTransition : MonoBehaviour, IDungeonPlayerInteractionHandler {

		#region FIELDS - GENERAL
		/// <summary>
		/// Should this transition advance time?
		/// This effectively probes the CalendarController for the next StoryBeat.
		/// </summary>
		[SerializeField, Title("General")]
		[PropertyTooltip("Should this transition advance time? This effectively probes the CalendarController for the next StoryBeat.")]
		private bool advanceTime = false;
		/// <summary>
		/// Should the music be fading out upon transitioning out?
		/// </summary>
		[SerializeField, HideIf("advanceTime")]
		[PropertyTooltip("Should the music be fading out upon transitioning out?")]
		private bool dontStopMusicOnTransition = false;
		#endregion

		#region FIELDS - DESTINATION : DUNGEON
		/// <summary>
		/// The type of transition.
		/// LocationTransition: Probes the CalendarController for the scene name associated with the given location.
		/// CustomTransition: An explicit scene name to manually load.
		/// </summary>
		[SerializeField, Title("Destination"), HideIf("advanceTime")]
		[PropertyTooltip("The type of transition to make." 
		                 + "\n \nLocationTransition: Probes the CalendarController for the scene name associated with the given location."
		                 + "\n \nCustomTransition: An explicit scene name to manually load.")]
		private GameTransitionType transitionType = GameTransitionType.LocationTransition;
		/// <summary>
		/// The destination scene to transition to, if not dynamic.
		/// </summary>
		[SerializeField, ShowIf("IsCustomTransition")]
		[PropertyTooltip("The scene to transition to when using a custom transition.")]
		private string sceneName = "";
		/// <summary>
		/// The destination overworld type, if dynamic.
		/// </summary>
		[SerializeField, ShowIf("IsTypedTransition")]
		[PropertyTooltip("The location type to transition to if using a location transition.")]
		private LocationType locationType = LocationType.None;
		/// <summary>
		/// The location to spawn at when the scene loads up.
		/// Default will not relocate the player on load.
		/// </summary>
		[SerializeField, HideIf("advanceTime")]
		[PropertyTooltip("The location to spawn at when the scene loads up. Default will not relocate the player on load.")]
		private DungeonSpawnType destinationSpawnType = DungeonSpawnType.Default;
		#endregion
		
		#region INTERFACE - IDUNGEONPLAYERINTERACTIONHANDLER
		/// <summary>
		/// Gets called when the player does try to interact with this object.
		/// </summary>
		public void OnPlayerInteract() {
			
			// Make the player wait.
			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);

			// If advancing time, go to the next story beat.
			if (this.advanceTime == true) {
				CalendarController.Instance.GoToNextStoryBeat();
				
			} else {
				// If not advancing time, figure out how to transition.
				if (this.transitionType == GameTransitionType.CustomTransition) {
					SceneController.instance.LoadScene(
						sceneName: this.sceneName, 
						spawnPositionType: this.destinationSpawnType, 
						dontStopMusic: this.dontStopMusicOnTransition);
					
				} else if (this.transitionType == GameTransitionType.LocationTransition) {
					SceneController.instance.LoadScene(
						dayNumber: CalendarController.Instance.CurrentDay.EpochDay,
						locationType: this.locationType,
						timeOfDay: CalendarController.Instance.CurrentTimeOfDay,
						spawnPositionType: this.destinationSpawnType, 
						dontStopMusic: this.dontStopMusicOnTransition);
					
				} else {
					throw new System.Exception("Couldn't determine the destination scene name!");
				}
			}
		}
		#endregion
		
		#region ODIN HELPERS
		private bool IsCustomTransition() {
			return this.transitionType == GameTransitionType.CustomTransition 
			       && this.advanceTime == false;

		}
		private bool IsTypedTransition() {
			return this.transitionType == GameTransitionType.LocationTransition 
			       && this.advanceTime == false;
		}
		#endregion
		
	}


}