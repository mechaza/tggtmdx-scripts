using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Grawly.DungeonCrawler.Generation;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// The different IDs which can refer to the different dungeons available.
	/// </summary>
	public enum CrawlerDungeonIDType {
		None		= 0,
		Mall		= 1,
		College		= 2,
		ShadyPines	= 3,
	}
}