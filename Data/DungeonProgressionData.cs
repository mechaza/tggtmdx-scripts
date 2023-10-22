using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly {

	/// <summary>
	/// A basic data structure that keeps track of the progress a player has made in a dungeon.
	/// Gets used in the GameVariables/GameSave/Whatever.
	/// </summary>
	[System.Serializable]
	public class DungeonProgressionData {

		#region FIELDS - STATE
		/// <summary>
		/// The ID of the dungeon this data is saving.
		/// </summary>
		public DungeonIDType dungeonIDType = DungeonIDType.Castle;
		/// <summary>
		/// The number of the highest floor that was reached.
		/// </summary>
		public int highestFloorReached = 0;
		#endregion

		#region CLONING
		/// <summary>
		/// Creates a clone of this dungeon progression.
		/// </summary>
		/// <returns></returns>
		public DungeonProgressionData Clone() {
			return new DungeonProgressionData() {
				dungeonIDType = this.dungeonIDType,
				highestFloorReached = this.highestFloorReached
			};
		}
		#endregion

	}


}