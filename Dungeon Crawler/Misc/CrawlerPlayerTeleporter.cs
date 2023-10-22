using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Battle.Modifiers.Afflictions;
using Grawly.UI;
using UnityEngine.Serialization;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Builders.GridFlow.Tilemap;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// Handles teleporting the player to the start of a dungeon upon generating.
	/// </summary>
	public class CrawlerPlayerTeleporter : DungeonEventListener {

		#region FIELDS - TOGGLES
		/// <summary>
		/// When teleporting, I may need to offset the y-value by a bit to make sure the camera is okay.
		/// </summary>
		[SerializeField]
		private float heightOffset = 1f;
		#endregion
		
		#region DUNGEON EVENTS
		/// <summary>
		/// Gets called when the dungeon has been generated.
		/// </summary>
		/// <param name="dungeon"></param>
		/// <param name="model"></param>
		public override void OnPostDungeonBuild(DungeonArchitect.Dungeon dungeon, DungeonModel model) {

			// If not playing, just abort. Also abort if there is no crawler player.
			if (Application.isPlaying == false) {
				return;
			} else if (CrawlerPlayer.Instance == null) {
				Debug.LogWarning("Crawler Player instance is null! Aborting teleport to spawn.");
				return;
			} 
			
			
			// Grab the marker for the spawn point from the dungeon and then grab its position.
			PropSocket spawnMarker = dungeon.Markers.First(m => m.SocketType == "SpawnPoint");
			var spawnCellPos = Matrix.GetTranslation(matrix: ref spawnMarker.Transform);
			
			// Reposition the crawler player over to its position.
			CrawlerPlayer.Instance.Teleport(
				targetPos: spawnCellPos, 
				heightOffset: this.heightOffset,
				instantaneous: true);
			
		}
		#endregion

		
	}
}