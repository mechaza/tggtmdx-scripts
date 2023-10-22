using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.DungeonCrawler {

	/// <summary>
	/// The different ways tile size can be defined.
	/// </summary>
	public enum CrawlerTileModeType {
		None		= 0,		// Just in case.
		Player		= 1,		// The size is defined by the player.
		Controller	= 2,		// The size is defined in the controller.
		Dungeon		= 3,		// The size is defined in the runtime dungeon.
		
	}
	
}

