using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;
using System.Linq;

namespace Grawly.Dungeon.Generation {

	/// <summary>
	/// Contains the definition of some treasure and what range of floors it can appear on.
	/// </summary>
	public struct DungeonTreasureRange {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The range of acceptable floors this treasure can appear on.
		/// </summary>
		[SerializeField, MinMaxSlider(minValue: 1, maxValue: 20)]
		private Vector2Int floorRange;
		/// <summary>
		/// The name of the treasure. Note that this is only relevant if there is more than one BattleBehavior.
		/// </summary>
		[SerializeField, ShowIf("MoreThanOneItem")]
		private string treasureName;
		/// <summary>
		/// The BattleBehaviors that should be added to the player's inventory.
		/// </summary>
		[SerializeField]
		private List<BattleBehavior> treasureBattleBehaviors;
		#endregion

		#region PROPERTIES - TREASURE
		/// <summary>
		/// The name of the treasure. 
		/// Uses the item's name if there is only one, but will use the name defined if there is more than one.
		/// </summary>
		public string TreasureName {
			get {
				// If there is more than one item, return the specified name.
				if (this.MoreThanOneItem() == true) {
					// Note that I will want to fix it so its an empty string if its null.
					if (this.treasureName == null) {
						this.treasureName = "";
					}
					return this.treasureName;
				} else {
					// If there is only one item, return the name of the first behavior.
					return this.treasureBattleBehaviors.First().behaviorName;
				}
			}
		}
		/// <summary>
		/// The BattleBehaviors that should be added to the player's inventory.
		/// </summary>
		public List<BattleBehavior> TreasureBattleBehaviors {
			get {
				return this.treasureBattleBehaviors;
			}
		}
		#endregion

		#region PROPERTIES - FLOOR NUMBERS
		/// <summary>
		/// The lowest possible floor this treasure can appear on.
		/// </summary>
		public int LowerBound {
			get {
				return this.floorRange.x;
			}
		}
		/// <summary>
		/// The highest possible floor this treasure can appear on.
		/// </summary>
		public int UpperBound {
			get {
				return this.floorRange.y;
			}
		}
		/// <summary>
		/// Checks if the given floor number is inside the range of accepable floors.
		/// </summary>
		/// <param name="currentFloor">The current floor.</param>
		/// <returns>Whether or not the given floor is in range.</returns>
		public bool CheckInRange(int currentFloor) {
			return this.LowerBound <= currentFloor && currentFloor <= this.UpperBound;
		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// Checks if there is more than one item.
		/// </summary>
		/// <returns></returns>
		private bool MoreThanOneItem() {
			return this.treasureBattleBehaviors.Count > 1;
		}
		#endregion

	}


}