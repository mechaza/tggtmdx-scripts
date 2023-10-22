using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// Contains both a badge and its positional data.
	/// </summary>
	public class BadgePlacement {

		#region FIELDS - STATE
		/// <summary>
		/// The actual badge stored with this placement.
		/// </summary>
		public Badge Badge { get; private set; }
		/// <summary>
		/// The data containing a badges positional/orientation data.
		/// </summary>
		public BadgeTransform BadgeTransform { get; private set; }
		#endregion

		#region PROPERTIES - POSITION, SIZE, AND ORIENTATION
		/// <summary>
		/// The width of this placement's fill, with rotation/flip transformations taken into account.
		/// </summary>
		private int DynamicFillWidth => this.Badge.GetFillSize(badgeTransform: this.BadgeTransform).x;
		/// <summary>
		/// The length of this placement's fill, with rotation/flip transformations taken into account.
		/// </summary>
		private int DynamicFillLength => this.Badge.GetFillSize(badgeTransform: this.BadgeTransform).y;
		/// <summary>
		/// The size of this placement's fill, with rotation/flip transformations taken into account.
		/// </summary>
		private Vector2Int DynamicFillSize => this.Badge.GetFillSize(badgeTransform: this.BadgeTransform);
		/// <summary>
		/// Is this placement upright?
		/// I.e., is it not rotated or flipped?
		/// </summary>
		public bool IsUpright {
			get {
				return this.BadgeTransform.RotationType == BadgeRotationType.Twelve
				       && this.BadgeTransform.FlipType == BadgeFlipType.Normal;
			}
		}
		/// <summary>
		/// The position of the pivot for this badge placement.
		/// </summary>
		public Vector2Int GlobalPivotPosition {
			get {
				// Using the standard of having the position at the bottom right, the transform's base x/y pos is fine.
				return new Vector2Int(
					x: this.BadgeTransform.XPos, 
					y: this.BadgeTransform.YPos);
			}
		}
		/*/// <summary>
		/// The top left position of the badge placement.
		/// </summary>
		public Vector2Int GlobalCornerPosition {
			get {
				
				// Figure out what the fill size of this badge is.
				Vector2Int fillSize = this.GetFillSize(
					badge: this.Badge, 
					rotationType: this.BadgeTransform.RotationType);
				
				// Subtract the fill size from the position of the badge on the grid.
				Vector2Int cornerPos = this.GlobalPivotPosition - fillSize;
				
				// Return it.
				return cornerPos;
				
			}
		}*/
		#endregion

		#region PROPERTIES - SLOT OCCUPANCY
		/// <summary>
		/// A list of coordinates on the grid where this badge placement has fills.
		/// I'm drunk.
		/// </summary>
		public List<Vector2Int> LocalFillCoordinates {
			get {

				// Ask the badge for what its fill positions will be with the transform's modifications applied.
				return this.Badge.GetTransformedLocalFillPositions(
					rotationType: this.BadgeTransform.RotationType,
					flipType: this.BadgeTransform.FlipType);

			}
		}
		/// <summary>
		/// A list of positions on the grid that this badge placement occupies in global space.
		/// </summary>
		public List<Vector2Int> GlobalFillCoordinates {
			get {
				
				// Grab the list of fill positions in local space from the badge.
				List<Vector2Int> localFillPositions = this.LocalFillCoordinates;
				
				/*// Transform the list by adding the position of the transform. (This is an offset, effectively.)
				List<Vector2Int> globalFillCoordinates = localFillPositions
					.Select(v => v + this.GlobalPivotPosition - this.Badge.FillSize + Vector2Int.one)
					.ToList();*/
				
				// Transform the list by adding the position of the transform. (This is an offset, effectively.)
				List<Vector2Int> globalFillCoordinates = localFillPositions
					.Select(v => v + this.GlobalPivotPosition - this.DynamicFillSize + Vector2Int.one)
					.ToList();
				
				return globalFillCoordinates;

			}
		}
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Creates a new BadgePlacement from one that was saved out to disk.
		/// </summary>
		/// <param name="serializableBadgePlacement">The placement that contains the data that was saved out.</param>
		/// <param name="badgeCollectionSet">The BadgeCollectionSet to retrieve badges from.</param>
		public BadgePlacement(SerializableBadgePlacement serializableBadgePlacement, BadgeCollectionSet badgeCollectionSet) {
			
			// Get the badge from the collection set based on the ID that was saved.
			this.Badge = badgeCollectionSet.GetBadge(badgeID: serializableBadgePlacement.BadgeID);
			
			// Clone the BadgeTransform.
			this.BadgeTransform = serializableBadgePlacement.BadgeTransform.Clone();

		}
		/// <summary>
		/// Creates a new BadgePlacement from the badge provided.
		/// </summary>
		/// <param name="badge"></param>
		public BadgePlacement(Badge badge) {
			this.Badge = badge;
			this.BadgeTransform = new BadgeTransform();
		}
		/// <summary>
		/// Creates a new BadgePlacement with the provided positional info.
		/// </summary>
		/// <param name="badge">The badge to create a placement for.</param>
		/// <param name="xPos">The x coordinate to start the pivot at.</param>
		/// <param name="yPos">The y coordinate to start the pivot at</param>
		/// <param name="flipType">The flip type to use.</param>
		/// <param name="rotationType">The rotation type to use.</param>
		public BadgePlacement(Badge badge, int xPos, int yPos, BadgeFlipType flipType = BadgeFlipType.Normal, BadgeRotationType rotationType = BadgeRotationType.Twelve) {
			this.Badge = badge;
			this.BadgeTransform = new BadgeTransform() {
				XPos = xPos,
				YPos = yPos,
				FlipType = flipType,
				RotationType = rotationType
			};
		}
		/// <summary>
		/// Creates a new BadgePlacement from the badge and badgetransform provided.
		/// </summary>
		/// <param name="badge"></param>
		/// <param name="badgeTransform"></param>
		public BadgePlacement(Badge badge, BadgeTransform badgeTransform) {
			this.Badge = badge;
			this.BadgeTransform = badgeTransform;
		}
		/// <summary>
		/// Creates a new Badgeplacement from the information provided in the default coordinates.
		/// This is mostly used in conjunction with BadgeGridTemplates that have default badges.
		/// </summary>
		/// <param name="badgeCoordinates">The coordinates containing data relevant to the badge being placed.</param>
		/// <param name="badgeCollectionSet">The collection set which contains references to badges that might be needed.</param>
		public BadgePlacement(DefaultBadgeCoordinates badgeCoordinates, BadgeCollectionSet badgeCollectionSet) {
			// this.Badge = new Badge(badgeTemplate: badgeCoordinates.badgeTemplate);
			this.Badge = badgeCollectionSet.GetBadge(badgeID: badgeCoordinates.BadgeID);
			this.BadgeTransform = new BadgeTransform() {
				XPos = badgeCoordinates.xPos,
				YPos = badgeCoordinates.yPos
			};
		}
		#endregion

		#region SETTERS - TRANSFORM
		/// <summary>
		/// Updates the coordinates on this badge placement.
		/// </summary>
		/// <param name="targetCoordinates">The coordinates that this badge should move to.</param>
		public void UpdateCoordinates(Vector2Int targetCoordinates) {
			
			// this.BadgeTransform.UpdateCoordinates(targetCoordinates: targetCoordinates);
			
			BadgeTransform targetBadgeTransform = new BadgeTransform() {
				XPos = targetCoordinates.x,
				YPos = targetCoordinates.y,
				FlipType = this.BadgeTransform.FlipType,
				RotationType = this.BadgeTransform.RotationType
			};
			
			// Pass it down.
			this.UpdateBadgeTransform(targetBadgeTransform: targetBadgeTransform);
			
		}
		/// <summary>
		/// Updates the coordinates and orientation on this bagde placement by using the information provided.
		/// </summary>
		/// <param name="targetBadgePlacement">The badge placement that has data to clone for this one.</param>
		public void UpdateBadgeTransform(BadgePlacement targetBadgePlacement) {
			// Clone the transform from the placement provided and cascade down.
			this.UpdateBadgeTransform(
				targetBadgeTransform: targetBadgePlacement.BadgeTransform.Clone());
		}
		/// <summary>
		/// Updates the coordinates and orientation on this badge placement.
		/// </summary>
		/// <param name="targetBadgeTransform">The new coordinates/orientation for this badge placement.</param>
		public void UpdateBadgeTransform(BadgeTransform targetBadgeTransform) {
			// Just reassign the transform.
			this.BadgeTransform = targetBadgeTransform;
		}
		#endregion

		#region ROTATION/FLIPS
		/// <summary>
		/// Rotates this badge placement clockwise.
		/// </summary>
		public void RotateClockwise() {
			
			// Calculate the new rotation type.
			BadgeRotationType newRotationType = (BadgeRotationType) (((int) this.BadgeTransform.RotationType + 1) % 4);
			
			Debug.Log("New rotation type: " + newRotationType);
			
			// Assign it to the transform.
			this.BadgeTransform.RotationType = newRotationType;
			
		}
		/// <summary>
		/// Rotates this badge placement counter clockwise.
		/// </summary>
		public void RotateCounterClockwise() {
			
			// Calculate the new rotation type. Note that counterclockwise rotations might go negetive.
			int newRotationTypeIndex = (((int) this.BadgeTransform.RotationType + 3) % 4);
			// newRotationTypeIndex = Mathf.Abs(value: newRotationTypeIndex);
			BadgeRotationType newRotationType = (BadgeRotationType) newRotationTypeIndex;
			
			Debug.Log("New rotation type: " + newRotationType);
			
			// Assign it to the transform.
			this.BadgeTransform.RotationType = newRotationType;
			
		}
		/// <summary>
		/// Flips the badge placement.
		/// </summary>
		public void Flip() {
			// Calculate the new flip type.
			this.BadgeTransform.FlipType = (BadgeFlipType) (((int) this.BadgeTransform.FlipType + 1) % 4);
		}
		#endregion
		
		#region CLONING
		/// <summary>
		/// Creates a clone of this badge placement.
		/// Mostly just used for when I need to manipulate the dummy piece.
		/// </summary>
		/// <returns></returns>
		public BadgePlacement Clone() {
			// The badge itself is probably fine as is since it doesn't contain positional data,
			// but I need to clone the transform as well to make sure its a totally new instance.
			return new BadgePlacement(
				badge: this.Badge, 
				badgeTransform: this.BadgeTransform.Clone());
		}
		#endregion
		
	}
}