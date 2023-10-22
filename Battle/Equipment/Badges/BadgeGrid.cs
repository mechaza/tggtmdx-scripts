using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using Grawly.Menus.BadgeBoardScreen;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// The way that badges should are placed and stored on a grid.
	/// </summary>
	public class BadgeGrid {

		#region FIELDS - STATE
		/// <summary>
		/// The grid template currently in use.
		/// </summary>
		private BadgeGridTemplate CurrentGridTemplate { get; set; }
		/// <summary>
		/// All of the badges that are currently in this grid, as well as how they are placed.
		/// </summary>
		public List<BadgePlacement> CurrentBadgePlacements { get; private set; } = new List<BadgePlacement>();
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// The badges that this grid is currently working with.
		/// Helpful when I just need quick access.
		/// </summary>
		public List<Badge> CurrentBadges => this.CurrentBadgePlacements.Select(bp => bp.Badge).ToList();
		#endregion
		
		#region PROPERTIES - STATE
		/// <summary>
		/// The full width of the grid.
		/// </summary>
		public int GridWidth => this.CurrentGridTemplate.GridWidth;
		/// <summary>
		/// The full length of the grid.
		/// </summary>
		public int GridLength => this.CurrentGridTemplate.GridLength;
		/// <summary>
		/// A list of positions that are currently filled by badges.
		/// </summary>
		private List<Vector2Int> CurrentFilledSlots {
			get {
				
				// Transform the fill positions inside of the placements into a list of the positions they all store.
				List<Vector2Int> filledSlots = this.CurrentBadgePlacements
					.SelectMany(bp => bp.GlobalFillCoordinates)
					.ToList();
				
				// For peace of mind, make sure every value is unique.
				// If not, that means there's badge overlap, which is illegal.
				Debug.Assert(filledSlots.Distinct().Count() == filledSlots.Count());
				
				// Return the list.
				return filledSlots;

			}
		}
		/// <summary>
		/// A list of coordinates which are usable in general.
		/// </summary>
		public List<Vector2Int> UsableSlotCoordinates => this.CurrentGridTemplate.UsableSlotCoordinates;
		/// <summary>
		/// A list of coordinates that are usable but not filled.
		/// </summary>
		public List<Vector2Int> AvailableSlotCoordinates {
			get {
				// Take the list of usable slots and remove the ones that have been filled.
				return this.UsableSlotCoordinates
					.Except(this.CurrentFilledSlots)
					.ToList();
			}
		}
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Creates a new BadgeGrid from a template provided.
		/// </summary>
		/// <param name="badgeGridTemplate">The grid template containing baseline information about this grid.</param>
		/// <param name="badgeCollectionSet">The badge collection to retrieve default badges from.</param>
		public BadgeGrid(BadgeGridTemplate badgeGridTemplate, BadgeCollectionSet badgeCollectionSet) {
			// Save the template.
			this.CurrentGridTemplate = badgeGridTemplate;
			// Also save any badges that come preinstalled with the grid.
			this.CurrentBadgePlacements = badgeGridTemplate.GenerateDefaultBadgePlacements(badgeCollectionSet: badgeCollectionSet);
		}
		/// <summary>
		/// Creates a new BadgeGrid from the template provided.
		/// Uses the badge placements passed in to restore state, usually after loading from disk.
		/// </summary>
		/// <param name="badgeGridTemplate">The grid template containing baseline information about this grid.</param>
		/// <param name="badgePlacements">The BadgePlacements that should be assigned to this grid.</param>
		public BadgeGrid(BadgeGridTemplate badgeGridTemplate, List<BadgePlacement> badgePlacements) {
			this.CurrentGridTemplate = badgeGridTemplate;
			this.CurrentBadgePlacements = badgePlacements;
		}
		#endregion
		
		#region CHECKERS - GRID
		/// <summary>
		/// Checks whether or not the coordinates passed in are "usable" in this grid.
		/// </summary>
		/// <param name="coordinatesToCheck">The coordinates to check against.</param>
		/// <returns>Whether or not the slot coordinates specified are within the bounds of this grid.</returns>
		public bool CheckValidSlotCoordinates(Vector2Int coordinatesToCheck) {
			// If any of the usable slot coordinates match up with whats in the usable slot list, it should be valid.
			return this.UsableSlotCoordinates.Any(v => 
				v.x == coordinatesToCheck.x
				&& v.y == coordinatesToCheck.y);
		}
		/// <summary>
		/// Checks whether or not the slot at the specified coordinates is filled.
		/// </summary>
		/// <param name="xPos">The x-position to check against.</param>
		/// <param name="yPos">The y-position to check against.</param>
		/// <returns>Whether or not the slot at the specified coordinates is filled.</returns>
		public bool CheckSlotFill(int xPos, int yPos) {
			return this.CurrentFilledSlots.Any(v => v.x == xPos && v.y == yPos);
		}
		#endregion
		
		#region CHECKERS - BADGE STATUS
		/// <summary>
		/// Does this grid have the badge specified?
		/// </summary>
		/// <param name="badge">The badge to check if this grid has or not.</param>
		/// <returns>Whether or not this grid has the provided badge.</returns>
		public bool HasBadge(Badge badge) {
			// Cascade down.
			return this.HasBadge(badgeID: badge.BadgeID);
		}
		/// <summary>
		/// Does this grid have the badge specified?
		/// </summary>
		/// <param name="badgeID">The ID of the badge to check against.</param>
		/// <returns>Whether or not this grid has the badge of the provided ID.</returns>
		public bool HasBadge(BadgeID badgeID) {
			return this.CurrentBadgePlacements			// Go through the badge placements...
				.Select(bp => bp.Badge.BadgeID)			// ...transform them into a list of IDs...
				.Any(bid => bid.Equals(badgeID));		// ...and check if any of them match the ID provided.
		}
		#endregion
		
		#region CHECKERS - BADGE ADDITIONS
		/// <summary>
		/// Can the specified badge be added to the grid?
		/// </summary>
		/// <param name="badge">The badge in question.</param>
		/// <param name="xPos">The x-position on the grid that the pivot should be located.</param>
		/// <param name="yPos">The y-position on the grid that the pivot should be located.</param>
		/// <param name="rotationType">The rotation of the badge on the grid.</param>
		/// <param name="flipType">The flip of the badge on the grid.</param>
		/// <returns>Whether or not the badge can be added to the grid.</returns>
		public bool CanAddBadge(Badge badge, int xPos, int yPos, BadgeRotationType rotationType, BadgeFlipType flipType) {
			// Cascade down.
			return this.CanAddBadge(badge: badge, badgeTransform: new BadgeTransform() {
				XPos = xPos,
				YPos = yPos,
				RotationType = rotationType,
				FlipType = flipType,
			});
		}
		/// <summary>
		/// Can the specified badge being controlled by the crane be added to the grid?
		/// </summary>
		/// <param name="dummyBoardPiece">The dummy board piece that is currently in the scene.</param>
		/// <returns>Whether or not the badge manipulated by the dummy piece can be added.</returns>
		public bool CanAddBadge(DummyBoardPiece dummyBoardPiece) {
			// Cascade down using the info contained in the dummy piece.
			return this.CanAddBadge(badgePlacement: dummyBoardPiece.CurrentWorkingPlacement);
		}
		/// <summary>
		/// Can the specified badge be added to the grid?
		/// </summary>
		/// <param name="badge">The badge in question.</param>
		/// <param name="badgeTransform">The data structure containing positional and orientational data.</param>
		/// <returns>Whether or not the badge can be added to the grid.</returns>
		public bool CanAddBadge(Badge badge, BadgeTransform badgeTransform) {
			// Cascade down again.
			return this.CanAddBadge(
				badgePlacement: new BadgePlacement(
					badge: badge, 
					badgeTransform: badgeTransform));
		}
		/// <summary>
		/// Can the specified badge be added to the grid?
		/// </summary>
		/// <param name="badgePlacement">The data structure containing both a badge and its positional data.</param>
		/// <returns>Whether or not the badge can be added to the grid.</returns>
		private bool CanAddBadge(BadgePlacement badgePlacement) {

			// Check if this grid contains a badge with the same ID of the badge placement being offered.
			if (this.HasBadge(badgeID: badgePlacement.Badge.BadgeID) == true) {
				// If this badge is already on the grid, remove the slots its occupying.
				// The implication is that this badge might just be getting moved to a new coordinate.
				BadgePlacement existingPlacement = this.GetBadgePlacement(badgeID: badgePlacement.Badge.BadgeID);
				return this.CheckBadgePlacementInBounds(
					badgePlacement: badgePlacement, 
					availableSlots: this.AvailableSlotCoordinates
						.Union(existingPlacement.GlobalFillCoordinates)
						.ToList()
					);
			} else {
				// If this badge doesn't exist on the grid, return all avoilable slots.
				return this.CheckBadgePlacementInBounds(
					badgePlacement: badgePlacement, 
					availableSlots: this.AvailableSlotCoordinates);
			}
			
		}
		#endregion
		
		#region CHECKERS - BADGE POSITIONING : BOUNDARIES
		/// <summary>
		/// Checks to make sure a badge placement is in-bounds.
		/// </summary>
		/// <param name="badgePlacement">The badge placement to check boundaries for.</param>
		/// <param name="availableSlots">The coordinates that are actually available for use by the badge placement.</param>
		/// <returns>Whether or not the placement is in bounds or not.</returns>
		private bool CheckBadgePlacementInBounds(BadgePlacement badgePlacement, List<Vector2Int> availableSlots) {
			
			// Go through each of the coordinates in the badge placement.
			foreach (Vector2Int slotCoordinate in badgePlacement.GlobalFillCoordinates) {
				// If a coordinate does not exist in the available slots, we have a problem.
				if (availableSlots.Contains(slotCoordinate) == false) {
					return false;
				}
			}
			
			// If we've gotten to this point, it means that all the
			// coordinates check out and the placement is good to go.
			return true;
			
		}
		#endregion
		
		#region GETTERS - GRID
		/// <summary>
		/// Gets the type of grid slot at the specified x/y coordinates.
		/// </summary>
		/// <param name="xPos">The x-position to check on the grid.</param>
		/// <param name="yPos">The y-position to check on the grid.</param>
		/// <returns>The type of slot associated with this coordinate on the grid.</returns>
		private BadgeGridSlotType GetSlotType(int xPos, int yPos) {
			// Get this information from the template.
			return this.CurrentGridTemplate.GetSlotType(
				xPos: xPos,
				yPos: yPos);
		}
		/// <summary>
		/// Returns the badge placement associated with the given BadgeID.
		/// Will fail if the badge is not part of the grid.
		/// </summary>
		/// <param name="badgeID"></param>
		/// <returns></returns>
		private BadgePlacement GetBadgePlacement(BadgeID badgeID) {
			return this.CurrentBadgePlacements.First(bp => bp.Badge.BadgeID.Equals(badgeID));
		}
		#endregion

		#region GETTERS - MODIFIERS
		/// <summary>
		/// Returns a list of all the modifiers of the given type inside this item.
		/// </summary>
		/// <typeparam name="T">The type of modifier to return.</typeparam>
		/// <returns>A list of modifiers this item contains.</returns>
		public List<T> GetModifiers<T>() {
			// Go through the badges and aggregate the modifiers they also own.
			return this.CurrentBadges
				.SelectMany(b => b.GetModifiers<T>())
				.ToList();
		}
		#endregion

		#region SETTERS - BADGES
		/// <summary>
		/// Removes all of the badges from this grid.
		/// </summary>
		public void RemoveAllBadges() {
			
			// Get a list of all the IDs. I need to do this so list manipulation doesn't break.
			List<BadgeID> allBadgeIDs = this.CurrentBadgePlacements
				.Select(bp => bp.Badge.BadgeID)
				.ToList();

			// Go through each badgeID and remove it from the grid.
			foreach (BadgeID badgeID in allBadgeIDs) {
				this.RemoveBadge(badgeID: badgeID);
			}
			
		}
		/// <summary>
		/// Removes a badge from the grid.
		/// </summary>
		/// <param name="badgeID">The ID of the badge to remove.</param>
		public void RemoveBadge(BadgeID badgeID) {
			
			// Obviously, the grid should actually have the badge being removed.
			Debug.Assert(this.HasBadge(badgeID: badgeID) == true);

			// Find the badge placement to remove.
			BadgePlacement badgePlacementToRemove = this.CurrentBadgePlacements.First(bp => bp.Badge.BadgeID.Equals(badgeID));

			// Actually go ahead and remove it.
			this.CurrentBadgePlacements.Remove(badgePlacementToRemove);

		}
		/// <summary>
		/// Adds a badge to the grid based on the badge and transform contained in the placement object.
		/// </summary>
		/// <param name="badgePlacement">The object which contains the badge as well as its badgeTransform.</param>
		public void AddBadge(BadgePlacement badgePlacement) {
			
			// If adding a new badge, it should NOT already exist in the grid!
			Debug.Assert(this.HasBadge(badgeID: badgePlacement.Badge.BadgeID) == false);
			
			// Add it to the list of currently existing placements.
			this.CurrentBadgePlacements.Add(badgePlacement);
			
		}
		/// <summary>
		/// Updates the badge of the specified ID with the new transform.
		/// Usually happens when moving an existing badge with the crane.
		/// </summary>
		/// <param name="updatedPlacement">The badge placement that will replace the badge of the same ID.</param>
		public void UpdateBadge(BadgePlacement updatedPlacement) {
			
			// This should only ever be called if the associated badge is already in the grid.
			Debug.Assert(this.HasBadge(badgeID: updatedPlacement.Badge.BadgeID) == true);
			
			// Get the badge with the same ID as the one passed in.
			BadgePlacement originalPlacement = this.GetBadgePlacement(badgeID: updatedPlacement.Badge.BadgeID);
			
			// Use the placement passed in to update the transform on the original.
			originalPlacement.UpdateBadgeTransform(targetBadgePlacement: updatedPlacement);

		}
		#endregion
		
	}
}