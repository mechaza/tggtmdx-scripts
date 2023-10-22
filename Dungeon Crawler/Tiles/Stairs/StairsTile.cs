using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using Grawly.DungeonCrawler.Generation;
using Grawly.DungeonCrawler.Legacy;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// The tile that marks the start of a dungeon.
	/// </summary>
	public abstract class StairsTile : CrawlerFloorTile, IPlayerInteractHandler, IPlayerApproachHandler, IPlayerLookAwayHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The kind of tile this is.
		/// </summary>
		public abstract CrawlerTileType SelfTileType { get; }
		/// <summary>
		/// The kind of tile to target when proceeding.
		/// </summary>
		public abstract CrawlerTileType TargetTileType { get; }
		/// <summary>
		/// The amount to increment/decrement when proceeding on this tile.
		/// </summary>
		protected abstract int FloorIncrementAmount { get; }
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The transform to use that marks the position/rotation the player should be snapped to when moving here.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private Transform targetTransform;
		#endregion

		#region PROPERTIES
		/// <summary>
		/// The transform the player should be snapped to when warping to this tile.
		/// </summary>
		public Transform TargetTransform {
			get {
				return this.targetTransform;
			}
		}
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// By default, don't worry about doing anything on approach.
		/// </summary>
		public void OnApproach(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			CrawlerActionPrompt.Instance?.Display(promptText: "Proceed");
			
		}
		/// <summary>
		/// When leaving, dismiss the prompt.
		/// </summary>
		public void OnLookAway() {
			
			// Dismiss the prompt. This should work regardless of it was shown or not.
			CrawlerActionPrompt.Instance?.Dismiss();
			
		}
		/// <summary>
		/// On interaction, do... something.
		/// </summary>
		public void OnInteract(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			// Dismiss the prompt.
			CrawlerActionPrompt.Instance?.Dismiss();
			
			// Begin the floor transition.
			CrawlerController.Instance.IncrementFloor(this.FloorIncrementAmount);
			
			// Build the next floor by passing in the increment amount and tile to warp to.
			/*LegacyCrawlerRuntimeDungeon.Instance.BuildNextFloor(
				incrementAmount: this.FloorIncrementAmount, 
				startingTileType: this.TargetTileType);*/
			
		}
		#endregion

	}

}