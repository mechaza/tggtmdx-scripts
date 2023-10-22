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
	/// A tile that the player can interact with to teleport back to the dungeon lobby.
	/// </summary>
	public class TeleporterTile : CrawlerFloorTile, IFloorGeneratedHandler {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The index associated with this particular teleporter.
		/// </summary>
		[SerializeField]
		private int teleporterIndex = 0;
		#endregion
		
		#region INTERFACE IMPLEMENTATION : IFLOORGENERATEDHANDLER
		/// <summary>
		/// Upon generating the floor, check if this event can actually be used.
		/// </summary>
		/// <param name="crawlerProgressionSet">The progression set of the current dungeon.</param>
		/// <param name="floorNumber">The floor the player is currently on.</param>
		public void OnFloorGenerated(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			// TODO: Actually implement the teleporter functionality instead of just turning it off.
			this.gameObject.SetActive(false);
			
		}
		#endregion
		
	}
}