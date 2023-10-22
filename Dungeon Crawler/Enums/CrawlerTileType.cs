using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.DungeonCrawler {

	/// <summary>
	/// Different major types of crawler tiles.
	/// Helpful for identifying tiles quickly.
	/// </summary>
	public enum CrawlerTileType {
		None		= 0,		// No special tile type.
		Start		= 1,		// The start tile.
		Goal		= 2,		// The goal tile.
		UpStairs	= 3,		// The Up Stairs Tile
		DownStairs	= 4,		// Down Stairs.
	}
	
}