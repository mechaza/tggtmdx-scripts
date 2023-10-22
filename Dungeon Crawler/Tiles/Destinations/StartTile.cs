using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Grawly.UI;

namespace Grawly.DungeonCrawler {
	
	public class StartTile : CrawlerFloorTile, IFloorGeneratedHandler {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The place on the start tile in which the player should be spawned.
		/// </summary>
		[SerializeField]
		private Transform playerSpawnAnchor;
		#endregion
		
		#region INTERFACE IMPLEMENTATION : IFLOORGENERATEDHANDLER
		/// <summary>
		/// Upon generating the floor, teleport the player here.
		/// </summary>
		/// <param name="crawlerProgressionSet">The progression set of the current dungeon.</param>
		/// <param name="floorNumber">The floor the player is currently on.</param>
		public void OnFloorGenerated(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			// Update the notification bar with the new floor label.
			NotificationController.Instance?.RebuildAreaLabel(
				text: CrawlerController.Instance.CurrentCrawlerTemplate.CrawlerDungeonIDType.ToString() + " " + floorNumber + "F");
			
			// Teleport the player.
			CrawlerPlayer.Instance.Teleport(
				targetAnchor: this.playerSpawnAnchor,
				instantaneous: true);
		}
		#endregion
		
	}

}

