using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using Grawly.UI.Legacy;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// A tile that contains a treasure that a player can open up.
	/// </summary>
	[DisallowMultipleComponent]
	public class TreasureTile : CrawlerFloorTile, IFloorGeneratedHandler, IPlayerInteractHandler {

		#region PROPERTIES - CHAT
		/// <summary>
		/// PLACEHOLDER
		/// </summary>
		private string ConfirmationText {
			get {
				string str = "";
				str += ": > Got item! ";
				str += this.TreasureTemplate.TreasureDescription;
				str += "; checker: true;";
				return str;
				// return ": > Got item!; checker: true;";
			}
		}
		#endregion
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The index associated with this particular treasure.
		/// Used when checking if it's available or not.
		/// </summary>
		[SerializeField]
		private int treasureIndex = 0;
		#endregion

		#region PROPERTIES
		/// <summary>
		/// The treasure associated with this specific tile.
		/// </summary>
		private TreasureTemplate TreasureTemplate {
			get {
				// Figure out what the current floor is.
				int currentFloor = CrawlerController.Instance.CurrentFloorNumber;
				// On this floor's template, get the treasure associated with this tile's treasure index.
				var treasureTemplate = CrawlerController.Instance.CurrentCrawlerTemplate.GetFloorTemplate(floorNumber: currentFloor).GetTreasure(index: this.treasureIndex);
				return treasureTemplate;
			}
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION : IFLOORGENERATEDHANDLER
		/// <summary>
		/// Upon generating the floor, check if this treasure can actually be used.
		/// </summary>
		/// <param name="crawlerProgressionSet">The progression set of the current dungeon.</param>
		/// <param name="floorNumber">The floor the player is currently on.</param>
		public void OnFloorGenerated(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			// Check whether the treasure is available for this floor.
			bool isAvailable = crawlerProgressionSet.CheckTreasureAvailability(
				floorNumber: floorNumber, 
				treasureIndex: this.treasureIndex);
			
			// If it is NOT available, deactivate this GameObject.
			if (isAvailable == false) {
				this.gameObject.SetActive(false);
			}
			
		}
		#endregion

		#region INTERFACE IMPLEMENTATION : IPLAYERINTERACTHANDLER
		public void OnInteract(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			// Make the player wait.
			CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Wait);	
			
			// Add the treasure items to the inventory.
			GameController.Instance.Variables.AddItemsToInventory(this.TreasureTemplate);
			
			// Update the variables to say that this treasure is no longer available.
			GameController.Instance.Variables
				.GetCrawlerProgressionSet(crawlerDungeonIDType: CrawlerController.Instance.CurrentCrawlerDungeonIDType)
				.GetFloorProgress(floorNumber: CrawlerController.Instance.CurrentFloorNumber)
				.SetTreasureAvailability(treasureIndex: this.treasureIndex, isAvailable: false);
			
			// Display a confirmation message.
			ChatControllerDX.GlobalOpen(
				scriptLine: this.ConfirmationText, 
				simpleClosedCallback: () => {
					CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Free);
					// Upon closing the confirmation, set this object as inactive.
					GameController.Instance.RunEndOfFrame(action: () => {
						this.gameObject.SetActive(false);
					});
				});
		}
		#endregion
		
	}
}