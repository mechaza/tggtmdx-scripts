using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// The tile that should allow the player to climb up the dungeon.
	/// </summary>
	public class UpStairsTile : StairsTile {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The kind of tile this is.
		/// </summary>
		public override CrawlerTileType SelfTileType => CrawlerTileType.UpStairs;
		/// <summary>
		/// The kind of tile to target when proceeding.
		/// </summary>
		public override CrawlerTileType TargetTileType => CrawlerTileType.Start;
		/// <summary>
		/// The amount to increment/decrement when proceeding on this tile.
		/// </summary>
		protected override int FloorIncrementAmount => 1;
		#endregion
		
	}
}