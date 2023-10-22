using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.DungeonCrawler.Legacy {
	
	/// <summary>
	/// Contains data used to prepare a crawler dungeon.
	/// </summary>
	public class LegacyCrawlerSessionParams {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The seed to use for this session.
		/// </summary>
		public int CurrentSeed { get; set; } = 0;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The current player for this session.
		/// </summary>
		public CrawlerPlayer CurrentPlayer { get; set; }
		/// <summary>
		/// The current DunGen RuntimeDungeon for this session.
		/// </summary>
		// public RuntimeDungeon CurrentRuntimeDungeon { get; set; }
		#endregion
		
	}

	
}