using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// Starts a battle when the player steps on it.
	/// </summary>
	public class BattleTile : CrawlerFloorTile, IPlayerLandHandler, IPlayerApproachHandler {

		#region FIELDS
		/// <summary>
		/// The battle template to use for this battle.
		/// </summary>
		[SerializeField]
		private BattleTemplate battleTemplate;
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Gets called when the tile is stepped on directly.
		/// </summary>
		public void OnLand(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			BattleController.Instance.StartBattle(battleTemplate);
		}
		/// <summary>
		/// Gets called when the tile is approached from the side and faced towards.
		/// </summary>
		public void OnApproach(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			Debug.Log("APPROACH");
		}
		#endregion

		
	}

	
}