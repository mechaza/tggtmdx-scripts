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
	/// Invokes the event associated with the provided ID when the player steps on it.
	/// </summary>
	public class EventTile : CrawlerFloorTile, IFloorGeneratedHandler, IPlayerLandHandler {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The index associated with this particular event.
		/// Used when checking if it's available or not.
		/// </summary>
		[SerializeField]
		private int eventIndex = 0;
		#endregion

		#region INTERFACE IMPLEMENTATION : IPLAYERLANDHANDLER
		/// <summary>
		/// Upon the player landing on this tile, invoke the event associated with this ID.
		/// </summary>
		/// <param name="crawlerProgressionSet">The progression set of the current dungeon.</param>
		/// <param name="floorNumber">The floor the player is currently on.</param>
		public void OnLand(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			Debug.Log("EXECUTING EVENT OF INDEX " + this.eventIndex + " ON FLOOR " + floorNumber);
			
			// Update the variables to say that this event is no longer available.
			GameController.Instance.Variables
				.GetCrawlerProgressionSet(crawlerDungeonIDType: CrawlerController.Instance.CurrentCrawlerDungeonIDType)
				.GetFloorProgress(floorNumber: CrawlerController.Instance.CurrentFloorNumber)
				.SetEventAvailability(eventIndex: this.eventIndex, isAvailable: false);
			
			// Execute the event. This will take care of making the player wait as well as freeing them when done.
			CrawlerController.Instance.CurrentFloorTemplate
				.GetEvent(index: this.eventIndex)
				.ExecuteEvent(parentTile: this);

		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION : IFLOORGENERATEDHANDLER
		/// <summary>
		/// Upon generating the floor, check if this event can actually be used.
		/// </summary>
		/// <param name="crawlerProgressionSet">The progression set of the current dungeon.</param>
		/// <param name="floorNumber">The floor the player is currently on.</param>
		public void OnFloorGenerated(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			
			// Check whether the event is available for this floor.
			bool isAvailable = crawlerProgressionSet.CheckEventAvailability(
				floorNumber: floorNumber, 
				eventIndex: this.eventIndex);
			
			// If it is NOT available, deactivate this GameObject.
			if (isAvailable == false) {
				this.gameObject.SetActive(false);
			}
			
		}
		#endregion
		
	}
}