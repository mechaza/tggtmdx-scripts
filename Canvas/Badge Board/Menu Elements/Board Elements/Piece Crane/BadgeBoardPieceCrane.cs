using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using Grawly.Battle.Equipment;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.Chat;
using Grawly.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Provides controls to manipulate pieces on the badge board.
	/// I'd prefer to do this from a "crane" type object rather than interface with the pieces directly.
	/// </summary>
	public class BadgeBoardPieceCrane : MonoBehaviour {

		#region PROPERTIES - STATE
		/// <summary>
		/// The badge that is currently being manipulated by the crane.
		/// </summary>
		private Badge CurrentBadge => BadgeBoardController.Instance.CurrentBoardParams.CurrentSelectedBadge;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : COMPONENTS
		/// <summary>
		/// The GameObject that should be selected when moving back to the center button.
		/// </summary>
		[SerializeField, TabGroup("Crane", "Scene References"), Title("Components")]
		private GameObject centerButtonSelectableGameObject;
		/// <summary>
		/// The "dummy" piece that is used to preview the "real" board piece being manipulated.
		/// </summary>
		[SerializeField, TabGroup("Crane", "Scene References")]
		private DummyBoardPiece dummyBoardPiece;
		/// <summary>
		/// The component that listens for button prompts to rotate/flip the dummy piece.
		/// Gets enabled/disabled when the crane is turned on/off.
		/// </summary>
		[SerializeField, TabGroup("Crane", "Scene References")]
		private BadgeBoardPieceCraneActionListener craneActionListener;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The dummy piece that is used to preview the real piece being manipulated.
		/// I'm just making this a property so Rider can quickly show me where its being used.
		/// </summary>
		private DummyBoardPiece DummyPiece => this.dummyBoardPiece;
		#endregion
		
		#region CHECKERS - CRANE MOVEMENT
		/// <summary>
		/// Checks whether or not the dummy piece can be dollied over to the coordinates specified.
		/// </summary>
		/// <param name="targetCoordinates">The coordinates that need to be checked if they can be moved to.</param>
		/// <returns>Whether or not its possible to dolly over to the target coordinates.</returns>
		private bool CanDollyToCoordinates(Vector2Int targetCoordinates) {
			// Cascade down using the regular badge board.
			return this.CanDollyToCoordinates(
				badgeBoard: BadgeBoardController.Instance.BadgeBoard, 
				targetCoordinates: targetCoordinates);
		}
		/// <summary>
		/// Checks whether or not the dummy piece can be dollied over to the coordinates specified.
		/// </summary>
		/// <param name="badgeBoard">The badge board this crane is laying on top of.</param>
		/// <param name="targetCoordinates"></param>
		/// <returns></returns>
		private bool CanDollyToCoordinates(BadgeBoard badgeBoard, Vector2Int targetCoordinates) {
			// This information is basically just stored in the badge board.
			return badgeBoard.CheckValidDollyCoordinates(slotCoordinates: targetCoordinates);
		}
		#endregion
		
		#region CRANE ACTIVATION
		/// <summary>
		/// Activates the crane to be able to place the provided badge onto the grid.
		/// </summary>
		/// <param name="badgeToPlace">The badge that should be placed on the grid.</param>
		public void ActivateCrane(Badge badgeToPlace) {
			// Cascade down.
			this.ActivateCrane(
				badgeToPlace: badgeToPlace,
				badgeBoard: BadgeBoardController.Instance.BadgeBoard, 
				dummyPiece: this.DummyPiece);
		}
		/// <summary>
		/// Activates the crane to be able to place the provided badge onto the grid.
		/// </summary>
		/// <param name="badgeToPlace">The badge to place onto the board.</param>
		/// <param name="badgeBoard">The board this crane will be acting on top of.</param>
		/// <param name="dummyPiece">The dummy piece used to represent the badge before placement.</param>
		private void ActivateCrane(Badge badgeToPlace, BadgeBoard badgeBoard, DummyBoardPiece dummyPiece) {
			
			// Enable the action listener component.
			this.craneActionListener.enabled = true;
			
			// Check real quick if a piece has already been placed for the selected badge.
			bool boardPieceAlreadyPlaced = badgeBoard.HasBoardPieceForBadge(badgeToCheck:badgeToPlace);

			// If a piece was already placed, hide it and build the dummy piece with its information.
			// While the real one still exists, I'll make sure to ignore it when performing placement checks.
			if (boardPieceAlreadyPlaced == true) {
				BadgeBoardPiece existingPiece = badgeBoard.GetBoardPiece(badge: badgeToPlace);
				existingPiece.HidePiece();
				dummyPiece.BuildDummyPiece(badgePlacement: existingPiece.AssignedBadgePlacement.Clone());
			} else {
				// If a piece doesn't already exist on the board, allow the dummy to infer default values.
				dummyPiece.BuildDummyPiece(badge: badgeToPlace);
			}
			
			// Dolly the crane over to the correct position.
			this.DollyCrane(
				dummyPiece: dummyPiece, 
				badgeBoard: badgeBoard,
				targetCoordinates: dummyPiece.CurrentWorkingPlacement.GlobalPivotPosition);
			
			// Select the center button so the dpad controls can be used.
			EventSystem.current.SetSelectedGameObject(this.centerButtonSelectableGameObject);
			
		}
		/// <summary>
		/// Performs cleanup on the crane when exiting control from it.
		/// </summary>
		public void DeactivateCrane() {
			
			// Disable the action listener component.
			this.craneActionListener.enabled = false;
			
			// Dismiss the dummy piece.
			this.DummyPiece.DismissDummyPiece();
			
		}
		#endregion

		#region CRANE MOVEMENT
		/// <summary>
		/// Dollies the crane over to the coordinates specified.
		/// </summary>
		/// <param name="targetCoordinates">The coordinates that the crane should move to.</param>
		private void DollyCrane(Vector2Int targetCoordinates) {
			// Cascade down.
			this.DollyCrane(
				dummyPiece: this.dummyBoardPiece,
				badgeBoard: BadgeBoardController.Instance.BadgeBoard, 
				targetCoordinates: targetCoordinates);
		}
		/// <summary>
		/// Dollies the crane over to the coordinates specified.
		/// </summary>
		/// <param name="dummyPiece">The dummy piece that should be relocated.</param>
		/// <param name="badgeBoard">The badge board this crane is working with.</param>
		/// <param name="targetCoordinates">The target coordinates to dolly to.</param>
		private void DollyCrane(DummyBoardPiece dummyPiece, BadgeBoard badgeBoard, Vector2Int targetCoordinates) {
		
			// Grab the slot located at the provided coordinates.
			BadgeBoardSlot targetSlot = badgeBoard.GetBoardSlot(slotCoordinates: targetCoordinates);
			
			// Cascade down.
			this.DollyCrane(
				dummyPiece: dummyPiece, 
				targetSlot: targetSlot);
			
		}
		/// <summary>
		/// Dollies the crane over to the slot specified.
		/// </summary>
		/// <param name="dummyPiece">The dummy piece that should be relocated.</param>
		/// <param name="targetSlot">The target slot containing the correct anchored position.</param>
		private void DollyCrane(DummyBoardPiece dummyPiece, BadgeBoardSlot targetSlot) {
			
			// Set the position of the dummy piece's own pivot to the target in the provided slot.
			dummyPiece.PivotRectTransform.position = targetSlot.TargetRectTransform.position;
			
			// Update the coordinates on the dummy piece's placement.
			dummyPiece.CurrentWorkingPlacement.UpdateCoordinates(
				targetCoordinates: targetSlot.SlotCoordinates);
			
		}
		#endregion

		#region PIECE CONFIRMATION
		/// <summary>
		/// Cancels placing a badge onto the board.
		/// It will also remove an existing badge from the grid if the one being placed already exists on the board.
		/// </summary>
		/// <param name="dummyPiece">The dummy piece with everything I need.</param>
		/// <param name="badgeGrid">The grid it will be placed on.</param>
		/// <param name="badgeBoard">The board that is currently on screen.</param>
		private void CancelBadgePlacement(DummyBoardPiece dummyPiece, BadgeGrid badgeGrid, BadgeBoard badgeBoard) {

			// Check if the grid still has the badge that was just being placed down.
			bool gridStillHasBadge = badgeGrid.HasBadge(dummyPiece.CurrentWorkingPlacement.Badge.BadgeID);

			// If the grid has the badge the dummy piece was manipulating,
			if (gridStillHasBadge == true) {
				// remove the "real" one from the grid.
				badgeGrid.RemoveBadge(dummyPiece.CurrentWorkingPlacement.Badge.BadgeID);
			}
			
			// Go back in the FSM.
			BadgeBoardController.Instance.TriggerEvent("Back");
			
		}
		/// <summary>
		/// Place a badge onto the board after confirming that it fits.
		/// </summary>
		/// <param name="dummyPiece">The dummy piece with everything I need.</param>
		/// <param name="badgeGrid">The grid it will be placed on.</param>
		/// <param name="badgeBoard">The board that is currently on screen.</param>
		private void ConfirmBadgePlacement(DummyBoardPiece dummyPiece, BadgeGrid badgeGrid, BadgeBoard badgeBoard) {
			
			// Check if this badge currently exists on the board.
			bool alreadyHasSameBadge = badgeGrid.HasBadge(
				badgeID: dummyPiece.CurrentWorkingPlacement.Badge.BadgeID);

			// If the same badge already exists...
			if (alreadyHasSameBadge == true) {
				// ...just update it in the grid.
				badgeGrid.UpdateBadge(updatedPlacement: dummyPiece.CurrentWorkingPlacement);
			} else {
				// If it doesn't exist in the grid, add a new entry entirely.
				badgeGrid.AddBadge(badgePlacement: dummyPiece.CurrentWorkingPlacement);
			}
			
			// Go back in the FSM.
			BadgeBoardController.Instance.TriggerEvent("Back");
			
		}
		#endregion
		
		#region DPAD EVENTS - EVENTSYSTEM
		/// <summary>
		/// Gets triggered on moving the dpad up.
		/// </summary>
		public void DpadUp() {
			
			// Calculate the potential coordinates that might be moved to.
			Vector2Int targetCoordinates = new Vector2Int(
				x: this.DummyPiece.CurrentWorkingPlacement.GlobalPivotPosition.x,
				y: this.DummyPiece.CurrentWorkingPlacement.GlobalPivotPosition.y - 1);

			// If its possible to dolly to the target, do so.
			if (this.CanDollyToCoordinates(targetCoordinates: targetCoordinates) == true) {
				this.DollyCrane(targetCoordinates: targetCoordinates);
			}
			
			// Re-select the center button. That's sort of the point for this.
			this.StartCoroutine(this.WaitAndReselect(gameObjectToSelect: this.centerButtonSelectableGameObject));
			
		}
		/// <summary>
		/// Gets triggered on moving the dpad right.
		/// </summary>
		public void DpadRight() {
			
			// Calculate the potential coordinates that might be moved to.
			Vector2Int targetCoordinates = new Vector2Int(
				x: this.DummyPiece.CurrentWorkingPlacement.GlobalPivotPosition.x + 1,
				y: this.DummyPiece.CurrentWorkingPlacement.GlobalPivotPosition.y);
			
			// If its possible to dolly to the target, do so.
			if (this.CanDollyToCoordinates(targetCoordinates: targetCoordinates) == true) {
				this.DollyCrane(targetCoordinates: targetCoordinates);
			}
			
			// Re-select the center button. That's sort of the point for this.
			this.StartCoroutine(this.WaitAndReselect(gameObjectToSelect: this.centerButtonSelectableGameObject));
			
		}
		/// <summary>
		/// Gets triggered on moving the dpad down.
		/// </summary>
		public void DpadDown() {
			
			// Calculate the potential coordinates that might be moved to.
			Vector2Int targetCoordinates = new Vector2Int(
				x: this.DummyPiece.CurrentWorkingPlacement.GlobalPivotPosition.x,
				y: this.DummyPiece.CurrentWorkingPlacement.GlobalPivotPosition.y + 1);
			
			// If its possible to dolly to the target, do so.
			if (this.CanDollyToCoordinates(targetCoordinates: targetCoordinates) == true) {
				this.DollyCrane(targetCoordinates: targetCoordinates);
			}
			
			// Re-select the center button. That's sort of the point for this.
			this.StartCoroutine(this.WaitAndReselect(gameObjectToSelect: this.centerButtonSelectableGameObject));
			
		}
		/// <summary>
		/// Gets triggered on moving the dpad left.
		/// </summary>
		public void DpadLeft() {
			
			// Calculate the potential coordinates that might be moved to.
			Vector2Int targetCoordinates = new Vector2Int(
				x: this.DummyPiece.CurrentWorkingPlacement.GlobalPivotPosition.x - 1,
				y: this.DummyPiece.CurrentWorkingPlacement.GlobalPivotPosition.y);
			
			// If its possible to dolly to the target, do so.
			if (this.CanDollyToCoordinates(targetCoordinates: targetCoordinates) == true) {
				this.DollyCrane(targetCoordinates: targetCoordinates);
			}
			
			// Re-select the center button. That's sort of the point for this.
			this.StartCoroutine(this.WaitAndReselect(gameObjectToSelect: this.centerButtonSelectableGameObject));
			
		}
		/// <summary>
		/// Gets triggered when submit is hit on the dpad's center button.
		/// </summary>
		public void DpadSubmit() {

			// Check with the badge grid if the dummy piece fits or not.
			bool dummyPieceFits = BadgeBoardController.Instance.CurrentBoardParams
				.CurrentWeapon
				.BadgeGrid
				.CanAddBadge(dummyBoardPiece: this.DummyPiece);

			// If the dummy piece fits, begin placing it.
			if (dummyPieceFits == true) {
				this.ConfirmBadgePlacement(
					dummyPiece: this.DummyPiece,
					badgeGrid:BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid, 
					badgeBoard: BadgeBoardController.Instance.BadgeBoard);
			} else {
				// If the piece doesn't fit, play an error sound.
				AudioController.instance?.PlaySFX(SFXType.Invalid);
			}
			
		}
		/// <summary>
		/// Gets triggered when cancel is hit on the dpad's center button.
		/// </summary>
		public void DpadCancel() {
			
			// Play the sound effect that goes back.
			AudioController.instance?.PlaySFX(SFXType.Close);
			
			// Cancel the placement.
			this.CancelBadgePlacement(
				dummyPiece: this.DummyPiece,
				badgeGrid:BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid, 
				badgeBoard: BadgeBoardController.Instance.BadgeBoard);
		}
		/// <summary>
		/// A helper method to just wait a frame and then re-select the specified menu list item.
		/// </summary>
		/// <param name="gameObjectToSelect"></param>
		/// <returns></returns>
		private IEnumerator WaitAndReselect(GameObject gameObjectToSelect) {
			yield return new WaitForEndOfFrame();
			EventSystem.current.SetSelectedGameObject(gameObjectToSelect);
		}
		#endregion

		#region DPAD EVENTS - CUSTOM MADE
		/// <summary>
		/// Rotates the dummy piece clockwise.
		/// </summary>
		public void RotateDummyPieceClockwise() {
			this.DummyPiece.RotateClockwise();
		}
		/// <summary>
		/// Rotates the dummy piece clockwise.
		/// </summary>
		public void RotateDummyPieceCounterClockwise() {
			this.DummyPiece.RotateCounterClockwise();
		}
		/// <summary>
		/// Rotates the dummy piece clockwise.
		/// </summary>
		public void FlipDummyPiece() {
			this.DummyPiece.Flip();
		}
		#endregion
		
	}
}