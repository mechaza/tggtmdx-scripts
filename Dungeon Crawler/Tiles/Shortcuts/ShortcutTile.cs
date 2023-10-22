using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Chat;
using Grawly.UI.Legacy;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// A shortcut that can be interacted with in a crawler dungeon
	/// that operates similar to Etrian Odyssey's shortcuts.
	/// </summary>
	public class ShortcutTile : CrawlerFloorTile, IPlayerApproachHandler, IPlayerLookAwayHandler, IPlayerInteractHandler {

		#region FIELDS - STATE
		/// <summary>
		/// Is this shortcut open and available to be traversed through the exit side?
		/// </summary>
		public bool IsOpen { get; set; } = false;
		#endregion
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The side of the tile that serves as the entrance to the shortcut.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private ShortcutSideType entranceSideType = ShortcutSideType.None;
		/// <summary>
		/// The text to display when prompting the player to interact.
		/// </summary>
		[SerializeField]
		private string promptText = "";
		/// <summary>
		/// The text to display in a chat window if trying to enter the shortcut from the wrong side.
		/// </summary>
		[SerializeField]
		private string declineText = "";
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The side of the tile that serves as the entrance to the shortcut.
		/// </summary>
		public ShortcutSideType EntranceSideType => this.entranceSideType;
		/// <summary>
		/// The side of the tile that serves as the exit to the shortcut.
		/// </summary>
		public ShortcutSideType ExitSideType {
			get {
				// The exit is just the reverse side of the entrance.
				switch (this.entranceSideType) {
					case ShortcutSideType.N:
						return ShortcutSideType.S;
					case ShortcutSideType.E:
						return ShortcutSideType.W;
					case ShortcutSideType.S:
						return ShortcutSideType.N;
					case ShortcutSideType.W:
						return ShortcutSideType.E;
					default:
						throw new System.Exception("Couldn't determine what the exit side is!");
				}
			}
		}
		#endregion

		#region FIELDS - TIMING
		/// <summary>
		/// The amount of time to take when fading out after entering the shortcut.
		/// </summary>
		[SerializeField, Title("Timing")]
		private float fadeOutTime = 0.5f;
		/// <summary>
		/// The amount of time to wait before fading back in.
		/// </summary>
		[SerializeField]
		private float waitTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading in after going through the shortcut.
		/// </summary>
		[SerializeField]
		private float fadeInTime = 0.5f;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The side graphics to decorate the sides of this prefab with.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private List<ShortcutTileSideGraphic> sideGraphics = new List<ShortcutTileSideGraphic>();
		/// <summary>
		/// The sprite to use when showing the up-arrow on the minimap.
		/// </summary>
		[SerializeField]
		private SpriteRenderer upArrowSpriteRenderer;
		/// <summary>
		/// The sprite to use when showing the down-arrow on the minimap.
		/// </summary>
		[SerializeField]
		private SpriteRenderer downArrowSpriteRenderer;
		/// <summary>
		/// The sprite to use when showing the right-arrow on the minimap.
		/// </summary>
		[SerializeField]
		private SpriteRenderer rightArrowSpriteRenderer;
		/// <summary>
		/// The sprite to use when showing the left-arrow on the minimap.
		/// </summary>
		[SerializeField]
		private SpriteRenderer leftArrowSpriteRenderer;
		/// <summary>
		/// The sprite to use when showing the two-way vertical arrow.
		/// </summary>
		[SerializeField]
		private SpriteRenderer verticalArrowSpriteRenderer;
		/// <summary>
		/// The sprite to use when showing the two-way horizontal arrow.
		/// </summary>
		[SerializeField]
		private SpriteRenderer horizontalArrowSpriteRenderer;
		#endregion

		#region UNITY CALLS
		private void Start() {
			this.ResetState();
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this shortcut.
		/// </summary>
		private void ResetState() {
			
			// Set shortcut access to be closed.
			this.SetShortcutAccess(isOpen: false);
			
			// Reset the state on each of the side graphics.
			this.sideGraphics.ForEach(sg => sg.ResetState());
			
			// Update the graphics accordingly.
			this.UpdateGraphics();
			
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Updates the graphics on the side of the shortcut tile to reflect the shortcut's state.
		/// </summary>
		private void UpdateGraphics() {
			// Just pass in the current state.
			this.UpdateGraphics(
				entranceSide: this.EntranceSideType, 
				exitSide: this.ExitSideType,
				isOpen: this.IsOpen);
		}
		/// <summary>
		/// Updates the graphics on the side of the shortcut tile to reflect the shortcut's state.
		/// </summary>
		/// <param name="entranceSide">The side to use for the entrance.</param>
		/// <param name="exitSide">The side to use for the exit.</param>
		/// <param name="isOpen">Whether the path is open or not.</param>
		private void UpdateGraphics(ShortcutSideType entranceSide, ShortcutSideType exitSide, bool isOpen) {

			// Go through each of the side graphics and set them based on the parameters specified.
			this.sideGraphics.ForEach(sg => sg.SetShortcutGraphic(
				entranceSide: entranceSide, 
				exitSide: exitSide, 
				isOpen: isOpen));
			
			// Turn off all the sprite renderers for the arrows.
			this.upArrowSpriteRenderer.gameObject.SetActive(false);
			this.downArrowSpriteRenderer.gameObject.SetActive(false);
			this.leftArrowSpriteRenderer.gameObject.SetActive(false);
			this.rightArrowSpriteRenderer.gameObject.SetActive(false);
			this.verticalArrowSpriteRenderer.gameObject.SetActive(false);
			this.horizontalArrowSpriteRenderer.gameObject.SetActive(false);
			
			// Switch based on what the exit is.
			// If the path is open, use the correct two-way arrow.
			// If the path is closed, use the arrow pointing towards the exit.
			switch (exitSide) {
				case ShortcutSideType.N:
					if (isOpen == true) {
						this.verticalArrowSpriteRenderer.gameObject.SetActive(true);
					} else {
						this.upArrowSpriteRenderer.gameObject.SetActive(true);
					}
					break;
				case ShortcutSideType.S:
					if (isOpen == true) {
						this.verticalArrowSpriteRenderer.gameObject.SetActive(true);
					} else {
						this.downArrowSpriteRenderer.gameObject.SetActive(true);
					}
					break;
				case ShortcutSideType.W:
					if (isOpen == true) {
						this.horizontalArrowSpriteRenderer.gameObject.SetActive(true);
					} else {
						this.leftArrowSpriteRenderer.gameObject.SetActive(true);
					}
					break;
				case ShortcutSideType.E:
					if (isOpen == true) {
						this.horizontalArrowSpriteRenderer.gameObject.SetActive(true);
					} else {
						this.rightArrowSpriteRenderer.gameObject.SetActive(true);
					}
					break;
				default:
					throw new System.Exception("This should never be reached!");
			}
			
			
			
		}
		#endregion

		#region SHORTCUT ACCESS
		/// <summary>
		/// Sets whether or not the shortcut should be opened or not.
		/// </summary>
		/// <param name="isOpen">The state of whether the shortcut is open or not.</param>
		public void SetShortcutAccess(bool isOpen) {
			
			// Update the state field.
			this.IsOpen = isOpen;
			
			// Also set the visibility of the side graphics.
			this.UpdateGraphics(
				entranceSide: this.EntranceSideType,
				exitSide: this.ExitSideType, 
				isOpen: isOpen);
			
		}
		#endregion
		
		#region SHORTCUT TRAVERSAL
		/// <summary>
		/// Gets the position that the crawler player should end up on when walking through the shortcut.
		/// </summary>
		/// <param name="crawlerPlayer"></param>
		/// <returns></returns>
		private Vector3 GetTargetPosition(CrawlerPlayer crawlerPlayer) {
			
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
		/// <summary>
		/// The actual function to run when entering the shortcut.
		/// </summary>
		private void TraverseShortcut() {
			
			// Calculate the target position.
			Vector3 targetPosition = this.GetTargetPosition(crawlerPlayer: CrawlerPlayer.Instance);
			
			// Cascade down.
			this.TraverseShortcut(
				crawlerPlayer: CrawlerPlayer.Instance, 
				targetPosition: targetPosition);
			
		}
		/// <summary>
		/// The actual function to run when entering the shortcut.
		/// </summary>
		/// <param name="targetPosition"></param>
		private void TraverseShortcut(CrawlerPlayer crawlerPlayer, Vector3 targetPosition) {
			
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
				this.SetShortcutAccess(isOpen: true);
				crawlerPlayer.SetState(state: CrawlerPlayerState.Free);
			});
			
		}
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

			// Check to see if the player is on any of the sides, then convert it to the shortcut side type.
			// (Using two enum types was a bad idea but I'm too far in right now.)
			CrawlerTileSideType playerTileSide = this.CheckSidesForPlayer();
			ShortcutSideType playerShortcutSide = (ShortcutSideType) playerTileSide;
			
			// Check if the player is on either the entrance or exit sides.
			bool onEntranceSide = (playerShortcutSide == this.EntranceSideType);
			bool onExitSide = (playerShortcutSide == this.ExitSideType);
			
			// The shortcut can be used if the player is on the entrance side or on the exit side (after being opened)
			bool canUseShortcut = (onEntranceSide == true) 
			                      || (onExitSide == true && this.IsOpen == true);

			// If the shortcut can actually be used, do so.
			if (canUseShortcut == true) {
				// Dismiss the prompt. This should work regardless of it was shown or not.
				CrawlerActionPrompt.Instance?.Dismiss();
				this.TraverseShortcut();
			} else {
				// If the shortcut *can't* be used, tell the player as so.
				CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Wait);
				ChatControllerDX.GlobalOpen(scriptLine: this.declineText, simpleClosedCallback: () => {
					// Setting the player free before invoking the OnLand should work I hope.
					CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Free);
					CrawlerPlayer.Instance.GetSteppedTiles()
						.ForEach(t => t.OnLand(
							crawlerProgressionSet: CrawlerController.Instance.CurrentProgressionSet, 
							floorNumber: CrawlerController.Instance.CurrentFloorNumber));
				});
			}
			
		}
		#endregion
		
	}

	#region ENUMS
	/// <summary>
	/// Defines the different sides that are on a shortcut tile.
	/// Used in defining entrances/exits.
	/// </summary>
	public enum ShortcutSideType {
		None = 0,
		N = 1,
		E = 2,
		S = 3,
		W = 4,
	}
	#endregion
}