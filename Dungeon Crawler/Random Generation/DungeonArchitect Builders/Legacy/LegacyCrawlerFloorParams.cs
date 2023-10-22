using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.DungeonCrawler.Legacy {
	
	/// <summary>
	/// Contains data used to prepare a crawler dungeon.
	/// </summary>
	public class LegacyCrawlerFloorParams {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The seed to use for th
		/// </summary>
		public int FloorNumber { get; set; } = 0;
		/// <summary>
		/// The type of tile to start on when building this floor.
		/// This is important when decending/ascending stairs.
		/// </summary>
		public CrawlerTileType StartingTileType { get; set; } = CrawlerTileType.Start;
		#endregion

	}

	
}