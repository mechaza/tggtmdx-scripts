using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.DungeonCrawler.Generation.Simple {
	
	/// <summary>
	/// Used in the CrawlerDungeonBuilder and its related classes to signal if a tile is empty or blocked.
	/// </summary>
	public enum SimpleCrawlerTileStateType {
		Empty 	= 0,
		Blocked	= 1,
	}
	
}