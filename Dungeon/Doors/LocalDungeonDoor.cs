using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Chat;
using Grawly.UI.Legacy;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Grawly.Dungeon.Interactable {
	
	/// <summary>
	/// The script that drives the routine that helps a player maneuiver through doors.
	/// </summary>
	public class LocalDungeonDoor : MonoBehaviour, IDungeonPlayerInteractionHandler {

		#region FIELDS - TOGGLES

		
		#endregion
		
		#region FIELDS - TWEENING : GENERAL
		/// <summary>
		/// The type of transition to use when interacting with this door.
		/// </summary>
		[SerializeField, TabGroup("Door", "Tweening")]
		private DungeonDoorTransitionType transitionType = DungeonDoorTransitionType.Fade;
		#endregion

		#region FIELDS - TWEENING : FADE
		/// <summary>
		/// The amount of time to take when fading out during a fade transition.
		/// </summary>
		[SerializeField, TabGroup("Door", "Tweening"), ShowIf("IsFadeTransition")]
		private float fadeOutTime = 0.5f;
		/// <summary>
		/// The amount of time to wait before fading back in in a fade transition.
		/// </summary>
		[SerializeField, TabGroup("Door", "Tweening"), ShowIf("IsFadeTransition")]
		private float fadeWaitTime = 1f;
		/// <summary>
		/// The amount of time to take when fading in during a fade transition.
		/// </summary>
		[SerializeField, TabGroup("Door", "Tweening"), ShowIf("IsFadeTransition")]
		private float fadeInTime = 0.5f;
		#endregion
		
		#region FIELDS - TWEENING : TWEEN
		/// <summary>
		/// The amount of time to take when tweening the player to the "enter" spot.
		/// </summary>
		[SerializeField, TabGroup("Door", "Tweening"), ShowIf("IsTweenTransition")]
		private float enterSpotTweenTime = 0.2f;
		/// <summary>
		/// The amount of time to take when the door's open animation plays.
		/// </summary>
		[SerializeField, TabGroup("Door", "Tweening"), ShowIf("IsTweenTransition")]
		private float doorOpenWaitTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the player to the "exit" spot.
		/// </summary>
		[SerializeField, TabGroup("Door", "Tweening"), ShowIf("IsTweenTransition")]
		private float exitSpotTweenTime = 0.2f;
		/// <summary>
		/// The amount of time to take when the door's close animation plays.
		/// </summary>
		[SerializeField, TabGroup("Door", "Tweening"), ShowIf("IsTweenTransition")]
		private float doorCloseWaitTime = 0.2f;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The spot where the dungeon player should be tweened to when about to enter the door.
		/// </summary>
		[SerializeField, TabGroup("Door", "Scene References")]
		private Transform doorPoint1;
		/// <summary>
		/// The spot where the dungeon player should be tweened to when exiting the door.
		/// </summary>
		[SerializeField, TabGroup("Door", "Scene References")]
		private Transform doorPoint2;
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// The routine that controls how the player should move through the door when using a tween.
		/// </summary>
		/// <param name="dungeonPlayer"></param>
		/// <param name="entryPoint"></param>
		/// <param name="exitPoint"></param>
		/// <returns></returns>
		private IEnumerator DoorTweenTransitionRoutine(DungeonPlayer dungeonPlayer, Transform entryPoint, Transform exitPoint) {
			
			// Make the dungeon player wait.
			dungeonPlayer.SetFSMState(DungeonPlayerStateType.Wait);

			// Wait a hot minute.
			yield return new WaitForSeconds(0.1f);
			
			// Begin tweening the player.
			dungeonPlayer.Relocate(pos: entryPoint, tweenTime: this.enterSpotTweenTime);
			yield return new WaitForSeconds(this.enterSpotTweenTime);
			
			// OPEN THE DOOR
			Debug.LogError("OPEN THE DOOR");
			yield return new WaitForSeconds(this.doorOpenWaitTime);
			
			// Tween the player to the exit spot.
			dungeonPlayer.Relocate(pos: exitPoint, tweenTime: this.exitSpotTweenTime);
			yield return new WaitForSeconds(this.exitSpotTweenTime);
			
			// CLOSE THE DOOR
			Debug.LogError("CLOSE THE DOOR");
			yield return new WaitForSeconds(this.doorCloseWaitTime);

			// Give control back to the player.
			dungeonPlayer.SetFSMState(DungeonPlayerStateType.Free);
			
		}
		/// <summary>
		/// The routine that controls how the player should move through the door when using a fade.
		/// </summary>
		/// <param name="dungeonPlayer"></param>
		/// <param name="exitPoint"></param>
		/// <returns></returns>
		private IEnumerator DoorFadeTransitionRoutine(DungeonPlayer dungeonPlayer, Transform exitPoint) {
			
			// Make the dungeon player wait.
			dungeonPlayer.SetFSMState(DungeonPlayerStateType.Wait);

			// Wait a hot minute.
			yield return new WaitForSeconds(0.1f);
			
			// Fade out.
			Flasher.FadeOut(fadeTime: this.fadeOutTime);
			yield return new WaitForSeconds(this.fadeOutTime + 0.1f);
			
			// Snap the player to the new position.
			dungeonPlayer.Relocate(pos: exitPoint);
			
			// Wait a little bit.
			yield return new WaitForSeconds(this.fadeWaitTime);

			// Fade back in.
			Flasher.instance.FadeIn(fadeTime: this.fadeInTime);
			yield return new WaitForSeconds(this.fadeInTime + 0.1f);
			
			// Give control back to the player.
			dungeonPlayer.SetFSMState(DungeonPlayerStateType.Free);
			
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IDUNGEONPALYERINTERACTIONHANDLER
		public void OnPlayerInteract() {

			// Figure out which transforms to use for the entry/exit points.
			Transform entryPoint = this.GetEntrySpot(dungeonPlayer: DungeonPlayer.Instance, spot1: this.doorPoint1, spot2: this.doorPoint2);
			Transform exitPoint = this.GetExitSpot(dungeonPlayer: DungeonPlayer.Instance, spot1: this.doorPoint1, spot2: this.doorPoint2);
			
			// Branch based on what kind of transition to use.
			switch (this.transitionType) {
				case DungeonDoorTransitionType.Tween:
					this.StartCoroutine(this.DoorTweenTransitionRoutine(
						dungeonPlayer: DungeonPlayer.Instance, 
						entryPoint: entryPoint, 
						exitPoint: exitPoint));
					break;
				case DungeonDoorTransitionType.Fade:
					this.StartCoroutine(this.DoorFadeTransitionRoutine(
						dungeonPlayer: DungeonPlayer.Instance, 
						exitPoint: exitPoint));
					break;
				default:
					throw new System.Exception("This case is not defined!");
			}
			
			
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Gets the point that the player should use as an entry point when interacting with this door.
		/// </summary>
		/// <param name="dungeonPlayer"></param>
		/// <param name="spot1"></param>
		/// <param name="spot2"></param>
		/// <returns></returns>
		private Transform GetEntrySpot(DungeonPlayer dungeonPlayer, Transform spot1, Transform spot2) {
			
			// Get the distances of the player from the two different spots.
			float spot1Distance = (dungeonPlayer.transform.position - spot1.position).magnitude;
			float spot2Distance = (dungeonPlayer.transform.position - spot2.position).magnitude;

			// If spot1's distance is shorter, that means its the entry point.
			if (spot1Distance < spot2Distance) {
				return spot1;
			} else if (spot2Distance < spot1Distance) {
				return spot2;
			} else {
				throw new System.Exception("This is probably reached if the distance between the two spots and" 
				                           + "the player are both equal. Refactor this function if its ever reached..");
			}

		}
		/// <summary>
		/// Gets the point that the player should transition to when interacting with this door.
		/// </summary>
		/// <param name="dungeonPlayer"></param>
		/// <param name="spot1"></param>
		/// <param name="spot2"></param>
		/// <returns></returns>
		private Transform GetExitSpot(DungeonPlayer dungeonPlayer, Transform spot1, Transform spot2) {
			// Get the distances of the player from the two different spots.
			float spot1Distance = (dungeonPlayer.transform.position - spot1.position).magnitude;
			float spot2Distance = (dungeonPlayer.transform.position - spot2.position).magnitude;

			// If spot1's distance is shorter, that means its the entry point.
			if (spot1Distance > spot2Distance) {
				return spot1;
			} else if (spot2Distance > spot1Distance) {
				return spot2;
			} else {
				throw new System.Exception("This is probably reached if the distance between the two spots and" 
				                           + "the player are both equal. Refactor this function if its ever reached..");
			}
		}
		#endregion
		
		#region ODIN HELPERS
		private bool IsFadeTransition() {
			return this.transitionType == DungeonDoorTransitionType.Fade;
		}
		private bool IsTweenTransition() {
			return this.transitionType == DungeonDoorTransitionType.Tween;
		}
		#endregion
		
	}
}