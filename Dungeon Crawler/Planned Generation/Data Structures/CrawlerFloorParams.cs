using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.DungeonCrawler.Generation {
	
	/// <summary>
	/// A data structure to encapsulate the data that should be used in creating a crawler map from a template.
	/// </summary>
	public class CrawlerFloorParams {

		#region FIELDS - STATE
		/// <summary>
		/// Parameters for all of the tiles to be used.
		/// </summary>
		public List<CrawlerMapTileParams> AllMapTileParams { get; private set; } = new List<CrawlerMapTileParams>();
		/// <summary>
		/// The theme to use when building out this map.
		/// </summary>
		public CrawlerMapThemeTemplate MapThemeTemplate { get; private set; }
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Builds these map params with the tile parameters and theme specified.
		/// </summary>
		/// <param name="tileParams">The definitions for each tile that should be built out.</param>
		/// <param name="themeTemplate">The theme containing the prefabs to use in building the crawler map.</param>
		public CrawlerFloorParams(List<CrawlerMapTileParams> tileParams, CrawlerMapThemeTemplate themeTemplate) {
			this.AllMapTileParams = tileParams;
			this.MapThemeTemplate = themeTemplate;
		}
		#endregion
		
		#region GETTERS - ID'S
		/// <summary>
		/// Gets the tile ID associated with the specified x/y coordinate.
		/// </summary>
		/// <param name="xPos">The x-coordinate associated with the tile ID.</param>
		/// <param name="yPos">The y-coordinate associated with the tile ID.</param>
		/// <returns>The Tile ID Type associated with the provided x/y coordinate.</returns>
		private CrawlerTileIDType GetTileID(int xPos, int yPos) {
			// Find the tile with the specified x/y coordinates and return its tile ID type.
			return this.AllMapTileParams
				.First(tp => tp.XPos == xPos && tp.YPos == yPos)
				.TileIDType;
		}
		#endregion
		
	}
}