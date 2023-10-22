using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// A way of describing a crawler floor's progress.
	/// </summary>
	[System.Serializable]
	public class CrawlerFloorProgress {

		#region FIELDS - STATE
		/// <summary>
		/// The floor number this object is associated with.
		/// </summary>
		public int floorNumber = -1;
		/// <summary>
		/// Has this floor been reached?
		/// </summary>
		public bool floorReached = false;
		/// <summary>
		/// Keeps track of which indexed events have been accessed.
		/// Up to 40 per floor.
		/// </summary>
		public List<bool> eventsAvailableList = new List<bool>() {
			true,true,true,true,true,true,true,true,true,true,
			true,true,true,true,true,true,true,true,true,true,
		};
		/// <summary>
		/// Keeps track of which indexed treasure has been opened.
		/// Up to 20 per floor.
		/// </summary>
		public List<bool> treasuresAvailableList = new List<bool>() {
			true, true, true, true, true, true, true, true, true, true, 
			true, true, true, true, true, true, true, true, true, true, 
		};
		/// <summary>
		/// A grid that keeps track of what mini map tiles the player has seen.
		/// </summary>
		public bool[,] visibleMiniMapTiles = new bool[64, 64];
		#endregion

		#region CHECKERS
		/// <summary>
		/// Has the event at the specified index been executed?
		/// </summary>
		/// <param name="eventIndex"></param>
		/// <returns></returns>
		public bool CheckEventAvailability(int eventIndex) {
			return this.eventsAvailableList[eventIndex];
		}
		/// <summary>
		/// Is the treasure at the specified index available?
		/// </summary>
		/// <param name="treasureIndex"></param>
		/// <returns></returns>
		public bool CheckTreasureAvailability(int treasureIndex) {
			return this.treasuresAvailableList[treasureIndex];
		}
		/// <summary>
		/// Checks whether or not a MiniMapTile at the specified coordinates has been marked as visible.
		/// </summary>
		/// <param name="tileCoordinates"></param>
		/// <returns></returns>
		public bool CheckMiniMapTileVisibility(Vector2Int tileCoordinates) {
			return this.visibleMiniMapTiles[tileCoordinates.x, tileCoordinates.y];
		}
		#endregion

		#region SETTERS
		/// <summary>
		/// Sets the availability of a treasure at the specified index to the flag passed in.
		/// </summary>
		/// <param name="treasureIndex">The index of the treasure to update.</param>
		/// <param name="isAvailable">Whether or not this treasure is available for the player to obtain.</param>
		public void SetTreasureAvailability(int treasureIndex, bool isAvailable) {
			this.treasuresAvailableList[treasureIndex] = isAvailable;
		}
		/// <summary>
		/// Sets the availability of an event at the specified index to the flag passed in.
		/// </summary>
		/// <param name="eventIndex">The index of the event to update.</param>
		/// <param name="isAvailable">Whether or not this event is available for further invokation.</param>
		public void SetEventAvailability(int eventIndex, bool isAvailable) {
			this.eventsAvailableList[eventIndex] = isAvailable;
		}
		/// <summary>
		/// Sets the visibility of a minimaptile at the specified coordinates.
		/// Helpful when reloading into a floor that was previously visited.
		/// </summary>
		/// <param name="tileCoordinates">The coordinates of the minimap tile to update the visibility for.</param>
		/// <param name="isVisible">Whether or not this tile should be set as visible.</param>
		public void SetMiniMapTileVisibility(Vector2Int tileCoordinates, bool isVisible) {
			this.visibleMiniMapTiles[tileCoordinates.x, tileCoordinates.y] = isVisible;
		}
		#endregion

		#region CLONING
		/// <summary>
		/// Returns a clone of this floor progress.
		/// </summary>
		/// <returns></returns>
		public CrawlerFloorProgress Clone() {
			CrawlerFloorProgress clone = new CrawlerFloorProgress() {
				floorNumber = this.floorNumber,
				floorReached = this.floorReached,
				eventsAvailableList = new List<bool>(this.eventsAvailableList),
				treasuresAvailableList = new List<bool>(this.treasuresAvailableList),
				visibleMiniMapTiles = this.visibleMiniMapTiles.Clone() as bool[,],
			};
			return clone;
		}
		#endregion
		
	}
}