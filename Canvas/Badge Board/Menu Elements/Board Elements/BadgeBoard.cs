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
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// The class that will handle manipulation of the badge grid itself.
	/// The big boy...
	/// </summary>
	public class BadgeBoard : MonoBehaviour {

		#region FIELDS - CACHE
		/// <summary>
		/// A dictionary to fill so that board slots at a given coordinate can be retrieved faster than iterating through a linear list.
		/// </summary>
		private Dictionary<Vector2Int, BadgeBoardSlot> BoardSlotCacheDict { get; set; } = new Dictionary<Vector2Int, BadgeBoardSlot>();
		#endregion
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when fading the backing in.
		/// </summary>
		[TabGroup("Board", "Tweening"), SerializeField, Title("Timing")]
		private float backingFadeInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading the backing in.
		/// </summary>
		[TabGroup("Board", "Tweening"), SerializeField]
		private float backingFadeOutTime = 0.5f;
		#endregion

		#region FIELDS - TWEENING : COLORS
		/// <summary>
		/// The color to use for the backing's front when displayed.
		/// </summary>
		[TabGroup("Board", "Tweening"), SerializeField, Title("Colors")]
		private Color backingFrontDisplayColor = Color.black;
		/// <summary>
		/// The color to use for the backing's dropshadow when displayed.
		/// </summary>
		[TabGroup("Board", "Tweening"), SerializeField]
		private Color backingDropshadowDisplayColor = Color.white;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : UI
		/// <summary>
		/// Contains all of the other objects in the board as children.
		/// </summary>
		[TabGroup("Board", "Scene References"), SerializeField, Title("UI")]
		private GameObject allObjects;
		/// <summary>
		/// The image to use for the backing's front.
		/// </summary>
		[TabGroup("Board", "Scene References"), SerializeField]
		private Image backingFrontImage;
		/// <summary>
		/// The image to use for the backing's dropshadow.
		/// </summary>
		[TabGroup("Board", "Scene References"), SerializeField]
		private Image backingDropshadowImage;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : COMPONENTS
		/// <summary>
		/// Provides access to tools that allow the player to manipulate pieces on the board.
		/// </summary>
		[TabGroup("Board", "Scene References"), SerializeField, Title("Components")]
		private BadgeBoardPieceCrane boardPieceCrane;
		/// <summary>
		/// All of the slots that are part of this board.
		/// </summary>
		[TabGroup("Board", "Scene References"), SerializeField]
		private List<BadgeBoardSlot> allBoardSlots = new List<BadgeBoardSlot>();
		/// <summary>
		/// A list of board pieces that can be used to represent badges on the board.
		/// </summary>
		[TabGroup("Board", "Scene References"), SerializeField]
		private List<BadgeBoardPiece> allBoardPieces = new List<BadgeBoardPiece>();
		#endregion
		
		#region PROPERTIES - SCENE REFERENCES : COMPONENTS
		/// <summary>
		/// Provides access to tools that allow the player to manipulate pieces on the board.
		/// </summary>
		public BadgeBoardPieceCrane BoardPieceCrane => this.boardPieceCrane;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// Go through each board slot and build the cache.
			foreach (BadgeBoardSlot boardSlot in this.allBoardSlots) {
				this.BoardSlotCacheDict.Add(
					key: boardSlot.SlotCoordinates, 
					value: boardSlot);
			}
		}
		#endregion
		
		#region CHECKERS - BOARD PIECES/SLOTS
		/// <summary>
		/// Checks whether or not the coordinates passed in are within the boundaries of this board's grid.
		/// </summary>
		/// <param name="slotCoordinates">The slot coordinates to check against.</param>
		/// <returns>Whether or not the slot coordinates are valid.</returns>
		public bool CheckValidDollyCoordinates(Vector2Int slotCoordinates) {
			
			int gridWidth = BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid.GridWidth;
			int gridLength = BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid.GridLength;

			bool insideXBounds = (-2 < slotCoordinates.x) && (slotCoordinates.x < gridWidth + 2);
			bool insideYBounds = (-2 < slotCoordinates.y) && (slotCoordinates.y < gridLength + 2);

			return (insideXBounds && insideYBounds);

			/*return BadgeBoardController.Instance
				.CurrentBoardParams
				.CurrentWeapon
				.BadgeGrid
				.CheckValidSlotCoordinates(coordinatesToCheck: slotCoordinates);*/
			
		}
		/// <summary>
		/// Checks whether or not a badge specified actually has a piece associated with it on the board.
		/// </summary>
		/// <param name="badgeToCheck">The badge to see if it has a piece on the board.</param>
		/// <returns>Whether or not the specified badge is represented on the board with a piece.</returns>
		public bool HasBoardPieceForBadge(Badge badgeToCheck) {
			// Check if any of the badges in use actually have the one passed in.
			return this.allBoardPieces
				.Where(bp => bp.IsInUse)
				.Any(bp => bp.AssignedBadgePlacement.Badge == badgeToCheck);
		}
		#endregion
		
		#region GETTERS - BOARD PIECES/SLOTS
		/// <summary>
		/// Gets the board piece associated with the specified badge.
		/// </summary>
		/// <param name="badge">The badge that the desired board piece should be representing.</param>
		/// <returns>The board piece that has the specified badge inside it.</returns>
		public BadgeBoardPiece GetBoardPiece(Badge badge) {
			
			// This should only be called if the board actually has the piece desired.
			Debug.Assert(this.HasBoardPieceForBadge(badgeToCheck: badge) == true);

			// Return the first piece that has a badge which matches the one passed in.
			return this.allBoardPieces.First(bp => bp.AssignedBadgePlacement.Badge == badge);

		}
		/// <summary>
		/// Gets the board slot associated with the coordinates provided.
		/// </summary>
		/// <param name="slotCoordinates">The coordinates of the desired slot.</param>
		/// <returns>The slot located at the provided coordinates.</returns>
		public BadgeBoardSlot GetBoardSlot(Vector2Int slotCoordinates) {
			// Grab the first slot whose coordinates match the ones specified.
			// return this.allBoardSlots.First(bs => bs.SlotCoordinates == slotCoordinates);
			return this.BoardSlotCacheDict[slotCoordinates];
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Kill all tweens currently operating on the board.
		/// </summary>
		private void KillAllTweens() {
			this.backingFrontImage.DOKill(complete: true);
			this.backingDropshadowImage.DOKill(complete: true);
		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			
			// Kill all tweens operating on this menu.
			this.KillAllTweens();
			
			// Snap the colors for both images to be clear.
			this.backingFrontImage.color = Color.clear;
			this.backingDropshadowImage.color = Color.clear;
			
			// Hide every board slot.
			this.HideBoardSlots(boardSlots: this.allBoardSlots);
			
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Presents the board controller using the data contained in the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters containing the information on how this board should be.</param>
		public void Present(BadgeBoardParams boardParams) {
			
			// Kill any tweens currently ongoing.
			this.KillAllTweens();
			
			// Tween the images to their display colors.
			this.backingFrontImage.DOColor(
					endValue: this.backingFrontDisplayColor,
					duration: this.backingFadeInTime)
				.SetEase(Ease.Linear);
			this.backingDropshadowImage.DOColor(
					endValue: this.backingDropshadowDisplayColor,
					duration: this.backingFadeInTime)
				.SetEase(Ease.Linear);
			
		}
		/// <summary>
		/// Dismisses this element from the screen.
		/// </summary>
		/// <param name="boardParams">The board params that were used to create this object.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			
			// Kill any outstanding tweens.
			this.KillAllTweens();
			
			// Tween the images to their display colors.
			this.backingFrontImage.DOColor(
					endValue: Color.clear, 
					duration: this.backingFadeOutTime)
				.SetEase(Ease.Linear);
			this.backingDropshadowImage.DOColor(
					endValue: Color.clear, 
					duration: this.backingFadeOutTime)
				.SetEase(Ease.Linear);
			
			// Hide every board slot.
			this.HideBoardSlots(boardSlots: this.allBoardSlots);
			
			// Clear the board pieces to reset the state.
			this.allBoardPieces.ForEach(bp => bp.ClearPiece());
			
		}
		#endregion

		#region BUILDING - MAIN CALLS
		/// <summary>
		/// Builds the board with the parameters specified.
		/// </summary>
		/// <param name="boardParams"></param>
		public void BuildBoard(BadgeBoardParams boardParams) {
			// Cascade down with the grid stored in the weapon of the params.
			this.BuildBoard(badgeGrid: boardParams.CurrentWeapon.BadgeGrid);
		}
		/// <summary>
		/// Builds the board with the badge grid provided.
		/// </summary>
		/// <param name="badgeGrid">The badge grid that should be used to build this board.</param>
		public void BuildBoard(BadgeGrid badgeGrid) {
			
			// Hide all of the slots inside this board.
			this.HideBoardSlots(boardSlots: this.allBoardSlots);
			
			// Now turn on the ones that are explicitly usable, as determined by the grid passed in.
			this.ShowUsableSlots(
				boardSlots: this.allBoardSlots, 
				badgeGrid: badgeGrid);
			
			// Clear the board pieces to reset the state.
			this.allBoardPieces.ForEach(bp => bp.ClearPiece());
			
			// Generate the pieces based on the placements inside the grid.
			List<BadgeBoardPiece> generatedBoardPieces = this.GenerateBoardPieces(
				badgePlacements: badgeGrid.CurrentBadgePlacements, 
				availablePieces: this.allBoardPieces);

			// Now that they exist visually, refresh them on the board.
			this.PlaceBoardPiece(badgeBoardPieces: generatedBoardPieces);
			
		}
		#endregion

		#region BUILDING - SLOTS
		/// <summary>
		/// Hides the board slots passed in from view, so they are totally invisible.
		/// </summary>
		/// <param name="boardSlots">The board slots to hide.</param>
		private void HideBoardSlots(List<BadgeBoardSlot> boardSlots) {
			
			// Go through each slot and hide it.
			foreach (BadgeBoardSlot boardSlot in boardSlots) {
				boardSlot.HideSlot();
			}
			
		}
		/// <summary>
		/// Shows the slots on the board that are usable.
		/// </summary>
		/// <param name="boardSlots">The board slots to populate with this badge grid..</param>
		/// <param name="badgeGrid">The badge grid containing the coordinates of available slots.</param>
		private void ShowUsableSlots(List<BadgeBoardSlot> boardSlots, BadgeGrid badgeGrid) {
			
			// Get the coordinates of all usable slots from the grid and turn them on here.
			badgeGrid.UsableSlotCoordinates
				.Select(v => this.GetBoardSlot(slotCoordinates: v))
				.ToList()
				.ForEach(bs => bs.ShowSlot());
			
		}
		#endregion
		
		#region BUILDING - PIECES
		/// <summary>
		/// Generates the required board pieces based on the placements specified.
		/// </summary>
		/// <param name="badgePlacements">The badge placements representing whats on the board.</param>
		/// <param name="availablePieces">The board pieces that have been cleared and are able to be used. It's possible not every one will be used.</param>
		/// <returns>A list of board pieces that are active and ready to manipulate.</returns>
		private List<BadgeBoardPiece> GenerateBoardPieces(List<BadgePlacement> badgePlacements, List<BadgeBoardPiece> availablePieces) {
			
			// Make sure that NONE of the pieces passed in are in use.
			Debug.Assert(availablePieces.Count(bp => bp.IsInUse) == 0);

			// Iterate through the placements provided and generate each one.
			for (int i = 0; i < badgePlacements.Count; i++) {
				availablePieces[i].BuildPiece(badgePlacement: badgePlacements[i]);
			}
			
			// Return only the pieces that were actually generated.
			return availablePieces.Where(bp => bp.IsInUse == true).ToList();
			
		}
		#endregion

		#region PIECE PLACEMENT
		/// <summary>
		/// Places the specified badge pieces onto the board in their appropriate positions.
		/// </summary>
		/// <param name="badgeBoardPieces">The board pieces that need to be placed down.</param>
		private void PlaceBoardPiece(List<BadgeBoardPiece> badgeBoardPieces) {
			// Just go through all of them.
			foreach (BadgeBoardPiece boardPiece in badgeBoardPieces) {
				this.PlaceBoardPiece(boardPiece: boardPiece);
			}
		}
		/// <summary>
		/// Places the specified board piece into its appropriate slot.
		/// </summary>
		/// <param name="boardPiece">The piece that should be positioned on the board.</param>
		private void PlaceBoardPiece(BadgeBoardPiece boardPiece) {

			// Get the slot that should serve as the anchor for this piece based on where its coordinates say its at.
			BadgeBoardSlot targetSlot = this.GetBoardSlot(slotCoordinates: boardPiece.AssignedBadgePlacement.GlobalPivotPosition);

			// Cascade down.
			this.PlaceBoardPiece(
				boardPiece: boardPiece,
				pivotSlot: targetSlot);
			
		}
		/// <summary>
		/// Places the specified board piece onto the specified slot.
		/// </summary>
		/// <param name="boardPiece">The piece that should be positioned on the board..</param>
		/// <param name="pivotSlot">The slot where the badge should be anchored.</param>
		private void PlaceBoardPiece(BadgeBoardPiece boardPiece, BadgeBoardSlot pivotSlot) {
			// Reposition the board piece based on the slot provided along with it.
			boardPiece.TargetRectTransform.position = pivotSlot.TargetRectTransform.position;
		}
		#endregion
		
		#region ANIMATIONS - BOARD PIECES
		/// <summary>
		/// Updates the visuals on whatever board piece should be pulsing at the current moment, if any.
		/// </summary>
		/// <param name="currentBadge">The badge currently being highlighted/manipulated.</param>
		public void UpdatePulsingBoardPiece(Badge currentBadge) {
			// Cascade down. Only use the pieces that are currently in use.
			this.UpdatePulsingBoardPiece(
				currentBadge: currentBadge,
				boardPieces: this.allBoardPieces.Where(bp => bp.IsInUse).ToList());
		}
		/// <summary>
		///  Updates the visuals on whatever board piece should be pulsing at the current moment, if any.
		/// </summary>
		/// <param name="currentBadge">The badge currently being highlighted/manipulated.</param>
		/// <param name="boardPieces">The pieces that are currently on the board.</param>
		private void UpdatePulsingBoardPiece(Badge currentBadge, List<BadgeBoardPiece> boardPieces) {

			// Stop the pulse animations on all the pieces. This is a state reset basically.
			this.StopPulsingBoardPieces(boardPieces: boardPieces);
			
			// Find the board pieces that contain the provided badge. Note there should only ever be 1 or 0 matches.
			List<BadgeBoardPiece> matchingPieces = boardPieces
				.Where(bp => bp.AssignedBadgePlacement.Badge == currentBadge)
				.ToList();
			Debug.Assert(matchingPieces.Count < 2);
			
			// Start the pulse animation on the matches (for right now, just one.)
			foreach (BadgeBoardPiece matchingPiece in matchingPieces) {
				matchingPiece.BeginPulseAnimation();
			}
			
		}
		/// <summary>
		/// Stops any and all board pieces from pulsing.
		/// </summary>
		public void StopAllPulsingBoardPieces() {
			// Cascade down.
			this.StopPulsingBoardPieces(boardPieces: this.allBoardPieces);
		}
		/// <summary>
		/// Stops pulsing the board pieces passed in.
		/// </summary>
		/// <param name="boardPieces">The pieces to stop pulsing.</param>
		private void StopPulsingBoardPieces(List<BadgeBoardPiece> boardPieces) {
			// Stop the pulse animations on all the pieces. This is a state reset basically.
			foreach (BadgeBoardPiece boardPiece in boardPieces) {
				boardPiece.StopPulseAnimation();
			}
		}
		#endregion
		
		
		
	}
}