using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// This allows a block to appear on the minimap when stepped on.
	/// TODO: Make it so blocks can appear by default or have different colors.
	/// </summary>
	public class MiniMapTile : CrawlerFloorTile, IFloorGeneratedHandler, IPlayerLandHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should this tile be turned on when the scene loads?
		/// Good for things like rooms that should have its layout known.
		/// </summary>
		[SerializeField]
		private bool enabledOnStart = false;
		/// <summary>
		/// Should surrounding minimap tiles be enabled upon landing on this tile specifically?
		/// Helpful for also displaying walls.
		/// </summary>
		[SerializeField, PropertyTooltip("Should surrounding minimap tiles be enabled upon landing on this tile specifically? Helpful for also displaying walls.")]
		private bool turnOnSurroundingTilesOnLand = true;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObjects to enable/disable, showing it on the minimap.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private List<GameObject> miniMapObjects = new List<GameObject>();
		#endregion

		#region UNITY CALLS
		private void Start() {
			// Upon start, make sure to enable/disable it based on the toggle.
			// this.miniMapObjects.ForEach(go => go.SetActive(this.enabledOnStart));
			// this.SetVisibility(visible: this.enabledOnStart);
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION : IPLAYERLANDHANDLER
		/// <summary>
		/// Gets called when the tile is stepped on directly.
		/// </summary>
		public void OnLand(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			// Tiles should always be turned on when landed on.
			this.SetVisibility(visible: true, updateFloorProgress: true);
			
			// Also turn on surrounding tiles if instructed to do so.
			if (this.turnOnSurroundingTilesOnLand == true) {
				CrawlerPlayer.Instance
					.GetSurroundingTiles<MiniMapTile>(includeCurentPosition: false)	// This is false bc otherwise it would be reduntant.
					.ForEach(mmt => mmt.SetVisibility(visible: true, updateFloorProgress: true));
			}
			
		}
		#endregion

		#region INTERFACE IMPLEMENTATION : IFLOORGENERATEDHANDLER
		/// <summary>
		/// Upon generating the floor, check if this minimaptile should be turned on.
		/// </summary>
		/// <param name="crawlerProgressionSet">The progression set of the current dungeon.</param>
		/// <param name="floorNumber">The floor the player is currently on.</param>
		public void OnFloorGenerated(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			// Check whether the minimaptile has been seen on this floor.
			bool isVisible = crawlerProgressionSet.CheckMiniMapTileVisibility(
				floorNumber: floorNumber, 
				tileCoordinates: this.CurrentCoordinates);
			
			// If the minimaptile has been seen before, turn on its visuals.
			if (isVisible == true) {
				this.SetVisibility(
					visible: true, 
					updateFloorProgress: false);
			}
			
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Sets whether or not the minimap objects should be visible.
		/// </summary>
		/// <param name="visible">The visibility of the mini map objects.</param>
		/// <param name="updateFloorProgress">Should the current floor progresssion be updated to mark this tile as seen?</param>
		public void SetVisibility(bool visible, bool updateFloorProgress = true) {
			// Go through each of the objects that represent the visuals and activate/deactivate them.
			this.miniMapObjects.ForEach(go => go.SetActive(visible));
			// If instructed to also update the floor progress, do so.
			if (updateFloorProgress == true) {
				CrawlerController.Instance.CurrentProgressionSet
					.GetFloorProgress(CrawlerController.Instance.CurrentFloorNumber)
					.SetMiniMapTileVisibility(tileCoordinates: this.CurrentCoordinates, isVisible: visible);
			}
		}
		#endregion
		
	}

	
}