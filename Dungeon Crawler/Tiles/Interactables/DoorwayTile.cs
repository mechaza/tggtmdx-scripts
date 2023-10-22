using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.Legacy;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// A tile that allows the CrawlerPlayer to go through a door.
	/// </summary>
	[DisallowMultipleComponent]
	public class DoorwayTile : CrawlerFloorTile, IPlayerInteractHandler, IPlayerApproachHandler, IPlayerLookAwayHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The text to display when standing in front of the doorway.
		/// </summary>
		[SerializeField, TabGroup("Doorway","Toggles")]
		private string promptText = "";
		/// <summary>
		/// The type of destination this doorway should use.
		/// If set to warp, a dedicated position will need to be specified.
		/// </summary>
		[SerializeField, TabGroup("Doorway","Toggles")]
		private DoorwayTileDestinationType destinationType = DoorwayTileDestinationType.Forward;
		/// <summary>
		/// If using a warp destination, this is the spot the crawler player should be warped to.
		/// </summary>
		[SerializeField, TabGroup("Doorway", "Toggles"), ShowIf("IsWarpDoorway")]
		private Transform warpPointTransform;
		#endregion

		#region FIELDS - TIMING
		/// <summary>
		/// The amount of time to take when fading out after entering the doorway.
		/// </summary>
		[SerializeField, TabGroup("Doorway", "Timing")]
		private float fadeOutTime = 0.5f;
		/// <summary>
		/// The amount of time to wait before fading back in.
		/// </summary>
		[SerializeField, TabGroup("Doorway", "Timing")]
		private float waitTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading in after going through the doorway.
		/// </summary>
		[SerializeField, TabGroup("Doorway", "Timing")]
		private float fadeInTime = 0.5f;
		#endregion
		
		#region INTERFACE IMPLEMENTATION
		public void OnApproach(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			// Upon approach, display the prompt text.
			CrawlerActionPrompt.Instance?.Display(promptText: this.promptText);
		}
		public void OnLookAway() {
			// Dismiss the prompt. This should work regardless of it was shown or not.
			CrawlerActionPrompt.Instance?.Dismiss();
		}
		public void OnInteract(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			// Dismiss the prompt.
			CrawlerActionPrompt.Instance?.Dismiss();

			// Calculate the target position and run the routine that enters the doorway.
			Vector3 targetPosition = this.GetTargetPosition(crawlerPlayer: CrawlerPlayer.Instance);
			this.EnterDoorway(crawlerPlayer: CrawlerPlayer.Instance, targetPosition: targetPosition);

		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets the position that the crawler player should end up on when walking through the door.
		/// </summary>
		/// <param name="crawlerPlayer"></param>
		/// <returns></returns>
		private Vector3 GetTargetPosition(CrawlerPlayer crawlerPlayer) {
			
			// This should only be called if the target position needs to be calculated.
			Debug.Assert(this.destinationType == DoorwayTileDestinationType.Forward);
			
			// Get the direction the crawler player is facing and use the tile size to calculate the target offset.
			Vector3 currentForwardDir = crawlerPlayer.transform.forward;
			int currentTileSize = crawlerPlayer.TileSize;
			Vector3 targetOffset = currentForwardDir * currentTileSize * 2;
			
			// Get the current player position and then add the offset.
			Vector3 currentPosition = crawlerPlayer.transform.position;
			Vector3 targetPosition = currentPosition + targetOffset;

			// Return this target position.
			return targetPosition;
			
		}
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// The actual function to run when entering a doorway.
		/// </summary>
		/// <param name="targetPosition"></param>
		private void EnterDoorway(CrawlerPlayer crawlerPlayer, Vector3 targetPosition) {
			
			// Make the crawler player wait.
			crawlerPlayer.SetState(state: CrawlerPlayerState.Wait);
			// Fade out.
			Flasher.instance.FadeOut(color: Color.black, fadeTime: this.fadeOutTime);
			
			// Wait a moment, then teleport the player and fade in.
			GameController.Instance.WaitThenRun(timeToWait: this.waitTime, () => {
				crawlerPlayer.Teleport(targetPos: targetPosition, instantaneous: true);
				Flasher.instance.FadeIn(fadeTime: this.fadeInTime);
			});
			
			// Wait a little longer, then free the player.
			GameController.Instance.WaitThenRun(timeToWait: this.waitTime + this.fadeInTime, () => {
				// Set the player to free. If any stepped tiles need to freeze the player again, this SHOULD still be ok?
				// I hope.
				crawlerPlayer.SetState(state: CrawlerPlayerState.Free);
				crawlerPlayer.GetSteppedTiles()
					.ForEach(t => t.OnLand(
						crawlerProgressionSet: CrawlerController.Instance.CurrentProgressionSet, 
						floorNumber: CrawlerController.Instance.CurrentFloorNumber));
			});
			
		}
		#endregion
		
		#region ODIN HELPERS
		private bool IsWarpDoorway() {
			return this.destinationType == DoorwayTileDestinationType.Warp;
		}
		#endregion
		
		#region ENUMS
		/// <summary>
		/// The different ways a doorway can move the player to a target destination.
		/// Most of the time, it will be forward, since that allows the player to pass through as normal.
		/// </summary>
		public enum DoorwayTileDestinationType {
			Forward		= 0,
			Warp		= 1,
		}
		#endregion
		
	}
}