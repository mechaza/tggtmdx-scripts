using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler.Generation {
	
	/// <summary>
	/// The different kinds of formats that a CrawlerMapTemplate can read from to generate its map.
	/// </summary>
	public enum CrawlerMapFormatType {
		None		= 0,
		CSV			= 1,
		JSON		= 2,
	}
	
}