using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// A "dummy" board piece that is used by the crane to preview where a "real" piece will potentially move to.
	/// </summary>
	public class DummyBoardPiece : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The badge placement that was cloned from the piece passed in
		/// and can be used to keep track of where this dummy piece is.
		/// </summary>
		public BadgePlacement CurrentWorkingPlacement { get; private set; }
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the other objects as children.
		/// </summary>
		[SerializeField, TabGroup("Dummy", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The RectTransform that should be manipulated when moving pieces.
		/// </summary>
		[SerializeField, TabGroup("Dummy", "Scene References")]
		private RectTransform pivotRectTransform;
		/// <summary>
		/// A list of the fills that are used to build this board piece.
		/// </summary>
		[SerializeField, TabGroup("Dummy", "Scene References")]
		private List<BadgeBoardPieceFill> pieceFills = new List<BadgeBoardPieceFill>();
		/// <summary>
		/// The cursor that helps communicate that this piece is currently being used.
		/// </summary>
		[SerializeField, TabGroup("Dummy", "Scene References")]
		private DummyPieceCursor dummyCursor;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The RectTransform that should be manipulated when moving pieces.
		/// </summary>
		public RectTransform PivotRectTransform => this.pivotRectTransform;
		#endregion
		
		#region GETTERS - FILLS
		/// <summary>
		/// Gets all of the fills that are used by the specified badge placement.
		/// </summary>
		/// <param name="badgePlacement"></param>
		/// <returns></returns>
		private List<BadgeBoardPieceFill> GetUsedFills(BadgePlacement badgePlacement) {
			// Probe the placement for the local fill coordinates and return the fills that coorespond to those locations.
			return badgePlacement.LocalFillCoordinates
				.Select(v => this.GetPieceFill(localFillPos: v))
				.ToList();
		}
		/// <summary>
		/// Gets the piece fill located at the specified local coordinates.
		/// </summary>
		/// <param name="localFillPos">The position of the desired fill in local space.</param>
		/// <returns>The fill located at the provided position, in local space.</returns>
		private BadgeBoardPieceFill GetPieceFill(Vector2Int localFillPos) {
			// Find the first fill whose coordinates match the ones passed in.
			return this.pieceFills.First(f => f.FillCoordinates == localFillPos);
		}
		#endregion
		
		#region BUILDING
		/// <summary>
		/// Builds a dummy piece from the badge provided.
		/// Assumes it does not actually exist in the grid, so defaults will be used.
		/// </summary>
		/// <param name="badge">The badge that may or may not be placed on the board.</param>
		/// <param name="xPos">The x coordinate to start the pivot at.</param>
		/// <param name="yPos">The y coordinate to start the pivot at.</param>
		public void BuildDummyPiece(Badge badge, int xPos = 2, int yPos = 2) {

			// Since this version of the function is creating a new BadgePlacement, neither of these conditions should be true here.
			bool gridHasBadge = BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid.HasBadge(badge: badge);
			bool boardHasPieceForBadge = BadgeBoardController.Instance.BadgeBoard.HasBoardPieceForBadge(badgeToCheck: badge);
			Debug.Assert(gridHasBadge == false);
			Debug.Assert(boardHasPieceForBadge == false);
			
			// Create a new placement from the information provided.
			BadgePlacement badgePlacement = new BadgePlacement(
				badge: badge,
				xPos: xPos, 
				yPos: yPos);
			
			// Cascade down to use this placement.
			this.BuildDummyPiece(badgePlacement: badgePlacement);
			
		}
		/// <summary>
		/// Builds a dummy piece from the placement provided.
		/// </summary>
		/// <param name="badgePlacement">The badge placement containing data about how this piece is shaped.</param>
		public void BuildDummyPiece(BadgePlacement badgePlacement) {
			
			// Since I am going to be manipulating this particular badge placement, it CANNOT exist in the grid.
			// I either create a new placement (as above) or I clone an existing one.
			bool badgePlacementInstanceExistsInGrid = BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon
				.BadgeGrid
				.CurrentBadgePlacements
				.Any(bp => bp == badgePlacement);
			Debug.Assert(badgePlacementInstanceExistsInGrid == false);
			
			// Turn all of the objects on.
			this.allObjects.SetActive(true);
			
			// Turn the cursor on as well.
			this.dummyCursor.EnableCursor();
			
			// Clone the badge placement inside the piece so that I can manipulate it accordingly.
			this.CurrentWorkingPlacement = badgePlacement;
			
			// Refresh the visuals. This is its own method because I use it often.
			this.RefreshDummyPieceVisuals(badgePlacement: badgePlacement);
			
		}
		/// <summary>
		/// Refreshes the visuals on the dummy piece only.
		/// This is helpful when rotating/flipping and I don't want to completlely reset the state as when I build from scratch.
		/// </summary>
		/// <param name="badgePlacement">The badgeplacement containing the rotation/flip transformations.</param>
		private void RefreshDummyPieceVisuals(BadgePlacement badgePlacement) {
			
			// Go through all the fills and clear them out as well.
			this.pieceFills.ForEach(f => f.ClearFill());
			
			// Get all of the fills that should be used and build them out.
			List<BadgeBoardPieceFill> usedFills = this.GetUsedFills(badgePlacement: this.CurrentWorkingPlacement);
			foreach (BadgeBoardPieceFill usedFill in usedFills) {
				usedFill.BuildFill(elementType: this.CurrentWorkingPlacement.Badge.ElementType);
			}
			
		}
		/// <summary>
		/// Dismisses the dummy piece when its utility is done with.
		/// </summary>
		public void DismissDummyPiece() {
			
			// Null out the current working placement.
			this.CurrentWorkingPlacement = null;
			
			// Turn all of the objects off.
			this.allObjects.SetActive(false);
			
			// Turn the cursor on as well.
			this.dummyCursor.DisableCursor();
			
		}
		#endregion

		#region ROTATIONS/FLIPS
		/// <summary>
		/// Rotates the dummy board piece clockwise and refreshes the visuals.
		/// </summary>
		public void RotateClockwise() {
			// Tell the working placement to rotate.
			this.CurrentWorkingPlacement.RotateClockwise();
			// Rebuild the piece.
			this.RefreshDummyPieceVisuals(badgePlacement: this.CurrentWorkingPlacement);
		}
		/// <summary>
		/// Rotates the dummy board piece counterclockwise and refreshes the visuals.
		/// </summary>
		public void RotateCounterClockwise() {
			// Tell the working placement to rotate.
			this.CurrentWorkingPlacement.RotateCounterClockwise();
			// Rebuild the piece.
			this.RefreshDummyPieceVisuals(badgePlacement: this.CurrentWorkingPlacement);
		}
		/// <summary>
		/// Flips the dummy board piece clockwise and refreshes the visuals.
		/// </summary>
		public void Flip() {
			// Tell the working placement to rotate.
			this.CurrentWorkingPlacement.Flip();
			// Rebuild the piece.
			this.RefreshDummyPieceVisuals(badgePlacement: this.CurrentWorkingPlacement);
		}
		#endregion
		
	}
}