using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;

namespace Grawly.Dungeon.Generation {

	/// <summary>
	/// Contains data on the acceptable range of floors a BattleTemplate can spawn.
	/// </summary>
	public struct BattleTemplateRange {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The range of acceptable floors this battle template can appear on.
		/// </summary>
		[SerializeField, MinMaxSlider(minValue: 1, maxValue: 20)]
		private Vector2Int floorRange;
		/// <summary>
		/// The BattleTemplate defined to appear on this range of floors.
		/// </summary>
		[SerializeField]
		private BattleTemplate battleTemplate;
		#endregion

		#region PROPERTIES - TEMPLATES
		/// <summary>
		/// The BattleTemplate defined to appear on this range of floors.
		/// </summary>
		public BattleTemplate BattleTemplate {
			get {
				return this.battleTemplate;
			}
		}
		#endregion

		#region PROPERTIES - FLOOR NUMBERS
		/// <summary>
		/// The lowest possible floor this template can appear on.
		/// </summary>
		public int LowerBound {
			get {
				return this.floorRange.x;
			}
		}
		/// <summary>
		/// The highest possible floor this template can appear on.
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

	}


}