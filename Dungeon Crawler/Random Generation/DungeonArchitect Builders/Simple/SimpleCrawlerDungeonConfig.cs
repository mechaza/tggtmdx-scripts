using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI.Legacy;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Builders.GridFlow.Tilemap;

namespace Grawly.DungeonCrawler.Generation.Simple {
	
	/// <summary>
	/// The dungeon config to use in building the simple crawler dungeon.
	/// </summary>
	public class SimpleCrawlerDungeonConfig : DungeonConfig {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// ???
		/// </summary>
		public int mazeWidth = 20; 
		/// <summary>
		/// ???
		/// </summary>
		public int mazeHeight = 25;
        /// <summary>
        /// ???
        /// </summary>
        public Vector2 gridSize = new Vector2(4, 4);
		#endregion
		
		
	}
}