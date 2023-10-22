using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;
using Grawly.Battle.Equipment.Badges.Behaviors;
using Sirenix.Utilities;

namespace Grawly.Battle.Equipment.Badges  {
	
	/// <summary>
	/// Defines the properties of a badge grid that is attached to a piece of equipment.
	/// Things like size, shape, slot types, etc are defined here.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Badges/Badge Grid Template")]
	public class BadgeGridTemplate : SerializedScriptableObject {
		
		#region FIELDS - GRID
		/// <summary>
		/// The matrix describing the grid slots' shape and properties.
		/// </summary>
		[OdinSerialize, TitleGroup("Grid Shape"), TableMatrix(DrawElementMethod = "DrawColoredEnumElement")]
		private BadgeGridSlotType[,] RawSlotMatrix { get; set; } = new BadgeGridSlotType[6,6];
		#endregion

		#region PROPERTIES - GRID SHAPE
		/// <summary>
		/// The length of this grid.
		/// </summary>
		public int GridLength => this.SlotMatrix.GetLength(1);
		/// <summary>
		/// The width of this grid.
		/// </summary>
		public int GridWidth => this.SlotMatrix.GetLength(0);
		/// <summary>
		/// The matrix that defines the size and shape of this badge grid.
		/// </summary>
		private BadgeGridSlotType[,] SlotMatrix {
			get {
				return RawSlotMatrix;
			}
		}
		/// <summary>
		/// A list of coordinates of all the slots which are available for placing badges on.
		/// </summary>
		public List<Vector2Int> UsableSlotCoordinates {
			get {
				
				// Create a placeholder list.
				List<Vector2Int> availableCoordinates = new List<Vector2Int>();
				
				// Iterate through the matrix and add all the non-blocked ones to the list.
				for (int x = 0; x < this.GridWidth; x++) {
					for (int y = 0; y < this.GridLength; y++) {
						// Grab the slot type at this coordinate.
						BadgeGridSlotType slotType = this.SlotMatrix[x, y];
						// Figure out if the slot is usable.
						bool isUsable = (slotType == BadgeGridSlotType.Standard)
						                || (slotType == BadgeGridSlotType.Boost)
						                || (slotType == BadgeGridSlotType.Nerf);
						// If it is, add the coordinates to the list and keep going.
						if (isUsable == true) {
							availableCoordinates.Add(new Vector2Int(x: x, y: y));
						}
					}
				}
				
				// Return the list.
				return availableCoordinates;

			}
		}
		#endregion

		#region FIELDS - BADGES
		/// <summary>
		/// Should there be default badges that come along with this grid?
		/// </summary>
		[OdinSerialize, TitleGroup("Default Badges")]
		private bool UseDefaultBadges { get; set; } = false;
		/// <summary>
		/// If using default badges, this is a list that contains a list of coordinates for pivots as well as the badges themselves.
		/// </summary>
		[OdinSerialize, TitleGroup("Default Badges"), ShowIf("UseDefaultBadges"), ListDrawerSettings(Expanded = true)]
		private List<DefaultBadgeCoordinates> DefaultBadgeCoordinates { get; set; } = new List<DefaultBadgeCoordinates>();
		#endregion

		#region GETTERS - BADGE PLACEMENTS
		/// <summary>
		/// Generates a list of default badge placements while using the badge collection passed in.
		/// </summary>
		/// <param name="badgeCollectionSet">The badge collection that contains the badges to use in the placements.</param>
		/// <returns>A list of badge placements that also correspond to the badges in the set passed in.</returns>
		public List<BadgePlacement> GenerateDefaultBadgePlacements(BadgeCollectionSet badgeCollectionSet) {
			// Cascade down using the default coordinates contained in this template.
			return this.GenerateDefaultBadgePlacements(
				badgeCollectionSet: badgeCollectionSet,
				defaultBadgeCoordinates: this.DefaultBadgeCoordinates);
		}
		/// <summary>
		/// Generates a list of default badge placements while using the badge collection passed in.
		/// </summary>
		/// <param name="badgeCollectionSet">The badge collection that contains the badges to use in the placements.</param>
		/// <param name="defaultBadgeCoordinates">The coordinates to use by default.</param>
		/// <returns>A list of badge placements that also correspond to the badges in the set passed in.</returns>
		private List<BadgePlacement> GenerateDefaultBadgePlacements(BadgeCollectionSet badgeCollectionSet, List<DefaultBadgeCoordinates> defaultBadgeCoordinates) {
			
			// This should only be called if the collection set has been populated with badges.
			Debug.Assert(badgeCollectionSet.HasBadges);
			
			// If using default badges, create a new list of placements from the list of coordinates.
			// Placements are able to accept coordinates since they technically contain the required information.
			if (this.UseDefaultBadges == true) {
				return defaultBadgeCoordinates
					.Select(bc => new BadgePlacement(badgeCoordinates: bc, badgeCollectionSet: badgeCollectionSet))	
					.ToList();
			} else {
				// If not using default badges, just return an empty list.
				return new List<BadgePlacement>();
			}
			
		}
		#endregion
		
		#region GETTERS - GRID TOGGLES
		/// <summary>
		/// Gets the type of grid slot at the specified x/y coordinates.
		/// </summary>
		/// <param name="xPos">The x-position to check on the grid.</param>
		/// <param name="yPos">The y-position to check on the grid.</param>
		/// <returns>The type of slot associated with this coordinate on the grid.</returns>
		public BadgeGridSlotType GetSlotType(int xPos, int yPos) {
			return this.SlotMatrix[xPos, yPos];
		}
		#endregion
		
		#region ODIN HELPERS
		/// <summary>
		/// A simple way of making a new slot grid of a custom size.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		[Title("Grid Shape"), Button]
		private void CreateNewSlotGrid(int width, int height) {
			this.RawSlotMatrix = new BadgeGridSlotType[width, height];
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					this.RawSlotMatrix[i, j] = BadgeGridSlotType.Standard;
				}
			}
		}
		private static BadgeGridSlotType DrawColoredEnumElement(Rect rect, BadgeGridSlotType value) {

			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
				int typesCount = System.Enum.GetValues(typeof(BadgeGridSlotType)).Length-1;
				value = (BadgeGridSlotType) (((int) value + 1) % typesCount);
				// value = !value;
				GUI.changed = true;
				Event.current.Use();
			}

			// Figure out what color to use.
			Color colorToUse = Color.gray;
			switch (value) {
				case BadgeGridSlotType.None:
					colorToUse = Color.black;
					break;
				case BadgeGridSlotType.Standard:
					colorToUse = Color.gray;
					break;
				case BadgeGridSlotType.Boost:
					colorToUse = Color.green;
					break;
				case BadgeGridSlotType.Nerf:
					colorToUse = Color.red;
					break;
				case BadgeGridSlotType.Boundary:
					throw new NotImplementedException("I dont want to reach this. this is why i subtract 1 above.");
					colorToUse = Color.red;
					break;
			}
			
#if UNITY_EDITOR
			UnityEditor.EditorGUI.DrawRect(rect.Padding(1), colorToUse);
			// UnityEditor.EditorGUI.DrawRect(rect.Padding(1), value ? new Color(0.1f, 0.8f, 0.2f) : new Color(0, 0, 0, 0.5f));
#endif
			return value;
		}
		#endregion
		
	}
}