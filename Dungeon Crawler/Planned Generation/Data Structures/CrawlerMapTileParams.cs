using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler.Generation {
	
	/// <summary>
	/// Holds the information for one specific tile when building a crawler map.
	/// </summary>
	public class CrawlerMapTileParams {

		#region FIELDS
		/// <summary>
		/// The X Coordinate for this tile.
		/// </summary>
		public int XPos { get; set; } = -1;
		/// <summary>
		/// The X Coordinate for this tile.
		/// </summary>
		public int YPos { get; set; } = -1;
		/// <summary>
		/// The TileID for this tile.
		/// </summary>
		public CrawlerTileIDType TileIDType { get; set; } = CrawlerTileIDType.None;
		#endregion

		#region CONSTRUCTORS
		public CrawlerMapTileParams(int xPos, int yPos, CrawlerTileIDType tileIDType) {
			this.XPos = xPos;
			this.YPos = yPos;
			this.TileIDType = tileIDType;
		}
		#endregion
		
	}
	
}