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
	public class TargetDungeonDoor : MonoBehaviour, IDungeonPlayerInteractionHandler {
		
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

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The spot where the dungeon player should be tweened to when transitioning to this door from another one.
		/// </summary>
		[SerializeField, TabGroup("Door", "Scene References")]
		private Transform exitPoint;
		/// <summary>
		/// The door the dungeon player should relocate to upon interacting with this door.
		/// </summary>
		[SerializeField, TabGroup("Door", "Scene References")]
		private TargetDungeonDoor targetDungeonDoor;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The spot where the dungeon player should be tweened to when transitioning to this door from another one.
		/// </summary>
		public Transform ExitPoint => this.exitPoint;
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Plays the transition for when the player opens the door.
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
			// When the player interacts with this door, 
			// play the routine that relocates them to the target.
			this.StartCoroutine(this.DoorFadeTransitionRoutine(
				dungeonPlayer: DungeonPlayer.Instance, 
				exitPoint: this.targetDungeonDoor.ExitPoint));
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